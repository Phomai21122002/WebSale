using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSale.Migrations
{
    /// <inheritdoc />
    public partial class AddNameOrderInBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameOrder",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameOrder",
                table: "Bills");
        }
    }
}
