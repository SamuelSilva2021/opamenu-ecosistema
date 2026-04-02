using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class AddTableLayoutCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "floor",
                table: "tables",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "layout_height",
                table: "tables",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "layout_width",
                table: "tables",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "layout_x",
                table: "tables",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "layout_y",
                table: "tables",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "floor",
                table: "tables");

            migrationBuilder.DropColumn(
                name: "layout_height",
                table: "tables");

            migrationBuilder.DropColumn(
                name: "layout_width",
                table: "tables");

            migrationBuilder.DropColumn(
                name: "layout_x",
                table: "tables");

            migrationBuilder.DropColumn(
                name: "layout_y",
                table: "tables");
        }
    }
}
