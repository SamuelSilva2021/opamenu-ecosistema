import type { SimplifiedPermission } from './permission.types';

// Tipos de Roles (Papéis)
export interface Role {
  id: string;
  name: string;
  description?: string;
  code?: string;
  tenantId?: string;
  applicationId?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
  permissions?: SimplifiedPermission[];
}

export interface CreateRoleRequest {
  name: string;
  description: string;
  code?: string;
  tenantId?: string;
  applicationId?: string;
  permissions?: SimplifiedPermission[];
  isActive?: boolean;
}

export interface UpdateRoleRequest {
  name?: string;
  description?: string;
  code?: string;
  isActive?: boolean;
  tenantId?: string;
  applicationId?: string;
  permissions?: SimplifiedPermission[];
}

// Tipos para associação de grupos de acesso a roles
export interface AssignRoleAccessGroupsRequest {
  accessGroupIds: string[];
}

export interface RoleAccessGroupsResponse {
  id: string;
  name: string;
  description?: string;
  groupType: string;
  isActive: boolean;
}

// Tipos para associação de permissões a roles
export interface AssignRolePermissionsRequest {
  permissionIds: string[];
}

export interface RolePermissionsResponse {
  id: string;
  name: string;
  description?: string;
  code?: string;
  moduleId?: string;
  moduleName?: string;
  isActive: boolean;
  operations?: {
    id: string;
    name: string;
    value: string;
  }[];
}