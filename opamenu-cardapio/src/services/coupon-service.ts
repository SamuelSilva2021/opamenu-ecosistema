import { httpClient } from './http-client';
import { Coupon, ApiResponse } from '@/types/api';

export const couponService = {
  getActiveCoupons: async (slug: string): Promise<Coupon[]> => {
    try {
      const response = await httpClient.get<ApiResponse<Coupon[]>>(`/public/${slug}/coupons`);
      if (response && response.data) {
        return response.data;
      }
      return [];
    } catch (error) {
      console.error('Error fetching coupons:', error);
      return [];
    }
  },

  validateCoupon: async (slug: string, code: string, orderValue: number): Promise<Coupon | null> => {
    try {
      const response = await httpClient.post<ApiResponse<Coupon>>(`/public/${slug}/coupons/validate`, { code, orderValue });
      if (response && response.data) {
        return response.data;
      }
      return null;
    } catch (error) {
      // console.error('Error validating coupon:', error);
      throw error;
    }
  }
};
