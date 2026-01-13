import { useState, useEffect, useCallback } from 'react';
import { StorefrontData, TenantBusinessInfo, Product, Category } from '@/types/api';
import { getStorefrontData } from '@/services/storefront-service';

export const useStorefront = (slug?: string) => {
  const [data, setData] = useState<StorefrontData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadStorefront = useCallback(async () => {
    if (!slug) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const result = await getStorefrontData(slug);
      setData(result);
    } catch (err: any) {
      console.error('Error loading storefront data:', err);
      
      // Verifica se o erro contém o código específico de erro interno
      let isInternalError = false;

      // Caso 1: Array de erros em err.data (formato padrão da API para validações)
      if (err?.data && Array.isArray(err.data)) {
        isInternalError = err.data.some((e: any) => e.code === 'COMMON_INTERNAL_SERVER_ERROR');
      }
      // Caso 2: Objeto único em err.data
      else if (err?.data?.code === 'COMMON_INTERNAL_SERVER_ERROR') {
        isInternalError = true;
      }

      if (isInternalError) {
        setError('COMMON_INTERNAL_SERVER_ERROR');
      } else {
        setError('Falha ao carregar dados da loja.');
      }
    } finally {
      setLoading(false);
    }
  }, [slug]);

  useEffect(() => {
    loadStorefront();
  }, [loadStorefront]);

  return {
    tenantBusiness: data?.tenantBusiness ?? null,
    products: data?.products ?? [],
    categories: data?.categories ?? [],
    coupons: data?.coupons ?? [],
    loading,
    error,
    refetch: loadStorefront,
  };
};
