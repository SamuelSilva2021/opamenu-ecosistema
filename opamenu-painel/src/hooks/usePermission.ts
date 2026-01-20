import { useAuthStore } from "@/store/auth.store";
import { hasPermission } from "@/lib/permissions";

export function usePermission() {
  const user = useAuthStore((state) => state.user);

  const can = (module: string, operation: string) => {
    return hasPermission(user, module, operation);
  };

  return { can };
}
