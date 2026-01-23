using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Menu;

namespace OpaMenu.Application.Services.Interfaces.Opamenu
{
    public interface IStorefrontService
    {
        /// <summary>
        /// Obtém todos os dados necessários para renderizar a loja (Tenant, Menu, Categorias)
        /// </summary>
        Task<ResponseDTO<MenuResponseDto>> GetStorefrontDataAsync(string slug);
    }
}
