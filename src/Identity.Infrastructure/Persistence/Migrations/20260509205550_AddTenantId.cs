using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_tenant_id",
                table: "Users",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_tenant_id",
                table: "Users",
                column: "tenant_id",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_tenant_id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_tenant_id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "Users");
        }
    }
}
