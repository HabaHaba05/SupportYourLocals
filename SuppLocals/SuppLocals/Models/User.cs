using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Maps.MapControl.WPF;

namespace SuppLocals
{
    public class User  
    {
        public Location Location;


        #region Database columns

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string HashedPsw { get; set; }

        public int VendorsCount { get; set; }

        #endregion
    }

}
