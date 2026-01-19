import type { PaginatedResponse, ApiResponse } from '../types';
import type { Operation, CreateOperationRequest, UpdateOperationRequest } from '../types/permission.types';
import { httpClient } from '../utils/http-client';
import { API_ENDPOINTS } from '../constants';

// Interface para ErrorDTO da API
interface ErrorDTO {
  code: string;
  property?: string;
  message: string;
  details?: string[];
}

// Helper function para tratar erros da API
const getErrorMessage = (errors: ErrorDTO[] | string[] | undefined): string => {
  if (!errors || !Array.isArray(errors) || errors.length === 0) {
    return 'Erro desconhecido na API';
  }
  return errors.map(error => typeof error === 'string' ? error : error.message).join(', ');
};

interface QueryParams {
  page?: number;
  limit?: number;
  search?: string;
  isActive?: boolean;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

// Interface específica para a resposta da API de Operations quando lista (diferente estrutura)
interface OperationsApiResponse {
  succeeded: boolean;
  successResult: any;
  errors: ErrorDTO[];
  headers: Record<string, any>;
  data: Operation[]; // A API retorna um array direto
  requestUrl: string | null;
  requestBody: string | null;
  rawRequestBody: string | null;
  exception: string | null;
}

interface OperationsDirectResponse {
  items: Operation[];
  page: number;
  limit: number;
  total: number;
  totalPages: number;
  succeeded?: boolean | null;
  code?: number;
  currentPage?: number;
  pageSize?: number;
}

/**
 * Serviço para gerenciar operações
 * Centraliza todas as chamadas à API de operações
 */
export class OperationService {
  private static readonly BASE_URL = API_ENDPOINTS.OPERATIONS;

  /**
   * Lista todas as operações
   */
  static async getOperations(params?: QueryParams): Promise<PaginatedResponse<Operation>> {
    const response = await httpClient.get<Operation[] | OperationsApiResponse | ApiResponse<Operation[]> | OperationsDirectResponse>(
      this.BASE_URL,
      { params }
    );
        
    // Verificação para novo formato direto (paginado)
    const raw = response as any;
    if (raw && Array.isArray(raw.items)) {
      const apiData = raw as OperationsDirectResponse;
      
      const page = apiData.page ?? apiData.currentPage ?? params?.page ?? 1;
      const limit = apiData.limit ?? apiData.pageSize ?? params?.limit ?? 10;
      const totalPages = apiData.totalPages ?? 1;
      const total = apiData.total ?? 0;

      return {
        data: apiData.items,
        totalCount: total,
        pageNumber: page,
        pageSize: limit,
        totalPages: totalPages,
        hasPreviousPage: page > 1,
        hasNextPage: page < totalPages,
      };
    }

    let operations: Operation[];
    
    // Verifica se é um array direto
    if (Array.isArray(response)) {
      operations = response;
    }
    // Verifica se é um envelope com 'data' (ApiResponse ou OperationsApiResponse)
    else if ('data' in response) {
      const envelope = response as OperationsApiResponse | ApiResponse<Operation[]>;
      
      if (envelope.succeeded === false) {
        throw new Error(`Erro na API: ${getErrorMessage(envelope.errors)}`);
      }
      
      if (!envelope.data) {
        operations = [];
      } else {
        operations = Array.isArray(envelope.data) ? envelope.data : [];
      }
    } 
    // Fallback para outros casos
    else {
      operations = [];
    }
        
    // Simula paginação no frontend para respostas não paginadas (fallback)
    const pageSize = params?.limit || 10;
    const currentPage = params?.page || 1;
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const paginatedOperations = operations.slice(startIndex, endIndex);
    
    const totalCount = operations.length;
    const totalPages = Math.ceil(totalCount / pageSize);
    
    return {
      data: paginatedOperations,
      totalCount: totalCount,
      pageNumber: currentPage,
      pageSize: pageSize,
      totalPages: totalPages,
      hasPreviousPage: currentPage > 1,
      hasNextPage: currentPage < totalPages,
    };
  }

  /**
   * Busca uma operação específica por ID
   */
  static async getOperationById(id: string): Promise<Operation> {
    const response = await httpClient.get<Operation | ApiResponse<Operation>>(`${this.BASE_URL}/${id}`);
    
    if ('succeeded' in response) {
      if (!response.succeeded) {
        const errorMessage = Array.isArray(response.errors) && response.errors.length > 0
          ? response.errors.join(', ')
          : 'Erro desconhecido na API';
        throw new Error(`Erro na API: ${errorMessage}`);
      }
      return response.data;
    }
    
    return response as Operation;
  }

  /**
   * Cria uma nova operação
   */
  static async createOperation(data: CreateOperationRequest): Promise<Operation> {
    try {
      console.log('🔄 OperationService: Enviando dados para criação:', data);
      
      const response = await httpClient.post<Operation | ApiResponse<Operation>>(this.BASE_URL, data);
      
      console.log('🔍 OperationService: Resposta da API:', response);
      
      if ('succeeded' in response) {
        if (!response.succeeded) {
          const errorMessage = Array.isArray(response.errors) && response.errors.length > 0
            ? response.errors.join(', ')
            : 'Erro desconhecido na API';
            
          console.error('❌ OperationService: API retornou erro:', {
            errors: response.errors,
            errorMessage
          });
          throw new Error(`Erro na API: ${errorMessage}`);
        }
        
        if (!response.data) {
          console.error('❌ OperationService: API não retornou dados válidos');
          throw new Error('API não retornou dados válidos para a operação criada');
        }
        
        console.log('✅ OperationService: Operação criada com sucesso:', response.data);
        return response.data;
      }
      
      // Resposta direta
      const operation = response as Operation;
      console.log('✅ OperationService: Operação criada com sucesso (direto):', operation);
      return operation;
      
    } catch (error: any) {
      console.error('❌ OperationService: Erro ao criar operação:', error);
      
      // Se for um erro de rede ou HTTP, vamos capturar mais detalhes
      if (error.response) {
        console.error('❌ OperationService: Detalhes do erro HTTP:', {
          status: error.response.status,
          statusText: error.response.statusText,
          data: error.response.data
        });
        
        // Tenta extrair a mensagem de erro mais específica
        if (error.response.data?.errors?.length > 0) {
          const specificError = Array.isArray(error.response.data.errors)
            ? error.response.data.errors.join(', ')
            : 'Erro na resposta da API';
          throw new Error(`Erro na API: ${specificError}`);
        }
        
        throw new Error(`Erro HTTP ${error.response.status}: ${error.response.statusText}`);
      }
      
      // Re-lança o erro original se não for um erro HTTP
      throw error;
    }
  }

  /**
   * Atualiza uma operação existente
   */
  static async updateOperation(id: string, data: UpdateOperationRequest): Promise<Operation> {
    const response = await httpClient.put<Operation | ApiResponse<Operation>>(`${this.BASE_URL}/${id}`, data);
    
    if ('succeeded' in response) {
      if (!response.succeeded) {
        const errorMessage = Array.isArray(response.errors) && response.errors.length > 0
          ? response.errors.join(', ')
          : 'Erro desconhecido na API';
        throw new Error(`Erro na API: ${errorMessage}`);
      }
      return response.data;
    }
    
    return response as Operation;
  }

  /**
   * Exclui uma operação
   */
  static async deleteOperation(id: string): Promise<void> {
    const response = await httpClient.delete<void | ApiResponse<void>>(`${this.BASE_URL}/${id}`);
    
    if (typeof response === 'object' && response !== null && 'succeeded' in response) {
      if (!response.succeeded) {
        const errorMessage = Array.isArray(response.errors) && response.errors.length > 0
          ? response.errors.join(', ')
          : 'Erro desconhecido na API';
        throw new Error(`Erro na API: ${errorMessage}`);
      }
    }
  }

  /**
   * Verifica se um valor já existe
   */
  static async checkValueExists(value: string, excludeId?: string): Promise<boolean> {
    try {
      const response = await httpClient.get<{ exists: boolean } | ApiResponse<{ exists: boolean }>>(
        `${this.BASE_URL}/check-value`,
        { 
          params: { 
            value,
            excludeId 
          } 
        }
      );
      
      if ('succeeded' in response) {
        return response.data?.exists || false;
      }
      
      return response.exists || false;
    } catch (error) {
      console.warn('Erro ao verificar valor da operação:', error);
      return false;
    }
  }
}