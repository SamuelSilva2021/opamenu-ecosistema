using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.AccessControl
{
    /// <inheritdoc />
    public partial class UpdateAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "access_group",
                keyColumn: "id",
                keyValue: new Guid("2987c0d9-519f-6245-c920-889f697a658c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "access_group",
                keyColumn: "id",
                keyValue: new Guid("aee4edda-f145-f4c7-7edb-20a33888df4b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "group_type",
                keyColumn: "id",
                keyValue: new Guid("4dcb20a2-b350-3384-25a1-27d98e79d6ac"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "group_type",
                keyColumn: "id",
                keyValue: new Guid("921ceb40-24ff-ed9e-7e0e-bf33042e9272"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("13482be3-f205-837c-cceb-e28f14015000"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("20bf5030-e615-8586-a0b1-c624ce259deb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("3ad894aa-3bdf-db04-9715-a1134c4c8825"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("4a8880e0-1906-49be-5b11-ad80ddadf8ca"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("6a8409cb-b54d-a80e-afce-e0ff0454f375"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("6f331ece-a4ea-74f6-158b-c7a5be39cc41"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("9b464720-4a29-d649-f64a-7b8ebec95bcf"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("a7060970-7bc8-8e98-4566-e00676be1578"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("aa19bde8-48c1-7dd4-1f18-bd177952614d"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("d8265dac-ea6a-353a-5bd5-065d519df46b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("de03bd4d-f7e9-0b1b-f3a3-2b5ea9ee30b8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("f415b1d6-6989-b41d-dcb8-4a0a824fc7f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "operation",
                keyColumn: "id",
                keyValue: new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "operation",
                keyColumn: "id",
                keyValue: new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "operation",
                keyColumn: "id",
                keyValue: new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "operation",
                keyColumn: "id",
                keyValue: new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("891db09d-58af-160d-f10b-c17b93a6e834"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("0b33e73e-832d-f7cf-129b-6e320e49ef20"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("1ce9de28-af8a-05d9-8775-9c26be0bf911"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("1e739438-54f3-e828-9bbd-bc0133fa960e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("243ac639-ef08-0bab-da93-621eb417a814"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("277512a5-1a9a-e351-d80d-21d53d72af34"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("31585bfb-3d5f-ade5-e06f-1916f7df6683"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("4f14a7f6-7ee3-97ab-1a12-dbcb79cbbf9f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("56f703c6-af2a-165c-f813-f0b731769f78"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("598f5406-6414-2905-5512-e1f22d8f570f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("5c4490c5-5d53-7ad4-af3c-6552895f6cb2"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("5e07e56b-9f39-773e-aaf8-1d65cfa98f88"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("5e7ae7c5-dd9b-8097-7cc3-f02ed6a0aa35"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("5fa84bb3-5842-897c-c69e-b2494ea75e9f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("60e25e4d-24fe-9fc7-b9c3-93817ee3fa27"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("621bd31f-5135-0337-0445-6356c52cb520"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("7bf9f602-f57e-0cb4-076f-32b77e025f2b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("7fe097e6-a043-43e3-a63a-37767762d13a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("8035f3a2-608e-dd8b-28eb-bbc4a624e686"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("82178c5f-dcb9-7adf-83a5-bc4a9d4b8950"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("83157f49-463a-4c4e-3f4f-0c34304064cb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("8b953ae4-f669-1ddd-091d-5e76bf18a0b9"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("8bb35719-b0dd-6b5a-fcca-6671beb129c1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("8e7f3c7c-d1ed-7e71-0fe6-6a6f0e90abae"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("94a68158-a871-8c40-22c1-3e8f1e65dd0f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("97868498-f4c2-82d0-141c-03a7efdc1394"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("a44cb1d7-9c1b-ce3a-0d8a-0b47779029dc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("b38d3e52-204e-7595-54c1-7c3514de2f50"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("bde69f79-41f1-1e23-6332-6c6473153c9c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("c46a3806-1202-0544-2271-06e70cdb4bb1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("c4b7da8d-3d9b-ba07-9ea5-4a9a02006021"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("c6df2e95-92ac-7466-18bc-52b113ec77af"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("c9b02319-a31d-11ce-845f-17b9cb213096"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("ca01c561-f141-a2d6-c6d9-780ae4b45158"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("cb18a34a-a3ce-9f50-f5ef-d833b0c11287"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("ce3570dd-dfac-8922-3a9d-943b37710f28"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("d27bdf7b-a132-a1ea-4485-401877ea4956"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("d533bf9d-f97f-c3b8-a7d5-0bc4a4ef9391"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("d596bb91-4de6-2530-b83b-b4bf6756c4f6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("d9108a4f-9203-04ac-6464-0f2d32ff2efc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("dcac12d5-446f-2318-77b6-6a76028800af"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("def72cac-85db-b40d-f612-3f030810e2cb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("df5e9c46-2527-5224-854b-bb486082ae4e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("e39eabd0-9348-b57d-ccbe-9c2f9bd3927a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("e4ae6e94-c94c-0054-6eff-4c5ac2c80971"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("ed24312c-0c7a-111e-99d1-015e82fe696a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("f2affb89-72cf-75f9-05ee-31c5f1ecc55e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("feec90d0-ac9b-f282-94a1-9ccd30ebf69b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("ff716dd2-1c6d-ef20-1bbb-d6a8c77b5fc2"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: new Guid("724add9f-febd-c615-19db-8e526968da25"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_access_group",
                keyColumn: "id",
                keyValue: new Guid("c0cb54ff-a183-7932-d250-f75da02e1c75"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_access_group",
                keyColumn: "id",
                keyValue: new Guid("dcececfe-1dc2-7d14-910b-408d435c5582"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("0a0ad000-30e2-e219-d9ac-84394c6e5bf9"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("343209af-3f1e-62cd-c5e2-ebd0d93c896e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("382fca83-7d5e-f15c-64fb-d58c212e3774"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("415aa751-2810-90b6-a342-0b197cd5f43c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("61b0047e-df45-a1ff-9201-e85fbb80de8d"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("6a6c4a8f-39f5-b674-0c75-8c6c84783d83"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("7ebd8351-6981-6177-eb12-2335bf96bcbd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("8bd67bc4-2d66-70e3-10c2-08c55be8d34e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("8d991424-decf-2ef1-3d5d-9578c4053bbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("93a44a8c-a499-bc6d-b156-cd02044f85eb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("973f191b-727f-cec6-95a3-861b4065fb45"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("9d088010-f633-94b6-3558-e59530b28ac7"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("a1956b56-243d-3659-0f17-5f72c912f74c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("abbcba5c-d2a2-5bd1-1ad8-2385ebd8e11e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("ac65d5ac-81fe-288a-13fd-d0ebdda9731a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("b2aa0e1c-a62d-9dba-aa48-1b71209e87e6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("b66b445c-9aa5-7a4a-dd4a-767ac959e161"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("be9e1f49-5f61-c0dc-4c97-a011ba164b88"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("c275fb34-e87e-1130-6a98-daba72f032f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("d4bf4b07-95cd-5c10-daff-8844445ac205"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("db3df29d-c3a7-a4c1-54c0-38362e8508c9"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("e0e5875f-dd7b-a8d2-8711-a49d4a0a0a0d"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("e43ebb58-9c3a-493a-0ea1-a65374863e96"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("f22f0f21-eda0-5a0d-9932-ae3408c4d73b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "user_account",
                columns: new[] { "id", "created_at", "DeletedAt", "email", "first_name", "is_email_verified", "last_login_at", "last_name", "password_hash", "password_reset_expires_at", "password_reset_token", "phone_number", "status", "tenant_id", "updated_at", "username" },
                values: new object[] { new Guid("6bc3c728-4ffa-67f6-2ed9-d4f18594f4c0"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@opamenu.com.br", "System", true, null, "Admin", "$2a$11$uwqx.wvei6NWpS2ACqoUp.SwWooJcx9lZSx2ZthW2QCWkvHLbWhqi", null, null, null, "Ativo", null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "admin" });

            migrationBuilder.InsertData(
                table: "account_access_group",
                columns: new[] { "id", "access_group_id", "CreatedAt", "expires_at", "GrantedAt", "granted_by", "is_active", "UpdatedAt", "user_account_id" },
                values: new object[] { new Guid("f1e8f88f-6526-5bf8-6a70-c5238f40dd92"), new Guid("2987c0d9-519f-6245-c920-889f697a658c"), new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), null, true, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6bc3c728-4ffa-67f6-2ed9-d4f18594f4c0") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "account_access_group",
                keyColumn: "id",
                keyValue: new Guid("f1e8f88f-6526-5bf8-6a70-c5238f40dd92"));

            migrationBuilder.DeleteData(
                table: "user_account",
                keyColumn: "id",
                keyValue: new Guid("6bc3c728-4ffa-67f6-2ed9-d4f18594f4c0"));

            migrationBuilder.UpdateData(
                table: "access_group",
                keyColumn: "id",
                keyValue: new Guid("2987c0d9-519f-6245-c920-889f697a658c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "access_group",
                keyColumn: "id",
                keyValue: new Guid("aee4edda-f145-f4c7-7edb-20a33888df4b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "group_type",
                keyColumn: "id",
                keyValue: new Guid("4dcb20a2-b350-3384-25a1-27d98e79d6ac"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "group_type",
                keyColumn: "id",
                keyValue: new Guid("921ceb40-24ff-ed9e-7e0e-bf33042e9272"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("13482be3-f205-837c-cceb-e28f14015000"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("20bf5030-e615-8586-a0b1-c624ce259deb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("3ad894aa-3bdf-db04-9715-a1134c4c8825"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("4a8880e0-1906-49be-5b11-ad80ddadf8ca"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("6a8409cb-b54d-a80e-afce-e0ff0454f375"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("6f331ece-a4ea-74f6-158b-c7a5be39cc41"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("9b464720-4a29-d649-f64a-7b8ebec95bcf"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("a7060970-7bc8-8e98-4566-e00676be1578"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("aa19bde8-48c1-7dd4-1f18-bd177952614d"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("d8265dac-ea6a-353a-5bd5-065d519df46b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("de03bd4d-f7e9-0b1b-f3a3-2b5ea9ee30b8"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "module",
                keyColumn: "id",
                keyValue: new Guid("f415b1d6-6989-b41d-dcb8-4a0a824fc7f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "operation",
                keyColumn: "id",
                keyValue: new Guid("0c6e99a2-af92-4ff8-80b7-332b1636479f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "operation",
                keyColumn: "id",
                keyValue: new Guid("830f881f-76d6-79db-c78a-5f4952ff9a4c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "operation",
                keyColumn: "id",
                keyValue: new Guid("a9cc2de8-8f88-b419-87ad-9d152f92a84b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "operation",
                keyColumn: "id",
                keyValue: new Guid("d1183fa8-5cae-7b65-cd00-3d1db4a55fac"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("2e6af181-5d42-e5aa-4b4b-d5248007b2e0"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("3ffe4f75-0bb0-4ad7-703f-518f5dfe811d"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("4e5ce036-8fe9-604a-3467-1b1d52a2d062"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("57918dc5-6eda-b645-0cb5-cf46df9363bc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("6ed448d2-c4b5-3cd5-e185-c6793bd3b152"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("7765f93c-ad43-8f7e-d12e-ffd3718e09c9"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("82e4ee38-f1df-4c99-3bb0-a8b58ba4a22e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("891db09d-58af-160d-f10b-c17b93a6e834"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("8b5cc96d-89b7-43a0-f90a-117e24243bf4"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("b1558ef2-06dd-d6aa-8859-955e696b406b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("d6afa7ac-9c27-8d3e-691f-188d47b0703e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission",
                keyColumn: "id",
                keyValue: new Guid("ff3110e4-d152-7a75-a688-afa08b77e035"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("0b33e73e-832d-f7cf-129b-6e320e49ef20"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("1ce9de28-af8a-05d9-8775-9c26be0bf911"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("1e739438-54f3-e828-9bbd-bc0133fa960e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("243ac639-ef08-0bab-da93-621eb417a814"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("277512a5-1a9a-e351-d80d-21d53d72af34"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("31585bfb-3d5f-ade5-e06f-1916f7df6683"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("4f14a7f6-7ee3-97ab-1a12-dbcb79cbbf9f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("56f703c6-af2a-165c-f813-f0b731769f78"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("598f5406-6414-2905-5512-e1f22d8f570f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("5c4490c5-5d53-7ad4-af3c-6552895f6cb2"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("5e07e56b-9f39-773e-aaf8-1d65cfa98f88"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("5e7ae7c5-dd9b-8097-7cc3-f02ed6a0aa35"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("5fa84bb3-5842-897c-c69e-b2494ea75e9f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("60e25e4d-24fe-9fc7-b9c3-93817ee3fa27"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("621bd31f-5135-0337-0445-6356c52cb520"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("7bf9f602-f57e-0cb4-076f-32b77e025f2b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("7fe097e6-a043-43e3-a63a-37767762d13a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("8035f3a2-608e-dd8b-28eb-bbc4a624e686"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("82178c5f-dcb9-7adf-83a5-bc4a9d4b8950"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("83157f49-463a-4c4e-3f4f-0c34304064cb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("8b953ae4-f669-1ddd-091d-5e76bf18a0b9"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("8bb35719-b0dd-6b5a-fcca-6671beb129c1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("8e7f3c7c-d1ed-7e71-0fe6-6a6f0e90abae"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("94a68158-a871-8c40-22c1-3e8f1e65dd0f"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("97868498-f4c2-82d0-141c-03a7efdc1394"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("a44cb1d7-9c1b-ce3a-0d8a-0b47779029dc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("b38d3e52-204e-7595-54c1-7c3514de2f50"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("bde69f79-41f1-1e23-6332-6c6473153c9c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("c46a3806-1202-0544-2271-06e70cdb4bb1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("c4b7da8d-3d9b-ba07-9ea5-4a9a02006021"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("c6df2e95-92ac-7466-18bc-52b113ec77af"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("c9b02319-a31d-11ce-845f-17b9cb213096"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("ca01c561-f141-a2d6-c6d9-780ae4b45158"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("cb18a34a-a3ce-9f50-f5ef-d833b0c11287"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("ce3570dd-dfac-8922-3a9d-943b37710f28"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("d27bdf7b-a132-a1ea-4485-401877ea4956"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("d533bf9d-f97f-c3b8-a7d5-0bc4a4ef9391"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("d596bb91-4de6-2530-b83b-b4bf6756c4f6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("d9108a4f-9203-04ac-6464-0f2d32ff2efc"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("dcac12d5-446f-2318-77b6-6a76028800af"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("def72cac-85db-b40d-f612-3f030810e2cb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("df5e9c46-2527-5224-854b-bb486082ae4e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("e39eabd0-9348-b57d-ccbe-9c2f9bd3927a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("e4ae6e94-c94c-0054-6eff-4c5ac2c80971"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("ed24312c-0c7a-111e-99d1-015e82fe696a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("f2affb89-72cf-75f9-05ee-31c5f1ecc55e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("feec90d0-ac9b-f282-94a1-9ccd30ebf69b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "permission_operation",
                keyColumn: "id",
                keyValue: new Guid("ff716dd2-1c6d-ef20-1bbb-d6a8c77b5fc2"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: new Guid("4534e34a-3a31-ef0c-2a17-b57ea1d45cb6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role",
                keyColumn: "id",
                keyValue: new Guid("724add9f-febd-c615-19db-8e526968da25"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_access_group",
                keyColumn: "id",
                keyValue: new Guid("c0cb54ff-a183-7932-d250-f75da02e1c75"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_access_group",
                keyColumn: "id",
                keyValue: new Guid("dcececfe-1dc2-7d14-910b-408d435c5582"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("0a0ad000-30e2-e219-d9ac-84394c6e5bf9"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("343209af-3f1e-62cd-c5e2-ebd0d93c896e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("382fca83-7d5e-f15c-64fb-d58c212e3774"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("415aa751-2810-90b6-a342-0b197cd5f43c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("61b0047e-df45-a1ff-9201-e85fbb80de8d"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("6a6c4a8f-39f5-b674-0c75-8c6c84783d83"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("7ebd8351-6981-6177-eb12-2335bf96bcbd"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("8bd67bc4-2d66-70e3-10c2-08c55be8d34e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("8d991424-decf-2ef1-3d5d-9578c4053bbb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("93a44a8c-a499-bc6d-b156-cd02044f85eb"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("973f191b-727f-cec6-95a3-861b4065fb45"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("9d088010-f633-94b6-3558-e59530b28ac7"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("a1956b56-243d-3659-0f17-5f72c912f74c"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("abbcba5c-d2a2-5bd1-1ad8-2385ebd8e11e"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("ac65d5ac-81fe-288a-13fd-d0ebdda9731a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("b2aa0e1c-a62d-9dba-aa48-1b71209e87e6"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("b66b445c-9aa5-7a4a-dd4a-767ac959e161"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("be9e1f49-5f61-c0dc-4c97-a011ba164b88"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("c275fb34-e87e-1130-6a98-daba72f032f1"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("d4bf4b07-95cd-5c10-daff-8844445ac205"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("db3df29d-c3a7-a4c1-54c0-38362e8508c9"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("e0e5875f-dd7b-a8d2-8711-a49d4a0a0a0d"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("e43ebb58-9c3a-493a-0ea1-a65374863e96"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "role_permission",
                keyColumn: "id",
                keyValue: new Guid("f22f0f21-eda0-5a0d-9932-ae3408c4d73b"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
