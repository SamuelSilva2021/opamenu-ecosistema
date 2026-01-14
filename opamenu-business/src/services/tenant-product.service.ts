import { api } from '../lib/api';
import type { TenantProduct, CreateTenantProductDTO, UpdateTenantProductDTO } from '../types/tenant-product.types';

export const tenantProductService = {
  getAll: async () => {
    const response = await api.get<TenantProduct[]>('/tenant-products');
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<TenantProduct>(`/tenant-products/${id}`);
    return response.data;
  },

  create: async (data: CreateTenantProductDTO) => {
    const response = await api.post<TenantProduct>('/tenant-products', data);
    return response.data;
  },

  update: async (id: string, data: UpdateTenantProductDTO) => {
    const response = await api.put<TenantProduct>(`/tenant-products/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    const response = await api.delete<boolean>(`/tenant-products/${id}`);
    return response.data;
  }
};
