using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ShopingAPI.DataLayer.Models;

namespace ShopingAPI.DataLayer.Data
{
    public class DataContext : IdentityDbContext<User,Role,string,IdentityUserClaim<string>,UserRole,IdentityUserLogin<string>,IdentityRoleClaim<string>,IdentityUserToken<string>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("public");
            base.OnModelCreating(builder);
            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
                userRole.HasOne(ur => ur.Role).WithMany(r=>r.UserRoles).HasForeignKey(k=>k.RoleId).IsRequired();
                userRole.HasOne(ur => ur.User).WithMany(r=>r.UserRoles).HasForeignKey(k=>k.UserId).IsRequired();
            });

            builder.Entity<User>().ToTable("aspnetusers");
            builder.Entity<IdentityUserToken<string>>().ToTable("aspnetusertokens");
            builder.Entity<IdentityUserLogin<string>>().ToTable("aspnetuserlogins");
            builder.Entity<IdentityUserClaim<string>>().ToTable("aspnetuserclaims");
            builder.Entity<Role>().ToTable("aspnetroles");
            builder.Entity<UserRole>().ToTable("aspnetuserroles");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("aspnetroleclaims");

          //Create Index
          //Product
            builder.Entity<Product>().HasIndex(p => new { p.ProductName });
            builder.Entity<Product>().HasIndex(p => new { p.Id });
            builder.Entity<Product>().HasIndex(p => new { p.CategoryId });
            builder.Entity<Product>().HasIndex(p => new { p.SubCategoryId });
            builder.Entity<Product>().HasIndex(p => new { p.BrandId });
            //Shop
            builder.Entity<Shop>().HasIndex(s => new { s.ShopCode });
            builder.Entity<Shop>().HasIndex(s => new { s.Id });
            builder.Entity<Shop>().HasIndex(s => new { s.UserId });
            builder.Entity<Shop>().HasIndex(s => new { s.City });
            builder.Entity<Shop>().HasIndex(s => new { s.CategoryIds });
            builder.Entity<Shop>().HasIndex(s => new { s.Id,s.UserId });
            builder.Entity<Shop>().HasIndex(s => new { s.City,s.CategoryIds });
            builder.Entity<Shop>().HasIndex(s => new { s.IsActive,s.Id });
            //User
            builder.Entity<User>().HasIndex(u => new { u.Id,u.TableName });
            //Favorite Shop
            builder.Entity<FavoriteShop>().HasIndex(f => new { f.UserId });
            builder.Entity<FavoriteShop>().HasIndex(f => new { f.UserId,f.IsActive });
            builder.Entity<FavoriteShop>().HasIndex(f => new { f.UserId,f.ShopId });

        }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<OrderSummary> OrderSummaries { get; set; }
        public DbSet<ShopingAPI.DataLayer.Models.Unit> Unit { get; set; }
        [NotMapped]
        public DbSet<VendorProducts> VendorProducts { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<DynamicTableName>DynamicTableNames { get; set; }
        public DbSet<FavoriteShop> FavoriteShops { get; set; }

    }
}
