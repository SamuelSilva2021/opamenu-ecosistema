// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'dashboard_summary_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DashboardSummaryDto _$DashboardSummaryDtoFromJson(Map<String, dynamic> json) =>
    DashboardSummaryDto(
      totalRevenue: (json['totalRevenue'] as num).toDouble(),
      totalRevenueGrowth: (json['totalRevenueGrowth'] as num).toDouble(),
      ordersToday: (json['ordersToday'] as num).toInt(),
      ordersTodayGrowth: (json['ordersTodayGrowth'] as num).toDouble(),
      totalOrders: (json['totalOrders'] as num).toInt(),
      totalOrdersGrowth: (json['totalOrdersGrowth'] as num).toDouble(),
      activeCustomers: (json['activeCustomers'] as num).toInt(),
      activeCustomersGrowth: (json['activeCustomersGrowth'] as num).toDouble(),
      recentOrders: (json['recentOrders'] as List<dynamic>)
          .map((e) => RecentOrderDto.fromJson(e as Map<String, dynamic>))
          .toList(),
    );

Map<String, dynamic> _$DashboardSummaryDtoToJson(
  DashboardSummaryDto instance,
) => <String, dynamic>{
  'totalRevenue': instance.totalRevenue,
  'totalRevenueGrowth': instance.totalRevenueGrowth,
  'ordersToday': instance.ordersToday,
  'ordersTodayGrowth': instance.ordersTodayGrowth,
  'totalOrders': instance.totalOrders,
  'totalOrdersGrowth': instance.totalOrdersGrowth,
  'activeCustomers': instance.activeCustomers,
  'activeCustomersGrowth': instance.activeCustomersGrowth,
  'recentOrders': instance.recentOrders,
};
