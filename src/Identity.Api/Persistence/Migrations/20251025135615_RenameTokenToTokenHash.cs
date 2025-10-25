using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameTokenToTokenHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRefreshTokens_Token",
                table: "UserRefreshTokens");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "UserRefreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "TokenHash",
                table: "UserRefreshTokens",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_TokenHash",
                table: "UserRefreshTokens",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRefreshTokens_TokenHash",
                table: "UserRefreshTokens");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "UserRefreshTokens");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "UserRefreshTokens",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserRefreshTokens_Token",
                table: "UserRefreshTokens",
                column: "Token",
                unique: true);
        }
    }
}
