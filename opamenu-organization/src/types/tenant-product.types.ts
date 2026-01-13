export const TenantProductStatus = {
  Inativo: 0,
  Ativo: 1,
  Descontinuado: 2,
} as const;

export type TenantProductStatus = (typeof TenantProductStatus)[keyof typeof TenantProductStatus];

export interface TenantProduct {
  id: string;
  name: string;
  slug: string;
  description?: string;
  category: string;
  version: string;
  status: TenantProductStatus;
  configurationSchema?: string;
  pricingModel: string;
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
  category: string;
  version?: string;
  status?: TenantProductStatus;
  configurationSchema?: string;
  pricingModel?: string;
  basePrice: number;
  setupFee?: number;
}

export interface UpdateTenantProductDTO {
  id: string;
  name?: string;
  slug?: string;
  description?: string;
  category?: string;
  version?: string;
  status?: TenantProductStatus;
  configurationSchema?: string;
  pricingModel?: string;
  basePrice?: number;
  setupFee?: number;
}

export interface ApiResponse<T> {
  succeeded: boolean;
  data: T;
  errors?: { message: string }[];
}
