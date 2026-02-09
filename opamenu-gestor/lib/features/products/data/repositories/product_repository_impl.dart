
import 'package:fpdart/fpdart.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../features/pos/domain/models/product_model.dart';
import '../datasources/product_remote_datasource.dart';
import '../../domain/repositories/product_repository.dart';

part 'product_repository_impl.g.dart';

@riverpod
ProductRepository productRepository(Ref ref) {
  final dataSource = ref.watch(productRemoteDataSourceProvider);
  return ProductRepositoryImpl(dataSource);
}

class ProductRepositoryImpl implements ProductRepository {
  final ProductRemoteDataSource _remoteDataSource;

  ProductRepositoryImpl(this._remoteDataSource);

  @override
  Future<Either<String, List<ProductModel>>> getProducts() async {
    try {
      final result = await _remoteDataSource.getProducts();
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, ProductModel>> createProduct(Map<String, dynamic> data) async {
    try {
      final result = await _remoteDataSource.createProduct(data);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, ProductModel>> updateProduct(String id, Map<String, dynamic> data) async {
    try {
      final result = await _remoteDataSource.updateProduct(id, data);
      return Right(result);
    } catch (e) {
      return Left(e.toString());
    }
  }

  @override
  Future<Either<String, void>> deleteProduct(String id) async {
    try {
      await _remoteDataSource.deleteProduct(id);
      return const Right(null);
    } catch (e) {
      return Left(e.toString());
    }
  }
}
