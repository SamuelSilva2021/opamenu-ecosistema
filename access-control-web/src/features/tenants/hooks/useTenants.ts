import { useState, useEffect, useCallback } from 'react';
import type { TenantSummary, Tenant, TenantFilters } from '../../../shared/types';
import { TenantService } from '../../../shared/services';

interface UseTenantsOptions {
  autoLoad?: boolean;
  pageSize?: number;
}

interface UseTenantsResult {
  tenants: TenantSummary[];
  loading: boolean;
  error: string | null;
  totalItems: number;
  currentPage: number;
  totalPages: number;
  filters: TenantFilters;
  loadTenants: (page?: number, searchFilters?: TenantFilters) => Promise<void>;
  handleSearch: (newFilters: TenantFilters) => Promise<void>;
  refetch: () => Promise<void>;
  clearError: () => void;
  getTenantById: (id: string) => Promise<Tenant>;
  updateTenant: (id: string, data: Partial<Tenant>) => Promise<Tenant>;
  deleteTenant: (id: string) => Promise<void>;
}

export const useTenants = (options: UseTenantsOptions = {}): UseTenantsResult => {
  const { autoLoad = true, pageSize = 10 } = options;

  const [tenants, setTenants] = useState<TenantSummary[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalItems, setTotalItems] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);
  const [filters, setFilters] = useState<TenantFilters>({});

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const loadTenants = useCallback(async (page: number = 1, searchFilters?: TenantFilters) => {
    setLoading(true);
    setError(null);

    const filtersToUse = searchFilters !== undefined ? searchFilters : filters;

    try {
      const response = await TenantService.getTenants({ page, limit: pageSize, filters: filtersToUse });

      setTenants(response.items || []);
      setCurrentPage(response.page || page);
      setTotalItems(response.total || 0);
      setTotalPages(response.totalPages || 0);
    } catch (err: unknown) {
      const message =
        typeof err === 'object' && err !== null && 'message' in err
          ? String((err as { message?: unknown }).message || 'Erro ao carregar tenants')
          : 'Erro ao carregar tenants';
      setError(message);
      setTenants([]);
    } finally {
      setLoading(false);
    }
  }, [pageSize, filters]);

  const handleSearch = useCallback(async (newFilters: TenantFilters) => {
    setFilters(newFilters);
    await loadTenants(1, newFilters);
  }, [loadTenants]);

  const refetch = useCallback(async () => {
    await loadTenants(currentPage);
  }, [loadTenants, currentPage]);

  const getTenantById = useCallback(async (id: string): Promise<Tenant> => {
    return TenantService.getTenantById(id);
  }, []);

  const updateTenant = useCallback(async (id: string, data: Partial<Tenant>): Promise<Tenant> => {
    setLoading(true);
    setError(null);

    try {
      const updated = await TenantService.updateTenant(id, data);

      setTenants(prev => prev.map(t => t.id === id ? { ...t, name: updated.name, slug: updated.slug, domain: updated.domain, status: updated.status, email: updated.email, phone: updated.phone } : t));

      return updated;
    } catch (err: unknown) {
      const message =
        typeof err === 'object' && err !== null && 'message' in err
          ? String((err as { message?: unknown }).message || 'Erro ao atualizar tenant')
          : 'Erro ao atualizar tenant';
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  }, []);

  const deleteTenant = useCallback(async (id: string): Promise<void> => {
    setLoading(true);
    setError(null);

    try {
      await TenantService.deleteTenant(id);
      setTenants(prev => prev.filter(t => t.id !== id));
      setTotalItems(prev => prev - 1);
    } catch (err: unknown) {
      const message =
        typeof err === 'object' && err !== null && 'message' in err
          ? String((err as { message?: unknown }).message || 'Erro ao remover tenant')
          : 'Erro ao remover tenant';
      setError(message);
      throw new Error(message);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (autoLoad) {
      loadTenants(1);
    }
  }, [autoLoad, loadTenants]);

  return {
    tenants,
    loading,
    error,
    totalItems,
    currentPage,
    totalPages,
    filters,
    loadTenants,
    handleSearch,
    refetch,
    clearError,
    getTenantById,
    updateTenant,
    deleteTenant,
  };
};
