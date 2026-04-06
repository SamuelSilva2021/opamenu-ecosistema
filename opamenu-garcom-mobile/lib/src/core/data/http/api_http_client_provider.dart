import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../app/app_environment_provider.dart';
import 'api_http_client.dart';
import 'api_http_client_contract.dart';

class ApiHttpClientProvider {
  static final Provider<ApiHttpClientContract> provider = Provider((ref) {
    final env = ref.watch(AppEnvironmentProvider.provider);
    return ApiHttpClient(env);
  });
}
