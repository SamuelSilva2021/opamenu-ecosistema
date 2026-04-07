import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../controllers/add_tab_items_controller.dart';

class AddTabItemsControllerProvider {
  static final AsyncNotifierProvider<AddTabItemsController, bool?> provider =
      AsyncNotifierProvider(AddTabItemsController.new);
}

