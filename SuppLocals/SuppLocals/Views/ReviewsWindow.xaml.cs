using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Windows.Foundation;

namespace SuppLocals
{
    /// <summary>
    ///     Interaction logic for ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        private readonly List<string> _stars = new List<string> {"☆☆☆☆☆", "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★"};
        private readonly User _user;
        private readonly Vendor _vendor;

        private double _average;

        private List<Review> _reviews;

        public ReviewsWindow(Vendor vendor, User activeUser)
        {
            // by default
            InitializeComponent();
            DataContext = this;
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


        public Visibility CanComment { get; set; }

        // Adding user comment when button pressed
        private void ConfirmClicked(object sender, RoutedEventArgs e)
        {
            var comment = comments.Text;
            ConfirmError.Visibility = Visibility.Hidden;

            var i = RView.Items.Count;

            if (string.IsNullOrEmpty(comment))
            {
                ConfirmError.Visibility = Visibility.Visible;
                return;
            }

            using (var db = new ReviewsDbTable())
            {
                var r = new Review
                {
                    VendorID = _vendor.ID,
                    CommentID = i,
                    SenderUsername = _user.Username,
                    Text = comment,
                    Stars = Rating.RatingValue,
                    Date = DateTime.Now.ToString("yyyy-MM-dd"),
                    Reply = ""
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
            RView.Items.Clear();

            var sum = 0;
            var number = 0;

            using var db = new ReviewsDbTable();
            _reviews = db.Reviews.Where(x => x.VendorID == _vendor.ID).ToList();

            for (var i = 0; i < 6; i++)
            {
                _vendor.ReviewsCount[i] = 0;
            }

            foreach (var review in _reviews)
            {
                _vendor.ReviewsCount[review.Stars]++;
                sum += review.Stars;
                number += 1;

                if (sum != 0 || number != 0)
                {
                    _average = (double) sum / number;
                }
                else
                {
                    _average = 0;
                }

                RView.Items.Add(review.SenderUsername + " " + _stars[review.Stars] + "\n" + review.Text + "\n" + review.Date);
            }

            UpdateRatingCounts();
        }


        private void PostComment(object sender, RoutedEventArgs e)
        {
            var btn = (Button) sender;
            var replyBox = ((Grid) btn.Parent).FindName("ReplyTextBox") as TextBox;

            var comment = ((Grid) btn.Parent).FindName("UserComment") as TextBlock;
          
            var replyGrid = ((Grid) btn.Parent).FindName("ReplyGrid") as Grid;
            var commentGrid = ((Grid) btn.Parent).FindName("CommentGrid") as Border;


            var fullComment = _vendor.Title + "\n" + "\n" + replyBox.Text + "\n" + DateTime.Now.ToString("yyyy-MM-dd");

            replyGrid.Visibility = Visibility.Collapsed;
            commentGrid.Visibility = Visibility.Visible;

           
            comment.Text = fullComment;

            // Saving the reply for the comment
            // only works if item IS SELECTED
            var index = RView.Items.IndexOf(RView.SelectedItem);
            
            using (var dbUser = new ReviewsDbTable())
            {
                var user = dbUser.Reviews.SingleOrDefault(x => x.CommentID == index);
                user.Reply = fullComment;
                dbUser.SaveChanges();
            }  
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            var control = sender as StackPanel;
            var commentGrid = control.FindName("CommentGrid") as Border;

            var comment = control.FindName("UserComment") as TextBlock;

            if (commentGrid != null)
                MessageBox.Show("YAY! NOT NULL");

              
        }


            //var comment = control.FindName("UserComment") as TextBlock;
            //var replyGrid = control.FindName("ReplyGrid") as Grid;



        

    }
}