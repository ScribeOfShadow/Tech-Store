//using Undisclosed_Shop.Helpers;
using Undisclosed_Shop.Models;
using Undisclosed_Shop.VeiwModels;
using Microsoft.AspNet.Identity.Owin;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Undisclosed_Shop.Controllers
{
    public class CheckoutController : Controller
    {
        ApplicationDbContext dB = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        string[] cmt = { "Excellent", "Good experience", "Satisfactory service", "Amazing Book" };
        int[] rt = { 5, 4, 3, 2, 1 };
        public CheckoutController()
        {
        }

        public CheckoutController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Checkout
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Payment()
        {
            var viewModel = new ShoppingCartViewModel();
            try
            {

                var cart = ShoppingCart.GetCart(this.HttpContext);
                if (cart.GetCartItems().Count() > 0)
                {
                    viewModel = new ShoppingCartViewModel()
                    {
                        CartItems = cart.GetCartItems(),
                        CartTotal = cart.GetTotal()
                    };

                    ViewBag.TotalCost = cart.GetTotal() + cart.GetDeliveryFee();
                    ViewBag.ShippingCost = cart.GetDeliveryFee();
                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("OurProducts", "Products");
                }
                // Set up our ViewModel

            }
            catch (Exception)
            {
                return RedirectToAction("OurProducts", "Products");
                //   throw;
            }

            // Return the view
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Payment([Bind(Include = "Name,City,State,PostalCode,Email,Id,Address")] ApplicationUser applicationUser, ShoppingCartViewModel shpVM) //The Method That Handles All Transactions
        {
            var FindPaymentMethods = dB.PaymentMethods;

            //Variables Required For Tables
            double deliveryFee = 0;
            int numProducts = 0;

            //Get Current Cart
            var cart = ShoppingCart.GetCart(this.HttpContext);

            //Create New Sale
            Sale sale = new Sale();
            sale.SaleDate = DateTime.Now.ToString();


            numProducts = cart.GetCount();

            if (numProducts == 1)
            {
                deliveryFee = 60;
            }
            else if (numProducts <= 3)
            {
                deliveryFee = 80;
            }
            else
            {
                deliveryFee = 100;
            }



            //Customer Details
            var getUsers = dB.Users.Where(v => v.Id != null).ToList();

            //Customer Details
            foreach (var user in getUsers)
            {
                sale.Email = User.Identity.Name;
                sale.Name = user.Name;
                sale.Address = user.Address;
                sale.City = user.City;
                sale.State = user.State;
                sale.PostalCode = user.PostalCode;
                sale.Country = user.Country;
                sale.PhoneNumber = user.PhoneNumber;
            }

            //var Discount = dB.UsedCouponLists.Where(s => s.userId == sale.Email);

            //  //Sale Total
            //  if (Discount.Any())
            //  {
            //      foreach (var discount in Discount)
            //      {
            //          if (discount.CouponDiscountAmount != null)
            //          {
            //              double? SaleFinal = cart.GetTotal() - (cart.GetTotal() * discount.CouponDiscountAmount) / 100;
            //              sale.SaleTotal = SaleFinal + deliveryFee;
            //              sale.Paid = true;
            //          }
            //      }
            //  }
            //  else
            //  {
            //      double SaleFinal = cart.GetTotal();
            //      sale.SaleTotal = SaleFinal + deliveryFee;
            //      sale.Paid = true;
            //  }

            //  Check product stock
            var x = cart.GetCartItems();
            List<Products> chck = (from q in dB.Products
                                   select q).ToList();
            //if (x.Count() > 0)
            //{
            //    foreach (var items in x)
            //    {
            //        if (chck.Count() > 0)
            //            foreach (Products c in chck)
            //            {
            //                if (items.ProductId == c.ProductId)
            //                {
            //                    Random random = new Random();
            //                    int start2 = random.Next(0, cmt.Length);

            //                    var addAna = AnalyticsHelper.Insert(c.ProductName, c.ProductDescription, cmt[start2].ToString(), rt[start2]);

            //                    c.ProductStock -= items.Count; //Temporary stock check pause... Make this -= to reduce stock
            //                    if (c.ProductStock == 0)
            //                    {
            //                        c.isActive = false; //Sets item to inactive due to stock being zero. 
            //                    }
            //                }
            //            }
            //    }
            //}


            //  Save Order to Delivery and Sale Table
            Delivery delivery = new Delivery
            {
                SaleId = sale.SaleId,
                DeliveryFee = deliveryFee,
                CurrentLocation = "131 George Sewpersadh St, Verulam, Ethekwini, 4340",
                isDelivered = false
            };
            dB.Sales.Add(sale);
            dB.SaveChanges();
           // delivery.SaleId = sale.SaleId;
            sale.Dispatched = false;
            sale.ConfirmOrder = null;
            sale.ConfirmDelivery = false;
            //dB.Deliveries.Add(delivery);




            //double? FinalCost;
            double? FinalCost = cart.GetTotal() + cart.GetDeliveryFee();

            //if (Discount.Any())
            //{
            //    foreach (var discount in Discount)
            //    {
            //        if (discount.CouponDiscountAmount != null)
            //        {
            //            FinalCost = cart.GetTotal() - (cart.GetTotal() * discount.CouponDiscountAmount) / 100 + cart.GetDeliveryFee();
            //        }
            //        if (discount.CouponUserIsValid == true)
            //        {
            //            discount.CouponUserIsValid = false;
            //            discount.Coupon.CouponCounter = +1;
            //        }
            //    }
            //}
            dB.SaveChanges();



            //SaleDetail Saved Using Helper Method
            string orderId = sale.SaleId.ToString();
            sale = cart.CreateOrder(sale);

            //New Email.
            //Creates a new PDF document
            PdfDocument document = new PdfDocument();
            //Adds page settings
            document.PageSettings.Orientation = PdfPageOrientation.Portrait;
            document.PageSettings.Margins.All = 50;
            //Adds a page to the document
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            //Loads the image from disk
            PdfImage image = PdfImage.FromFile(Server.MapPath("~/Content/Images/Evetech.jpeg"));
            RectangleF bounds = new RectangleF(10, 10, 200, 200);
            //Draws the image to the PDF page
            page.Graphics.DrawImage(image, bounds);
            PdfBrush solidBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
            bounds = new RectangleF(0, bounds.Bottom + 90, graphics.ClientSize.Width, 30);
            //Draws a rectangle to place the heading in that region.
            graphics.DrawRectangle(solidBrush, bounds);
            //Creates a font for adding the heading in the page
            PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);
            //Creates a text element to add the invoice number
            PdfTextElement element = new PdfTextElement("Invoice " + sale.SaleId.ToString() + " for" + " " + sale.Name, subHeadingFont);
            element.Brush = PdfBrushes.White;

            //Draws the heading on the page
            PdfLayoutResult res = element.Draw(page, new PointF(10, bounds.Top + 8));
            string currentDate = "Date Purchased " + sale.SaleDate.ToString();
            //Measures the width of the text to place it in the correct location
            SizeF textSize = subHeadingFont.MeasureString(currentDate);
            PointF textPosition = new PointF(graphics.ClientSize.Width - textSize.Width - 10, res.Bounds.Y);
            //Draws the date by using DrawString method
            graphics.DrawString(currentDate, subHeadingFont, element.Brush, textPosition);
            PdfFont timesRoman = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            //Creates text elements to add the address and draw it to the page.
            element = new PdfTextElement("Bill To " + sale.Address.ToString() + ", " + sale.City + ", " + sale.State, timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
            res = element.Draw(page, new PointF(10, res.Bounds.Bottom + 25));
            element = new PdfTextElement("Total Price R" + sale.SaleTotal.ToString(), timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
            res = element.Draw(page, new PointF(10, res.Bounds.Bottom + 25));
            PdfPen linePen = new PdfPen(new PdfColor(126, 151, 173), 0.70f);
            PointF startPoint = new PointF(0, res.Bounds.Bottom + 3);
            PointF endPoint = new PointF(graphics.ClientSize.Width, res.Bounds.Bottom + 3);
            //Draws a line at the bottom of the address
            graphics.DrawLine(linePen, startPoint, endPoint);

            //Creates the datasource for the table
            DataTable invoiceDetails = new DataTable();

            //Add columns to the DataTable
            invoiceDetails.Columns.Add("Products Name");
            invoiceDetails.Columns.Add("Quantity");
            invoiceDetails.Columns.Add("Price");

            //Add rows to the DataTable
            foreach (var item in sale.SaleDetails.Where(m => m.SaleId == sale.SaleId))
            {
                invoiceDetails.Rows.Add(new object[] { item.Products.ProductName, item.Quantity, item.Products.ProductPrice });
            }


            //Creates text elements to add the address and draw it to the page.



            //Creates a PDF grid
            PdfGrid grid = new PdfGrid();
            //Adds the data source
            grid.DataSource = invoiceDetails;
            //Creates the grid cell styles
            PdfGridCellStyle cellStyle = new PdfGridCellStyle();
            cellStyle.Borders.All = PdfPens.White;
            PdfGridRow header = grid.Headers[0];
            //Creates the header style
            PdfGridCellStyle headerStyle = new PdfGridCellStyle();
            headerStyle.Borders.All = new PdfPen(new PdfColor(126, 151, 173));
            headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(126, 151, 173));
            headerStyle.TextBrush = PdfBrushes.White;
            headerStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14f, PdfFontStyle.Regular);

            //Adds cell customizations
            for (int i = 0; i < header.Cells.Count; i++)
            {
                if (i == 0 || i == 1)
                    header.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                else
                    header.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Right, PdfVerticalAlignment.Middle);
            }

            //Applies the header style
            header.ApplyStyle(headerStyle);
            cellStyle.Borders.Bottom = new PdfPen(new PdfColor(217, 217, 217), 0.70f);
            cellStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12f);
            cellStyle.TextBrush = new PdfSolidBrush(new PdfColor(131, 130, 136));
            //Creates the layout format for grid
            PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
            // Creates layout format settings to allow the table pagination
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //Draws the grid to the PDF page.
            PdfGridLayoutResult gridResult = grid.Draw(page, new RectangleF(new PointF(0, res.Bounds.Bottom + 40), new SizeF(graphics.ClientSize.Width, graphics.ClientSize.Height - 100)), layoutFormat);

            MemoryStream outputStream = new MemoryStream();
            document.Save(outputStream);
            outputStream.Position = 0;

            var invoicePdf = new System.Net.Mail.Attachment(outputStream, System.Net.Mime.MediaTypeNames.Application.Pdf);
            string docname = "Invoice.pdf";
            invoicePdf.ContentDisposition.FileName = docname;

            MailMessage mail = new MailMessage();
            string emailTo = sale.Email;
            MailAddress from = new MailAddress("HotelListVX@gmail.com");
            mail.From = from;
            mail.Subject = "Your invoice for order number #" + sale.SaleId;
            mail.Body = "Dear Customer" + sale.Name + " find your invoice in the attached PDF document.";
            mail.To.Add(emailTo);

            mail.Attachments.Add(invoicePdf);

            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential networkCredential = new NetworkCredential("HotelListVX@gmail.com", "ujzzmzrxomafbwkb");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = networkCredential;
            smtp.Port = 587;
            smtp.Send(mail);
            //Clean-up.
            //Close the document.
            document.Close(true);
            //Dispose of email.
            mail.Dispose();


            try
            {
                var GetName = User.Identity.Name;
                var Findis = UserManager.FindByEmailAsync(GetName);
                if (FindPaymentMethods.FirstOrDefault(g => g.Key == Keys.PY_PayFast).Id == shpVM.PaymentMethodId)
                {
                    #region PayFast
                    //  required values for the PayFast Merchant
                    string name = "TechStore Sale Number: #" + sale.SaleTotal;
                    string description = "This is a once-off and non-refundable payment. ";

                    string site = "https://sandbox.payfast.co.za/eng/process";
                    string merchant_id = "";
                    string merchant_key = "";

                    string paymentMode = System.Configuration.ConfigurationManager.AppSettings["PaymentMode"];

                    if (paymentMode == "test")
                    {
                        site = "https://sandbox.payfast.co.za/eng/process?";
                        merchant_id = "10000100";
                        merchant_key = "46f0cd694581a";
                    }

                    // Build the query string for payment site

                    StringBuilder str = new StringBuilder();
                    str.Append("merchant_id=" + HttpUtility.UrlEncode(merchant_id));
                    str.Append("&merchant_key=" + HttpUtility.UrlEncode(merchant_key));
                    str.Append("&return_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_ReturnURL"]));
                    str.Append("&cancel_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_CancelURL"]));
                    str.Append("&notify_url=" + HttpUtility.UrlEncode(System.Configuration.ConfigurationManager.AppSettings["PF_NotifyURL"]));

                    str.Append("&m_payment_id=" + HttpUtility.UrlEncode(sale.SaleId.ToString()));
                    str.Append("&amount=" + HttpUtility.UrlEncode(FinalCost.ToString()));
                    str.Append("&item_name=" + HttpUtility.UrlEncode(name));
                    str.Append("&item_description=" + HttpUtility.UrlEncode(description));

                    // Redirect to PayFast
                    return Redirect(site + str.ToString());
                    #endregion
                }
                else if (FindPaymentMethods.FirstOrDefault(g => g.Key == Keys.PY_EFT).Id == shpVM.PaymentMethodId)
                {

                    // Method use EFT Method to do work
                    using (ApplicationDbContext core = new ApplicationDbContext())
                    {
                        int BankDtId = 0;
                        var CheckIfCardNoExist = core.BankingDetails.FirstOrDefault(b => b.CardNo == shpVM.bankingDetails.CardNo) ?? null;
                        if (CheckIfCardNoExist == null)
                        {
                            // Save bank details entered if banking details selected use that one in else statment
                            string ApplicationUserId = Findis?.Result?.Id.ToString();
                            BankingDetails banking = new BankingDetails()
                            {
                                CardNo = shpVM?.bankingDetails?.CardNo,
                                BankId = shpVM?.bankingDetails?.BankId,
                                ApplicationUserId = ApplicationUserId,
                                ExpDate = shpVM?.bankingDetails?.ExpDate,
                                CCV = shpVM?.bankingDetails?.CCV
                            };
                            core.BankingDetails.Add(banking);
                            core.SaveChanges();
                            BankDtId = banking.Id;
                        }
                        else
                        {
                            BankDtId = CheckIfCardNoExist.Id;
                        }
                        OnlineSalesOrCODInfo onlineSalesOrCODInfo = new OnlineSalesOrCODInfo()
                        {
                            Amount = (decimal)FinalCost,
                            BankingDetailsId = BankDtId,
                            CreatedDate = DateTime.Now,
                            CurrencyCode = "ZAR",
                            SalesId = sale.SaleId

                        };
                        core.OnlineSalesInfos.Add(onlineSalesOrCODInfo);
                        core.SaveChanges();

                        return RedirectToAction("Success", "Checkout", new { Amount = FinalCost, TranscationRefNo = sale.TranscationId, FullName = User.Identity.Name, TypeOfPayment = Keys.PY_EFT });

                    }
                }
                else
                {

                    return RedirectToAction("Success", "Checkout", new { Amount = FinalCost, TranscationRefNo = sale.TranscationId, FullName = User.Identity.Name, TypeOfPayment = Keys.PY_COD });
                }




            }
            catch (Exception e)
            {
                string message = e.ToString();
                return RedirectToAction("Failed", "Checkout");
                // throw;
            }
        }

        public ActionResult Success(string Amount, string TranscationRefNo, string FullName, string TypeOfPayment)
        {
            ViewBag.Name = FullName;
            if (TypeOfPayment == Keys.PY_COD)
                ViewBag.NewShow = $"Your items have been order Please note that you will be liable to pay the amount of ZAR{Amount} on the day of delivery" +
                     $"Transcation No : {TranscationRefNo}" +
                    $"Thanks for Ordering Enjoy";

            else
                ViewBag.NewShow = $"Your items have been ordered and payment of the amount ZAR{Amount} has been received  " +
                    $"Transcation No : {TranscationRefNo}" +
                    $"Thanks for Ordering Enjoy";



            return View();

        }
        public ActionResult Failed()
        {
            return View();
        }

        // GET: Checkout/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Checkout/Create
        public ActionResult Create(ApplicationUser applicationUser)
        {
            return View();
        }

        // POST: Checkout/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Checkout/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Checkout/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Checkout/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Checkout/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public JsonResult GetBankResult(int? Id)
        {
            using (ApplicationDbContext core = new ApplicationDbContext())
            {
                var FindBankDetails = core.BankingDetails.FirstOrDefault(x => x.Id == Id) ?? null;
                return Json(FindBankDetails, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult CheckPaymentMethod(int? Id)
        {
            using (ApplicationDbContext core = new ApplicationDbContext())
            {
                var CheckPayMet = core.PaymentMethods.FirstOrDefault(x => x.Id == Id) ?? null;
                if (CheckPayMet.Name.ToLower().Contains("eft"))
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);

            }
        }
    }
}