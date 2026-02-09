// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'dashboard_summary_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DashboardSummaryDto _$DashboardSummaryDtoFromJson(Map<String, dynamic> json) =>
    DashboardSummaryDto(
      totalRevenue: JsonParserUtils.toDouble(json['totalRevenue']),
      totalRevenueGrowth: JsonParserUtils.toDouble(json['totalRevenueGrowth']),
      ordersToday: JsonParserUtils.toInt(json['ordersToday']),
      ordersTodayGrowth: JsonParserUtils.toDouble(json['ordersTodayGrowth']),
      totalOrders: JsonParserUtils.toInt(json['totalOrders']),
      totalOrdersGrowth: JsonParserUtils.toDouble(json['totalOrdersGrowth']),
      averageTicket: JsonParserUtils.toDouble(json['averageTicket']),
      activeCustomers: JsonParserUtils.toInt(json['activeCustomers']),
      activeCustomersGrowth: JsonParserUtils.toDouble(
        json['activeCustomersGrowth'],
      ),
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
  total: JsonParserUtils.toDouble(json['total']),
);

Map<String, dynamic> _$DailySaleDtoToJson(DailySaleDto instance) =>
    <String, dynamic>{'date': instance.date, 'total': instance.total};

CategorySaleDto _$CategorySaleDtoFromJson(Map<String, dynamic> json) =>
    CategorySaleDto(
      categoryName: json['categoryName'] as String,
      total: JsonParserUtils.toDouble(json['total']),
      quantity: JsonParserUtils.toInt(json['quantity']),
    );

Map<String, dynamic> _$CategorySaleDtoToJson(CategorySaleDto instance) =>
    <String, dynamic>{
      'categoryName': instance.categoryName,
      'total': instance.total,
      'quantity': instance.quantity,
    };
