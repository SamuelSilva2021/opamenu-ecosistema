using AutoMapper;
using OpaMenu.Domain.DTOs.Customer;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Domain.Interfaces;
using OpaMenu.Commons.Api.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Application.Services.Interfaces.Opamenu;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;


namespace OpaMenu.Application.Services.Opamenu
{
    public class CustomerService(
        ICustomerRepository customerRepository,
        ITenantCustomerRepository tenantCustomerRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ITenantRepository tenantRepository,
        ICustomerLoyaltyRepository customerLoyaltyRepository
        ) : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository = customerRepository;
        private readonly ICurrentUserService _currentUserService = currentUserService;
        private readonly ITenantCustomerRepository _tenantCustomerRepository = tenantCustomerRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly ICustomerLoyaltyRepository _customerLoyaltyRepository = customerLoyaltyRepository;

        public async Task<ResponseDTO<CustomerResponseDto?>> CreatePublicCustomerAsync(CreateCustomerRequestDto request, string slug)
        {
            try
            {
                var tenant = await _tenantRepository.GetBySlugAsync(slug);
                if (tenant == null)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);

                request.Phone = System.Text.RegularExpressions.Regex.Replace(request.Phone, @"\D", "");

                var existingTenantCustomer = await _tenantCustomerRepository.GetByTenantIdAndCustomerPhoneAsync(tenant.Id, request.Phone);
                if (existingTenantCustomer != null)
                {
                    var existingCustomerDto = _mapper.Map<CustomerResponseDto>(existingTenantCustomer.Customer);
                    var loyaltyBalance = await _customerLoyaltyRepository.GetByCustomerAndTenantAsync(existingTenantCustomer.Customer.Id, tenant.Id);
                    existingCustomerDto.LoyaltyBalance = loyaltyBalance?.Balance ?? 0;
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
                customerDto.LoyaltyBalance = 0; // Novo cliente
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

        public async Task<PagedResponseDTO<CustomerResponseDto>> GetPagedAsync(string? search, int page, int limit)
        {
            try
            {
                var tenantId = _currentUserService.GetTenantGuid();
                if (!tenantId.HasValue)
                    return new PagedResponseDTO<CustomerResponseDto> { Succeeded = false, Errors = new List<ErrorDTO> { new ErrorDTO { Message = "Estabelecimento não identificado." } } };

                var (items, totalCount) = await _tenantCustomerRepository.GetPagedByTenantIdAsync(tenantId.Value, search, page, limit);
                var customersEntity = items.Select(tc => tc.Customer).ToList();
                var customersDto = _mapper.Map<IEnumerable<CustomerResponseDto>>(customersEntity);
                
                return StaticResponseBuilder<CustomerResponseDto>.BuildPagedOk(customersDto, totalCount, page, limit);
            }
            catch (Exception ex)
            {
                var response = StaticResponseBuilder<CustomerResponseDto>.BuildErrorResponse(ex);
                return new PagedResponseDTO<CustomerResponseDto>
                {
                    Succeeded = false,
                    Errors = response.Errors,
                    Code = response.Code
                };
            }
        }

        public async Task<ResponseDTO<CustomerResponseDto?>> GetByPhoneAsync(string phone)
        {
            try
            {
                var tenantId = _currentUserService.GetTenantGuid();
                if (!tenantId.HasValue)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildError("Estabelecimento não identificado.");

                phone = System.Text.RegularExpressions.Regex.Replace(phone, @"\D", "");

                var tenantCustomer = await _tenantCustomerRepository.GetByTenantIdAndCustomerPhoneAsync(tenantId.Value, phone);
                if (tenantCustomer == null)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);

                var customerDto = _mapper.Map<CustomerResponseDto>(tenantCustomer.Customer);
                return StaticResponseBuilder<CustomerResponseDto?>.BuildOk(customerDto);
            }
            catch (Exception ex)
            {
                return StaticResponseBuilder<CustomerResponseDto?>.BuildErrorResponse(ex);
            }
        }

        public async Task<ResponseDTO<CustomerResponseDto?>> GetPublicCustomer(string slug, string phoneNumber)
        {
            try
            {
                var tenant = await _tenantRepository.GetBySlugAsync(slug);
                if (tenant == null)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);

                // Limpar telefone
                phoneNumber = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"\D", "");

                var tenantCustomer = await _tenantCustomerRepository.GetByTenantIdAndCustomerPhoneAsync(tenant.Id, phoneNumber);
                if (tenantCustomer == null)
                    return StaticResponseBuilder<CustomerResponseDto?>.BuildNotFound(null);

                var customerDto = _mapper.Map<CustomerResponseDto>(tenantCustomer.Customer);
                
                // Buscar saldo de fidelidade
                var loyaltyBalance = await _customerLoyaltyRepository.GetByCustomerAndTenantAsync(tenantCustomer.Customer.Id, tenant.Id);
                customerDto.LoyaltyBalance = loyaltyBalance?.Balance ?? 0;

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

                if (!string.IsNullOrEmpty(request.Phone))
                {
                    customer.Phone = System.Text.RegularExpressions.Regex.Replace(request.Phone, @"\D", "");
                }
                
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

