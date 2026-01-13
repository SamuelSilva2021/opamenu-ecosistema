import { api } from "@/lib/axios";
import type { Customer } from "./types";

export const customersService = {
  getCustomers: async (): Promise<Customer[]> => {
    const response = await api.get<Customer[]>("/customers");
    return response.data;
  },
};
