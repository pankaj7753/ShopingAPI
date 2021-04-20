using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.DataLayer.Data
{
    public class DataContextOld : DbContext
    {
        public DataContextOld(DbContextOptions<DataContextOld> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<OrderSummary> OrderSummaries { get; set; }
        public DbSet<ShopingAPI.DataLayer.Models.Unit> Unit { get; set; }
        [NotMapped]
        public DbSet<VendorProducts> VendorProducts { get; set; }

        public DbSet<OrderStatus> OrderStatuses { get; set; }

    }
}
