import '../../../../core/domain/result/result.dart';
import '../repositories/tabs_repository.dart';

class DeleteTabUseCase {
  final TabsRepository repository;

  const DeleteTabUseCase(this.repository);

  Future<Result<bool>> call({
    required String accessToken,
    required String tabId,
  }) {
    return repository.deleteTab(accessToken: accessToken, tabId: tabId);
  }
}

