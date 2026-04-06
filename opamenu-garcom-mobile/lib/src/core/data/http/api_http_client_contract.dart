import '../../domain/result/result.dart';
import 'api_http_response.dart';

abstract class ApiHttpClientContract {
  Future<Result<ApiHttpResponse>> get(Uri uri, {Map<String, String>? headers});

  Future<Result<ApiHttpResponse>> postJson(
    Uri uri, {
    required Map<String, Object?> body,
    Map<String, String>? headers,
  });
}

