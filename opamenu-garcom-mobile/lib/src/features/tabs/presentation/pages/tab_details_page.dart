import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../tables/domain/entities/table_entity.dart';
import '../../domain/entities/tab_entity.dart';
import 'tab_details_page_state.dart';

class TabDetailsPage extends ConsumerStatefulWidget {
  final TableEntity table;
  final TabEntity tab;

  const TabDetailsPage({
    super.key,
    required this.table,
    required this.tab,
  });

  @override
  ConsumerState<TabDetailsPage> createState() => TabDetailsPageState();
}
