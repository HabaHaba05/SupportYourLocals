using System;
using System.Collections.Generic;
using System.Windows;

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

            // test reviews
            rewList.Add(new Review("Hui ☆☆☆☆☆", "If i could give negative stars i would.", DateTime.Now.ToString("yyyy-MM-dd")));
            rewList.Add(new Review("Pam ☆☆☆☆☆", "Everything is not working. Piece of crap.", DateTime.Now.ToString("yyyy-MM-dd")));

            rView.Items.Clear();

            foreach (Review r in rewList)
            {
                rView.Items.Add(r.Sender + "\n" + r.Text + "\n" + r.Date);
                ZeroRating.Text = (int.Parse(ZeroRating.Text) + 1).ToString();
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
                Review r = new Review(user, comment, DateTime.Now.ToString("yyyy-MM-dd"));

                // 5 Star review
                if (Rating.RatingValue.Equals(5))
                {
                    rView.Items.Add(r.Sender + "  ★★★★★\n" + r.Text + "\n" + r.Date);
                    FiveRating.Text = (int.Parse(FiveRating.Text) + 1).ToString();
                }

                // 4 Stars review
                else if (Rating.RatingValue.Equals(4))
                {
                    rView.Items.Add(r.Sender + "  ★★★★☆\n" + r.Text + "\n" + r.Date);
                    FourRating.Text = (int.Parse(FourRating.Text) + 1).ToString();
                }

                // 3 Stars review
                else if (Rating.RatingValue.Equals(3))
                {
                    rView.Items.Add(r.Sender + "  ★★★☆☆\n" + r.Text + "\n" + r.Date);
                    ThreeRating.Text = (int.Parse(ThreeRating.Text) + 1).ToString();
                }

                // 2 Stars review
                else if (Rating.RatingValue.Equals(2))
                {
                    rView.Items.Add(r.Sender + "  ★★☆☆☆\n" + r.Text + "\n" + r.Date);
                    TwoRating.Text = (int.Parse(TwoRating.Text) + 1).ToString();
                }

                // 1 Star review
                else if (Rating.RatingValue.Equals(1))
                {
                    rView.Items.Add(r.Sender + "  ★☆☆☆☆\n" + r.Text + "\n" + r.Date);
                    OneRating.Text = (int.Parse(OneRating.Text) + 1).ToString();
                }

                // 0 Stars review
                else
                {
                    rView.Items.Add(r.Sender + "  ☆☆☆☆☆\n" + r.Text + "\n" + r.Date);
                    ZeroRating.Text = (int.Parse(ZeroRating.Text) + 1).ToString();
                }

                // clearing fields after comment commited
                reviewer.Clear();
                comments.Clear();
            }
        }
    }
}
