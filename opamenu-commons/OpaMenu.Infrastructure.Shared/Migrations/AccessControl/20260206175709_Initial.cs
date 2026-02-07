using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpaMenu.Infrastructure.Shared.Migrations.AccessControl
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    secret_key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    url = table.Column<string>(type: "text", nullable: false),
                    code = table.Column<string>(type: "text", nullable: true),
                    auxiliar_schema = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    visible = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "group_type",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_group_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    code = table.Column<string>(type: "text", nullable: true),
                    application_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_module", x => x.id);
                    table.ForeignKey(
                        name: "FK_module_application_application_id",
                        column: x => x.application_id,
                        principalTable: "application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "access_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    group_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_access_group_group_type_group_type_id",
                        column: x => x.group_type_id,
                        principalTable: "group_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    actions = table.Column<List<string>>(type: "jsonb", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_permission_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_account",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    password_reset_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    password_reset_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    role_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_account_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "role_access_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_access_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_access_group_access_group_access_group_id",
                        column: x => x.access_group_id,
                        principalTable: "access_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_access_group_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_access_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    granted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_access_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_access_group_access_group_access_group_id",
                        column: x => x.access_group_id,
                        principalTable: "access_group",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_access_group_user_account_user_account_id",
                        column: x => x.user_account_id,
                        principalTable: "user_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "group_type",
                columns: new[] { "id", "code", "CreatedAt", "description", "is_active", "name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("4dcb20a2-b350-3384-25a1-27d98e79d6ac"), "SYSTEM_ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Equipe de analistas do sistema", true, "Administradores do Sistema", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("921ceb40-24ff-ed9e-7e0e-bf33042e9272"), "TENANT_ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Administradores dos estabelecimentos", true, "Administradores do Tenant", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "module",
                columns: new[] { "id", "application_id", "code", "CreatedAt", "description", "is_active", "key", "name", "UpdatedAt", "url" },
                values: new object[,]
                {
                    { new Guid("13482be3-f205-837c-cceb-e28f14015000"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Visão geral do sistema", true, "DASHBOARD", "Dashboard", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/dashboard" },
                    { new Guid("20bf5030-e615-8586-a0b1-c624ce259deb"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de clientes", true, "CUSTOMER", "Clientes", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/customers" },
                    { new Guid("3ad894aa-3bdf-db04-9715-a1134c4c8825"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de cupons", true, "COUPON", "Cupons", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/coupons" },
                    { new Guid("4822a024-3dd0-674d-638d-ddee1f7b1a2b"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de adicionais", true, "ADITIONAL", "Adicionais", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/addons" },
                    { new Guid("6f331ece-a4ea-74f6-158b-c7a5be39cc41"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de produtos", true, "PRODUCT", "Produtos", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/products" },
                    { new Guid("9b464720-4a29-d649-f64a-7b8ebec95bcf"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de pedidos", true, "ORDER", "Pedidos", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/orders" },
                    { new Guid("a7060970-7bc8-8e98-4566-e00676be1578"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de pagamentos", true, "PAYMENT", "Pagamentos", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/payments" },
                    { new Guid("aa19bde8-48c1-7dd4-1f18-bd177952614d"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Configurações do sistema/estabelecimento", true, "SETTINGS", "Configurações", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/settings" },
                    { new Guid("d8265dac-ea6a-353a-5bd5-065d519df46b"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de categorias", true, "CATEGORY", "Categorias", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/categories" },
                    { new Guid("de03bd4d-f7e9-0b1b-f3a3-2b5ea9ee30b8"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Programa de fidelidade", true, "LOYALTY", "Fidelidade", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/loyalty" },
                    { new Guid("f30c45da-30a3-45cb-f932-88af5354e1e6"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de grupos de adicionais", true, "ADITIONAL_GROUP", "Grupos de Adicionais", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/addon-groups" },
                    { new Guid("f415b1d6-6989-b41d-dcb8-4a0a824fc7f1"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de mesas", true, "TABLE", "Mesas", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/tables" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "application_id", "code", "CreatedAt", "description", "is_active", "IsSystem", "name", "tenant_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), null, "ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Administrador do estabelecimento", true, false, "Administrador", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("724add9f-febd-c615-19db-8e526968da25"), null, "SUPER_ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Acesso total ao sistema", true, false, "Super Administrador", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "access_group",
                columns: new[] { "id", "code", "CreatedAt", "description", "group_type_id", "is_active", "name", "tenant_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2987c0d9-519f-6245-c920-889f697a658c"), "GRP_SYS_ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Grupo de super administradores", new Guid("4dcb20a2-b350-3384-25a1-27d98e79d6ac"), true, "System Admins", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aee4edda-f145-f4c7-7edb-20a33888df4b"), "GRP_TENANT_ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Grupo de administradores de tenant", new Guid("921ceb40-24ff-ed9e-7e0e-bf33042e9272"), true, "Tenant Admins", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "role_permission",
                columns: new[] { "id", "actions", "CreatedAt", "is_active", "module_key", "role_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("23656d51-12f7-68d9-7f6d-29e7cd61b1ef"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "LOYALTY", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2553895f-dca2-074e-70bd-91f176a9a95e"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "ORDER", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("296d5a1c-cfee-0528-923e-97665c86fff4"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "TABLE", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("3a5beddc-59ef-836a-5b68-b118654c6cfe"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "ADITIONAL", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("3f442530-6a55-c4de-1f7c-d84ca02127b7"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "TABLE", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("48110f70-f135-7fb4-e381-bd1cadeaa493"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "CUSTOMER", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("4ca88129-5a57-5042-36ee-4821d5bdd77d"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "SETTINGS", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("4d30ea3f-980a-5243-b88d-3ab629c248d4"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "CUSTOMER", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5238e92d-8af1-3a3e-7ada-e005537e89b0"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "ORDER", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6109438b-666b-c9ec-b56d-b3826c27e0f7"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "DASHBOARD", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("621b724b-3f07-a045-902c-e3742108cd46"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "DASHBOARD", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6454f469-be9d-6322-23cc-67d0c099bf33"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "PAYMENT", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("710cf9d8-d056-c475-f3fd-63ef9f64bf2b"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "CATEGORY", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7207671f-3e58-dec0-c03c-256591af4c2f"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "COUPON", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("792de7db-6f24-f089-86c7-bebcf98f0fbb"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "COUPON", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8a36cd1b-803d-1e1f-bd56-98b59b75a5de"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "PRODUCT", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("95f0c29e-53f9-5810-0c06-be2f013ed4c0"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "CATEGORY", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("95fa54d7-c059-2560-77ef-5580ce321834"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "LOYALTY", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("96874e72-413b-1374-d3f9-65a348bc5f1c"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "ADITIONAL", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a0e6a4fe-ce63-744f-46f0-f043ffc9f52a"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "ADITIONAL_GROUP", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a1b99574-afa9-e011-da32-a4b56a6e8aa3"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "SETTINGS", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ae09b375-68a5-cf77-c7a9-8a797517f359"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "ADITIONAL_GROUP", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cd3226c5-c36a-b664-fe33-98299080ff6d"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "PAYMENT", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f42751c1-e9eb-37ca-e52d-9de6b1ec1595"), new List<string> { "READ", "CREATE", "UPDATE", "DELETE" }, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "PRODUCT", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "user_account",
                columns: new[] { "id", "created_at", "DeletedAt", "email", "first_name", "is_email_verified", "last_login_at", "last_name", "password_hash", "password_reset_expires_at", "password_reset_token", "phone_number", "role_id", "status", "tenant_id", "updated_at", "username" },
                values: new object[] { new Guid("6bc3c728-4ffa-67f6-2ed9-d4f18594f4c0"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@opamenu.com.br", "System", true, null, "Admin", "$2a$11$rR/VYsNgEYRwaJt/bMn2ieq.izZrI8dUMfd4yottdElTWQL/vh7eO", null, null, null, new Guid("724add9f-febd-c615-19db-8e526968da25"), "Ativo", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "admin" });

            migrationBuilder.InsertData(
                table: "account_access_group",
                columns: new[] { "id", "access_group_id", "CreatedAt", "expires_at", "GrantedAt", "granted_by", "is_active", "UpdatedAt", "user_account_id" },
                values: new object[] { new Guid("f1e8f88f-6526-5bf8-6a70-c5238f40dd92"), new Guid("2987c0d9-519f-6245-c920-889f697a658c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6bc3c728-4ffa-67f6-2ed9-d4f18594f4c0") });

            migrationBuilder.InsertData(
                table: "role_access_group",
                columns: new[] { "id", "access_group_id", "CreatedAt", "is_active", "role_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c0cb54ff-a183-7932-d250-f75da02e1c75"), new Guid("2987c0d9-519f-6245-c920-889f697a658c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dcececfe-1dc2-7d14-910b-408d435c5582"), new Guid("aee4edda-f145-f4c7-7edb-20a33888df4b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_access_group_group_type_id",
                table: "access_group",
                column: "group_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_account_access_group_access_group_id",
                table: "account_access_group",
                column: "access_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_account_access_group_user_account_id_access_group_id",
                table: "account_access_group",
                columns: new[] { "user_account_id", "access_group_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_group_type_code",
                table: "group_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_module_application_id",
                table: "module",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_module_key",
                table: "module",
                column: "key");

            migrationBuilder.CreateIndex(
                name: "IX_role_code",
                table: "role",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_access_group_access_group_id_role_id",
                table: "role_access_group",
                columns: new[] { "access_group_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_access_group_role_id",
                table: "role_access_group",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_role_id_module_key",
                table: "role_permission",
                columns: new[] { "role_id", "module_key" });

            migrationBuilder.CreateIndex(
                name: "IX_user_account_email",
                table: "user_account",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_account_role_id",
                table: "user_account",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_account_tenant_id",
                table: "user_account",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_account_username",
                table: "user_account",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_access_group");

            migrationBuilder.DropTable(
                name: "module");

            migrationBuilder.DropTable(
                name: "role_access_group");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "user_account");

            migrationBuilder.DropTable(
                name: "application");

            migrationBuilder.DropTable(
                name: "access_group");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "group_type");
        }
    }
}
