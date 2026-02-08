using Authenticator.API.Core.Application.Interfaces.AccessControl.UserAccounts;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.Users
{
    [ApiController]
    [Route("api/user-accounts-painel")]
    [Produces("application/json")]
    [Authorize]
    [Tags("Colaboradores (Painel)")]
    public class UserAccountsPainelController(IUserAccountPainelService userAccountPainelService) : BaseController
    {
        private readonly IUserAccountPainelService _userService = userAccountPainelService;

        /// <summary>
        /// Lista colaboradores do tenant atual com paginação
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<PagedResponseDTO<UserAccountDTO>>>> GetPaged([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] string? search = null)
        {
            var response = await _userService.GetAllEmployeePagedAsync(page, limit, search);
            return BuildResponse(response);
        }

        /// <summary>
        /// Obtém detalhes de um colaborador por ID (restrito ao tenant)
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseDTO<UserAccountDTO>>> GetById([FromRoute] Guid id)
        {
            var response = await _userService.GetEmployeeByIdAsync(id);
            return BuildResponse(response);
        }

        /// <summary>
        /// Cria um novo colaborador no tenant atual
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ResponseDTO<UserAccountDTO>>> Create([FromBody] UserAccountCreateDTO request)
        {
            var response = await _userService.CreateEmployeeAsync(request);
            return BuildResponse(response);
        }

        /// <summary>
        /// Atualiza dados de um colaborador (restrito ao tenant)
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseDTO<UserAccountDTO>>> Update([FromRoute] Guid id, [FromBody] UserAccountUpdateDto request)
        {
            var response = await _userService.UpdateEmployeeAsync(id, request);
            return BuildResponse(response);
        }

        /// <summary>
        /// Altera o status de um colaborador (Ativo/Inativo)
        /// </summary>
        [HttpPatch("{id:guid}/toggle-status")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseDTO<UserAccountDTO>>> ToggleStatus([FromRoute] Guid id)
        {
            var response = await _userService.ToggleEmployeeStatusAsync(id);
            return BuildResponse(response);
        }

        /// <summary>
        /// Remove um colaborador (restrito ao tenant)
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseDTO<bool>>> Delete([FromRoute] Guid id)
        {
            var response = await _userService.DeleteEmployeeAsync(id);
            return BuildResponse(response);
        }
    }
}
