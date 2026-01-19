import type { UserInfo } from "@/features/auth/types";

export function hasPermission(user: UserInfo | null, moduleKey: string, operation: string = "READ"): boolean {
  if (!user || !user.permissions || !user.permissions.accessGroups) {
    return false;
  }

  // Iterate through all access groups
  for (const group of user.permissions.accessGroups) {
    if (!group.roles) continue;
    // Iterate through all roles in the group
    for (const role of group.roles) {
        if (!role.modules) continue;
        // Check if any module matches the key and has the operation
        const module = role.modules.find(m => m.key === moduleKey);
        if (module && module.operations.includes(operation)) {
            return true;
        }
    }
  }

  return false;
}
