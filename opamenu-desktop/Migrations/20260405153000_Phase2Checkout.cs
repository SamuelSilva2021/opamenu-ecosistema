using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Desktop.Migrations
{
    public partial class Phase2Checkout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentBreakdownJson",
                table: "LocalOrders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AditionalsJson",
                table: "LocalOrderItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "LocalOrderItems",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentBreakdownJson",
                table: "LocalOrders");

            migrationBuilder.DropColumn(
                name: "AditionalsJson",
                table: "LocalOrderItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "LocalOrderItems");
        }
    }
}
