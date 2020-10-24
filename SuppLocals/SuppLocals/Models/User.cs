using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Maps.MapControl.WPF;

namespace SuppLocals
{
    public class User : IEquatable<User>
    {
        public Location Location;


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required] public string Username { get; set; }

        [Required] public string HashedPsw { get; set; }

        [Required] public string Email { get; set; }

        public byte[] Image { get; set; }

        public int VendorsCount { get; set; }

        public bool Equals([AllowNull] User other)
        {
            if (other == null)
            {
                return false;
            }

            return (Username == other.Username);
        }

        public BitmapImage GetProfileImage()
        {
            if (Image == null)
            {
                return new BitmapImage(new Uri("pack://application:,,,/Assets/profile.png"));
            }

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(Image);
            bi.EndInit();

            return bi;
        }
    }
}