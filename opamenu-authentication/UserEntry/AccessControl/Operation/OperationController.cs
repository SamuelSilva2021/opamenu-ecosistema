using Authenticator.API.Core.Application.Interfaces.AccessControl.Operation;
using Authenticator.API.Core.Domain.AccessControl.Operations.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry.AccessControl.Operation
{
    /// <summary>
    /// Operation Controller
    /// </summary>
    /// <param name="operationService"></param>
    [Route("api/operation")]
    [ApiController]
    [Authorize]
    public class OperationController(IOperationService operationService) : BaseController
    {
        private readonly IOperationService _operationService = operationService;

        [HttpGet]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OperationDTO>>> GetAllOperationAsync([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await _operationService.GetAllOperationPagedAsync(page, limit);
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
        public async Task<ActionResult<OperationDTO>> GetOperationByIdAsync([FromRoute] Guid id)
        {
            var response = await _operationService.GetOperationByIdAsync(id);
            return BuildResponse(response);
        }

        [HttpPost]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OperationDTO>> AddOperationAsync([FromBody] OperationCreateDTO operation)
        {
            var response = await _operationService.AddOperationAsync(operation);
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
        public async Task<ActionResult<bool>> DeleteOperationAsync([FromRoute] Guid id)
        {
            var response = await _operationService.DeleteOperationAsync(id);
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
        public async Task<ActionResult<OperationDTO>> UpdateOperationAsync([FromRoute] Guid id, [FromBody] OperationUpdateDTO operation)
        {
            var response = await _operationService.UpdateOperationAsync(id, operation);
            return BuildResponse(response);
        }
    }
}
