using Geocoding;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals
{
    public class Vendor : IEquatable<Vendor>, IComparable<Vendor>
    {
        public List<Review> Reviews = new List<Review>();

        [NotMapped] public Location Location { get; set; }

        public int[] ReviewsCount = {0, 0, 0, 0, 0, 0};

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required] public int UserID { get; set; }

        [Required] public string Title { get; set; }

        public string About { get; set; }

        [Required] public string Address { get; set; }

        [Required] public string County { get; set; }

        [Required] public string Municipality { get; set; }

        [Required] public double Latitude { get; set; }

        [Required] public double Longitude { get; set; }

        [Required] public string VendorType { get; set; }


        #region IComparable


        public int CompareTo(Vendor other)
        {
            if (other == null) return 1;

            return ReviewsCount.Average().CompareTo(other.ReviewsCount.Average());
        }

        public static bool operator >(Vendor operand1, Vendor operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }

        public static bool operator <(Vendor operand1, Vendor operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }

        public static bool operator >=(Vendor operand1, Vendor operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        public static bool operator <=(Vendor operand1, Vendor operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }

        #endregion

        #region IEquatable

        public bool Equals([AllowNull] Vendor other)
        {
            if (other == null)
            {
                return false;
            }

            return (Title == other.Title);
        }
        
        #endregion
           
        public void UpdateReviewsCount()
        {
            using var db = new AppDbContext();
            var reviews = (from r in db.Reviews
                        where r.VendorID == ID
                        select r).ToList();

            for(int i =0; i < 6; i++)
            {
                ReviewsCount[i] = 0;
            }
            
            reviews.ForEach(x => ReviewsCount[x.Stars]++);
        }

        public double Average()
        {
            UpdateReviewsCount();
            var suma = 0;
            for(var i =0; i<6; i++)
            {
                suma += ReviewsCount[i] * i;
            }

            if(ReviewsCount.Sum() == 0) 
            {
                return 0;
            }

            return (double)suma / (double)ReviewsCount.Sum();
        }

    }
}