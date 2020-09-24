using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        public ReviewsWindow()
        {
            List<Review> rewList = new List<Review>();

            // by default
            InitializeComponent();

            // Create the image element.
            Image img = new Image();
            img.Margin = new Thickness(5);

            // Create source.
            BitmapImage bi = new BitmapImage();
            // BitmapImage.UriSource must be in a BeginInit/EndInit block.
            bi.BeginInit();
            bi.UriSource = new Uri(@"Assets/usericon.jpg", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            // Set the image source.
            img.Source = bi;

            
           
            rewList.Add(new Review(img, "Hui", "If i could give zero stars i would."));
            rewList.Add(new Review(img, "Hui", "If i could give zero stars i would."));

            rView.Items.Clear();

            foreach (Review r in rewList)
            {
                rView.Items.Add(r.Image + " " + r.Sender + "\n" + r.Text);
            }

        }

        // Adding user comment when button pressed
        private void confirmClicked(object sender, RoutedEventArgs e)
        {
            var user = reviewer.Text;
            var comment = comments.Text;

            if (user.Length.Equals(0) || comment.Length.Equals(0))
            {
                MessageBox.Show("You cannot post an empty review!");
                return;
            }
            else
            {
                Image img = new Image();
                Review r = new Review(img, user, comment);

                rView.Items.Add(r.Image + " " + r.Sender + "\n" + r.Text);

                // clearing fields after comment commited
                reviewer.Clear();
                comments.Clear();
            }
        }
    }
}
