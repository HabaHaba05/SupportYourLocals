using SuppLocals.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for AllVendors.xaml
    /// </summary>
    public partial class AllVendors : Window
    {

        public AllVendors(string username)
        {
            InitializeComponent();
            this.DataContext = new AllVendorsVM(username);
        }

    }
}
