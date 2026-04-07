import '../../../../core/domain/result/result.dart';
import '../entities/tab_entity.dart';
import '../repositories/tabs_repository.dart';

class UpdateTabUseCase {
  final TabsRepository repository;

  const UpdateTabUseCase(this.repository);

  Future<Result<TabEntity>> call({
    required String accessToken,
    required String tabId,
    String? name,
    String? tableId,
  }) {
    return repository.updateTab(
      accessToken: accessToken,
      tabId: tabId,
      name: name,
      tableId: tableId,
    );
  }
}

