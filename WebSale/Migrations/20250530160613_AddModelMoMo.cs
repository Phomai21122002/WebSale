using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebSale.Migrations
{
    /// <inheritdoc />
    public partial class AddModelMoMo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MomoId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MomoModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePaid = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomoModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_MomoId",
                table: "Orders",
                column: "MomoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_MomoModel_MomoId",
                table: "Orders",
                column: "MomoId",
                principalTable: "MomoModel",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_MomoModel_MomoId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "MomoModel");

            migrationBuilder.DropIndex(
                name: "IX_Orders_MomoId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MomoId",
                table: "Orders");
        }
    }
}
