import '../../../../core/domain/result/result.dart';
import '../entities/product_entity.dart';

abstract class ProductsRepository {
  Future<Result<List<ProductEntity>>> getMenuProducts({required String accessToken});
}

