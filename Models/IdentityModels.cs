using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using BookStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Undisclosed_Shop.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNo { get; set; }
        public string IdNumber { get; set; }
        public virtual List<SaleDetail> SaleDetails { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here

            userIdentity.AddClaim(new Claim("Id", Id));
            userIdentity.AddClaim(new Claim("Name", Name));
            userIdentity.AddClaim(new Claim("Address", Address));
            userIdentity.AddClaim(new Claim("City", City));
            userIdentity.AddClaim(new Claim("State", State));
            userIdentity.AddClaim(new Claim("PostalCode", PostalCode));
            userIdentity.AddClaim(new Claim("Country", Country));
            userIdentity.AddClaim(new Claim("Phone", PhoneNo));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Undisclosed_Shop.Models.Products> Products { get; set; }

        public System.Data.Entity.DbSet<Undisclosed_Shop.Models.Cart> Carts { get; set; }

        public System.Data.Entity.DbSet<Undisclosed_Shop.Models.SaleDetail> SaleDetails { get; set; }

        public System.Data.Entity.DbSet<Undisclosed_Shop.Models.Sale> Sales { get; set; }

        public System.Data.Entity.DbSet<Undisclosed_Shop.Models.PaymentMethod> PaymentMethods { get; set; }
        public DbSet<BankingDetails> BankingDetails { get; set; }
        public DbSet<BankInfo> BankInfos { get; set; }
        public DbSet<OnlineSalesOrCODInfo> OnlineSalesInfos { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
    }
}