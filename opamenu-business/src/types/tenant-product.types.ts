export const ETenantProductPricingModel = {
  Assinatura: 'Assinatura',
} as const;

export type ETenantProductPricingModel =
  (typeof ETenantProductPricingModel)[keyof typeof ETenantProductPricingModel];

export const EProductStatus = {
  Inativo: 'Inativo',
  Ativo: 'Ativo',
  Descontinuado: 'Descontinuado',
} as const;

export type EProductStatus = (typeof EProductStatus)[keyof typeof EProductStatus];

export const ETenantProductCategory = {
  WebApp: 'WebApp',
  MobileApp: 'MobileApp',
  APIService: 'APIService',
  DesktopApp: 'DesktopApp',
  Plugin: 'Plugin',
  Other: 'Other',
} as const;

export type ETenantProductCategory =
  (typeof ETenantProductCategory)[keyof typeof ETenantProductCategory];

export interface TenantProduct {
  id: string;
  name: string;
  slug: string;
  description?: string;
  category: ETenantProductCategory;
  version: string;
  status: EProductStatus;
  configurationSchema?: string;
  pricingModel: ETenantProductPricingModel;
  basePrice: number;
  setupFee: number;
  createdAt: string;
  updatedAt?: string;
  totalSubscriptions: number;
  activeSubscriptions: number;
}

export interface CreateTenantProductDTO {
  name: string;
  slug: string;
  description?: string;
  category: ETenantProductCategory;
  version?: string;
  status?: EProductStatus;
  configurationSchema?: string;
  pricingModel?: ETenantProductPricingModel;
  basePrice: number;
  setupFee?: number;
}

export interface UpdateTenantProductDTO {
  id: string;
  name?: string;
  slug?: string;
  description?: string;
  category?: ETenantProductCategory;
  version?: string;
  status?: EProductStatus;
  configurationSchema?: string;
  pricingModel?: ETenantProductPricingModel;
  basePrice?: number;
  setupFee?: number;
}

export interface ApiResponse<T> {
  succeeded: boolean;
  data: T;
  errors?: { message: string }[];
}
