import 'package:json_annotation/json_annotation.dart';

enum CashShiftStatus {
  @JsonValue(1)
  open,
  @JsonValue(2)
  closed;

  static CashShiftStatus fromValue(dynamic value) {
    if (value == 1 || value == 'open' || value == 'Open') {
      return CashShiftStatus.open;
    }
    return CashShiftStatus.closed;
  }
}
