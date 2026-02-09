using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OpaMenu.Infrastructure.Shared.Migrations.Opamenu
{
    /// <inheritdoc />
    public partial class RefactorOrderStatusFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migrar status dos pedidos (String based)
            migrationBuilder.Sql(@"
                UPDATE orders SET status = 
                CASE 
                    WHEN status = 'Confirmed' THEN 'Preparing'
                    ELSE status
                END;");

            // Migrar status dos itens de pedido (Integer based)
            migrationBuilder.Sql(@"
                UPDATE order_items SET status = 
                CASE 
                    WHEN status = 2 THEN 1 -- Preparing(old:2) -> Preparing(new:1)
                    WHEN status = 3 THEN 2 -- Ready
                    WHEN status = 4 THEN 3 -- OutForDelivery
                    WHEN status = 5 THEN 4 -- Delivered
                    WHEN status = 6 THEN 5 -- Cancelled
                    WHEN status = 7 THEN 6 -- Rejected
                    ELSE status
                END;");

            // Migrar status do histórico (Integer based)
            migrationBuilder.Sql(@"
                UPDATE order_status_histories SET status = 
                CASE 
                    WHEN status = 2 THEN 1 
                    WHEN status = 3 THEN 2 
                    WHEN status = 4 THEN 3 
                    WHEN status = 5 THEN 4 
                    WHEN status = 6 THEN 5 
                    WHEN status = 7 THEN 6 
                    ELSE status
                END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverter status
            migrationBuilder.Sql(@"
                UPDATE order_status_histories SET status = 
                CASE 
                    WHEN status = 6 THEN 7 
                    WHEN status = 5 THEN 6 
                    WHEN status = 4 THEN 5 
                    WHEN status = 3 THEN 4 
                    WHEN status = 2 THEN 3 
                    WHEN status = 1 THEN 2 
                    ELSE status
                END;");

            migrationBuilder.Sql(@"
                UPDATE order_items SET status = 
                CASE 
                    WHEN status = 6 THEN 7 
                    WHEN status = 5 THEN 6 
                    WHEN status = 4 THEN 5 
                    WHEN status = 3 THEN 4 
                    WHEN status = 2 THEN 3 
                    WHEN status = 1 THEN 2 
                    ELSE status
                END;");

            migrationBuilder.Sql(@"
                UPDATE orders SET status = 'Confirmed' WHERE status = 'Preparing'; -- Reversão aproximada
            ");
        }
    }
}
