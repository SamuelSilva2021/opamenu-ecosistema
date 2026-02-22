import 'package:json_annotation/json_annotation.dart';
import '../enums/cash_movement_type.dart';

part 'cash_register_requests.g.dart';

@JsonSerializable()
class OpenCashShiftRequestDto {
  final double openingBalance;

  OpenCashShiftRequestDto({required this.openingBalance});

  factory OpenCashShiftRequestDto.fromJson(Map<String, dynamic> json) =>
      _$OpenCashShiftRequestDtoFromJson(json);

  Map<String, dynamic> toJson() => _$OpenCashShiftRequestDtoToJson(this);
}

@JsonSerializable()
class CloseCashShiftRequestDto {
  final double closingBalance;

  CloseCashShiftRequestDto({required this.closingBalance});

  factory CloseCashShiftRequestDto.fromJson(Map<String, dynamic> json) =>
      _$CloseCashShiftRequestDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CloseCashShiftRequestDtoToJson(this);
}

@JsonSerializable()
class AddCashMovementRequestDto {
  final CashMovementType type;
  final double amount;
  final String description;
  final String? orderId;

  AddCashMovementRequestDto({
    required this.type,
    required this.amount,
    required this.description,
    this.orderId,
  });

  factory AddCashMovementRequestDto.fromJson(Map<String, dynamic> json) =>
      _$AddCashMovementRequestDtoFromJson(json);

  Map<String, dynamic> toJson() => _$AddCashMovementRequestDtoToJson(this);
}
