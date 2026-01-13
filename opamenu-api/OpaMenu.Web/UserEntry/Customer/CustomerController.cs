using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs;
using OpaMenu.Domain.DTOs.Customer;
using OpaMenu.Web.UserEntry;

namespace OpaMenu.Web.UserEntry.Customer
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController(
        ILogger<CustomersController> logger,
        ICustomerService customerService
        ) : BaseController
    {
        private readonly ILogger<CustomersController> _logger = logger;
        private readonly ICustomerService _customerService = customerService;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerResponseDto>>>> GetAllCustomers()
        {
            var serviceResponse = await _customerService.GetAll();
            return BuildResponse(serviceResponse);
        }
    }
}
