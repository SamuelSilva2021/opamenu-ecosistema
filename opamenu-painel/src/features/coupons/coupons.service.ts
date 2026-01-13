import { api } from "@/lib/axios";
import type { Coupon, CreateCouponRequest, UpdateCouponRequest } from "./types";

export const couponsService = {
  getCoupons: async (): Promise<Coupon[]> => {
    const response = await api.get<Coupon[]>("/coupon");
    return response.data || [];
  },

  getCouponById: async (id: number): Promise<Coupon> => {
    const response = await api.get<Coupon>(`/coupon/${id}`);
    return response.data || {};
  },

  createCoupon: async (data: CreateCouponRequest): Promise<Coupon> => {
    const response = await api.post<Coupon>("/coupon", data);
    return response.data || {};
  },

  updateCoupon: async (id: number, data: UpdateCouponRequest): Promise<Coupon> => {
    const response = await api.put<Coupon>(`/coupon/${id}`, data);
    return response.data || {};
  },

  deleteCoupon: async (id: number): Promise<boolean> => { 
    const response = await api.delete<boolean>(`/coupon/${id}`);
    return response.data || false;
  }
};
