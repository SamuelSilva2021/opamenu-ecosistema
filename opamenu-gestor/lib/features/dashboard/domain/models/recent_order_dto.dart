import 'package:json_annotation/json_annotation.dart';

part 'recent_order_dto.g.dart';

@JsonSerializable()
class RecentOrderDto {
  final int id;
  final String customerName;
  final double amount;
  final DateTime createdAt;

  RecentOrderDto({
    required this.id,
    required this.customerName,
    required this.amount,
    required this.createdAt,
  });

  factory RecentOrderDto.fromJson(Map<String, dynamic> json) => _$RecentOrderDtoFromJson(json);
  Map<String, dynamic> toJson() => _$RecentOrderDtoToJson(this);
}
