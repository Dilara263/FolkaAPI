﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FolkaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAppliedCouponCodeToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppliedCouponCode",
                table: "Orders",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppliedCouponCode",
                table: "Orders");
        }
    }
}
