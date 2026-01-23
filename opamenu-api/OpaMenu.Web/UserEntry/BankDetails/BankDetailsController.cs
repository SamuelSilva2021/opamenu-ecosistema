using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.BankDetails;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Infrastructure.Anotations;

namespace OpaMenu.Web.UserEntry.BankDetails
{
    [Route("api/bank-details")]
    [ApiController]
    public class BankDetailsController(IBankDetailsService bankDetailsService) : BaseController
    {
        private readonly IBankDetailsService _bankDetailsService = bankDetailsService;

        [HttpGet]
        //[MapPermission(MODULE_BANK, OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<BankDetailsDto>>>> GetAll()
        {
            var serviceResponse = await _bankDetailsService.GetAllAsync();
            return BuildResponse(serviceResponse);
        }
        [HttpGet("{id}")]
        //[MapPermission(MODULE_BANK, OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<BankDetailsDto?>>> GetById(Guid id)
        {
            var result = await _bankDetailsService.GetByIdAsync(id);
            return BuildResponse(result);
        }

        [HttpPost]
        //[MapPermission(MODULE_BANK, OPERATION_INSERT)]
        public async Task<ActionResult<ResponseDTO<BankDetailsDto>>> Create(CreateBankDetailsRequestDto dto)
        {
            var result = await _bankDetailsService.CreateAsync(dto);
            return BuildResponse(result);
        }

        [HttpPut("{id}")]
        //[MapPermission(MODULE_BANK, OPERATION_UPDATE)]
        public async Task<ActionResult<ResponseDTO<BankDetailsDto>>> Update(Guid id, UpdateBankDetailsRequestDto dto)
        {
            var result = await _bankDetailsService.UpdateAsync(dto);
            return BuildResponse(result);
        }
        [HttpDelete("{id}")]
        //[MapPermission(MODULE_BANK, OPERATION_UPDATE)]
        public async Task<ActionResult<ResponseDTO<BankDetailsDto>>> Delete(Guid id)
        {
            var result = await _bankDetailsService.DeleteAsync(id);
            return BuildResponse(result);
        }
    }
}
