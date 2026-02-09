
import 'package:fpdart/fpdart.dart';
import '../../../../features/pos/domain/models/product_model.dart';

abstract class ProductRepository {
  Future<Either<String, List<ProductModel>>> getProducts();
  Future<Either<String, ProductModel>> createProduct(Map<String, dynamic> data);
  Future<Either<String, ProductModel>> updateProduct(String id, Map<String, dynamic> data);
  Future<Either<String, void>> deleteProduct(String id);
}
