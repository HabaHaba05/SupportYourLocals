using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals.ViewModels
{
    public class AddServiceVM : BaseViewModel, IDataErrorInfo
    {
        private readonly User user;

        #region Private props

        private string _title;
        private string _about;
        private string _address;
        private ObservableCollection<TextBlock> _suggestStack;
        private Location _myMapCenter = new Location(54.68, 25.27);


        #endregion

        #region Public props
      
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }
        public string About
        {
            get { return _about; }
            set
            {
                _about = value;
                NotifyPropertyChanged("About");
            }
        }
        public string Address
        {
            get { return _address; }
            set
            {
                if(_address == value)
                {
                    SuggestStack = null;
                    return;
                }
                _address = value;
                GetAddressSuggestions();
                ChangeMapCenter();
                NotifyPropertyChanged("Address");
            }
        }
        public string SelectedVendorType { private get; set; }

        public ObservableCollection<TextBlock> SuggestStack
        {
            get
            {
                return _suggestStack;
            }
            set
            {
                _suggestStack = value;
                NotifyPropertyChanged("SuggestStack");
            }
        }

        public Location MyMapCenter
        {
            get { return _myMapCenter; }
            set
            {
                _myMapCenter = value;
                NotifyPropertyChanged("MyMapCenter");
            }
        }

        #endregion

        #region Commands

        public RelayCommand CreateVendorBtnClick{get; private set; }
        public RelayCommand LostFocusCommand { get; private set; }

        #endregion

        #region Data validation

        public string Error { get { return null; } }
        public Dictionary<string, string> ErrorCollection { get; private set; } = new Dictionary<string, string>();
        public string this[string name]
        {
            get
            {
                string result = null;

                switch (name)
                {
                    case "Title":
                        if (string.IsNullOrWhiteSpace(Title))
                            result = "Title cannot be empty";
                        break;
                    case "About":
                        if (string.IsNullOrWhiteSpace(About))
                            result = "Description cannot be empty";
                        break;
                    case "Address":
                        if (string.IsNullOrWhiteSpace(Address))
                            result = "Address cannot be empty";
                        break;
                }

                if (ErrorCollection.ContainsKey(name))
                    ErrorCollection[name] = result;
                else if (result != null)
                    ErrorCollection.Add(name, result);

                NotifyPropertyChanged("ErrorCollection");
                return result;
            }
        }

        #endregion

        #region Constructor

        public AddServiceVM(User user)
        {
            this.user = user;

            CreateVendorBtnClick = new RelayCommand(async (x) => await CreateVendor(), o => AllFieldsValid());

            LostFocusCommand = new RelayCommand(o => { Thread.Sleep(250); SuggestStack = null; }, o => true);;
        }

        #endregion

        #region Methods

        private bool AllFieldsValid()
        {
            return (!String.IsNullOrEmpty(Title) || !String.IsNullOrEmpty(About) || !String.IsNullOrEmpty(Address));
        }

        private async Task CreateVendor()
        {
            Location location = await MapMethods.ConvertAddressToLocation(_address);
            if (location.Longitude == 0)
            {
                MessageBox.Show("We cant find an address");
                return;
            }

            using (VendorsDbTable db = new VendorsDbTable())
            {
                Vendor vendor = new Vendor()
                {
                    Title = _title,
                    About = _about,
                    Address = _address,
                    VendorType = SelectedVendorType,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    UserID = user.ID
                };

                using (UsersDbTable dbUser = new UsersDbTable())
                {
                    var user = dbUser.Users.SingleOrDefault(x => x.ID == this.user.ID);
                    user.VendorsCount++;
                    dbUser.SaveChanges();
                }

                db.Vendors.Add(vendor);

                db.SaveChanges();
            }

            MessageBox.Show("Created");
            Title = "";
            About = "";
            Address = "";

        }

        private async Task GetAddressSuggestions()
        {
            if (String.IsNullOrEmpty(Address))
            {
                return;
            };

            var data = await AutoComplete.GetData(Address);

            List<TextBlock> list = new List<TextBlock>();

            foreach (var obj in data)
            {   if (obj != Address)
                {
                    TextBlock block = new TextBlock
                    {
                        Text = obj,
                        Margin = new Thickness(2, 3, 2, 3),
                        Cursor = Cursors.Hand
                    };

                    AddEvents(block);
                    list.Add(block);
                }
            }

            SuggestStack = new ObservableCollection<TextBlock>(list);
        }
        
        private void AddEvents(TextBlock block)
        {
            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                Address = (sender as TextBlock).Text;
                SuggestStack = null;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };
        }

        private async Task ChangeMapCenter()
        {
            MyMapCenter = await MapMethods.ConvertAddressToLocation(Address);
        }

        #endregion

    }

}
