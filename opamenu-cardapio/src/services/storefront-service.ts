import { httpClient, withApiErrorHandling } from './http-client';
import { 
  StorefrontData, 
  ApiResponse, 
  MenuResponseDto, 
  TenantBusinessResponseDto, 
  TenantBusinessInfo, 
  CategoryResponseDto, 
  Category, 
  ProductDto, 
  ProductWithAddons, 
  AddonGroup, 
  ProductAddonGroupResponseDto,
  CouponDto,
  Coupon
} from '@/types/api';

const safeJsonParse = <T>(value: any, fallback: T): T => {
  if (typeof value !== 'string') return value as T;
  try {
    return JSON.parse(value);
  } catch {
    return fallback;
  }
};

const mapTenantBusiness = (dto: TenantBusinessResponseDto): TenantBusinessInfo => {
  return {
    id: dto.id,
    name: dto.name,
    slug: dto.slug,
    description: dto.description || '',
    logoUrl: dto.logoUrl,
    bannerUrl: dto.bannerUrl,
    instagramUrl: dto.instagramUrl,
    facebookUrl: dto.facebookUrl,
    whatsappNumber: dto.whatsappNumber || '',
    phone: dto.phone || '',
    email: dto.email,
    addressStreet: dto.addressStreet || '',
    addressNumber: dto.addressNumber || '',
    addressNeighborhood: dto.addressNeighborhood || '',
    addressCity: dto.addressCity || '',
    addressState: dto.addressState || '',
    addressZipcode: dto.addressZipcode,
    openingHours: safeJsonParse(dto.openingHours, null),
    paymentMethods: safeJsonParse(dto.paymentMethods, []),
    isOpen: false, // Calculated on frontend or needs another field?
    latitude: dto.latitude,
    longitude: dto.longitude,
    loyaltyProgram: dto.loyaltyProgram
  };
};

const mapCategory = (dto: CategoryResponseDto): Category => {
  return {
    id: dto.id,
    name: dto.name,
    description: dto.description,
    isActive: dto.isActive
  };
};

const mapProduct = (dto: ProductDto): ProductWithAddons => {
  return {
    id: dto.id,
    name: dto.name,
    description: dto.description || '',
    price: dto.price,
    categoryId: dto.categoryId,
    imageUrl: dto.imageUrl,
    isActive: dto.isActive,
    displayOrder: dto.displayOrder,
    createdAt: dto.createdAt,
    updatedAt: dto.updatedAt,
    categoryName: dto.categoryName,
    addonGroups: (dto.addonGroups || []).map(pag => mapProductAddonGroup(pag))
  };
};

const mapProductAddonGroup = (pag: ProductAddonGroupResponseDto): AddonGroup => {
  const group = pag.addonGroup;
  return {
    ...group,
    // Use overrides from the relationship if they exist, otherwise fallback to group defaults
    displayOrder: pag.displayOrder,
    isRequired: pag.isRequired,
    minSelections: pag.minSelectionsOverride ?? group.minSelections,
    maxSelections: pag.maxSelectionsOverride ?? group.maxSelections,
    addons: (group.addons || []).map(addon => ({
      ...addon
    }))
  };
};

const deduplicateCategories = (categories: CategoryResponseDto[]): CategoryResponseDto[] => {
  const seen = new Set<number>();
  return categories.filter(category => {
    if (seen.has(category.id)) {
      return false;
    }
    seen.add(category.id);
    return true;
  });
};

export const getStorefrontData = async (slug: string): Promise<StorefrontData | null> => {
  return withApiErrorHandling(async () => {
    const response = await httpClient.get<MenuResponseDto>(`/public/${slug}/storefront`);       
    
    const tenantBusiness = response.tenantBusiness ? mapTenantBusiness(response.tenantBusiness) : null;
    if (!tenantBusiness) return null;
    const uniqueCategories = deduplicateCategories(response.categories || []);
    return {
         tenantBusiness,
         categories: uniqueCategories.map(mapCategory),
         products: (response.products || []).map(mapProduct),
         coupons: response.coupons as Coupon[] || []
       };
  });
};
