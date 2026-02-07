using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Microsoft.EntityFrameworkCore;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Dapper;
using Npgsql;
using System.Data;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using OpaMenu.Infrastructure.Shared.Data.Context.AccessControl;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.Module
{
    public class ModuleRepository(IDbContextProvider dbContextProvider, AccessControlDbContext context, IConfiguration configuration) : BaseRepository<ModuleEntity>(dbContextProvider), IModuleRepository
    {
        private readonly AccessControlDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        private class FlatRowDTO
        {
            public Guid RoleId { get; set; }
            public string? RoleCode { get; set; }
            public string? ModuleKey { get; set; }
            public string? Actions { get; set; } // JSON list from DB
        }

        public async Task<UserPermissionsDTO> GetModulesByUserAsync(Guid userId)
        {
            var connectionString = _configuration.GetConnectionString("AccessControlDatabase");

            using (var connection = new NpgsqlConnection(connectionString)) 
            {
                var sql = @"SELECT 
                                r.id as RoleId,
                                r.code as RoleCode,
                                rp.module_key as ModuleKey, 
                                rp.actions as Actions
                            FROM public.user_account ua
                            JOIN public.role r ON ua.role_id = r.id
                                AND r.is_active = true
                            JOIN public.role_permission rp ON r.id = rp.role_id
                                AND rp.is_active = true
                            WHERE ua.id = @UserId";

                var param = new DynamicParameters();
                param.Add("@UserId", userId, DbType.Guid);

                var rows = await connection.QueryAsync<FlatRowDTO>(sql, param);

                // Reconstruct the DTO structure for compatibility
                var result = new List<AccessGroupBasicDTO>
                {
                    new AccessGroupBasicDTO
                    {
                        Id = Guid.Empty, // No longer using specific groups
                        Code = "DEFAULT",
                        Roles = rows.GroupBy(r => new { r.RoleId, r.RoleCode })
                                     .Select(rg => new RolesBasicDTO
                                     {
                                         Id = rg.Key.RoleId,
                                         Code = rg.Key.RoleCode,
                                         Modules = rg.Select(m => new ModuleBasicDTO
                                         {
                                             Id = Guid.Empty, // Module ID is less relevant now than Key
                                             Key = m.ModuleKey ?? string.Empty,
                                             Operations = !string.IsNullOrEmpty(m.Actions) 
                                                ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(m.Actions) ?? []
                                                : []
                                         }).ToList()
                                     }).ToList()
                    }
                };

                return new UserPermissionsDTO
                {
                    UserId = userId,
                    AccessGroups = result
                };
            }
        }
    }
}


