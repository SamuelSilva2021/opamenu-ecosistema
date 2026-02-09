
import 'dart:convert';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../features/auth/domain/models/permission_model.dart';

part 'permissions_provider.g.dart';

@riverpod
Future<List<PermissionModel>> permissions(Ref ref) async {
  const storage = FlutterSecureStorage();
  final permissionsJson = await storage.read(key: 'user_permissions');
  
  if (permissionsJson == null) {
    return [];
  }

  final List<dynamic> decoded = jsonDecode(permissionsJson);
  return decoded.map((p) => PermissionModel.fromString(p.toString())).toList();
}

@riverpod
bool hasPermission(Ref ref, String permissionKey) {
  final permissionsAsync = ref.watch(permissionsProvider);
  return permissionsAsync.maybeWhen(
    data: (permissions) => permissions.any((p) => p.key == permissionKey),
    orElse: () => false,
  );
}
