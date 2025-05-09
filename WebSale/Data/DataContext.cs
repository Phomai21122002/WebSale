using Microsoft.EntityFrameworkCore;
using WebSale.Models;

namespace WebSale.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) {
            
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<ImageCategory> ImageCategories { get; set; }
        public DbSet<ImageFeedBack> ImageFeedBacks { get; set; }
        public DbSet<ImageProduct> ImageProducts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Provinces> Provinces { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<VnpayModel> VnInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAddress>()
                .HasKey(ua => new { ua.UserId, ua.AddressId });
            modelBuilder.Entity<UserAddress>()
                .HasOne(u => u.User)
                .WithMany(ua => ua.UserAddresses)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<UserAddress>()
                .HasOne(a => a.Address)
                .WithMany(ua => ua.UserAddresses)
                .HasForeignKey(a => a.AddressId);

            modelBuilder.Entity<Cart>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Cart>()
                .HasKey(c => new { c.UserId, c.ProductId });
            modelBuilder.Entity<Cart>()
                .HasOne(u => u.User)
                .WithMany(c => c.Carts)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<Cart>()
                .HasOne(p => p.Product)
                .WithMany(cp => cp.Carts)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<OrderProduct>()
                .HasKey(od => new { od.OrderId, od.ProductId });
            modelBuilder.Entity<OrderProduct>()
                .HasOne(o => o.Order)
                .WithMany(od => od.OrderProducts)
                .HasForeignKey(o => o.OrderId);
            modelBuilder.Entity<OrderProduct>()
                .HasOne(p => p.Product)
                .WithMany(od => od.OrderProducts)
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductDetail)
                .WithOne(pd => pd.Product)
                .HasForeignKey<Product>(p => p.ProductDetailId);

            modelBuilder.Entity<Provinces>()
                .HasMany(p => p.Districts)
                .WithOne(u => u.Provinces)
                .HasForeignKey(d => d.ParentCode)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<District>()
                .HasMany(d => d.Wards)
                .WithOne(w => w.District)
                .HasForeignKey(w => w.ParentCode)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
