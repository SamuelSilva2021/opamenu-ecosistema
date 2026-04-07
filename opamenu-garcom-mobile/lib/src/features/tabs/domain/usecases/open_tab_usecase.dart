import '../../../../core/domain/result/result.dart';
import '../entities/tab_entity.dart';
import '../repositories/tabs_repository.dart';

class OpenTabUseCase {
  final TabsRepository repository;

  const OpenTabUseCase(this.repository);

  Future<Result<TabEntity>> call({
    required String accessToken,
    required String tableId,
    String? name,
  }) {
    return repository.openTab(accessToken: accessToken, tableId: tableId, name: name);
  }
}

