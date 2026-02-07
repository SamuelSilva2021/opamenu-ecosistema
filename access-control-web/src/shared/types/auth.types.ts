// Tipos de Autenticação baseados na API
import type { UserPermissions, SimplifiedRole } from './permission.types';

export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface LoginResponseData {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expiresIn: number;
}

export interface UserInfo {
  id: string;
  username: string;
  email: string;
  fullName: string;
  permissions: UserPermissions; // Mantido para compatibilidade
  role?: SimplifiedRole;         // Novo formato 3-níveis
  tenant: {
    id: string;
    name: string;
    slug: string;
    customDomain: string | null;
  };
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface AuthUser {
  id: string;
  email: string;
  username: string;
  fullName: string;
  role?: SimplifiedRole; // Novo formato
  tenant: {
    id: string;
    name: string;
    slug: string;
    customDomain: string | null;
  };
}

export interface AuthState {
  isAuthenticated: boolean;
  user: AuthUser | null;
  token: string | null;
  isLoading: boolean;
}