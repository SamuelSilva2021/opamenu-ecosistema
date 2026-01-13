using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;

namespace Authenticator.API.Core.Application.Interfaces;

/// <summary>
/// Interface especÃ­fica para operaÃ§Ãµes de repositÃ³rio de contas de usuÃ¡rio
/// Herda de IBaseRepository para ter acesso aos mÃ©todos CRUD bÃ¡sicos
/// </summary>
public interface IUserAccountsRepository : IBaseRepository<UserAccountEntity>
{
    /// <summary>
    /// Busca um usuÃ¡rio pelo email
    /// </summary>
    /// <param name="email">Email do usuÃ¡rio</param>
    /// <returns>UsuÃ¡rio encontrado ou null</returns>
    Task<UserAccountEntity?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca um usuÃ¡rio pelo username
    /// </summary>
    /// <param name="username">Nome de usuÃ¡rio</param>
    /// <returns>UsuÃ¡rio encontrado ou null</returns>
    Task<UserAccountEntity?> GetByUsernameAsync(string username);

    /// <summary>
    /// Busca um usuÃ¡rio pelo email ou username
    /// </summary>
    /// <param name="emailOrUsername">Email ou username</param>
    /// <returns>UsuÃ¡rio encontrado ou null</returns>
    Task<UserAccountEntity?> GetByEmailOrUsernameAsync(string emailOrUsername);

    /// <summary>
    /// Verifica se um email jÃ¡ estÃ¡ em uso
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <param name="excludeUserId">ID do usuÃ¡rio a ser excluÃ­do da verificaÃ§Ã£o (para updates)</param>
    /// <returns>True se o email jÃ¡ existe</returns>
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null);

    /// <summary>
    /// Verifica se um username jÃ¡ estÃ¡ em uso
    /// </summary>
    /// <param name="username">Username a ser verificado</param>
    /// <param name="excludeUserId">ID do usuÃ¡rio a ser excluÃ­do da verificaÃ§Ã£o (para updates)</param>
    /// <returns>True se o username jÃ¡ existe</returns>
    Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null);

    /// <summary>
    /// Busca usuÃ¡rios ativos por tenant
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <returns>Lista de usuÃ¡rios ativos do tenant</returns>
    Task<IEnumerable<UserAccountEntity>> GetActiveUsersByTenantAsync(Guid tenantId);

    /// <summary>
    /// Busca usuÃ¡rios por tenant com paginaÃ§Ã£o
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="pageNumber">NÃºmero da pÃ¡gina</param>
    /// <param name="pageSize">Tamanho da pÃ¡gina</param>
    /// <returns>Lista paginada de usuÃ¡rios do tenant</returns>
    Task<IEnumerable<UserAccountEntity>> GetUsersByTenantPagedAsync(Guid tenantId, int pageNumber, int pageSize);

    /// <summary>
    /// Atualiza a data do Ãºltimo login do usuÃ¡rio
    /// </summary>
    /// <param name="userId">ID do usuÃ¡rio</param>
    /// <returns>Task</returns>
    Task UpdateLastLoginAsync(Guid userId);

    /// <summary>
    /// Define o token de reset de senha para um usuÃ¡rio
    /// </summary>
    /// <param name="userId">ID do usuÃ¡rio</param>
    /// <param name="resetToken">Token de reset</param>
    /// <param name="expiresAt">Data de expiraÃ§Ã£o do token</param>
    /// <returns>Task</returns>
    Task SetPasswordResetTokenAsync(Guid userId, string resetToken, DateTime expiresAt);

    /// <summary>
    /// Busca usuÃ¡rio pelo token de reset de senha vÃ¡lido
    /// </summary>
    /// <param name="resetToken">Token de reset</param>
    /// <returns>UsuÃ¡rio encontrado ou null</returns>
    Task<UserAccountEntity?> GetByValidPasswordResetTokenAsync(string resetToken);

    /// <summary>
    /// Limpa o token de reset de senha do usuÃ¡rio
    /// </summary>
    /// <param name="userId">ID do usuÃ¡rio</param>
    /// <returns>Task</returns>
    Task ClearPasswordResetTokenAsync(Guid userId);

    /// <summary>
    /// Ativa ou desativa um usuÃ¡rio
    /// </summary>
    /// <param name="userId">ID do usuÃ¡rio</param>
    /// <param name="isActive">Status ativo</param>
    /// <returns>Task</returns>
    Task SetUserActiveStatusAsync(Guid userId, bool isActive);

    /// <summary>
    /// Marca o email como verificado
    /// </summary>
    /// <param name="userId">ID do usuÃ¡rio</param>
    /// <returns>Task</returns>
    Task MarkEmailAsVerifiedAsync(Guid userId);
}
