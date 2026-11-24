using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RUAZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProductStockAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "BuyerId",
                table: "Products",
                type: "uuid",
                nullable: true);
        }
    }
}
