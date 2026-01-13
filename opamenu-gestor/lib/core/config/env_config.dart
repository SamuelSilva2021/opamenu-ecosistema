import 'package:flutter/foundation.dart';

class EnvConfig {
  static String get authBaseUrl {
    if (kReleaseMode) {
      return 'https://auth.opamenu.com.br';
    } else {
      if (defaultTargetPlatform == TargetPlatform.android) {
        return 'https://10.0.2.2:7019';
      }
      return 'https://localhost:7019';
    }
  }

  static String get apiBaseUrl {
    if (kReleaseMode) {
      return 'https://api.opamenu.com.br';
    } else {
      if (defaultTargetPlatform == TargetPlatform.android) {
        return 'https://10.0.2.2:7243';
      }
      return 'https://localhost:7243';
    }
  }
}
