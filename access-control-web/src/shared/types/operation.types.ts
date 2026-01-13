import type { OperationType } from './permission.types';

export type { OperationType };

export interface OperationPermissions {
  canRead: boolean;      // SELECT
  canCreate: boolean;    // CREATE  
  canUpdate: boolean;    // UPDATE
  canDelete: boolean;    // DELETE
  hasAnyOperation: (operations: OperationType[]) => boolean;
  hasAllOperations: (operations: OperationType[]) => boolean;
}

export interface OperationGuardProps {
  module: string;
  operations: OperationType[];
  children: React.ReactNode;
  fallback?: React.ReactNode;
}

export interface ConditionalRenderProps {
  module: string;
  renderIf: (permissions: OperationPermissions) => boolean;
  children: React.ReactNode;
  fallback?: React.ReactNode;
}