import 'package:fpdart/fpdart.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/models/product_model.dart';
import '../datasources/products_remote_datasource.dart';

part 'products_repository.g.dart';

@riverpod
ProductsRepository productsRepository(Ref ref) {
  return ProductsRepository(ref.watch(productsRemoteDataSourceProvider));
}

class ProductsRepository {
  final ProductsRemoteDataSource _dataSource;

  ProductsRepository(this._dataSource);

  Future<Either<String, List<ProductModel>>> getProducts() async {
    try {
      final products = await _dataSource.getProducts();
      return Right(products);
    } catch (e) {
      return Left(e.toString());
    }
  }
}
