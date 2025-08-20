using FolkaAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

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
                    Id = "1",
                    Name = "Dilara",
                    Email = "dilara@email.com",
                    PasswordHash = "$2a$11$6czEq3jqxd33yQeZKWSGteUzDigkSa4zEqQiyndKNfQkhK7jEp6yK,",
                    PhoneNumber = "+90 555 123 4567",
                    Address = "Folka Sanat Sokağı, No:12"
                },
                new User
                {
                    Id = "2",
                    Name = "Test User",
                    Email = "test@user.com",
                    PasswordHash = "$2a$11$Up..XmfOQrriidmoh/lHNuIp7fxpuKDlU8zboh1ecD9.SudkPhfMC",
                    PhoneNumber = null,
                    Address = null
                }
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
        }
    }
}