using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Application.Services.AccessControl;

public sealed class AccessGroupService(
    IAccessGroupRepository accessGroupRepository,
    IGroupTypeRepository groupTypeRepository) : IAccessGroupService
{
    private readonly IAccessGroupRepository _accessGroupRepository = accessGroupRepository;
    private readonly IGroupTypeRepository _groupTypeRepository = groupTypeRepository;

    public async Task<PagedResultDto<AccessGroupDto>> GetGroupsAsync(int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var (items, total) = await _accessGroupRepository.GetPagedWithTypeAsync(page, limit, search);
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        return new PagedResultDto<AccessGroupDto>
        {
            Items = items.Select(MapGroup).ToList(),
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        };
    }

    public async Task<AccessGroupDto?> GetGroupByIdAsync(Guid id)
    {
        var group = await _accessGroupRepository.GetByIdWithTypeAsync(id);
        return group == null ? null : MapGroup(group);
    }

    public async Task<AccessGroupDto?> CreateGroupAsync(CreateAccessGroupRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        if (request.GroupTypeId == Guid.Empty)
        {
            return null;
        }

        var groupTypeExists = await _groupTypeRepository.ExistsAsync(request.GroupTypeId);
        if (!groupTypeExists)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var entity = new AccessGroupEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Code = request.Code,
            TenantId = request.TenantId,
            GroupTypeId = request.GroupTypeId,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = null
        };

        await _accessGroupRepository.AddAsync(entity);
        await _accessGroupRepository.SaveChangesAsync();

        return await GetGroupByIdAsync(entity.Id);
    }

    public async Task<(AccessGroupDto? Group, bool NotFound, bool BadRequest)> UpdateGroupAsync(Guid id, UpdateAccessGroupRequestDto request)
    {
        var entity = await _accessGroupRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return (null, true, false);
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (null, false, true);
            }

            entity.Name = request.Name.Trim();
        }

        if (request.Description != null)
        {
            entity.Description = request.Description;
        }

        if (request.Code != null)
        {
            entity.Code = request.Code;
        }

        if (request.GroupTypeId.HasValue)
        {
            if (request.GroupTypeId.Value == Guid.Empty)
            {
                return (null, false, true);
            }

            var groupTypeExists = await _groupTypeRepository.ExistsAsync(request.GroupTypeId.Value);
            if (!groupTypeExists)
            {
                return (null, false, true);
            }

            entity.GroupTypeId = request.GroupTypeId.Value;
        }

        if (request.TenantId.HasValue)
        {
            entity.TenantId = request.TenantId;
        }

        if (request.IsActive.HasValue)
        {
            entity.IsActive = request.IsActive.Value;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _accessGroupRepository.SaveChangesAsync();
        return (await GetGroupByIdAsync(id), false, false);
    }

    public async Task DeleteGroupAsync(Guid id)
    {
        var entity = await _accessGroupRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return;
        }

        await _accessGroupRepository.DeleteAsync(entity);
        await _accessGroupRepository.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<GroupTypeDto>> GetGroupTypesAsync()
    {
        var items = await _groupTypeRepository.GetAllAsync();
        return items.Select(MapGroupType).ToList();
    }

    public async Task<GroupTypeDto?> GetGroupTypeByIdAsync(Guid id)
    {
        var entity = await _groupTypeRepository.GetByIdAsync(id);
        return entity == null ? null : MapGroupType(entity);
    }

    public async Task<GroupTypeDto?> CreateGroupTypeAsync(CreateGroupTypeRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Code))
        {
            return null;
        }

        var code = request.Code.Trim();
        var exists = await _groupTypeRepository.CodeExistsAsync(code);
        if (exists)
        {
            return null;
        }

        var entity = new GroupTypeEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description,
            Code = code,
            IsActive = request.IsActive ?? true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        await _groupTypeRepository.AddAsync(entity);
        await _groupTypeRepository.SaveChangesAsync();
        return MapGroupType(entity);
    }

    public async Task<(GroupTypeDto? GroupType, bool NotFound, bool BadRequest)> UpdateGroupTypeAsync(Guid id, UpdateGroupTypeRequestDto request)
    {
        var entity = await _groupTypeRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return (null, true, false);
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (null, false, true);
            }

            entity.Name = request.Name.Trim();
        }

        if (request.Description != null)
        {
            entity.Description = request.Description;
        }

        if (request.Code != null)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return (null, false, true);
            }

            var code = request.Code.Trim();
            var exists = await _groupTypeRepository.CodeExistsAsync(code, excludeId: id);
            if (exists)
            {
                return (null, false, true);
            }

            entity.Code = code;
        }

        if (request.IsActive.HasValue)
        {
            entity.IsActive = request.IsActive.Value;
        }

        if (request.CreatedAt.HasValue)
        {
            entity.CreatedAt = request.CreatedAt.Value;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _groupTypeRepository.SaveChangesAsync();
        return (MapGroupType(entity), false, false);
    }

    public async Task DeleteGroupTypeAsync(Guid id)
    {
        var entity = await _groupTypeRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return;
        }

        await _groupTypeRepository.DeleteAsync(entity);
        await _groupTypeRepository.SaveChangesAsync();
    }

    private static AccessGroupDto MapGroup(AccessGroupEntity g)
    {
        return new AccessGroupDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Code = g.Code,
            TenantId = g.TenantId,
            GroupTypeId = g.GroupTypeId,
            GroupTypeName = g.GroupType?.Name,
            IsActive = g.IsActive,
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt
        };
    }

    private static GroupTypeDto MapGroupType(GroupTypeEntity gt)
    {
        return new GroupTypeDto
        {
            Id = gt.Id,
            Name = gt.Name,
            Description = gt.Description,
            Code = gt.Code,
            IsActive = gt.IsActive,
            CreatedAt = gt.CreatedAt,
            UpdatedAt = gt.UpdatedAt
        };
    }
}
