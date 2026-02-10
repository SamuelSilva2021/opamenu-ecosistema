using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class AddCollaboratorEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "driver_id",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "driver_id1",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "collaborators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    type = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    user_account_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_collaborators", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_driver_id1",
                table: "orders",
                column: "driver_id1");

            migrationBuilder.CreateIndex(
                name: "IX_collaborators_tenant_id",
                table: "collaborators",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_collaborators_type",
                table: "collaborators",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_collaborators_user_account_id",
                table: "collaborators",
                column: "user_account_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_collaborators_driver_id1",
                table: "orders",
                column: "driver_id1",
                principalTable: "collaborators",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_collaborators_driver_id1",
                table: "orders");

            migrationBuilder.DropTable(
                name: "collaborators");

            migrationBuilder.DropIndex(
                name: "IX_orders_driver_id1",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "driver_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "driver_id1",
                table: "orders");
        }
    }
}
