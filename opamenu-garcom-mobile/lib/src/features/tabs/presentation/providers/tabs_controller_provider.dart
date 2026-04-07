import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../domain/entities/tab_entity.dart';
import '../controllers/tabs_controller.dart';

class TabsControllerProvider {
  static final AsyncNotifierProvider<TabsController, List<TabEntity>> provider =
      AsyncNotifierProvider(TabsController.new);
}
