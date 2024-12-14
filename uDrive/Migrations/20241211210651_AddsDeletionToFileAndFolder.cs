using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uDrive.Migrations
{
    /// <inheritdoc />
    public partial class AddsDeletionToFileAndFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeletionAsked",
                table: "UserFolders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionAskedAt",
                table: "UserFolders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DeletionAsked",
                table: "UserFiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionAskedAt",
                table: "UserFiles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletionAsked",
                table: "UserFolders");

            migrationBuilder.DropColumn(
                name: "DeletionAskedAt",
                table: "UserFolders");

            migrationBuilder.DropColumn(
                name: "DeletionAsked",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "DeletionAskedAt",
                table: "UserFiles");
        }
    }
}
