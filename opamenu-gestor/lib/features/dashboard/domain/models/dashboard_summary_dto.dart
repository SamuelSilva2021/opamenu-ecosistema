import 'package:json_annotation/json_annotation.dart';
import 'recent_order_dto.dart';

part 'dashboard_summary_dto.g.dart';

@JsonSerializable()
class DashboardSummaryDto {
  final double totalRevenue;
  final double totalRevenueGrowth;
  
  final int ordersToday;
  final double ordersTodayGrowth;
  
  final int totalOrders;
  final double totalOrdersGrowth;

  final double averageTicket;
  
  final int activeCustomers;
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
  final double total;

  DailySaleDto({required this.date, required this.total});

  factory DailySaleDto.fromJson(Map<String, dynamic> json) => _$DailySaleDtoFromJson(json);
  Map<String, dynamic> toJson() => _$DailySaleDtoToJson(this);
}

@JsonSerializable()
class CategorySaleDto {
  final String categoryName;
  final double total;
  final int quantity;

  CategorySaleDto({required this.categoryName, required this.total, required this.quantity});

  factory CategorySaleDto.fromJson(Map<String, dynamic> json) => _$CategorySaleDtoFromJson(json);
  Map<String, dynamic> toJson() => _$CategorySaleDtoToJson(this);
}
