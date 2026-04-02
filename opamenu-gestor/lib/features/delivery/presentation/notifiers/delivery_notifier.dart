import 'dart:async';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../pos/domain/enums/order_status.dart';
import '../../../pos/domain/models/order_response_dto.dart';
import '../../../pos/data/repositories/orders_repository.dart';

part 'delivery_notifier.g.dart';

class DeliveryState {
  final List<OrderResponseDto> newOrders;
  final List<OrderResponseDto> preparingOrders;
  final List<OrderResponseDto> readyOrders;
  final List<OrderResponseDto> dispatchOrders;

  DeliveryState({
    required this.newOrders,
    required this.preparingOrders,
    required this.readyOrders,
    required this.dispatchOrders,
  });
}

@riverpod
class DeliveryNotifier extends _$DeliveryNotifier {
  @override
  FutureOr<DeliveryState> build() async {
    final repository = ref.watch(ordersRepositoryProvider);
    final orders = await repository.getDeliveryBoardOrders();

    final newOrders = <OrderResponseDto>[];
    final preparingOrders = <OrderResponseDto>[];
    final readyOrders = <OrderResponseDto>[];
    final dispatchOrders = <OrderResponseDto>[];

    for (final order in orders) {
      switch (order.status) {
        case OrderStatus.pending:
          newOrders.add(order);
          break;
        case OrderStatus.preparing:
          preparingOrders.add(order);
          break;
        case OrderStatus.ready:
          readyOrders.add(order);
          break;
        case OrderStatus.outForDelivery:
          dispatchOrders.add(order);
          break;
        default:
          // Ignore finished orders for now in the active board columns
          break;
      }
    }

    return DeliveryState(
      newOrders: newOrders,
      preparingOrders: preparingOrders,
      readyOrders: readyOrders,
      dispatchOrders: dispatchOrders,
    );
  }

  Future<void> refresh() async {
    ref.invalidateSelf();
    await future;
  }
}
