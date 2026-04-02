import 'dart:io';

import 'package:drift/drift.dart';
import 'package:drift/native.dart';
import 'package:path_provider/path_provider.dart';
import 'package:path/path.dart' as p;

import 'tables/local_orders_table.dart';
import 'tables/sync_queue_table.dart';

part 'app_database.g.dart';

@DriftDatabase(tables: [LocalOrders, SyncQueue])
class AppDatabase extends _$AppDatabase {
  AppDatabase() : super(_openConnection());

  @override
  int get schemaVersion => 1;

  static QueryExecutor _openConnection() {
    return LazyDatabase(() async {
      final dir = await getApplicationSupportDirectory();
      final file = File(p.join(dir.path, 'opamenu.db'));
      return NativeDatabase.createInBackground(file);
    });
  }
}
