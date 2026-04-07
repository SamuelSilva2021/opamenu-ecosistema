import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../tables/domain/entities/table_entity.dart';
import '../pages/tabs_page_state.dart';

class TabsPage extends ConsumerStatefulWidget {
  final TableEntity table;

  const TabsPage({super.key, required this.table});

  @override
  ConsumerState<TabsPage> createState() => TabsPageState();
}
