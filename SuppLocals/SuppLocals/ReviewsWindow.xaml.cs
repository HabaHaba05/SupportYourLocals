using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

            rewList.Add(new Review("Hui", "If i could give zero stars i would.", DateTime.Now.ToString("yyyy-MM-dd")));
            rewList.Add(new Review("Pam", "Everything is not working. Piece of crap.", DateTime.Now.ToString("yyyy-MM-dd")));

            rView.Items.Clear();

            foreach (Review r in rewList)
            {
                rView.Items.Add(r.Sender + "\n" + r.Text + "\n Date published: " + r.Date);
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
                Review r = new Review(user, comment, DateTime.Now.ToString("yyyy-MM-dd"));

                rView.Items.Add(r.Sender + "\n" + r.Text + "\n Date published: " + r.Date);

                // clearing fields after comment commited
                reviewer.Clear();
                comments.Clear();
            }
        }

        
    }
}
