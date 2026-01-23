using AutoMapper;
using OpaMenu.Domain.DTOs.BankDetails;
using OpaMenu.Infrastructure.Shared.Entities.MultiTenant.Tenant;

namespace OpaMenu.Application.Mappings
{
    /// <summary>
    /// Profile de mapeamento para entidade BankDetails
    /// </summary>
    public class BankDetailsMappingProfile : Profile
    {
        public BankDetailsMappingProfile()
        {
            // Entity -> Dto
            CreateMap<BankDetailsEntity, BankDetailsDto>()
                .ForMember(dest => dest.IsPixKeySelected, opt => opt.MapFrom(src => src.IsPixKeySelected));
            
            // Create -> Entity
            CreateMap<CreateBankDetailsRequestDto, BankDetailsEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore()) // Deve ser setado no serviço
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
                
            // Update -> Entity
            CreateMap<UpdateBankDetailsRequestDto, BankDetailsEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID vem do path ou já está na entidade
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
