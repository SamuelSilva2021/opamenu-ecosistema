import '../../../../core/domain/result/result.dart';
import '../entities/tab_entity.dart';
import '../repositories/tabs_repository.dart';

class FetchTabsUseCase {
  final TabsRepository repository;

  const FetchTabsUseCase(this.repository);

  Future<Result<List<TabEntity>>> call({
    required String accessToken,
    required String tableId,
  }) {
    return repository.getTabs(accessToken: accessToken, tableId: tableId);
  }
}

