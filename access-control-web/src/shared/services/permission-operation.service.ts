import type { PaginatedResponse, ApiResponse } from '../types';
import type { 
  PermissionOperation, 
  CreatePermissionOperationRequest, 
  UpdatePermissionOperationRequest,
  PermissionOperationBulkRequest 
} from '../types/permission.types';
import { httpClient } from '../utils/http-client';
import { API_ENDPOINTS } from '../constants';

interface QueryParams {
  page?: number;
  limit?: number;
  search?: string;
  isActive?: boolean;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  permissionId?: string;
  operationId?: string;
}

// Interface específica para a resposta da API de Permission Operations
interface PermissionOperationsApiResponse {
  items: PermissionOperation[];
  page: number;
  limit: number;
  total: number;
  totalPages: number;
}

/**
 * Serviço para gerenciar relações Permissão-Operação
 * Centraliza todas as chamadas à API de permission operations
 */
export class PermissionOperationService {
  private static readonly BASE_URL = API_ENDPOINTS.PERMISSION_OPERATIONS;

  /**
   * Lista todas as relações permissão-operação com paginação
   */
  static async getPermissionOperations(params?: QueryParams): Promise<PaginatedResponse<PermissionOperation>> {
    const response = await httpClient.get<PermissionOperationsApiResponse | ApiResponse<PermissionOperationsApiResponse>>(
      this.BASE_URL,
      { params }
    );
    
    
    let apiData: PermissionOperationsApiResponse;
    
    if ('succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao buscar permission operations');
      }
      apiData = response.data;
    } else {
      apiData = response as PermissionOperationsApiResponse;
    }
    
    
    // Verifica se apiData tem a propriedade items
    if (!apiData || !apiData.items) {
      // Se não tiver items, retorna vazio para evitar quebrar a tela
      return {
        data: [],
        totalCount: 0,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 0,
        hasPreviousPage: false,
        hasNextPage: false
      };
    }
    
    return {
      data: apiData.items,
      totalCount: apiData.total,
      pageNumber: apiData.page,
      pageSize: apiData.limit,
      totalPages: apiData.totalPages,
      hasPreviousPage: apiData.page > 1,
      hasNextPage: apiData.page < apiData.totalPages
    };
  }

  /**
   * Busca relação por ID
   */
  static async getPermissionOperationById(id: string): Promise<PermissionOperation> {
    const response = await httpClient.get<PermissionOperation | ApiResponse<PermissionOperation>>(`${this.BASE_URL}/${id}`);
    if ('succeeded' in response) return response.data;
    return response as PermissionOperation;
  }

  /**
   * Busca relações por ID da permissão
   */
  static async getByPermissionId(permissionId: string): Promise<PermissionOperation[]> {
    const response = await httpClient.get<PermissionOperation[] | ApiResponse<PermissionOperation[]>>(`${this.BASE_URL}/permission/${permissionId}`);
    if ('succeeded' in response) return response.data;
    return response as PermissionOperation[];
  }

  /**
   * Busca relações por ID da operação
   */
  static async getByOperationId(operationId: string): Promise<PermissionOperation[]> {
    const response = await httpClient.get<PermissionOperation[] | ApiResponse<PermissionOperation[]>>(`${this.BASE_URL}/operation/${operationId}`);
    if ('succeeded' in response) return response.data;
    return response as PermissionOperation[];
  }

  /**
   * Busca uma relação específica entre permissão e operação
   */
  static async getByPermissionAndOperation(permissionId: string, operationId: string): Promise<PermissionOperation> {
    const response = await httpClient.get<PermissionOperation | ApiResponse<PermissionOperation>>(
      `${this.BASE_URL}/permission/${permissionId}/operation/${operationId}`
    );
    if ('succeeded' in response) return response.data;
    return response as PermissionOperation;
  }

  /**
   * Cria uma nova relação permissão-operação
   */
  static async createPermissionOperation(permissionOperation: CreatePermissionOperationRequest): Promise<PermissionOperation> {
    const response = await httpClient.post<PermissionOperation | ApiResponse<PermissionOperation>>(this.BASE_URL, permissionOperation);
    if ('succeeded' in response) return response.data;
    return response as PermissionOperation;
  }

  /**
   * Cria múltiplas relações permissão-operação (bulk)
   */
  static async createPermissionOperationsBulk(bulkRequest: PermissionOperationBulkRequest): Promise<PermissionOperation[]> {
    const response = await httpClient.post<PermissionOperation[] | ApiResponse<PermissionOperation[]>>(`${this.BASE_URL}/bulk`, bulkRequest);
    if ('succeeded' in response) return response.data;
    return response as PermissionOperation[];
  }

  /**
   * Atualiza uma relação permissão-operação existente
   */
  static async updatePermissionOperation(id: string, permissionOperation: UpdatePermissionOperationRequest): Promise<PermissionOperation> {
    const response = await httpClient.put<PermissionOperation | ApiResponse<PermissionOperation>>(`${this.BASE_URL}/${id}`, permissionOperation);
    if ('succeeded' in response) return response.data;
    return response as PermissionOperation;
  }

  /**
   * Remove uma relação permissão-operação (soft delete)
   */
  static async deletePermissionOperation(id: string): Promise<boolean> {
    await httpClient.delete(`${this.BASE_URL}/${id}`);
    return true;
  }

  /**
   * Remove todas as relações de uma permissão (soft delete)
   */
  static async deleteAllByPermissionId(permissionId: string): Promise<boolean> {
    await httpClient.delete(`${this.BASE_URL}/permission/${permissionId}`);
    return true;
  }

  /**
   * Remove relações específicas de uma permissão (soft delete)
   */
  static async deleteByPermissionAndOperations(permissionId: string, operationIds: string[]): Promise<boolean> {
    await httpClient.delete(`${this.BASE_URL}/permission/${permissionId}/operations`, {
      data: { operationIds }
    });
    return true;
  }

  /**
   * Alterna o status de uma relação permissão-operação
   */
  static async togglePermissionOperationStatus(id: string): Promise<PermissionOperation> {
    const response = await httpClient.patch<PermissionOperation | ApiResponse<PermissionOperation>>(`${this.BASE_URL}/${id}/toggle-status`);
    if ('succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao alternar status');
      }
      return response.data;
    }
    return response as PermissionOperation;
  }
}
