import 'package:drift/drift.dart';

class SyncQueue extends Table {
  IntColumn get id => integer().autoIncrement()();
  TextColumn get localOrderId => text()();
  IntColumn get attempts => integer().withDefault(const Constant(0))();
  TextColumn get lastError => text().nullable()();
  DateTimeColumn get nextRetryAt => dateTime()();
}
