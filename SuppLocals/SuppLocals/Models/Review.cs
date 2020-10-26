using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SuppLocals
{
    public class Review : IEquatable<Review>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required] public int VendorID { get; set; }

        [Required] public int CommentID { get; set; }

        [Required] public string SenderUsername { get; set; }

        [Required] public string Text { get; set; }

        [Required] public int Stars { get; set; }

        [Required] public string Date { get; set; }

        public string Reply { get; set; }

        public bool Equals([AllowNull] Review other)
        {
            if (other == null)
            {
                return false;
            }

            return (VendorID == other.VendorID && SenderUsername == other.SenderUsername && Text == other.Text);
        }
    }
}