using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuctionOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Auctions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_OrderId",
                table: "Auctions",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Orders_OrderId",
                table: "Auctions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Orders_OrderId",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_OrderId",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Auctions");
        }
    }
}
