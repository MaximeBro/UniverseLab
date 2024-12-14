using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uDrive.Migrations
{
    /// <inheritdoc />
    public partial class RemovesUnusedFieldsToFileAndFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileIcon",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "UserFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileIcon",
                table: "UserFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "UserFiles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
