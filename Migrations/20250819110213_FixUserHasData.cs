using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FolkaAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixUserHasData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1",
                column: "PasswordHash",
                value: "$2a$11$6czEq3jqxd33yQeZKWSGteUzDigkSa4zEqQiyndKNfQkhK7jEp6yK,");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "2",
                column: "PasswordHash",
                value: "$2a$11$Up..XmfOQrriidmoh/lHNuIp7fxpuKDlU8zboh1ecD9.SudkPhfMC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "1",
                column: "PasswordHash",
                value: "$2a$11$/9Z/DxO.BO4rod6TXoqgYugWuJYvSuuRXVrRbQAyjWwkjTkv8d/yq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "2",
                column: "PasswordHash",
                value: "$2a$11$DQPPsfBdhW2iqGME7qErI.sfuht8pgwnSNYMogMJIWE6dUNUP7r2a");
        }
    }
}
