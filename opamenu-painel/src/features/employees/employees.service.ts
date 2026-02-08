import { apiAuth } from "@/lib/axios";
import type { ApiResponse } from "@/types/api";
import type {
    Employee,
    CreateEmployeeRequest,
    UpdateEmployeeRequest,
    RolesApiResponse,
} from "./types";

export const employeesService = {
    getEmployees: async (params: { page?: number; limit?: number; search?: string } = {}): Promise<ApiResponse<{ items: Employee[]; total: number }>> => {
        const { page = 1, limit = 10, search } = params;
        const searchParams = new URLSearchParams({
            page: page.toString(),
            limit: limit.toString(),
            ...(search && { search }),
        });

        const response = await apiAuth.get<ApiResponse<{ items: Employee[]; total: number }>>(
            `/user-accounts-painel?${searchParams}`
        );
        return response.data;
    },

    getEmployeeById: async (id: string): Promise<ApiResponse<Employee>> => {
        const response = await apiAuth.get<ApiResponse<Employee>>(`/user-accounts-painel/${id}`);
        return response.data;
    },

    createEmployee: async (data: CreateEmployeeRequest): Promise<ApiResponse<Employee>> => {
        const response = await apiAuth.post<ApiResponse<Employee>>("/user-accounts-painel", data);
        return response.data;
    },

    updateEmployee: async (id: string, data: UpdateEmployeeRequest): Promise<ApiResponse<Employee>> => {
        const response = await apiAuth.put<ApiResponse<Employee>>(`/user-accounts-painel/${id}`, data);
        return response.data;
    },

    deleteEmployee: async (id: string): Promise<ApiResponse<boolean>> => {
        const response = await apiAuth.delete<ApiResponse<boolean>>(`/user-accounts-painel/${id}`);
        return response.data;
    },

    toggleEmployeeStatus: async (id: string): Promise<ApiResponse<Employee>> => {
        const response = await apiAuth.patch<ApiResponse<Employee>>(`/user-accounts-painel/${id}/toggle-status`);
        return response.data;
    },

    // Roles management
    getRoles: async (params: { page?: number; limit?: number; search?: string } = {}): Promise<ApiResponse<RolesApiResponse>> => {
        const { page = 1, limit = 10, search } = params;
        const searchParams = new URLSearchParams({
            page: page.toString(),
            limit: limit.toString(),
            ...(search && { search }),
        });

        const response = await apiAuth.get<ApiResponse<RolesApiResponse>>(
            `/roles-painel?${searchParams}`
        );
        return response.data;
    },
};
