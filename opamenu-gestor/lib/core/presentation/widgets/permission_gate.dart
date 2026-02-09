
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/presentation/providers/permissions_provider.dart';

class PermissionGate extends ConsumerWidget {
  final String module;
  final String? operation;
  final Widget child;
  final Widget? fallback;

  const PermissionGate({
    super.key,
    required this.module,
    this.operation,
    required this.child,
    this.fallback,
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final permissionsAsync = ref.watch(permissionsProvider);

    return permissionsAsync.when(
      data: (permissions) {
        final hasPermission = permissions.any((p) {
          if (operation != null) {
            return p.module == module && p.operation == operation;
          }
          return p.module == module;
        });
        
        if (hasPermission) {
          return child;
        }

        return fallback ?? const SizedBox.shrink();
      },
      loading: () => const SizedBox.shrink(),
      error: (_, __) => const SizedBox.shrink(),
    );
  }
}
