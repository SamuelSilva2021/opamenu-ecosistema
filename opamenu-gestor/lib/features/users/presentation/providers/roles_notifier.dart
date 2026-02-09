
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:opamenu_gestor/features/users/domain/models/role_model.dart';
import '../../data/repositories/user_repository_impl.dart';

part 'roles_notifier.g.dart';

@riverpod
Future<List<RoleModel>> roles(Ref ref) async {
  final repository = ref.watch(userRepositoryProvider);
  final result = await repository.getRoles();
  return result.fold(
    (l) => throw Exception(l),
    (r) => r,
  );
}
