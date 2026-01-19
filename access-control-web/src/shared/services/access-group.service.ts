import type { AccessGroup, CreateAccessGroupRequest, UpdateAccessGroupRequest, AccessGroupApiResponse } from '../types';
import { httpClient } from '../utils/http-client';
import { API_ENDPOINTS } from '../constants';
import type { ApiResponse } from '../types';

interface GetAccessGroupsParams {
  page?: number;
  limit?: number;
  search?: string;
}

interface AccessGroupDirectResponse<T> {
  items?: T[];
  page?: number;
  limit?: number;
  total?: number;
  totalPages?: number;
  currentPage?: number;
  pageSize?: number;
  succeeded?: boolean | null;
  code?: number;
  errors?: string[];
}

interface AccessGroupEnvelopeResponse<T> {
  data?: AccessGroupDirectResponse<T>;
  succeeded?: boolean | null;
  errors?: string[];
}

/**
 * Serviço para gerenciar Access Groups
 * Centraliza todas as operações CRUD relacionadas aos grupos de acesso
 */
export class AccessGroupService {

  static async getAccessGroups(params: GetAccessGroupsParams = {}): Promise<AccessGroupApiResponse<AccessGroup>> {
    try {
      const { page = 1, limit = 10, search } = params;

      const searchParams = new URLSearchParams({
        page: page.toString(),
        limit: limit.toString(),
        ...(search && { search }),
      });

      const url = `${API_ENDPOINTS.ACCESS_GROUPS}?${searchParams}`;
      const response = await httpClient.get<AccessGroupDirectResponse<AccessGroup> | AccessGroupEnvelopeResponse<AccessGroup>>(url);
      const raw = response as AccessGroupDirectResponse<AccessGroup> & AccessGroupEnvelopeResponse<AccessGroup>;

      if (raw && Array.isArray(raw.items)) {
        const normalized: AccessGroupApiResponse<AccessGroup> = {
          items: raw.items ?? [],
          page: raw.page ?? raw.currentPage ?? 1,
          limit: raw.limit ?? raw.pageSize ?? limit,
          total: raw.total ?? 0,
          totalPages: raw.totalPages ?? 0,
        };

        return normalized;
      }

      if (raw && raw.data && Array.isArray(raw.data.items)) {
        if (raw.succeeded === false) {
          const errorMsg = raw.errors?.join(', ') || 'API retornou succeeded=false';
          console.error('❌ AccessGroupService: API failed:', errorMsg);
          throw new Error(errorMsg);
        }

        const normalized: AccessGroupApiResponse<AccessGroup> = {
          items: raw.data.items ?? [],
          page: raw.data.page ?? 1,
          limit: raw.data.limit ?? limit,
          total: raw.data.total ?? 0,
          totalPages: raw.data.totalPages ?? 0,
        };

        return normalized;
      }

      console.error('❌ AccessGroupService: Formato de resposta inesperado:', raw);
      throw new Error('Formato de resposta inesperado da API de grupos de acesso');

    } catch (error: unknown) {
      console.error('💥 AccessGroupService: Erro na requisição:', error);
      
      // Se for 404, retorna resultado vazio (normal para sistema novo)
      if (
        typeof error === 'object' &&
        error !== null &&
        'status' in error &&
        (error as { status?: number }).status === 404
      ) {
        return {
          items: [],
          page: 1,
          limit: 10,
          total: 0,
          totalPages: 0
        };
      }
      
      // Para outros erros, propaga
      throw error;
    }
  }
  
  /**
   * Método de compatibilidade - busca todos os grupos (sem paginação)
   * @deprecated Use getAccessGroups() com paginação
   */
  static async getAllAccessGroups(): Promise<AccessGroup[]> {
    const response = await this.getAccessGroups({ page: 1, limit: 1000 });
    return response.items || [];
  }

  /**
   * Busca um grupo de acesso por ID
   */
  static async getAccessGroupById(id: string): Promise<AccessGroup> {
    const response = await httpClient.get<AccessGroup>(API_ENDPOINTS.ACCESS_GROUP_BY_ID(id));

    return response;
  }

  /**
   * Cria um novo grupo de acesso
   */
  static async createAccessGroup(data: CreateAccessGroupRequest): Promise<AccessGroup> {
    const response = await httpClient.post<AccessGroup | ApiResponse<AccessGroup>>(API_ENDPOINTS.ACCESS_GROUPS, data);
    
    if ('succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao criar grupo de acesso');
      }
      return response.data!;
    }
    
    return response as AccessGroup;
  }

  /**
   * Atualiza um grupo de acesso existente
   */
  static async updateAccessGroup(id: string, data: UpdateAccessGroupRequest): Promise<AccessGroup> {
    const response = await httpClient.put<AccessGroup | ApiResponse<AccessGroup>>(API_ENDPOINTS.ACCESS_GROUP_BY_ID(id), data);
    
    if ('succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao atualizar grupo de acesso');
      }
      return response.data!;
    }
    
    return response as AccessGroup;
  }

  /**
   * Remove um grupo de acesso
   */
  static async deleteAccessGroup(id: string): Promise<void> {
    const response = await httpClient.delete<boolean | ApiResponse<boolean>>(API_ENDPOINTS.ACCESS_GROUP_BY_ID(id));
    
    if (typeof response === 'object' && response !== null && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao remover grupo de acesso');
      }
    }
  }
}
