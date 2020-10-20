using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace SuppLocals
{
    /// <summary>
    ///     Interaction logic for ReviewsWindow.xaml
    /// </summary>
    public partial class ReviewsWindow : Window
    {
        private readonly List<string> _stars = new List<string> { "☆☆☆☆☆", "★☆☆☆☆", "★★☆☆☆", "★★★☆☆", "★★★★☆", "★★★★★" };
        private readonly User _user;
        private readonly Vendor _vendor;

        private decimal _average;

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

        private class Comment
        {
            public string Text { get; set; }
            public int Likes { get; set; }
            public int Dislikes { get; set; } 
        }


        public Visibility CanComment { get; set; }

        // Adding user comment when button pressed
        private void ConfirmClicked(object sender, RoutedEventArgs e)
        {
            var comment = comments.Text;
            ConfirmError.Visibility = Visibility.Hidden;
            
            // counter for comments to replies
            var i = RView.Items.Count;

            if (string.IsNullOrWhiteSpace(comment))
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
                    Likes = 0,
                    Dislikes = 0
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
                    _average = (decimal) sum / number;
                }
                else
                {
                    _average = 0;
                }

                var Comment = review.SenderUsername + " " + _stars[review.Stars] + "\n" + review.Text + "\n" + review.Date;

                RView.Items.Add(new Comment { Text = Comment, Likes = review.Likes, Dislikes = review.Dislikes});
            }

            UpdateRatingCounts();
        }
       

        private void LikeClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;

            var like = ((StackPanel)button.Parent).FindName("LikeCount") as TextBlock;
            like.FontWeight = FontWeights.Bold;
            //button.Content = new SolidColorBrush(Colors.Blue);

            LikesOrDislikes(button, like, "likes");   
        }

        private void DislikeClicked(object sender, RoutedEventArgs e)
        {  
            var button = sender as RadioButton;

            var dislike = ((StackPanel)button.Parent).FindName("DislikeCount") as TextBlock;
            dislike.FontWeight = FontWeights.Bold;

            LikesOrDislikes(button, dislike, "dislikes");
        }


        private void LikesOrDislikes(RadioButton button, TextBlock t, string name)
        {
            var item = button.DataContext;
            var index = RView.Items.IndexOf(item);

            using (var dbUser = new ReviewsDbTable())
            {
                var user = dbUser.Reviews.SingleOrDefault(x => x.CommentID == index);

                if(name == "dislikes")
                {
                    user.Dislikes++;
                    t.Text = user.Dislikes.ToString();
                }
                else 
                {
                    user.Likes++;
                    t.Text = user.Likes.ToString();
                }
                dbUser.SaveChanges();
            }
        }
    }
}