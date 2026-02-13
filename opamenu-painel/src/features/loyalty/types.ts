export const ELoyaltyProgramType = {
  PointsPerValue: 0,
  OrderCount: 1,
  ItemCount: 2,
} as const;
export type ELoyaltyProgramType = (typeof ELoyaltyProgramType)[keyof typeof ELoyaltyProgramType];

export const ELoyaltyRewardType = {
  PercentageDiscount: 0,
  FixedValueDiscount: 1,
  FreeProduct: 2,
} as const;
export type ELoyaltyRewardType = (typeof ELoyaltyRewardType)[keyof typeof ELoyaltyRewardType];

export interface LoyaltyProgramFilter {
  productId?: string;
  categoryId?: string;
}

export interface LoyaltyProgram {
  id: string;
  name: string;
  description?: string;
  pointsPerCurrency: number;
  currencyValue: number;
  minOrderValue: number;
  pointsValidityDays?: number;
  isActive: boolean;
  type: ELoyaltyProgramType;
  targetCount?: number;
  rewardType?: ELoyaltyRewardType;
  rewardValue?: number;
  filters: LoyaltyProgramFilter[];
}

export interface CreateLoyaltyProgramRequest {
  name: string;
  description?: string;
  pointsPerCurrency: number;
  currencyValue: number;
  minOrderValue: number;
  pointsValidityDays?: number;
  isActive: boolean;
  type: ELoyaltyProgramType;
  targetCount?: number;
  rewardType?: ELoyaltyRewardType;
  rewardValue?: number;
  filters: LoyaltyProgramFilter[];
}

export interface UpdateLoyaltyProgramRequest extends CreateLoyaltyProgramRequest {
  id: string;
}
