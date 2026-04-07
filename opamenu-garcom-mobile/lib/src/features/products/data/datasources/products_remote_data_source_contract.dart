import '../../../../core/domain/result/result.dart';
import '../models/product_model.dart';

abstract class ProductsRemoteDataSourceContract {
  Future<Result<List<ProductModel>>> getMenuProducts({required String accessToken});
}

