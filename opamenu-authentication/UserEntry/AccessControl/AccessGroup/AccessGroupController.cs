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
        ) : BaseController
    {
        private readonly ILogger<AccessGroupController> _logger = logger;
        private readonly IGroupTypeService _groupTypeService = groupTypeService;
        private readonly IAccessGroupService _accessGroupService = accessGroupService;

        #region GET
        [HttpGet]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<AccessGroupDTO>>>> GetAllAccessGroups([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await _accessGroupService.GetPagedAsync(page, limit);
            return BuildResponse(response);
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> GetAccessGroupById([FromRoute] Guid id)
        {
            var response = await _accessGroupService.GetByIdAsync(id);
            return BuildResponse(response);
        }

        [HttpGet("group-types")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<PagedResponseDTO<GroupTypeDTO>>>> GetAllGroupTypes([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await _groupTypeService.GetPagedAsync(page, limit);
            return BuildResponse(response);
        }

        [HttpGet("group-types/{id:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> GetGroupTypeById([FromRoute] Guid id)
        {
            var response = await _groupTypeService.GetByIdAsync(id);
            return BuildResponse(response);
        }

        #endregion GET

        #region POST
        [HttpPost]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> CreateAccessGroup([FromBody] CreateAccessGroupDTO createAccessGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _accessGroupService.CreateAsync(createAccessGroupDTO);
            return BuildResponse(response);
        }

        [HttpPost("group-types")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> CreateGroupType([FromBody] GroupTypeCreateDTO groupTypeCreateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _groupTypeService.CreateAsync(groupTypeCreateDTO);
            return BuildResponse(response);
        }
        #endregion

        #region PUT
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
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
            return BuildResponse(response);
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<AccessGroupDTO>>> ToggleStatusAccessGroup([FromRoute] Guid id, [FromBody] UpdateAccessGroupDTO updateAccessGroupDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _accessGroupService.ToggleStatus(id, updateAccessGroupDTO);
            return BuildResponse(response);
        }


        [HttpPut("group-types/{id:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
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
            return BuildResponse(response);
        }

        [HttpPut("group-types/{id:guid}/toggle-status")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<GroupTypeDTO>>> ToggleStatusGroupType([FromRoute] Guid id, [FromBody] GroupTypeUpdateDTO groupTypeUpdateDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var response = await _groupTypeService.ToggleStatus(id, groupTypeUpdateDTO);
            return BuildResponse(response);
        }

        #endregion PUT

        #region DELETE
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> DeleteAccessGroup([FromRoute] Guid id)
        {
            var response = await _accessGroupService.DeleteAsync(id);
            return BuildResponse(response);
        }

        [HttpDelete("group-types/{id:guid}")]
        [Authorize(Roles = "SUPER_ADMIN")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> DeleteGroupType([FromRoute] Guid id)
        {
            var response = await _groupTypeService.DeleteAsync(id);
            return BuildResponse(response);
        }

        #endregion DELETE
    }
}
