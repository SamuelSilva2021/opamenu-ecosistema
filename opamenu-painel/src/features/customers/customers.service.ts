import { api } from "@/lib/axios";
import type { Customer } from "./types";

export const customersService = {
  getCustomers: async (params?: { search?: string }): Promise<Customer[]> => {
    const response = await api.get<Customer[]>("/customers", { params });
    return response.data;
  },

  getCustomersPaginated: async (params: { page: number; limit: number; search?: string }): Promise<{ data: Customer[]; nextCursor?: number }> => {
    // Primeiro tentamos passar os parâmetros para a API
    const response = await api.get<Customer[] | { data: Customer[], meta: any }>("/customers", { params });
    
    // Verificamos o formato da resposta
    if (Array.isArray(response.data)) {
      // API Legada: Retorna todos os registros. Simulamos paginação no frontend.
      // Isso garante performance de renderização, mas não de rede (até o backend ser atualizado).
      const allCustomers = response.data;
      
      // Filtragem manual caso a API não tenha filtrado (redundância segura)
      const filtered = params.search 
        ? allCustomers.filter(c => 
            c.name.toLowerCase().includes(params.search!.toLowerCase()) || 
            c.phone.includes(params.search!)
          )
        : allCustomers;

      const start = (params.page - 1) * params.limit;
      const end = start + params.limit;
      const sliced = filtered.slice(start, end);
      
      return {
        data: sliced,
        nextCursor: end < filtered.length ? params.page + 1 : undefined
      };
    } else {
      // API Nova (Paginada): Retorna estrutura correta
      return {
        data: response.data.data,
        nextCursor: response.data.meta.page < response.data.meta.lastPage ? response.data.meta.page + 1 : undefined
      };
    }
  },
};
