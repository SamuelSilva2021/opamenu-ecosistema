import type { PaginatedResponse, ApiResponse, SubscriptionPlan } from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

export interface PlanFilters {
  name?: string;
  isActive?: boolean;
}

interface GetPlansParams {
  page?: number;
  limit?: number;
  filters?: PlanFilters;
}

export class PlanService {
  static async getPlans(params: GetPlansParams = {}): Promise<PaginatedResponse<SubscriptionPlan>> {
    const { page = 1, limit = 10, filters } = params;

    const queryParams: Record<string, string> = {
      page: page.toString(),
      limit: limit.toString(),
    };

    if (filters) {
      if (filters.name) queryParams['filter.name'] = filters.name;
      if (filters.isActive !== undefined) queryParams['filter.isActive'] = filters.isActive.toString();
    }

    const searchParams = new URLSearchParams(queryParams);
    const url = `${API_ENDPOINTS.PLANS}?${searchParams}`;
    const response = await httpClient.get<any>(url);

    if (response && 'items' in response && Array.isArray(response.items)) {
      return {
        data: response.items,
        totalCount: response.total || 0,
        pageNumber: response.page || page,
        pageSize: response.limit || limit,
        totalPages: response.totalPages || 0,
        hasPreviousPage: (response.page || page) > 1,
        hasNextPage: (response.page || page) < (response.totalPages || 0)
      };
    }

    if (response && 'succeeded' in response && 'data' in response) {
      const paginatedData = response.data;
      if (paginatedData && 'items' in paginatedData && Array.isArray(paginatedData.items)) {
        return {
          data: paginatedData.items,
          totalCount: paginatedData.total || 0,
          pageNumber: paginatedData.page || page,
          pageSize: paginatedData.limit || limit,
          totalPages: paginatedData.totalPages || 0,
          hasPreviousPage: (paginatedData.page || page) > 1,
          hasNextPage: (paginatedData.page || page) < (paginatedData.totalPages || 0)
        };
      }
      return response.data;
    }

    return response as PaginatedResponse<SubscriptionPlan>;
  }

  static async getPlanById(id: string): Promise<SubscriptionPlan> {
    const response = await httpClient.get<SubscriptionPlan | ApiResponse<SubscriptionPlan>>(API_ENDPOINTS.PLAN_BY_ID(id));
    
    if ('succeeded' in response) {
      if (!response.succeeded) throw new Error('Plano não encontrado');
      return response.data;
    }
    return response as SubscriptionPlan;
  }

  static async createPlan(data: Partial<SubscriptionPlan>): Promise<SubscriptionPlan> {
    const response = await httpClient.post<SubscriptionPlan | ApiResponse<SubscriptionPlan>>(API_ENDPOINTS.PLANS, data);
    
    if ('succeeded' in response) {
      if (!response.succeeded) throw new Error('Erro ao criar plano');
      return response.data;
    }
    return response as SubscriptionPlan;
  }

  static async updatePlan(id: string, data: Partial<SubscriptionPlan>): Promise<SubscriptionPlan> {
    const response = await httpClient.put<SubscriptionPlan | ApiResponse<SubscriptionPlan>>(API_ENDPOINTS.PLAN_BY_ID(id), data);
    
    if ('succeeded' in response) {
      if (!response.succeeded) throw new Error('Erro ao atualizar plano');
      return response.data;
    }
    return response as SubscriptionPlan;
  }

  static async deletePlan(id: string): Promise<void> {
    await httpClient.delete<void>(API_ENDPOINTS.PLAN_BY_ID(id));
  }
}
