import { api } from "@/lib/axios";
import type { ApiResponse } from "@/types/api";
import type { DeliveryArea, CreateDeliveryAreaRequest } from "./types";

export const deliveryAreaService = {
  getDeliveryAreas: async (): Promise<DeliveryArea[]> => {
    const response = await api.get<DeliveryArea[]>("/delivery-areas");
    return response.data;
  },

  createDeliveryArea: async (data: CreateDeliveryAreaRequest): Promise<ApiResponse<DeliveryArea>> => {
    const response = await api.post<ApiResponse<DeliveryArea>>("/delivery-areas", data);
    return response.data;
  },

  updateDeliveryArea: async (id: string, data: CreateDeliveryAreaRequest): Promise<ApiResponse<DeliveryArea>> => {
    const response = await api.put<ApiResponse<DeliveryArea>>(`/delivery-areas/${id}`, data);
    return response.data;
  },

  deleteDeliveryArea: async (id: string): Promise<ApiResponse<boolean>> => {
    const response = await api.delete<ApiResponse<boolean>>(`/delivery-areas/${id}`);
    return response.data;
  },
};
