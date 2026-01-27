using Microsoft.EntityFrameworkCore;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;
using System.Security.Cryptography;
using System.Text;

namespace OpaMenu.Infrastructure.Shared.Data.Context.Opamenu;

public static class OpamenuDbContextSeed
{
    private static readonly DateTime _seedDate = new(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc);

    public static void OpamenuSeed(this ModelBuilder modelBuilder)
    {
        // 1. Payment Methods (System Defaults)
        var pmCash = CreatePaymentMethod("money", "Dinheiro", "Pagamento em espécie", "fas fa-money-bill-wave", false, 1);
        var pmCreditCard = CreatePaymentMethod("credit_card", "Cartão de Crédito", "Pagamento via cartão de crédito", "fas fa-credit-card", true, 2);
        var pmDebitCard = CreatePaymentMethod("debit_card", "Cartão de Débito", "Pagamento via cartão de débito", "fas fa-credit-card", true, 3);
        var pmPix = CreatePaymentMethod("pix", "Pix", "Pagamento instantâneo via Pix", "fab fa-pix", true, 4);
        var pmVoucher = CreatePaymentMethod("meal_voucher", "Vale Refeição", "Pagamento via vale refeição", "fas fa-ticket-alt", true, 5);

        modelBuilder.Entity<PaymentMethodEntity>().HasData(pmCash, pmCreditCard, pmDebitCard, pmPix, pmVoucher);
    }

    private static PaymentMethodEntity CreatePaymentMethod(string slug, string name, string description, string iconUrl, bool isOnline, int displayOrder)
    {
        return new PaymentMethodEntity
        {
            Id = GenerateId($"PM_{slug}"),
            Slug = slug,
            Name = name,
            Description = description,
            IconUrl = iconUrl,
            IsOnline = isOnline,
            IsActive = true,
            DisplayOrder = displayOrder,
            CreatedAt = _seedDate,
            UpdatedAt = _seedDate
        };
    }

    private static Guid GenerateId(string input)
    {
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}
