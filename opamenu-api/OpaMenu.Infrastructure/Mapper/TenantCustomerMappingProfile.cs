using AutoMapper;
using OpaMenu.Domain.DTOs.TenantCustomer;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Infrastructure.Mapper
{
    public class TenantCustomerMappingProfile : Profile
    {
        public TenantCustomerMappingProfile()
        {
            CreateMap<TenantCustomerEntity, TenantCustomerResponseDto>();
            
            CreateMap<UpdateTenantCustomerRequestDto, TenantCustomerEntity>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

