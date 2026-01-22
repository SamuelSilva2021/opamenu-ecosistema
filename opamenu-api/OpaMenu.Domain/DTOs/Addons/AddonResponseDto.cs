namespace OpaMenu.Domain.DTOs.Addons
{
    /// <summary>
    /// DTO de resposta para adicional individual
    /// </summary>
    public class AddonResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public Guid AddonGroupId { get; set; }
        public bool IsActive { get; set; }
    }
}
