using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Undisclosed_Shop.Models
{
    public class ShoppingCart
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();
        }

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        //Add To Cart
        public int AddToCart(Products product)
        {
            // Get the matching cart and item instances
            var cartItem = dbContext.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ProductId == product.ProductId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    ProductId = product.ProductId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                dbContext.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            dbContext.SaveChanges();

            return cartItem.Count;
        }

        //Remove From Cart
        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = dbContext.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.ProductId == id);


            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    dbContext.Carts.Remove(cartItem);
                }
                // Save changes
                dbContext.SaveChanges();
            }
            return itemCount;
        }

        //Empty Cart
        public void EmptyCart()
        {
            var cartItems = dbContext.Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                dbContext.Carts.Remove(cartItem);
            }
            // Save changes
            dbContext.SaveChanges();
        }

        //Items In Cart
        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in dbContext.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        //Total Cost Of Cart
        public double GetTotal()
        {
            // Multiply item price by count of that item to get 
            // the current price for each of those items in the cart
            // sum all item price totals to get the cart total
            double? total;

            total = (from cartItems in dbContext.Carts
                     where cartItems.CartId == ShoppingCartId
                     select (int?)cartItems.Count *
                     cartItems.Products.ProductPrice).Sum();




            return total ?? double.NaN;
        }

        //Get Cart Items
        public List<Cart> GetCartItems()
        {
            return dbContext.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
        }

        //Get Cart Items
        public double GetDeliveryFee()
        {
            double deliveryFee = 0;
            int numProducts = 0;

            numProducts = GetCount();

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

            return deliveryFee;
        }

        //Cart Finalize
        public Sale CreateOrder(Sale sale)
        {
            double orderTotal = 0;
            sale.SaleDetails = new List<SaleDetail>();

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var saleDetail = new SaleDetail()
                {
                    ProductId = item.ProductId,
                    SaleId = sale.SaleId,
                    ProductPrice = item.Products.ProductPrice,
                    Quantity = item.Count,
                };

                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Products.ProductPrice);
                sale.SaleDetails.Add(saleDetail);
                dbContext.SaleDetails.Add(saleDetail);

            }
            // Set the order's total to the orderTotal count
            sale.SaleTotal = orderTotal;

            // Save the order
            dbContext.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return sale;
        }


    }
}