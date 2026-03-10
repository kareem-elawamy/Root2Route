using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRolesAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationMembers_OrganizationRoles_OrganizationRoleId",
                table: "OrganizationMembers");

            migrationBuilder.DropIndex(
                name: "IX_OrganizationMembers_OrganizationRoleId",
                table: "OrganizationMembers");

            migrationBuilder.DropColumn(
                name: "OrganizationRoleId",
                table: "OrganizationMembers");

            migrationBuilder.CreateTable(
                name: "OrganizationMemberOrganizationRole",
                columns: table => new
                {
                    OrganizationMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationRolesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMemberOrganizationRole", x => new { x.OrganizationMemberId, x.OrganizationRolesId });
                    table.ForeignKey(
                        name: "FK_OrganizationMemberOrganizationRole_OrganizationMembers_OrganizationMemberId",
                        column: x => x.OrganizationMemberId,
                        principalTable: "OrganizationMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationMemberOrganizationRole_OrganizationRoles_OrganizationRolesId",
                        column: x => x.OrganizationRolesId,
                        principalTable: "OrganizationRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMemberOrganizationRole_OrganizationRolesId",
                table: "OrganizationMemberOrganizationRole",
                column: "OrganizationRolesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationMemberOrganizationRole");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationRoleId",
                table: "OrganizationMembers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMembers_OrganizationRoleId",
                table: "OrganizationMembers",
                column: "OrganizationRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationMembers_OrganizationRoles_OrganizationRoleId",
                table: "OrganizationMembers",
                column: "OrganizationRoleId",
                principalTable: "OrganizationRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
