import React from 'react';
import { useOperationPermissions } from '../../hooks/useOperationPermissions';
import type { ConditionalRenderProps } from '../../types/operation.types';

/**
 * Componente para renderização condicional mais complexa baseada
 * em uma função que avalia as permissões de operação
 */
export const ConditionalRender: React.FC<ConditionalRenderProps> = ({
  module,
  renderIf,
  children,
  fallback = null,
}) => {
  const permissions = useOperationPermissions(module);

  if (!renderIf(permissions)) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
};