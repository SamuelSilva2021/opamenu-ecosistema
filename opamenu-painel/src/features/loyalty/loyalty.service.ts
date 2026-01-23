import { api } from "@/lib/axios";
import type { CreateLoyaltyProgramRequest, LoyaltyProgram, UpdateLoyaltyProgramRequest } from "./types";

export const loyaltyService = {
  getProgram: async (): Promise<LoyaltyProgram | null> => {
    try {
      const response = await api.get<LoyaltyProgram>("/loyalty/program");
      return response.data;
    } catch (error: any) {
      if (error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  },

  createProgram: async (data: CreateLoyaltyProgramRequest): Promise<LoyaltyProgram> => {
    const response = await api.post<LoyaltyProgram>("/loyalty/program", data);
    return response.data;
  },

  updateProgram: async (data: UpdateLoyaltyProgramRequest): Promise<LoyaltyProgram> => {
    const response = await api.put<LoyaltyProgram>(`/loyalty/program`, data);
    return response.data;
  },

  toggleStatus: async (id: string, status: boolean): Promise<void> => {
    await api.patch(`/loyalty/program/${id}/toggle-status?status=${status}`);
  }
};
