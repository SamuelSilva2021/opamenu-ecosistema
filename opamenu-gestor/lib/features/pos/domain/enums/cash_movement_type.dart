import 'package:json_annotation/json_annotation.dart';

enum CashMovementType {
  @JsonValue(1)
  opening,
  @JsonValue(2)
  orderPayment,
  @JsonValue(3)
  inbound,
  @JsonValue(4)
  outbound,
  @JsonValue(5)
  reversed,
  @JsonValue(6)
  closing;

  static CashMovementType fromValue(int value) {
    return CashMovementType.values.elementAt(value - 1);
  }
}
