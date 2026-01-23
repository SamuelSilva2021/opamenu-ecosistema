export interface LoyaltyProgram {
  id: string;
  name: string;
  description?: string;
  pointsPerCurrency: number;
  currencyValue: number;
  minOrderValue: number;
  pointsValidityDays?: number;
  isActive: boolean;
}

export interface CreateLoyaltyProgramRequest {
  name: string;
  description?: string;
  pointsPerCurrency: number;
  currencyValue: number;
  minOrderValue: number;
  pointsValidityDays?: number;
  isActive: boolean;
}

export interface UpdateLoyaltyProgramRequest extends CreateLoyaltyProgramRequest {
  id: string;
}
