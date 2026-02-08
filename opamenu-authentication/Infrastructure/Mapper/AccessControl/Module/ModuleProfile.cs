using Authenticator.API.Core.Domain.AccessControl.Modules.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.Module
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            // CreateDTO para Entity
            CreateMap<ModuleCreateDTO, ModuleEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Application, opt => opt.Ignore());
               

            // UpdateDTO para Entity
            CreateMap<ModuleUpdateDTO, ModuleEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.Application, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Entity para ResponseDTO
            CreateMap<ModuleEntity, ModuleDTO>()
                .ForMember(dest => dest.ApplicationName,
                           opt => opt.MapFrom(src => src.Application != null ? src.Application.Name : null));

            // Entity para ListDTO
            CreateMap<ModuleEntity, ModuleListDTO>()
                .ForMember(dest => dest.ApplicationName,
                           opt => opt.MapFrom(src => src.Application != null ? src.Application.Name : null));

            // ResponseDTO para Entity (para casos específicos)
            CreateMap<ModuleDTO, ModuleEntity>()
                .ForMember(dest => dest.Application, opt => opt.Ignore());
        }
    }
}

