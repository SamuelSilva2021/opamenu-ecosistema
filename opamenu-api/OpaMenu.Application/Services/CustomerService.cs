using AutoMapper;
using OpaMenu.Application.Services.Interfaces;
using OpaMenu.Domain.DTOs.Customer;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;


namespace OpaMenu.Application.Services
{
    public class CustomerService(
        ICustomerRepository customerRepository,
        ITenantCustomerRepository tenantCustomerRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ITenantRepository tenantRepository
        ) : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly ITenantCustomerRepository _tenantCustomerRepository = tenantCustomerRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ITenantRepository _tenantRepository = tenantRepository;

        public async Task<ResponseDTO<CustomerResponseDto?>> CreatePublicCustomerAsync(CreateCustomerRequestDto request, string slug)
        {
            try
            {
                var tenant = await _tenantRepository.GetBySlugAsync(slug);
                if (tenant == null)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);

                var existingTenantCustomer = await _tenantCustomerRepository.GetByTenantIdAndCustomerPhoneAsync(tenant.Id, request.Phone);
                if (existingTenantCustomer != null)
                {
                    var existingCustomerDto = _mapper.Map<CustomerResponseDto>(existingTenantCustomer.Customer);
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildOk(existingCustomerDto);
                }
                var customerEntity = _mapper.Map<CustomerEntity>(request);
                var createdCustomer = await _customerRepository.CreateAsync(customerEntity);
                var tenantCustomerEntity = new TenantCustomerEntity
                {
                    TenantId = tenant.Id,
                    CustomerId = createdCustomer!.Id
                };
                await _tenantCustomerRepository.CreateAsync(tenantCustomerEntity);
                var customerDto = _mapper.Map<CustomerResponseDto>(createdCustomer);
                return StaticResponseBuilder<CustomerResponseDto?>.BuildCreated( customerDto );

            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<CustomerResponseDto?>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<IEnumerable<CustomerResponseDto>>> GetAll()
        {
            try
            {
                var tenantCustomers = await _tenantCustomerRepository.GetByTenantIdAsync(_currentUserService.GetTenantGuid()!.Value);
                var customersEntity = tenantCustomers.Select(tc => tc.Customer).ToList();
                var customersDto = _mapper.Map<IEnumerable<CustomerResponseDto>>(customersEntity);
                return StaticResponseBuilder<IEnumerable<CustomerResponseDto>>.BuildOk( customersDto );
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<IEnumerable<CustomerResponseDto>>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<CustomerResponseDto?>> GetPublicCustomer(string slug, string phoneNumber)
        {
            try
            {
                var tenant = await _tenantRepository.GetBySlugAsync(slug);
                if (tenant == null)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);

                var tenantCustomer = await _tenantCustomerRepository.GetByTenantIdAndCustomerPhoneAsync(tenant.Id, phoneNumber);
                if (tenantCustomer == null)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);

                var customerDto = _mapper.Map<CustomerResponseDto>(tenantCustomer.Customer);
                return StaticResponseBuilder<CustomerResponseDto?>.BuildOk( customerDto );
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<CustomerResponseDto?>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<CustomerResponseDto?>> UpdatePublicCustomerAsync(UpdateCustomerRequestDto request, string slug)
        {
            try
            {
                var tenant = await _tenantRepository.GetBySlugAsync(slug);
                if (tenant == null)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);

                var customer = await _customerRepository.GetByIdAsync(request.Id);
                
                if (customer == null)
                     return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);
                
                customer.Name = request.Name ?? customer.Name;

                if (!string.IsNullOrEmpty(request.Phone)) customer.Phone = request.Phone;
                
                customer.Email = request.Email ?? customer.Email;
                customer.PostalCode = request.PostalCode ?? customer.PostalCode;
                customer.Street = request.Street ?? customer.Street;
                customer.StreetNumber = request.StreetNumber ?? customer.StreetNumber;
                customer.Neighborhood = request.Neighborhood ?? customer.Neighborhood;
                customer.City = request.City ?? customer.City;
                customer.State = request.State ?? customer.State;
                customer.Complement = request.Complement ?? customer.Complement;
                customer.UpdatedAt = DateTime.UtcNow;

                await _customerRepository.UpdateAsync(customer);

                var customerDto = _mapper.Map<CustomerResponseDto>(customer);
                return StaticResponseBuilder<CustomerResponseDto?>.BuildOk(customerDto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<CustomerResponseDto?>.BuildErrorResponse(ex);
            }
        }
    }
}

