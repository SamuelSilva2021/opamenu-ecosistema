import 'dart:convert';
import 'dart:io';

import '../../domain/failures/network_failure.dart';
import '../../domain/result/failure_result.dart';
import '../../domain/result/result.dart';
import '../../domain/result/success_result.dart';
import '../../../app/app_environment.dart';
import 'api_http_client_contract.dart';
import 'api_http_response.dart';

class ApiHttpClient implements ApiHttpClientContract {
  final AppEnvironment environment;

  const ApiHttpClient(this.environment);

  @override
  Future<Result<ApiHttpResponse>> get(Uri uri, {Map<String, String>? headers}) {
    return _send('GET', uri, headers: headers);
  }

  @override
  Future<Result<ApiHttpResponse>> postJson(
    Uri uri, {
    required Map<String, Object?> body,
    Map<String, String>? headers,
  }) {
    return _send(
      'POST',
      uri,
      headers: {
        'Content-Type': 'application/json',
        ...?headers,
      },
      body: jsonEncode(body),
    );
  }

  Future<Result<ApiHttpResponse>> _send(
    String method,
    Uri uri, {
    Map<String, String>? headers,
    String? body,
  }) async {
    final client = HttpClient();
    if (environment.allowInsecureCertificates) {
      client.badCertificateCallback = (certificate, host, port) => true;
    }

    try {
      final request = await client.openUrl(method, uri);
      headers?.forEach(request.headers.set);
      if (body != null) {
        request.write(body);
      }

      final response = await request.close();
      final responseBody = await response.transform(utf8.decoder).join();
      final responseHeaders = <String, String>{};
      response.headers.forEach((name, values) {
        responseHeaders[name] = values.join(',');
      });

      return SuccessResult(
        ApiHttpResponse(
          statusCode: response.statusCode,
          headers: responseHeaders,
          body: responseBody,
        ),
      );
    } on SocketException catch (e) {
      return FailureResult(NetworkFailure(e.message));
    } on HandshakeException catch (e) {
      return FailureResult(NetworkFailure(e.message));
    } on HttpException catch (e) {
      return FailureResult(NetworkFailure(e.message));
    } catch (e) {
      return FailureResult(NetworkFailure(e.toString()));
    } finally {
      client.close(force: true);
    }
  }
}
