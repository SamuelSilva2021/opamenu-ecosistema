using Authenticator.API.Infrastructure.Repositories;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;
using OpaMenu.Infrastructure.Shared.Data.Context;
using Authenticator.API.Infrastructure.Providers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Authenticator.API.Tests
{
    public class RepositoryTests
    {
        private static AccessControlDbContext GetAccessControlInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AccessControlDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new TestAccessControlDbContext(options);
        }

        protected virtual MultiTenantDbContext GetMultiTenantInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MultiTenantDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new TestMultiTenantDbContext(options);
        }





        [Fact]
        public void DbContextProvider_Should_Return_AccessControlContext_For_UserAccountEntity()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);

            // Act
            var context = provider.GetContext<UserAccountEntity>();

            // Assert
            context.Should().BeAssignableTo<AccessControlDbContext>();
        }

        [Fact]
        public void DbContextProvider_Should_Return_MultiTenantContext_For_TenantEntity()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);

            // Act
            var context = provider.GetContext<TenantEntity>();

            // Assert
            context.Should().BeAssignableTo<MultiTenantDbContext>();
        }

        [Fact]
        public async Task Repository_Should_Add_UserAccount_Successfully()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);
            var repository = new BaseRepository<UserAccountEntity>(provider);

            var userAccount = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                FirstName = "Test",
                LastName = "User",
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = false
            };

            // Act
            var result = await repository.AddAsync(userAccount);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userAccount.Id);
            result.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Repository_Should_Add_Tenant_Successfully()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);
            var repository = new BaseRepository<TenantEntity>(provider);

            var tenant = new TenantEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Tenant",
                Slug = "test-tenant",
                Domain = "test.example.com",
                Status = ETenantStatus.Ativo,
                Document = "12345678901",
                RazaoSocial = "Test Company Ltd",
                Email = "contact@test.example.com"
            };

            // Act
            var result = await repository.AddAsync(tenant);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(tenant.Id);
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Repository_Should_Get_All_UserAccounts()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);
            var repository = new BaseRepository<UserAccountEntity>(provider);

            var userAccount1 = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "user1",
                Email = "user1@example.com",
                PasswordHash = "hash1",
                FirstName = "User",
                LastName = "One",
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = true
            };

            var userAccount2 = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "user2",
                Email = "user2@example.com",
                PasswordHash = "hash2",
                FirstName = "User",
                LastName = "Two",
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = false
            };

            await repository.AddAsync(userAccount1);
            await repository.AddAsync(userAccount2);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(u => u.Username == "user1");
            result.Should().Contain(u => u.Username == "user2");
        }

        [Fact]
        public async Task Repository_Should_Find_UserAccount_By_Predicate()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);
            var repository = new BaseRepository<UserAccountEntity>(provider);

            var userAccount = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "findme",
                Email = "findme@example.com",
                PasswordHash = "hash",
                FirstName = "Find",
                LastName = "Me",
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = true
            };

            await repository.AddAsync(userAccount);

            // Act
            var result = await repository.FindAsync(u => u.Username == "findme");

            // Assert
            result.Should().HaveCount(1);
            result.First().Username.Should().Be("findme");
        }

        [Fact]
        public async Task Repository_Should_Update_UserAccount()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);
            var repository = new BaseRepository<UserAccountEntity>(provider);

            var userAccount = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "updateme",
                Email = "updateme@example.com",
                PasswordHash = "hash",
                FirstName = "Update",
                LastName = "Me",
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = false,
                UpdatedAt = DateTime.Now
            };

            await repository.AddAsync(userAccount);
            var originalUpdatedAt = userAccount.UpdatedAt;

            // Act
            userAccount.IsEmailVerified = true;
            await Task.Delay(100);
            await repository.UpdateAsync(userAccount);

            // Assert
            var updatedEntity = await repository.GetByIdAsync(userAccount.Id);
            updatedEntity.Should().NotBeNull();
            updatedEntity!.IsEmailVerified.Should().BeTrue();
            updatedEntity.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Repository_Should_Delete_UserAccount()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);
            var repository = new BaseRepository<UserAccountEntity>(provider);

            var userAccount = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "deleteme",
                Email = "deleteme@example.com",
                PasswordHash = "hash",
                FirstName = "Delete",
                LastName = "Me",
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = false
            };

            await repository.AddAsync(userAccount);

            // Act
            await repository.DeleteAsync(userAccount);

            // Assert
            var deletedEntity = await repository.GetByIdAsync(userAccount.Id);
            deletedEntity.Should().BeNull();
        }

        [Fact]
        public async Task Repository_Should_Count_UserAccounts()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);
            var repository = new BaseRepository<UserAccountEntity>(provider);

            var userAccount1 = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "count1",
                Email = "count1@example.com",
                PasswordHash = "hash1",
                FirstName = "Count",
                LastName = "One",
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = true
            };

            var userAccount2 = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "count2",
                Email = "count2@example.com",
                PasswordHash = "hash2",
                FirstName = "Count",
                LastName = "Two",
                Status = EUserAccountStatus.Inativo,
                IsEmailVerified = false
            };

            await repository.AddAsync(userAccount1);
            await repository.AddAsync(userAccount2);

            // Act
            var totalCount = await repository.CountAsync();
            var activeCount = await repository.CountAsync(u => u.Status == EUserAccountStatus.Ativo);

            // Assert
            totalCount.Should().Be(2);
            activeCount.Should().Be(1);
        }

        [Fact]
        public async Task Repository_Should_Perform_Virtual_Delete_On_UserAccount()
        {
            // Arrange
            var accessControlContext = GetAccessControlInMemoryContext();
            var multiTenantContext = GetMultiTenantInMemoryContext();
            var provider = new DbContextProvider(accessControlContext, multiTenantContext);
            var repository = new BaseRepository<UserAccountEntity>(provider);

            var userAccount = new UserAccountEntity
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                Username = "softdelete",
                Email = "softdelete@example.com",
                PasswordHash = "hash",
                FirstName = "Soft",
                LastName = "Delete",
                Status = EUserAccountStatus.Ativo,
                IsEmailVerified = false
            };

            await repository.AddAsync(userAccount);

            // Act
            await repository.DeleteVirtualAsync(userAccount.Id);

            // Assert
            var entity = await repository.GetByIdAsync(userAccount.Id);
            entity.Should().NotBeNull();
            entity!.DeletedAt.Should().NotBeNull();
            entity.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }

    public class TestAccessControlDbContext : AccessControlDbContext
    {
        public TestAccessControlDbContext(DbContextOptions<AccessControlDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Ignore property that causes issues with InMemory provider
            modelBuilder.Entity<TenantEntity>().Ignore(e => e.Settings);
        }
    }

    public class TestMultiTenantDbContext : MultiTenantDbContext
    {
        public TestMultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Ignore property that causes issues with InMemory provider
            modelBuilder.Entity<TenantEntity>().Ignore(e => e.Settings);
        }
    }
}


