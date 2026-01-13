import { api } from '../lib/api';
import type { LoginResponse, LoginRequest, User } from '../types/auth.types';
import { jwtDecode } from 'jwt-decode';

export const authService = {
  login: async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await api.post('/auth/login', data);
    return response.data?.data || response.data; 
  },

  logout: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  },

  getUserFromToken: (token: string): User | null => {
    try {
      const decoded: any = jwtDecode(token);
      // Ajuste o mapeamento conforme as claims do seu JWT
      // Exemplo comum: "sub" -> id, "email" -> email, "role" -> roles
      // Se as roles vierem como array ou string, trate aqui
      
      return {
        id: decoded.sub || decoded.nameid,
        email: decoded.email,
        name: decoded.unique_name || decoded.name || decoded.email,
        roles: Array.isArray(decoded.role) ? decoded.role : [decoded.role],
        tenantId: decoded.tenantId || decoded.TenantId
      };
    } catch (error) {
      console.error("Erro ao decodificar token", error);
      return null;
    }
  }
};
