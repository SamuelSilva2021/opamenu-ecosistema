import 'package:json_annotation/json_annotation.dart';
import '../../../../core/utils/json_parser_utils.dart';

part 'recent_order_dto.g.dart';

@JsonSerializable()
class RecentOrderDto {
  final String id;
  final String customerName;
  
  @JsonKey(fromJson: JsonParserUtils.toDouble)
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
