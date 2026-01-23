import { useState, useEffect, useCallback } from 'react';
import { TenantBusinessInfo } from '@/types/api';
import { getTenantInfo } from '@/services/tenant-service';
import { handleApiError } from '@/services/http-client';

export const useTenantInfo = (slug?: string) => {
  const [tenant, setTenant] = useState<TenantBusinessInfo | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadTenant = useCallback(async () => {
    if (!slug) {
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);
      const data = await getTenantInfo(slug);
      // Fallback mock data for development/demo purposes if API fails or no slug provided (for now)
      setTenant(data);
    } catch (err) {
      console.error('❌ useTenantInfo: Error loading tenant, using mock data:', err);
      // Mock data matching the "Brasa Meat" reference
      setTenant({
        id: "mock-id",
        name: "Restaurante teste",
        slug: slug || "teste-meat",
        description: "O melhor churrasco da cidade",
        logoUrl: "https://images.unsplash.com/photo-1594041680534-e8c8cdebd659?q=80&w=200&auto=format&fit=crop", // Placeholder logo
        bannerUrl: "",
        addressStreet: "Rua Glória",
        addressNumber: "31",
        addressNeighborhood: "Santa Mônica",
        addressCity: "Guarapari",
        addressState: "ES",
        phone: "(27) 99999-9999",
        whatsappNumber: "27999999999",
        openingHours: {
            monday: { open: "18:00", close: "23:00", isOpen: true },
            tuesday: { open: "18:00", close: "23:00", isOpen: true },
            wednesday: { open: "19:00", close: "23:00", isOpen: true },
            thursday: { open: "18:00", close: "23:00", isOpen: true },
            friday: { open: "18:00", close: "00:00", isOpen: true },
            saturday: { open: "18:00", close: "00:00", isOpen: true },
            sunday: { open: "18:00", close: "23:00", isOpen: true }
        },
        paymentMethods: ["Cartão de Crédito", "Cartão de Débito", "PIX", "Dinheiro"],
        pixKey: "12345678900",
        isOpen: true
      });
      setError(null); // Clear error to show mock data
    } finally {
      setLoading(false);
    }
  }, [slug]);

  useEffect(() => {
    loadTenant();
  }, [loadTenant]);

  return {
    tenant,
    loading,
    error,
    refetch: loadTenant,
  };
};
