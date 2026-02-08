using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
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
        ) : BaseController
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
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Lista roles com paginação",
            Description = "Retorna uma lista paginada de todos os roles do tenant atual"
        )]
        public async Task<ActionResult> GetAllRolesPaged([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await _roleService.GetAllRolesPagedAsync(page, limit);
            return BuildResponse(response);
        }

        /// <summary>
        /// Busca role por ID
        /// </summary>
        /// <param name="id">ID do role</param>
        /// <returns>Role encontrado</returns>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Busca role por ID",Description = "Retorna um role específico pelo seu ID"
        )]
        public async Task<ActionResult> GetRoleById([FromRoute] Guid id)
        {
            var response = await _roleService.GetRoleByIdAsync(id);
            return BuildResponse(response);
        }

        /// <summary>
        /// Busca permissões por role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <returns>Lista de permissões do role</returns>
        [HttpGet("{roleId:guid}/permissions")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Busca permissões do role",Description = "Retorna todas as permissões associadas ao role"
        )]
        public async Task<ActionResult<IEnumerable<SimplifiedPermissionDTO>>> GetPermissionsByRole([FromRoute] Guid roleId)
        {
            var response = await _roleService.GetPermissionsByRoleAsync(roleId);
            return BuildResponse(response);
        }

        /// <summary>
        /// Busca grupos de acesso por role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <returns>Lista de grupos de acesso do role</returns>
        [HttpGet("{roleId:guid}/access-groups")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Busca grupos de acesso do role",Description = "Retorna todos os grupos de acesso associados ao role"
        )]
        public async Task<ActionResult<IEnumerable<AccessGroupDTO>>> GetAccessGroupsByRole([FromRoute] Guid roleId)
        {
            var response = await _roleService.GetAccessGroupsByRoleAsync(roleId);
            return BuildResponse(response);
        }

        #endregion

        #region POST

        /// <summary>
        /// Cria um novo role
        /// </summary>
        /// <param name="dto">Dados para criação do role</param>
        /// <returns>Role criado</returns>
        [HttpPost]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Cria novo role",Description = "Cria um novo role no sistema"
        )]
        public async Task<ActionResult> CreateRole([FromBody] RoleCreateDTO dto)
        {
            var response = await _roleService.AddRoleAsync(dto);
            return BuildResponse(response);
        }

        /// <summary>
        /// Atribui permissões a um role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{roleId:guid}/permissions")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
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
        public async Task<ActionResult> AssignPermissionsToRole([FromRoute] Guid roleId, [FromBody] List<SimplifiedPermissionDTO> permissions)
        {
            var response = await _roleService.AssignPermissionsToRoleAsync(roleId, permissions);
            return BuildResponse(response);
        }

        /// <summary>
        /// Atribui grupos de acesso a um role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <param name="accessGroupIds">Lista de IDs dos grupos de acesso</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("{roleId:guid}/access-groups")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Atribui grupos de acesso ao role",Description = "Associa uma lista de grupos de acesso ao role especificado")]
        public async Task<ActionResult> AssignAccessGroupsToRole([FromRoute] Guid roleId, [FromBody] List<Guid> accessGroupIds)
        {
            var response = await _roleService.AssignAccessGroupsToRoleAsync(roleId, accessGroupIds);
            return BuildResponse(response);
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
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Atualiza role",Description = "Atualiza os dados de um role existente")]
        public async Task<ActionResult> UpdateRole([FromRoute] Guid id, [FromBody] RoleUpdateDTO dto)
        {
            var response = await _roleService.UpdateRoleAsync(id, dto);
            return BuildResponse(response);
        }

        /// <summary>
        /// Atualiza o status
        /// </summary>
        /// <param name="id">ID do role</param>
        /// <param name="dto">Dados para atualização</param>
        /// <returns>Role atualizado</returns>
        [HttpPut("{id:guid}/toggle-status")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Atualiza o status da role", Description = "Atualiza o status de um role existente")]
        public async Task<ActionResult> ToggleStatus([FromRoute] Guid id, [FromBody] RoleUpdateDTO dto)
        {
            var response = await _roleService.ToggleStatus(id, dto);
            return BuildResponse(response);
        }

        #endregion

        #region DELETE

        /// <summary>
        /// Remove um role
        /// </summary>
        /// <param name="id">ID do role</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Remove role",Description = "Remove um role do sistema")]
        public async Task<ActionResult> DeleteRole([FromRoute] Guid id)
        {
            var response = await _roleService.DeleteRoleAsync(id);
            return BuildResponse(response);
        }

        /// <summary>
        /// Remove permissões de um role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <param name="permissionIds">Lista de IDs das permissões</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{roleId:guid}/permissions")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Remove permissões do role",Description = "Remove a associação de permissões do role especificado")]
        public async Task<ActionResult> RemovePermissionsFromRole([FromRoute] Guid roleId, [FromBody] List<string> moduleKeys)
        {
            var response = await _roleService.RemovePermissionsFromRoleAsync(roleId, moduleKeys);
            return BuildResponse(response);
        }

        /// <summary>
        /// Remove grupos de acesso de um role
        /// </summary>
        /// <param name="roleId">ID do role</param>
        /// <param name="accessGroupIds">Lista de IDs dos grupos de acesso</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("{roleId:guid}/access-groups")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Remove grupos de acesso do role",Description = "Remove a associação de grupos de acesso do role especificado")]
        public async Task<ActionResult> RemoveAccessGroupsFromRole([FromRoute] Guid roleId, [FromBody] List<Guid> accessGroupIds)
        {
            var response = await _roleService.RemoveAccessGroupsFromRoleAsync(roleId, accessGroupIds);
            return BuildResponse(response);
        }

        #endregion
    }
}
