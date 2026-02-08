import type { SimplifiedRole } from "@/features/auth/types";

export interface Employee {
    id: string;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    fullName: string;
    status: string;
    roleId?: string;
    roleName?: string;
    role?: SimplifiedRole;
    createdAt: string;
    updatedAt?: string;
    lastLoginAt?: string;
    isEmailVerified: boolean;
}

export interface CreateEmployeeRequest {
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    password?: string;
    confirmPassword?: string;
    roleId?: string;
    status?: string;
}

export interface UpdateEmployeeRequest {
    firstName: string;
    lastName: string;
    email: string;
    roleId?: string;
    status?: string;
}

export interface Role {
    id: string;
    name: string;
    description?: string;
    isDefault: boolean;
}

export interface RolesApiResponse {
    items: Role[];
    total: number;
    page: number;
    limit: number;
}
