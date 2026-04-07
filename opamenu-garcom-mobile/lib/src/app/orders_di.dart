import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/data/http/api_http_client_provider.dart';
import '../features/orders/data/datasources/orders_remote_data_source.dart';
import '../features/orders/data/datasources/orders_remote_data_source_contract.dart';
import '../features/orders/data/repositories/orders_repository_impl.dart';
import '../features/orders/domain/repositories/orders_repository.dart';
import '../features/orders/domain/usecases/create_table_order_usecase.dart';
import 'app_environment_provider.dart';

class OrdersDi {
  static final Provider<OrdersRemoteDataSourceContract> remoteDataSourceProvider = Provider(
    (ref) {
      final client = ref.watch(ApiHttpClientProvider.provider);
      final environment = ref.watch(AppEnvironmentProvider.provider);
      return OrdersRemoteDataSource(client: client, environment: environment);
    },
  );

  static final Provider<OrdersRepository> repositoryProvider = Provider((ref) {
    final remoteDataSource = ref.watch(remoteDataSourceProvider);
    return OrdersRepositoryImpl(remoteDataSource);
  });

  static final Provider<CreateTableOrderUseCase> createTableOrderUseCaseProvider = Provider(
    (ref) {
      final repository = ref.watch(repositoryProvider);
      return CreateTableOrderUseCase(repository);
    },
  );
}

