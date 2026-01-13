using Authenticator.API.Core.Application.Interfaces.AccessControl.UserAccounts;
using Authenticator.API.Core.Application.Interfaces.AccessControl.AccountAccessGroups;
using Authenticator.API.Core.Domain.AccessControl.AccountAccessGroups.DTOs;
using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using Authenticator.API.Core.Domain.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Authenticator.API.UserEntry.Users
{
    [ApiController]
    [Route("api/users")]
    [Produces("application/json")]
    [Authorize]
    [Tags("Usuários")]
    public class UsersController(IUserAccountService userAccountService, IAccountAccessGroupService accountAccessGroupService) : ControllerBase
    {
        private readonly IUserAccountService _userService = userAccountService;
        private readonly IAccountAccessGroupService _accountAccessGroupService = accountAccessGroupService;

        /// <summary>
        /// Lista usuários do tenant atual com paginação
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<PagedResponseDTO<UserAccountDTO>>>> GetPaged([FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await _userService.GetAllUserAccountsPagedAsync(page, limit);
            return Ok(response);
        }

        /// <summary>
        /// Lista todos os usuários ativos do tenant atual
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<UserAccountDTO>>>> GetActive()
        {
            var response = await _userService.GetAllActiveUsersAsync();
            return Ok(response);
        }

        /// <summary>
        /// Obtém detalhes de um usuário por ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<UserAccountDTO>>> GetById([FromRoute] Guid id)
        {
            var response = await _userService.GetUserAccountByIdAsync(id);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Cria um novo usuário no tenant atual
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<UserAccountDTO>>> Create([FromBody] UserAccountCreateDTO request)
        {
            var response = await _userService.AddUserAccountAsync(request);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Atualiza dados de um usuário
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<UserAccountDTO>>> Update([FromRoute] Guid id, [FromBody] UserAccountUpdateDTO request)
        {
            var response = await _userService.UpdateUserAccountAsync(id, request);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Remove um usuário por ID
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> Delete([FromRoute] Guid id)
        {
            var response = await _userService.DeleteUserAccountAsync(id);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Altera a senha do usuário autenticado
        /// </summary>
        //[HttpPost("change-password")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<ResponseDTO<bool>>> ChangePassword([FromBody] UserAccountChangePasswordDTO request)
        //{
        //    var response = await _userService.ChangePasswordAsync(request);
        //    return StatusCode(response.Code, response);
        //}

        /// <summary>
        /// Inicia fluxo de esqueci a senha
        /// </summary>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> ForgotPassword([FromBody] UserAccountForgotPasswordDTO request)
        {
            var response = await _userService.ForgotPasswordAsync(request);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Reseta a senha usando token de reset
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> ResetPassword([FromBody] UserAccountResetPasswordDTO request)
        {
            var response = await _userService.ResetPasswordAsync(request);
            return StatusCode(response.Code, response);
        }

        #region User Access Groups
        /// <summary>
        /// Lista grupos de acesso vinculados a um usuário
        /// </summary>
        [HttpGet("{id:guid}/access-groups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<AccessGroupDTO>>>> GetUserAccessGroups([FromRoute] Guid id)
        {
            var response = await _accountAccessGroupService.GetUserAccessGroupsAsync(id);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Atribui grupos de acesso a um usuário
        /// </summary>
        [HttpPost("{id:guid}/access-groups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> AssignUserAccessGroups([FromRoute] Guid id, [FromBody] AssignUserAccessGroupsDTO request)
        {
            var response = await _accountAccessGroupService.AssignAccessGroupsAsync(id, request);
            return StatusCode(response.Code, response);
        }

        /// <summary>
        /// Revoga um grupo de acesso de um usuário
        /// </summary>
        [HttpDelete("{id:guid}/access-groups/{groupId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDTO<bool>>> RevokeUserAccessGroup([FromRoute] Guid id, [FromRoute] Guid groupId)
        {
            var response = await _accountAccessGroupService.RevokeAccessGroupAsync(id, groupId);
            return StatusCode(response.Code, response);
        }
        #endregion
    }
}