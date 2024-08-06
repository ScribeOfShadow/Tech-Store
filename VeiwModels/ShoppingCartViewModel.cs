using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Undisclosed_Shop.Models;

namespace Undisclosed_Shop.VeiwModels
{
    public class ShoppingCartViewModel
    {
        [Key]
        public List<Cart> CartItems { get; set; }
        public double? CartTotal
        {
            get; set;
        }
        public int? PaymentMethodId { get; set; }

        public BankingDetails bankingDetails { get; set; }

        public int? BankingInfo { get; set; }

    }
}