using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NguyenMinhTri_2122110517.Model;
using System;

namespace NguyenMinhTri_2122110517.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Contact relationship
            modelBuilder.Entity<Contact>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.User_Id);

            // Order relationship
            modelBuilder.Entity<Order>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.User_Id);

            // OrderDetail relationship
            modelBuilder.Entity<OrderDetail>()
                .HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.Order_Id);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.Product_Id);

            // Post - Topic relationship
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Topic)
                .WithMany()
                .HasForeignKey(p => p.Topic_Id);

            // Product relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.Category_Id);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.Brand_Id);

            // Unique constraints for User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
