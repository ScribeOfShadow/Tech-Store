using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Undisclosed_Shop.Models
{
    public class Delivery
    {
        [Key]
        public int DeliveryId { get; set; }

        public int SaleId { get; set; }

        public double DeliveryFee { get; set; }

        public string CurrentLocation { get; set; }

        public string DeliveryDate { get; set; }

        public bool isDelivered { get; set; }

        public bool? IsPickedUpForReturn { get; set; }
        public bool? IsForReturn { get; set; }
        public DateTime? PickUpReturnDate { get; set; }
        public virtual Sale sale { get; set; }
    }
}
