using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class UpdateLoyaltyBalanceIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_customer_loyalty_balances_tenant_id_customer_id",
                table: "customer_loyalty_balances");

            migrationBuilder.CreateIndex(
                name: "IX_customer_loyalty_balances_tenant_id_customer_id_loyalty_pro~",
                table: "customer_loyalty_balances",
                columns: new[] { "tenant_id", "customer_id", "loyalty_program_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_customer_loyalty_balances_tenant_id_customer_id_loyalty_pro~",
                table: "customer_loyalty_balances");

            migrationBuilder.CreateIndex(
                name: "IX_customer_loyalty_balances_tenant_id_customer_id",
                table: "customer_loyalty_balances",
                columns: new[] { "tenant_id", "customer_id" },
                unique: true);
        }
    }
}
