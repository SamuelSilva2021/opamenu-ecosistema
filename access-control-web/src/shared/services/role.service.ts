import type { 
  Role, 
  CreateRoleRequest, 
  UpdateRoleRequest,
  Permission,
  AccessGroup,
  ApiResponse
} from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

interface GetRolesParams {
  page?: number;
  limit?: number;
  search?: string;
}

interface RolesApiResponse {
  items: Role[];
  page: number;
  limit: number;
  total: number;
  totalPages: number;
}

interface RolesDirectResponse {
  items: Role[];
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

export class RoleService {
  static async getRoles(params: GetRolesParams = {}): Promise<RolesApiResponse> {
    try {
      const { page = 1, limit = 10, search } = params;
      
      const searchParams = new URLSearchParams({
        page: page.toString(),
        limit: limit.toString(),
        ...(search && { search }),
      });
      
      const url = `${API_ENDPOINTS.ROLES}?${searchParams}`;
      const response = await httpClient.get<RolesApiResponse | ApiResponse<RolesApiResponse> | RolesDirectResponse>(url);
      
      // Verifica se é o formato direto paginado
      const raw = response as any;
      if (raw && Array.isArray(raw.items)) {
        const apiData = raw as RolesDirectResponse;
        return {
          items: apiData.items,
          page: apiData.page ?? apiData.currentPage ?? 1,
          limit: apiData.limit ?? apiData.pageSize ?? limit,
          total: apiData.total ?? 0,
          totalPages: apiData.totalPages ?? 1
        };
      }

      // Verifica se é um envelope (com ou sem data, mas com indicador de sucesso)
      if ('succeeded' in response) {
        // Se for erro explícito
        if (response.succeeded === false) {
          throw new Error(response.errors?.join(', ') || 'API retornou succeeded=false');
        }
        
        // Se for sucesso envelopado
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
      
      return response as RolesApiResponse;
      
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

  static async getAllRoles(): Promise<Role[]> {
    const response = await this.getRoles({ page: 1, limit: 1000 });
    return response.items || [];
  }

  static async getRoleById(id: string): Promise<Role> {
    const response = await httpClient.get<Role | ApiResponse<Role>>(API_ENDPOINTS.ROLE_BY_ID(id));
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Role não encontrado');
      }
      return response.data;
    }
    
    return response as Role;
  }

  static async createRole(role: CreateRoleRequest): Promise<Role> {
    const response = await httpClient.post<Role | ApiResponse<Role>>(API_ENDPOINTS.ROLES, role);
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao criar role');
      }
      return response.data;
    }
    
    return response as Role;
  }

  static async updateRole(id: string, role: UpdateRoleRequest): Promise<Role> {
    const response = await httpClient.put<Role | ApiResponse<Role>>(API_ENDPOINTS.ROLE_BY_ID(id), role);
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao atualizar role');
      }
      return response.data;
    }
    
    return response as Role;
  }

  static async deleteRole(id: string): Promise<void> {
    const response = await httpClient.delete<void | ApiResponse<void>>(API_ENDPOINTS.ROLE_BY_ID(id));
    
    if (response && typeof response === 'object' && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao remover role');
      }
    }
  }

  // ===== PERMISSIONS =====

  static async getPermissionsByRole(roleId: string): Promise<Permission[]> {
    const response = await httpClient.get<Permission[] | ApiResponse<Permission[]>>(`${API_ENDPOINTS.ROLES}/${roleId}/permissions`);
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao buscar permissões do role');
      }
      return response.data;
    }
    
    if (Array.isArray(response)) {
      return response;
    }
    return [];
  }

  static async assignPermissionsToRole(roleId: string, permissionIds: string[]): Promise<void> {
    const response = await httpClient.post<boolean | ApiResponse<boolean>>(
      `${API_ENDPOINTS.ROLES}/${roleId}/permissions`, 
      permissionIds
    );
    
    if (typeof response === 'object' && response !== null && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao atribuir permissões ao role');
      }
    }
  }

  static async removePermissionsFromRole(roleId: string, permissionIds: string[]): Promise<void> {
    const response = await httpClient.delete<boolean | ApiResponse<boolean>>(
      `${API_ENDPOINTS.ROLES}/${roleId}/permissions`,
      { data: permissionIds }
    );
    
    if (typeof response === 'object' && response !== null && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao remover permissões do role');
      }
    }
  }

  // ===== ACCESS GROUPS =====

  static async getAccessGroupsByRole(roleId: string): Promise<AccessGroup[]> {
    const response = await httpClient.get<AccessGroup[] | ApiResponse<AccessGroup[]>>(`${API_ENDPOINTS.ROLES}/${roleId}/access-groups`);
    
    if ('succeeded' in response) {
      if (!response.succeeded || !response.data) {
        throw new Error(response.errors?.join(', ') || 'Erro ao buscar grupos de acesso do role');
      }
      return response.data;
    }
    
    if (Array.isArray(response)) {
      return response;
    }
    return [];
  }

  static async assignAccessGroupsToRole(roleId: string, accessGroupIds: string[]): Promise<void> {
    const response = await httpClient.post<boolean | ApiResponse<boolean>>(
      `${API_ENDPOINTS.ROLES}/${roleId}/access-groups`, 
      accessGroupIds
    );
    
    if (typeof response === 'object' && response !== null && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao atribuir grupos de acesso ao role');
      }
    }
  }

  static async removeAccessGroupsFromRole(roleId: string, accessGroupIds: string[]): Promise<void> {
    const response = await httpClient.delete<boolean | ApiResponse<boolean>>(
      `${API_ENDPOINTS.ROLES}/${roleId}/access-groups`,
      { data: accessGroupIds }
    );
    
    if (typeof response === 'object' && response !== null && 'succeeded' in response) {
      if (!response.succeeded) {
        throw new Error(response.errors?.join(', ') || 'Erro ao remover grupos de acesso do role');
      }
    }
  }

  static async toggleRoleStatus(role: Role): Promise<Role> {
    return this.updateRole(role.id, { 
      isActive: !role.isActive 
    });
  }
}