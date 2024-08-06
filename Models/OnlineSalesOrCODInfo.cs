using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Undisclosed_Shop.Models
{
    public class OnlineSalesOrCODInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OnlineSalesId { get; set; }

        public int? SalesId { get; set; }
        [ForeignKey("SalesId")]
        public Sale Sale { get; set; }

        public decimal? Amount { get; set; }
        public string CurrencyCode { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? CreatedDate { get; set; }

        public int? BankingDetailsId { get; set; }
        [ForeignKey("BankingDetailsId")]
        public BankingDetails BankingDetails { get; set; }





    }
}