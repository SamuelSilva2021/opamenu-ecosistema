using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Authenticator.API.UserEntry.AccessControl.Roles
{
    /// <summary>
    /// Controller para gerenciamento de roles (papéis)
    /// </summary>
    [Route("api/roles")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Roles")]
    [Authorize]
    public class RoleController(
        ILogger<RoleController> logger,
        IRoleService roleService
        ) : ControllerBase
    {
        private readonly ILogger<RoleController> _logger = logger;
        private readonly IRoleService _roleService = roleService;

        #region GET

        /// <summary>
        /// Busca todos os roles com paginação
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="limit">Limite de itens por página</param>
        /// <returns>Lista paginada de roles</returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Lista roles com paginação",
            Description = "Retorna uma lista paginada de todos os roles do tenant atual"
        )]
        public async Task<ActionResult> GetAllRolesPaged([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                _logger.LogInformation("Buscando roles - Página: {Page}, Limite: {Limit}", page, limit);
                
                var response = await _roleService.GetAllRolesPagedAsync(page, limit);
                
                _logger.LogInformation("Roles encontrados: {Count}", response.Data?.Items?.Count() ?? 0);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar roles paginados");
                throw;
            }
        }

        /// <summary>
        /// Busca role por ID
        /// </summary>
        /// <param name="id">ID do role</param>
        /// <returns>Role encontrado</returns>
        [HttpGet("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Busca role por ID",
            Description = "Retorna um role específico pelo seu ID"
        )]
        public async Task<ActionResult> GetRoleById([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation("Buscando role por ID: {RoleId}", id);
                
                var response = await _roleService.GetRoleByIdAsync(id);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar role por ID: {RoleId}", id);
                throw;
            }
        }

        /// <summary>
        /// Busca permissões por role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <returns>Lista de permissões do role</returns>
        [HttpGet("{roleId:guid}/permissions")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Busca permissões do role",
            Description = "Retorna todas as permissões associadas ao role"
        )]
        public async Task<ActionResult<IEnumerable<PermissionDTO>>> GetPermissionsByRole([FromRoute] Guid roleId)
        {
            try
            {
                _logger.LogInformation("Buscando permissões do role: {RoleId}", roleId);
                
                var response = await _roleService.GetPermissionsByRoleAsync(roleId);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar permissões do role: {RoleId}", roleId);
                throw;
            }
        }

        /// <summary>
        /// Busca grupos de acesso por role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <returns>Lista de grupos de acesso do role</returns>
        [HttpGet("{roleId:guid}/access-groups")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Busca grupos de acesso do role",
            Description = "Retorna todos os grupos de acesso associados ao role"
        )]
        public async Task<ActionResult<IEnumerable<AccessGroupDTO>>> GetAccessGroupsByRole([FromRoute] Guid roleId)
        {
            try
            {
                _logger.LogInformation("Buscando grupos de acesso do role: {RoleId}", roleId);
                
                var response = await _roleService.GetAccessGroupsByRoleAsync(roleId);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar grupos de acesso do role: {RoleId}", roleId);
                throw;
            }
        }

        #endregion

        #region POST

        /// <summary>
        /// Cria um novo role
        /// </summary>
        /// <param name="dto">Dados para criação do role</param>
        /// <returns>Role criado</returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Cria novo role",
            Description = "Cria um novo role no sistema"
        )]
        public async Task<ActionResult> CreateRole([FromBody] RoleCreateDTO dto)
        {
            try
            {
                _logger.LogInformation("Criando novo role: {RoleName}", dto.Name);
                
                var response = await _roleService.AddRoleAsync(dto);
                
                if (response.Succeeded)
                {
                    _logger.LogInformation("Role criado com sucesso: {RoleId}", response.Data?.Id);
                    return StatusCode(response.Code, response);
                }
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar role: {RoleName}", dto.Name);
                throw;
            }
        }

        /// <summary>
        /// Atribui permissões a um role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <param name="permissionIds">Lista de IDs das permissões</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{roleId:guid}/permissions")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Atribui permissões ao role",
            Description = "Associa uma lista de permissões ao role especificado"
        )]
        public async Task<ActionResult> AssignPermissionsToRole([FromRoute] Guid roleId, [FromBody] List<Guid> permissionIds)
        {
            try
            {
                _logger.LogInformation("Atribuindo permissões ao role: {RoleId}, Permissões: {Count}", roleId, permissionIds.Count);
                
                var response = await _roleService.AssignPermissionsToRoleAsync(roleId, permissionIds);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atribuir permissões ao role: {RoleId}", roleId);
                throw;
            }
        }

        /// <summary>
        /// Atribui grupos de acesso a um role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <param name="accessGroupIds">Lista de IDs dos grupos de acesso</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{roleId:guid}/access-groups")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Atribui grupos de acesso ao role",
            Description = "Associa uma lista de grupos de acesso ao role especificado"
        )]
        public async Task<ActionResult> AssignAccessGroupsToRole([FromRoute] Guid roleId, [FromBody] List<Guid> accessGroupIds)
        {
            try
            {
                _logger.LogInformation("Atribuindo grupos de acesso ao role: {RoleId}, Grupos: {Count}", roleId, accessGroupIds.Count);
                
                var response = await _roleService.AssignAccessGroupsToRoleAsync(roleId, accessGroupIds);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atribuir grupos de acesso ao role: {RoleId}", roleId);
                throw;
            }
        }

        #endregion

        #region PUT

        /// <summary>
        /// Atualiza um role
        /// </summary>
        /// <param name="id">ID do role</param>
        /// <param name="dto">Dados para atualização</param>
        /// <returns>Role atualizado</returns>
        [HttpPut("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Atualiza role",
            Description = "Atualiza os dados de um role existente"
        )]
        public async Task<ActionResult> UpdateRole([FromRoute] Guid id, [FromBody] RoleUpdateDTO dto)
        {
            try
            {
                _logger.LogInformation("Atualizando role: {RoleId}", id);
                
                var response = await _roleService.UpdateRoleAsync(id, dto);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar role: {RoleId}", id);
                throw;
            }
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Remove um role
        /// </summary>
        /// <param name="id">ID do role</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Remove role",
            Description = "Remove um role do sistema"
        )]
        public async Task<ActionResult> DeleteRole([FromRoute] Guid id)
        {
            try
            {
                _logger.LogInformation("Removendo role: {RoleId}", id);
                
                var response = await _roleService.DeleteRoleAsync(id);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover role: {RoleId}", id);
                throw;
            }
        }

        /// <summary>
        /// Remove permissões de um role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <param name="permissionIds">Lista de IDs das permissões</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{roleId:guid}/permissions")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Remove permissões do role",
            Description = "Remove a associação de permissões do role especificado"
        )]
        public async Task<ActionResult> RemovePermissionsFromRole([FromRoute] Guid roleId, [FromBody] List<Guid> permissionIds)
        {
            try
            {
                _logger.LogInformation("Removendo permissões do role: {RoleId}, Permissões: {Count}", roleId, permissionIds.Count);
                
                var response = await _roleService.RemovePermissionsFromRoleAsync(roleId, permissionIds);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover permissões do role: {RoleId}", roleId);
                throw;
            }
        }

        /// <summary>
        /// Remove grupos de acesso de um role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <param name="accessGroupIds">Lista de IDs dos grupos de acesso</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{roleId:guid}/access-groups")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Remove grupos de acesso do role",
            Description = "Remove a associação de grupos de acesso do role especificado"
        )]
        public async Task<ActionResult> RemoveAccessGroupsFromRole([FromRoute] Guid roleId, [FromBody] List<Guid> accessGroupIds)
        {
            try
            {
                _logger.LogInformation("Removendo grupos de acesso do role: {RoleId}, Grupos: {Count}", roleId, accessGroupIds.Count);
                
                var response = await _roleService.RemoveAccessGroupsFromRoleAsync(roleId, accessGroupIds);
                
                return StatusCode(response.Code, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover grupos de acesso do role: {RoleId}", roleId);
                throw;
            }
        }

        #endregion
    }
}