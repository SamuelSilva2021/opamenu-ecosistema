import { api } from "@/lib/axios";
import type { BankDetailsDto, CreateBankDetailsRequestDto, UpdateBankDetailsRequestDto } from "./types";

export const bankDetailsService = {
  getAll: async () => {
    const response = await api.get<BankDetailsDto[]>("/bank-details");
    return response.data;
  },
  getById: async (id: string) => {
    const response = await api.get<BankDetailsDto>(`/bank-details/${id}`);
    return response.data;
  },
  create: async (data: CreateBankDetailsRequestDto) => {
    const response = await api.post<BankDetailsDto>("/bank-details", data);
    return response.data;
  },
  update: async (data: UpdateBankDetailsRequestDto) => {
    const response = await api.put<BankDetailsDto>(`/bank-details/${data.id}`, data);
    return response.data;
  },
  delete: async (id: string) => {
    const response = await api.delete<boolean>(`/bank-details/${id}`);
    return response.data;
  }
};
