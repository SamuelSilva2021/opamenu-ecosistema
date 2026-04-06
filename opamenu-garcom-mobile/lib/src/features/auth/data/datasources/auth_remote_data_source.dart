import 'dart:convert';

import '../../../../app/app_environment.dart';
import '../../../../core/data/http/api_http_client_contract.dart';
import '../../../../core/data/http/api_http_response.dart';
import '../../../../core/data/models/api_response_model.dart';
import '../../../../core/domain/failures/unauthorized_failure.dart';
import '../../../../core/domain/failures/unexpected_failure.dart';
import '../../../../core/domain/failures/validation_failure.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../models/auth_tokens_model.dart';
import '../models/user_info_model.dart';
import 'auth_remote_data_source_contract.dart';

class AuthRemoteDataSource implements AuthRemoteDataSourceContract {
  final ApiHttpClientContract client;
  final AppEnvironment environment;

  const AuthRemoteDataSource({
    required this.client,
    required this.environment,
  });

  @override
  Future<Result<AuthTokensModel>> login({
    required String usernameOrEmail,
    required String password,
  }) async {
    final uri = environment.authBaseUri.resolve('/api/auth/login');
    final response = await client.postJson(
      uri,
      body: {
        'usernameOrEmail': usernameOrEmail,
        'password': password,
      },
    );

    if (response is FailureResult<ApiHttpResponse>) {
      return FailureResult(response.failure);
    }
    final http = (response as SuccessResult).value;
    return _parseApiResponse(http.statusCode, http.body, AuthTokensModel.fromJson);
  }

  @override
  Future<Result<UserInfoModel>> me({required String accessToken}) async {
    final uri = environment.authBaseUri.resolve('/api/auth/me');
    final response = await client.get(
      uri,
      headers: {
        'Authorization': 'Bearer $accessToken',
      },
    );

    if (response is FailureResult<ApiHttpResponse>) {
      return FailureResult(response.failure);
    }
    final http = (response as SuccessResult).value;
    return _parseApiResponse(http.statusCode, http.body, UserInfoModel.fromJson);
  }

  Future<Result<T>> _parseApiResponse<T>(
    int statusCode,
    String body,
    T Function(Map<String, Object?> json) dataParser,
  ) async {
    if (statusCode == 401 || statusCode == 403) {
      return const FailureResult(UnauthorizedFailure('Não autorizado'));
    }

    final decoded = _decodeJson(body);
    if (decoded == null) {
      return FailureResult(UnexpectedFailure('Resposta inválida do servidor'));
    }

    final apiResponse = ApiResponseModel<T>.fromJson(decoded, dataParser: dataParser);
    if (!apiResponse.succeeded || apiResponse.data == null) {
      final message = apiResponse.errors.isEmpty
          ? 'Falha na operação'
          : apiResponse.errors.map((e) => e.message).where((e) => e.isNotEmpty).join('\n');
      return FailureResult(ValidationFailure(message));
    }

    return SuccessResult(apiResponse.data as T);
  }

  Map<String, Object?>? _decodeJson(String body) {
    try {
      final decoded = jsonDecode(body);
      if (decoded is Map<String, Object?>) return decoded;
      if (decoded is Map) return decoded.cast<String, Object?>();
      return null;
    } catch (_) {
      return null;
    }
  }
}
