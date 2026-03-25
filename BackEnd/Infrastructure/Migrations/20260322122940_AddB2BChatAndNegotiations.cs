using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddB2BChatAndNegotiations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Conversations_ConversationId",
                table: "Chats");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsSystemMessage",
                table: "Chats");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "ChatMessages",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "ChatMessages",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "ChatMessages",
                newName: "ChatRoomId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_ConversationId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_ChatRoomId");

            migrationBuilder.AddColumn<decimal>(
                name: "ProposedPrice",
                table: "ChatMessages",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProposedQuantity",
                table: "ChatMessages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RelatedOrderId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ChatMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChatRooms_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalSchema: "security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_BuyerId",
                table: "ChatRooms",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_OrganizationId",
                table: "ChatRooms",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_ProductId",
                table: "ChatRooms",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_ChatRooms_ChatRoomId",
                table: "ChatMessages",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_SenderId",
                table: "ChatMessages",
                column: "SenderId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_ChatRooms_ChatRoomId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ProposedPrice",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "ProposedQuantity",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "RelatedOrderId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ChatMessages");

            migrationBuilder.RenameTable(
                name: "ChatMessages",
                newName: "Chats");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Chats",
                newName: "Timestamp");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Chats",
                newName: "Message");

            migrationBuilder.RenameColumn(
                name: "ChatRoomId",
                table: "Chats",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_ChatRoomId",
                table: "Chats",
                newName: "IX_Chats_ConversationId");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "Chats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemMessage",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastMessageContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastMessageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Conversations_Users_BuyerId",
                        column: x => x.BuyerId,
                        principalSchema: "security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversations_Users_SellerId",
                        column: x => x.SellerId,
                        principalSchema: "security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_BuyerId",
                table: "Conversations",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ProductId",
                table: "Conversations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_SellerId",
                table: "Conversations",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Conversations_ConversationId",
                table: "Chats",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
