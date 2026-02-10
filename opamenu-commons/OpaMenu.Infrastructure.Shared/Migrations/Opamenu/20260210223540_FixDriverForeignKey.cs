using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class FixDriverForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_collaborators_driver_id1",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_driver_id1",
                table: "orders");

            migrationBuilder.CreateIndex(
                name: "IX_orders_driver_id",
                table: "orders",
                column: "driver_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_collaborators_driver_id",
                table: "orders",
                column: "driver_id",
                principalTable: "collaborators",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_collaborators_driver_id",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_driver_id",
                table: "orders");

            migrationBuilder.CreateIndex(
                name: "IX_orders_driver_id1",
                table: "orders",
                column: "driver_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_collaborators_driver_id1",
                table: "orders",
                column: "driver_id1",
                principalTable: "collaborators",
                principalColumn: "id");
        }
    }
}
