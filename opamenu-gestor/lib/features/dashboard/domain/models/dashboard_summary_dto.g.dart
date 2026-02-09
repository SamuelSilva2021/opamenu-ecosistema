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
      averageTicket: (json['averageTicket'] as num).toDouble(),
      activeCustomers: (json['activeCustomers'] as num).toInt(),
      activeCustomersGrowth: (json['activeCustomersGrowth'] as num).toDouble(),
      recentOrders: (json['recentOrders'] as List<dynamic>)
          .map((e) => RecentOrderDto.fromJson(e as Map<String, dynamic>))
          .toList(),
      dailySales: (json['dailySales'] as List<dynamic>)
          .map((e) => DailySaleDto.fromJson(e as Map<String, dynamic>))
          .toList(),
      categorySales: (json['categorySales'] as List<dynamic>)
          .map((e) => CategorySaleDto.fromJson(e as Map<String, dynamic>))
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
  'averageTicket': instance.averageTicket,
  'activeCustomers': instance.activeCustomers,
  'activeCustomersGrowth': instance.activeCustomersGrowth,
  'recentOrders': instance.recentOrders,
  'dailySales': instance.dailySales,
  'categorySales': instance.categorySales,
};

DailySaleDto _$DailySaleDtoFromJson(Map<String, dynamic> json) => DailySaleDto(
  date: json['date'] as String,
  total: (json['total'] as num).toDouble(),
);

Map<String, dynamic> _$DailySaleDtoToJson(DailySaleDto instance) =>
    <String, dynamic>{'date': instance.date, 'total': instance.total};

CategorySaleDto _$CategorySaleDtoFromJson(Map<String, dynamic> json) =>
    CategorySaleDto(
      categoryName: json['categoryName'] as String,
      total: (json['total'] as num).toDouble(),
      quantity: (json['quantity'] as num).toInt(),
    );

Map<String, dynamic> _$CategorySaleDtoToJson(CategorySaleDto instance) =>
    <String, dynamic>{
      'categoryName': instance.categoryName,
      'total': instance.total,
      'quantity': instance.quantity,
    };
