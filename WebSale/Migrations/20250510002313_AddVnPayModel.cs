using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSale.Migrations
{
    /// <inheritdoc />
    public partial class AddVnPayModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VnpayId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VnInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VnInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_VnpayId",
                table: "Orders",
                column: "VnpayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_VnInfos_VnpayId",
                table: "Orders",
                column: "VnpayId",
                principalTable: "VnInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_VnInfos_VnpayId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "VnInfos");

            migrationBuilder.DropIndex(
                name: "IX_Orders_VnpayId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VnpayId",
                table: "Orders");
        }
    }
}
