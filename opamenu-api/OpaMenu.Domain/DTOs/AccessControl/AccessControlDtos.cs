namespace OpaMenu.Domain.DTOs.AccessControl;

public sealed class PagedResultDto<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}

public sealed class ModuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Key { get; set; }
    public string? Code { get; set; }
    public Guid? ApplicationId { get; set; }
    public Guid? ModuleTypeId { get; set; }
    public string? ModuleTypeName { get; set; }
    public string? ApplicationName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CreateModuleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? ApplicationId { get; set; }
    public bool IsActive { get; set; }
}

public sealed class UpdateModuleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? ApplicationId { get; set; }
    public Guid? ModuleTypeId { get; set; }
    public bool IsActive { get; set; }
}

public sealed class SimplifiedPermissionDto
{
    public string Module { get; set; } = string.Empty;
    public List<string> Actions { get; set; } = [];
}

public sealed class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? ApplicationId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<SimplifiedPermissionDto>? Permissions { get; set; }
}

public sealed class CreateRoleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? ApplicationId { get; set; }
    public List<SimplifiedPermissionDto>? Permissions { get; set; }
    public bool? IsActive { get; set; }
}

public sealed class UpdateRoleRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool? IsActive { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? ApplicationId { get; set; }
    public List<SimplifiedPermissionDto>? Permissions { get; set; }
}

public sealed class GroupTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CreateGroupTypeRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool? IsActive { get; set; }
}

public sealed class UpdateGroupTypeRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public sealed class AccessGroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? GroupTypeId { get; set; }
    public string? GroupTypeName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class CreateAccessGroupRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? TenantId { get; set; }
    public Guid GroupTypeId { get; set; }
}

public sealed class UpdateAccessGroupRequestDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? GroupTypeId { get; set; }
    public Guid? TenantId { get; set; }
    public bool? IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class UserAccountDto
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string FullName { get; set; } = string.Empty;
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
}

public sealed class CreateUserAccountRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
}

public sealed class UpdateUserAccountRequestDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Status { get; set; }
    public bool? IsEmailVerified { get; set; }
    public Guid? TenantId { get; set; }
    public Guid? RoleId { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public sealed class AssignUserAccessGroupsRequestDto
{
    public List<Guid> AccessGroupIds { get; set; } = [];
}

public sealed class ForgotPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
}

public sealed class ResetPasswordRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

