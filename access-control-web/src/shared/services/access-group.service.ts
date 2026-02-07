import type {
    AccessGroup,
    CreateAccessGroupRequest,
    UpdateAccessGroupRequest,
    ApiResponse
} from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

interface GetAccessGroupsParams {
    page?: number;
    limit?: number;
    search?: string;
}

interface AccessGroupsApiResponse {
    items: AccessGroup[];
    page: number;
    limit: number;
    total: number;
    totalPages: number;
}

interface AccessGroupsDirectResponse {
    items: AccessGroup[];
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

export class AccessGroupService {
    static async getAccessGroups(params: GetAccessGroupsParams = {}): Promise<AccessGroupsApiResponse> {
        try {
            const { page = 1, limit = 10, search } = params;

            const searchParams = new URLSearchParams({
                page: page.toString(),
                limit: limit.toString(),
                ...(search && { search }),
            });

            const url = `${API_ENDPOINTS.ACCESS_GROUPS}?${searchParams}`;
            const response = await httpClient.get<AccessGroupsApiResponse | ApiResponse<AccessGroupsApiResponse> | AccessGroupsDirectResponse>(url);

            // Verifica se é o formato direto paginado
            const raw = response as any;
            if (raw && Array.isArray(raw.items)) {
                const apiData = raw as AccessGroupsDirectResponse;
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

            return response as AccessGroupsApiResponse;

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

    static async getAccessGroupById(id: string): Promise<AccessGroup> {
        const response = await httpClient.get<AccessGroup | ApiResponse<AccessGroup>>(API_ENDPOINTS.ACCESS_GROUP_BY_ID(id));

        if ('succeeded' in response) {
            if (!response.succeeded || !response.data) {
                throw new Error(response.errors?.join(', ') || 'Grupo de acesso não encontrado');
            }
            return response.data;
        }

        return response as AccessGroup;
    }

    static async createAccessGroup(data: CreateAccessGroupRequest): Promise<AccessGroup> {
        const response = await httpClient.post<AccessGroup | ApiResponse<AccessGroup>>(API_ENDPOINTS.ACCESS_GROUPS, data);

        if ('succeeded' in response) {
            if (!response.succeeded || !response.data) {
                throw new Error(response.errors?.join(', ') || 'Erro ao criar grupo de acesso');
            }
            return response.data;
        }

        return response as AccessGroup;
    }

    static async updateAccessGroup(id: string, data: UpdateAccessGroupRequest): Promise<AccessGroup> {
        const response = await httpClient.put<AccessGroup | ApiResponse<AccessGroup>>(API_ENDPOINTS.ACCESS_GROUP_BY_ID(id), data);

        if ('succeeded' in response) {
            if (!response.succeeded || !response.data) {
                throw new Error(response.errors?.join(', ') || 'Erro ao atualizar grupo de acesso');
            }
            return response.data;
        }

        return response as AccessGroup;
    }

    static async deleteAccessGroup(id: string): Promise<void> {
        const response = await httpClient.delete<void | ApiResponse<void>>(API_ENDPOINTS.ACCESS_GROUP_BY_ID(id));

        if (response && typeof response === 'object' && 'succeeded' in response) {
            if (!response.succeeded) {
                throw new Error(response.errors?.join(', ') || 'Erro ao remover grupo de acesso');
            }
        }
    }

    static async getAllAccessGroups(): Promise<AccessGroup[]> {
        const response = await this.getAccessGroups({ page: 1, limit: 1000 });
        return response.items;
    }
}
