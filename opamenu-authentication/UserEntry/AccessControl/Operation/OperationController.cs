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

        /// <summary>
        /// Busca todas as operações
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Busca uma operação pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adiciona uma nova operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Deleta uma operação pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Atualiza uma operação pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
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
