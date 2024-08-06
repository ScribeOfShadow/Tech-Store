using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Undisclosed_Shop.Models
{
    public class Products
    {

        [Key]
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public int ProductStock { get; set; }

        [Required]
        public double ProductPrice { get; set; }

        [Required]
        public string ProductDescription { get; set; }

        [Required]
        public string ProductCategory { get; set; }

        public string ProductImage { get; set; }


    }
}