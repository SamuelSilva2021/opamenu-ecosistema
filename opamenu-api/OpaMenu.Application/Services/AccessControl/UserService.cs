using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using System.Security.Cryptography;

namespace OpaMenu.Application.Services.AccessControl;

public sealed class UserService(
    IUserAccountRepository userAccountRepository,
    IRoleRepository roleRepository,
    IAccessGroupRepository accessGroupRepository,
    IAccountAccessGroupRepository accountAccessGroupRepository) : IUserService
{
    private readonly IUserAccountRepository _userAccountRepository = userAccountRepository;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
    private readonly IAccountAccessGroupRepository _accountAccessGroupRepository = accountAccessGroupRepository;

    public async Task<PagedResultDto<UserAccountDto>> GetUsersAsync(int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var (items, total) = await _userAccountRepository.GetPagedAsync(page, limit, search);
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        return new PagedResultDto<UserAccountDto>
        {
            Items = items.Select(u => ToDto(u)).ToList(),
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        };
    }

    public async Task<List<UserAccountDto>> GetActiveUsersAsync()
    {
        var users = await _userAccountRepository.GetActiveAsync();
        return users.Select(u => ToDto(u)).ToList();
    }

    public async Task<UserAccountDto?> GetByIdAsync(Guid id)
    {
        var user = await _userAccountRepository.GetByIdWithRoleAsync(id);
        return user == null ? null : ToDto(user);
    }

    public async Task<UserAccountDto?> CreateAsync(CreateUserAccountRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.ConfirmPassword) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
        {
            return null;
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return null;
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var existsEmail = await _userAccountRepository.EmailExistsAsync(email);
        if (existsEmail)
        {
            return null;
        }

        var usernameBase = email.Split('@')[0];
        var username = await GenerateUniqueUsernameAsync(usernameBase);

        if (request.RoleId.HasValue)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId.Value);
            if (role == null)
            {
                return null;
            }
        }

        var now = DateTime.UtcNow;
        var entity = new UserAccountEntity
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PhoneNumber = request.PhoneNumber,
            Status = EUserAccountStatus.Ativo,
            IsEmailVerified = false,
            RoleId = request.RoleId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userAccountRepository.AddAsync(entity);
        await _userAccountRepository.SaveChangesAsync();

        var roleName = await ResolveRoleNameAsync(entity.RoleId);

        return new UserAccountDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Username = entity.Username,
            Email = entity.Email,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            PhoneNumber = entity.PhoneNumber,
            Status = entity.Status.ToString(),
            IsEmailVerified = entity.IsEmailVerified,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            LastLoginAt = entity.LastLoginAt,
            FullName = (entity.FirstName + " " + entity.LastName).Trim(),
            RoleId = entity.RoleId,
            RoleName = roleName
        };
    }

    public async Task<(UserAccountDto? User, bool NotFound)> UpdateAsync(Guid id, UpdateUserAccountRequestDto request)
    {
        var entity = await _userAccountRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return (null, true);
        }

        if (request.Username != null)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return (null, false);
            }

            var newUsername = request.Username.Trim();
            var exists = await _userAccountRepository.UsernameExistsAsync(newUsername, excludeUserId: id);
            if (exists)
            {
                return (null, false);
            }

            entity.Username = newUsername;
        }

        if (request.Email != null)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return (null, false);
            }

            var newEmail = request.Email.Trim().ToLowerInvariant();
            var exists = await _userAccountRepository.EmailExistsAsync(newEmail, excludeUserId: id);
            if (exists)
            {
                return (null, false);
            }

            entity.Email = newEmail;
        }

        if (request.FirstName != null)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                return (null, false);
            }

            entity.FirstName = request.FirstName.Trim();
        }

        if (request.LastName != null)
        {
            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                return (null, false);
            }

            entity.LastName = request.LastName.Trim();
        }

        if (request.PhoneNumber != null)
        {
            entity.PhoneNumber = request.PhoneNumber;
        }

        if (request.TenantId.HasValue)
        {
            entity.TenantId = request.TenantId;
        }

        if (request.RoleId.HasValue)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId.Value);
            if (role == null)
            {
                return (null, false);
            }

            entity.RoleId = request.RoleId;
        }

        if (request.IsEmailVerified.HasValue)
        {
            entity.IsEmailVerified = request.IsEmailVerified.Value;
        }

        if (request.Status != null)
        {
            if (!Enum.TryParse<EUserAccountStatus>(request.Status, ignoreCase: true, out var status))
            {
                return (null, false);
            }

            entity.Status = status;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _userAccountRepository.SaveChangesAsync();

        var roleName = await ResolveRoleNameAsync(entity.RoleId);
        return (ToDto(entity, roleName), false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _userAccountRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return;
        }

        entity.DeletedAt = DateTime.UtcNow;
        entity.Status = EUserAccountStatus.Deletado;
        entity.UpdatedAt = DateTime.UtcNow;
        await _userAccountRepository.SaveChangesAsync();
    }

    public async Task<ResponseDTO<PagedResultDto<UserAccountDto>>> GetEmployeesPainelAsync(Guid tenantId, int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var (items, total) = await _userAccountRepository.GetPagedForTenantAsync(tenantId, page, limit, search);
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        return StaticResponseBuilder<PagedResultDto<UserAccountDto>>.BuildOk(new PagedResultDto<UserAccountDto>
        {
            Items = items.Select(u => ToDto(u)).ToList(),
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    public async Task<ResponseDTO<UserAccountDto>> GetEmployeePainelByIdAsync(Guid tenantId, Guid id)
    {
        var user = await _userAccountRepository.GetByIdForTenantWithRoleAsync(tenantId, id);
        if (user == null)
        {
            return StaticResponseBuilder<UserAccountDto>.BuildNotFound(new UserAccountDto());
        }

        return StaticResponseBuilder<UserAccountDto>.BuildOk(ToDto(user));
    }

    public async Task<ResponseDTO<UserAccountDto>> CreateEmployeePainelAsync(Guid tenantId, CreateUserAccountRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.ConfirmPassword) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
        {
            return StaticResponseBuilder<UserAccountDto>.BuildError("Dados inválidos.");
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return StaticResponseBuilder<UserAccountDto>.BuildError("As senhas não coincidem.");
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var existsEmail = await _userAccountRepository.EmailExistsAsync(email);
        if (existsEmail)
        {
            return StaticResponseBuilder<UserAccountDto>.BuildError("Email já cadastrado.");
        }

        if (request.RoleId.HasValue)
        {
            var role = await _roleRepository.GetByIdForTenantAsync(tenantId, request.RoleId.Value);
            if (role == null)
            {
                return StaticResponseBuilder<UserAccountDto>.BuildError("Role inválida.");
            }
        }

        var usernameBase = email.Split('@')[0];
        var username = await GenerateUniqueUsernameAsync(usernameBase);
        var now = DateTime.UtcNow;

        var entity = new UserAccountEntity
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PhoneNumber = request.PhoneNumber,
            Status = EUserAccountStatus.Ativo,
            IsEmailVerified = false,
            RoleId = request.RoleId,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _userAccountRepository.AddAsync(entity);
        await _userAccountRepository.SaveChangesAsync();

        var roleName = await ResolveRoleNameAsync(entity.RoleId);

        return StaticResponseBuilder<UserAccountDto>.BuildOk(ToDto(entity, roleName));
    }

    public async Task<ResponseDTO<UserAccountDto>> UpdateEmployeePainelAsync(Guid tenantId, Guid id, UpdateUserAccountRequestDto request)
    {
        var entity = await _userAccountRepository.GetByIdForTenantTrackedAsync(tenantId, id);
        if (entity == null)
        {
            return StaticResponseBuilder<UserAccountDto>.BuildNotFound(new UserAccountDto());
        }

        if (request.Email != null)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return StaticResponseBuilder<UserAccountDto>.BuildError("Email inválido.");
            }

            var newEmail = request.Email.Trim().ToLowerInvariant();
            var exists = await _userAccountRepository.EmailExistsAsync(newEmail, excludeUserId: id);
            if (exists)
            {
                return StaticResponseBuilder<UserAccountDto>.BuildError("Email já cadastrado.");
            }

            entity.Email = newEmail;
        }

        if (request.FirstName != null)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                return StaticResponseBuilder<UserAccountDto>.BuildError("Nome inválido.");
            }
            entity.FirstName = request.FirstName.Trim();
        }

        if (request.LastName != null)
        {
            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                return StaticResponseBuilder<UserAccountDto>.BuildError("Sobrenome inválido.");
            }
            entity.LastName = request.LastName.Trim();
        }

        if (request.PhoneNumber != null)
        {
            entity.PhoneNumber = request.PhoneNumber;
        }

        if (request.RoleId.HasValue)
        {
            var role = await _roleRepository.GetByIdForTenantAsync(tenantId, request.RoleId.Value);
            if (role == null)
            {
                return StaticResponseBuilder<UserAccountDto>.BuildError("Role inválida.");
            }

            entity.RoleId = request.RoleId;
        }

        if (request.Status != null)
        {
            if (!Enum.TryParse<EUserAccountStatus>(request.Status, ignoreCase: true, out var status))
            {
                return StaticResponseBuilder<UserAccountDto>.BuildError("Status inválido.");
            }

            entity.Status = status;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _userAccountRepository.SaveChangesAsync();

        var roleName = await ResolveRoleNameAsync(entity.RoleId);
        return StaticResponseBuilder<UserAccountDto>.BuildOk(ToDto(entity, roleName));
    }

    public async Task<ResponseDTO<UserAccountDto>> ToggleEmployeeStatusPainelAsync(Guid tenantId, Guid id)
    {
        var entity = await _userAccountRepository.GetByIdForTenantTrackedAsync(tenantId, id);
        if (entity == null)
        {
            return StaticResponseBuilder<UserAccountDto>.BuildNotFound(new UserAccountDto());
        }

        entity.Status = entity.Status == EUserAccountStatus.Ativo ? EUserAccountStatus.Inativo : EUserAccountStatus.Ativo;
        entity.UpdatedAt = DateTime.UtcNow;
        await _userAccountRepository.SaveChangesAsync();

        var roleName = await ResolveRoleNameAsync(entity.RoleId);
        return StaticResponseBuilder<UserAccountDto>.BuildOk(ToDto(entity, roleName));
    }

    public async Task<ResponseDTO<bool>> DeleteEmployeePainelAsync(Guid tenantId, Guid id)
    {
        var entity = await _userAccountRepository.GetByIdForTenantTrackedAsync(tenantId, id);
        if (entity == null)
        {
            return StaticResponseBuilder<bool>.BuildOk(true);
        }

        entity.DeletedAt = DateTime.UtcNow;
        entity.Status = EUserAccountStatus.Deletado;
        entity.UpdatedAt = DateTime.UtcNow;
        await _userAccountRepository.SaveChangesAsync();
        return StaticResponseBuilder<bool>.BuildOk(true);
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return true;
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var entity = await _userAccountRepository.GetByEmailTrackedAsync(email);
        if (entity == null)
        {
            return true;
        }

        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes);

        entity.PasswordResetToken = token;
        entity.PasswordResetExpiresAt = DateTime.UtcNow.AddHours(1);
        entity.UpdatedAt = DateTime.UtcNow;
        await _userAccountRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Token) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return false;
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var entity = await _userAccountRepository.GetByEmailTrackedAsync(email);
        if (entity == null)
        {
            return false;
        }

        if (entity.PasswordResetToken == null ||
            !string.Equals(entity.PasswordResetToken, request.Token, StringComparison.Ordinal) ||
            entity.PasswordResetExpiresAt == null ||
            entity.PasswordResetExpiresAt <= DateTime.UtcNow)
        {
            return false;
        }

        entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        entity.PasswordResetToken = null;
        entity.PasswordResetExpiresAt = null;
        entity.UpdatedAt = DateTime.UtcNow;
        await _userAccountRepository.SaveChangesAsync();

        return true;
    }

    public async Task<List<AccessGroupDto>> GetUserAccessGroupsAsync(Guid userId)
    {
        var groups = await _accessGroupRepository.GetActiveGroupsWithTypeByUserIdAsync(userId);
        return groups.Select(ag => new AccessGroupDto
        {
            Id = ag.Id,
            Name = ag.Name,
            Description = ag.Description,
            Code = ag.Code,
            TenantId = ag.TenantId,
            GroupTypeId = ag.GroupTypeId,
            GroupTypeName = ag.GroupType != null ? ag.GroupType.Name : null,
            IsActive = ag.IsActive,
            CreatedAt = ag.CreatedAt,
            UpdatedAt = ag.UpdatedAt
        }).ToList();
    }

    public async Task<(bool Success, bool NotFound, bool BadRequest)> AssignUserAccessGroupsAsync(Guid userId, AssignUserAccessGroupsRequestDto request)
    {
        var userExists = await _userAccountRepository.ExistsAsync(userId);
        if (!userExists)
        {
            return (false, true, false);
        }

        var requestedIds = (request.AccessGroupIds ?? []).Where(id => id != Guid.Empty).Distinct().ToList();
        if (requestedIds.Count == 0)
        {
            var existingAll = await _accountAccessGroupRepository.GetByUserIdAsync(userId);
            foreach (var rel in existingAll)
            {
                rel.IsActive = false;
                rel.UpdatedAt = DateTime.UtcNow;
            }
            await _accountAccessGroupRepository.SaveChangesAsync();
            return (true, false, false);
        }

        var validGroups = await _accessGroupRepository.GetExistingIdsAsync(requestedIds);
        if (validGroups.Count != requestedIds.Count)
        {
            return (false, false, true);
        }

        var existing = await _accountAccessGroupRepository.GetByUserIdAsync(userId);
        var now = DateTime.UtcNow;

        foreach (var rel in existing)
        {
            rel.IsActive = validGroups.Contains(rel.AccessGroupId);
            rel.UpdatedAt = now;
        }

        var existingGroupIds = existing.Select(x => x.AccessGroupId).ToHashSet();
        foreach (var groupId in validGroups)
        {
            if (existingGroupIds.Contains(groupId))
            {
                continue;
            }

            await _accountAccessGroupRepository.AddAsync(new AccountAccessGroupEntity
            {
                Id = Guid.NewGuid(),
                UserAccountId = userId,
                AccessGroupId = groupId,
                IsActive = true,
                GrantedBy = null,
                GrantedAt = now,
                ExpiresAt = null,
                CreatedAt = now,
                UpdatedAt = null
            });
        }

        await _accountAccessGroupRepository.SaveChangesAsync();
        return (true, false, false);
    }

    public async Task<bool> RevokeUserAccessGroupAsync(Guid userId, Guid groupId)
    {
        var rel = await _accountAccessGroupRepository.GetByUserIdAndGroupIdAsync(userId, groupId);
        if (rel == null)
        {
            return true;
        }

        rel.IsActive = false;
        rel.UpdatedAt = DateTime.UtcNow;
        await _accountAccessGroupRepository.SaveChangesAsync();
        return true;
    }

    private static UserAccountDto ToDto(UserAccountEntity u, string? roleName = null)
    {
        var resolvedRoleName = roleName ?? (u.Role != null ? (u.Role.Code ?? u.Role.Name) : null);

        return new UserAccountDto
        {
            Id = u.Id,
            TenantId = u.TenantId,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhoneNumber = u.PhoneNumber,
            Status = u.Status.ToString(),
            IsEmailVerified = u.IsEmailVerified,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt,
            LastLoginAt = u.LastLoginAt,
            FullName = (u.FirstName + " " + u.LastName).Trim(),
            RoleId = u.RoleId,
            RoleName = resolvedRoleName
        };
    }

    private async Task<string?> ResolveRoleNameAsync(Guid? roleId)
    {
        if (!roleId.HasValue)
        {
            return null;
        }

        var role = await _roleRepository.GetByIdAsync(roleId.Value);
        return role == null ? null : (role.Code ?? role.Name);
    }

    private async Task<string> GenerateUniqueUsernameAsync(string usernameBase)
    {
        var baseValue = string.IsNullOrWhiteSpace(usernameBase) ? "user" : usernameBase.Trim();
        var candidate = baseValue;

        var exists = await _userAccountRepository.UsernameExistsAsync(candidate);
        if (!exists)
        {
            return candidate;
        }

        for (var i = 0; i < 50; i++)
        {
            candidate = $"{baseValue}{RandomNumberGenerator.GetInt32(1000, 9999)}";
            exists = await _userAccountRepository.UsernameExistsAsync(candidate);
            if (!exists)
            {
                return candidate;
            }
        }

        return $"{baseValue}{Guid.NewGuid():N}";
    }
}
