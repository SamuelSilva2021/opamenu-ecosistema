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
  
  final int activeCustomers;
  final double activeCustomersGrowth;
  
  final List<RecentOrderDto> recentOrders;

  DashboardSummaryDto({
    required this.totalRevenue,
    required this.totalRevenueGrowth,
    required this.ordersToday,
    required this.ordersTodayGrowth,
    required this.totalOrders,
    required this.totalOrdersGrowth,
    required this.activeCustomers,
    required this.activeCustomersGrowth,
    required this.recentOrders,
  });

  factory DashboardSummaryDto.fromJson(Map<String, dynamic> json) => _$DashboardSummaryDtoFromJson(json);
  Map<String, dynamic> toJson() => _$DashboardSummaryDtoToJson(this);
}
