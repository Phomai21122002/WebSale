using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSale.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTagAndTwoFactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "ProductDetails");

            migrationBuilder.RenameColumn(
                name: "url",
                table: "Users",
                newName: "Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Users",
                newName: "url");

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "ProductDetails",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
