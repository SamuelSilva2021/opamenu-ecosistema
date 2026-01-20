using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class ProgramaDeFidelidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customer_loyalty_balances",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    balance = table.Column<int>(type: "integer", nullable: false),
                    total_earned = table.Column<int>(type: "integer", nullable: false),
                    last_activity_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_loyalty_balances", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_loyalty_balances_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_programs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    points_per_currency = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    min_order_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    points_validity_days = table.Column<int>(type: "integer", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyalty_programs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customer_loyalty_balance_id = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<int>(type: "integer", nullable: true),
                    points = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loyalty_transactions", x => x.id);
                    table.ForeignKey(
                        name: "FK_loyalty_transactions_customer_loyalty_balances_customer_loy~",
                        column: x => x.customer_loyalty_balance_id,
                        principalTable: "customer_loyalty_balances",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_loyalty_transactions_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1,
                column: "description",
                value: "Pratos principais do cardÃ¡pio");

            migrationBuilder.UpdateData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "description", "name" },
                values: new object[] { "Pagamento via cartÃ£o de crÃ©dito", "CrÃ©dito" });

            migrationBuilder.UpdateData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "description", "name" },
                values: new object[] { "Pagamento via cartÃ£o de dÃ©bito", "DÃ©bito" });

            migrationBuilder.UpdateData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 3,
                column: "description",
                value: "Pagamento instantÃ¢neo via PIX");

            migrationBuilder.UpdateData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 4,
                column: "description",
                value: "Pagamento em espÃ©cie");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "description", "name" },
                values: new object[] { "HambÃºrguer artesanal com carne bovina, queijo, alface, tomate e molho especial", "HambÃºrguer ClÃ¡ssico" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 2,
                column: "description",
                value: "Pizza tradicional com molho de tomate, mussarela e manjericÃ£o fresco");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 5,
                column: "description",
                value: "Pudim caseiro de leite condensado com calda de aÃ§Ãºcar");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "description", "name" },
                values: new object[] { "Batatas crocantes cortadas na casa, tempero especial - serve atÃ© 3 pessoas", "PorÃ§Ã£o de Batata Frita" });

            migrationBuilder.CreateIndex(
                name: "IX_customer_loyalty_balances_customer_id",
                table: "customer_loyalty_balances",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_loyalty_balances_tenant_id_customer_id",
                table: "customer_loyalty_balances",
                columns: new[] { "tenant_id", "customer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_programs_tenant_id",
                table: "loyalty_programs",
                column: "tenant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_transactions_customer_loyalty_balance_id",
                table: "loyalty_transactions",
                column: "customer_loyalty_balance_id");

            migrationBuilder.CreateIndex(
                name: "IX_loyalty_transactions_order_id",
                table: "loyalty_transactions",
                column: "order_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loyalty_programs");

            migrationBuilder.DropTable(
                name: "loyalty_transactions");

            migrationBuilder.DropTable(
                name: "customer_loyalty_balances");

            migrationBuilder.UpdateData(
                table: "categories",
                keyColumn: "id",
                keyValue: 1,
                column: "description",
                value: "Pratos principais do cardápio");

            migrationBuilder.UpdateData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "description", "name" },
                values: new object[] { "Pagamento via cartão de crédito", "Crédito" });

            migrationBuilder.UpdateData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "description", "name" },
                values: new object[] { "Pagamento via cartão de débito", "Débito" });

            migrationBuilder.UpdateData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 3,
                column: "description",
                value: "Pagamento instantâneo via PIX");

            migrationBuilder.UpdateData(
                table: "payment_methods",
                keyColumn: "id",
                keyValue: 4,
                column: "description",
                value: "Pagamento em espécie");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "description", "name" },
                values: new object[] { "Hambúrguer artesanal com carne bovina, queijo, alface, tomate e molho especial", "Hambúrguer Clássico" });

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 2,
                column: "description",
                value: "Pizza tradicional com molho de tomate, mussarela e manjericão fresco");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 5,
                column: "description",
                value: "Pudim caseiro de leite condensado com calda de açúcar");

            migrationBuilder.UpdateData(
                table: "products",
                keyColumn: "id",
                keyValue: 6,
                columns: new[] { "description", "name" },
                values: new object[] { "Batatas crocantes cortadas na casa, tempero especial - serve até 3 pessoas", "Porção de Batata Frita" });
        }
    }
}
