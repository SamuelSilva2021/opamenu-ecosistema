// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'app_database.dart';

// ignore_for_file: type=lint
class $LocalOrdersTable extends LocalOrders
    with TableInfo<$LocalOrdersTable, LocalOrder> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $LocalOrdersTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _idMeta = const VerificationMeta('id');
  @override
  late final GeneratedColumn<int> id = GeneratedColumn<int>(
    'id',
    aliasedName,
    false,
    hasAutoIncrement: true,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultConstraints: GeneratedColumn.constraintIsAlways(
      'PRIMARY KEY AUTOINCREMENT',
    ),
  );
  static const VerificationMeta _localIdMeta = const VerificationMeta(
    'localId',
  );
  @override
  late final GeneratedColumn<String> localId = GeneratedColumn<String>(
    'local_id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _cloudIdMeta = const VerificationMeta(
    'cloudId',
  );
  @override
  late final GeneratedColumn<String> cloudId = GeneratedColumn<String>(
    'cloud_id',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _tableNumberMeta = const VerificationMeta(
    'tableNumber',
  );
  @override
  late final GeneratedColumn<String> tableNumber = GeneratedColumn<String>(
    'table_number',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _statusMeta = const VerificationMeta('status');
  @override
  late final GeneratedColumn<String> status = GeneratedColumn<String>(
    'status',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _payloadMeta = const VerificationMeta(
    'payload',
  );
  @override
  late final GeneratedColumn<String> payload = GeneratedColumn<String>(
    'payload',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _createdAtMeta = const VerificationMeta(
    'createdAt',
  );
  @override
  late final GeneratedColumn<DateTime> createdAt = GeneratedColumn<DateTime>(
    'created_at',
    aliasedName,
    false,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _updatedAtMeta = const VerificationMeta(
    'updatedAt',
  );
  @override
  late final GeneratedColumn<DateTime> updatedAt = GeneratedColumn<DateTime>(
    'updated_at',
    aliasedName,
    false,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: true,
  );
  @override
  List<GeneratedColumn> get $columns => [
    id,
    localId,
    cloudId,
    tableNumber,
    status,
    payload,
    createdAt,
    updatedAt,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'local_orders';
  @override
  VerificationContext validateIntegrity(
    Insertable<LocalOrder> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('id')) {
      context.handle(_idMeta, id.isAcceptableOrUnknown(data['id']!, _idMeta));
    }
    if (data.containsKey('local_id')) {
      context.handle(
        _localIdMeta,
        localId.isAcceptableOrUnknown(data['local_id']!, _localIdMeta),
      );
    } else if (isInserting) {
      context.missing(_localIdMeta);
    }
    if (data.containsKey('cloud_id')) {
      context.handle(
        _cloudIdMeta,
        cloudId.isAcceptableOrUnknown(data['cloud_id']!, _cloudIdMeta),
      );
    }
    if (data.containsKey('table_number')) {
      context.handle(
        _tableNumberMeta,
        tableNumber.isAcceptableOrUnknown(
          data['table_number']!,
          _tableNumberMeta,
        ),
      );
    }
    if (data.containsKey('status')) {
      context.handle(
        _statusMeta,
        status.isAcceptableOrUnknown(data['status']!, _statusMeta),
      );
    } else if (isInserting) {
      context.missing(_statusMeta);
    }
    if (data.containsKey('payload')) {
      context.handle(
        _payloadMeta,
        payload.isAcceptableOrUnknown(data['payload']!, _payloadMeta),
      );
    } else if (isInserting) {
      context.missing(_payloadMeta);
    }
    if (data.containsKey('created_at')) {
      context.handle(
        _createdAtMeta,
        createdAt.isAcceptableOrUnknown(data['created_at']!, _createdAtMeta),
      );
    } else if (isInserting) {
      context.missing(_createdAtMeta);
    }
    if (data.containsKey('updated_at')) {
      context.handle(
        _updatedAtMeta,
        updatedAt.isAcceptableOrUnknown(data['updated_at']!, _updatedAtMeta),
      );
    } else if (isInserting) {
      context.missing(_updatedAtMeta);
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {id};
  @override
  LocalOrder map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return LocalOrder(
      id: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}id'],
      )!,
      localId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}local_id'],
      )!,
      cloudId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}cloud_id'],
      ),
      tableNumber: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}table_number'],
      ),
      status: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}status'],
      )!,
      payload: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}payload'],
      )!,
      createdAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}created_at'],
      )!,
      updatedAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}updated_at'],
      )!,
    );
  }

  @override
  $LocalOrdersTable createAlias(String alias) {
    return $LocalOrdersTable(attachedDatabase, alias);
  }
}

class LocalOrder extends DataClass implements Insertable<LocalOrder> {
  final int id;
  final String localId;
  final String? cloudId;
  final String? tableNumber;
  final String status;
  final String payload;
  final DateTime createdAt;
  final DateTime updatedAt;
  const LocalOrder({
    required this.id,
    required this.localId,
    this.cloudId,
    this.tableNumber,
    required this.status,
    required this.payload,
    required this.createdAt,
    required this.updatedAt,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['id'] = Variable<int>(id);
    map['local_id'] = Variable<String>(localId);
    if (!nullToAbsent || cloudId != null) {
      map['cloud_id'] = Variable<String>(cloudId);
    }
    if (!nullToAbsent || tableNumber != null) {
      map['table_number'] = Variable<String>(tableNumber);
    }
    map['status'] = Variable<String>(status);
    map['payload'] = Variable<String>(payload);
    map['created_at'] = Variable<DateTime>(createdAt);
    map['updated_at'] = Variable<DateTime>(updatedAt);
    return map;
  }

  LocalOrdersCompanion toCompanion(bool nullToAbsent) {
    return LocalOrdersCompanion(
      id: Value(id),
      localId: Value(localId),
      cloudId: cloudId == null && nullToAbsent
          ? const Value.absent()
          : Value(cloudId),
      tableNumber: tableNumber == null && nullToAbsent
          ? const Value.absent()
          : Value(tableNumber),
      status: Value(status),
      payload: Value(payload),
      createdAt: Value(createdAt),
      updatedAt: Value(updatedAt),
    );
  }

  factory LocalOrder.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return LocalOrder(
      id: serializer.fromJson<int>(json['id']),
      localId: serializer.fromJson<String>(json['localId']),
      cloudId: serializer.fromJson<String?>(json['cloudId']),
      tableNumber: serializer.fromJson<String?>(json['tableNumber']),
      status: serializer.fromJson<String>(json['status']),
      payload: serializer.fromJson<String>(json['payload']),
      createdAt: serializer.fromJson<DateTime>(json['createdAt']),
      updatedAt: serializer.fromJson<DateTime>(json['updatedAt']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'id': serializer.toJson<int>(id),
      'localId': serializer.toJson<String>(localId),
      'cloudId': serializer.toJson<String?>(cloudId),
      'tableNumber': serializer.toJson<String?>(tableNumber),
      'status': serializer.toJson<String>(status),
      'payload': serializer.toJson<String>(payload),
      'createdAt': serializer.toJson<DateTime>(createdAt),
      'updatedAt': serializer.toJson<DateTime>(updatedAt),
    };
  }

  LocalOrder copyWith({
    int? id,
    String? localId,
    Value<String?> cloudId = const Value.absent(),
    Value<String?> tableNumber = const Value.absent(),
    String? status,
    String? payload,
    DateTime? createdAt,
    DateTime? updatedAt,
  }) => LocalOrder(
    id: id ?? this.id,
    localId: localId ?? this.localId,
    cloudId: cloudId.present ? cloudId.value : this.cloudId,
    tableNumber: tableNumber.present ? tableNumber.value : this.tableNumber,
    status: status ?? this.status,
    payload: payload ?? this.payload,
    createdAt: createdAt ?? this.createdAt,
    updatedAt: updatedAt ?? this.updatedAt,
  );
  LocalOrder copyWithCompanion(LocalOrdersCompanion data) {
    return LocalOrder(
      id: data.id.present ? data.id.value : this.id,
      localId: data.localId.present ? data.localId.value : this.localId,
      cloudId: data.cloudId.present ? data.cloudId.value : this.cloudId,
      tableNumber: data.tableNumber.present
          ? data.tableNumber.value
          : this.tableNumber,
      status: data.status.present ? data.status.value : this.status,
      payload: data.payload.present ? data.payload.value : this.payload,
      createdAt: data.createdAt.present ? data.createdAt.value : this.createdAt,
      updatedAt: data.updatedAt.present ? data.updatedAt.value : this.updatedAt,
    );
  }

  @override
  String toString() {
    return (StringBuffer('LocalOrder(')
          ..write('id: $id, ')
          ..write('localId: $localId, ')
          ..write('cloudId: $cloudId, ')
          ..write('tableNumber: $tableNumber, ')
          ..write('status: $status, ')
          ..write('payload: $payload, ')
          ..write('createdAt: $createdAt, ')
          ..write('updatedAt: $updatedAt')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode => Object.hash(
    id,
    localId,
    cloudId,
    tableNumber,
    status,
    payload,
    createdAt,
    updatedAt,
  );
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is LocalOrder &&
          other.id == this.id &&
          other.localId == this.localId &&
          other.cloudId == this.cloudId &&
          other.tableNumber == this.tableNumber &&
          other.status == this.status &&
          other.payload == this.payload &&
          other.createdAt == this.createdAt &&
          other.updatedAt == this.updatedAt);
}

class LocalOrdersCompanion extends UpdateCompanion<LocalOrder> {
  final Value<int> id;
  final Value<String> localId;
  final Value<String?> cloudId;
  final Value<String?> tableNumber;
  final Value<String> status;
  final Value<String> payload;
  final Value<DateTime> createdAt;
  final Value<DateTime> updatedAt;
  const LocalOrdersCompanion({
    this.id = const Value.absent(),
    this.localId = const Value.absent(),
    this.cloudId = const Value.absent(),
    this.tableNumber = const Value.absent(),
    this.status = const Value.absent(),
    this.payload = const Value.absent(),
    this.createdAt = const Value.absent(),
    this.updatedAt = const Value.absent(),
  });
  LocalOrdersCompanion.insert({
    this.id = const Value.absent(),
    required String localId,
    this.cloudId = const Value.absent(),
    this.tableNumber = const Value.absent(),
    required String status,
    required String payload,
    required DateTime createdAt,
    required DateTime updatedAt,
  }) : localId = Value(localId),
       status = Value(status),
       payload = Value(payload),
       createdAt = Value(createdAt),
       updatedAt = Value(updatedAt);
  static Insertable<LocalOrder> custom({
    Expression<int>? id,
    Expression<String>? localId,
    Expression<String>? cloudId,
    Expression<String>? tableNumber,
    Expression<String>? status,
    Expression<String>? payload,
    Expression<DateTime>? createdAt,
    Expression<DateTime>? updatedAt,
  }) {
    return RawValuesInsertable({
      if (id != null) 'id': id,
      if (localId != null) 'local_id': localId,
      if (cloudId != null) 'cloud_id': cloudId,
      if (tableNumber != null) 'table_number': tableNumber,
      if (status != null) 'status': status,
      if (payload != null) 'payload': payload,
      if (createdAt != null) 'created_at': createdAt,
      if (updatedAt != null) 'updated_at': updatedAt,
    });
  }

  LocalOrdersCompanion copyWith({
    Value<int>? id,
    Value<String>? localId,
    Value<String?>? cloudId,
    Value<String?>? tableNumber,
    Value<String>? status,
    Value<String>? payload,
    Value<DateTime>? createdAt,
    Value<DateTime>? updatedAt,
  }) {
    return LocalOrdersCompanion(
      id: id ?? this.id,
      localId: localId ?? this.localId,
      cloudId: cloudId ?? this.cloudId,
      tableNumber: tableNumber ?? this.tableNumber,
      status: status ?? this.status,
      payload: payload ?? this.payload,
      createdAt: createdAt ?? this.createdAt,
      updatedAt: updatedAt ?? this.updatedAt,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (id.present) {
      map['id'] = Variable<int>(id.value);
    }
    if (localId.present) {
      map['local_id'] = Variable<String>(localId.value);
    }
    if (cloudId.present) {
      map['cloud_id'] = Variable<String>(cloudId.value);
    }
    if (tableNumber.present) {
      map['table_number'] = Variable<String>(tableNumber.value);
    }
    if (status.present) {
      map['status'] = Variable<String>(status.value);
    }
    if (payload.present) {
      map['payload'] = Variable<String>(payload.value);
    }
    if (createdAt.present) {
      map['created_at'] = Variable<DateTime>(createdAt.value);
    }
    if (updatedAt.present) {
      map['updated_at'] = Variable<DateTime>(updatedAt.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('LocalOrdersCompanion(')
          ..write('id: $id, ')
          ..write('localId: $localId, ')
          ..write('cloudId: $cloudId, ')
          ..write('tableNumber: $tableNumber, ')
          ..write('status: $status, ')
          ..write('payload: $payload, ')
          ..write('createdAt: $createdAt, ')
          ..write('updatedAt: $updatedAt')
          ..write(')'))
        .toString();
  }
}

class $SyncQueueTable extends SyncQueue
    with TableInfo<$SyncQueueTable, SyncQueueData> {
  @override
  final GeneratedDatabase attachedDatabase;
  final String? _alias;
  $SyncQueueTable(this.attachedDatabase, [this._alias]);
  static const VerificationMeta _idMeta = const VerificationMeta('id');
  @override
  late final GeneratedColumn<int> id = GeneratedColumn<int>(
    'id',
    aliasedName,
    false,
    hasAutoIncrement: true,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultConstraints: GeneratedColumn.constraintIsAlways(
      'PRIMARY KEY AUTOINCREMENT',
    ),
  );
  static const VerificationMeta _localOrderIdMeta = const VerificationMeta(
    'localOrderId',
  );
  @override
  late final GeneratedColumn<String> localOrderId = GeneratedColumn<String>(
    'local_order_id',
    aliasedName,
    false,
    type: DriftSqlType.string,
    requiredDuringInsert: true,
  );
  static const VerificationMeta _attemptsMeta = const VerificationMeta(
    'attempts',
  );
  @override
  late final GeneratedColumn<int> attempts = GeneratedColumn<int>(
    'attempts',
    aliasedName,
    false,
    type: DriftSqlType.int,
    requiredDuringInsert: false,
    defaultValue: const Constant(0),
  );
  static const VerificationMeta _lastErrorMeta = const VerificationMeta(
    'lastError',
  );
  @override
  late final GeneratedColumn<String> lastError = GeneratedColumn<String>(
    'last_error',
    aliasedName,
    true,
    type: DriftSqlType.string,
    requiredDuringInsert: false,
  );
  static const VerificationMeta _nextRetryAtMeta = const VerificationMeta(
    'nextRetryAt',
  );
  @override
  late final GeneratedColumn<DateTime> nextRetryAt = GeneratedColumn<DateTime>(
    'next_retry_at',
    aliasedName,
    false,
    type: DriftSqlType.dateTime,
    requiredDuringInsert: true,
  );
  @override
  List<GeneratedColumn> get $columns => [
    id,
    localOrderId,
    attempts,
    lastError,
    nextRetryAt,
  ];
  @override
  String get aliasedName => _alias ?? actualTableName;
  @override
  String get actualTableName => $name;
  static const String $name = 'sync_queue';
  @override
  VerificationContext validateIntegrity(
    Insertable<SyncQueueData> instance, {
    bool isInserting = false,
  }) {
    final context = VerificationContext();
    final data = instance.toColumns(true);
    if (data.containsKey('id')) {
      context.handle(_idMeta, id.isAcceptableOrUnknown(data['id']!, _idMeta));
    }
    if (data.containsKey('local_order_id')) {
      context.handle(
        _localOrderIdMeta,
        localOrderId.isAcceptableOrUnknown(
          data['local_order_id']!,
          _localOrderIdMeta,
        ),
      );
    } else if (isInserting) {
      context.missing(_localOrderIdMeta);
    }
    if (data.containsKey('attempts')) {
      context.handle(
        _attemptsMeta,
        attempts.isAcceptableOrUnknown(data['attempts']!, _attemptsMeta),
      );
    }
    if (data.containsKey('last_error')) {
      context.handle(
        _lastErrorMeta,
        lastError.isAcceptableOrUnknown(data['last_error']!, _lastErrorMeta),
      );
    }
    if (data.containsKey('next_retry_at')) {
      context.handle(
        _nextRetryAtMeta,
        nextRetryAt.isAcceptableOrUnknown(
          data['next_retry_at']!,
          _nextRetryAtMeta,
        ),
      );
    } else if (isInserting) {
      context.missing(_nextRetryAtMeta);
    }
    return context;
  }

  @override
  Set<GeneratedColumn> get $primaryKey => {id};
  @override
  SyncQueueData map(Map<String, dynamic> data, {String? tablePrefix}) {
    final effectivePrefix = tablePrefix != null ? '$tablePrefix.' : '';
    return SyncQueueData(
      id: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}id'],
      )!,
      localOrderId: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}local_order_id'],
      )!,
      attempts: attachedDatabase.typeMapping.read(
        DriftSqlType.int,
        data['${effectivePrefix}attempts'],
      )!,
      lastError: attachedDatabase.typeMapping.read(
        DriftSqlType.string,
        data['${effectivePrefix}last_error'],
      ),
      nextRetryAt: attachedDatabase.typeMapping.read(
        DriftSqlType.dateTime,
        data['${effectivePrefix}next_retry_at'],
      )!,
    );
  }

  @override
  $SyncQueueTable createAlias(String alias) {
    return $SyncQueueTable(attachedDatabase, alias);
  }
}

class SyncQueueData extends DataClass implements Insertable<SyncQueueData> {
  final int id;
  final String localOrderId;
  final int attempts;
  final String? lastError;
  final DateTime nextRetryAt;
  const SyncQueueData({
    required this.id,
    required this.localOrderId,
    required this.attempts,
    this.lastError,
    required this.nextRetryAt,
  });
  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    map['id'] = Variable<int>(id);
    map['local_order_id'] = Variable<String>(localOrderId);
    map['attempts'] = Variable<int>(attempts);
    if (!nullToAbsent || lastError != null) {
      map['last_error'] = Variable<String>(lastError);
    }
    map['next_retry_at'] = Variable<DateTime>(nextRetryAt);
    return map;
  }

  SyncQueueCompanion toCompanion(bool nullToAbsent) {
    return SyncQueueCompanion(
      id: Value(id),
      localOrderId: Value(localOrderId),
      attempts: Value(attempts),
      lastError: lastError == null && nullToAbsent
          ? const Value.absent()
          : Value(lastError),
      nextRetryAt: Value(nextRetryAt),
    );
  }

  factory SyncQueueData.fromJson(
    Map<String, dynamic> json, {
    ValueSerializer? serializer,
  }) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return SyncQueueData(
      id: serializer.fromJson<int>(json['id']),
      localOrderId: serializer.fromJson<String>(json['localOrderId']),
      attempts: serializer.fromJson<int>(json['attempts']),
      lastError: serializer.fromJson<String?>(json['lastError']),
      nextRetryAt: serializer.fromJson<DateTime>(json['nextRetryAt']),
    );
  }
  @override
  Map<String, dynamic> toJson({ValueSerializer? serializer}) {
    serializer ??= driftRuntimeOptions.defaultSerializer;
    return <String, dynamic>{
      'id': serializer.toJson<int>(id),
      'localOrderId': serializer.toJson<String>(localOrderId),
      'attempts': serializer.toJson<int>(attempts),
      'lastError': serializer.toJson<String?>(lastError),
      'nextRetryAt': serializer.toJson<DateTime>(nextRetryAt),
    };
  }

  SyncQueueData copyWith({
    int? id,
    String? localOrderId,
    int? attempts,
    Value<String?> lastError = const Value.absent(),
    DateTime? nextRetryAt,
  }) => SyncQueueData(
    id: id ?? this.id,
    localOrderId: localOrderId ?? this.localOrderId,
    attempts: attempts ?? this.attempts,
    lastError: lastError.present ? lastError.value : this.lastError,
    nextRetryAt: nextRetryAt ?? this.nextRetryAt,
  );
  SyncQueueData copyWithCompanion(SyncQueueCompanion data) {
    return SyncQueueData(
      id: data.id.present ? data.id.value : this.id,
      localOrderId: data.localOrderId.present
          ? data.localOrderId.value
          : this.localOrderId,
      attempts: data.attempts.present ? data.attempts.value : this.attempts,
      lastError: data.lastError.present ? data.lastError.value : this.lastError,
      nextRetryAt: data.nextRetryAt.present
          ? data.nextRetryAt.value
          : this.nextRetryAt,
    );
  }

  @override
  String toString() {
    return (StringBuffer('SyncQueueData(')
          ..write('id: $id, ')
          ..write('localOrderId: $localOrderId, ')
          ..write('attempts: $attempts, ')
          ..write('lastError: $lastError, ')
          ..write('nextRetryAt: $nextRetryAt')
          ..write(')'))
        .toString();
  }

  @override
  int get hashCode =>
      Object.hash(id, localOrderId, attempts, lastError, nextRetryAt);
  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      (other is SyncQueueData &&
          other.id == this.id &&
          other.localOrderId == this.localOrderId &&
          other.attempts == this.attempts &&
          other.lastError == this.lastError &&
          other.nextRetryAt == this.nextRetryAt);
}

class SyncQueueCompanion extends UpdateCompanion<SyncQueueData> {
  final Value<int> id;
  final Value<String> localOrderId;
  final Value<int> attempts;
  final Value<String?> lastError;
  final Value<DateTime> nextRetryAt;
  const SyncQueueCompanion({
    this.id = const Value.absent(),
    this.localOrderId = const Value.absent(),
    this.attempts = const Value.absent(),
    this.lastError = const Value.absent(),
    this.nextRetryAt = const Value.absent(),
  });
  SyncQueueCompanion.insert({
    this.id = const Value.absent(),
    required String localOrderId,
    this.attempts = const Value.absent(),
    this.lastError = const Value.absent(),
    required DateTime nextRetryAt,
  }) : localOrderId = Value(localOrderId),
       nextRetryAt = Value(nextRetryAt);
  static Insertable<SyncQueueData> custom({
    Expression<int>? id,
    Expression<String>? localOrderId,
    Expression<int>? attempts,
    Expression<String>? lastError,
    Expression<DateTime>? nextRetryAt,
  }) {
    return RawValuesInsertable({
      if (id != null) 'id': id,
      if (localOrderId != null) 'local_order_id': localOrderId,
      if (attempts != null) 'attempts': attempts,
      if (lastError != null) 'last_error': lastError,
      if (nextRetryAt != null) 'next_retry_at': nextRetryAt,
    });
  }

  SyncQueueCompanion copyWith({
    Value<int>? id,
    Value<String>? localOrderId,
    Value<int>? attempts,
    Value<String?>? lastError,
    Value<DateTime>? nextRetryAt,
  }) {
    return SyncQueueCompanion(
      id: id ?? this.id,
      localOrderId: localOrderId ?? this.localOrderId,
      attempts: attempts ?? this.attempts,
      lastError: lastError ?? this.lastError,
      nextRetryAt: nextRetryAt ?? this.nextRetryAt,
    );
  }

  @override
  Map<String, Expression> toColumns(bool nullToAbsent) {
    final map = <String, Expression>{};
    if (id.present) {
      map['id'] = Variable<int>(id.value);
    }
    if (localOrderId.present) {
      map['local_order_id'] = Variable<String>(localOrderId.value);
    }
    if (attempts.present) {
      map['attempts'] = Variable<int>(attempts.value);
    }
    if (lastError.present) {
      map['last_error'] = Variable<String>(lastError.value);
    }
    if (nextRetryAt.present) {
      map['next_retry_at'] = Variable<DateTime>(nextRetryAt.value);
    }
    return map;
  }

  @override
  String toString() {
    return (StringBuffer('SyncQueueCompanion(')
          ..write('id: $id, ')
          ..write('localOrderId: $localOrderId, ')
          ..write('attempts: $attempts, ')
          ..write('lastError: $lastError, ')
          ..write('nextRetryAt: $nextRetryAt')
          ..write(')'))
        .toString();
  }
}

abstract class _$AppDatabase extends GeneratedDatabase {
  _$AppDatabase(QueryExecutor e) : super(e);
  $AppDatabaseManager get managers => $AppDatabaseManager(this);
  late final $LocalOrdersTable localOrders = $LocalOrdersTable(this);
  late final $SyncQueueTable syncQueue = $SyncQueueTable(this);
  @override
  Iterable<TableInfo<Table, Object?>> get allTables =>
      allSchemaEntities.whereType<TableInfo<Table, Object?>>();
  @override
  List<DatabaseSchemaEntity> get allSchemaEntities => [localOrders, syncQueue];
}

typedef $$LocalOrdersTableCreateCompanionBuilder =
    LocalOrdersCompanion Function({
      Value<int> id,
      required String localId,
      Value<String?> cloudId,
      Value<String?> tableNumber,
      required String status,
      required String payload,
      required DateTime createdAt,
      required DateTime updatedAt,
    });
typedef $$LocalOrdersTableUpdateCompanionBuilder =
    LocalOrdersCompanion Function({
      Value<int> id,
      Value<String> localId,
      Value<String?> cloudId,
      Value<String?> tableNumber,
      Value<String> status,
      Value<String> payload,
      Value<DateTime> createdAt,
      Value<DateTime> updatedAt,
    });

class $$LocalOrdersTableFilterComposer
    extends Composer<_$AppDatabase, $LocalOrdersTable> {
  $$LocalOrdersTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<int> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get localId => $composableBuilder(
    column: $table.localId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get cloudId => $composableBuilder(
    column: $table.cloudId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get tableNumber => $composableBuilder(
    column: $table.tableNumber,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get status => $composableBuilder(
    column: $table.status,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get payload => $composableBuilder(
    column: $table.payload,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get createdAt => $composableBuilder(
    column: $table.createdAt,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get updatedAt => $composableBuilder(
    column: $table.updatedAt,
    builder: (column) => ColumnFilters(column),
  );
}

class $$LocalOrdersTableOrderingComposer
    extends Composer<_$AppDatabase, $LocalOrdersTable> {
  $$LocalOrdersTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<int> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get localId => $composableBuilder(
    column: $table.localId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get cloudId => $composableBuilder(
    column: $table.cloudId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get tableNumber => $composableBuilder(
    column: $table.tableNumber,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get status => $composableBuilder(
    column: $table.status,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get payload => $composableBuilder(
    column: $table.payload,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get createdAt => $composableBuilder(
    column: $table.createdAt,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get updatedAt => $composableBuilder(
    column: $table.updatedAt,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$LocalOrdersTableAnnotationComposer
    extends Composer<_$AppDatabase, $LocalOrdersTable> {
  $$LocalOrdersTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<int> get id =>
      $composableBuilder(column: $table.id, builder: (column) => column);

  GeneratedColumn<String> get localId =>
      $composableBuilder(column: $table.localId, builder: (column) => column);

  GeneratedColumn<String> get cloudId =>
      $composableBuilder(column: $table.cloudId, builder: (column) => column);

  GeneratedColumn<String> get tableNumber => $composableBuilder(
    column: $table.tableNumber,
    builder: (column) => column,
  );

  GeneratedColumn<String> get status =>
      $composableBuilder(column: $table.status, builder: (column) => column);

  GeneratedColumn<String> get payload =>
      $composableBuilder(column: $table.payload, builder: (column) => column);

  GeneratedColumn<DateTime> get createdAt =>
      $composableBuilder(column: $table.createdAt, builder: (column) => column);

  GeneratedColumn<DateTime> get updatedAt =>
      $composableBuilder(column: $table.updatedAt, builder: (column) => column);
}

class $$LocalOrdersTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $LocalOrdersTable,
          LocalOrder,
          $$LocalOrdersTableFilterComposer,
          $$LocalOrdersTableOrderingComposer,
          $$LocalOrdersTableAnnotationComposer,
          $$LocalOrdersTableCreateCompanionBuilder,
          $$LocalOrdersTableUpdateCompanionBuilder,
          (
            LocalOrder,
            BaseReferences<_$AppDatabase, $LocalOrdersTable, LocalOrder>,
          ),
          LocalOrder,
          PrefetchHooks Function()
        > {
  $$LocalOrdersTableTableManager(_$AppDatabase db, $LocalOrdersTable table)
    : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$LocalOrdersTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$LocalOrdersTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$LocalOrdersTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<int> id = const Value.absent(),
                Value<String> localId = const Value.absent(),
                Value<String?> cloudId = const Value.absent(),
                Value<String?> tableNumber = const Value.absent(),
                Value<String> status = const Value.absent(),
                Value<String> payload = const Value.absent(),
                Value<DateTime> createdAt = const Value.absent(),
                Value<DateTime> updatedAt = const Value.absent(),
              }) => LocalOrdersCompanion(
                id: id,
                localId: localId,
                cloudId: cloudId,
                tableNumber: tableNumber,
                status: status,
                payload: payload,
                createdAt: createdAt,
                updatedAt: updatedAt,
              ),
          createCompanionCallback:
              ({
                Value<int> id = const Value.absent(),
                required String localId,
                Value<String?> cloudId = const Value.absent(),
                Value<String?> tableNumber = const Value.absent(),
                required String status,
                required String payload,
                required DateTime createdAt,
                required DateTime updatedAt,
              }) => LocalOrdersCompanion.insert(
                id: id,
                localId: localId,
                cloudId: cloudId,
                tableNumber: tableNumber,
                status: status,
                payload: payload,
                createdAt: createdAt,
                updatedAt: updatedAt,
              ),
          withReferenceMapper: (p0) => p0
              .map((e) => (e.readTable(table), BaseReferences(db, table, e)))
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$LocalOrdersTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $LocalOrdersTable,
      LocalOrder,
      $$LocalOrdersTableFilterComposer,
      $$LocalOrdersTableOrderingComposer,
      $$LocalOrdersTableAnnotationComposer,
      $$LocalOrdersTableCreateCompanionBuilder,
      $$LocalOrdersTableUpdateCompanionBuilder,
      (
        LocalOrder,
        BaseReferences<_$AppDatabase, $LocalOrdersTable, LocalOrder>,
      ),
      LocalOrder,
      PrefetchHooks Function()
    >;
typedef $$SyncQueueTableCreateCompanionBuilder =
    SyncQueueCompanion Function({
      Value<int> id,
      required String localOrderId,
      Value<int> attempts,
      Value<String?> lastError,
      required DateTime nextRetryAt,
    });
typedef $$SyncQueueTableUpdateCompanionBuilder =
    SyncQueueCompanion Function({
      Value<int> id,
      Value<String> localOrderId,
      Value<int> attempts,
      Value<String?> lastError,
      Value<DateTime> nextRetryAt,
    });

class $$SyncQueueTableFilterComposer
    extends Composer<_$AppDatabase, $SyncQueueTable> {
  $$SyncQueueTableFilterComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnFilters<int> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get localOrderId => $composableBuilder(
    column: $table.localOrderId,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<int> get attempts => $composableBuilder(
    column: $table.attempts,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<String> get lastError => $composableBuilder(
    column: $table.lastError,
    builder: (column) => ColumnFilters(column),
  );

  ColumnFilters<DateTime> get nextRetryAt => $composableBuilder(
    column: $table.nextRetryAt,
    builder: (column) => ColumnFilters(column),
  );
}

class $$SyncQueueTableOrderingComposer
    extends Composer<_$AppDatabase, $SyncQueueTable> {
  $$SyncQueueTableOrderingComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  ColumnOrderings<int> get id => $composableBuilder(
    column: $table.id,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get localOrderId => $composableBuilder(
    column: $table.localOrderId,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<int> get attempts => $composableBuilder(
    column: $table.attempts,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<String> get lastError => $composableBuilder(
    column: $table.lastError,
    builder: (column) => ColumnOrderings(column),
  );

  ColumnOrderings<DateTime> get nextRetryAt => $composableBuilder(
    column: $table.nextRetryAt,
    builder: (column) => ColumnOrderings(column),
  );
}

class $$SyncQueueTableAnnotationComposer
    extends Composer<_$AppDatabase, $SyncQueueTable> {
  $$SyncQueueTableAnnotationComposer({
    required super.$db,
    required super.$table,
    super.joinBuilder,
    super.$addJoinBuilderToRootComposer,
    super.$removeJoinBuilderFromRootComposer,
  });
  GeneratedColumn<int> get id =>
      $composableBuilder(column: $table.id, builder: (column) => column);

  GeneratedColumn<String> get localOrderId => $composableBuilder(
    column: $table.localOrderId,
    builder: (column) => column,
  );

  GeneratedColumn<int> get attempts =>
      $composableBuilder(column: $table.attempts, builder: (column) => column);

  GeneratedColumn<String> get lastError =>
      $composableBuilder(column: $table.lastError, builder: (column) => column);

  GeneratedColumn<DateTime> get nextRetryAt => $composableBuilder(
    column: $table.nextRetryAt,
    builder: (column) => column,
  );
}

class $$SyncQueueTableTableManager
    extends
        RootTableManager<
          _$AppDatabase,
          $SyncQueueTable,
          SyncQueueData,
          $$SyncQueueTableFilterComposer,
          $$SyncQueueTableOrderingComposer,
          $$SyncQueueTableAnnotationComposer,
          $$SyncQueueTableCreateCompanionBuilder,
          $$SyncQueueTableUpdateCompanionBuilder,
          (
            SyncQueueData,
            BaseReferences<_$AppDatabase, $SyncQueueTable, SyncQueueData>,
          ),
          SyncQueueData,
          PrefetchHooks Function()
        > {
  $$SyncQueueTableTableManager(_$AppDatabase db, $SyncQueueTable table)
    : super(
        TableManagerState(
          db: db,
          table: table,
          createFilteringComposer: () =>
              $$SyncQueueTableFilterComposer($db: db, $table: table),
          createOrderingComposer: () =>
              $$SyncQueueTableOrderingComposer($db: db, $table: table),
          createComputedFieldComposer: () =>
              $$SyncQueueTableAnnotationComposer($db: db, $table: table),
          updateCompanionCallback:
              ({
                Value<int> id = const Value.absent(),
                Value<String> localOrderId = const Value.absent(),
                Value<int> attempts = const Value.absent(),
                Value<String?> lastError = const Value.absent(),
                Value<DateTime> nextRetryAt = const Value.absent(),
              }) => SyncQueueCompanion(
                id: id,
                localOrderId: localOrderId,
                attempts: attempts,
                lastError: lastError,
                nextRetryAt: nextRetryAt,
              ),
          createCompanionCallback:
              ({
                Value<int> id = const Value.absent(),
                required String localOrderId,
                Value<int> attempts = const Value.absent(),
                Value<String?> lastError = const Value.absent(),
                required DateTime nextRetryAt,
              }) => SyncQueueCompanion.insert(
                id: id,
                localOrderId: localOrderId,
                attempts: attempts,
                lastError: lastError,
                nextRetryAt: nextRetryAt,
              ),
          withReferenceMapper: (p0) => p0
              .map((e) => (e.readTable(table), BaseReferences(db, table, e)))
              .toList(),
          prefetchHooksCallback: null,
        ),
      );
}

typedef $$SyncQueueTableProcessedTableManager =
    ProcessedTableManager<
      _$AppDatabase,
      $SyncQueueTable,
      SyncQueueData,
      $$SyncQueueTableFilterComposer,
      $$SyncQueueTableOrderingComposer,
      $$SyncQueueTableAnnotationComposer,
      $$SyncQueueTableCreateCompanionBuilder,
      $$SyncQueueTableUpdateCompanionBuilder,
      (
        SyncQueueData,
        BaseReferences<_$AppDatabase, $SyncQueueTable, SyncQueueData>,
      ),
      SyncQueueData,
      PrefetchHooks Function()
    >;

class $AppDatabaseManager {
  final _$AppDatabase _db;
  $AppDatabaseManager(this._db);
  $$LocalOrdersTableTableManager get localOrders =>
      $$LocalOrdersTableTableManager(_db, _db.localOrders);
  $$SyncQueueTableTableManager get syncQueue =>
      $$SyncQueueTableTableManager(_db, _db.syncQueue);
}
