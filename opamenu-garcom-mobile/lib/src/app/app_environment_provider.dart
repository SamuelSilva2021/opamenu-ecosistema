import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'app_environment.dart';

class AppEnvironmentProvider {
  static final Provider<AppEnvironment> provider = Provider((ref) {
    return AppEnvironment.current();
  });
}

