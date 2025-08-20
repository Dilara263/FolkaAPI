using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FolkaAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "1",
                column: "Image",
                value: "http://10.0.2.2:5227/images/totteBag.webp");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "2",
                column: "Image",
                value: "http://10.0.2.2:5227/images/tassel.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "3",
                column: "Image",
                value: "https://picsum.photos/seed/bag2/400/400");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "4",
                column: "Image",
                value: "http://10.0.2.2:5227/images/fular.webp");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "5",
                column: "Image",
                value: "https://picsum.photos/seed/necklace2/400/400");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "1",
                column: "Image",
                value: "https://images.unsplash.com/photo-1596700874052-16a7042c1613?q=80&w=1887&auto=format&fit=crop");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "2",
                column: "Image",
                value: "https://images.unsplash.com/photo-1588661646271-e23114a79b29?q=80&w=1887&auto=format&fit=crop");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "3",
                column: "Image",
                value: "https://images.unsplash.com/photo-1588661646271-e23114a79b29?q=80&w=1887&auto=format&fit=crop");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "4",
                column: "Image",
                value: "https://images.unsplash.com/photo-1596700874052-16a7042c1613?q=80&w=1887&auto=format&fit=crop");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: "5",
                column: "Image",
                value: "https://images.unsplash.com/photo-1588661646271-e23114a79b29?q=80&w=1887&auto=format&fit=crop");
        }
    }
}
