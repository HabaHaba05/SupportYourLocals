using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuppLocals
{
    public class Review
    { 
 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        public int VendorID { get; set; }

        [Required]
        public string SenderUsername { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int Stars { get; set; }

        public string Date { get; set; }

    }
}
