using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Application.DTOs;
using OpaMenu.Web.UserEntry;

namespace OpaMenu.Web.UserEntry.Coupon;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CouponController(ICouponService couponService) : BaseController
{
    private readonly ICouponService _couponService = couponService;

    [HttpGet]
    public async Task<ActionResult<ResponseDTO<IEnumerable<CouponDto>>>> GetAll()
    {
        var result = await _couponService.GetAllAsync();
        return BuildResponse(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ResponseDTO<CouponDto?>>> GetById(int id)
    {
        var result = await _couponService.GetByIdAsync(id);
        return BuildResponse(result);
    }

    [HttpPost]
    public async Task<ActionResult<ResponseDTO<CouponDto>>> Create(CreateCouponRequestDto dto)
    {
        var result = await _couponService.CreateAsync(dto);
        return BuildResponse(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseDTO<CouponDto>>> Update(int id, UpdateCouponRequestDto dto)
    {
        var result = await _couponService.UpdateAsync(id, dto);
        return BuildResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseDTO<bool>>> Delete(int id)
    {
        var result = await _couponService.DeleteAsync(id);
        return BuildResponse(result);
    }
}
