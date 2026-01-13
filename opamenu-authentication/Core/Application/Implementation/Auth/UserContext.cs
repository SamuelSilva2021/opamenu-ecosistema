using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.Authentication;
using Authenticator.API.Infrastructure.Helpers;

namespace Authenticator.API.Core.Application.Implementation.Auth
{
    /// <summary>
    /// Implementação do contexto do usuário autenticado
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Usuário autenticado atual
        /// </summary>
        public AuthenticatedUser? CurrentUser => 
            _httpContextAccessor.HttpContext?.User.ToAuthenticatedUser();
    }
}
