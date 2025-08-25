using FolkaAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System;

namespace FolkaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }
        public DbSet<AppliedCoupon> AppliedCoupons { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = "1", Name = "Hand-Knitted Tote Bag", Price = "250 TL", Description = "Yüksek kaliteli iplikle elde örülmüş, güzel ve dayanıklı bir bez çanta. Günlük kullanım için mükemmeldir.", Image = "http://10.0.2.2:5227/images/totteBag.webp", Category = "Çanta", Stock = 8 },
                new Product { Id = "2", Name = "Bohemian Tassel Necklace", Price = "120 TL", Description = "Bu eşsiz, el yapımı püsküllü kolye ile tarzınıza bohem-şık bir dokunuş katın.", Image = "http://10.0.2.2:5227/images/tassel.jpg", Category = "Takı", Stock = 15 },
                new Product { Id = "3", Name = "El Örgüsü Omuz Çantası", Price = "350 TL", Description = "Zarif, el yapımı omuz çantası. Her kıyafetle uyum sağlar.", Image = "https://picsum.photos/seed/bag2/400/400", Category = "Çanta", Stock = 5 },
                new Product { Id = "4", Name = "Silk Floral Scarf (Fular)", Price = "150 TL", Description = "Güzel bir çiçek baskısına sahip, hafif ve yumuşak bir ipek fular. Çok yönlü ve şık.", Image = "http://10.0.2.2:5227/images/fular.webp", Category = "Fular", Stock = 20 },
                new Product { Id = "5", Name = "Gümüş Hayat Ağacı Kolye", Price = "450 TL", Description = "925 ayar gümüş, el işçiliği hayat ağacı kolye.", Image = "https://picsum.photos/seed/necklace2/400/400", Category = "Takı", Stock = 10 }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = "04f97db4-3c37-4e07-a46a-1630c9c004b7",
                    Name = "Dilara Dereli",
                    Email = "dilaraemail",
                    PasswordHash = "$2a$11$E3Ytpo3XyvjWEz.DOg.cmU1jYv2cv6JyZUTtUPWITG",
                    PhoneNumber = "5419187223",
                    Address = "Yeni Mah"
                },
                new User
                {
                    Id = "9befa110-b89c-4b6e-a1be-f5a17b7efddd",
                    Name = "emo",
                    Email = "emoemail",
                    PasswordHash = "$2a$11$6771O0X6P0BxBV0tV0UukKzLbqnMT48O0Xhng4q76WUnMBDJGO",
                    PhoneNumber = "16165",
                    Address = "sixsos"
                }
            );

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon { Id = "IND10", Description = "%10 İndirim Kuponu", DiscountValue = 0.10m, DiscountType = "Percentage", MinOrderAmount = 50, IsActive = true, ExpiryDate = new DateTime(2026, 8, 20, 0, 0, 0, DateTimeKind.Utc) },
                new Coupon { Id = "FREEKARGO", Description = "Ücretsiz Kargo Kuponu", DiscountValue = 15m, DiscountType = "Amount", MinOrderAmount = 100, IsActive = true, ExpiryDate = new DateTime(2027, 2, 20, 0, 0, 0, DateTimeKind.Utc) },
                new Coupon { Id = "YAZINDIRIMI", Description = "Yaz İndirimi %20", DiscountValue = 0.20m, DiscountType = "Percentage", MinOrderAmount = 75, IsActive = true, ExpiryDate = new DateTime(2025, 9, 20, 0, 0, 0, DateTimeKind.Utc) }
            );

            modelBuilder.Entity<UserCoupon>().HasData(
                new UserCoupon { Id = 1, UserId = "04f97db4-3c37-4e07-a46a-1630c9c004b7", CouponId = "IND10", IsUsed = false, AcquiredDate = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc) }, // Dilara Dereli için IND10
                new UserCoupon { Id = 2, UserId = "04f97db4-3c37-4e07-a46a-1630c9c004b7", CouponId = "YAZINDIRIMI", IsUsed = false, AcquiredDate = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc) }, // Dilara Dereli için YAZINDIRIMI
                new UserCoupon { Id = 3, UserId = "9befa110-b89c-4b6e-a1be-f5a17b7efddd", CouponId = "FREEKARGO", IsUsed = false, AcquiredDate = new DateTime(2025, 8, 20, 0, 0, 0, DateTimeKind.Utc) } // emo için FREEKARGO
            );

            modelBuilder.Entity<CartItem>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .IsRequired();

            modelBuilder.Entity<UserFavorite>()
                .HasKey(uf => new { uf.UserId, uf.ProductId });

            modelBuilder.Entity<UserFavorite>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(uf => uf.UserId)
                .IsRequired();

            modelBuilder.Entity<UserFavorite>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(uf => uf.ProductId)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .IsRequired();

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .IsRequired();

            modelBuilder.Entity<OrderItem>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .IsRequired();

            modelBuilder.Entity<AppliedCoupon>()
                .HasKey(ac => ac.UserId);

            modelBuilder.Entity<AppliedCoupon>()
                .HasOne(ac => ac.User)
                .WithMany()
                .HasForeignKey(ac => ac.UserId)
                .IsRequired();

            modelBuilder.Entity<AppliedCoupon>()
                .HasOne(ac => ac.Coupon)
                .WithMany()
                .HasForeignKey(ac => ac.CouponId)
                .IsRequired();
        }
    }
}
