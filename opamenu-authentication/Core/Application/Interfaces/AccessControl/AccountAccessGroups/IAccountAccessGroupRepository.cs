using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace Authenticator.API.Core.Application.Interfaces.AccessControl.AccountAccessGroups
{
    /// <summary>
    /// RepositÃ³rio para gerenciar vÃ­nculos de grupos de acesso com contas de usuÃ¡rio
    /// </summary>
    public interface IAccountAccessGroupRepository : IBaseRepository<AccountAccessGroupEntity>
    {
        /// <summary>
        /// Lista vÃ­nculos ativos de grupos de acesso de um usuÃ¡rio
        /// </summary>
        Task<IEnumerable<AccountAccessGroupEntity>> GetByUserAsync(Guid userId);

        /// <summary>
        /// Atribui (ou reativa) grupos de acesso a um usuÃ¡rio
        /// </summary>
        Task AssignGroupsAsync(Guid userId, IEnumerable<Guid> accessGroupIds, Guid? grantedBy, DateTime? expiresAt);

        /// <summary>
        /// Revoga (desativa) um vÃ­nculo de grupo de acesso de um usuÃ¡rio
        /// </summary>
        Task<bool> RevokeAsync(Guid userId, Guid accessGroupId);
    }
}
