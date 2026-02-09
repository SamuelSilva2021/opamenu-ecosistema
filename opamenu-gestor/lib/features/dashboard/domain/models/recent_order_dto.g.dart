// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'recent_order_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

RecentOrderDto _$RecentOrderDtoFromJson(Map<String, dynamic> json) =>
    RecentOrderDto(
      id: json['id'] as String,
      customerName: json['customerName'] as String,
      amount: JsonParserUtils.toDouble(json['amount']),
      createdAt: DateTime.parse(json['createdAt'] as String),
    );

Map<String, dynamic> _$RecentOrderDtoToJson(RecentOrderDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'customerName': instance.customerName,
      'amount': instance.amount,
      'createdAt': instance.createdAt.toIso8601String(),
    };
