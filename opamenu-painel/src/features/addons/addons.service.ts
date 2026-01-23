import { api } from "@/lib/axios";
import type { ApiResponse } from "@/types/api";
import type {
  Addon,
  AddonGroup,
  CreateAddonGroupRequest,
  CreateAddonRequest,
  UpdateAddonGroupRequest,
  UpdateAddonRequest,
} from "./types";

export const addonsService = {
  // Groups
  getGroups: async (): Promise<AddonGroup[]> => {
    const response = await api.get<AddonGroup[]>("/addongroups");
    return response.data;
  },

  getGroupById: async (id: string): Promise<AddonGroup> => {
    const response = await api.get<AddonGroup>(`/addongroups/${id}`);
    return response.data;
  },

  createGroup: async (data: CreateAddonGroupRequest): Promise<ApiResponse<AddonGroup>> => {
    const response = await api.post<ApiResponse<AddonGroup>>("/addongroups", data);
    return response.data;
  },

  updateGroup: async (id: string, data: UpdateAddonGroupRequest): Promise<ApiResponse<AddonGroup>> => {
    const response = await api.put<ApiResponse<AddonGroup>>(`/addongroups/${id}`, data);
    return response.data;
  },

  deleteGroup: async (id: string): Promise<ApiResponse<boolean>> => {
    const response = await api.delete<ApiResponse<boolean>>(`/addongroups/${id}`);
    return response.data;
  },
  
  toggleGroupStatus: async (id: string): Promise<ApiResponse<AddonGroup>> => {
      const response = await api.patch<ApiResponse<AddonGroup>>(`/addongroups/${id}/toggle-status`);
      return response.data;
  },

  // Addons (Items)
  getAddons: async (): Promise<Addon[]> => {
    const response = await api.get<Addon[]>("/addons");
    return response.data;
  },

  createAddon: async (data: CreateAddonRequest): Promise<ApiResponse<Addon>> => {
    const response = await api.post<ApiResponse<Addon>>("/addons", data);
    return response.data;
  },

  updateAddon: async (id: string, data: UpdateAddonRequest): Promise<ApiResponse<Addon>> => {
    const response = await api.put<ApiResponse<Addon>>(`/addons/${id}`, data);
    return response.data;
  },

  deleteAddon: async (id: string): Promise<ApiResponse<boolean>> => {
    const response = await api.delete<ApiResponse<boolean>>(`/addons/${id}`);
    return response.data;
  },

  toggleAddonStatus: async (id: string): Promise<ApiResponse<Addon>> => {
    const response = await api.patch<ApiResponse<Addon>>(`/addons/${id}/toggle-status`);
    return response.data;
  },
};
