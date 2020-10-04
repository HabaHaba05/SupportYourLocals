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
            {/*
                Review r = new Review(user + "  " + STARS[Rating.RatingValue], comment, DateTime.Now.ToString("yyyy-MM-dd"));
                _service.ReviewsCount[Rating.RatingValue]++;
                _service.Reviews.Add(r);

                rView.Items.Add(r.Sender + "\n" + r.Text + "\n" + r.Date);*/
                UpdateRatingCounts();

                // clearing fields after comment commited
                reviewer.Clear();
                comments.Clear();
            }
        }

        private void UpdateRatingCounts()
        {
            ZeroRating.Text = _service.ReviewsCount[0].ToString();
            OneRating.Text = _service.ReviewsCount[1].ToString();
            TwoRating.Text = _service.ReviewsCount[2].ToString();
            ThreeRating.Text = _service.ReviewsCount[3].ToString();
            FourRating.Text = _service.ReviewsCount[4].ToString();
            FiveRating.Text = _service.ReviewsCount[5].ToString();
        }
    }
}