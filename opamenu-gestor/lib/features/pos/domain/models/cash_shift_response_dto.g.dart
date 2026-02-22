// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_shift_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CashShiftResponseDto _$CashShiftResponseDtoFromJson(
  Map<String, dynamic> json,
) => CashShiftResponseDto(
  id: json['id'] as String,
  userId: json['userId'] as String,
  userName: json['userName'] as String?,
  openedAt: DateTime.parse(json['openedAt'] as String),
  closedAt: json['closedAt'] == null
      ? null
      : DateTime.parse(json['closedAt'] as String),
  openingBalance: (json['openingBalance'] as num).toDouble(),
  closingBalance: (json['closingBalance'] as num?)?.toDouble(),
  expectedBalance: (json['expectedBalance'] as num).toDouble(),
  status: $enumDecode(_$CashShiftStatusEnumMap, json['status']),
  movements:
      (json['movements'] as List<dynamic>?)
          ?.map(
            (e) => CashMovementResponseDto.fromJson(e as Map<String, dynamic>),
          )
          .toList() ??
      const [],
);

Map<String, dynamic> _$CashShiftResponseDtoToJson(
  CashShiftResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'userId': instance.userId,
  'userName': instance.userName,
  'openedAt': instance.openedAt.toIso8601String(),
  'closedAt': instance.closedAt?.toIso8601String(),
  'openingBalance': instance.openingBalance,
  'closingBalance': instance.closingBalance,
  'expectedBalance': instance.expectedBalance,
  'status': _$CashShiftStatusEnumMap[instance.status]!,
  'movements': instance.movements,
};

const _$CashShiftStatusEnumMap = {
  CashShiftStatus.open: 1,
  CashShiftStatus.closed: 2,
};
