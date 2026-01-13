using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;

namespace Authenticator.API.Core.Application.Interfaces;

/// <summary>
/// Interface específica para UserAccounts
/// Herdada de IBaseRepository para operações CRUD básicas
/// </summary>
public interface IUserAccountsRepository : IBaseRepository<UserAccountEntity>
{
    /// <summary>
    /// Busca um usuário pelo email
    /// </summary>
    /// <param name="email">Email usuário</param>
    /// <returns>usuário encontrado ou null</returns>
    Task<UserAccountEntity?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca um usuário pelo username
    /// </summary>
    /// <param name="username">Nome de usuário</param>
    /// <returns>usuário encontrado ou null</returns>
    Task<UserAccountEntity?> GetByUsernameAsync(string username);

    /// <summary>
    /// Busca um usuário pelo email ou username
    /// </summary>
    /// <param name="emailOrUsername">Email ou username</param>
    /// <returns>usuário encontrado ou null</returns>
    Task<UserAccountEntity?> GetByEmailOrUsernameAsync(string emailOrUsername);

    /// <summary>
    /// Verifica se um email já está em uso
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <param name="excludeUserId">ID do usuário a ser exclusão da verificação (para updates)</param>
    /// <returns>True se o email já existe</returns>
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);

    /// <summary>
    /// Verifica se um username já está em uso
    /// </summary>
    /// <param name="username">Username a ser verificado</param>
    /// <param name="excludeUserId">ID do usuário a ser exclusão da verificação (para updates)</param>
    /// <returns>True se o username já existe</returns>
    Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null);

    /// <summary>
    /// Busca usuários ativos por tenant
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <returns>Lista de usuários ativos do tenant</returns>
    Task<IEnumerable<UserAccountEntity>> GetActiveUsersByTenantAsync(Guid tenantId);

    /// <summary>
    /// Busca usuários por tenant com paginaÃ§Ã£o
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="pageNumber">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de usuários do tenant</returns>
    Task<IEnumerable<UserAccountEntity>> GetUsersByTenantPagedAsync(Guid tenantId, int pageNumber, int pageSize);

    /// <summary>
    /// Busca todos os usuários com paginação
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<IEnumerable<UserAccountEntity>> GetUsersPagedAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Atualiza a data do último login do usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Task</returns>
    Task UpdateLastLoginAsync(Guid userId);

    /// <summary>
    /// Define o token de reset de senha para um usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="resetToken">Token de reset</param>
    /// <param name="expiresAt">Data de expiraÃ§Ã£o do token</param>
    /// <returns>Task</returns>
    Task SetPasswordResetTokenAsync(Guid userId, string resetToken, DateTime expiresAt);

    /// <summary>
    /// Busca usuário pelo token de reset de senha válido
    /// </summary>
    /// <param name="resetToken">Token de reset</param>
    /// <returns>usuário encontrado ou null</returns>
    Task<UserAccountEntity?> GetByValidPasswordResetTokenAsync(string resetToken);

    /// <summary>
    /// Limpa o token de reset de senha do usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Task</returns>
    Task ClearPasswordResetTokenAsync(Guid userId);

    /// <summary>
    /// Ativa ou desativa um usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="isActive">Status ativo</param>
    /// <returns>Task</returns>
    Task SetUserActiveStatusAsync(Guid userId, bool isActive);

    /// <summary>
    /// Marca o email como verificado
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Task</returns>
    Task MarkEmailAsVerifiedAsync(Guid userId);
}
