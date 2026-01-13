import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../data/repositories/products_repository.dart';
import '../../domain/models/product_model.dart';

part 'product_provider.g.dart';

@riverpod
Future<List<ProductModel>> products(Ref ref) async {
  final repository = ref.watch(productsRepositoryProvider);
  final result = await repository.getProducts();
  
  return result.fold(
    (error) => throw Exception(error),
    (products) => products,
  );
}

@riverpod
class ProductSearchQuery extends _$ProductSearchQuery {
  @override
  String build() => '';

  void setQuery(String query) => state = query;
}

@riverpod
Future<List<ProductModel>> filteredProducts(Ref ref) async {
  final products = await ref.watch(productsProvider.future);
  final query = ref.watch(productSearchQueryProvider);

  if (query.isEmpty) return products;

  return products.where((p) =>
    p.name.toLowerCase().contains(query.toLowerCase()) ||
    (p.description?.toLowerCase().contains(query.toLowerCase()) ?? false)
  ).toList();
}
