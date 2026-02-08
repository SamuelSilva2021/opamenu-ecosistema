using Authenticator.API.Core.Domain.Api;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;

namespace Authenticator.API.Core.Application.Interfaces.Auth
{
    public interface IUserValidation
    {
        IList<ErrorDTO> LoginValidation(UserAccountEntity user, string password, List<string> roles);
        IList<ErrorDTO> LoginValidationAccessControl(UserAccountEntity user, string password, List<string> roles);
    }
}
