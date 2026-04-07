import '../../../../core/domain/result/result.dart';
import '../entities/product_entity.dart';
import '../repositories/products_repository.dart';

class FetchMenuProductsUseCase {
  final ProductsRepository repository;

  const FetchMenuProductsUseCase(this.repository);

  Future<Result<List<ProductEntity>>> call({required String accessToken}) {
    return repository.getMenuProducts(accessToken: accessToken);
  }
}

