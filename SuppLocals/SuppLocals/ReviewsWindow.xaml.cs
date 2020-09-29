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
        private Service _service;
       
        public ReviewsWindow(Service service)
        {
            
            // by default
            InitializeComponent();
            this.DataContext = this;
            _service = service;

            rView.Items.Clear();

            foreach (Review r in service.reviews)
            {
                rView.Items.Add(r.Sender + "\n" + r.Text + "\n" + r.Date);
            }

            updateRatingCounts();
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
                Review r = new Review(user + STARS[Rating.RatingValue], comment, DateTime.Now.ToString("yyyy-MM-dd"));
                _service.reviewsCount[Rating.RatingValue]++;
                _service.reviews.Add(r);

                rView.Items.Add(r.Sender + "\n" + r.Text + "\n" + r.Date);
                updateRatingCounts();

                // clearing fields after comment commited
                reviewer.Clear();
                comments.Clear();
            }
        }

        private void updateRatingCounts()
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
