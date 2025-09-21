using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment11._1._1
{
    public class Book
    {
        [Key]
        [MaxLength(13)]            // ISBN-13 typical length
        public string ISBN { get; set; } = "";

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(60)]
        public string Author { get; set; } = "";

        [MaxLength(500)]
        public string Description { get; set; } = "";
    }
}
