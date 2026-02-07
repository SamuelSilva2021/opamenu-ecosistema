import type {
  Role,
  CreateRoleRequest,
  UpdateRoleRequest,
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

  // No novo modelo, as permissões são enviadas diretamente no CreateRoleRequest/UpdateRoleRequest
  // Métodos de associação granular (assignPermissionsToRole) foram removidos para simplificação

  // Métodos de associação de AccessGroups foram removidos pois o modelo agora é Usuário -> Role direto

  static async toggleRoleStatus(role: Role): Promise<Role> {
    return this.updateRole(role.id, {
      isActive: !role.isActive
    });
  }
}