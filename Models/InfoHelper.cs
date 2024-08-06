using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Undisclosed_Shop.Models;

namespace Undisclosed_Shop.Models
{
    public class InfoHelper
    {

        public static IEnumerable<SelectListItem> GetUserBankDetailsSave()
        {
            using (ApplicationDbContext core = new ApplicationDbContext())
            {
                var GetUser = HttpContext.Current.User;

                var Findis = GetUser.Identity.GetUserId();
                core.Database.CommandTimeout = 100;
                var list = core.BankingDetails;
                List<SelectListItem> caseTypes = new List<SelectListItem>
                {
                     new SelectListItem { Text = "-Select Card Type-", Value = "-1".ToString() }
                };

                var types = list.Where(x => x.ApplicationUserId == Findis).Include(x => x.Bank).Select(x => new SelectListItem
                {
                    Text = x.Bank.BankName + " " + x.CardNo,
                    Value = x.Id.ToString(),
                }).ToList() ?? null;

                if (caseTypes.Count() > 0)
                {
                    caseTypes.AddRange(types);
                }

                return caseTypes;
            }
        }


        public static IEnumerable<SelectListItem> GetBanks()
        {
            using (ApplicationDbContext core = new ApplicationDbContext())
            {



                core.Database.CommandTimeout = 100;
                var list = core.BankInfos;
                List<SelectListItem> caseTypes = new List<SelectListItem>
                {
                     new SelectListItem { Text = "-Select Payment Method -", Value = "-1".ToString() }
                };

                var types = list.Select(x => new SelectListItem
                {
                    Text = x.BankName + "| " + x.BankCode + "|  " + x.BranchCode,
                    Value = x.Id.ToString(),
                }).ToList();

                caseTypes.AddRange(types);
                return caseTypes;
            }
        }


        public static IEnumerable<SelectListItem> GetPaymentMethods()
        {
            using (ApplicationDbContext core = new ApplicationDbContext())
            {



                core.Database.CommandTimeout = 100;
                var list = core.PaymentMethods;
                List<SelectListItem> caseTypes = new List<SelectListItem>
                {
                     new SelectListItem { Text = "-Select Bank -", Value = "-1".ToString() }
                };

                var types = list.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }).ToList();

                caseTypes.AddRange(types);
                return caseTypes;
            }
        }



    }
}