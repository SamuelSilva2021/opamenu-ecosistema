import 'package:json_annotation/json_annotation.dart';
import '../enums/cash_movement_type.dart';

part 'cash_movement_response_dto.g.dart';

@JsonSerializable()
class CashMovementResponseDto {
  final String id;
  final CashMovementType type;
  final double amount;
  final String description;
  final int? paymentMethod; // We can map this to an enum later if needed
  final String? orderId;
  final int? orderNumber;
  final DateTime createdAt;

  CashMovementResponseDto({
    required this.id,
    required this.type,
    required this.amount,
    required this.description,
    this.paymentMethod,
    this.orderId,
    this.orderNumber,
    required this.createdAt,
  });

  factory CashMovementResponseDto.fromJson(Map<String, dynamic> json) =>
      _$CashMovementResponseDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CashMovementResponseDtoToJson(this);
}
