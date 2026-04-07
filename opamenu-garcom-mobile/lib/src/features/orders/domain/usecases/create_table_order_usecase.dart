import '../../../../core/domain/result/result.dart';
import '../entities/order_entity.dart';
import '../repositories/orders_repository.dart';

class CreateTableOrderUseCase {
  final OrdersRepository repository;

  const CreateTableOrderUseCase(this.repository);

  Future<Result<OrderEntity>> call({
    required String accessToken,
    required String tableId,
    required String tabId,
    required String productId,
    required int quantity,
    String? notes,
  }) {
    return repository.createTableOrder(
      accessToken: accessToken,
      tableId: tableId,
      tabId: tabId,
      productId: productId,
      quantity: quantity,
      notes: notes,
    );
  }
}

