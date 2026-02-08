import { config } from '../config';

// Constantes da API baseadas na configuração do ambiente
export const API_BASE_URL = config.api.baseUrl;

export const API_ENDPOINTS = {
  // Auth
  LOGIN: '/auth/login-access-control',
  REFRESH: '/auth/refresh',
  LOGOUT: '/auth/logout',
  VALIDATE: '/auth/validate',
  ME: '/auth/me',
  USER_PERMISSIONS: '/auth/permissions',
  
  // Users
  USERS: '/users',
  USER_BY_ID: (id: string) => `/users/${id}`,
  
  // Tenants
  TENANTS: '/tenants',
  TENANT_BY_ID: (id: string) => `/tenants/${id}`,
  TENANT_BY_SLUG: (slug: string) => `/tenants/slug/${slug}`,
  
  // Access Groups
  ACCESS_GROUPS: '/access-group',
  ACCESS_GROUP_BY_ID: (id: string) => `/access-group/${id}`,
  GROUP_TYPES: '/access-group/group-types',
  GROUP_TYPE_BY_ID: (id: string) => `/access-group/group-types/${id}`,
  
  // Roles
  ROLES: '/roles',
  ROLE_BY_ID: (id: string) => `/roles/${id}`,
  
  // Permissions
  PERMISSIONS: '/permissions',
  PERMISSION_BY_ID: (id: string) => `/permissions/${id}`,
  
  // Modules
  MODULES: '/modules',
  MODULE_BY_ID: (id: string) => `/modules/${id}`,
  
  // Applications
  APPLICATIONS: '/applications',
  APPLICATION_BY_ID: (id: string) => `/applications/${id}`,

  // Operations
  OPERATIONS: '/operation',
  OPERATION_BY_ID: (id: string) => `/operation/${id}`,

  // Permission Operations
  PERMISSION_OPERATIONS: '/permission-operations',

  // Subscriptions
  SUBSCRIPTIONS: '/subscription',
  SUBSCRIPTION_BY_TENANT: (tenantId: string) => `/subscription/tenant/${tenantId}`,
} as const;

export const HTTP_STATUS = {
  OK: 200,
  CREATED: 201,
  NO_CONTENT: 204,
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  INTERNAL_SERVER_ERROR: 500,
} as const;