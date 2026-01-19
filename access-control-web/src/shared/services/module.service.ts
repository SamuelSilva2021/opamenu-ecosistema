import type { PaginatedResponse, ApiResponse } from '../types';
import type { Module, CreateModuleRequest, UpdateModuleRequest } from '../types/permission.types';
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

interface ModulesApiResponse {
  items?: Module[];
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

interface ModulesEnvelopeResponse {
  data?: ModulesApiResponse;
  succeeded?: boolean | null;
  errors?: string[];
}

export class ModuleService {
  private static readonly BASE_URL = API_ENDPOINTS.MODULES;

  static async getModules(params?: QueryParams): Promise<PaginatedResponse<Module>> {
    const response = await httpClient.get<ModulesApiResponse | ModulesEnvelopeResponse>(
      this.BASE_URL,
      { params }
    );
        
    const raw = response as ModulesApiResponse & ModulesEnvelopeResponse;
    
    let apiData: ModulesApiResponse | undefined;
    
    // Verifica se a resposta é direta (formato novo)
    if (raw && Array.isArray(raw.items)) {
      apiData = raw;
    } 
    // Verifica se a resposta é envelopada (formato antigo/padrão)
    else if (raw && raw.data && Array.isArray(raw.data.items)) {
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
      totalPages: totalPages,
      hasPreviousPage: page > 1,
      hasNextPage: page < totalPages,
    };
  }

  static async getModuleById(id: string): Promise<Module> {
    const response = await httpClient.get<Module>(`${this.BASE_URL}/${id}`);
    return response;
  }

  static async createModule(data: CreateModuleRequest): Promise<Module> {
    const response = await httpClient.post<Module | ApiResponse<Module>>(
      this.BASE_URL,
      data
    );
    
    if ('succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao criar módulo');
      }
      return response.data;
    }
    
    return response as Module;
  }

  /**
   * Atualiza um módulo existente
   */
  static async updateModule(id: string, data: UpdateModuleRequest): Promise<Module> {
    const response = await httpClient.put<Module | ApiResponse<Module>>(
      `${this.BASE_URL}/${id}`,
      data
    );
    
    if ('succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao atualizar módulo');
      }
      return response.data;
    }
    
    return response as Module;
  }

  /**
   * Exclui um módulo
   */
  static async deleteModule(id: string): Promise<void> {
    await httpClient.delete(`${this.BASE_URL}/${id}`);
  }

  /**
   * Verifica se uma chave de módulo já existe
   */
  static async checkModuleKeyExists(moduleKey: string, excludeId?: string): Promise<boolean> {
    try {
      const response = await this.getModules();
      
      const existingModule = response.data.find(module => 
        module.key?.toLowerCase() === moduleKey.toLowerCase() && // Mudança: moduleKey → key
        (!excludeId || module.id !== excludeId)
      );
      
      return !!existingModule;
    } catch (error) {
      console.error('Erro ao verificar chave do módulo:', error);
      throw error;
    }
  }

  /**
   * Alterna o status ativo/inativo de um módulo
   */
  static async toggleModuleStatus(id: string): Promise<Module> {
    const response = await httpClient.patch<Module | ApiResponse<Module>>(
      `${this.BASE_URL}/${id}/toggle-status`,
      {}
    );
    
    if ('succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao alternar status do módulo');
      }
      return response.data;
    }
    
    const directResponse = response as Module;
    if (directResponse && (directResponse.id || directResponse.key || directResponse.name)) {
      return directResponse;
    }
    
    console.error('❌ [ModuleService] Formato desconhecido:', response);
    throw new Error('Falha ao alternar status: formato de resposta desconhecido');
  }
}