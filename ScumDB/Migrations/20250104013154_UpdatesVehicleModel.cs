using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScumDB.Migrations
{
    /// <inheritdoc />
    public partial class UpdatesVehicleModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Vehicles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Vehicles",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
