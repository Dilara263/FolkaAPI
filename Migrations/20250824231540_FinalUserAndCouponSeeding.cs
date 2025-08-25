using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FolkaAPI.Migrations
{
    /// <inheritdoc />
    public partial class FinalUserAndCouponSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "TEXT", nullable: false),
                    DiscountType = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MinOrderAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", nullable: true),
                    ProductPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProductImage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCoupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CouponId = table.Column<string>(type: "TEXT", nullable: false),
                    IsUsed = table.Column<bool>(type: "INTEGER", nullable: false),
                    AcquiredDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCoupons_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCoupons_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_UserFavorites_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", nullable: true),
                    ProductImage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Description", "DiscountType", "DiscountValue", "ExpiryDate", "IsActive", "MinOrderAmount" },
                values: new object[,]
                {
                    { "FREEKARGO", "Ücretsiz Kargo Kuponu", "Amount", 15m, new DateTime(2027, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, 100 },
                    { "IND10", "%10 İndirim Kuponu", "Percentage", 0.10m, new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, 50 },
                    { "YAZINDIRIMI", "Yaz İndirimi %20", "Percentage", 0.20m, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, 75 }
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

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Email", "Name", "PasswordHash", "PhoneNumber" },
                values: new object[,]
                {
                    { "04f97db4-3c37-4e07-a46a-1630c9c004b7", "Yeni Mah", "dilaraemail", "Dilara Dereli", "$2a$11$E3Ytpo3XyvjWEz.DOg.cmU1jYv2cv6JyZUTtUPWITG", "5419187223" },
                    { "9befa110-b89c-4b6e-a1be-f5a17b7efddd", "sixsos", "emoemail", "emo", "$2a$11$6771O0X6P0BxBV0tV0UukKzLbqnMT48O0Xhng4q76WUnMBDJGO", "16165" }
                });

            migrationBuilder.InsertData(
                table: "UserCoupons",
                columns: new[] { "Id", "AcquiredDate", "CouponId", "IsUsed", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "IND10", false, "04f97db4-3c37-4e07-a46a-1630c9c004b7" },
                    { 2, new DateTime(2025, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "YAZINDIRIMI", false, "04f97db4-3c37-4e07-a46a-1630c9c004b7" },
                    { 3, new DateTime(2025, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "FREEKARGO", false, "9befa110-b89c-4b6e-a1be-f5a17b7efddd" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_CouponId",
                table: "UserCoupons",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCoupons_UserId",
                table: "UserCoupons",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_ProductId",
                table: "UserFavorites",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "UserCoupons");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
