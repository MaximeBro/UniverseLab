using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace uDrive.Migrations
{
    /// <inheritdoc />
    public partial class AddsColorToFolders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_UserFolders_ParentId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Users_UserIdentifier",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "UserFiles");

            migrationBuilder.RenameIndex(
                name: "IX_Files_UserIdentifier",
                table: "UserFiles",
                newName: "IX_UserFiles_UserIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_Files_ParentId",
                table: "UserFiles",
                newName: "IX_UserFiles_ParentId");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "UserFolders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFiles",
                table: "UserFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFiles_UserFolders_ParentId",
                table: "UserFiles",
                column: "ParentId",
                principalTable: "UserFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFiles_Users_UserIdentifier",
                table: "UserFiles",
                column: "UserIdentifier",
                principalTable: "Users",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFiles_UserFolders_ParentId",
                table: "UserFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFiles_Users_UserIdentifier",
                table: "UserFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFiles",
                table: "UserFiles");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "UserFolders");

            migrationBuilder.RenameTable(
                name: "UserFiles",
                newName: "Files");

            migrationBuilder.RenameIndex(
                name: "IX_UserFiles_UserIdentifier",
                table: "Files",
                newName: "IX_Files_UserIdentifier");

            migrationBuilder.RenameIndex(
                name: "IX_UserFiles_ParentId",
                table: "Files",
                newName: "IX_Files_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_UserFolders_ParentId",
                table: "Files",
                column: "ParentId",
                principalTable: "UserFolders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Users_UserIdentifier",
                table: "Files",
                column: "UserIdentifier",
                principalTable: "Users",
                principalColumn: "Identifier",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
