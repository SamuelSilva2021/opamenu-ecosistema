using Authenticator.API.Core.Application.Interfaces.AccessControl.Module;
using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.AccessControl.Module
{
    [Route("api/modules")]
    [ApiController]
    [Authorize(Roles = "SUPER_ADMIN")]
    public class ModuleController(IModuleService moduleTypeService) : ControllerBase
    {
        private readonly IModuleService _moduleService = moduleTypeService;

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ModuleDTO>>> GetAllModulePagedAsync([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await _moduleService.GetAllModulePagedAsync(page, limit);
            return Ok(response);
        }

        [HttpGet]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> GetModuleByIdAsync([FromRoute] Guid id)
        {
            var response = await _moduleService.GetModuleByIdAsync(id);
            return StatusCode(response.Code, response);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> AddModuleAsync([FromBody] ModuleCreateDTO moduleType)
        {
            var response = await _moduleService.AddModuleAsync(moduleType);
            return StatusCode(response.Code, response);
        }
        [HttpPut]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ModuleDTO>> UpdateModuleTypeAsync([FromRoute] Guid id, [FromBody] ModuleUpdateDTO moduleType)
        {
            var response = await _moduleService.UpdateModuleAsync(id, moduleType);
            return StatusCode(response.Code, response);
        }
        [HttpDelete]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteModuleTypeAsync([FromRoute] Guid id)
        {
            var response = await _moduleService.DeleteModuleAsync(id);
            return StatusCode(response.Code, response);
        }
    }
}
