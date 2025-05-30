using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSale.Migrations
{
    /// <inheritdoc />
    public partial class AddModelMoMoDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MomoModel_MomoId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MomoModel",
                table: "MomoModel");

            migrationBuilder.RenameTable(
                name: "MomoModel",
                newName: "MomoInfos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MomoInfos",
                table: "MomoInfos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MomoInfos_MomoId",
                table: "Orders",
                column: "MomoId",
                principalTable: "MomoInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MomoInfos_MomoId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MomoInfos",
                table: "MomoInfos");

            migrationBuilder.RenameTable(
                name: "MomoInfos",
                newName: "MomoModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MomoModel",
                table: "MomoModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MomoModel_MomoId",
                table: "Orders",
                column: "MomoId",
                principalTable: "MomoModel",
                principalColumn: "Id");
        }
    }
}
