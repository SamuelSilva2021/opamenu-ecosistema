import '../../../../core/domain/result/result.dart';
import '../models/create_table_order_request_model.dart';
import '../models/order_model.dart';

abstract class OrdersRemoteDataSourceContract {
  Future<Result<OrderModel>> createTableOrder({
    required String accessToken,
    required CreateTableOrderRequestModel request,
  });
}

