using Authenticator.API.Core.Application.Interfaces.AccessControl.Permissions;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.AccessControl.Permissions
{
    /// <summary>
    /// Controller para gerenciamento de permissões
    /// </summary>
    /// <param name="permissionService"></param>
    [Route("api/permissions")]
    [ApiController]
    [Authorize(Roles = "SUPER_ADMIN")]
    public class PermissionController(IPermissionService permissionService) : ControllerBase
    {
        private readonly IPermissionService _permissionService = permissionService;

        /// <summary>
        /// Busca todas as permissões
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PermissionDTO>>> GetAllPermissionsAsync()
        {
            var response = await _permissionService.GetAllPermissionsAsync();
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Busca permissões paginadas
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="limit">Limite de itens por página</param>
        /// <returns></returns>
        [HttpGet]
        [Route("paged")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPermissionsPagedAsync([FromQuery] int page = 1, [FromQuery] int limit = 20)
        {
            var response = await _permissionService.GetAllPermissionsPagedAsync(page, limit);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Busca uma permissão pelo ID
        /// </summary>
        /// <param name="id">ID da permissão</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionDTO>> GetPermissionByIdAsync([FromRoute] Guid id)
        {
            var response = await _permissionService.GetPermissionByIdAsync(id);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Busca permissões por módulo
        /// </summary>
        /// <param name="moduleId">ID do módulo</param>
        /// <returns></returns>
        [HttpGet]
        [Route("by-module/{moduleId:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PermissionDTO>>> GetPermissionsByModuleAsync([FromRoute] Guid moduleId)
        {
            var response = await _permissionService.GetPermissionsByModuleAsync(moduleId);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Busca permissões por role
        /// </summary>
        /// <param name="roleId">ID da role</param>
        /// <returns></returns>
        [HttpGet]
        [Route("by-role/{roleId:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PermissionDTO>>> GetPermissionsByRoleAsync([FromRoute] Guid roleId)
        {
            var response = await _permissionService.GetPermissionsByRoleAsync(roleId);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Adiciona uma nova permissão
        /// </summary>
        /// <param name="permission">Dados da permissão</param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionDTO>> AddPermissionAsync([FromBody] PermissionCreateDTO permission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _permissionService.AddPermissionAsync(permission);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Atualiza uma permissão
        /// </summary>
        /// <param name="id">ID da permissão</param>
        /// <param name="permission">Dados atualizados da permissão</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PermissionDTO>> UpdatePermissionAsync([FromRoute] Guid id, [FromBody] PermissionUpdateDTO permission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _permissionService.UpdatePermissionAsync(id, permission);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Deleta uma permissão
        /// </summary>
        /// <param name="id">ID da permissão</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeletePermissionAsync([FromRoute] Guid id)
        {
            var response = await _permissionService.DeletePermissionAsync(id);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Associa operações a uma permissão
        /// </summary>
        /// <param name="permissionId">ID da permissão</param>
        /// <param name="operationIds">Lista de IDs das operações</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{permissionId:guid}/operations")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> AssignOperationsToPermissionAsync([FromRoute] Guid permissionId, [FromBody] List<Guid> operationIds)
        {
            if (operationIds == null || !operationIds.Any())
            {
                return BadRequest("Lista de operações não pode estar vazia");
            }

            var response = await _permissionService.AssignOperationsToPermissionAsync(permissionId, operationIds);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Remove operações de uma permissão
        /// </summary>
        /// <param name="permissionId">ID da permissão</param>
        /// <param name="operationIds">Lista de IDs das operações</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{permissionId:guid}/operations")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> RemoveOperationsFromPermissionAsync([FromRoute] Guid permissionId, [FromBody] List<Guid> operationIds)
        {
            if (operationIds == null || !operationIds.Any())
            {
                return BadRequest("Lista de operações não pode estar vazia");
            }

            var response = await _permissionService.RemoveOperationsFromPermissionAsync(permissionId, operationIds);
            return StatusCode(response.Code, response);
        }
    }
}