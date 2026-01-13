using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using Authenticator.API.Core.Application.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using Authenticator.API.Infrastructure.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.UserAccount;

/// <summary>
/// Implementação específica do repositório para contas de usuário
/// Herda de Repository{UserAccountEntity} para ter acesso aos métodos CRUD básicos
/// </summary>
public class UserAccountsRepository(
    IDbContextProvider dbContextProvider,
    IMemoryCache cache,
    ILogger<UserAccountsRepository> logger) : BaseRepository<UserAccountEntity>(dbContextProvider), IUserAccountsRepository
{
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<UserAccountsRepository> _logger = logger;

    /// <summary>
    /// Busca um usuário pelo email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>usuário encontrado ou null</returns>
    public async Task<UserAccountEntity?> GetByEmailAsync(string email)
    {
        var cacheKey = $"user_email_{email.ToLowerInvariant()}";

        if (_cache.TryGetValue(cacheKey, out UserAccountEntity? cachedUser))
            return cachedUser;

        var user = await FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.DeletedAt == null);

        if (user != null)
            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(15));

        return user;
    }

    /// <summary>
    /// Busca um usuário pelo username
    /// </summary>
    /// <param name="username">Nome de usuário</param>
    /// <returns>usuário encontrado ou null</returns>
    public async Task<UserAccountEntity?> GetByUsernameAsync(string username)
    {
        var cacheKey = $"user_username_{username.ToLowerInvariant()}";

        if (_cache.TryGetValue(cacheKey, out UserAccountEntity? cachedUser))
        {
            _logger.LogDebug("usuário encontrado no cache para username: {Username}", username);
            return cachedUser;
        }

        var user = await FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.DeletedAt == null);

        if (user != null)
            _cache.Set(cacheKey, user, TimeSpan.FromMinutes(15));

        return user;
    }

    /// <summary>
    /// Busca um usuário pelo email ou username
    /// </summary>
    /// <param name="emailOrUsername">Email ou username</param>
    /// <returns>usuário encontrado ou null</returns>
    public async Task<UserAccountEntity?> GetByEmailOrUsernameAsync(string emailOrUsername)
    {
        return await FirstOrDefaultAsync(u =>
            (u.Email.ToLower() == emailOrUsername.ToLower() ||
             u.Username.ToLower() == emailOrUsername.ToLower()) &&
            u.DeletedAt == null);
    }

    /// <summary>
    /// Verifica se um email já está em uso
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <param name="excludeUserId">ID do usuário a ser exclusão da verificação (para updates)</param>
    /// <returns>True se o email já existe</returns>
    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null)
    {
        if (excludeUserId.HasValue)
        {
            return await AnyAsync(u => u.Email.ToLower() == email.ToLower() &&
                                      u.Id != excludeUserId.Value &&
                                      u.DeletedAt == null);
        }

        return await AnyAsync(u => u.Email.ToLower() == email.ToLower() && u.DeletedAt == null);
    }

    /// <summary>
    /// Verifica se um username já está em uso
    /// </summary>
    /// <param name="username">Username a ser verificado</param>
    /// <param name="excludeUserId">ID do usuário a ser exclusão da verificação (para updates)</param>
    /// <returns>True se o username já existe</returns>
    public async Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null)
    {
        if (excludeUserId.HasValue)
        {
            return await AnyAsync(u => u.Username.ToLower() == username.ToLower() &&
                                      u.Id != excludeUserId.Value &&
                                      u.DeletedAt == null);
        }

        return await AnyAsync(u => u.Username.ToLower() == username.ToLower() && u.DeletedAt == null);
    }

    /// <summary>
    /// Busca usuários ativos por tenant
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <returns>Lista de usuários ativos do tenant</returns>
    public async Task<IEnumerable<UserAccountEntity>> GetActiveUsersByTenantAsync(Guid tenantId)
    {
        return await FindAsync(u => u.TenantId == tenantId &&
                                   u.Status == EUserAccountStatus.Ativo &&
                                   u.DeletedAt == null);
    }

    /// <summary>
    /// Busca usuários por tenant com paginaÃ§Ã£o
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="pageNumber">Número da página</param>
    /// <param name="pageSize">Tamanho da página</param>
    /// <returns>Lista paginada de usuários do tenant</returns>
    public async Task<IEnumerable<UserAccountEntity>> GetUsersByTenantPagedAsync(Guid tenantId, int pageNumber, int pageSize) =>
        await GetPagedAsync(u => u.TenantId == tenantId && u.DeletedAt == null, pageNumber, pageSize);

    public async Task<IEnumerable<UserAccountEntity>> GetUsersPagedAsync(int pageNumber, int pageSize) => 
        await GetPagedAsync(u => u.DeletedAt == null, pageNumber, pageSize);

    /// <summary>
    /// Atualiza a data do Ãºltimo login do usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Task</returns>
    public async Task UpdateLastLoginAsync(Guid userId)
    {

        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(user);

            _cache.Remove($"user_email_{user.Email.ToLowerInvariant()}");
            _cache.Remove($"user_username_{user.Username.ToLowerInvariant()}");
        }
    }

    /// <summary>
    /// Define o token de reset de senha para um usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="resetToken">Token de reset</param>
    /// <param name="expiresAt">Data de expiraÃ§Ã£o do token</param>
    /// <returns>Task</returns>
    public async Task SetPasswordResetTokenAsync(Guid userId, string resetToken, DateTime expiresAt)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.PasswordResetToken = resetToken;
            user.PasswordResetExpiresAt = expiresAt;
            user.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(user);
        }
    }

    /// <summary>
    /// Busca usuário pelo token de reset de senha vÃ¡lido
    /// </summary>
    /// <param name="resetToken">Token de reset</param>
    /// <returns>usuário encontrado ou null</returns>
    public async Task<UserAccountEntity?> GetByValidPasswordResetTokenAsync(string resetToken) =>
        await FirstOrDefaultAsync(u => u.PasswordResetToken == resetToken &&
                                             u.PasswordResetExpiresAt > DateTime.UtcNow &&
                                             u.DeletedAt == null);

    /// <summary>
    /// Limpa o token de reset de senha do usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Task</returns>
    public async Task ClearPasswordResetTokenAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.PasswordResetToken = null;
            user.PasswordResetExpiresAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(user);
        }
    }

    /// <summary>
    /// Ativa ou desativa um usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="isActive">Status ativo</param>
    /// <returns>Task</returns>
    public async Task SetUserActiveStatusAsync(Guid userId, bool isActive)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.Status = EUserAccountStatus.Ativo;
            user.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(user);

            _cache.Remove($"user_email_{user.Email.ToLowerInvariant()}");
            _cache.Remove($"user_username_{user.Username.ToLowerInvariant()}");
        }
    }

    /// <summary>
    /// Marca o email como verificado
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Task</returns>
    public async Task MarkEmailAsVerifiedAsync(Guid userId)
    {
        var user = await GetByIdAsync(userId);
        if (user != null)
        {
            user.IsEmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(user);

            // Invalidar cache
            _cache.Remove($"user_email_{user.Email.ToLowerInvariant()}");
            _cache.Remove($"user_username_{user.Username.ToLowerInvariant()}");
        }
    }
}


