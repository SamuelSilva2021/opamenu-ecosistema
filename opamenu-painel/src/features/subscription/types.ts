export interface SubscriptionProductDto {
  id: string;
  name: string;
  slug: string;
  category: string;
  status: string;
}

export interface SubscriptionPlanDto {
  id: string;
  name: string;
  slug: string;
  price: number;
  billingCycle: string;
}

export interface SubscriptionStatusResponseDto {
  id: string;
  tenantId: string;
  productId: string;
  planId: string;
  status: string;
  planName: string;
  daysRemaining: number;
  isTrial: boolean;
  trialEndsAt?: string;
  currentPeriodStart: string;
  currentPeriodEnd: string;
  cancelAtPeriodEnd: boolean;
  cancelledAt?: string;
  customPricing?: number;
  usageLimits?: string;
  createdAt: string;
  updatedAt?: string;
  tenant?: any;
  product?: SubscriptionProductDto;
  plan?: SubscriptionPlanDto;
}

export interface CancelSubscriptionRequestDto {
  reason?: string;
}

export interface ChangePlanRequestDto {
  newPlanId: string;
}
