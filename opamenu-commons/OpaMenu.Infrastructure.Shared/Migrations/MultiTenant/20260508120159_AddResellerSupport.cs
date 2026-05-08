using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.MultiTenant
{
    /// <inheritdoc />
    public partial class AddResellerSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_tenant_id",
                table: "tenants",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "tenants",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "Cliente");

            migrationBuilder.AddColumn<string>(
                name: "category",
                table: "plans",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "Customer");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_parent_tenant_id",
                table: "tenants",
                column: "parent_tenant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_tenants_parent_tenant_id",
                table: "tenants",
                column: "parent_tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenants_tenants_parent_tenant_id",
                table: "tenants");

            migrationBuilder.DropIndex(
                name: "IX_tenants_parent_tenant_id",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "parent_tenant_id",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "type",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "category",
                table: "plans");
        }
    }
}
