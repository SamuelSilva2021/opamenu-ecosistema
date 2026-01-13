import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/models/create_order_request_dto.dart';
import '../../domain/models/order_response_dto.dart';
import '../datasources/orders_remote_datasource.dart';
import '../models/paged_response_model.dart';

part 'orders_repository.g.dart';

@riverpod
OrdersRepository ordersRepository(Ref ref) {
  return OrdersRepository(ref.watch(ordersRemoteDataSourceProvider));
}

class OrdersRepository {
  final OrdersRemoteDataSource _datasource;

  OrdersRepository(this._datasource);

  Future<PagedResponseModel<List<OrderResponseDto>>> getOrders({int page = 1, int pageSize = 10}) async {
    return await _datasource.getOrders(page: page, pageSize: pageSize);
  }

  Future<OrderResponseDto> createOrder(CreateOrderRequestDto order) async {
    return await _datasource.createOrder(order);
  }

  Future<OrderResponseDto> addItemsToOrder(int orderId, List<CreateOrderItemRequestDto> items) async {
    return await _datasource.addItemsToOrder(orderId, items);
  }
}
