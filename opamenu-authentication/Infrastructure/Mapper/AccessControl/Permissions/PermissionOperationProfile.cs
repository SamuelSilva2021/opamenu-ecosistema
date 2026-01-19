using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.PermissionOperations.DTOs;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.Permissions
{
    public class PermissionOperationProfile: Profile
    {
        public PermissionOperationProfile()
        {
            // PermissionOperationEntity -> PermissionOperationDTO
            CreateMap<PermissionOperationEntity, PermissionOperationDTO>();

            // CreateDTO -> Entity
            CreateMap<PermissionOperationCreateDTO, PermissionOperationEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Permission, opt => opt.Ignore())
                .ForMember(dest => dest.Operation, opt => opt.Ignore());

            // UpdateDTO -> Entity
            CreateMap<PermissionOperationUpdateDTO, PermissionOperationEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PermissionId, opt => opt.Ignore())
                .ForMember(dest => dest.OperationId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Permission, opt => opt.Ignore())
                .ForMember(dest => dest.Operation, opt => opt.Ignore());

            // Entity -> ResponseDTO
            CreateMap<PermissionOperationEntity, PermissionOperationDTO>()
                .ForMember(dest => dest.PermissionName, opt => opt.MapFrom(src =>
                    src.Permission != null ? src.Permission.Name : string.Empty))
                .ForMember(dest => dest.OperationName, opt => opt.MapFrom(src =>
                    src.Operation != null ? src.Operation.Name : string.Empty))
                .ForMember(dest => dest.OperationCode, opt => opt.MapFrom(src =>
                    src.Operation != null ? src.Operation.Value : string.Empty))
                .ForMember(dest => dest.OperationDescription, opt => opt.MapFrom(src =>
                    src.Operation != null ? src.Operation.Description : string.Empty));
        }
    }
}

