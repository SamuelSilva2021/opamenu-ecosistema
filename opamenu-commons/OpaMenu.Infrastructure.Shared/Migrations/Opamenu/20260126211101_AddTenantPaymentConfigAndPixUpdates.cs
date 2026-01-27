using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class AddTenantPaymentConfigAndPixUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_payments_created_at",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "processed_at",
                table: "payments",
                newName: "qr_code_expiration_at");

            // Manual adjustment for Postgres casting
            migrationBuilder.Sql("ALTER TABLE tenant_payment_methods ALTER COLUMN configuration TYPE jsonb USING configuration::jsonb");

            migrationBuilder.AlterColumn<Guid>(
                name: "tenant_id",
                table: "payments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "payments",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "paid_at",
                table: "payments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "payment_method_id",
                table: "payments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "provider",
                table: "payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "provider_payment_id",
                table: "payments",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "qr_code",
                table: "payments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "payment_transaction",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<int>(type: "integer", nullable: false),
                    provider_event_id = table.Column<string>(type: "text", nullable: true),
                    RawPayload = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_transaction", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_transaction_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_payment_configs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "text", nullable: false),
                    payment_method = table.Column<string>(type: "text", nullable: false),
                    pix_key = table.Column<string>(type: "text", nullable: false),
                    client_id = table.Column<string>(type: "text", nullable: false),
                    is_sandbox = table.Column<bool>(type: "boolean", nullable: true),
                    client_secret = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_payment_configs", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payments_payment_method_id",
                table: "payments",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_provider_payment_id",
                table: "payments",
                column: "provider_payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_tenant_id_order_id",
                table: "payments",
                columns: new[] { "tenant_id", "order_id" });

            migrationBuilder.CreateIndex(
                name: "IX_payment_transaction_payment_id",
                table: "payment_transaction",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_transaction_provider_event_id",
                table: "payment_transaction",
                column: "provider_event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_payments_payment_methods_payment_method_id",
                table: "payments",
                column: "payment_method_id",
                principalTable: "payment_methods",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_payments_payment_methods_payment_method_id",
                table: "payments");

            migrationBuilder.DropTable(
                name: "payment_transaction");

            migrationBuilder.DropTable(
                name: "tenant_payment_configs");

            migrationBuilder.DropIndex(
                name: "IX_payments_payment_method_id",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_payments_provider_payment_id",
                table: "payments");

            migrationBuilder.DropIndex(
                name: "IX_payments_tenant_id_order_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "paid_at",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "payment_method_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "provider",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "provider_payment_id",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "qr_code",
                table: "payments");

            migrationBuilder.RenameColumn(
                name: "qr_code_expiration_at",
                table: "payments",
                newName: "processed_at");

            migrationBuilder.AlterColumn<string>(
                name: "configuration",
                table: "tenant_payment_methods",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "tenant_id",
                table: "payments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_payments_created_at",
                table: "payments",
                column: "created_at");
        }
    }
}
