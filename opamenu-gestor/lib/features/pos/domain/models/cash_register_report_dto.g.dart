// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'cash_register_report_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CashRegisterReportDto _$CashRegisterReportDtoFromJson(
  Map<String, dynamic> json,
) => CashRegisterReportDto(
  startDate: DateTime.parse(json['startDate'] as String),
  endDate: DateTime.parse(json['endDate'] as String),
  totalGrossSales: (json['totalGrossSales'] as num).toDouble(),
  totalNetSales: (json['totalNetSales'] as num).toDouble(),
  totalInflows: (json['totalInflows'] as num).toDouble(),
  totalOutflows: (json['totalOutflows'] as num).toDouble(),
  totalDiscrepancy: (json['totalDiscrepancy'] as num).toDouble(),
  salesByPaymentMethod: (json['salesByPaymentMethod'] as List<dynamic>)
      .map((e) => PaymentMethodSummaryDto.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$CashRegisterReportDtoToJson(
  CashRegisterReportDto instance,
) => <String, dynamic>{
  'startDate': instance.startDate.toIso8601String(),
  'endDate': instance.endDate.toIso8601String(),
  'totalGrossSales': instance.totalGrossSales,
  'totalNetSales': instance.totalNetSales,
  'totalInflows': instance.totalInflows,
  'totalOutflows': instance.totalOutflows,
  'totalDiscrepancy': instance.totalDiscrepancy,
  'salesByPaymentMethod': instance.salesByPaymentMethod,
};

PaymentMethodSummaryDto _$PaymentMethodSummaryDtoFromJson(
  Map<String, dynamic> json,
) => PaymentMethodSummaryDto(
  paymentMethod: $enumDecode(_$PaymentMethodEnumMap, json['paymentMethod']),
  paymentMethodName: json['paymentMethodName'] as String,
  totalAmount: (json['totalAmount'] as num).toDouble(),
  count: (json['count'] as num).toInt(),
);

Map<String, dynamic> _$PaymentMethodSummaryDtoToJson(
  PaymentMethodSummaryDto instance,
) => <String, dynamic>{
  'paymentMethod': _$PaymentMethodEnumMap[instance.paymentMethod]!,
  'paymentMethodName': instance.paymentMethodName,
  'totalAmount': instance.totalAmount,
  'count': instance.count,
};

const _$PaymentMethodEnumMap = {
  PaymentMethod.creditCard: 0,
  PaymentMethod.debitCard: 1,
  PaymentMethod.pix: 2,
  PaymentMethod.cash: 3,
  PaymentMethod.bankTransfer: 4,
  PaymentMethod.ticket: 5,
};
