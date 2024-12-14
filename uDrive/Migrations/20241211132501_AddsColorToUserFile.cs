using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uDrive.Migrations
{
    /// <inheritdoc />
    public partial class AddsColorToUserFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "UserFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "UserFiles");
        }
    }
}
