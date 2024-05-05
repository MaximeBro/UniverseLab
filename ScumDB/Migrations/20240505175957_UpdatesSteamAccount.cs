﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScumDB.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesSteamAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SavedAt",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavedAt",
                table: "Accounts");
        }
    }
}
