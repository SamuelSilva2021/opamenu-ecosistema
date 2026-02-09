
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:opamenu_gestor/features/products/data/repositories/category_repository.dart';
import 'package:opamenu_gestor/features/products/domain/models/category_model.dart';

part 'category_notifier.g.dart';

@riverpod
class CategoryNotifier extends _$CategoryNotifier {
  @override
  Future<List<CategoryModel>> build() async {
    final repository = ref.watch(categoryRepositoryProvider);
    final result = await repository.getCategories();
    return result.fold(
      (error) => throw Exception(error),
      (categories) => categories,
    );
  }

  Future<void> addCategory(String name, String? description) async {
    state = const AsyncValue.loading();
    final repository = ref.read(categoryRepositoryProvider);
    final result = await repository.createCategory({
      'name': name,
      'description': description,
      'displayOrder': 0,
      'isActive': true,
    });

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (category) {
        ref.invalidateSelf();
      },
    );
  }

  Future<void> updateCategory(CategoryModel category) async {
    state = const AsyncValue.loading();
    final repository = ref.read(categoryRepositoryProvider);
    final result = await repository.updateCategory(category.id, category.toJson());

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (updated) {
        ref.invalidateSelf();
      },
    );
  }

  Future<void> deleteCategory(String id) async {
    state = const AsyncValue.loading();
    final repository = ref.read(categoryRepositoryProvider);
    final result = await repository.deleteCategory(id);

    result.fold(
      (error) => state = AsyncValue.error(error, StackTrace.current),
      (_) {
        ref.invalidateSelf();
      },
    );
  }
}
