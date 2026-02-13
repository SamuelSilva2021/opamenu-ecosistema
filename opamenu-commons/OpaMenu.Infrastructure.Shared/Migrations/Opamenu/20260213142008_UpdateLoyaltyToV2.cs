using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class UpdateLoyaltyToV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_loyalty_programs_tenant_id",
                table: "loyalty_programs");

            migrationBuilder.AddColumn<int>(
                name: "reward_type",
                table: "loyalty_programs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "reward_value",
                table: "loyalty_programs",
                type: "numeric(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "target_count",
                table: "loyalty_programs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "loyalty_programs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "loyalty_program_filters",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    loyalty_program_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyalty_program_filters", x => x.id);
                    table.ForeignKey(
                        name: "FK_loyalty_program_filters_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_loyalty_program_filters_loyalty_programs_loyalty_program_id",
                        column: x => x.loyalty_program_id,
                        principalTable: "loyalty_programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_loyalty_program_filters_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_programs_tenant_id",
                table: "loyalty_programs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_program_filters_category_id",
                table: "loyalty_program_filters",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_program_filters_loyalty_program_id",
                table: "loyalty_program_filters",
                column: "loyalty_program_id");

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_program_filters_product_id",
                table: "loyalty_program_filters",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loyalty_program_filters");

            migrationBuilder.DropIndex(
                name: "IX_loyalty_programs_tenant_id",
                table: "loyalty_programs");

            migrationBuilder.DropColumn(
                name: "reward_type",
                table: "loyalty_programs");

            migrationBuilder.DropColumn(
                name: "reward_value",
                table: "loyalty_programs");

            migrationBuilder.DropColumn(
                name: "target_count",
                table: "loyalty_programs");

            migrationBuilder.DropColumn(
                name: "type",
                table: "loyalty_programs");

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_programs_tenant_id",
                table: "loyalty_programs",
                column: "tenant_id",
                unique: true);
        }
    }
}
