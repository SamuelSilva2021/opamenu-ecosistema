using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Desktop.Migrations
{
    /// <inheritdoc />
    public partial class PrintEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrinterMappings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Destination = table.Column<int>(type: "INTEGER", nullable: false),
                    ConnectionType = table.Column<int>(type: "INTEGER", nullable: false),
                    PaperSize = table.Column<int>(type: "INTEGER", nullable: false),
                    Profile = table.Column<int>(type: "INTEGER", nullable: false),
                    WindowsPrinterName = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: true),
                    Port = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrinterMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrintJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastAttemptAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Destination = table.Column<int>(type: "INTEGER", nullable: false),
                    PayloadType = table.Column<string>(type: "TEXT", nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Attempts = table.Column<int>(type: "INTEGER", nullable: false),
                    LastError = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrinterMappings_Destination",
                table: "PrinterMappings",
                column: "Destination",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrintJobs_Status",
                table: "PrintJobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PrintJobs_Status_CreatedAt",
                table: "PrintJobs",
                columns: new[] { "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrinterMappings");

            migrationBuilder.DropTable(
                name: "PrintJobs");
        }
    }
}
