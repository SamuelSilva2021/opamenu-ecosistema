using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Opamenu.Infrastructure.Shared.Migrations.AccessControl
{
    /// <inheritdoc />
    public partial class AddPlanEnforcementModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "module",
                columns: new[] { "id", "application_id", "code", "CreatedAt", "description", "is_active", "key", "name", "UpdatedAt", "url" },
                values: new object[,]
                {
                    { new Guid("17aa0a71-ea05-4848-442e-a5e98ef3e4d6"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de colaboradores e entregadores", true, "COLLABORATOR", "Colaboradores", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/collaborators" },
                    { new Guid("70c000d9-f2be-c561-2e0b-92fe204d008b"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Acesso ao PDV desktop", true, "PDV_DESKTOP", "PDV Desktop", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/apps/desktop" },
                    { new Guid("d02bae9a-5aac-ee67-096c-bc82a33e40a4"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Cardápio público com pedidos online", true, "CARDAPIO_DELIVERY", "Cardápio Digital e Delivery", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/cardapio" },
                    { new Guid("d6733e5d-7e2c-ebab-89f9-609b57cc822d"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Acesso ao aplicativo gestor", true, "APP_GESTOR", "App Gestor", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/apps/gestor" },
                    { new Guid("ed1c0371-3723-3e4d-de63-f22dcc4849bb"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Acesso ao aplicativo do garçom", true, "APP_GARCOM", "App do Garçom", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/apps/garcom" }
                });

            migrationBuilder.InsertData(
                table: "role_permission",
                columns: new[] { "id", "actions", "CreatedAt", "is_active", "module_key", "role_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("6e5a356e-3f16-64a4-97dc-0af9213324ca"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "APP_GARCOM", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7bbf3b21-c5ea-40b3-d4f3-6fe5672d89df"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "COLLABORATOR", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("90625223-1075-a0fe-6f15-47df677b87df"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "PDV_DESKTOP", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a331de75-d5a9-52aa-d39c-868691f7b8c1"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "COLLABORATOR", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a876c989-c280-2770-72d9-166821118d7a"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "CARDAPIO_DELIVERY", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b895d9ad-7ccb-5000-a687-e39b0dc2fc55"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "APP_GARCOM", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c84a5232-c300-a26b-2766-287df3ceb0c5"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "APP_GESTOR", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cc12efd1-ee2c-9452-91b6-9f1a03d4b505"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "CARDAPIO_DELIVERY", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("cd8cd147-ae8a-c061-3ba1-3c6e577e62bb"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "PDV_DESKTOP", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f6f32a48-4a00-bdd2-019c-f1b4d3fd7764"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "APP_GESTOR", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("17aa0a71-ea05-4848-442e-a5e98ef3e4d6"));

            migrationBuilder.DeleteData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("70c000d9-f2be-c561-2e0b-92fe204d008b"));

            migrationBuilder.DeleteData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("d02bae9a-5aac-ee67-096c-bc82a33e40a4"));

            migrationBuilder.DeleteData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("d6733e5d-7e2c-ebab-89f9-609b57cc822d"));

            migrationBuilder.DeleteData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("ed1c0371-3723-3e4d-de63-f22dcc4849bb"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("6e5a356e-3f16-64a4-97dc-0af9213324ca"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("7bbf3b21-c5ea-40b3-d4f3-6fe5672d89df"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("90625223-1075-a0fe-6f15-47df677b87df"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("a331de75-d5a9-52aa-d39c-868691f7b8c1"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("a876c989-c280-2770-72d9-166821118d7a"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("b895d9ad-7ccb-5000-a687-e39b0dc2fc55"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("c84a5232-c300-a26b-2766-287df3ceb0c5"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("cc12efd1-ee2c-9452-91b6-9f1a03d4b505"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("cd8cd147-ae8a-c061-3ba1-3c6e577e62bb"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("f6f32a48-4a00-bdd2-019c-f1b4d3fd7764"));
        }
    }
}
