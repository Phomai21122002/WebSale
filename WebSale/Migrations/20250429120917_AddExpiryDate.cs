using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSale.Migrations
{
    /// <inheritdoc />
    public partial class AddExpiryDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Decription",
                table: "ProductDetails",
                newName: "Description");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "ProductDetails",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "ProductDetails");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ProductDetails",
                newName: "Decription");
        }
    }
}
