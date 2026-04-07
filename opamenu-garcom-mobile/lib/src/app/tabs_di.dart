import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../core/data/http/api_http_client_provider.dart';
import '../features/tabs/data/datasources/tabs_remote_data_source.dart';
import '../features/tabs/data/datasources/tabs_remote_data_source_contract.dart';
import '../features/tabs/data/repositories/tabs_repository_impl.dart';
import '../features/tabs/domain/repositories/tabs_repository.dart';
import '../features/tabs/domain/usecases/add_tab_items_usecase.dart';
import '../features/tabs/domain/usecases/delete_tab_usecase.dart';
import '../features/tabs/domain/usecases/fetch_tab_items_usecase.dart';
import '../features/tabs/domain/usecases/fetch_tabs_usecase.dart';
import '../features/tabs/domain/usecases/open_tab_usecase.dart';
import '../features/tabs/domain/usecases/update_tab_usecase.dart';
import 'app_environment_provider.dart';

class TabsDi {
  static final Provider<TabsRemoteDataSourceContract> remoteDataSourceProvider = Provider(
    (ref) {
      final client = ref.watch(ApiHttpClientProvider.provider);
      final environment = ref.watch(AppEnvironmentProvider.provider);
      return TabsRemoteDataSource(client: client, environment: environment);
    },
  );

  static final Provider<TabsRepository> repositoryProvider = Provider((ref) {
    final remoteDataSource = ref.watch(remoteDataSourceProvider);
    return TabsRepositoryImpl(remoteDataSource);
  });

  static final Provider<FetchTabsUseCase> fetchTabsUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return FetchTabsUseCase(repository);
  });

  static final Provider<OpenTabUseCase> openTabUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return OpenTabUseCase(repository);
  });

  static final Provider<UpdateTabUseCase> updateTabUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return UpdateTabUseCase(repository);
  });

  static final Provider<DeleteTabUseCase> deleteTabUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return DeleteTabUseCase(repository);
  });

  static final Provider<FetchTabItemsUseCase> fetchTabItemsUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return FetchTabItemsUseCase(repository);
  });

  static final Provider<AddTabItemsUseCase> addTabItemsUseCaseProvider = Provider((ref) {
    final repository = ref.watch(repositoryProvider);
    return AddTabItemsUseCase(repository);
  });
}
