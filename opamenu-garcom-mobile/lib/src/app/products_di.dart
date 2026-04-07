import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/data/http/api_http_client_provider.dart';
import '../features/products/data/datasources/products_remote_data_source.dart';
import '../features/products/data/datasources/products_remote_data_source_contract.dart';
import '../features/products/data/repositories/products_repository_impl.dart';
import '../features/products/domain/repositories/products_repository.dart';
import '../features/products/domain/usecases/fetch_menu_products_usecase.dart';
import 'app_environment_provider.dart';

class ProductsDi {
  static final Provider<ProductsRemoteDataSourceContract> remoteDataSourceProvider = Provider(
    (ref) {
      final client = ref.watch(ApiHttpClientProvider.provider);
      final environment = ref.watch(AppEnvironmentProvider.provider);
      return ProductsRemoteDataSource(client: client, environment: environment);
    },
  );

  static final Provider<ProductsRepository> repositoryProvider = Provider((ref) {
    final remoteDataSource = ref.watch(remoteDataSourceProvider);
    return ProductsRepositoryImpl(remoteDataSource);
  });

  static final Provider<FetchMenuProductsUseCase> fetchMenuProductsUseCaseProvider = Provider(
    (ref) {
      final repository = ref.watch(repositoryProvider);
      return FetchMenuProductsUseCase(repository);
    },
  );
}

