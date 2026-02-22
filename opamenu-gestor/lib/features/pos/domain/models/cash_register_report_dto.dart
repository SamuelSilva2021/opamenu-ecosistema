import 'package:json_annotation/json_annotation.dart';
import '../enums/payment_method.dart';

part 'cash_register_report_dto.g.dart';

@JsonSerializable()
class CashRegisterReportDto {
  final DateTime startDate;
  final DateTime endDate;
  final double totalGrossSales;
  final double totalNetSales;
  final double totalInflows;
  final double totalOutflows;
  final double totalDiscrepancy;
  final List<PaymentMethodSummaryDto> salesByPaymentMethod;

  CashRegisterReportDto({
    required this.startDate,
    required this.endDate,
    required this.totalGrossSales,
    required this.totalNetSales,
    required this.totalInflows,
    required this.totalOutflows,
    required this.totalDiscrepancy,
    required this.salesByPaymentMethod,
  });

  factory CashRegisterReportDto.fromJson(Map<String, dynamic> json) =>
      _$CashRegisterReportDtoFromJson(json);

  Map<String, dynamic> toJson() => _$CashRegisterReportDtoToJson(this);
}

@JsonSerializable()
class PaymentMethodSummaryDto {
  final PaymentMethod paymentMethod;
  final String paymentMethodName;
  final double totalAmount;
  final int count;

  PaymentMethodSummaryDto({
    required this.paymentMethod,
    required this.paymentMethodName,
    required this.totalAmount,
    required this.count,
  });

  factory PaymentMethodSummaryDto.fromJson(Map<String, dynamic> json) =>
      _$PaymentMethodSummaryDtoFromJson(json);

  Map<String, dynamic> toJson() => _$PaymentMethodSummaryDtoToJson(this);
}
