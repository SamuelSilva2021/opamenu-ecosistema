import type { UserInfo } from "@/features/auth/types";

export function hasPermission(user: UserInfo | null, moduleKey: string, operation: string = "READ"): boolean {
  if (!user) return false;

  const normalizedOp = operation.toUpperCase();

  // 1. Tenta usar a nova estrutura simplificada (RoleId direto no UserInfo)
  if (user.role && user.role.permissions) {
    const perm = user.role.permissions.find(p => p.module === moduleKey);
    return !!(perm && perm.actions && perm.actions.includes(normalizedOp));
  }

  // 2. Fallback para a estrutura antiga complexa (enquanto o backend migra ou para compatibilidade)
  if (user.permissions && user.permissions.accessGroups) {
    for (const group of user.permissions.accessGroups) {
      if (!group.roles) continue;
      for (const role of group.roles) {
        if (!role.modules) continue;
        const module = role.modules.find(m => m.key === moduleKey);
        if (module && module.operations && module.operations.map(o => o.toUpperCase()).includes(normalizedOp)) {
          return true;
        }
      }
    }
  }

  return false;
}
