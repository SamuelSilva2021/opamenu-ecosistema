import 'package:flutter/services.dart';

class PhoneUtils {
  static String sanitize(String input) {
    return input.replaceAll(RegExp(r'[^0-9]'), '');
  }

  static String formatDisplay(String input) {
    final digits = sanitize(input);
    if (digits.isEmpty) return '';

    final buf = StringBuffer();
    final len = digits.length;

    buf.write('(');
    for (var i = 0; i < len && i < 2; i++) {
      buf.write(digits[i]);
    }
    if (len >= 2) buf.write(') ');

    if (len <= 10) {
      // Fixo: (dd) dddd-dddd
      final firstCount = len > 2 ? (len - 2 >= 4 ? 4 : len - 2) : 0;
      for (var i = 0; i < firstCount; i++) {
        buf.write(digits[2 + i]);
      }
      if (len - 2 > 4) {
        buf.write('-');
        final remaining = len - 6;
        for (var i = 0; i < remaining && i < 4; i++) {
          buf.write(digits[6 + i]);
        }
      }
    } else {
      // Celular: (dd) ddddd-dddd
      final firstCount = len > 2 ? (len - 2 >= 5 ? 5 : len - 2) : 0;
      for (var i = 0; i < firstCount; i++) {
        buf.write(digits[2 + i]);
      }
      if (len - 2 > 5) {
        buf.write('-');
        final remaining = len - 7;
        for (var i = 0; i < remaining && i < 4; i++) {
          buf.write(digits[7 + i]);
        }
      }
    }

    return buf.toString();
  }
}

class PhoneMaskTextInputFormatter extends TextInputFormatter {
  @override
  TextEditingValue formatEditUpdate(TextEditingValue oldValue, TextEditingValue newValue) {
    final digits = PhoneUtils.sanitize(newValue.text);
    final formatted = PhoneUtils.formatDisplay(digits);
    return TextEditingValue(
      text: formatted,
      selection: TextSelection.collapsed(offset: formatted.length),
    );
  }
}

