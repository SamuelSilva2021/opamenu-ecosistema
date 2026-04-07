import '../../../../core/domain/result/result.dart';
import '../entities/tab_item_entity.dart';
import '../repositories/tabs_repository.dart';

class FetchTabItemsUseCase {
  final TabsRepository repository;

  const FetchTabItemsUseCase(this.repository);

  Future<Result<List<TabItemEntity>>> call({
    required String accessToken,
    required String tabId,
  }) {
    return repository.getTabItems(accessToken: accessToken, tabId: tabId);
  }
}

