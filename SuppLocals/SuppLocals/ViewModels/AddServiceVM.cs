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
        private readonly User _user;


        private string _title;
        private string _about;
        private string _address;
        private ObservableCollection<TextBlock> _suggestStack;
        private Location _myMapCenter = new Location(54.68, 25.27);


        #region Public props
      
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyPropertyChanged("Title");
            }
        }
        public string About
        {
            get => _about;
            set
            {
                _about = value;
                NotifyPropertyChanged("About");
            }
        }
        public string Address
        {
            get => _address;
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
            get => _suggestStack;
            set
            {
                _suggestStack = value;
                NotifyPropertyChanged("SuggestStack");
            }
        }

        public Location MyMapCenter
        {
            get => _myMapCenter;
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

        public string Error => null;
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
            this._user = user;

            CreateVendorBtnClick = new RelayCommand(async (x) => await CreateVendor(), o => AllFieldsValid());

            LostFocusCommand = new RelayCommand(o => { Thread.Sleep(250); SuggestStack = null; }, o => true);;
        }

        #endregion

        #region Methods

        private bool AllFieldsValid()
        {
            return (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(About) || !string.IsNullOrEmpty(Address));
        }

        private async Task CreateVendor()
        {
            var placeInfo = await MapMethods.ConvertAddressToLocation(_address);

            if (placeInfo.Count() == 0)
            {
                MessageBox.Show("We cant find an address");
                return;
            }

            await using (VendorsDbTable db = new VendorsDbTable())
            {
                var vendor = new Vendor()
                {
                    Title = _title,
                    About = _about,
                    Address = _address,
                    VendorType = SelectedVendorType,
                    Latitude = Double.Parse(placeInfo[0]),
                    Longitude = Double.Parse(placeInfo[1]),
                    Municipality = placeInfo[2],
                    County = placeInfo[3],
                    UserID = _user.ID

                };

                await using (UsersDbTable dbUser = new UsersDbTable())
                {
                    var user = dbUser.Users.SingleOrDefault(x => x.ID == this._user.ID);
                    user.VendorsCount++;
                    await dbUser.SaveChangesAsync();
                }

                await db.Vendors.AddAsync(vendor);

                await db.SaveChangesAsync();
            }

            MessageBox.Show("Created");
            Title = "";
            About = "";
            Address = "";

        }

        private async Task GetAddressSuggestions()
        {
            if (string.IsNullOrEmpty(Address))
            {
                return;
            };

            var data = await AutoComplete.GetData(Address);

            var list = new List<TextBlock>();

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
            if (block == null) throw new ArgumentNullException(nameof(block));
            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                Address = (sender as TextBlock)?.Text;
                SuggestStack = null;
            };

            block.MouseEnter += (sender, e) =>
            {
                var b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                var b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };
        }

        private async Task ChangeMapCenter()
        {
            var data = await MapMethods.ConvertAddressToLocation(Address);
            MyMapCenter = new Location(Double.Parse(data[0]), Double.Parse(data[1]));
        }

        #endregion

    }

}
