namespace OpaMenu.Web.Models.DTOs
{
    /// <summary>
    /// Representa o item de um pedido que é um complemento (addon).
    /// </summary>
    public class OrderItemAddonDto
    {
        /// <summary>
        /// Identificador único do item de pedido complemento.
        /// </summary>
        public Guid AddonId { get; set; }
        /// <summary>
        /// Nome do complemento.
        /// </summary>
        public int Quantity { get; set; } = 1;
    }
}
