using AutoMapper;
using OpaMenu.Domain.DTOs.Customer;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Infrastructure.Mapper;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<CustomerEntity, CustomerResponseDto>();
        
        CreateMap<CreateCustomerRequestDto, CustomerEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TenantCustomers, opt => opt.Ignore());

        CreateMap<UpdateCustomerRequestDto, CustomerEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.TenantCustomers, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
    }
}

