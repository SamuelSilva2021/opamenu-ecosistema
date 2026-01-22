using System;
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
                name: "operation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operation", x => x.id);
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
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
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
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_account", x => x.id);
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
                name: "permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    module_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_permission_module_module_id",
                        column: x => x.module_id,
                        principalTable: "module",
                        principalColumn: "id");
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
                name: "permission_operation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_operation", x => x.id);
                    table.ForeignKey(
                        name: "FK_permission_operation_operation_operation_id",
                        column: x => x.operation_id,
                        principalTable: "operation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_permission_operation_permission_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_role_permission_permission_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_role_permission_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
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
                    { new Guid("4a8880e0-1906-49be-5b11-ad80ddadf8ca"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de grupos de adicionais", true, "ADDON_GROUP", "Grupos de Adicionais", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/addon-groups" },
                    { new Guid("6a8409cb-b54d-a80e-afce-e0ff0454f375"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de adicionais", true, "ADDON", "Adicionais", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/addons" },
                    { new Guid("6f331ece-a4ea-74f6-158b-c7a5be39cc41"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de produtos", true, "PRODUCT", "Produtos", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/products" },
                    { new Guid("9b464720-4a29-d649-f64a-7b8ebec95bcf"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de pedidos", true, "ORDER", "Pedidos", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/orders" },
                    { new Guid("a7060970-7bc8-8e98-4566-e00676be1578"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de pagamentos", true, "PAYMENT", "Pagamentos", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/payments" },
                    { new Guid("aa19bde8-48c1-7dd4-1f18-bd177952614d"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Configurações do sistema/estabelecimento", true, "SETTINGS", "Configurações", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/settings" },
                    { new Guid("d8265dac-ea6a-353a-5bd5-065d519df46b"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de categorias", true, "CATEGORY", "Categorias", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/categories" },
                    { new Guid("de03bd4d-f7e9-0b1b-f3a3-2b5ea9ee30b8"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Programa de fidelidade", true, "LOYALTY", "Fidelidade", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/loyalty" },
                    { new Guid("f415b1d6-6989-b41d-dcb8-4a0a824fc7f1"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de mesas", true, "TABLE", "Mesas", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/tables" }
                });

            migrationBuilder.InsertData(
                table: "operation",
                columns: new[] { "id", "CreatedAt", "description", "is_active", "name", "UpdatedAt", "value" },
                values: new object[,]
                {
                    { new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Permite atualizar registros existentes", true, "Atualização", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "UPDATE" },
                    { new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Permite excluir registros", true, "Exclusão", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "DELETE" },
                    { new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Permite criar novos registros", true, "Criação", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "CREATE" },
                    { new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar registros", true, "Leitura", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "READ" }
                });

            migrationBuilder.InsertData(
                table: "role",
                columns: new[] { "id", "application_id", "code", "CreatedAt", "description", "is_active", "name", "tenant_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), null, "ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Administrador do estabelecimento", true, "Administrador", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("724add9f-febd-c615-19db-8e526968da25"), null, "SUPER_ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Acesso total ao sistema", true, "Super Administrador", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "user_account",
                columns: new[] { "id", "created_at", "DeletedAt", "email", "first_name", "is_email_verified", "last_login_at", "last_name", "password_hash", "password_reset_expires_at", "password_reset_token", "phone_number", "status", "tenant_id", "updated_at", "username" },
                values: new object[] { new Guid("6bc3c728-4ffa-67f6-2ed9-d4f18594f4c0"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@opamenu.com.br", "System", true, null, "Admin", "$2a$11$rR/VYsNgEYRwaJt/bMn2ieq.izZrI8dUMfd4yottdElTWQL/vh7eO", null, null, null, "Ativo", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "admin" });

            migrationBuilder.InsertData(
                table: "access_group",
                columns: new[] { "id", "code", "CreatedAt", "description", "group_type_id", "is_active", "name", "tenant_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2987c0d9-519f-6245-c920-889f697a658c"), "GRP_SYS_ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Grupo de super administradores", new Guid("4dcb20a2-b350-3384-25a1-27d98e79d6ac"), true, "System Admins", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aee4edda-f145-f4c7-7edb-20a33888df4b"), "GRP_TENANT_ADMIN", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Grupo de administradores de tenant", new Guid("921ceb40-24ff-ed9e-7e0e-bf33042e9272"), true, "Tenant Admins", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "permission",
                columns: new[] { "id", "CreatedAt", "is_active", "module_id", "tenant_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("4a8880e0-1906-49be-5b11-ad80ddadf8ca"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("aa19bde8-48c1-7dd4-1f18-bd177952614d"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("13482be3-f205-837c-cceb-e28f14015000"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("6f331ece-a4ea-74f6-158b-c7a5be39cc41"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a7060970-7bc8-8e98-4566-e00676be1578"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("3ad894aa-3bdf-db04-9715-a1134c4c8825"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("20bf5030-e615-8586-a0b1-c624ce259deb"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("891db09d-58af-160d-f10b-c17b93a6e834"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("f415b1d6-6989-b41d-dcb8-4a0a824fc7f1"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("de03bd4d-f7e9-0b1b-f3a3-2b5ea9ee30b8"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("6a8409cb-b54d-a80e-afce-e0ff0454f375"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("9b464720-4a29-d649-f64a-7b8ebec95bcf"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d8265dac-ea6a-353a-5bd5-065d519df46b"), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "account_access_group",
                columns: new[] { "id", "access_group_id", "CreatedAt", "expires_at", "GrantedAt", "granted_by", "is_active", "UpdatedAt", "user_account_id" },
                values: new object[] { new Guid("f1e8f88f-6526-5bf8-6a70-c5238f40dd92"), new Guid("2987c0d9-519f-6245-c920-889f697a658c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6bc3c728-4ffa-67f6-2ed9-d4f18594f4c0") });

            migrationBuilder.InsertData(
                table: "permission_operation",
                columns: new[] { "id", "CreatedAt", "is_active", "operation_id", "permission_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0b33e73e-832d-f7cf-129b-6e320e49ef20"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("1ce9de28-af8a-05d9-8775-9c26be0bf911"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("1e739438-54f3-e828-9bbd-bc0133fa960e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("891db09d-58af-160d-f10b-c17b93a6e834"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("243ac639-ef08-0bab-da93-621eb417a814"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("277512a5-1a9a-e351-d80d-21d53d72af34"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("891db09d-58af-160d-f10b-c17b93a6e834"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("31585bfb-3d5f-ade5-e06f-1916f7df6683"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("4f14a7f6-7ee3-97ab-1a12-dbcb79cbbf9f"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("56f703c6-af2a-165c-f813-f0b731769f78"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("598f5406-6414-2905-5512-e1f22d8f570f"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5c4490c5-5d53-7ad4-af3c-6552895f6cb2"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e07e56b-9f39-773e-aaf8-1d65cfa98f88"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5e7ae7c5-dd9b-8097-7cc3-f02ed6a0aa35"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("5fa84bb3-5842-897c-c69e-b2494ea75e9f"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("60e25e4d-24fe-9fc7-b9c3-93817ee3fa27"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("621bd31f-5135-0337-0445-6356c52cb520"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7bf9f602-f57e-0cb4-076f-32b77e025f2b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7fe097e6-a043-43e3-a63a-37767762d13a"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8035f3a2-608e-dd8b-28eb-bbc4a624e686"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("82178c5f-dcb9-7adf-83a5-bc4a9d4b8950"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("83157f49-463a-4c4e-3f4f-0c34304064cb"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8b953ae4-f669-1ddd-091d-5e76bf18a0b9"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8bb35719-b0dd-6b5a-fcca-6671beb129c1"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8e7f3c7c-d1ed-7e71-0fe6-6a6f0e90abae"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("94a68158-a871-8c40-22c1-3e8f1e65dd0f"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("97868498-f4c2-82d0-141c-03a7efdc1394"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a44cb1d7-9c1b-ce3a-0d8a-0b47779029dc"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b38d3e52-204e-7595-54c1-7c3514de2f50"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("bde69f79-41f1-1e23-6332-6c6473153c9c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c46a3806-1202-0544-2271-06e70cdb4bb1"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c4b7da8d-3d9b-ba07-9ea5-4a9a02006021"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("891db09d-58af-160d-f10b-c17b93a6e834"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c6df2e95-92ac-7466-18bc-52b113ec77af"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c9b02319-a31d-11ce-845f-17b9cb213096"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ca01c561-f141-a2d6-c6d9-780ae4b45158"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cb18a34a-a3ce-9f50-f5ef-d833b0c11287"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ce3570dd-dfac-8922-3a9d-943b37710f28"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d27bdf7b-a132-a1ea-4485-401877ea4956"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d533bf9d-f97f-c3b8-a7d5-0bc4a4ef9391"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d596bb91-4de6-2530-b83b-b4bf6756c4f6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d9108a4f-9203-04ac-6464-0f2d32ff2efc"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"), new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dcac12d5-446f-2318-77b6-6a76028800af"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("def72cac-85db-b40d-f612-3f030810e2cb"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"), new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("df5e9c46-2527-5224-854b-bb486082ae4e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e39eabd0-9348-b57d-ccbe-9c2f9bd3927a"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e4ae6e94-c94c-0054-6eff-4c5ac2c80971"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ed24312c-0c7a-111e-99d1-015e82fe696a"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f2affb89-72cf-75f9-05ee-31c5f1ecc55e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"), new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("feec90d0-ac9b-f282-94a1-9ccd30ebf69b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ff716dd2-1c6d-ef20-1bbb-d6a8c77b5fc2"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"), new Guid("891db09d-58af-160d-f10b-c17b93a6e834"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "role_access_group",
                columns: new[] { "id", "access_group_id", "CreatedAt", "is_active", "role_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c0cb54ff-a183-7932-d250-f75da02e1c75"), new Guid("2987c0d9-519f-6245-c920-889f697a658c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("dcececfe-1dc2-7d14-910b-408d435c5582"), new Guid("aee4edda-f145-f4c7-7edb-20a33888df4b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "role_permission",
                columns: new[] { "id", "CreatedAt", "is_active", "permission_id", "role_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("0a0ad000-30e2-e219-d9ac-84394c6e5bf9"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("343209af-3f1e-62cd-c5e2-ebd0d93c896e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("382fca83-7d5e-f15c-64fb-d58c212e3774"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("415aa751-2810-90b6-a342-0b197cd5f43c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("61b0047e-df45-a1ff-9201-e85fbb80de8d"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6a6c4a8f-39f5-b674-0c75-8c6c84783d83"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("891db09d-58af-160d-f10b-c17b93a6e834"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7ebd8351-6981-6177-eb12-2335bf96bcbd"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8bd67bc4-2d66-70e3-10c2-08c55be8d34e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8d991424-decf-2ef1-3d5d-9578c4053bbb"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("93a44a8c-a499-bc6d-b156-cd02044f85eb"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("973f191b-727f-cec6-95a3-861b4065fb45"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("9d088010-f633-94b6-3558-e59530b28ac7"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a1956b56-243d-3659-0f17-5f72c912f74c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("891db09d-58af-160d-f10b-c17b93a6e834"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("abbcba5c-d2a2-5bd1-1ad8-2385ebd8e11e"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("ac65d5ac-81fe-288a-13fd-d0ebdda9731a"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b2aa0e1c-a62d-9dba-aa48-1b71209e87e6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b66b445c-9aa5-7a4a-dd4a-767ac959e161"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("be9e1f49-5f61-c0dc-4c97-a011ba164b88"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c275fb34-e87e-1130-6a98-daba72f032f1"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d4bf4b07-95cd-5c10-daff-8844445ac205"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("db3df29d-c3a7-a4c1-54c0-38362e8508c9"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e0e5875f-dd7b-a8d2-8711-a49d4a0a0a0d"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e43ebb58-9c3a-493a-0ea1-a65374863e96"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"), new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f22f0f21-eda0-5a0d-9932-ae3408c4d73b"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"), new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
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
                name: "IX_operation_value",
                table: "operation",
                column: "value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permission_module_id",
                table: "permission",
                column: "module_id");

            migrationBuilder.CreateIndex(
                name: "IX_permission_operation_operation_id",
                table: "permission_operation",
                column: "operation_id");

            migrationBuilder.CreateIndex(
                name: "IX_permission_operation_permission_id_operation_id",
                table: "permission_operation",
                columns: new[] { "permission_id", "operation_id" },
                unique: true);

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
                name: "IX_role_permission_permission_id",
                table: "role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_role_id_permission_id",
                table: "role_permission",
                columns: new[] { "role_id", "permission_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_account_email",
                table: "user_account",
                column: "email",
                unique: true);

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
                name: "permission_operation");

            migrationBuilder.DropTable(
                name: "role_access_group");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "user_account");

            migrationBuilder.DropTable(
                name: "operation");

            migrationBuilder.DropTable(
                name: "access_group");

            migrationBuilder.DropTable(
                name: "permission");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "group_type");

            migrationBuilder.DropTable(
                name: "module");

            migrationBuilder.DropTable(
                name: "application");
        }
    }
}
