namespace OpaMenu.Domain.DTOs.PaymentMethod
{
    public record PaymentMethodResponseDto(
        int Id,
        string Name,
        string Slug,
        string? Description,
        string? IconUrl,
        bool IsActive,
        bool IsOnline,
        int DisplayOrder
    );
}
