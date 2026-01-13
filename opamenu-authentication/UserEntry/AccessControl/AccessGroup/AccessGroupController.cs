using Authenticator.API.Core.Application.Interfaces.AccessControl.AccessGroup;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using Authenticator.API.Infrastructure.Configurations;
using Authenticator.API.Infrastructure.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Authenticator.API.UserEntry.AccessControl.AccessGroup
{
    /// <summary>
    /// Controlador para gerenciar grupos de acesso
    /// </summary>
    [Route("api/access-group")]
    [ApiController]
    [Produces("application/json")]
    [Tags("Grupos de Acesso")]
    [Authorize]
    public class AccessGroupController(
        ILogger<AccessGroupController> logger,
        IGroupTypeService groupTypeService,
        IAccessGroupService accessGroupService
        ) : BusinessControleBase
    {
        private readonly ILogger<AccessGroupController> _logger = logger;
        private readonly IGroupTypeService _groupTypeService = groupTypeService;
        private readonly IAccessGroupService _accessGroupService = accessGroupService;

        #region GET
        [HttpGet]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_SELECT)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<AccessGroupDTO>>>> GetAllAccessGroups([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var accessGroups = await _accessGroupService.GetPagedAsync(page, limit);
            return StatusCode(accessGroups.Code, accessGroups);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_SELECT)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> GetAccessGroupById([FromRoute] Guid id)
        {
            var accessGroup = await _accessGroupService.GetByIdAsync(id);
            return StatusCode(accessGroup.Code, accessGroup);
        }

        [HttpGet("group-types")]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_SELECT)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<PagedResponseDTO<GroupTypeDTO>>>> GetAllGroupTypes([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await _groupTypeService.GetPagedAsync(page, limit);
            return StatusCode(response.Code, response);
        }

        [HttpGet("group-types/{id:guid}")]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_SELECT)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> GetGroupTypeById([FromRoute] Guid id)
        {
            var groupType = await _groupTypeService.GetByIdAsync(id);
            return StatusCode(groupType.Code, groupType);
        }

        #endregion GET

        #region POST
        [HttpPost]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_INSERT)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> CreateAccessGroup([FromBody] CreateAccessGroupDTO createAccessGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var response = await _accessGroupService.CreateAsync(createAccessGroupDTO);
            return StatusCode(response.Code, response);
        }

        [HttpPost("group-types")]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_INSERT)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> CreateGroupType([FromBody] GroupTypeCreateDTO groupTypeCreateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _groupTypeService.CreateAsync(groupTypeCreateDTO);
            return StatusCode(response.Code, response);
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_UPDATE)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> UpdateAccessGroup([FromRoute] Guid id, [FromBody] UpdateAccessGroupDTO updateAccessGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _accessGroupService.UpdateAsync(id, updateAccessGroupDTO);
            return StatusCode(response.Code, response);
        }


        [HttpPut("group-types/{id:guid}")]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_UPDATE)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> UpdateGroupType([FromRoute] Guid id, [FromBody] GroupTypeUpdateDTO groupTypeUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _groupTypeService.UpdateAsync(id, groupTypeUpdateDTO);
            return StatusCode(response.Code, response);
        }

        #endregion PUT

        #region DELETE
        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_DELETE)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> DeleteAccessGroup([FromRoute] Guid id)
        {
            var response = await _accessGroupService.DeleteAsync(id);
            return StatusCode(response.Code, response);
        }

        [HttpDelete("group-types/{id:guid}")]
        [Produces("application/json")]
        [MapPermission(MODULE_ACCESS_GROUP, OPERATION_DELETE)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> DeleteGroupType([FromRoute] Guid id)
        {
            var response = await _groupTypeService.DeleteAsync(id);
            return StatusCode(response.Code, response);
        }

        #endregion DELETE
    }
}
