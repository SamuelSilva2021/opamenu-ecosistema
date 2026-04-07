using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class AddTabs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "tab_id",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tabs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    opened_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    closed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tabs", x => x.id);
                    table.ForeignKey(
                        name: "FK_tabs_tables_table_id",
                        column: x => x.table_id,
                        principalTable: "tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantWhatsAppConfigEntity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false),
                    InstanceId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApiKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    WelcomeBotEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    OrderStatusLookupEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    WelcomeMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantWhatsAppConfigEntity", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_tab_id",
                table: "orders",
                column: "tab_id");

            migrationBuilder.CreateIndex(
                name: "IX_tabs_table_id",
                table: "tabs",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_tabs_tenant_id_table_id_status",
                table: "tabs",
                columns: new[] { "tenant_id", "table_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantWhatsAppConfigEntity_tenant_id",
                table: "TenantWhatsAppConfigEntity",
                column: "tenant_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_tabs_tab_id",
                table: "orders",
                column: "tab_id",
                principalTable: "tabs",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_tabs_tab_id",
                table: "orders");

            migrationBuilder.DropTable(
                name: "tabs");

            migrationBuilder.DropTable(
                name: "TenantWhatsAppConfigEntity");

            migrationBuilder.DropIndex(
                name: "IX_orders_tab_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "tab_id",
                table: "orders");
        }
    }
}
