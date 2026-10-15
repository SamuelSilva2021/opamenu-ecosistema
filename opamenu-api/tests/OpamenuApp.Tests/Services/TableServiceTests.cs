using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Opamenu.Application.Services.Interfaces;
using Opamenu.Application.Services.Interfaces.Opamenu;
using Opamenu.Domain.DTOs.Table;
using Opamenu.Domain.Interfaces;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OpamenuApp.Tests.Services;

public class TableServiceTests
{
    private readonly Mock<ITableRepository> _mockTableRepository = new();
    private readonly Mock<ICurrentUserService> _mockCurrentUserService = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IUrlBuilderService> _mockUrlBuilderService = new();
    private readonly Mock<ILogger<global::Opamenu.Application.Services.Opamenu.TableService>> _mockLogger = new();

    private readonly Guid _tenantId = Guid.NewGuid();

    [Fact]
    public async Task GetByIdWithDetailsAsync_WhenTableExists_ReturnsOk()
    {
        _mockCurrentUserService.Setup(x => x.GetTenantGuid()).Returns(_tenantId);

        var tableId = Guid.NewGuid();
        var entity = new TableEntity
        {
            Id = tableId,
            TenantId = _tenantId,
            Name = "Mesa 01",
            Capacity = 4,
            IsActive = true
        };

        _mockTableRepository
            .Setup(x => x.GetByIdWithDetailsAsync(_tenantId, tableId))
            .ReturnsAsync(entity);

        _mockMapper
            .Setup(x => x.Map<TableFullResponseDto>(entity))
            .Returns(new TableFullResponseDto
            {
                Id = tableId,
                Name = "Mesa 01",
                Capacity = 4,
                IsActive = true,
                Tabs = Array.Empty<global::Opamenu.Domain.DTOs.Tab.TabResponseDto>()
            });

        var service = new global::Opamenu.Application.Services.Opamenu.TableService(
            _mockTableRepository.Object,
            _mockCurrentUserService.Object,
            _mockMapper.Object,
            _mockUrlBuilderService.Object,
            _mockLogger.Object);

        var result = await service.GetByIdWithDetailsAsync(tableId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(tableId, result.Data!.Id);
    }

    [Fact]
    public async Task GetPagedWithTabsAsync_WhenTenantExists_ReturnsPagedOk()
    {
        _mockCurrentUserService.Setup(x => x.GetTenantGuid()).Returns(_tenantId);

        var tables = new List<TableEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantId,
                Name = "Mesa 01",
                Capacity = 4,
                IsActive = true
            }
        };

        _mockTableRepository
            .Setup(x => x.GetPagedWithTabsAsync(_tenantId, 1, 50))
            .ReturnsAsync(tables);

        _mockTableRepository
            .Setup(x => x.CountByTenantIdAsync(_tenantId))
            .ReturnsAsync(1);

        _mockMapper
            .Setup(x => x.Map<IEnumerable<TableFullResponseDto>>(tables))
            .Returns(new List<TableFullResponseDto>
            {
                new()
                {
                    Id = tables[0].Id,
                    Name = "Mesa 01",
                    Capacity = 4,
                    IsActive = true,
                    Tabs = Array.Empty<global::Opamenu.Domain.DTOs.Tab.TabResponseDto>()
                }
            });

        var service = new global::Opamenu.Application.Services.Opamenu.TableService(
            _mockTableRepository.Object,
            _mockCurrentUserService.Object,
            _mockMapper.Object,
            _mockUrlBuilderService.Object,
            _mockLogger.Object);

        var result = await service.GetPagedWithTabsAsync(1, 50);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!);
        Assert.Equal(1, result.TotalItems);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(50, result.PageSize);
    }
}
