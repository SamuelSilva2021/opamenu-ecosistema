import 'package:json_annotation/json_annotation.dart';
import '../enums/cash_shift_status.dart';
import 'cash_movement_response_dto.dart';

part 'cash_shift_response_dto.g.dart';

@JsonSerializable()
class CashShiftResponseDto {
  final String id;
  final String userId;
  final String? userName;
  final DateTime openedAt;
  final DateTime? closedAt;
  final double openingBalance;
  final double? closingBalance;
  final double expectedBalance;
  final CashShiftStatus status;
  final List<CashMovementResponseDto> movements;

  CashShiftResponseDto({
    required this.id,
    required this.userId,
    this.userName,
    required this.openedAt,
    this.closedAt,
    required this.openingBalance,
    this.closingBalance,
    required this.expectedBalance,
    required this.status,
    this.movements = const [],
  });

  factory CashShiftResponseDto.fromJson(Map<String, dynamic> json) =>
      _$CashShiftResponseDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CashShiftResponseDtoToJson(this);
}
