import type { TenantSummary } from './tenant.types';

export interface SubscriptionPlan {
  id: string;
  name: string;
  slug: string;
  description?: string | null;
  price: number;
  billingCycle: string;
  features?: string | null;
  status?: string;
  isTrial?: boolean;
  trialPeriodDays?: number;
  sortOrder?: number;
}

export interface PlanFilters {
  name?: string;
  isActive?: boolean;
}

export type SubscriptionStatus =
  | 'Ativo'
  | 'Pendente'
  | 'Trial'
  | 'Cancelado'
  | string;

export interface Subscription {
  id: string;
  tenantId: string;
  productId: string;
  planId: string;
  status: SubscriptionStatus;
  trialEndsAt?: string | null;
  currentPeriodStart: string;
  currentPeriodEnd: string;
  cancelAtPeriodEnd: boolean;
  cancelledAt?: string | null;
  customPricing?: number | null;
  usageLimits?: string | null;
  createdAt: string;
  updatedAt?: string | null;
  tenant?: TenantSummary | null;
  plan?: SubscriptionPlan | null;
}

export interface SubscriptionResponse {
  data: Subscription;
  succeeded: boolean;
  code: number;
  errors?: { message: string }[];
}

