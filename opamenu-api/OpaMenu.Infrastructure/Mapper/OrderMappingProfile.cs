using AutoMapper;
using OpaMenu.Domain.DTOs;
using OpaMenu.Infrastructure.Shared.Entities;

namespace OpaMenu.Infrastructure.Mapper;

/// <summary>
/// Profile de mapeamento do AutoMapper para entidades Order
/// </summary>
public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // Mapeamento de OrderEntity para OrderResponseDto
        CreateMap<OrderEntity, OrderResponseDto>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        // Mapeamento de CreateOrderRequestDto para OrderEntity
        CreateMap<CreateOrderRequestDto, OrderEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => FormatAddress(src.DeliveryAddress)))
            .ForMember(dest => dest.Status, opt => opt.Ignore()) // Definido no serviço
            .ForMember(dest => dest.Total, opt => opt.Ignore()) // Calculado no serviço
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        // Mapeamento de UpdateOrderRequestDto para OrderEntity
        CreateMap<UpdateOrderRequestDto, OrderEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Mapeamento de OrderItemEntity para OrderItemResponseDto
        CreateMap<OrderItemEntity, OrderItemResponseDto>()
            .ForMember(dest => dest.Addons, opt => opt.MapFrom(src => src.Addons))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));

        // Mapeamento de CreateOrderItemRequestDto para OrderItemEntity
        CreateMap<CreateOrderItemRequestDto, OrderItemEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.ProductName, opt => opt.Ignore()) // Obtido do produto
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore()) // Obtido do produto
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()) // Calculado
            .ForMember(dest => dest.Addons, opt => opt.MapFrom(src => src.Addons));

        // Mapeamento de OrderItemAddonEntity para OrderItemAddonResponseDto
        CreateMap<OrderItemAddonEntity, OrderItemAddonResponseDto>();

        // Mapeamento de CreateOrderItemAddonRequestDto para OrderItemAddonEntity
        CreateMap<CreateOrderItemAddonRequestDto, OrderItemAddonEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItemId, opt => opt.Ignore())
            .ForMember(dest => dest.OrderItem, opt => opt.Ignore())
            .ForMember(dest => dest.Addon, opt => opt.Ignore())
            .ForMember(dest => dest.AddonName, opt => opt.Ignore()) // Obtido do addon
            .ForMember(dest => dest.UnitPrice, opt => opt.Ignore()) // Obtido do addon
            .ForMember(dest => dest.Subtotal, opt => opt.Ignore()); // Calculado
    }

    private string FormatAddress(AddressDto? address)
    {
        if (address == null) return string.Empty;

        var parts = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(address.Street))
            parts.Add(address.Street);
            
        if (!string.IsNullOrWhiteSpace(address.Number))
            parts.Add(address.Number);
            
        if (!string.IsNullOrWhiteSpace(address.Complement))
            parts.Add(address.Complement);
            
        if (!string.IsNullOrWhiteSpace(address.Neighborhood))
            parts.Add(address.Neighborhood);
            
        if (!string.IsNullOrWhiteSpace(address.City))
            parts.Add(address.City);
            
        if (!string.IsNullOrWhiteSpace(address.State))
            parts.Add(address.State);
            
        if (!string.IsNullOrWhiteSpace(address.ZipCode))
            parts.Add(address.ZipCode);

        return string.Join(", ", parts);
    }
}
