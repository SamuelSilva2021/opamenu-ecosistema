using AutoMapper;
using Opamenu.Domain.DTOs.TenantCustomer;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Mapper
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

