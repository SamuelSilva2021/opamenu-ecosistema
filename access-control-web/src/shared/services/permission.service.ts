import type {
    Permission,
    CreatePermissionRequest,
    UpdatePermissionRequest,
    ApiResponse
} from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

interface GetPermissionsParams {
    page?: number;
    limit?: number;
    search?: string;
    moduleId?: string;
    tenantId?: string;
}

interface PermissionsApiResponse {
    items: Permission[];
    page: number;
    limit: number;
    total: number;
    totalPages: number;
}

interface PermissionsDirectResponse {
    items: Permission[];
    page: number;
    limit: number;
    total: number;
    totalPages: number;
    succeeded?: boolean | null;
    code?: number;
    currentPage?: number;
    pageSize?: number;
    errors?: string[];
}

export class PermissionService {
    static async getPermissions(params: GetPermissionsParams = {}): Promise<PermissionsApiResponse> {
        try {
            const { page = 1, limit = 10, search, moduleId, tenantId } = params;

            const searchParams = new URLSearchParams({
                page: page.toString(),
                limit: limit.toString(),
                ...(search && { search }),
                ...(moduleId && { moduleId }),
                ...(tenantId && { tenantId }),
            });

            const url = `${API_ENDPOINTS.PERMISSIONS}?${searchParams}`;
            const response = await httpClient.get<PermissionsApiResponse | ApiResponse<PermissionsApiResponse> | PermissionsDirectResponse>(url);

            // Verifica se é o formato direto paginado
            const raw = response as any;
            if (raw && Array.isArray(raw.items)) {
                const apiData = raw as PermissionsDirectResponse;
                return {
                    items: apiData.items,
                    page: apiData.page ?? apiData.currentPage ?? 1,
                    limit: apiData.limit ?? apiData.pageSize ?? limit,
                    total: apiData.total ?? 0,
                    totalPages: apiData.totalPages ?? 1
                };
            }

            // Verifica se é um envelope
            if ('succeeded' in response) {
                if (response.succeeded === false) {
                    throw new Error(response.errors?.join(', ') || 'API retornou succeeded=false');
                }

                if (response.succeeded === true && 'data' in response) {
                    return response.data || {
                        items: [],
                        page: 1,
                        limit: 10,
                        total: 0,
                        totalPages: 0
                    };
                }
            }

            return response as PermissionsApiResponse;

        } catch (error: any) {
            if (error.status === 404) {
                return {
                    items: [],
                    page: 1,
                    limit: 10,
                    total: 0,
                    totalPages: 0
                };
            }
            throw error;
        }
    }

    static async getPermissionsByTenant(tenantId: string): Promise<Permission[]> {
        const response = await this.getPermissions({
            page: 1,
            limit: 1000,
            tenantId
        });
        return response.items;
    }

    static async getPermissionById(id: string): Promise<Permission> {
        const response = await httpClient.get<Permission | ApiResponse<Permission>>(API_ENDPOINTS.PERMISSION_BY_ID(id));

        if ('succeeded' in response) {
            if (!response.succeeded || !response.data) {
                throw new Error(response.errors?.join(', ') || 'Permissão não encontrada');
            }
            return response.data;
        }

        return response as Permission;
    }

    static async createPermission(data: CreatePermissionRequest): Promise<Permission> {
        const response = await httpClient.post<Permission | ApiResponse<Permission>>(API_ENDPOINTS.PERMISSIONS, data);

        if ('succeeded' in response) {
            if (!response.succeeded || !response.data) {
                throw new Error(response.errors?.join(', ') || 'Erro ao criar permissão');
            }
            return response.data;
        }

        return response as Permission;
    }

    static async updatePermission(id: string, data: UpdatePermissionRequest): Promise<Permission> {
        const response = await httpClient.put<Permission | ApiResponse<Permission>>(API_ENDPOINTS.PERMISSION_BY_ID(id), data);

        if ('succeeded' in response) {
            if (!response.succeeded || !response.data) {
                throw new Error(response.errors?.join(', ') || 'Erro ao atualizar permissão');
            }
            return response.data;
        }

        return response as Permission;
    }

    static async deletePermission(id: string): Promise<void> {
        const response = await httpClient.delete<void | ApiResponse<void>>(API_ENDPOINTS.PERMISSION_BY_ID(id));

        if (response && typeof response === 'object' && 'succeeded' in response) {
            if (!response.succeeded) {
                throw new Error(response.errors?.join(', ') || 'Erro ao remover permissão');
            }
        }
    }
}
