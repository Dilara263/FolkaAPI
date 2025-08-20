using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FolkaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Image = table.Column<string>(type: "TEXT", nullable: true),
                    Category = table.Column<string>(type: "TEXT", nullable: true),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Description", "Image", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { "1", "Çanta", "Yüksek kaliteli iplikle elde örülmüş, güzel ve dayanıklı bir bez çanta. Günlük kullanım için mükemmeldir.", "http://10.0.2.2:5227/images/totteBag.webp", "Hand-Knitted Tote Bag", "250 TL", 8 },
                    { "2", "Takı", "Bu eşsiz, el yapımı püsküllü kolye ile tarzınıza bohem-şık bir dokunuş katın.", "http://10.0.2.2:5227/images/tassel.jpg", "Bohemian Tassel Necklace", "120 TL", 15 },
                    { "3", "Çanta", "Zarif, el yapımı omuz çantası. Her kıyafetle uyum sağlar.", "https://picsum.photos/seed/bag2/400/400", "El Örgüsü Omuz Çantası", "350 TL", 5 },
                    { "4", "Fular", "Güzel bir çiçek baskısına sahip, hafif ve yumuşak bir ipek fular. Çok yönlü ve şık.", "http://10.0.2.2:5227/images/fular.webp", "Silk Floral Scarf (Fular)", "150 TL", 20 },
                    { "5", "Takı", "925 ayar gümüş, el işçiliği hayat ağacı kolye.", "https://picsum.photos/seed/necklace2/400/400", "Gümüş Hayat Ağacı Kolye", "450 TL", 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
