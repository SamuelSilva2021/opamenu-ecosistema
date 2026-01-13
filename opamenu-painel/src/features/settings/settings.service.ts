import { api } from "@/lib/axios";
import type { TenantBusinessResponseDto, UpdateTenantBusinessRequestDto } from "./types";

export const settingsService = {
  getSettings: async () => {
    const response = await api.get<TenantBusinessResponseDto>("/settings");
    return response.data;
  },
  updateSettings: async (data: UpdateTenantBusinessRequestDto) => {
    const response = await api.put<TenantBusinessResponseDto>("/settings", data);
    return response.data;
  },
};
