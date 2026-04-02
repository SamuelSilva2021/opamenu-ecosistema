import 'package:drift/drift.dart';

class LocalOrders extends Table {
  IntColumn get id => integer().autoIncrement()();
  TextColumn get localId => text()();          // UUID local
  TextColumn get cloudId => text().nullable()(); // ID do pedido na nuvem
  TextColumn get tableNumber => text().nullable()();
  TextColumn get status => text()(); // 'pending_sync' | 'synced' | 'error'
  TextColumn get payload => text()(); // JSON completo do pedido
  DateTimeColumn get createdAt => dateTime()();
  DateTimeColumn get updatedAt => dateTime()();
}
