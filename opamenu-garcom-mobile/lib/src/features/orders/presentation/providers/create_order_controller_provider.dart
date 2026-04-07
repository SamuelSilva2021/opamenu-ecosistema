import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../domain/entities/order_entity.dart';
import '../controllers/create_order_controller.dart';

class CreateOrderControllerProvider {
  static final AsyncNotifierProvider<CreateOrderController, OrderEntity?> provider =
      AsyncNotifierProvider(CreateOrderController.new);
}

