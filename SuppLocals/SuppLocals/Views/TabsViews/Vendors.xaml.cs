using SuppLocals.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for Vendors.xaml
    /// </summary>
    public partial class Vendors : UserControl
    {

        public Vendors()
        {
            InitializeComponent();
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dynamic selectedItem = ListView2.SelectedItem;
            var username = selectedItem.Username;

            AllVendors vendors = new AllVendors(username);
            vendors.Show();     
        }
    }
}
