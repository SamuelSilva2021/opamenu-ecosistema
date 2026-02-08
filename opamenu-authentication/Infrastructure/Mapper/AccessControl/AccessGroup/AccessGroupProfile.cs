using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.AccessGroup
{
    public class AccessGroupProfile : Profile
    {
        public AccessGroupProfile()
        {
            // Entity -> DTO
            CreateMap<AccessGroupEntity, AccessGroupDTO>()
                .ForMember(dest => dest.GroupTypeName,
                           opt => opt.MapFrom(src => src.GroupType != null ? src.GroupType.Name : null));

            // DTO -> Entity
            CreateMap<CreateAccessGroupDTO, AccessGroupEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateAccessGroupDTO, AccessGroupEntity>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}

