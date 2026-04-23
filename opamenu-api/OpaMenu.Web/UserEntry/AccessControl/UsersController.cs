using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using System.Security.Cryptography;

namespace OpaMenu.Web.UserEntry.AccessControl;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "ADMIN,SUPER_ADMIN")]
public sealed class UsersController(AccessControlDbContext dbContext) : ControllerBase
{
    private readonly AccessControlDbContext _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<UserAccountDto>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? search = null)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var query = _dbContext.UserAccounts.AsNoTracking().Include(u => u.Role).Where(u => u.DeletedAt == null).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(u =>
                u.Username.Contains(s) ||
                u.Email.Contains(s) ||
                u.FirstName.Contains(s) ||
                u.LastName.Contains(s));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(u => new UserAccountDto
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
                RoleName = u.Role != null ? (u.Role.Code ?? u.Role.Name) : null
            })
            .ToListAsync();

        return Ok(new PagedResultDto<UserAccountDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<UserAccountDto>>> GetActiveUsers()
    {
        var users = await _dbContext.UserAccounts.AsNoTracking().Include(u => u.Role)
            .Where(u => u.DeletedAt == null && u.Status == EUserAccountStatus.Ativo)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new UserAccountDto
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
                RoleName = u.Role != null ? (u.Role.Code ?? u.Role.Name) : null
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserAccountDto>> GetById([FromRoute] Guid id)
    {
        var user = await _dbContext.UserAccounts.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserAccountDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            Status = user.Status.ToString(),
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLoginAt = user.LastLoginAt,
            FullName = (user.FirstName + " " + user.LastName).Trim(),
            RoleId = user.RoleId,
            RoleName = user.Role != null ? (user.Role.Code ?? user.Role.Name) : null
        });
    }

    [HttpPost]
    public async Task<ActionResult<UserAccountDto>> Create([FromBody] CreateUserAccountRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.ConfirmPassword) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest();
        }

        if (!string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return BadRequest();
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var existsEmail = await _dbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Email.ToLower() == email && u.DeletedAt == null);
        if (existsEmail)
        {
            return BadRequest();
        }

        var usernameBase = email.Split('@')[0];
        var username = await GenerateUniqueUsernameAsync(usernameBase);

        if (request.RoleId.HasValue)
        {
            var existsRole = await _dbContext.Roles.AsNoTracking().AnyAsync(r => r.Id == request.RoleId.Value);
            if (!existsRole)
            {
                return BadRequest();
            }
        }

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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.UserAccounts.Add(entity);
        await _dbContext.SaveChangesAsync();

        var roleName = entity.RoleId.HasValue
            ? await _dbContext.Roles.AsNoTracking().Where(r => r.Id == entity.RoleId.Value).Select(r => r.Code ?? r.Name).FirstOrDefaultAsync()
            : null;

        return Ok(new UserAccountDto
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
        });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserAccountDto>> Update([FromRoute] Guid id, [FromBody] UpdateUserAccountRequestDto request)
    {
        var entity = await _dbContext.UserAccounts.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NotFound();
        }

        if (request.Username != null)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest();
            }

            var newUsername = request.Username.Trim();
            var exists = await _dbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Id != id && u.Username == newUsername && u.DeletedAt == null);
            if (exists)
            {
                return BadRequest();
            }

            entity.Username = newUsername;
        }

        if (request.Email != null)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest();
            }

            var newEmail = request.Email.Trim().ToLowerInvariant();
            var exists = await _dbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Id != id && u.Email.ToLower() == newEmail && u.DeletedAt == null);
            if (exists)
            {
                return BadRequest();
            }

            entity.Email = newEmail;
        }

        if (request.FirstName != null)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
            {
                return BadRequest();
            }

            entity.FirstName = request.FirstName.Trim();
        }

        if (request.LastName != null)
        {
            if (string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest();
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
            var existsRole = await _dbContext.Roles.AsNoTracking().AnyAsync(r => r.Id == request.RoleId.Value);
            if (!existsRole)
            {
                return BadRequest();
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
                return BadRequest();
            }

            entity.Status = status;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        var roleName = entity.RoleId.HasValue
            ? await _dbContext.Roles.AsNoTracking().Where(r => r.Id == entity.RoleId.Value).Select(r => r.Code ?? r.Name).FirstOrDefaultAsync()
            : null;

        return Ok(new UserAccountDto
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
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var entity = await _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
        if (entity == null)
        {
            return NoContent();
        }

        entity.DeletedAt = DateTime.UtcNow;
        entity.Status = EUserAccountStatus.Deletado;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Ok(true);
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var entity = await _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.DeletedAt == null);
        if (entity == null)
        {
            return Ok(true);
        }

        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes);

        entity.PasswordResetToken = token;
        entity.PasswordResetExpiresAt = DateTime.UtcNow.AddHours(1);
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(true);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Token) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest();
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var entity = await _dbContext.UserAccounts.FirstOrDefaultAsync(u => u.Email.ToLower() == email && u.DeletedAt == null);
        if (entity == null)
        {
            return BadRequest();
        }

        if (entity.PasswordResetToken == null ||
            !string.Equals(entity.PasswordResetToken, request.Token, StringComparison.Ordinal) ||
            entity.PasswordResetExpiresAt == null ||
            entity.PasswordResetExpiresAt <= DateTime.UtcNow)
        {
            return BadRequest();
        }

        entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        entity.PasswordResetToken = null;
        entity.PasswordResetExpiresAt = null;
        entity.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Ok(true);
    }

    [HttpGet("{userId:guid}/access-groups")]
    public async Task<ActionResult<List<AccessGroupDto>>> GetUserAccessGroups([FromRoute] Guid userId)
    {
        var groups = await _dbContext.AccountAccessGroups
            .AsNoTracking()
            .Where(aag => aag.UserAccountId == userId && aag.IsActive)
            .Select(aag => aag.AccessGroup)
            .Where(ag => ag.IsActive)
            .Include(ag => ag.GroupType)
            .Select(ag => new AccessGroupDto
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
            })
            .ToListAsync();

        return Ok(groups);
    }

    [HttpPost("{userId:guid}/access-groups")]
    public async Task<ActionResult<bool>> AssignUserAccessGroups([FromRoute] Guid userId, [FromBody] AssignUserAccessGroupsRequestDto request)
    {
        var userExists = await _dbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Id == userId && u.DeletedAt == null);
        if (!userExists)
        {
            return NotFound();
        }

        var requestedIds = (request.AccessGroupIds ?? []).Where(id => id != Guid.Empty).Distinct().ToList();
        if (requestedIds.Count == 0)
        {
            var existingAll = await _dbContext.AccountAccessGroups.Where(aag => aag.UserAccountId == userId).ToListAsync();
            foreach (var rel in existingAll)
            {
                rel.IsActive = false;
                rel.UpdatedAt = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
            return Ok(true);
        }

        var validGroups = await _dbContext.AccessGroups.AsNoTracking().Where(g => requestedIds.Contains(g.Id)).Select(g => g.Id).ToListAsync();
        if (validGroups.Count != requestedIds.Count)
        {
            return BadRequest();
        }

        var existing = await _dbContext.AccountAccessGroups.Where(aag => aag.UserAccountId == userId).ToListAsync();
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

            _dbContext.AccountAccessGroups.Add(new AccountAccessGroupEntity
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

        await _dbContext.SaveChangesAsync();
        return Ok(true);
    }

    [HttpDelete("{userId:guid}/access-groups/{groupId:guid}")]
    public async Task<ActionResult<bool>> RevokeUserAccessGroup([FromRoute] Guid userId, [FromRoute] Guid groupId)
    {
        var rel = await _dbContext.AccountAccessGroups.FirstOrDefaultAsync(aag => aag.UserAccountId == userId && aag.AccessGroupId == groupId);
        if (rel == null)
        {
            return Ok(true);
        }

        rel.IsActive = false;
        rel.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return Ok(true);
    }

    private async Task<string> GenerateUniqueUsernameAsync(string usernameBase)
    {
        var baseValue = string.IsNullOrWhiteSpace(usernameBase) ? "user" : usernameBase.Trim();
        var candidate = baseValue;

        var exists = await _dbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Username == candidate && u.DeletedAt == null);
        if (!exists)
        {
            return candidate;
        }

        for (var i = 0; i < 50; i++)
        {
            candidate = $"{baseValue}{RandomNumberGenerator.GetInt32(1000, 9999)}";
            exists = await _dbContext.UserAccounts.AsNoTracking().AnyAsync(u => u.Username == candidate && u.DeletedAt == null);
            if (!exists)
            {
                return candidate;
            }
        }

        return $"{baseValue}{Guid.NewGuid():N}";
    }
}

