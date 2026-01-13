using Authenticator.API.Core.Domain.Authentication;

namespace Authenticator.API.Core.Application.Interfaces.Auth
{
    /// <summary>
    /// Interface para obter o contexto do usuário autenticado
    /// </summary>
    public interface IUserContext
    {
        AuthenticatedUser? CurrentUser { get; }
    }
}
