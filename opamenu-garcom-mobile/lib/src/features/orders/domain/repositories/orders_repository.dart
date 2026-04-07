import '../../../../core/domain/result/result.dart';
import '../entities/order_entity.dart';

abstract class OrdersRepository {
  Future<Result<OrderEntity>> createTableOrder({
    required String accessToken,
    required String tableId,
    required String tabId,
    required String productId,
    required int quantity,
    String? notes,
  });
}

