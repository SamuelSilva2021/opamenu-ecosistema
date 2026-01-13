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
    [Authorize(Roles = "SUPER_ADMIN")]
    public class OperationController(IOperationService operationService) : ControllerBase
    {
        private readonly IOperationService _operationService = operationService;

        /// <summary>
        /// Busca todas as operações
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OperationDTO>>> GetAllOperationAsync()
        {
            var response = await _operationService.GetAllOperationAsync();
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Busca uma operação pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OperationDTO>> GetOperationByIdAsync([FromRoute] Guid id)
        {
            var response = await _operationService.GetOperationByIdAsync(id);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Adiciona uma nova operação
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OperationDTO>> AddOperationAsync([FromBody] OperationCreateDTO operation)
        {
            var response = await _operationService.AddOperationAsync(operation);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Deleta uma operação pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> DeleteOperationAsync([FromRoute] Guid id)
        {
            var response = await _operationService.DeleteOperationAsync(id);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Atualiza uma operação pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OperationDTO>> UpdateOperationAsync([FromRoute] Guid id, [FromBody] OperationUpdateDTO operation)
        {
            var response = await _operationService.UpdateOperationAsync(id, operation);
            return StatusCode(response.Code, response);
        }
    }
}
