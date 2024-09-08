using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScumDB.Migrations
{
    /// <inheritdoc />
    public partial class AddsSteamAccountSteamName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SteamName",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SteamName",
                table: "Accounts");
        }
    }
}
