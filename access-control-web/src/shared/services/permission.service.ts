import type { PaginatedResponse, ApiResponse } from '../types';
import type { Permission, CreatePermissionRequest, UpdatePermissionRequest } from '../types/permission.types';
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
const getErrorMessage = (errors: ErrorDTO[] | undefined): string => {
  if (!errors || !Array.isArray(errors) || errors.length === 0) {
    return 'Erro desconhecido na API';
  }
  return errors.map(error => error.message).join(', ');
};

interface QueryParams {
  page?: number;
  limit?: number;
  search?: string;
  isActive?: boolean;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  moduleId?: string;
  roleId?: string;
  tenantId?: string;
}

// Interface específica para a resposta da API de Permissions
interface PermissionsApiResponse {
  succeeded: boolean;
  successResult: any;
  errors: ErrorDTO[];
  headers: Record<string, any>;
  data: Permission[];
  requestUrl: string | null;
  requestBody: string | null;
  rawRequestBody: string | null;
  exception: string | null;
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
}

/**
 * Serviço para gerenciar permissões
 * Centraliza todas as chamadas à API de permissões
 */
export class PermissionService {
  private static readonly BASE_URL = API_ENDPOINTS.PERMISSIONS;

  /**
   * Lista todas as permissões com paginação
   */
  static async getPermissions(params?: QueryParams): Promise<PaginatedResponse<Permission>> {    
    const response = await httpClient.get<Permission[] | PermissionsApiResponse | PermissionsDirectResponse>(
      this.BASE_URL,
      { params }
    );
    
    // Verificação para novo formato direto
    const raw = response as any;
    if (raw && Array.isArray(raw.items)) {
      const apiData = raw as PermissionsDirectResponse;
      return {
        data: apiData.items,
        totalCount: apiData.total ?? 0,
        pageNumber: apiData.page ?? apiData.currentPage ?? 1,
        pageSize: apiData.limit ?? apiData.pageSize ?? 10,
        totalPages: apiData.totalPages ?? 1,
        hasPreviousPage: (apiData.page ?? apiData.currentPage ?? 1) > 1,
        hasNextPage: (apiData.page ?? apiData.currentPage ?? 1) < (apiData.totalPages ?? 1)
      };
    }
    
    let permissions: Permission[];
    
    // Verifica se a resposta é um array direto ou um ResponseDTO
    if (Array.isArray(response)) {
      permissions = response;
    } else {
      const apiData = response as PermissionsApiResponse;
      
      // Verifica se a operação foi bem-sucedida
      if (!apiData.succeeded) {
        throw new Error(`Erro na API: ${getErrorMessage(apiData.errors)}`);
      }
      
      // Verifica se apiData tem a propriedade data
      if (!apiData.data || !Array.isArray(apiData.data)) {
        throw new Error('Resposta da API inválida: propriedade data não encontrada ou não é array');
      }
      
      permissions = apiData.data;
    }
    
    
    // Aplicar filtros no frontend se necessário
    let filteredPermissions = permissions;
    
    // Filtro por busca
    if (params?.search) {
      const searchLower = params.search.toLowerCase();
      filteredPermissions = filteredPermissions.filter(permission =>
        permission.name.toLowerCase().includes(searchLower) ||
        (permission.description && permission.description.toLowerCase().includes(searchLower)) ||
        (permission.moduleName && permission.moduleName.toLowerCase().includes(searchLower))
      );
    }
    
    // Filtro por status
    if (params?.isActive !== undefined) {
      filteredPermissions = filteredPermissions.filter(permission => 
        permission.isActive === params.isActive
      );
    }
    
    // Filtro por módulo
    if (params?.moduleId) {
      filteredPermissions = filteredPermissions.filter(permission => 
        permission.moduleId === params.moduleId
      );
    }
    
    // Filtro por role
    if (params?.roleId) {
      filteredPermissions = filteredPermissions.filter(permission => 
        permission.roleId === params.roleId
      );
    }
    
    // Ordenação
    const sortBy = params?.sortBy || 'name';
    const sortOrder = params?.sortOrder || 'asc';
    
    filteredPermissions.sort((a, b) => {
      let aValue: any = a[sortBy as keyof Permission];
      let bValue: any = b[sortBy as keyof Permission];
      
      // Tratamento especial para datas
      if (sortBy === 'createdAt' || sortBy === 'updatedAt') {
        aValue = new Date(aValue);
        bValue = new Date(bValue);
      }
      
      // Tratamento para strings
      if (typeof aValue === 'string' && typeof bValue === 'string') {
        aValue = aValue.toLowerCase();
        bValue = bValue.toLowerCase();
      }
      
      let comparison = 0;
      if (aValue > bValue) comparison = 1;
      if (aValue < bValue) comparison = -1;
      
      return sortOrder === 'desc' ? -comparison : comparison;
    });
    
    // Aplicar paginação no frontend
    const pageSize = params?.limit || 10;
    const currentPage = params?.page || 1;
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const paginatedPermissions = filteredPermissions.slice(startIndex, endIndex);
    
    const totalCount = filteredPermissions.length;
    const totalPages = Math.ceil(totalCount / pageSize);
    
    return {
      data: paginatedPermissions,
      totalCount: totalCount,
      pageNumber: currentPage,
      pageSize: pageSize,
      totalPages: totalPages,
      hasPreviousPage: currentPage > 1,
      hasNextPage: currentPage < totalPages,
    };
  }

  /**
   * Busca permissão por ID
   */
  static async getPermissionById(id: string): Promise<Permission> {
    const response = await httpClient.get<Permission | ApiResponse<Permission>>(`${this.BASE_URL}/${id}`);
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao buscar permissão');
      }
      return response.data;
    }
    
    return response as Permission;
  }

  /**
   * Busca permissões por módulo
   */
  static async getPermissionsByModule(moduleId: string): Promise<Permission[]> {
    const response = await httpClient.get<Permission[] | ApiResponse<Permission[]>>(`${this.BASE_URL}/module/${moduleId}`);
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao buscar permissões por módulo');
      }
      return response.data;
    }
    
    return response as Permission[];
  }

  /**
   * Busca permissões por role
   */
  static async getPermissionsByRole(roleId: string): Promise<Permission[]> {
    const response = await httpClient.get<Permission[] | ApiResponse<Permission[]>>(`${this.BASE_URL}/role/${roleId}`);
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao buscar permissões por role');
      }
      return response.data;
    }
    
    return response as Permission[];
  }

  /**
   * Busca permissões por tenant
   */
  static async getPermissionsByTenant(tenantId: string): Promise<Permission[]> {
    const response = await this.getPermissions({ tenantId, limit: 1000 });
    return response.data;
  }

  /**
   * Cria uma nova permissão
   */
  static async createPermission(permission: CreatePermissionRequest): Promise<Permission> {
    const response = await httpClient.post<Permission | ApiResponse<Permission>>(this.BASE_URL, permission);
    
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao criar permissão');
      }
      return response.data;
    }
    
    return response as Permission;
  }

  /**
   * Atualiza uma permissão existente
   */
  static async updatePermission(id: string, permission: UpdatePermissionRequest): Promise<Permission> {
    const response = await httpClient.put<Permission | ApiResponse<Permission>>(`${this.BASE_URL}/${id}`, permission);
    
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao atualizar permissão');
      }
      return response.data;
    }
    
    return response as Permission;
  }

  /**
   * Remove uma permissão (soft delete)
   */
  static async deletePermission(id: string): Promise<boolean> {
    const response = await httpClient.delete<boolean | ApiResponse<boolean>>(`${this.BASE_URL}/${id}`);
    
    
    if (typeof response === 'object' && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao remover permissão');
      }
      return true;
    }
    
    return true;
  }

  /**
   * Alterna o status de uma permissão
   */
  static async togglePermissionStatus(id: string): Promise<Permission> {
    const response = await httpClient.patch<Permission | ApiResponse<Permission>>(`${this.BASE_URL}/${id}/toggle-status`);
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao alternar status da permissão');
      }
      return response.data;
    }
    
    return response as Permission;
  }
}
