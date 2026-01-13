using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Services;
using OpaMenu.Application.Interfaces;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Domain.DTOs.Table;
using OpaMenu.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace PedejaApp.Tests.Unit.Services
{
    public class TableServiceTests
    {
        private readonly Mock<ITableRepository> _mockTableRepository;
        private readonly Mock<ICurrentUserService> _mockCurrentUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUrlBuilderService> _mockUrlBuilderService;
        private readonly Mock<ILogger<TableService>> _mockLogger;
        private readonly TableService _tableService;

        private readonly Guid _tenantId = Guid.NewGuid();

        public TableServiceTests()
        {
            _mockTableRepository = new Mock<ITableRepository>();
            _mockCurrentUserService = new Mock<ICurrentUserService>();
            _mockMapper = new Mock<IMapper>();
            _mockUrlBuilderService = new Mock<IUrlBuilderService>();
            _mockLogger = new Mock<ILogger<TableService>>();

            _mockCurrentUserService.Setup(x => x.GetTenantGuid()).Returns(_tenantId);

            _tableService = new TableService(
                _mockTableRepository.Object,
                _mockCurrentUserService.Object,
                _mockMapper.Object,
                _mockUrlBuilderService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnPagedTables()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var tables = new List<TableEntity> { new TableEntity { Id = 1, Name = "Mesa 1", TenantId = _tenantId } };
            var tableDtos = new List<TableResponseDto> { new TableResponseDto(1, "Mesa 1", 4, true, null) };

            _mockTableRepository.Setup(x => x.GetPagedByTenantIdAsync(_tenantId, pageNumber, pageSize))
                .ReturnsAsync(tables);
            _mockTableRepository.Setup(x => x.CountByTenantIdAsync(_tenantId))
                .ReturnsAsync(1);
            _mockMapper.Setup(x => x.Map<IEnumerable<TableResponseDto>>(tables))
                .Returns(tableDtos);

            // Act
            var result = await _tableService.GetPagedAsync(pageNumber, pageSize);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(1, result.TotalItems);
            Assert.Single(result.Data!);
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldCreateTable()
        {
            // Arrange
            var createDto = new CreateTableRequestDto("Mesa 1", 4);
            var tableEntity = new TableEntity { Id = 1, Name = "Mesa 1", Capacity = 4, TenantId = _tenantId };
            var tableResponse = new TableResponseDto(1, "Mesa 1", 4, true, null);

            _mockTableRepository.Setup(x => x.ExistsByNameAsync(createDto.Name, _tenantId))
                .ReturnsAsync(false);
            _mockMapper.Setup(x => x.Map<TableEntity>(createDto))
                .Returns(tableEntity);
            _mockMapper.Setup(x => x.Map<TableResponseDto>(tableEntity))
                .Returns(tableResponse);

            // Act
            var result = await _tableService.CreateAsync(createDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal("Mesa 1", result.Data!.Name);
            _mockTableRepository.Verify(x => x.AddAsync(tableEntity), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateName_ShouldReturnError()
        {
            // Arrange
            var createDto = new CreateTableRequestDto("Mesa 1", 4);

            _mockTableRepository.Setup(x => x.ExistsByNameAsync(createDto.Name, _tenantId))
                .ReturnsAsync(true);

            // Act
            var result = await _tableService.CreateAsync(createDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("JÃ¡ existe uma mesa com este nome", result.Errors.First().Message);
            _mockTableRepository.Verify(x => x.AddAsync(It.IsAny<TableEntity>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_ShouldUpdateTable()
        {
            // Arrange
            var id = 1;
            var updateDto = new UpdateTableRequestDto("Mesa Atualizada", 6, true);
            var existingTable = new TableEntity { Id = id, Name = "Mesa 1", Capacity = 4, TenantId = _tenantId };
            
            _mockTableRepository.Setup(x => x.GetByIdAsync(id, _tenantId))
                .ReturnsAsync(existingTable);
            _mockTableRepository.Setup(x => x.GetByNameAsync(updateDto.Name!, _tenantId))
                .ReturnsAsync((TableEntity?)null);
            
            // Act
            var result = await _tableService.UpdateAsync(id, updateDto);

            // Assert
            Assert.True(result.Succeeded);
            _mockMapper.Verify(x => x.Map(updateDto, existingTable), Times.Once);
            _mockTableRepository.Verify(x => x.UpdateAsync(existingTable), Times.Once);
        }

        [Fact]
        public async Task GenerateQrCodeAsync_ShouldGenerateUrlAndUpdateTable()
        {
            // Arrange
            var id = 1;
            var table = new TableEntity { Id = id, Name = "Mesa 1", TenantId = _tenantId };
            var baseUrl = "https://api.opamenu.com";
            var expectedUrl = $"{baseUrl}/menu/{_tenantId}/{id}";

            _mockTableRepository.Setup(x => x.GetByIdAsync(id, _tenantId))
                .ReturnsAsync(table);
            _mockUrlBuilderService.Setup(x => x.GetBaseUrl())
                .Returns(baseUrl);

            // Act
            var result = await _tableService.GenerateQrCodeAsync(id);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Equal(expectedUrl, result.Data);
            Assert.Equal(expectedUrl, table.QrCodeUrl);
            _mockTableRepository.Verify(x => x.UpdateAsync(table), Times.Once);
        }
    }
}

