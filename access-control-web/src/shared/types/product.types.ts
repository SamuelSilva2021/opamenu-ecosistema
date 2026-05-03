export interface TenantProduct {
  id: string;
  name: string;
  slug: string;
  description?: string | null;
  category: string;
  version: string;
  status: string;
  configurationSchema?: string | null;
  pricingModel: string;
  basePrice: number;
  setupFee: number;
  createdAt: string;
  updatedAt?: string | null;
  totalSubscriptions: number;
  activeSubscriptions: number;
}

export interface CreateTenantProductRequest {
  name: string;
  slug: string;
  description?: string;
  category: string;
  version?: string;
  status?: string;
  configurationSchema?: string;
  pricingModel?: string;
  basePrice: number;
  setupFee?: number;
}

export interface UpdateTenantProductRequest extends Partial<CreateTenantProductRequest> {}
