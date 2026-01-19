import type { PaginatedResponse, ApiResponse } from '../types';
import type { GroupType, CreateGroupTypeRequest, UpdateGroupTypeRequest } from '../types/access-group.types';
import { httpClient } from '../utils/http-client';
import { API_ENDPOINTS } from '../constants';

interface QueryParams {
  page?: number;
  limit?: number;
  search?: string;
  isActive?: boolean;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

// Interface específica para a resposta da API de Group Types (formato direto)
interface GroupTypesApiResponse {
  items?: GroupType[];
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

// Formato envelopado (quando a API usa wrapper com data/succeeded/errors)
interface GroupTypesEnvelopeResponse {
  data?: GroupTypesApiResponse;
  succeeded?: boolean | null;
  errors?: string[];
}

export class GroupTypeService {
  private static readonly BASE_URL = API_ENDPOINTS.GROUP_TYPES;

  /**
   * Lista todos os tipos de grupos com paginação
   */
  static async getGroupTypes(params?: QueryParams): Promise<PaginatedResponse<GroupType>> {
    const response = await httpClient.get<GroupTypesApiResponse | GroupTypesEnvelopeResponse>(
      this.BASE_URL,
      { params }
    );

    const raw = response as GroupTypesApiResponse & GroupTypesEnvelopeResponse;

    let apiData: GroupTypesApiResponse | undefined;

    if (raw && Array.isArray(raw.items)) {
      apiData = raw;
    } else if (raw && raw.data && Array.isArray(raw.data.items)) {
      if (raw.succeeded === false) {
        const errorMsg = raw.errors?.join(', ') || 'API retornou succeeded=false';
        throw new Error(errorMsg);
      }
      apiData = raw.data;
    }

    if (!apiData || !apiData.items) {
      throw new Error('Resposta da API inválida: propriedade items não encontrada');
    }

    const page = apiData.page ?? apiData.currentPage ?? params?.page ?? 1;
    const limit = apiData.limit ?? apiData.pageSize ?? params?.limit ?? 10;
    const totalPages = apiData.totalPages ?? 1;
    const total = apiData.total ?? 0;

    return {
      data: apiData.items,
      totalCount: total,
      pageNumber: page,
      pageSize: limit,
      totalPages,
      hasPreviousPage: page > 1,
      hasNextPage: page < totalPages,
    };
  }

  /**
   * Busca um tipo de grupo específico por ID
   */
  static async getGroupTypeById(id: string): Promise<GroupType> {
    const response = await httpClient.get<GroupType>(`${this.BASE_URL}/${id}`);

    return response;
  }

  /**
   * Cria um novo tipo de grupo
   */
  static async createGroupType(data: CreateGroupTypeRequest): Promise<GroupType> {
    const response = await httpClient.post<GroupType | ApiResponse<GroupType>>(
      this.BASE_URL,
      data
    );
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao criar tipo de grupo');
      }
      return response.data;
    }
    
    return response as GroupType;
  }

  /**
   * Atualiza um tipo de grupo existente
   */
  static async updateGroupType(id: string, data: UpdateGroupTypeRequest): Promise<GroupType> {
    const response = await httpClient.put<GroupType | ApiResponse<GroupType>>(
      `${this.BASE_URL}/${id}`,
      data
    );

    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao atualizar tipo de grupo');
      }
      return response.data;
    }

    return response as GroupType;
  }

  /**
   * Exclui um tipo de grupo
   */
  static async deleteGroupType(id: string): Promise<void> {
    const response = await httpClient.delete<void | ApiResponse<void>>(`${this.BASE_URL}/${id}`);
    
    if (response && typeof response === 'object' && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao excluir tipo de grupo');
      }
    }
  }

  /**
   * Verifica se um código já existe
   */
  static async checkCodeExists(code: string, excludeId?: string): Promise<boolean> {
    try {
      const params: QueryParams = { search: code };
      const response = await this.getGroupTypes(params);
      
      const existingGroupType = response.data.find(gt => 
        gt.code.toLowerCase() === code.toLowerCase() && 
        (!excludeId || gt.id !== excludeId)
      );
      
      return !!existingGroupType;
    } catch (error) {
      console.error('Erro ao verificar código:', error);
      throw error;
    }
  }

  /**
   * Alterna o status ativo/inativo de um tipo de grupo
   */
  static async toggleGroupTypeStatus(id: string): Promise<GroupType> {
    const response = await httpClient.patch<GroupType | ApiResponse<GroupType>>(`${this.BASE_URL}/${id}/toggle-status`);

    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao alternar status do tipo de grupo');
      }
      return response.data;
    }

    return response as GroupType;
  }
}
