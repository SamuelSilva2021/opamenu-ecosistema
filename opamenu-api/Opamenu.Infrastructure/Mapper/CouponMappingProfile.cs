using AutoMapper;
using Opamenu.Domain.DTOs.Coupon;
using Opamenu.Infrastructure.Shared.Entities;
using Opamenu.Infrastructure.Shared.Entities.Opamenu;

namespace Opamenu.Infrastructure.Mapper;

public class CouponMappingProfile : Profile
{
    public CouponMappingProfile()
    {
        CreateMap<CouponEntity, CouponDto>();
        CreateMap<CreateCouponRequestDto, CouponEntity>();
        CreateMap<UpdateCouponRequestDto, CouponEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

