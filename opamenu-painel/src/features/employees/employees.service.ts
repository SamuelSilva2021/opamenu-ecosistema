import { apiAuth } from "@/lib/axios";
import type { ApiResponse } from "@/types/api";
import type {
    Employee,
    CreateEmployeeRequest,
    UpdateEmployeeRequest,
    RolesApiResponse,
    Role,
    Module
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
            ...(search && { name: search }), // Changed 'search' to 'name' to match backend
        });

        const response = await apiAuth.get<ApiResponse<RolesApiResponse>>(
            `/roles-painel?${searchParams}`
        );
        return response.data;
    },

    getRoleById: async (id: string): Promise<ApiResponse<Role>> => {
        const response = await apiAuth.get<ApiResponse<Role>>(`/roles-painel/${id}`);
        return response.data;
    },

    createRole: async (data: any): Promise<ApiResponse<Role>> => {
        const response = await apiAuth.post<ApiResponse<Role>>("/roles-painel", data);
        return response.data;
    },

    updateRole: async (id: string, data: any): Promise<ApiResponse<Role>> => {
        const response = await apiAuth.put<ApiResponse<Role>>(`/roles-painel/${id}`, data);
        return response.data;
    },

    deleteRole: async (id: string): Promise<ApiResponse<boolean>> => {
        const response = await apiAuth.delete<ApiResponse<boolean>>(`/roles-painel/${id}`);
        return response.data;
    },

    getAvailableModules: async (): Promise<ApiResponse<Module[]>> => {
        const response = await apiAuth.get<ApiResponse<Module[]>>("/roles-painel/modules");
        return response.data;
    },
};
