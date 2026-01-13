import { useOperationPermissions } from '../useOperationPermissions';
import { ModuleKey } from '../../types/permission.types';

/**
 * Hook específico para operações do módulo ACCESS_GROUP
 */
export const useAccessGroupOperations = () => {
  return useOperationPermissions(ModuleKey.ACCESS_GROUP);
};

/**
 * Hook específico para operações do módulo USER_MODULE
 */
export const useUserOperations = () => {
  return useOperationPermissions(ModuleKey.USER_MODULE);
};

/**
 * Hook específico para operações do módulo GROUP_TYPE
 */
export const useGroupTypeOperations = () => {
  return useOperationPermissions(ModuleKey.GROUP_TYPE);
};

/**
 * Hook específico para operações do módulo MODULES
 */
export const useModulesOperations = () => {
  return useOperationPermissions(ModuleKey.MODULES);
};

/**
 * Hook específico para operações do módulo OPERATION_MODULE
 */
export const useOperationModuleOperations = () => {
  return useOperationPermissions(ModuleKey.OPERATION_MODULE);
};

/**
 * Hook específico para operações do módulo ROLE_MODULE
 */
export const useRoleOperations = () => {
  return useOperationPermissions(ModuleKey.ROLE_MODULE);
};

/**
 * Hook específico para operações do módulo PERMISSION_MODULE
 */
export const usePermissionOperations = () => {
  return useOperationPermissions(ModuleKey.PERMISSION_MODULE);
};