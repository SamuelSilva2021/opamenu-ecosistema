using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class RenameAddonsToAditionals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_item_addons");

            migrationBuilder.DropTable(
                name: "product_addon_groups");

            migrationBuilder.DropTable(
                name: "addons");

            migrationBuilder.DropTable(
                name: "addon_groups");

            migrationBuilder.CreateTable(
                name: "aditional_groups",
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
                    table.PrimaryKey("PK_aditional_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "aditionals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    aditional_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aditionals", x => x.id);
                    table.ForeignKey(
                        name: "FK_aditionals_aditional_groups_aditional_group_id",
                        column: x => x.aditional_group_id,
                        principalTable: "aditional_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "product_aditional_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    aditional_group_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_product_aditional_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_product_aditional_groups_aditional_groups_aditional_group_id",
                        column: x => x.aditional_group_id,
                        principalTable: "aditional_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_product_aditional_groups_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_item_aditionals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    aditional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    aditional_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_item_aditionals", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_item_aditionals_aditionals_aditional_id",
                        column: x => x.aditional_id,
                        principalTable: "aditionals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_item_aditionals_order_items_order_item_id",
                        column: x => x.order_item_id,
                        principalTable: "order_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_aditionals_aditional_group_id",
                table: "aditionals",
                column: "aditional_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_aditionals_aditional_id",
                table: "order_item_aditionals",
                column: "aditional_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_aditionals_order_item_id",
                table: "order_item_aditionals",
                column: "order_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_aditional_groups_aditional_group_id",
                table: "product_aditional_groups",
                column: "aditional_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_aditional_groups_product_id_aditional_group_id",
                table: "product_aditional_groups",
                columns: new[] { "product_id", "aditional_group_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_item_aditionals");

            migrationBuilder.DropTable(
                name: "product_aditional_groups");

            migrationBuilder.DropTable(
                name: "aditionals");

            migrationBuilder.DropTable(
                name: "aditional_groups");

            migrationBuilder.CreateTable(
                name: "addon_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    max_selections = table.Column<int>(type: "integer", nullable: true),
                    min_selections = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addon_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "addons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    addon_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "product_addon_groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    addon_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    display_order = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    max_selections_override = table.Column<int>(type: "integer", nullable: true),
                    min_selections_override = table.Column<int>(type: "integer", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "order_item_addons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    addon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    addon_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_addons_addon_group_id",
                table: "addons",
                column: "addon_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_addons_addon_id",
                table: "order_item_addons",
                column: "addon_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_item_addons_order_item_id",
                table: "order_item_addons",
                column: "order_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_addon_groups_addon_group_id",
                table: "product_addon_groups",
                column: "addon_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_addon_groups_product_id_addon_group_id",
                table: "product_addon_groups",
                columns: new[] { "product_id", "addon_group_id" },
                unique: true);
        }
    }
}
