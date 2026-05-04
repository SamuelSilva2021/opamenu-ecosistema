using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OpaMenu.Infrastructure.Shared.Migrations.AccessControl
{
    /// <inheritdoc />
    public partial class AddDeliveryAreaModuleSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "module",
                columns: new[] { "id", "application_id", "code", "CreatedAt", "description", "is_active", "key", "name", "UpdatedAt", "url" },
                values: new object[] { new Guid("3fcb9ed2-920b-85e5-c519-3a68fa841072"), null, null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Gerenciamento de taxas de entrega por localidade", true, "DELIVERY_AREA", "Taxas de Entrega", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "/delivery-areas" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("23656d51-12f7-68d9-7f6d-29e7cd61b1ef"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("2553895f-dca2-074e-70bd-91f176a9a95e"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("296d5a1c-cfee-0528-923e-97665c86fff4"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("3a5beddc-59ef-836a-5b68-b118654c6cfe"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("3f442530-6a55-c4de-1f7c-d84ca02127b7"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("48110f70-f135-7fb4-e381-bd1cadeaa493"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("4ca88129-5a57-5042-36ee-4821d5bdd77d"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("4d30ea3f-980a-5243-b88d-3ab629c248d4"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("5238e92d-8af1-3a3e-7ada-e005537e89b0"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("6109438b-666b-c9ec-b56d-b3826c27e0f7"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("621b724b-3f07-a045-902c-e3742108cd46"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("6454f469-be9d-6322-23cc-67d0c099bf33"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("710cf9d8-d056-c475-f3fd-63ef9f64bf2b"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("7207671f-3e58-dec0-c03c-256591af4c2f"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("792de7db-6f24-f089-86c7-bebcf98f0fbb"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("8a36cd1b-803d-1e1f-bd56-98b59b75a5de"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("95f0c29e-53f9-5810-0c06-be2f013ed4c0"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("95fa54d7-c059-2560-77ef-5580ce321834"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("96874e72-413b-1374-d3f9-65a348bc5f1c"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("a0e6a4fe-ce63-744f-46f0-f043ffc9f52a"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("a1b99574-afa9-e011-da32-a4b56a6e8aa3"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("ae09b375-68a5-cf77-c7a9-8a797517f359"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("cd3226c5-c36a-b664-fe33-98299080ff6d"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("f42751c1-e9eb-37ca-e52d-9de6b1ec1595"),
                column: "actions",
                value: "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]");

            migrationBuilder.InsertData(
                table: "role_permission",
                columns: new[] { "id", "actions", "CreatedAt", "is_active", "module_key", "role_id", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("23944e86-b43f-c4db-4541-c1081d8a9303"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "DELIVERY_AREA", new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2ddebe19-fa61-fc81-213f-c28122ed190b"), "[\"READ\",\"CREATE\",\"UPDATE\",\"DELETE\"]", new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), true, "DELIVERY_AREA", new Guid("724add9f-febd-c615-19db-8e526968da25"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("3fcb9ed2-920b-85e5-c519-3a68fa841072"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("23944e86-b43f-c4db-4541-c1081d8a9303"));

            migrationBuilder.DeleteData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("2ddebe19-fa61-fc81-213f-c28122ed190b"));

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("23656d51-12f7-68d9-7f6d-29e7cd61b1ef"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("2553895f-dca2-074e-70bd-91f176a9a95e"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("296d5a1c-cfee-0528-923e-97665c86fff4"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("3a5beddc-59ef-836a-5b68-b118654c6cfe"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("3f442530-6a55-c4de-1f7c-d84ca02127b7"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("48110f70-f135-7fb4-e381-bd1cadeaa493"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("4ca88129-5a57-5042-36ee-4821d5bdd77d"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("4d30ea3f-980a-5243-b88d-3ab629c248d4"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("5238e92d-8af1-3a3e-7ada-e005537e89b0"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("6109438b-666b-c9ec-b56d-b3826c27e0f7"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("621b724b-3f07-a045-902c-e3742108cd46"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("6454f469-be9d-6322-23cc-67d0c099bf33"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("710cf9d8-d056-c475-f3fd-63ef9f64bf2b"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("7207671f-3e58-dec0-c03c-256591af4c2f"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("792de7db-6f24-f089-86c7-bebcf98f0fbb"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("8a36cd1b-803d-1e1f-bd56-98b59b75a5de"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("95f0c29e-53f9-5810-0c06-be2f013ed4c0"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("95fa54d7-c059-2560-77ef-5580ce321834"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("96874e72-413b-1374-d3f9-65a348bc5f1c"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("a0e6a4fe-ce63-744f-46f0-f043ffc9f52a"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("a1b99574-afa9-e011-da32-a4b56a6e8aa3"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("ae09b375-68a5-cf77-c7a9-8a797517f359"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("cd3226c5-c36a-b664-fe33-98299080ff6d"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("f42751c1-e9eb-37ca-e52d-9de6b1ec1595"),
                column: "actions",
                value: new List<string> { "READ", "CREATE", "UPDATE", "DELETE" });
        }
    }
}
