using Authenticator.API.Core.Domain.AccessControl.AccessGroup.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.AccessGroups.DTOs;
using Authenticator.API.Core.Domain.Api;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.AccessGroup
{
    /// <summary>
    /// Perfil de mapeamento para GroupType
    /// </summary>
    public class GroupTypeProfile : Profile
    {
        /// <summary>
        /// Construtor que configura os mapeamentos
        /// </summary>
        public GroupTypeProfile()
        {
            // Entidade → DTO (resposta)
            CreateMap<GroupTypeEntity, GroupTypeDTO>();

            // CreateDTO → Entidade (para criação)
            CreateMap<GroupTypeCreateDTO, GroupTypeEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.AccessGroups, opt => opt.Ignore());

            // UpdateDTO → Entidade (para atualização)
            CreateMap<GroupTypeUpdateDTO, GroupTypeEntity>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.CreatedAt, DateTimeKind.Utc)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.AccessGroups, opt => opt.Ignore());
        }
    }
}

