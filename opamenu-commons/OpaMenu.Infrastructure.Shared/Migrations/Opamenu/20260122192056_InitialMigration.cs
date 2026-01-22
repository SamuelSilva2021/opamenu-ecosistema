using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "addon_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    min_selections = table.Column<int>(type: "integer", nullable: true),
                    max_selections = table.Column<int>(type: "integer", nullable: true),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addon_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coupons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    discount_type = table.Column<string>(type: "text", nullable: false),
                    discount_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    min_order_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    max_discount_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    usage_limit = table.Column<int>(type: "integer", nullable: true),
                    usage_count = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    first_order_only = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coupons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "text", nullable: true),
                    street = table.Column<string>(type: "text", nullable: true),
                    street_number = table.Column<string>(type: "text", nullable: true),
                    neighborhood = table.Column<string>(type: "text", nullable: true),
                    city = table.Column<string>(type: "text", nullable: true),
                    state = table.Column<string>(type: "text", nullable: true),
                    complement = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_programs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    points_per_currency = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    currency_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
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
                name: "payment_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    icon_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_online = table.Column<bool>(type: "boolean", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tables",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    qr_code_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tables", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "addons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    addon_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addons", x => x.id);
                    table.ForeignKey(
                        name: "FK_addons_addon_groups_addon_group_id",
                        column: x => x.addon_group_id,
                        principalTable: "addon_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 10, scale: 2, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "customer_loyalty_balances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "tenant_customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    first_purchase_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_purchase_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    total_orders = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_customers", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_customers_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_payment_methods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    configuration = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_payment_methods", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_payment_methods_payment_methods_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "payment_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    customer_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    customer_email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    delivery_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    delivery_fee = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    coupon_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    is_delivery = table.Column<bool>(type: "boolean", nullable: false),
                    order_type = table.Column<string>(type: "text", nullable: false),
                    table_id = table.Column<Guid>(type: "uuid", nullable: true),
                    estimated_preparation_minutes = table.Column<int>(type: "integer", nullable: true),
                    estimated_delivery_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    queue_position = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                    table.ForeignKey(
                        name: "FK_orders_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_orders_tables_table_id",
                        column: x => x.table_id,
                        principalTable: "tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "product_addon_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    addon_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    min_selections_override = table.Column<int>(type: "integer", nullable: true),
                    max_selections_override = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_addon_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_addon_groups_addon_groups_addon_group_id",
                        column: x => x.addon_group_id,
                        principalTable: "addon_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_addon_groups_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    original_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    mime_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    file_size = table.Column<long>(type: "bigint", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false),
                    aspect_ratio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    upload_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    thumbnail_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    medium_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    large_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_images_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "loyalty_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_loyalty_balance_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    points = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_order_items_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_rejections",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    rejected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    rejected_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_rejections", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_rejections_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_status_histories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_status_histories", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_status_histories_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    method = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    gateway_transaction_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    gateway_response = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_payments_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_item_addons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    addon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    addon_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_item_addons", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_item_addons_addons_addon_id",
                        column: x => x.addon_id,
                        principalTable: "addons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_item_addons_order_items_order_item_id",
                        column: x => x.order_item_id,
                        principalTable: "order_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "payment_refunds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    refunded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    refunded_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    gateway_refund_id = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    gateway_response = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment_refunds", x => x.id);
                    table.ForeignKey(
                        name: "FK_payment_refunds_payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "addon_groups",
                columns: new[] { "id", "created_at", "description", "display_order", "is_active", "is_required", "max_selections", "min_selections", "name", "tenant_id", "type", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Escolha seus complementos favoritos", 1, true, false, 3, 0, "Complementos do Hamburguer", null, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "id", "created_at", "description", "display_order", "is_active", "name", "tenant_id", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lanches diversos", 1, true, "Lanches", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "customers",
                columns: new[] { "id", "city", "complement", "created_at", "email", "name", "neighborhood", "phone", "postal_code", "state", "street", "street_number", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000005"), null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "exemplo@exemplo.com", "Cliente Exemplo", null, "11999999999", null, null, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "loyalty_programs",
                columns: new[] { "id", "created_at", "currency_value", "description", "is_active", "min_order_value", "name", "points_per_currency", "points_validity_days", "tenant_id", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000060"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0.10m, "Ganhe pontos a cada compra e troque por descontos!", true, 20.00m, "Programa de Fidelidade Padrão", 1.0m, null, new Guid("00000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "payment_methods",
                columns: new[] { "id", "created_at", "description", "display_order", "icon_url", "is_active", "is_online", "name", "slug", "updated_at" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000010"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pagamento via cartão de crédito", 1, null, true, true, "Crédito", "credito", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("00000000-0000-0000-0000-000000000011"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pagamento via cartão de débito", 2, null, true, true, "Débito", "debito", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("00000000-0000-0000-0000-000000000012"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pagamento via PIX", 3, null, true, true, "PIX", "pix", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("00000000-0000-0000-0000-000000000013"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pagamento em dinheiro", 4, null, true, false, "Dinheiro", "dinheiro", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "tables",
                columns: new[] { "id", "capacity", "created_at", "is_active", "name", "qr_code_url", "tenant_id", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000040"), 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Mesa 1", null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "addons",
                columns: new[] { "id", "addon_group_id", "created_at", "description", "display_order", "image_url", "is_active", "name", "price", "tenant_id", "updated_at" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000020"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tiras crocantes de bacon", 1, null, true, "Bacon", 4.00m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("00000000-0000-0000-0000-000000000021"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cebolas caramelizadas na manteiga", 2, null, true, "Cebola Caramelizada", 3.00m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("00000000-0000-0000-0000-000000000022"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Adicione uma fatia extra de queijo", 3, null, true, "Queijo Extra", 2.50m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "customer_loyalty_balances",
                columns: new[] { "id", "balance", "created_at", "customer_id", "last_activity_at", "tenant_id", "total_earned", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000070"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("00000000-0000-0000-0000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("00000000-0000-0000-0000-000000000001"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "orders",
                columns: new[] { "id", "coupon_code", "created_at", "customer_email", "customer_id", "customer_name", "customer_phone", "delivery_address", "delivery_fee", "estimated_delivery_time", "estimated_preparation_minutes", "is_delivery", "notes", "order_type", "status", "subtotal", "table_id", "tenant_id", "total", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000006"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000005"), "Cliente Exemplo", "11999999999", "Rua Exemplo, 123, Bairro, Cidade, Estado", 5.00m, null, null, true, null, "Delivery", "Pending", 25.90m, null, new Guid("00000000-0000-0000-0000-000000000001"), 30.90m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "products",
                columns: new[] { "id", "category_id", "created_at", "description", "display_order", "image_url", "is_active", "name", "price", "tenant_id", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hamburguer artesanal com carne bovina, queijo, alface, tomate e molho especial", 1, null, true, "Hamburguer Clássico", 25.90m, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "tenant_customers",
                columns: new[] { "id", "created_at", "customer_id", "display_name", "first_purchase_at", "last_purchase_at", "notes", "tenant_id", "total_orders", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000050"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("00000000-0000-0000-0000-000000000005"), null, null, null, null, new Guid("00000000-0000-0000-0000-000000000001"), 0m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "order_items",
                columns: new[] { "id", "created_at", "notes", "order_id", "product_id", "product_name", "quantity", "subtotal", "tenant_id", "unit_price", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000080"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new Guid("00000000-0000-0000-0000-000000000006"), new Guid("00000000-0000-0000-0000-000000000003"), "Hamburguer Clássico", 1, 25.90m, null, 25.90m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "product_addon_groups",
                columns: new[] { "id", "addon_group_id", "created_at", "display_order", "is_required", "max_selections_override", "min_selections_override", "product_id", "tenant_id", "updated_at" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000030"), new Guid("00000000-0000-0000-0000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, false, null, null, new Guid("00000000-0000-0000-0000-000000000003"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_addons_addon_group_id",
                table: "addons",
                column: "addon_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_display_order",
                table: "categories",
                column: "display_order");

            migrationBuilder.CreateIndex(
                name: "IX_categories_is_active",
                table: "categories",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_coupons_tenant_id_code",
                table: "coupons",
                columns: new[] { "tenant_id", "code" },
                unique: true);

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
                name: "IX_customers_email",
                table: "customers",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_customers_phone",
                table: "customers",
                column: "phone");

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

            migrationBuilder.CreateIndex(
                name: "IX_order_item_addons_addon_id",
                table: "order_item_addons",
                column: "addon_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_addons_order_item_id",
                table: "order_item_addons",
                column: "order_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_product_id",
                table: "order_items",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_rejections_order_id",
                table: "order_rejections",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_order_rejections_rejected_at",
                table: "order_rejections",
                column: "rejected_at");

            migrationBuilder.CreateIndex(
                name: "IX_order_status_histories_order_id",
                table: "order_status_histories",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_status_histories_status",
                table: "order_status_histories",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_order_status_histories_timestamp",
                table: "order_status_histories",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_orders_created_at",
                table: "orders",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_customer_phone",
                table: "orders",
                column: "customer_phone");

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_type",
                table: "orders",
                column: "order_type");

            migrationBuilder.CreateIndex(
                name: "IX_orders_status",
                table: "orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_orders_table_id",
                table: "orders",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_methods_display_order",
                table: "payment_methods",
                column: "display_order");

            migrationBuilder.CreateIndex(
                name: "IX_payment_methods_is_active",
                table: "payment_methods",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_payment_methods_is_online",
                table: "payment_methods",
                column: "is_online");

            migrationBuilder.CreateIndex(
                name: "IX_payment_methods_slug",
                table: "payment_methods",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payment_refunds_payment_id",
                table: "payment_refunds",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_refunds_refunded_at",
                table: "payment_refunds",
                column: "refunded_at");

            migrationBuilder.CreateIndex(
                name: "IX_payments_created_at",
                table: "payments",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_payments_gateway_transaction_id",
                table: "payments",
                column: "gateway_transaction_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_order_id",
                table: "payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_status",
                table: "payments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_product_addon_groups_addon_group_id",
                table: "product_addon_groups",
                column: "addon_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_addon_groups_product_id_addon_group_id",
                table: "product_addon_groups",
                columns: new[] { "product_id", "addon_group_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_product_images_is_primary",
                table: "product_images",
                column: "is_primary");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_product_id",
                table: "product_images",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_images_upload_date",
                table: "product_images",
                column: "upload_date");

            migrationBuilder.CreateIndex(
                name: "IX_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_products_display_order",
                table: "products",
                column: "display_order");

            migrationBuilder.CreateIndex(
                name: "IX_products_is_active",
                table: "products",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "IX_tables_tenant_id_name",
                table: "tables",
                columns: new[] { "tenant_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_customers_customer_id",
                table: "tenant_customers",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_customers_tenant_id_customer_id",
                table: "tenant_customers",
                columns: new[] { "tenant_id", "customer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_payment_methods_payment_method_id",
                table: "tenant_payment_methods",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_payment_methods_tenant_id_payment_method_id",
                table: "tenant_payment_methods",
                columns: new[] { "tenant_id", "payment_method_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coupons");

            migrationBuilder.DropTable(
                name: "loyalty_programs");

            migrationBuilder.DropTable(
                name: "loyalty_transactions");

            migrationBuilder.DropTable(
                name: "order_item_addons");

            migrationBuilder.DropTable(
                name: "order_rejections");

            migrationBuilder.DropTable(
                name: "order_status_histories");

            migrationBuilder.DropTable(
                name: "payment_refunds");

            migrationBuilder.DropTable(
                name: "product_addon_groups");

            migrationBuilder.DropTable(
                name: "product_images");

            migrationBuilder.DropTable(
                name: "tenant_customers");

            migrationBuilder.DropTable(
                name: "tenant_payment_methods");

            migrationBuilder.DropTable(
                name: "customer_loyalty_balances");

            migrationBuilder.DropTable(
                name: "addons");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "addon_groups");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropTable(
                name: "tables");
        }
    }
}
