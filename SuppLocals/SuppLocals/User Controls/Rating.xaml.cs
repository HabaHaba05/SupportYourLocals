using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for Rating.xaml
    /// </summary>
    public partial class Rating : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Rating", typeof(int), typeof(Rating),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(RatingChanged)));

        public Rating()
        {
            InitializeComponent();
        }

        public int Value
        {
            get
            {
                return (int)GetValue(ValueProperty);
            }
            set
            {
                if (value < 0)
                {
                    SetValue(ValueProperty, 0);
                }
                else if (value > 5)
                {
                    SetValue(ValueProperty, 5);
                }
                else
                {
                    SetValue(ValueProperty, value);
                }
            }
        }

        private static void RatingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Rating item = sender as Rating;
            int newval = (int)e.NewValue;
            UIElementCollection childs = ((Grid)(item.Content)).Children;

            ToggleButton button;

            for (int i = 0; i < newval; i++)
            {
                button = childs[i] as ToggleButton;
                if (button != null)
                    button.IsChecked = true;
            }

            for (int i = newval; i < childs.Count; i++)
            {
                button = childs[i] as ToggleButton;
                if (button != null)
                    button.IsChecked = false;
            }
        }

        private void ClickEventHandler(object sender, RoutedEventArgs args)
        {
            ToggleButton button = sender as ToggleButton;
            int newvalue = int.Parse(button.Tag.ToString());

            // value of rating (ex. 3)
            Value = newvalue;
            RatingValue = Value;
        }

        public static int RatingValue { get; set; }
    }
}