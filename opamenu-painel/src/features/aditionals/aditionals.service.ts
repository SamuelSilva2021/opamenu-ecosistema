import { api } from "@/lib/axios";
import type { ApiResponse } from "@/types/api";
import type {
  Aditional,
  AditionalGroup,
  CreateAditionalGroupRequest,
  CreateAditionalRequest,
  UpdateAditionalGroupRequest,
  UpdateAditionalRequest,
} from "./types";

export const aditionalsService = {
  // Groups
  getGroups: async (): Promise<AditionalGroup[]> => {
    const response = await api.get<AditionalGroup[]>("/aditionalgroups");
    return response.data;
  },

  getGroupById: async (id: string): Promise<AditionalGroup> => {
    const response = await api.get<AditionalGroup>(`/aditionalgroups/${id}`);
    return response.data;
  },

  createGroup: async (data: CreateAditionalGroupRequest): Promise<ApiResponse<AditionalGroup>> => {
    const response = await api.post<ApiResponse<AditionalGroup>>("/aditionalgroups", data);
    return response.data;
  },

  updateGroup: async (id: string, data: UpdateAditionalGroupRequest): Promise<ApiResponse<AditionalGroup>> => {
    const response = await api.put<ApiResponse<AditionalGroup>>(`/aditionalgroups/${id}`, data);
    return response.data;
  },

  deleteGroup: async (id: string): Promise<ApiResponse<boolean>> => {
    const response = await api.delete<ApiResponse<boolean>>(`/aditionalgroups/${id}`);
    return response.data;
  },

  toggleGroupStatus: async (id: string): Promise<ApiResponse<AditionalGroup>> => {
    const response = await api.patch<ApiResponse<AditionalGroup>>(`/aditionalgroups/${id}/toggle-status`);
    return response.data;
  },

  // Aditionals (Items)
  getAditionals: async (): Promise<Aditional[]> => {
    const response = await api.get<Aditional[]>("/aditionals");
    return response.data;
  },

  createAditional: async (data: CreateAditionalRequest): Promise<ApiResponse<Aditional>> => {
    const response = await api.post<ApiResponse<Aditional>>("/aditionals", data);
    return response.data;
  },

  updateAditional: async (id: string, data: UpdateAditionalRequest): Promise<ApiResponse<Aditional>> => {
    const response = await api.put<ApiResponse<Aditional>>(`/aditionals/${id}`, data);
    return response.data;
  },

  deleteAditional: async (id: string): Promise<ApiResponse<boolean>> => {
    const response = await api.delete<ApiResponse<boolean>>(`/aditionals/${id}`);
    return response.data;
  },

  toggleAditionalStatus: async (id: string): Promise<ApiResponse<Aditional>> => {
    const response = await api.patch<ApiResponse<Aditional>>(`/aditionals/${id}/toggle-status`);
    return response.data;
  },
};
