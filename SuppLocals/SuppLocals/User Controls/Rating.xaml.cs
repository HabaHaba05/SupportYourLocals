using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SuppLocals
{
    /// <summary>
    ///     Interaction logic for Rating.xaml
    /// </summary>
    public partial class Rating : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Rating", typeof(int),
            typeof(Rating),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, RatingChanged));

        public Rating()
        {
            InitializeComponent();
        }

        public int Value
        {
            get => (int) GetValue(ValueProperty);
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

        public static int RatingValue { get; set; }

        private static void RatingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = sender as Rating;
            var newValue = (int) e.NewValue;
            var children = ((Grid) item.Content).Children;

            ToggleButton button;

            for (var i = 0; i < newValue; i++)
            {
                button = children[i] as ToggleButton;
                if (button != null)
                    button.IsChecked = true;
            }

            for (var i = newValue; i < children.Count; i++)
            {
                button = children[i] as ToggleButton;
                if (button != null)
                    button.IsChecked = false;
            }
        }

        private void ClickEventHandler(object sender, RoutedEventArgs args)
        {
            var button = sender as ToggleButton;
            var newValue = int.Parse(button.Tag.ToString());

            // value of rating (ex. 3)
            Value = newValue;
            RatingValue = Value;
        }
    }
}