using AutoMapper;
using OpaMenu.Domain.DTOs.Coupon;
using OpaMenu.Infrastructure.Shared.Entities;
using OpaMenu.Infrastructure.Shared.Entities.Opamenu;

namespace OpaMenu.Infrastructure.Mapper;

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

