
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../data/repositories/additional_repository_impl.dart';
import '../../domain/models/additional_group_model.dart';

part 'additional_notifier.g.dart';

@riverpod
class AdditionalNotifier extends _$AdditionalNotifier {
  @override
  FutureOr<List<AdditionalGroupModel>> build() async {
    final repository = ref.watch(additionalRepositoryProvider);
    final result = await repository.getAdditionalGroups();
    return result.fold(
      (error) => throw Exception(error),
      (groups) => groups,
    );
  }

  Future<void> addGroup(String name, String? description) async {
    state = const AsyncValue.loading();
    final repository = ref.read(additionalRepositoryProvider);
    final result = await repository.createAdditionalGroup({
      'name': name,
      'description': description,
      'minSelection': 0,
      'maxSelection': 1,
      'isActive': true,
    });

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (_) => ref.invalidateSelf(),
    );
  }

  Future<void> updateGroup(AdditionalGroupModel group) async {
    state = const AsyncValue.loading();
    final repository = ref.read(additionalRepositoryProvider);
    final result = await repository.updateAdditionalGroup(group.id, group.toJson());

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (_) => ref.invalidateSelf(),
    );
  }

  Future<void> deleteGroup(String id) async {
    state = const AsyncValue.loading();
    final repository = ref.read(additionalRepositoryProvider);
    final result = await repository.deleteAdditionalGroup(id);

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (_) => ref.invalidateSelf(),
    );
  }

  // --- ITEMS ---

  Future<void> addItem(String groupId, String name, double price, String? description) async {
    state = const AsyncValue.loading();
    final repository = ref.read(additionalRepositoryProvider);
    final result = await repository.createAdditional({
      'name': name,
      'price': price,
      'description': description,
      'isActive': true,
      'additionalGroupId': groupId,
    });

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (_) => ref.invalidateSelf(),
    );
  }

  Future<void> updateItem(String id, String groupId, String name, double price, String? description, bool isActive) async {
    state = const AsyncValue.loading();
    final repository = ref.read(additionalRepositoryProvider);
    final result = await repository.updateAdditional(id, {
      'name': name,
      'price': price,
      'description': description,
      'isActive': isActive,
      'additionalGroupId': groupId,
    });

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (_) => ref.invalidateSelf(),
    );
  }

  Future<void> deleteItem(String id) async {
    state = const AsyncValue.loading();
    final repository = ref.read(additionalRepositoryProvider);
    final result = await repository.deleteAdditional(id);

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (_) => ref.invalidateSelf(),
    );
  }
}
