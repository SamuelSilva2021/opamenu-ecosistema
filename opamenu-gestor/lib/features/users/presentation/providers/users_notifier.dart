
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:opamenu_gestor/features/users/domain/models/user_model.dart';
import '../../data/repositories/user_repository_impl.dart';

part 'users_notifier.g.dart';

@riverpod
class UsersNotifier extends _$UsersNotifier {
  @override
  FutureOr<List<UserModel>> build() async {
    final repository = ref.watch(userRepositoryProvider);
    final result = await repository.getUsers();
    return result.fold(
      (l) => throw Exception(l),
      (r) => r,
    );
  }

  Future<void> addUser(Map<String, dynamic> data) async {
    state = const AsyncValue.loading();
    final repository = ref.read(userRepositoryProvider);
    final result = await repository.createUser(data);
    
    result.fold(
      (l) => state = AsyncValue.error(l, StackTrace.current),
      (r) => ref.invalidateSelf(),
    );
  }

  Future<void> updateUser(String id, Map<String, dynamic> data) async {
    state = const AsyncValue.loading();
    final repository = ref.read(userRepositoryProvider);
    final result = await repository.updateUser(id, data);
    
    result.fold(
      (l) => state = AsyncValue.error(l, StackTrace.current),
      (r) => ref.invalidateSelf(),
    );
  }

  Future<void> deleteUser(String id) async {
    state = const AsyncValue.loading();
    final repository = ref.read(userRepositoryProvider);
    final result = await repository.deleteUser(id);
    
    result.fold(
      (l) => state = AsyncValue.error(l, StackTrace.current),
      (r) => ref.invalidateSelf(),
    );
  }
}
