using Authenticator.API.Core.Application.Interfaces.AccessControl.Roles;
using Authenticator.API.Core.Domain.AccessControl.Roles.DTOs;
using Authenticator.API.Core.Domain.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.AccessControl.Roles
{
    [ApiController]
    [Route("api/roles-painel")]
    [Produces("application/json")]
    [Authorize]
    [Tags("Roles (Painel)")]
    public class RolesPainelController(IRolePainelService rolePainelService) : BaseController
    {
        private readonly IRolePainelService _roleService = rolePainelService;

        /// <summary>
        /// Lista roles do tenant atual com paginação
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseDTO<PagedResponseDTO<RoleDTO>>>> GetPaged([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] string? name = null)
        {
            var response = await _roleService.GetAllRolesPagedAsync(page, limit, name);
            return BuildResponse(response);
        }

        /// <summary>
        /// Obtém detalhes de um role por ID (restrito ao tenant)
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseDTO<RoleDTO>>> GetById([FromRoute] Guid id)
        {
            var response = await _roleService.GetRoleByIdAsync(id);
            return BuildResponse(response);
        }

        /// <summary>
        /// Cria um novo role para o tenant atual
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseDTO<RoleDTO>>> Create([FromBody] RoleCreateDTO dto)
        {
            var response = await _roleService.AddRoleAsync(dto);
            return BuildResponse(response);
        }

        /// <summary>
        /// Atualiza um role (restrito ao tenant)
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseDTO<RoleDTO>>> Update([FromRoute] Guid id, [FromBody] RoleUpdateDTO dto)
        {
            var response = await _roleService.UpdateRoleAsync(id, dto);
            return BuildResponse(response);
        }

        /// <summary>
        /// Exclui um role (restrito ao tenant)
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseDTO<bool>>> Delete([FromRoute] Guid id)
        {
            var response = await _roleService.DeleteRoleAsync(id);
            return BuildResponse(response);
        }

        /// <summary>
        /// Lista módulos disponíveis para atribuição de permissões
        /// </summary>
        [HttpGet("modules")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<SimplifiedModuleDTO>>>> GetModules()
        {
            var response = await _roleService.GetAvailableModulesAsync();
            return BuildResponse(response);
        }
    }
}
