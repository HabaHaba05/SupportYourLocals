using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        private readonly List<string> STARS = new List<string> { "☆☆☆☆☆", "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★" };
        private readonly Vendor _vendor;
        private readonly User _user;

        private List<Review> reviews;

        private double _average;

        public Visibility CanComment { get; set; }

        public ReviewsWindow(Vendor vendor, User activeUser)
        {

            // by default
            InitializeComponent();
            this.DataContext = this;
            _vendor = vendor;
            _user = activeUser;


            if (_vendor.UserID == _user.ID)
            {
                CanComment = Visibility.Hidden;

            }
            else
            {
                CanComment = Visibility.Visible;
            }

            PopulateData();
        }


        // Adding user comment when button pressed
        private void ConfirmClicked(object sender, RoutedEventArgs e)
        {
            var comment = comments.Text;
            ConfirmError.Visibility = Visibility.Hidden;

            if (string.IsNullOrEmpty(comment))
            {
                ConfirmError.Visibility = Visibility.Visible;
                return;
            }
            using (ReviewsDbTable db = new ReviewsDbTable())
            {
                Review r = new Review()
                {
                    VendorID = _vendor.ID,
                    SenderUsername = _user.Username,
                    Text = comment,
                    Stars = Rating.RatingValue,
                    Date = DateTime.Now.ToString("yyyy-MM-dd")

                };

                db.Reviews.Add(r);
                db.SaveChanges();
            }

            PopulateData();

            comments.Clear();

        }

        private void UpdateRatingCounts()
        {
            ZeroRating.Text = _vendor.ReviewsCount[0].ToString();
            OneRating.Text = _vendor.ReviewsCount[1].ToString();
            TwoRating.Text = _vendor.ReviewsCount[2].ToString();
            ThreeRating.Text = _vendor.ReviewsCount[3].ToString();
            FourRating.Text = _vendor.ReviewsCount[4].ToString();
            FiveRating.Text = _vendor.ReviewsCount[5].ToString();

            Average.Text = _average.ToString("0.0");
        }

        private void PopulateData()
        {
            rView.Items.Clear();

            var sum = 0;
            var number = 0;


            using ReviewsDbTable db = new ReviewsDbTable();
            reviews = db.Reviews.Where(x => x.VendorID == _vendor.ID).ToList();

            for (int i = 0; i < 6; i++)
            {
                _vendor.ReviewsCount[i] = 0;
            }

            foreach (var review in reviews)
            {
                _vendor.ReviewsCount[review.Stars]++;
                sum += review.Stars;
                number += 1;

                if (sum != 0 || number != 0)
                {
                    _average = ((double)sum / number);
                }
                else
                {
                    _average = 0;
                }

                rView.Items.Add(review.SenderUsername + " " + STARS[review.Stars] + "\n" + review.Text + "\n" + review.Date);
            }

            UpdateRatingCounts();
        }

        public Visibility ReplyVisibility { get; set; }



        private void ReplyClicked(object sender, RoutedEventArgs e)
        {
            ListView i = rView;
          
        }


    }
}