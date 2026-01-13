import { apiAuth } from "@/lib/axios";
import type { AuthResponse, LoginRequest, PermissionsResponse, RegisterTenantRequest, PlanDto } from "./types";
import type { RegisterFormData } from "./validation";

export const authService = {
  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await apiAuth.post<AuthResponse>("/auth/login", data);
    return response.data;
  },

  register: async (data: RegisterFormData): Promise<ApiResponse<any>> => {
        const payload: RegisterTenantRequest = {
            companyName: data.companyName,
            document: data.document,
            firstName: data.firstName,
            lastName: data.lastName,
            email: data.email,
            password: data.password,
            confirmPassword: data.confirmPassword
        };
        const response = await apiAuth.post<ApiResponse<any>>("/Register", payload);
        return response.data;
    },

    getPlans: async (): Promise<PlanDto[]> => {
        const response = await apiAuth.get<PlanDto[]>("/plans/active");
        return response.data;
    },

    activatePlan: async (planId: string): Promise<ApiResponse<string>> => {
        const response = await apiAuth.post<ApiResponse<string>>(`/subscription/activate/${planId}`, {});
        return response.data;
    },

  getPermissions: async (): Promise<PermissionsResponse> => {
    const response = await apiAuth.get<PermissionsResponse>("/auth/me");
    return response.data;
  },
};

interface ApiResponse<T> {
    succeeded: boolean;
    message: string | null;
    errors: any[] | null;
    data: T;
}
