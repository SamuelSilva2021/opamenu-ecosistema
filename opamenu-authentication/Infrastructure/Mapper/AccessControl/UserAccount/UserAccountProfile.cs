using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts.Enum;
using Authenticator.API.Core.Domain.AccessControl.UserAccounts.DTOs;
using OpaMenu.Infrastructure.Shared.Entities.AccessControl.UserAccounts;
using AutoMapper;

namespace Authenticator.API.Infrastructure.Mapper.AccessControl.UserAccount
{
    public class UserAccountProfile : Profile
    {
        public UserAccountProfile()
        {
            // Entity para DTO
            CreateMap<UserAccountEntity, UserAccountDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));

            CreateMap<UserAccountEntity, UserAccountWithGroupsDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
                .ForMember(dest => dest.AccessGroups, opt => opt.MapFrom(src => src.AccountAccessGroups.Select(aag => aag.AccessGroup)));

            CreateMap<UserAccountEntity, UserAccountListDTO>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));

            CreateMap<UserAccountDTO, UserAccountEntity>()
               .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
               .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
               .ForMember(dest => dest.PasswordResetExpiresAt, opt => opt.Ignore())
               .ForMember(dest => dest.AccountAccessGroups, opt => opt.Ignore());

            // CreateDTO para Entity
            CreateMap<UserAccountCreateDTO, UserAccountEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EUserAccountStatus.Ativo))
                .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.AccountAccessGroups, opt => opt.Ignore());

            // UpdateDTO para Entity
            CreateMap<UserAccountUpdateDto, UserAccountEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Username, opt => opt.Condition(src => src.Username != null))
                .ForMember(dest => dest.Email, opt => opt.Condition(src => src.Email != null))
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => src.FirstName != null))
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => src.LastName != null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.Condition(src => src.PhoneNumber != null))
                .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status.HasValue))
                .ForMember(dest => dest.IsEmailVerified, opt => opt.Condition(src => src.IsEmailVerified.HasValue))
                .ForMember(dest => dest.TenantId, opt => opt.Condition(src => src.TenantId.HasValue))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.CreatedAt, DateTimeKind.Utc)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetToken, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordResetExpiresAt, opt => opt.Ignore())
                .ForMember(dest => dest.AccountAccessGroups, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // CreateDTO para ResponseDTO (para retorno após criação)
            CreateMap<UserAccountCreateDTO, UserAccountDTO>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => EUserAccountStatus.Ativo))
                .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.LastLoginAt, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()));


        }
    }
}


