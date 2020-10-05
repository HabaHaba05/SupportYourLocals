using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        private readonly List<string> STARS = new List<string>{"☆☆☆☆☆", "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★"};
        private readonly Vendor _service;
       
        public ReviewsWindow(Vendor service)
        {
            
            // by default
            InitializeComponent();
            this.DataContext = this;
            _service = service;

            rView.Items.Clear();
        }

        
        // Adding user comment when button pressed
        private void ConfirmClicked(object sender, RoutedEventArgs e)
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
                Review r = new Review(user + "  " + STARS[Rating.RatingValue], comment, DateTime.Now.ToString("yyyy-MM-dd"));
                _service.reviewsCount[Rating.RatingValue]++;
                _service.reviews.Add(r);

                rView.Items.Add(r.Sender + "\n" + r.Text + "\n" + r.Date);
                UpdateRatingCounts();

                // clearing fields after comment commited
                reviewer.Clear();
                comments.Clear();

                Rating.UncheckAll(sender);
            }
        }

        private void UpdateRatingCounts()
        {
            ZeroRating.Text = _service.reviewsCount[0].ToString();
            OneRating.Text = _service.reviewsCount[1].ToString();
            TwoRating.Text = _service.reviewsCount[2].ToString();
            ThreeRating.Text = _service.reviewsCount[3].ToString();
            FourRating.Text = _service.reviewsCount[4].ToString();
            FiveRating.Text = _service.reviewsCount[5].ToString();
        }
    }
}