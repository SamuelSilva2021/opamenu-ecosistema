import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../controllers/tables_controller.dart';
import '../../domain/entities/table_entity.dart';

class TablesControllerProvider {
  static final AsyncNotifierProvider<TablesController, List<TableEntity>> provider =
      AsyncNotifierProvider(TablesController.new);
}

