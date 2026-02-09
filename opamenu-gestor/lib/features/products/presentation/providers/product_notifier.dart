
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../data/datasources/product_remote_datasource.dart';
import '../../../pos/domain/models/product_model.dart';

part 'product_notifier.g.dart';

@riverpod
class ProductNotifier extends _$ProductNotifier {
  @override
  FutureOr<List<ProductModel>> build() async {
    final dataSource = ref.watch(productRemoteDataSourceProvider);
    return await dataSource.getProducts();
  }

  Future<void> addProduct(Map<String, dynamic> data) async {
    state = const AsyncValue.loading();
    final dataSource = ref.read(productRemoteDataSourceProvider);
    try {
      await dataSource.createProduct(data);
      ref.invalidateSelf();
    } catch (e) {
      state = AsyncValue.error(e, StackTrace.current);
    }
  }

  Future<void> updateProduct(String id, Map<String, dynamic> data) async {
    state = const AsyncValue.loading();
    final dataSource = ref.read(productRemoteDataSourceProvider);
    try {
      await dataSource.updateProduct(id, data);
      ref.invalidateSelf();
    } catch (e) {
      state = AsyncValue.error(e, StackTrace.current);
    }
  }

  Future<void> deleteProduct(String id) async {
    state = const AsyncValue.loading();
    final dataSource = ref.read(productRemoteDataSourceProvider);
    try {
      await dataSource.deleteProduct(id);
      ref.invalidateSelf();
    } catch (e) {
      state = AsyncValue.error(e, StackTrace.current);
    }
  }
}
