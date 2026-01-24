using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Domain.DTOs.Customer;
using OpaMenu.Infrastructure.Anotations;
using OpaMenu.Infrastructure.Filters;

namespace OpaMenu.Web.UserEntry.Customer
{
    [Route("api/customers")]
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(PermissionAuthorizationFilter))]
    public class CustomersController(
        ILogger<CustomersController> logger,
        ICustomerService customerService
        ) : BaseController
    {
        private readonly ILogger<CustomersController> _logger = logger;
        private readonly ICustomerService _customerService = customerService;

        [HttpGet]
        [MapPermission(MODULE_CUSTOMER, OPERATION_SELECT)]
        public async Task<ActionResult<ResponseDTO<IEnumerable<CustomerResponseDto>>>> GetAllCustomers()
        {
            var serviceResponse = await _customerService.GetAll();
            return BuildResponse(serviceResponse);
        }
    }
}
