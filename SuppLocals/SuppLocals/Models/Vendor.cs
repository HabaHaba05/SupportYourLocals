﻿using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SuppLocals
{
    public class Vendor : IEquatable<Vendor>
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

        public bool Equals([AllowNull] Vendor other)
        {
            if (other == null)
            {
                return false;
            }

            return (Title == other.Title);
        }
    }
}