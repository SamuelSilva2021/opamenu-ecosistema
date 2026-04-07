import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../domain/entities/product_entity.dart';
import '../../domain/repositories/products_repository.dart';
import '../datasources/products_remote_data_source_contract.dart';
import '../models/product_model.dart';

class ProductsRepositoryImpl implements ProductsRepository {
  final ProductsRemoteDataSourceContract remoteDataSource;

  const ProductsRepositoryImpl(this.remoteDataSource);

  @override
  Future<Result<List<ProductEntity>>> getMenuProducts({required String accessToken}) async {
    final result = await remoteDataSource.getMenuProducts(accessToken: accessToken);
    if (result is FailureResult<List<ProductModel>>) {
      return FailureResult(result.failure);
    }

    final models = (result as SuccessResult<List<ProductModel>>).value;
    final entities = models.map((e) => e.toEntity()).toList(growable: false);
    return SuccessResult<List<ProductEntity>>(entities);
  }
}

