import type { ReactNode } from "react";
import { usePermission } from "@/hooks/usePermission";

interface PermissionGateProps {
  module: string;
  operation: string;
  children: ReactNode;
  fallback?: ReactNode;
}

export function PermissionGate({ module, operation, children, fallback = null }: PermissionGateProps) {
  const { can } = usePermission();

  if (can(module, operation)) {
    return <>{children}</>;
  }

  return <>{fallback}</>;
}
