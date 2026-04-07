import '../../../../core/domain/result/result.dart';
import '../repositories/tabs_repository.dart';

class AddTabItemsUseCase {
  final TabsRepository repository;

  const AddTabItemsUseCase(this.repository);

  Future<Result<bool>> call({
    required String accessToken,
    required String tabId,
    required String productId,
    required int quantity,
    String? notes,
  }) {
    return repository.addTabItems(
      accessToken: accessToken,
      tabId: tabId,
      productId: productId,
      quantity: quantity,
      notes: notes,
    );
  }
}

