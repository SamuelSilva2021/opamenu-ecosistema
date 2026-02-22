import 'package:json_annotation/json_annotation.dart';

enum PaymentMethod {
  @JsonValue(0)
  creditCard,
  @JsonValue(1)
  debitCard,
  @JsonValue(2)
  pix,
  @JsonValue(3)
  cash,
  @JsonValue(4)
  bankTransfer,
  @JsonValue(5)
  ticket;

  String get displayName {
    switch (this) {
      case PaymentMethod.creditCard:
        return 'Cartão de Crédito';
      case PaymentMethod.debitCard:
        return 'Cartão de Débito';
      case PaymentMethod.pix:
        return 'Pix';
      case PaymentMethod.cash:
        return 'Dinheiro';
      case PaymentMethod.bankTransfer:
        return 'Transferência Bancária';
      case PaymentMethod.ticket:
        return 'Boleto';
    }
  }
}
