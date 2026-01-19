import { UserAccountStatus } from '../types';
import type { 
  UserAccount, 
  CreateUserAccountRequest, 
  UpdateUserAccountRequest,
  ApiResponse
} from '../types';
import type { AccessGroup } from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

interface GetUsersParams {
  page?: number;
  limit?: number;
  search?: string;
}

interface UsersApiResponse {
  items: UserAccount[];
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

interface ForgotPasswordRequest {
  email: string;
}

interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
}

/**
 * Serviço para gerenciar Usuários (UserAccounts)
 * Centraliza todas as operações CRUD relacionadas aos usuários do sistema
 * 
 * Endpoints da API:
 * - GET /api/users - Lista usuários paginados
 * - GET /api/users/active - Lista usuários ativos
 * - GET /api/users/{id} - Busca usuário por ID
 * - POST /api/users - Cria novo usuário
 * - PUT /api/users/{id} - Atualiza usuário
 * - DELETE /api/users/{id} - Remove usuário
 * - POST /api/users/forgot-password - Esqueci senha
 * - POST /api/users/reset-password - Reset senha
 */
export class UserService {
  // ========== USERS CRUD ==========
  
  /**
   * Busca usuários com paginação
   * API: GET /api/users?page=1&limit=10
   */
  static async getUsers(params: GetUsersParams = {}): Promise<UsersApiResponse> {
    try {
      const { page = 1, limit = 10, search } = params;
            
      const searchParams = new URLSearchParams({
        page: page.toString(),
        limit: limit.toString(),
        ...(search && { search }),
      });
      
      const url = `${API_ENDPOINTS.USERS}?${searchParams}`;
      const response = await httpClient.get<UsersApiResponse | ApiResponse<UsersApiResponse>>(url);
            
      // 1. Prioridade: Verificação para resposta direta (UsersApiResponse)
      const raw = response as any;
      if (raw && Array.isArray(raw.items)) {
        const apiData = raw as UsersApiResponse;
        return {
           items: apiData.items,
           page: apiData.page ?? apiData.currentPage ?? page,
           limit: apiData.limit ?? apiData.pageSize ?? limit,
           total: apiData.total ?? 0,
           totalPages: apiData.totalPages ?? 1,
           succeeded: apiData.succeeded
        };
      }

      // 2. Verificação para resposta envelopada (ApiResponse)
      if ('succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          console.error('❌ UserService: API failed:', errorMsg);
          throw new Error(errorMsg);
        }
        
        if (response.succeeded === true && 'data' in response) {
          const envelope = response as ApiResponse<UsersApiResponse>;
          return envelope.data;
        }
        
        // Se succeeded é null mas não tem items (já passou pelo check acima),
        // pode ser um caso estranho, mas vamos retornar vazio se não tiver data.
        if ('data' in response) {
            return (response as ApiResponse<UsersApiResponse>).data;
        }
      }

      console.warn('⚠️ UserService: Formato de resposta desconhecido, retornando vazio');
      return {
        items: [],
        page: 1,
        limit: 10,
        total: 0,
        totalPages: 0
      };
      
    } catch (error: any) {
      console.error('💥 UserService: Erro na requisição:', error);
      
      // Se for 404, retorna resultado vazio (normal para sistema novo)
      if (error.status === 404) {
        return {
          items: [],
          page: 1,
          limit: 10,
          total: 0,
          totalPages: 0
        };
      }
      
      throw new Error(error.message || 'Erro ao buscar usuários');
    }
  }

  /**
   * Busca usuários ativos
   * API: GET /api/users/active
   */
  static async getActiveUsers(): Promise<UserAccount[]> {
    try {      
      const response = await httpClient.get<UserAccount[] | ApiResponse<UserAccount[]>>(`${API_ENDPOINTS.USERS}/active`);
      
      if ('succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          console.error('❌ UserService: API failed:', errorMsg);
          throw new Error(errorMsg);
        }
        return response.data || [];
      }

      if (Array.isArray(response)) {
        return response;
      }

      return [];
      
    } catch (error: any) {
      console.error('💥 UserService: Erro ao buscar usuários ativos:', error);
      
      if (error.status === 404) {
        return [];
      }
      
      throw new Error(error.message || 'Erro ao buscar usuários ativos');
    }
  }

  /**
   * Busca usuário por ID
   * API: GET /api/users/{id}
   */
  static async getUserById(id: string): Promise<UserAccount> {
    try {      
      const response = await httpClient.get<UserAccount | ApiResponse<UserAccount>>(`${API_ENDPOINTS.USERS}/${id}`);
      
      if ('succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          throw new Error(errorMsg);
        }
        
        if (!response.data) {
          throw new Error('Usuário não encontrado');
        }
        
        return response.data;
      }
      
      return response as UserAccount;
      
    } catch (error: any) {
      console.error('❌ UserService: Erro ao buscar usuário:', error);
      throw new Error(error.message || 'Erro ao buscar usuário');
    }
  }

  /**
   * Cria novo usuário
   * API: POST /api/users
   */
  static async createUser(userData: CreateUserAccountRequest): Promise<UserAccount> {
    try {      
      const response = await httpClient.post<UserAccount | ApiResponse<UserAccount>>(API_ENDPOINTS.USERS, userData);
      
      // Verificação para resposta envelopada (ApiResponse)
      if ('succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          console.error('❌ UserService: API failed:', errorMsg);
          throw new Error(errorMsg);
        }
        
        if (!response.data) {
          throw new Error('Erro ao criar usuário - resposta inválida');
        }
        
        return response.data;
      }
      
      // Verificação para resposta direta
      if (response && (response as any).id) {
        return response as UserAccount;
      }

      throw new Error('Erro ao criar usuário - formato de resposta desconhecido');
      
    } catch (error: any) {
      console.error('💥 UserService: Erro ao criar usuário:', error);
      throw new Error(error.message || 'Erro ao criar usuário');
    }
  }

  /**
   * Atualiza usuário existente
   * API: PUT /api/users/{id}
   */
  static async updateUser(id: string, userData: UpdateUserAccountRequest): Promise<UserAccount> {
    try {
      
      // Usando <any> e cast para lidar com retorno direto ou envelopado
      const response = await httpClient.put<UserAccount | ApiResponse<UserAccount>>(`${API_ENDPOINTS.USERS}/${id}`, userData);
      
      if ('succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          console.error('❌ UserService: API failed:', errorMsg);
          throw new Error(errorMsg);
        }
        
        if (!response.data) {
          throw new Error('Erro ao atualizar usuário - resposta inválida');
        }
        return response.data;
      }
      
      return response as UserAccount;
      
    } catch (error: any) {
      console.error('💥 UserService: Erro ao atualizar usuário:', error);
      throw new Error(error.message || 'Erro ao atualizar usuário');
    }
  }

  /**
   * Remove usuário
   * API: DELETE /api/users/{id}
   */
  static async deleteUser(id: string): Promise<void> {
    try {      
      const response = await httpClient.delete<boolean | ApiResponse<boolean>>(`${API_ENDPOINTS.USERS}/${id}`);
      
      if (typeof response === 'object' && response !== null && 'succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          console.error('❌ UserService: API failed:', errorMsg);
          throw new Error(errorMsg);
        }
      }
            
    } catch (error: any) {
      console.error('💥 UserService: Erro ao remover usuário:', error);
      throw new Error(error.message || 'Erro ao remover usuário');
    }
  }

  // ========== PASSWORD MANAGEMENT ==========

  /**
   * Inicia fluxo de esqueci a senha
   * API: POST /api/users/forgot-password
   */
  static async forgotPassword(email: string): Promise<void> {
    try {      
      const requestData: ForgotPasswordRequest = { email };
      const response = await httpClient.post<boolean | ApiResponse<boolean>>(`${API_ENDPOINTS.USERS}/forgot-password`, requestData);
      
      if (typeof response === 'object' && response !== null && 'succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          console.error('❌ UserService: API failed:', errorMsg);
          throw new Error(errorMsg);
        }
      }
            
    } catch (error: any) {
      console.error('💥 UserService: Erro ao enviar email de recuperação:', error);
      throw new Error(error.message || 'Erro ao enviar email de recuperação');
    }
  }

  /**
   * Reseta senha usando token
   * API: POST /api/users/reset-password
   */
  static async resetPassword(email: string, token: string, newPassword: string): Promise<void> {
    try {      
      const requestData: ResetPasswordRequest = { email, token, newPassword };
      const response = await httpClient.post<boolean | ApiResponse<boolean>>(`${API_ENDPOINTS.USERS}/reset-password`, requestData);
      
      if (typeof response === 'object' && response !== null && 'succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          console.error('❌ UserService: API failed:', errorMsg);
          throw new Error(errorMsg);
        }
      }
            
    } catch (error: any) {
      console.error('💥 UserService: Erro ao resetar senha:', error);
      throw new Error(error.message || 'Erro ao resetar senha');
    }
  }

  // ========== UTILITY METHODS ==========

  /**
   * Toggle status do usuário (ativo/inativo)
   */
  static async toggleUserStatus(user: UserAccount): Promise<UserAccount> {
    const newStatus = user.status === UserAccountStatus.Active ? UserAccountStatus.Inactive : UserAccountStatus.Active;
    
    return this.updateUser(user.id, {
      status: newStatus
    });
  }

  /**
   * Valida se email já está em uso
   */
  static async validateEmail(email: string, excludeUserId?: string): Promise<boolean> {
    try {
      const users = await this.getUsers({ limit: 100 });
      
      return !users.items.some(user => 
        user.email.toLowerCase() === email.toLowerCase() && 
        user.id !== excludeUserId
      );
    } catch (error) {
      console.warn('UserService: Erro ao validar email, assumindo válido:', error);
      return true;
    }
  }

  // ========== USER ACCESS GROUPS ==========

  /**
   * Lista grupos de acesso de um usuário
   */
  static async getUserAccessGroups(userId: string): Promise<AccessGroup[]> {
    try {
      
      const response = await httpClient.get<AccessGroup[] | ApiResponse<AccessGroup[]>>(`${API_ENDPOINTS.USERS}/${userId}/access-groups`);
      
      if ('succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          throw new Error(errorMsg);
        }
        return response.data || [];
      }
      
      if (Array.isArray(response)) {
        return response;
      }

      return [];
      
    } catch (error: any) {
      console.error('❌ UserService: Erro ao buscar grupos do usuário:', error);
      
      if (error.status === 404) {
        return [];
      }
      
      throw new Error(error.message || 'Erro ao buscar grupos do usuário');
    }
  }

  /**
   * Atribui grupos de acesso a um usuário
   */
  static async assignUserAccessGroups(userId: string, accessGroupIds: string[]): Promise<boolean> {
    try {
      
      const response = await httpClient.post<boolean | ApiResponse<boolean>>(`${API_ENDPOINTS.USERS}/${userId}/access-groups`, {
        accessGroupIds
      });

      if (typeof response === 'object' && response !== null && 'succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          throw new Error(errorMsg);
        }
        return response.data || true;
      }

      return (response as unknown as boolean) || true;
      
    } catch (error: any) {
      console.error('❌ UserService: Erro ao atribuir grupos:', error);
      throw new Error(error.message || 'Erro ao atribuir grupos ao usuário');
    }
  }

  /**
   * Remove um grupo de acesso específico de um usuário
   */
  static async revokeUserAccessGroup(userId: string, groupId: string): Promise<boolean> {
    try {
      
      const response = await httpClient.delete<boolean | ApiResponse<boolean>>(`${API_ENDPOINTS.USERS}/${userId}/access-groups/${groupId}`);

      if (typeof response === 'object' && response !== null && 'succeeded' in response) {
        if (response.succeeded === false) {
          const errorMsg = response.errors?.join(', ') || 'API retornou succeeded=false';
          throw new Error(errorMsg);
        }
        return response.data || true;
      }

      return (response as unknown as boolean) || true;
      
    } catch (error: any) {
      console.error('❌ UserService: Erro ao remover grupo:', error);
      throw new Error(error.message || 'Erro ao remover grupo do usuário');
    }
  }
}
