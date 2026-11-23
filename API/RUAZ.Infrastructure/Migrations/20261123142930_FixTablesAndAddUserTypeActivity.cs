using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RUAZ.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTablesAndAddUserTypeActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntityBase_EntityBase_UserId",
                table: "EntityBase");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityBase",
                table: "EntityBase");

            migrationBuilder.DropIndex(
                name: "IX_EntityBase_UserId",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "BuyerId",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "EntityBase");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EntityBase");

            migrationBuilder.RenameTable(
                name: "EntityBase",
                newName: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameNormalized",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeActivity",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserId",
                table: "Products",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TypeActivity",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "EntityBase");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "EntityBase",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "NameNormalized",
                table: "EntityBase",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EntityBase",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EntityBase",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "BuyerId",
                table: "EntityBase",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "EntityBase",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EntityBase",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "EntityBase",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "EntityBase",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "EntityBase",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "EntityBase",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EntityBase",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "EntityBase",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "EntityBase",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityBase",
                table: "EntityBase",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EntityBase_UserId",
                table: "EntityBase",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EntityBase_EntityBase_UserId",
                table: "EntityBase",
                column: "UserId",
                principalTable: "EntityBase",
                principalColumn: "Id");
        }
    }
}
