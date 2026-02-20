using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class AddLoyaltyProgramIdToBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "loyalty_program_id",
                table: "customer_loyalty_balances",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_loyalty_balances_loyalty_program_id",
                table: "customer_loyalty_balances",
                column: "loyalty_program_id");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_loyalty_balances_loyalty_programs_loyalty_program_~",
                table: "customer_loyalty_balances",
                column: "loyalty_program_id",
                principalTable: "loyalty_programs",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_loyalty_balances_loyalty_programs_loyalty_program_~",
                table: "customer_loyalty_balances");

            migrationBuilder.DropIndex(
                name: "IX_customer_loyalty_balances_loyalty_program_id",
                table: "customer_loyalty_balances");

            migrationBuilder.DropColumn(
                name: "loyalty_program_id",
                table: "customer_loyalty_balances");
        }
    }
}
