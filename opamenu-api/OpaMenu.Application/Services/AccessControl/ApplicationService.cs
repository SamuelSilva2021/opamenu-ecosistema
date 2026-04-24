using OpaMenu.Application.Services.Interfaces.AccessControl;
using OpaMenu.Domain.DTOs.AccessControl;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;

namespace OpaMenu.Application.Services.AccessControl;

public sealed class ApplicationService(IApplicationRepository applicationRepository) : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository = applicationRepository;

    public async Task<PagedResultDto<ApplicationDto>> GetApplicationsAsync(int page, int limit, string? search)
    {
        page = page <= 0 ? 1 : page;
        limit = limit <= 0 ? 10 : limit;

        var (items, total) = await _applicationRepository.GetPagedAsync(page, limit, search);
        var totalPages = (int)Math.Ceiling(total / (double)limit);

        return new PagedResultDto<ApplicationDto>
        {
            Items = items.Select(Map).ToList(),
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = totalPages
        };
    }

    public async Task<ApplicationDto?> GetByIdAsync(Guid id)
    {
        var app = await _applicationRepository.GetByIdAsync(id);
        return app == null ? null : Map(app);
    }

    public async Task<ApplicationDto?> CreateAsync(CreateApplicationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return null;
        }

        var entity = new ApplicationEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description ?? string.Empty,
            Code = request.Code,
            Url = string.Empty,
            SecretKey = null,
            AuxiliarSchema = null,
            IsActive = request.IsActive,
            Visible = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        await _applicationRepository.AddAsync(entity);
        await _applicationRepository.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<(ApplicationDto? Application, bool NotFound)> UpdateAsync(Guid id, UpdateApplicationRequestDto request)
    {
        var entity = await _applicationRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return (null, true);
        }

        if (request.Name != null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return (null, false);
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

        if (request.IsActive.HasValue)
        {
            entity.IsActive = request.IsActive.Value;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        await _applicationRepository.SaveChangesAsync();

        return (Map(entity), false);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _applicationRepository.GetByIdTrackedAsync(id);
        if (entity == null)
        {
            return;
        }

        await _applicationRepository.DeleteAsync(entity);
        await _applicationRepository.SaveChangesAsync();
    }

    private static ApplicationDto Map(ApplicationEntity a)
    {
        return new ApplicationDto
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
            Code = a.Code,
            IsActive = a.IsActive,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}

