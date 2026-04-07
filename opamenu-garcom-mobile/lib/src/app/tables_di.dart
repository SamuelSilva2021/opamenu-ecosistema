import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/data/http/api_http_client_provider.dart';
import '../features/tables/data/datasources/tables_remote_data_source.dart';
import '../features/tables/data/datasources/tables_remote_data_source_contract.dart';
import '../features/tables/data/repositories/tables_repository_impl.dart';
import '../features/tables/domain/repositories/tables_repository.dart';
import '../features/tables/domain/usecases/fetch_tables_usecase.dart';
import 'app_environment_provider.dart';

class TablesDi {
  static final Provider<TablesRemoteDataSourceContract> remoteDataSourceProvider = Provider(
    (ref) {
      final client = ref.watch(ApiHttpClientProvider.provider);
      final environment = ref.watch(AppEnvironmentProvider.provider);
      return TablesRemoteDataSource(client: client, environment: environment);
    },
  );

  static final Provider<TablesRepository> repositoryProvider = Provider((ref) {
    final remoteDataSource = ref.watch(remoteDataSourceProvider);
    return TablesRepositoryImpl(remoteDataSource);
  });

  static final Provider<FetchTablesUseCase> fetchTablesUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return FetchTablesUseCase(repository);
  });
}

