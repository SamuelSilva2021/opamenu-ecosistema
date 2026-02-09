import 'package:json_annotation/json_annotation.dart';

enum OrderStatus {
  @JsonValue(0)
  pending,
  @JsonValue(1)
  preparing,
  @JsonValue(2)
  ready,
  @JsonValue(3)
  outForDelivery,
  @JsonValue(4)
  delivered,
  @JsonValue(5)
  cancelled,
  @JsonValue(6)
  rejected;

  String get label {
    switch (this) {
      case OrderStatus.pending:
        return 'Pendente';
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
