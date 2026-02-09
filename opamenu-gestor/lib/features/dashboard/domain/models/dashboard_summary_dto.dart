import 'package:json_annotation/json_annotation.dart';
import '../../../../core/utils/json_parser_utils.dart';
import 'recent_order_dto.dart';

part 'dashboard_summary_dto.g.dart';

@JsonSerializable()
class DashboardSummaryDto {
  @JsonKey(fromJson: JsonParserUtils.toDouble)
  final double totalRevenue;
  
  @JsonKey(fromJson: JsonParserUtils.toDouble)
  final double totalRevenueGrowth;
  
  @JsonKey(fromJson: JsonParserUtils.toInt)
  final int ordersToday;
  
  @JsonKey(fromJson: JsonParserUtils.toDouble)
  final double ordersTodayGrowth;
  
  @JsonKey(fromJson: JsonParserUtils.toInt)
  final int totalOrders;
  
  @JsonKey(fromJson: JsonParserUtils.toDouble)
  final double totalOrdersGrowth;

  @JsonKey(fromJson: JsonParserUtils.toDouble)
  final double averageTicket;
  
  @JsonKey(fromJson: JsonParserUtils.toInt)
  final int activeCustomers;
  
  @JsonKey(fromJson: JsonParserUtils.toDouble)
  final double activeCustomersGrowth;
  
  final List<RecentOrderDto> recentOrders;
  final List<DailySaleDto> dailySales;
  final List<CategorySaleDto> categorySales;

  DashboardSummaryDto({
    required this.totalRevenue,
    required this.totalRevenueGrowth,
    required this.ordersToday,
    required this.ordersTodayGrowth,
    required this.totalOrders,
    required this.totalOrdersGrowth,
    required this.averageTicket,
    required this.activeCustomers,
    required this.activeCustomersGrowth,
    required this.recentOrders,
    required this.dailySales,
    required this.categorySales,
  });

  factory DashboardSummaryDto.fromJson(Map<String, dynamic> json) => _$DashboardSummaryDtoFromJson(json);
  Map<String, dynamic> toJson() => _$DashboardSummaryDtoToJson(this);
}

@JsonSerializable()
class DailySaleDto {
  final String date;
  
  @JsonKey(fromJson: JsonParserUtils.toDouble)
  final double total;

  DailySaleDto({required this.date, required this.total});

  factory DailySaleDto.fromJson(Map<String, dynamic> json) => _$DailySaleDtoFromJson(json);
  Map<String, dynamic> toJson() => _$DailySaleDtoToJson(this);
}

@JsonSerializable()
class CategorySaleDto {
  final String categoryName;
  
  @JsonKey(fromJson: JsonParserUtils.toDouble)
  final double total;
  
  @JsonKey(fromJson: JsonParserUtils.toInt)
  final int quantity;

  CategorySaleDto({required this.categoryName, required this.total, required this.quantity});

  factory CategorySaleDto.fromJson(Map<String, dynamic> json) => _$CategorySaleDtoFromJson(json);
  Map<String, dynamic> toJson() => _$CategorySaleDtoToJson(this);
}
