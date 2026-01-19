// Tipos baseados na API saas-authentication-api
import type { AccessGroup } from './access-group.types';

export const UserAccountStatus = {
  Active: 'Ativo',
  Inactive: 'Inativo',
  Pending: 'Pendente',
  Suspended: 'Suspenso'
} as const;

export type UserAccountStatus = typeof UserAccountStatus[keyof typeof UserAccountStatus];

export interface UserAccount {
  id: string;
  tenantId?: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  status: UserAccountStatus;
  isEmailVerified: boolean;
  createdAt: string;
  updatedAt?: string;
  lastLoginAt?: string;
  fullName: string;
  accessGroups?: AccessGroup[];
}

export interface CreateUserAccountRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  tenantId?: string;
}

export interface UpdateUserAccountRequest {
  username?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  status?: UserAccountStatus;
  isEmailVerified?: boolean;
  tenantId?: string;
  createdAt?: string;
}

// Tipos para associação de grupos a usuários
export interface AssignUserAccessGroupsRequest {
  accessGroupIds: string[];
}

export interface UserAccessGroupsResponse {
  id: string;
  name: string;
  description?: string;
  groupType: string;
  isActive: boolean;
}