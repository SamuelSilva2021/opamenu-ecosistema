using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.AccessControl.Module
{
    [Route("api/modules")]
    [ApiController]
    [Authorize]
    public class ModuleController(IModuleService moduleTypeService) : BaseController
    {
        private readonly IModuleService _moduleService = moduleTypeService;

        [HttpGet]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ModuleDTO>>> GetAllModulePagedAsync([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            //Deixei em 100 por que essa rota carrega as permissões do front. Depois vou reestruturar melhor
            var response = await _moduleService.GetAllModulePagedAsync(page, limit);
            return BuildResponse(response);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> GetModuleByIdAsync([FromRoute] Guid id)
        {
            var response = await _moduleService.GetModuleByIdAsync(id);
            return BuildResponse(response);
        }

        [HttpPost]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> AddModuleAsync([FromBody] ModuleCreateDTO moduleType)
        {
            var response = await _moduleService.AddModuleAsync(moduleType);
            return BuildResponse(response);
        }
        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> UpdateModuleTypeAsync([FromRoute] Guid id, [FromBody] ModuleUpdateDTO moduleType)
        {
            var response = await _moduleService.UpdateModuleAsync(id, moduleType);
            return BuildResponse(response);
        }

        [HttpPatch]
        [Route("{id:guid}/toggle-status")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> ToggleStatus([FromRoute] Guid id)
        {
            var response = await _moduleService.ToggleStatus(id);
            return BuildResponse(response);
        }
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteModuleTypeAsync([FromRoute] Guid id)
        {
            var response = await _moduleService.DeleteModuleAsync(id);
            return BuildResponse(response);
        }
    }
}
