using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Desktop.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocalOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrderType = table.Column<string>(type: "TEXT", nullable: false),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: true),
                    CustomerDocument = table.Column<string>(type: "TEXT", nullable: true),
                    TableNumber = table.Column<string>(type: "TEXT", nullable: true),
                    OrderObservation = table.Column<string>(type: "TEXT", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "TEXT", nullable: false),
                    ChangeAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentMethod = table.Column<string>(type: "TEXT", nullable: true),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: false),
                    LocalId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CloudId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSyncAttempt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocalOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    LocalOrderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductName = table.Column<string>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalOrderItems_LocalOrders_LocalOrderId",
                        column: x => x.LocalOrderId,
                        principalTable: "LocalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocalOrderItems_LocalOrderId",
                table: "LocalOrderItems",
                column: "LocalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalOrders_LocalId",
                table: "LocalOrders",
                column: "LocalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalOrders_SyncStatus",
                table: "LocalOrders",
                column: "SyncStatus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocalOrderItems");

            migrationBuilder.DropTable(
                name: "LocalOrders");
        }
    }
}
