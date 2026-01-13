using Authenticator.API.Core.Domain.AccessControl.Operations.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.Operation
{
    public class OperationProfile : Profile
    {
        /// <summary>
        /// Construtor da classe de mapeamento de Operation
        /// </summary>
        public OperationProfile()
        {
            // Entity para ResponseDTO
            CreateMap<OperationEntity, OperationDTO>();

            // CreateDTO para Entity
            CreateMap<OperationCreateDTO, OperationEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PermissionOperations, opt => opt.Ignore());

            // UpdateDTO para Entity
            CreateMap<OperationUpdateDTO, OperationEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.PermissionOperations, opt => opt.Ignore());

            // ResponseDTO para Entity (para casos de atualização)
            CreateMap<OperationDTO, OperationEntity>()
                .ForMember(dest => dest.PermissionOperations, opt => opt.Ignore());
        }
    }
}

