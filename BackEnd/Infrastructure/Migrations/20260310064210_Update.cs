using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_MarketItems_MarketItemId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Users_ApplicationUserId",
                table: "Bids");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_MarketItems_ProductId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketItems_Organizations_OrganizationId",
                table: "MarketItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MarketItems_Organizations_OrganizationId1",
                table: "MarketItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_MarketItems_MarketItemId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_Users_ApplicationUserId",
                table: "OrganizationMembers");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMembers_ApplicationUserId",
                table: "OrganizationMembers");

            migrationBuilder.DropIndex(
                name: "IX_Bids_ApplicationUserId",
                table: "Bids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MarketItems",
                table: "MarketItems");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "MarketItems");

            migrationBuilder.DropColumn(
                name: "SourceCropId",
                table: "MarketItems");

            migrationBuilder.RenameTable(
                name: "MarketItems",
                newName: "Products");

            migrationBuilder.RenameColumn(
                name: "MarketItemId",
                table: "OrderItems",
                newName: "productid");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_MarketItemId",
                table: "OrderItems",
                newName: "IX_OrderItems_productid");

            migrationBuilder.RenameColumn(
                name: "MarketItemId",
                table: "Auctions",
                newName: "productid");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_MarketItemId",
                table: "Auctions",
                newName: "IX_Auctions_productid");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "RejectionReason");

            migrationBuilder.RenameIndex(
                name: "IX_MarketItems_OrganizationId1",
                table: "Products",
                newName: "IX_Products_OrganizationId1");

            migrationBuilder.RenameIndex(
                name: "IX_MarketItems_OrganizationId",
                table: "Products",
                newName: "IX_Products_OrganizationId");

            migrationBuilder.AlterColumn<int>(
                name: "ProductType",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Products_productid",
                table: "Auctions",
                column: "productid",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Products_ProductId",
                table: "Conversations",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_productid",
                table: "OrderItems",
                column: "productid",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Organizations_OrganizationId",
                table: "Products",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Organizations_OrganizationId1",
                table: "Products",
                column: "OrganizationId1",
                principalTable: "Organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Products_productid",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Products_ProductId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_productid",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Organizations_OrganizationId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Organizations_OrganizationId1",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "MarketItems");

            migrationBuilder.RenameColumn(
                name: "productid",
                table: "OrderItems",
                newName: "MarketItemId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_productid",
                table: "OrderItems",
                newName: "IX_OrderItems_MarketItemId");

            migrationBuilder.RenameColumn(
                name: "productid",
                table: "Auctions",
                newName: "MarketItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Auctions_productid",
                table: "Auctions",
                newName: "IX_Auctions_MarketItemId");

            migrationBuilder.RenameColumn(
                name: "RejectionReason",
                table: "MarketItems",
                newName: "ImageUrl");

            migrationBuilder.RenameIndex(
                name: "IX_Products_OrganizationId1",
                table: "MarketItems",
                newName: "IX_MarketItems_OrganizationId1");

            migrationBuilder.RenameIndex(
                name: "IX_Products_OrganizationId",
                table: "MarketItems",
                newName: "IX_MarketItems_OrganizationId");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "OrganizationMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Bids",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProductType",
                table: "MarketItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ItemType",
                table: "MarketItems",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SourceCropId",
                table: "MarketItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarketItems",
                table: "MarketItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_ApplicationUserId",
                table: "OrganizationMembers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_ApplicationUserId",
                table: "Bids",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_MarketItems_MarketItemId",
                table: "Auctions",
                column: "MarketItemId",
                principalTable: "MarketItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Users_ApplicationUserId",
                table: "Bids",
                column: "ApplicationUserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_MarketItems_ProductId",
                table: "Conversations",
                column: "ProductId",
                principalTable: "MarketItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MarketItems_Organizations_OrganizationId",
                table: "MarketItems",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketItems_Organizations_OrganizationId1",
                table: "MarketItems",
                column: "OrganizationId1",
                principalTable: "Organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_MarketItems_MarketItemId",
                table: "OrderItems",
                column: "MarketItemId",
                principalTable: "MarketItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_Users_ApplicationUserId",
                table: "OrganizationMembers",
                column: "ApplicationUserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
