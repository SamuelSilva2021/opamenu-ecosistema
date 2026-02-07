export interface LoginRequest {
  usernameOrEmail: string;
  password?: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  expiresIn: number;
  tenantStatus?: string;
  subscriptionStatus?: string;
  requiresPayment?: boolean;
}

export interface ErrorDTO {
  code: string;
  message: string;
  property?: string;
  details?: string[];
}

export interface AuthResponse {
  data: LoginResponse;
  succeeded: boolean;
  message?: string;
  errors?: ErrorDTO[];
}

export interface RegisterTenantRequest {
  companyName: string;
  document?: string;
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface PlanDto {
  id: string;
  name: string;
  description: string;
  price: number;
  billingCycle: string;
  features?: string[];
  isActive: boolean;
}

export interface ActivateTrialRequest {
  planId: string;
}

// Permissions Types
export interface ModuleBasicDTO {
  id: string;
  key: string;
  operations: string[];
}

export interface RolesBasicDTO {
  id: string;
  code: string;
  modules: ModuleBasicDTO[];
}

// Nova estrutura simplificada
export interface SimplifiedPermission {
  module: string;
  actions: string[];
}

export interface SimplifiedRole {
  id: string;
  name: string;
  permissions: SimplifiedPermission[];
}

export interface RolesBasicDTO {
  id: string;
  code: string;
  modules: ModuleBasicDTO[];
}

export interface AccessGroupBasicDTO {
  id: string;
  code: string;
  roles: RolesBasicDTO[];
}

export interface UserPermissionsDTO {
  userId: string;
  accessGroups: AccessGroupBasicDTO[];
}

export interface TenantInfoDTO {
  id: string;
  name: string;
  slug: string;
  customDomain?: string;
}

export interface UserInfo {
  id: string;
  username: string;
  email: string;
  fullName: string;
  permissions: UserPermissionsDTO; // Mantido para compatibilidade
  role?: SimplifiedRole; // Novo formato simplificado
  tenant?: TenantInfoDTO;
}

export interface PermissionsResponse {
  data: UserInfo;
  succeeded: boolean;
  message?: string;
  errors?: ErrorDTO[];
}
