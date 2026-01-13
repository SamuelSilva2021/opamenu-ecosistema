export const DiscountType = {
  Percentage: 1,
  FixedAmount: 2
} as const;

export type DiscountType = typeof DiscountType[keyof typeof DiscountType];

export interface Coupon {
  id: number;
  code: string;
  description?: string;
  discountType: DiscountType;
  discountValue: number;
  minOrderValue?: number;
  maxDiscountValue?: number;
  usageLimit?: number;
  usageCount: number;
  startDate?: string;
  expirationDate?: string;
  isActive: boolean;
  firstOrderOnly: boolean;
  createdAt: string;
}

export interface CreateCouponRequest {
  code: string;
  description?: string;
  discountType: DiscountType;
  discountValue: number;
  minOrderValue?: number;
  maxDiscountValue?: number;
  usageLimit?: number;
  startDate?: string;
  expirationDate?: string;
  firstOrderOnly: boolean;
}

export interface UpdateCouponRequest {
  code: string;
  description?: string;
  discountType: DiscountType;
  discountValue: number;
  minOrderValue?: number;
  maxDiscountValue?: number;
  usageLimit?: number;
  startDate?: string;
  expirationDate?: string;
  firstOrderOnly: boolean;
  isActive: boolean;
}
