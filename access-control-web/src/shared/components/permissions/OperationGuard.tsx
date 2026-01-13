import React from 'react';
import { useOperationPermissions } from '../../hooks/useOperationPermissions';
import type { OperationGuardProps } from '../../types/operation.types';

/**
 * Componente que renderiza seus filhos apenas se o usuário tiver
 * as operações necessárias no módulo especificado
 */
export const OperationGuard: React.FC<OperationGuardProps> = ({
  module,
  operations,
  children,
  fallback = null,
}) => {
  const { hasAllOperations } = useOperationPermissions(module);

  if (!hasAllOperations(operations)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};