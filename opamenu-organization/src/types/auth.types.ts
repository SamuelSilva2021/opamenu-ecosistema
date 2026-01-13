export interface User {
  id: string;
  email: string;
  name: string;
  roles: string[];
  tenantId?: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expiresIn: number;
  tenantStatus?: string;
  subscriptionStatus?: string;
  requiresPayment: boolean;
}

export interface LoginRequest {
  usernameOrEmail: string;
  password: string;
}

export interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  requiresPayment: boolean;
}
