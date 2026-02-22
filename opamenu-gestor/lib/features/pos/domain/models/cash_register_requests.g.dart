// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_register_requests.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

OpenCashShiftRequestDto _$OpenCashShiftRequestDtoFromJson(
  Map<String, dynamic> json,
) => OpenCashShiftRequestDto(
  openingBalance: (json['openingBalance'] as num).toDouble(),
);

Map<String, dynamic> _$OpenCashShiftRequestDtoToJson(
  OpenCashShiftRequestDto instance,
) => <String, dynamic>{'openingBalance': instance.openingBalance};

CloseCashShiftRequestDto _$CloseCashShiftRequestDtoFromJson(
  Map<String, dynamic> json,
) => CloseCashShiftRequestDto(
  closingBalance: (json['closingBalance'] as num).toDouble(),
);

Map<String, dynamic> _$CloseCashShiftRequestDtoToJson(
  CloseCashShiftRequestDto instance,
) => <String, dynamic>{'closingBalance': instance.closingBalance};

AddCashMovementRequestDto _$AddCashMovementRequestDtoFromJson(
  Map<String, dynamic> json,
) => AddCashMovementRequestDto(
  type: $enumDecode(_$CashMovementTypeEnumMap, json['type']),
  amount: (json['amount'] as num).toDouble(),
  description: json['description'] as String,
  orderId: json['orderId'] as String?,
);

Map<String, dynamic> _$AddCashMovementRequestDtoToJson(
  AddCashMovementRequestDto instance,
) => <String, dynamic>{
  'type': _$CashMovementTypeEnumMap[instance.type]!,
  'amount': instance.amount,
  'description': instance.description,
  'orderId': instance.orderId,
};

const _$CashMovementTypeEnumMap = {
  CashMovementType.opening: 1,
  CashMovementType.orderPayment: 2,
  CashMovementType.inbound: 3,
  CashMovementType.outbound: 4,
  CashMovementType.reversed: 5,
  CashMovementType.closing: 6,
};
