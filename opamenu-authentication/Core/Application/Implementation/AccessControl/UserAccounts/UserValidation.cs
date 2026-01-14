using Authenticator.API.Core.Application.Interfaces.Auth;
using Authenticator.API.Core.Domain.Api;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;

namespace Authenticator.API.Core.Application.Implementation.AccessControl.UserAccounts
{
    public class UserValidation: IUserValidation
    {
        public IList<ErrorDTO> LoginValidation(UserAccountEntity user, string password, List<string> roles)
        {
            var isSuperAdmin = roles.Select(x => x.Contains("SUPER_ADMIN")).Any();

            var erros = new List<ErrorDTO>();
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                erros.Add(new ErrorDTO { Message = "Credenciais inválidas", Code = "INVALID_CREDENTIALS" });

            if (user!.Status != EUserAccountStatus.Ativo)
                erros.Add(new ErrorDTO { Message = "Usuário inativo", Code = "INATIVE_USER", Details = ["Entre em contato com o suporte para mais detalhes"]});

            if(!isSuperAdmin && user.TenantId == null)
                erros.Add(new ErrorDTO { Message = "Usuário sem loja associada", Code = "USER_WITHOUT_TENANT", Details = ["É preciso estar vinculado a uma loja para acessar o painel."] });

            return erros;
        }
    }
}
