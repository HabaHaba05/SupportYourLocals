using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        private readonly List<string> STARS = new List<string>{"☆☆☆☆☆", "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★"};
        private readonly Vendor _vendor;
        private readonly User _user;

        private List<Review> reviews; 

        public Visibility CanComment { get; set; }

        public ReviewsWindow(Vendor vendor , User activeUser)
        {
            
            // by default
            InitializeComponent();
            this.DataContext = this;
            _vendor = vendor;
            _user = activeUser;
            
            if(_vendor.UserID == _user.ID)
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
            if (String.IsNullOrEmpty(comment))
            {
                MessageBox.Show("Comment can't be empty");
                return;
            }
            using(ReviewsDbTable db = new ReviewsDbTable())
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
        }

        private void PopulateData()
        {
            rView.Items.Clear();

            using ReviewsDbTable db = new ReviewsDbTable();
            reviews = db.Reviews.Where(x => x.VendorID == _vendor.ID).ToList();

            foreach (var review in reviews)
            {
                _vendor.ReviewsCount[review.Stars]++;
                rView.Items.Add(review.SenderUsername + " " + STARS[review.Stars] + "\n" + review.Text + "\n" + review.Date);
            }

            UpdateRatingCounts();
        }
    }
}