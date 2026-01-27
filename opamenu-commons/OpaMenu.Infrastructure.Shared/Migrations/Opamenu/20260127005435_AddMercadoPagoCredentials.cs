using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class AddMercadoPagoCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "access_token",
                table: "tenant_payment_configs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "public_key",
                table: "tenant_payment_configs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "access_token",
                table: "tenant_payment_configs");

            migrationBuilder.DropColumn(
                name: "public_key",
                table: "tenant_payment_configs");
        }
    }
}
