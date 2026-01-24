import { useState, useEffect, useCallback } from 'react';
import { Product, Category } from '@/types/api';
import { 
  getCachedProducts, 
  getCachedCategories, 
  productService,
  categoryService 
} from '@/services/product-service';
import { handleApiError } from '@/services/http-client';

// Hook para carregar produtos do menu
export const useMenuProducts = (slug?: string) => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadProducts = useCallback(async () => {
    if (!slug) {
        setLoading(false);
        return;
    }

    try {
      setLoading(true);
      setError(null);
      
      const data = await getCachedProducts(slug);
      // Verificar se os dados são um array ou um objeto com array
      let productsArray: Product[] = [];
      
      if (Array.isArray(data)) {
        productsArray = data;
      } else if (data && typeof data === 'object') {
        const possibleArrayKeys = ['data', 'products', 'items', 'result', 'content'];
        
        for (const key of possibleArrayKeys) {
          if (data[key] && Array.isArray(data[key])) {
            productsArray = data[key];
            break;
          }
        }
        
        // Se não encontrou em propriedades específicas, verificar todas as propriedades
        if (productsArray.length === 0) {
          const values = Object.values(data);
          const arrayValue = values.find(value => Array.isArray(value));
          if (arrayValue) {
            productsArray = arrayValue as Product[];
          }
        }
      }
      
      setProducts(productsArray);
    } catch (err) {
      console.error('❌ useMenuProducts: Error loading products:', err);
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  }, [slug]);
  
  useEffect(() => {
    loadProducts();
  }, [loadProducts]);

  return {
    products,
    loading,
    error,
    refetch: loadProducts,
  };
};

// Hook para carregar categorias ativas
export const useCategories = (slug?: string) => {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadCategories = useCallback(async () => {
    if (!slug) {
        setLoading(false);
        return;
    }

    try {
      setLoading(true);
      setError(null);
      
      const data = await getCachedCategories(slug);
      
      // Verificar se os dados são um array ou um objeto com array
      let categoriesArray: Category[] = [];
      
      if (Array.isArray(data)) {
        categoriesArray = data;
      } else if (data && typeof data === 'object') {
        // Tentar encontrar o array dentro do objeto
        const possibleArrayKeys = ['data', 'categories', 'items', 'result', 'content'];
        
        for (const key of possibleArrayKeys) {
          if (data[key] && Array.isArray(data[key])) {
            categoriesArray = data[key];
            break;
          }
        }
        
        // Se não encontrou em propriedades específicas, verificar todas as propriedades
        if (categoriesArray.length === 0) {
          const values = Object.values(data);
          const arrayValue = values.find(value => Array.isArray(value));
          if (arrayValue) {
            categoriesArray = arrayValue as Category[];
          }
        }
      }
      
      setCategories(categoriesArray);
    } catch (err) {
      console.error('❌ useCategories: Error loading categories:', err);
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  }, [slug]);

  useEffect(() => {
    loadCategories();
  }, [loadCategories]);

  return {
    categories,
    loading,
    error,
    refetch: loadCategories,
  };
};

// Hook para carregar produtos por categoria
export const useProductsByCategory = (categoryId: string | null) => {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadProductsByCategory = useCallback(async (catId: string) => {
    try {
      setLoading(true);
      setError(null);
      
      const data = await productService.getProductsByCategory(catId);
      setProducts(data);
    } catch (err) {
      console.error('Error loading products by category:', err);
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (categoryId) {
      loadProductsByCategory(categoryId);
    } else {
      setProducts([]);
    }
  }, [categoryId, loadProductsByCategory]);

  return {
    products,
    loading,
    error,
    refetch: categoryId ? () => loadProductsByCategory(categoryId) : undefined,
  };
};

// Hook para busca de produtos
export const useProductSearch = () => {
  const [searchResults, setSearchResults] = useState<Product[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const searchProducts = useCallback(async (
    query: string,
    categoryId?: string
  ) => {
    if (!query.trim()) {
      setSearchResults([]);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      
      const response = await productService.getProducts({
        search: query,
        categoryId,
        isActive: true,
        pageSize: 50, // Limite para busca
      });
      
      setSearchResults(response.items);
    } catch (err) {
      console.error('Error searching products:', err);
      setError(handleApiError(err));
    } finally {
      setLoading(false);
    }
  }, []);

  const clearSearch = useCallback(() => {
    setSearchResults([]);
    setError(null);
  }, []);

  return {
    searchResults,
    loading,
    error,
    searchProducts,
    clearSearch,
  };
};

// Hook para verificar conectividade da API
export const useApiHealth = () => {
  const [isOnline, setIsOnline] = useState(true);
  const [checking, setChecking] = useState(false);

  const checkHealth = useCallback(async () => {
    try {
      setChecking(true);
      const { httpClient } = await import('@/services/http-client');
      const isHealthy = await httpClient.healthCheck();
      setIsOnline(isHealthy);
      return isHealthy;
    } catch {
      setIsOnline(false);
      return false;
    } finally {
      setChecking(false);
    }
  }, []);

  useEffect(() => {
    checkHealth();
    
    // Verificar conectividade periodicamente
    const interval = setInterval(checkHealth, 30000); // 30 segundos
    
    return () => clearInterval(interval);
  }, [checkHealth]);

  return {
    isOnline,
    checking,
    checkHealth,
  };
};

// Hook para debounce (útil para busca)
export const useDebounce = <T>(value: T, delay: number): T => {
  const [debouncedValue, setDebouncedValue] = useState<T>(value);

  useEffect(() => {
    const handler = setTimeout(() => {
      setDebouncedValue(value);
    }, delay);

    return () => {
      clearTimeout(handler);
    };
  }, [value, delay]);

  return debouncedValue;
};
