using Authenticator.API.Core.Application.Interfaces;
using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Infrastructure.Data.Context;
using Authenticator.API.Infrastructure.Repositories.AccessControl.AccessGroup;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Authenticator.API.Infrastructure.Repositories.AccessControl.Module
{
    /// <summary>
    /// Implementação específica do repositório para tipos de módulo
    /// </summary>
    /// <param name="dbContextProvider"></param>
    /// <param name="logger"></param>
    public class ModuleRepository(
        IDbContextProvider dbContextProvider,
        AccessControlDbContext context,
        IConfiguration configuration
        ) : BaseRepository<ModuleEntity>(dbContextProvider), IModuleRepository
    {
        private readonly IDbContextProvider _dbContextProvider = dbContextProvider;
        private readonly AccessControlDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<UserPermissionsDTO> GetModulesByUserAsync(Guid userId)
        {
            var connectionString = _configuration.GetConnectionString("AccessControlDatabase");

            using (var connection = new NpgsqlConnection(connectionString)) 
            {
                var sql = @"SELECT 
                                ag.id as GroupId,
	                            ag.code as GroupCode,
	                            r.id as RoleId,
	                            r.code as RoleCode,
	                            m.id as ModuleId,
	                            m.key ModuleKey, 
	                            o.value as Operations
                            FROM public.access_group ag
                            JOIN public.account_access_group aag ON ag.id = aag.access_group_id
                                and aag.is_active = true
                            JOIN public.role_access_group rag ON aag.access_group_id = rag.access_group_id
                                and rag.is_active = true
                            JOIN public.role r ON rag.role_id = r.id
                                and r.is_active = true
                            JOIN public.role_permission rp ON r.id = rp.role_id
                                and rp.is_active = true
                            JOIN public.permission p ON rp.permission_id = p.id
                                and p.is_active = true
                            JOIN public.permission_operation po ON p.id = po.permission_id
                                and po.is_active = true
                            JOIN public.module m ON p.module_id = m.id
                                and m.is_active = true
                            JOIN public.operation o ON po.operation_id = o.id
                            WHERE aag.user_account_id = @UserId
                              AND aag.is_active = true";

                var param = new DynamicParameters();
                param.Add("@UserId", userId, DbType.Guid);

                var rows = await connection.QueryAsync<FlatRowDTO>(sql, param);

                var result = rows
                    .GroupBy(g => new { g.GroupId, g.GroupCode })
                    .Select(g => new AccessGroupBasicDTO
                    {
                        Id = g.Key.GroupId,
                        Code = g.Key.GroupCode,
                        Roles = g.GroupBy(r => new { r.RoleId, r.RoleCode })
                                 .Select(rg => new RolesBasicDTO
                                 {
                                     Id = rg.Key.RoleId,
                                     Code = rg.Key.RoleCode,
                                     Modules = rg.GroupBy(m => new { m.ModuleId, m.ModuleKey })
                                                 .Select(mg => new ModuleBasicDTO
                                                 {
                                                     Id = mg.Key.ModuleId,
                                                     Key = mg.Key.ModuleKey,
                                                     Operations = mg.Select(o => o.Operations).Distinct().ToList()
                                                 }).ToList()
                                 }).ToList()
                    }).ToList();

                return new UserPermissionsDTO
                {
                    UserId = userId,
                    AccessGroups = result
                };
            }
        }
    }
}


