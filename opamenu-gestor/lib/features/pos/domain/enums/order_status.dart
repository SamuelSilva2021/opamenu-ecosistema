import 'package:json_annotation/json_annotation.dart';

enum OrderStatus {
  @JsonValue(0)
  pending,
  @JsonValue(1)
  confirmed,
  @JsonValue(2)
  preparing,
  @JsonValue(3)
  ready,
  @JsonValue(4)
  outForDelivery,
  @JsonValue(5)
  delivered,
  @JsonValue(6)
  cancelled,
  @JsonValue(7)
  rejected;

  String get label {
    switch (this) {
      case OrderStatus.pending:
        return 'Pendente';
      case OrderStatus.confirmed:
        return 'Confirmado';
      case OrderStatus.preparing:
        return 'Em Preparo';
      case OrderStatus.ready:
        return 'Pronto';
      case OrderStatus.outForDelivery:
        return 'Saiu para Entrega';
      case OrderStatus.delivered:
        return 'Entregue';
      case OrderStatus.cancelled:
        return 'Cancelado';
      case OrderStatus.rejected:
        return 'Rejeitado';
    }
  }
}
