import { api } from "@/lib/axios";
import type { TenantBusinessResponseDto, UpdateTenantBusinessRequestDto, TenantPaymentConfigDto } from "./types";

export const settingsService = {
  getSettings: async () => {
    const response = await api.get<TenantBusinessResponseDto>("/settings");
    return response.data;
  },
  updateSettings: async (data: UpdateTenantBusinessRequestDto) => {
    const response = await api.put<TenantBusinessResponseDto>("/settings", data);
    return response.data;
  },
  getPixConfig: async () => {
    const response = await api.get<TenantPaymentConfigDto>("/tenant-payment-config/pix");
    return response.data;
  },
  upsertPixConfig: async (data: TenantPaymentConfigDto) => {
    const response = await api.post<TenantPaymentConfigDto>("/tenant-payment-config", data);
    return response.data;
  }
};
