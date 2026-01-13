using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccountAccessGroups
{
    /// <summary>
    /// Serviço de domínio para gerenciar vínculos de grupos de acesso com usuários
    /// </summary>
    public interface IAccountAccessGroupService
    {
        /// <summary>
        /// Lista grupos de acesso vinculados a um usuário
        /// </summary>
        Task<ResponseDTO<IEnumerable<AccessGroupDTO>>> GetUserAccessGroupsAsync(Guid userId);

        /// <summary>
        /// Atribui grupos de acesso a um usuário
        /// </summary>
        Task<ResponseDTO<bool>> AssignAccessGroupsAsync(Guid userId, AssignUserAccessGroupsDTO request);

        /// <summary>
        /// Revoga um grupo de acesso de um usuário
        /// </summary>
        Task<ResponseDTO<bool>> RevokeAccessGroupAsync(Guid userId, Guid accessGroupId);
    }
}