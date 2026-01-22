using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpaMenu.Infrastructure.Shared.Migrations.MultiTenant
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    billing_cycle = table.Column<string>(type: "text", maxLength: 20, nullable: false),
                    max_users = table.Column<int>(type: "integer", nullable: false),
                    max_storage_gb = table.Column<int>(type: "integer", nullable: false),
                    features = table.Column<string>(type: "jsonb", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", nullable: false),
                    is_trial = table.Column<bool>(type: "boolean", nullable: false),
                    trial_period_days = table.Column<int>(type: "integer", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plans", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "text", nullable: false),
                    version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "varchar(20)", nullable: false),
                    configuration_schema = table.Column<string>(type: "jsonb", nullable: true),
                    pricing_model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    setup_fee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", nullable: false),
                    trial_ends_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    current_period_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    current_period_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    cancel_at_period_end = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    custom_pricing = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    usage_limits = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscriptions_plans_plan_id",
                        column: x => x.plan_id,
                        principalTable: "plans",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_subscriptions_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    domain = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "varchar(20)", nullable: false),
                    settings = table.Column<string>(type: "jsonb", nullable: false),
                    document = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    razao_social = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    inscricao_estadual = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    inscricao_municipal = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    address_street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    address_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address_complement = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_neighborhood = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    address_state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    address_zipcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    address_country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    billing_street = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    billing_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    billing_complement = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    billing_neighborhood = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    billing_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    billing_state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    billing_zipcode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    billing_country = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    legal_representative_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    legal_representative_cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    legal_representative_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    legal_representative_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    active_subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenants_subscriptions_active_subscription_id",
                        column: x => x.active_subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_business_infos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    banner_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    instagram_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    facebook_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    whatsapp_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    opening_hours = table.Column<string>(type: "jsonb", nullable: true),
                    payment_methods = table.Column<string>(type: "jsonb", nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_business_infos", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_business_infos_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "plans",
                columns: new[] { "id", "billing_cycle", "created_at", "description", "features", "is_trial", "max_storage_gb", "max_users", "name", "price", "slug", "sort_order", "status", "trial_period_days", "updated_at" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "Mensal", DateTime.UtcNow, "Plano gratuito para pequenos negócios", null, false, 1, 1, "Free", 0m, "free", 0, "Ativo", 7, null },
                    { Guid.NewGuid(), "Mensal", DateTime.UtcNow, "Plano profissional para negócios em crescimento", null, false, 1, 10, "Pro", 99.90m, "pro", 0, "Ativo", 0, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_plans_slug",
                table: "plans",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_slug",
                table: "products",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_plan_id",
                table: "subscriptions",
                column: "plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_product_id",
                table: "subscriptions",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscriptions_tenant_id_product_id",
                table: "subscriptions",
                columns: new[] { "tenant_id", "product_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_business_infos_tenant_id",
                table: "tenant_business_infos",
                column: "tenant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_active_subscription_id",
                table: "tenants",
                column: "active_subscription_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_document",
                table: "tenants",
                column: "document",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_domain",
                table: "tenants",
                column: "domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_slug",
                table: "tenants",
                column: "slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_subscriptions_tenants_tenant_id",
                table: "subscriptions",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_subscriptions_plans_plan_id",
                table: "subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_subscriptions_products_product_id",
                table: "subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_subscriptions_tenants_tenant_id",
                table: "subscriptions");

            migrationBuilder.DropTable(
                name: "tenant_business_infos");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "subscriptions");
        }
    }
}
