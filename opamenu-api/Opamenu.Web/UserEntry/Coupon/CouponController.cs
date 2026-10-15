using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opamenu.Domain.DTOs.Coupon;
using Opamenu.Application.DTOs;
using Opamenu.Web.UserEntry;
using Opamenu.Commons.Api.DTOs;
using Opamenu.Infrastructure.Anotations;
using Opamenu.Infrastructure.Filters;
using Opamenu.Application.Services.Interfaces.Opamenu;

namespace Opamenu.Web.UserEntry.Coupon;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[ServiceFilter(typeof(PermissionAuthorizationFilter))]
public class CouponController(ICouponService couponService) : BaseController
{
    private readonly ICouponService _couponService = couponService;

    [HttpGet]
    [MapPermission(MODULE_COUPON, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<IEnumerable<CouponDto>>>> GetAll()
    {
        var result = await _couponService.GetAllAsync();
        return BuildResponse(result);
    }

    [HttpGet("{id}")]
    [MapPermission(MODULE_COUPON, OPERATION_SELECT)]
    public async Task<ActionResult<ResponseDTO<CouponDto?>>> GetById(Guid id)
    {
        var result = await _couponService.GetByIdAsync(id);
        return BuildResponse(result);
    }

    [HttpPost]
    [MapPermission(MODULE_COUPON, OPERATION_INSERT)]
    public async Task<ActionResult<ResponseDTO<CouponDto>>> Create(CreateCouponRequestDto dto)
    {
        var result = await _couponService.CreateAsync(dto);
        return BuildResponse(result);
    }

    [HttpPut("{id}")]
    [MapPermission(MODULE_COUPON, OPERATION_UPDATE)]
    public async Task<ActionResult<ResponseDTO<CouponDto>>> Update(Guid id, UpdateCouponRequestDto dto)
    {
        var result = await _couponService.UpdateAsync(id, dto);
        return BuildResponse(result);
    }

    [HttpDelete("{id}")]
    [MapPermission(MODULE_COUPON, OPERATION_DELETE)]
    public async Task<ActionResult<ResponseDTO<bool>>> Delete(Guid id)
    {
        var result = await _couponService.DeleteAsync(id);
        return BuildResponse(result);
    }
}
