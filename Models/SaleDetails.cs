using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Undisclosed_Shop.Models
{
    public class SaleDetail
    {
        [Key]
        public int SaleDetailId { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double ProductPrice { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products Products { get; set; }

        [ForeignKey("SaleId")]
        public virtual Sale Sale { get; set; }
    }
}
