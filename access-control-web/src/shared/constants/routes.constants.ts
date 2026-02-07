// Constantes de rotas da aplicação
export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  DASHBOARD: '/dashboard',

  // Test/Debug
  OPERATIONS_TEST: '/operations-test',

  // Users
  USERS: '/users',
  USER_CREATE: '/users/create',
  USER_EDIT: (id: string) => `/users/${id}/edit`,
  USER_DETAIL: (id: string) => `/users/${id}`,

  // Roles
  ROLES: '/roles',
  ROLE_CREATE: '/roles/create',
  ROLE_EDIT: (id: string) => `/roles/${id}/edit`,
  ROLE_DETAIL: (id: string) => `/roles/${id}`,

  // Modules
  MODULES: '/modules',
  MODULE_CREATE: '/modules/create',
  MODULE_EDIT: (id: string) => `/modules/${id}/edit`,
  MODULE_DETAIL: (id: string) => `/modules/${id}`,

  // Tenants
  TENANTS: '/tenants',
  TENANT_CREATE: '/tenants/create',
  TENANT_EDIT: (id: string) => `/tenants/${id}/edit`,
  TENANT_DETAIL: (id: string) => `/tenants/${id}`,

  // Settings
  SETTINGS: '/settings',
  PROFILE: '/profile',
} as const;