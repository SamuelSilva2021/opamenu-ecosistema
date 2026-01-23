using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;

namespace OpaMenu.Application.Services.Interfaces.Opamenu;

/// <summary>
/// Interface para gerenciamento do serviço de cupons.
/// </summary>
public interface ICouponService
{
    /// <summary>
    /// Busca todos os cupons.
    /// </summary>
    /// <returns></returns>
    Task<ResponseDTO<IEnumerable<CouponDto>>> GetAllAsync();
    /// <summary>
    /// Busca um cupom pelo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ResponseDTO<CouponDto?>> GetByIdAsync(Guid id);
    /// <summary>
    /// Cria um novo cupom.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<ResponseDTO<CouponDto>> CreateAsync(CreateCouponRequestDto dto);
    /// <summary>
    /// Atualiza um cupom existente.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<ResponseDTO<CouponDto>> UpdateAsync(Guid id, UpdateCouponRequestDto dto);
    /// <summary>
    /// Deleta um cupom pelo ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ResponseDTO<bool>> DeleteAsync(Guid id);
    /// <summary>
    /// Busca todos os cupons ativos para a loja.
    /// </summary>
    /// <returns></returns>
    Task<ResponseDTO<IEnumerable<CouponDto>>> GetActiveCouponsForStorefrontAsync();
    /// <summary>
    /// Valida um cupom pelo código e valor do pedido.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="orderValue"></param>
    /// <returns></returns>
    Task<ResponseDTO<CouponDto?>> ValidateCouponAsync(string code, decimal orderValue);

    #region Métodos publicos adicionais
    /// <summary>
    /// Busca todos os cupons ativos pelo slug da loja.
    /// </summary>
    /// <param name="slug"></param>
    /// <returns></returns>
    Task<ResponseDTO<IEnumerable<CouponDto>>> GetActiveCouponsBySlugAsync(string slug);
    /// <summary>
    /// Valida um cupom pelo slug da loja, código e valor do pedido.
    /// </summary>
    /// <param name="slug"></param>
    /// <param name="code"></param>
    /// <param name="orderValue"></param>
    /// <returns></returns>
    Task<ResponseDTO<CouponDto?>> ValidateCouponBySlugAsync(string slug, string code, decimal orderValue);
    #endregion
}
