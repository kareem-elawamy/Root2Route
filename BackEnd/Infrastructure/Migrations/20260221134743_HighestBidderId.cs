using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class HighestBidderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HighestBidderId",
                table: "Auctions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_HighestBidderId",
                table: "Auctions",
                column: "HighestBidderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Users_HighestBidderId",
                table: "Auctions",
                column: "HighestBidderId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Users_HighestBidderId",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_HighestBidderId",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "HighestBidderId",
                table: "Auctions");
        }
    }
}
