using OpaMenu.Infrastructure.Shared.Entities.AccessControl;
using Authenticator.API.Core.Domain.AccessControl.Permissions.DTOs;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.Permissions
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            // PermissionOperationEntity -> PermissionOperationDTO
            CreateMap<PermissionOperationEntity, PermissionOperationDTO>();

            // CreateDTO -> Entity
            CreateMap<PermissionCreateDTO, PermissionEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.Name, opt => opt.Ignore()) 
                .ForMember(dest => dest.Description, opt => opt.Ignore()) 
                .ForMember(dest => dest.Code, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Module, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
                .ForMember(dest => dest.PermissionOperations, opt => opt.Ignore());

            // UpdateDTO -> Entity
            CreateMap<PermissionUpdateDTO, PermissionEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.Name, opt => opt.Ignore()) 
                .ForMember(dest => dest.Description, opt => opt.Ignore()) 
                .ForMember(dest => dest.Code, opt => opt.Ignore()) 
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Module, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
                .ForMember(dest => dest.PermissionOperations, opt => opt.Ignore());

            CreateMap<PermissionEntity, PermissionDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                src.Module != null ? src.Module.Name : $"Permission_{src.Id}"))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                src.Module != null ? src.Module.Description : string.Empty))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src =>
                src.Module != null ? src.Module.Key : null))
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src =>
                src.Module != null ? src.Module.Name : null))
            .ForMember(dest => dest.Operations, opt => opt.MapFrom(src =>
                src.PermissionOperations != null && src.PermissionOperations.Any(po => po.IsActive)
                    ? src.PermissionOperations
                        .Where(po => po.IsActive && po.Operation != null)
                        .Select(po => new PermissionOperationDTO
                        {
                            Id = po.Operation.Id,
                            Name = po.Operation.Name,
                            Code = po.Operation.Value!,
                            Description = po.Operation.Description!
                        }).ToList()
                    : new List<PermissionOperationDTO>()));

        }
    }
}

