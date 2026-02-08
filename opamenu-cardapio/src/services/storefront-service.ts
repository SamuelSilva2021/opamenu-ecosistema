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
  const paymentMethodsData = safeJsonParse(dto.paymentMethods, []);
  let paymentMethods: string[] = [];
  let pixKey: string | undefined = undefined;

  if (Array.isArray(paymentMethodsData)) {
    paymentMethods = paymentMethodsData;
  } else if (typeof paymentMethodsData === 'object' && paymentMethodsData !== null) {
    // @ts-ignore
    paymentMethods = Array.isArray(paymentMethodsData.methods) ? paymentMethodsData.methods : [];
    // @ts-ignore
    pixKey = paymentMethodsData.pixKey;
  }

  // Prioritize top-level pixKey from DTO if available (from BankDetails)
  if (dto.pixKey) {
    pixKey = dto.pixKey;
  }

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
    paymentMethods: paymentMethods,
    pixKey: pixKey,
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
    addonGroups: (dto.aditionalGroups || [])
      .map(pag => mapProductAddonGroup(pag))
      .filter((group): group is AddonGroup => group !== null)
  };
};

const mapProductAddonGroup = (pag: ProductAddonGroupResponseDto): AddonGroup | null => {
  const group = pag.aditionalGroup;
  if (!group) {
    console.warn('⚠️ ProductAddonGroup missing aditionalGroup data:', pag);
    return null;
  }
  return {
    ...group,
    id: group.id,
    name: group.name,
    description: group.description,
    type: group.type,
    minSelections: pag.minSelectionsOverride ?? group.minSelections,
    maxSelections: pag.maxSelectionsOverride ?? group.maxSelections,
    isRequired: pag.isRequired, // Use override from relation
    displayOrder: pag.displayOrder, // Use override from relation
    isActive: group.isActive,
    addons: (group.aditionals || []).map(addon => ({
      ...addon,
      price: addon.price
    }))
  };
};

const deduplicateCategories = (categories: CategoryResponseDto[]): CategoryResponseDto[] => {
  const seen = new Set<string>();
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
