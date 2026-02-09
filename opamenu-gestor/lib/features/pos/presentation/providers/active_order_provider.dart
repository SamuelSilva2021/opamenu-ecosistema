import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/models/order_response_dto.dart';

part 'active_order_provider.g.dart';

@Riverpod(keepAlive: true)
class ActiveOrder extends _$ActiveOrder {
  @override
  OrderResponseDto? build() => null;

  void setOrder(OrderResponseDto? order) => state = order;
  void clear() => state = null;
}

@Riverpod(keepAlive: true)
class ActiveTable extends _$ActiveTable {
  @override
  String? build() => null;

  void setTableId(String? id) => state = id;
  void clear() => state = null;
}
