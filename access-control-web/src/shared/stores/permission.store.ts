import { create } from 'zustand';
import type { UserPermissions, OperationType, SimplifiedRole } from '../types/permission.types';

interface PermissionState {
  permissions: UserPermissions | null;
  role: SimplifiedRole | null;
  isInitialized: boolean;
  setPermissions: (permissions: UserPermissions | null, role?: SimplifiedRole | null) => void;
  clearPermissions: () => void;
  hasModuleAccess: (moduleKey: string) => boolean;
  canPerformOperation: (moduleKey: string, operation: string) => boolean;
  getAccessibleModules: () => string[];
  getModuleOperations: (moduleKey: string) => string[];
}

export const usePermissionStore = create<PermissionState>((set, get) => ({
  permissions: null,
  role: null,
  isInitialized: false,

  setPermissions: (permissions, role = null) => set({
    permissions,
    role,
    isInitialized: true
  }),

  clearPermissions: () => set({ permissions: null, role: null, isInitialized: true }),

  hasModuleAccess: (moduleKey: string): boolean => {
    const { role, permissions } = get();

    // 1. Nova estrutura simplificada
    if (role && role.permissions) {
      return role.permissions.some(p => p.module === moduleKey);
    }

    // 2. Fallback estrutura antiga
    if (permissions && permissions.accessGroups) {
      return permissions.accessGroups.some(group =>
        group.roles.some(role =>
          role.modules.some(module => module.key === moduleKey)
        )
      );
    }

    return false;
  },

  canPerformOperation: (moduleKey: string, operation: string): boolean => {
    const { role, permissions } = get();
    const normalizedOp = operation.toUpperCase();

    // 1. Nova estrutura simplificada
    if (role && role.permissions) {
      const perm = role.permissions.find(p => p.module === moduleKey);
      return !!(perm && perm.actions && perm.actions.includes(normalizedOp));
    }

    // 2. Fallback estrutura antiga
    if (permissions && permissions.accessGroups) {
      return permissions.accessGroups.some(group =>
        group.roles.some(role =>
          role.modules.some(module =>
            module.key === moduleKey && module.operations.map(o => o.toUpperCase()).includes(normalizedOp)
          )
        )
      );
    }

    return false;
  },

  getAccessibleModules: (): string[] => {
    const { role, permissions } = get();
    const modules = new Set<string>();

    if (role && role.permissions) {
      role.permissions.forEach(p => modules.add(p.module));
    } else if (permissions && permissions.accessGroups) {
      permissions.accessGroups.forEach(group =>
        group.roles.forEach(role =>
          role.modules.forEach(module => modules.add(module.key))
        )
      );
    }

    return Array.from(modules);
  },

  getModuleOperations: (moduleKey: string): string[] => {
    const { role, permissions } = get();
    const operations = new Set<string>();

    if (role && role.permissions) {
      const perm = role.permissions.find(p => p.module === moduleKey);
      if (perm) perm.actions.forEach(op => operations.add(op));
    } else if (permissions && permissions.accessGroups) {
      permissions.accessGroups.forEach(group =>
        group.roles.forEach(role =>
          role.modules.forEach(module => {
            if (module.key === moduleKey) {
              module.operations.forEach(op => operations.add(op));
            }
          })
        )
      );
    }

    return Array.from(operations);
  }
}));

// Hook personalizado para usar permissões
export const usePermissions = () => {
  const {
    hasModuleAccess,
    canPerformOperation,
    getAccessibleModules,
    getModuleOperations
  } = usePermissionStore();

  return {
    hasAccess: (moduleKey: string, operation?: OperationType) => {
      if (!operation) {
        return hasModuleAccess(moduleKey);
      }
      return canPerformOperation(moduleKey, operation);
    },
    getAccessibleModules,
    getModuleOperations,
    canCreate: (moduleKey: string) => canPerformOperation(moduleKey, 'CREATE'),
    canRead: (moduleKey: string) => canPerformOperation(moduleKey, 'SELECT'),
    canUpdate: (moduleKey: string) => canPerformOperation(moduleKey, 'UPDATE'),
    canDelete: (moduleKey: string) => canPerformOperation(moduleKey, 'DELETE')
  };
};