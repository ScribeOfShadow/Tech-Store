using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Undisclosed_Shop.Models;
using Undisclosed_Shop.VeiwModels;

namespace Undisclosed_Shop.Controllers
{
        [Authorize]
        public class ShoppingCartController : Controller
        {
            ApplicationDbContext dB = new ApplicationDbContext();


            // GET: ShoppingCart
            public ActionResult Index()
            {
                var cart = ShoppingCart.GetCart(this.HttpContext);

                // Set up our ViewModel


                var viewModel = new ShoppingCartViewModel()
                {

                    CartItems = cart.GetCartItems(),
                    CartTotal = cart.GetTotal()

                };

                ViewBag.TotalCost = cart.GetTotal() + cart.GetDeliveryFee();
                ViewBag.ShippingCost = cart.GetDeliveryFee();

                // Return the view
                return View(viewModel);
            }

            //Add To Cart
            [HttpPost]

            public ActionResult AddToCart(int id)
            {
                // Retrieve the item from the database
                var addedItem = dB.Products
                    .Single(products => products.ProductId == id);

                var test = addedItem.ProductName;
                // Add it to the shopping cart
                var cart = ShoppingCart.GetCart(this.HttpContext);

                int count = cart.AddToCart(addedItem);


                var results = new ShoppingCartRemoveViewModel
                {
                    Message = Server.HtmlEncode(addedItem.ProductName) +
                        " has been added to your shopping cart.",
                    CartTotal = cart.GetTotal(),
                    CartCount = cart.GetCount(),
                    ItemCount = count,
                    DeleteId = id
                };
                return Json(results);
            }
            //End

            //Remove From Cart
            [HttpPost]
            public ActionResult RemoveFromCart(int id)
            {
                // Remove the item from the cart
                var cart = ShoppingCart.GetCart(this.HttpContext);

                // Get the name of the item to display confirmation

                // Get the name of the album to display confirmation
                string itemName = dB.Products
                    .Single(item => item.ProductId == id).ProductName;

                // Remove from cart
                int itemCount = cart.RemoveFromCart(id);

                // Display the confirmation message
                var results = new ShoppingCartRemoveViewModel
                {
                    Message = "One (1) " + Server.HtmlEncode(itemName) +
                        " has been removed from your shopping cart.",
                    CartTotal = cart.GetTotal(),
                    CartCount = cart.GetCount(),
                    ItemCount = itemCount,
                    DeleteId = id
                };
                return Json(results);
            }
            //End

            // GET: /ShoppingCart/CartSummary
            [ChildActionOnly]
            public ActionResult CartSummary()
            {
                var cart = ShoppingCart.GetCart(this.HttpContext);

                ViewData["CartCount"] = cart.GetCount();
                return PartialView("CartSummary");
            }


            // GET: ShoppingCart/Details/5
            public ActionResult Details(int id)
            {
                return View();
            }

            // GET: ShoppingCart/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: ShoppingCart/Create
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

            // GET: ShoppingCart/Edit/5
            public ActionResult Edit(int id)
            {
                return View();
            }

            // POST: ShoppingCart/Edit/5
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

            // GET: ShoppingCart/Delete/5
            public ActionResult Delete(int id)
            {
                return View();
            }

            // POST: ShoppingCart/Delete/5
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
            public ActionResult DetailsCon()
            {
                var cart = ShoppingCart.GetCart(this.HttpContext);

                // Set up our ViewModel
                var viewModel = new ShoppingCartViewModel()
                {
                    CartItems = cart.GetCartItems(),
                    CartTotal = cart.GetTotal()
                };

                ViewBag.TotalCost = cart.GetTotal() + cart.GetDeliveryFee();
                ViewBag.ShippingCost = cart.GetDeliveryFee();
                // Return the view
                return View(viewModel);
            }
        }
    
}