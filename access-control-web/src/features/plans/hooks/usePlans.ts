import { useState, useEffect, useCallback } from 'react';
import type { SubscriptionPlan } from '../../../shared/types';
import { PlanService, PlanFilters } from '../../../shared/services/plan.service';

interface UsePlansOptions {
  autoLoad?: boolean;
  pageSize?: number;
}

export const usePlans = (options: UsePlansOptions = {}) => {
  const { autoLoad = true, pageSize = 10 } = options;

  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [totalItems, setTotalItems] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [filters, setFilters] = useState<PlanFilters>({});

  const loadPlans = useCallback(async (page: number = 1, searchFilters?: PlanFilters) => {
    setLoading(true);
    setError(null);

    const filtersToUse = searchFilters !== undefined ? searchFilters : filters;

    try {
      const response = await PlanService.getPlans({ page, limit: pageSize, filters: filtersToUse });
      setPlans(response.data || []);
      setCurrentPage(response.pageNumber || page);
      setTotalItems(response.totalCount || 0);
    } catch (err: any) {
      setError(err.message || 'Erro ao carregar planos');
      setPlans([]);
    } finally {
      setLoading(false);
    }
  }, [pageSize, filters]);

  const handleSearch = useCallback(async (newFilters: PlanFilters) => {
    setFilters(newFilters);
    await loadPlans(1, newFilters);
  }, [loadPlans]);

  const createPlan = useCallback(async (data: Partial<SubscriptionPlan>) => {
    setLoading(true);
    try {
      const newPlan = await PlanService.createPlan(data);
      await loadPlans(currentPage);
      return newPlan;
    } catch (err: any) {
      setError(err.message || 'Erro ao criar plano');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadPlans, currentPage]);

  const updatePlan = useCallback(async (id: string, data: Partial<SubscriptionPlan>) => {
    setLoading(true);
    try {
      const updated = await PlanService.updatePlan(id, data);
      await loadPlans(currentPage);
      return updated;
    } catch (err: any) {
      setError(err.message || 'Erro ao atualizar plano');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadPlans, currentPage]);

  const deletePlan = useCallback(async (id: string) => {
    setLoading(true);
    try {
      await PlanService.deletePlan(id);
      await loadPlans(currentPage);
    } catch (err: any) {
      setError(err.message || 'Erro ao remover plano');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadPlans, currentPage]);

  useEffect(() => {
    if (autoLoad) {
      loadPlans(1);
    }
  }, [autoLoad, loadPlans]);

  return {
    plans,
    loading,
    error,
    totalItems,
    currentPage,
    loadPlans,
    handleSearch,
    createPlan,
    updatePlan,
    deletePlan,
    clearError: () => setError(null)
  };
};
