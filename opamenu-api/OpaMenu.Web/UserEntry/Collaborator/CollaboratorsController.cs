using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Collaborator;
using OpaMenu.Web.UserEntry.Http;

namespace OpaMenu.Web.UserEntry.Collaborator;

[ApiController]
[Route("api/collaborators")]
[Produces("application/json")]
[Authorize]
[Tags("Colaboradores")]
public class CollaboratorsController(ICollaboratorService service) : BaseController
{
    private readonly ICollaboratorService _service = service;

    [HttpPost]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<CollaboratorResponseDto>>> Create([FromBody] CreateCollaboratorRequestDto request)
    {
        var result = await _service.CreateAsync(request);
        return BuildResponse(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<CollaboratorResponseDto>>> Update(Guid id, [FromBody] UpdateCollaboratorRequestDto request)
    {
        var result = await _service.UpdateAsync(id, request);
        return BuildResponse(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<bool>>> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return BuildResponse(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<CollaboratorResponseDto>>> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return BuildResponse(result);
    }

    [HttpGet]
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    public async Task<ActionResult<ResponseDTO<IEnumerable<CollaboratorResponseDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return BuildResponse(result);
    }
}
