import type { PagedResponse, Tenant, TenantSummary, TenantFilters } from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

interface GetTenantsParams {
  page?: number;
  limit?: number;
  filters?: TenantFilters;
}

export class TenantService {
  static async getTenants(params: GetTenantsParams = {}): Promise<PagedResponse<TenantSummary>> {
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
    const response = await httpClient.get<PagedResponse<TenantSummary>>(url);

    if (response.succeeded === false) {
      throw new Error('Falha ao buscar tenants');
    }

    return response;
  }

  static async getTenantById(id: string): Promise<Tenant> {
    const tenant = await httpClient.get<Tenant>(API_ENDPOINTS.TENANT_BY_ID(id));

    if (!tenant || !tenant.id) {
      throw new Error('Tenant não encontrado');
    }

    return tenant;
  }

  static async updateTenant(id: string, data: Partial<Tenant>): Promise<Tenant> {
    const tenant = await httpClient.put<Tenant>(API_ENDPOINTS.TENANT_BY_ID(id), data);

    if (!tenant || !tenant.id) {
      throw new Error('Erro ao atualizar tenant');
    }

    return tenant;
  }

  static async deleteTenant(id: string): Promise<void> {
    await httpClient.delete<boolean | void>(API_ENDPOINTS.TENANT_BY_ID(id));
  }
}
