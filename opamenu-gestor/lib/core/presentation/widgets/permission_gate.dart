
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../providers/permissions_provider.dart';

class PermissionGate extends ConsumerWidget {
  final String module;
  final String? operation;
  final Widget child;
  final Widget fallback;

  const PermissionGate({
    super.key,
    required this.module,
    this.operation,
    required this.child,
    this.fallback = const SizedBox.shrink(),
  });

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    if (module.isEmpty) return child;
    
    final permissionKey = operation != null ? '$module:$operation' : module;
    final hasAccess = ref.watch(hasPermissionProvider(permissionKey));
    
    return hasAccess ? child : fallback;
  }
}
