import { api } from '../lib/api';
import type { ApiResponse, CreatePlanDTO, Plan, PlanFilter, UpdatePlanDTO, PlanListResponse } from '../types/plan.types';

export const planService = {
  getAll: async (filter: PlanFilter) => {
    const params = new URLSearchParams();
    if (filter.name) params.append('name', filter.name);
    if (filter.isActive !== undefined) params.append('isActive', filter.isActive.toString());
    if (filter.status) params.append('status', filter.status);
    params.append('page', filter.page.toString());
    params.append('pageSize', filter.pageSize.toString());

    const response = await api.get<PlanListResponse>(`/plans?${params.toString()}`);
    return response.data;
  },

  getById: async (id: string) => {
    const response = await api.get<ApiResponse<Plan>>(`/plans/${id}`);
    return response.data;
  },

  create: async (data: CreatePlanDTO) => {
    const response = await api.post<Plan>('/plans', data);
    return response.data;
  },

  update: async (id: string, data: Omit<UpdatePlanDTO, 'id'>) => {
    const response = await api.put<Plan>(`/plans/${id}`, data);
    return response.data;
  },

  delete: async (id: string) => {
    const response = await api.delete<boolean>(`/plans/${id}`);
    return response.data;
  }
};
