import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../domain/entities/tab_item_entity.dart';
import '../controllers/tab_items_controller.dart';

class TabItemsControllerProvider {
  static final AsyncNotifierProvider<TabItemsController, List<TabItemEntity>> provider =
      AsyncNotifierProvider(TabItemsController.new);
}

