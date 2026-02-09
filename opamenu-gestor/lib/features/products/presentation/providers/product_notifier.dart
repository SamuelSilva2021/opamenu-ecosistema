
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../data/repositories/product_repository_impl.dart';
import '../../../pos/domain/models/product_model.dart';

part 'product_notifier.g.dart';

@riverpod
class ProductNotifier extends _$ProductNotifier {
  @override
  FutureOr<List<ProductModel>> build() async {
    final repository = ref.watch(productRepositoryProvider);
    final result = await repository.getProducts();
    return result.fold(
      (l) => throw Exception(l),
      (r) => r,
    );
  }

  Future<void> addProduct(Map<String, dynamic> data) async {
    state = const AsyncValue.loading();
    final repository = ref.read(productRepositoryProvider);
    final result = await repository.createProduct(data);
    
    result.fold(
      (l) => state = AsyncValue.error(l, StackTrace.current),
      (r) => ref.invalidateSelf(),
    );
  }

  Future<void> updateProduct(String id, Map<String, dynamic> data) async {
    state = const AsyncValue.loading();
    final repository = ref.read(productRepositoryProvider);
    final result = await repository.updateProduct(id, data);
    
    result.fold(
      (l) => state = AsyncValue.error(l, StackTrace.current),
      (r) => ref.invalidateSelf(),
    );
  }

  Future<void> deleteProduct(String id) async {
    state = const AsyncValue.loading();
    final repository = ref.read(productRepositoryProvider);
    final result = await repository.deleteProduct(id);
    
    result.fold(
      (l) => state = AsyncValue.error(l, StackTrace.current),
      (r) => ref.invalidateSelf(),
    );
  }
}
