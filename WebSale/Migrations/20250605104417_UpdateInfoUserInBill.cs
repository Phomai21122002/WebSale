using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSale.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInfoUserInBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Bills",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Bills",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Bills");
        }
    }
}
