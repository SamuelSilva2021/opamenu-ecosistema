import { useMemo } from 'react';
import { usePermissions } from '../stores/permission.store';
import type { OperationType, OperationPermissions } from '../types/operation.types';

/**
 * Hook para verificação de operações específicas em um módulo
 * Fornece uma interface padronizada para controle de operações CRUD
 * 
 * NOTA: As permissões são sempre carregadas antes da renderização da UI
 * através do AppInitializationProvider, então não precisamos de loading state aqui
 */
export const useOperationPermissions = (moduleKey: string): OperationPermissions => {
  const { hasAccess } = usePermissions();

  return useMemo(() => {
    const canRead = hasAccess(moduleKey, 'SELECT');
    const canCreate = hasAccess(moduleKey, 'CREATE');
    const canUpdate = hasAccess(moduleKey, 'UPDATE');
    const canDelete = hasAccess(moduleKey, 'DELETE');

    const hasAnyOperation = (operations: OperationType[]): boolean => {
      return operations.some(operation => hasAccess(moduleKey, operation));
    };

    const hasAllOperations = (operations: OperationType[]): boolean => {
      return operations.every(operation => hasAccess(moduleKey, operation));
    };

    return {
      canRead,
      canCreate,
      canUpdate,
      canDelete,
      hasAnyOperation,
      hasAllOperations,
    };
  }, [moduleKey, hasAccess]);
};