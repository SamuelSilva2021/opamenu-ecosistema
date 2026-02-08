import { api } from "@/lib/axios";
import type { Customer } from "./types";

export const customersService = {
  getCustomers: async (params?: { search?: string }): Promise<Customer[]> => {
    const response = await api.get<Customer[] | { data: Customer[] }>("/customers", { params });
    if (Array.isArray(response.data)) {
      return response.data;
    }
    return (response.data as any).data || [];
  },

  getCustomersPaginated: async (params: { page: number; limit: number; search?: string }): Promise<{ data: Customer[]; nextCursor?: number }> => {
    const response = await api.get<Customer[] | { data: Customer[], meta: any }>("/customers", { params });

    if (Array.isArray(response.data)) {
      const allCustomers = response.data;

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
      const pagedData = response.data as any;
      return {
        data: pagedData.data || [],
        nextCursor: pagedData.currentPage < pagedData.totalPages ? pagedData.currentPage + 1 : undefined
      };
    }
  },
};
