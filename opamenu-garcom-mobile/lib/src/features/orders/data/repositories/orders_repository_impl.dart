import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../../domain/entities/order_entity.dart';
import '../../domain/repositories/orders_repository.dart';
import '../datasources/orders_remote_data_source_contract.dart';
import '../models/create_order_item_model.dart';
import '../models/create_table_order_request_model.dart';
import '../models/order_model.dart';

class OrdersRepositoryImpl implements OrdersRepository {
  final OrdersRemoteDataSourceContract remoteDataSource;

  const OrdersRepositoryImpl(this.remoteDataSource);

  @override
  Future<Result<OrderEntity>> createTableOrder({
    required String accessToken,
    required String tableId,
    required String tabId,
    required String productId,
    required int quantity,
    String? notes,
  }) async {
    final request = CreateTableOrderRequestModel(
      tableId: tableId,
      tabId: tabId,
      items: [
        CreateOrderItemModel(
          productId: productId,
          quantity: quantity,
          notes: notes,
        ),
      ],
    );

    final result = await remoteDataSource.createTableOrder(
      accessToken: accessToken,
      request: request,
    );

    if (result is FailureResult<OrderModel>) {
      return FailureResult(result.failure);
    }

    final model = (result as SuccessResult<OrderModel>).value;
    return SuccessResult<OrderEntity>(model.toEntity());
  }
}

