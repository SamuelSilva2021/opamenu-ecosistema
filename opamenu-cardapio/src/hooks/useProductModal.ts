import { useState, useCallback } from 'react';
import { ProductWithAddons, ProductSelection } from '@/types/api';
import { getCachedProductWithAddons } from '@/services/product-service';

export const useProductModal = (slug?: string) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<ProductWithAddons | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const openModal = useCallback(async (productId: number) => {
    if (!slug) return;

    setIsLoading(true);
    setError(null);
    
    try {
      const productWithAddons = await getCachedProductWithAddons(productId, slug);
      setSelectedProduct(productWithAddons);
      setIsOpen(true);
    } catch (err) {
      setError('Erro ao carregar detalhes do produto. Tente novamente.');
    } finally {
      setIsLoading(false);
    }
  }, [slug]);

  const closeModal = useCallback(() => {
    setIsOpen(false);
    setSelectedProduct(null);
    setError(null);
  }, []);

  return {
    isOpen,
    selectedProduct,
    isLoading,
    error,
    openModal,
    closeModal,
  };
};
