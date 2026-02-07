import type { PaginatedResponse, Tenant, TenantSummary, TenantFilters, ApiResponse, Module } from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

interface GetTenantsParams {
  page?: number;
  limit?: number;
  filters?: TenantFilters;
}

interface RawTenantResponse {
  items: TenantSummary[];
  page: number;
  limit: number;
  total: number;
  totalPages: number;
  succeeded: boolean | null;
}

export class TenantService {
  static async getTenants(params: GetTenantsParams = {}): Promise<PaginatedResponse<TenantSummary>> {
    const { page = 1, limit = 10, filters } = params;

    const queryParams: Record<string, string> = {
      page: page.toString(),
      limit: limit.toString(),
    };

    if (filters) {
      if (filters.name) queryParams['filter.name'] = filters.name;
      if (filters.slug) queryParams['filter.slug'] = filters.slug;
      if (filters.domain) queryParams['filter.domain'] = filters.domain;
      if (filters.email) queryParams['filter.email'] = filters.email;
      if (filters.phone) queryParams['filter.phone'] = filters.phone;
      if (filters.status) queryParams['filter.status'] = filters.status;
    }

    const searchParams = new URLSearchParams(queryParams);

    const url = `${API_ENDPOINTS.TENANTS}?${searchParams}`;
    const response = await httpClient.get<RawTenantResponse | PaginatedResponse<TenantSummary> | ApiResponse<PaginatedResponse<TenantSummary>>>(url);

    if ('items' in response && Array.isArray(response.items)) {
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

    if ('succeeded' in response && 'data' in response) {
      if (response.succeeded === false) {
        throw new Error('Falha ao buscar tenants');
      }
      return response.data;
    }

    return response as PaginatedResponse<TenantSummary>;
  }

  static async getTenantById(id: string): Promise<Tenant> {
    const response = await httpClient.get<Tenant | ApiResponse<Tenant>>(API_ENDPOINTS.TENANT_BY_ID(id));

    let tenant: Tenant;
    if ('succeeded' in response) {
      if (response.succeeded === false) {
        throw new Error('Tenant não encontrado');
      }
      tenant = response.data || (response as unknown as Tenant);
    } else {
      tenant = response as Tenant;
    }

    if (!tenant || !tenant.id) {
      throw new Error('Tenant não encontrado');
    }

    return tenant;
  }

  static async updateTenant(id: string, data: Partial<Tenant>): Promise<Tenant> {
    const response = await httpClient.put<Tenant | ApiResponse<Tenant>>(API_ENDPOINTS.TENANT_BY_ID(id), data);

    let tenant: Tenant;
    if ('succeeded' in response) {
      if (response.succeeded === false) {
        throw new Error('Erro ao atualizar tenant');
      }
      tenant = response.data || (response as unknown as Tenant);
    } else {
      tenant = response as Tenant;
    }

    if (!tenant || !tenant.id) {
      throw new Error('Erro ao atualizar tenant');
    }

    return tenant;
  }

  static async deleteTenant(id: string): Promise<void> {
    await httpClient.delete<boolean | void>(API_ENDPOINTS.TENANT_BY_ID(id));
  }

  static async getModules(tenantId: string): Promise<Module[]> {
    const response = await httpClient.get<Module[] | ApiResponse<Module[]>>(`${API_ENDPOINTS.TENANTS}/${tenantId}/modules`);

    if ('succeeded' in response) {
      if (!response.succeeded) {
        throw new Error('Erro ao buscar módulos do tenant');
      }
      return response.data;
    }

    return response as Module[];
  }

  static async addModule(tenantId: string, moduleId: string): Promise<void> {
    const response = await httpClient.post<boolean | ApiResponse<boolean>>(`${API_ENDPOINTS.TENANTS}/${tenantId}/modules/${moduleId}`, {});

    if (typeof response === 'object' && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error('Erro ao adicionar módulo ao tenant');
      }
    }
  }

  static async removeModule(tenantId: string, moduleId: string): Promise<void> {
    const response = await httpClient.delete<boolean | ApiResponse<boolean>>(`${API_ENDPOINTS.TENANTS}/${tenantId}/modules/${moduleId}`);

    if (typeof response === 'object' && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error('Erro ao remover módulo do tenant');
      }
    }
  }
}
