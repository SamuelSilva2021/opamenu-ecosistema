import 'dart:convert';

import '../../../../app/app_environment.dart';
import '../../../../core/data/http/api_http_client_contract.dart';
import '../../../../core/data/http/api_http_response.dart';
import '../../../../core/domain/failures/unauthorized_failure.dart';
import '../../../../core/domain/failures/unexpected_failure.dart';
import '../../../../core/domain/failures/validation_failure.dart';
import '../../../../core/domain/result/failure_result.dart';
import '../../../../core/domain/result/result.dart';
import '../../../../core/domain/result/success_result.dart';
import '../models/table_model.dart';
import 'tables_remote_data_source_contract.dart';

class TablesRemoteDataSource implements TablesRemoteDataSourceContract {
  final ApiHttpClientContract client;
  final AppEnvironment environment;

  const TablesRemoteDataSource({
    required this.client,
    required this.environment,
  });

  @override
  Future<Result<List<TableModel>>> getTables({required String accessToken}) async {
    final uri = environment.coreBaseUri.resolve('/api/tables?pageNumber=1&pageSize=200');
    final response = await client.get(
      uri,
      headers: {
        'Authorization': 'Bearer $accessToken',
      },
    );

    if (response is FailureResult<ApiHttpResponse>) {
      return FailureResult(response.failure);
    }

    final http = (response as SuccessResult<ApiHttpResponse>).value;
    if (http.statusCode == 401 || http.statusCode == 403) {
      return const FailureResult(UnauthorizedFailure('Não autorizado'));
    }
    if (http.statusCode == 204) {
      return const SuccessResult(<TableModel>[]);
    }

    final decoded = _decodeJson(http.body);
    if (decoded == null) {
      if (http.statusCode < 200 || http.statusCode >= 300) {
        return FailureResult(
          UnexpectedFailure('Erro HTTP ${http.statusCode} ao carregar mesas'),
        );
      }
      return FailureResult(UnexpectedFailure('Resposta inválida do servidor'));
    }

    if (decoded is List) {
      final models = decoded
          .whereType<Map>()
          .map((e) => TableModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false);
      return SuccessResult(models);
    }

    final map = decoded is Map ? decoded.cast<String, Object?>() : null;
    if (map == null) {
      return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));
    }
    final succeeded = map['succeeded'];
    if (succeeded is bool && !succeeded) {
      return FailureResult(ValidationFailure(_extractErrors(map) ?? 'Falha ao carregar mesas'));
    }

    final data = map['data'];
    if (data is List) {
      final models = data
          .whereType<Map>()
          .map((e) => TableModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false);
      return SuccessResult(models);
    }

    return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));
  }

  Object? _decodeJson(String body) {
    try {
      final normalized = body.startsWith('\uFEFF') ? body.substring(1) : body;
      return jsonDecode(normalized);
    } catch (_) {
      return null;
    }
  }

  String? _extractErrors(Map<String, Object?> json) {
    final errors = json['errors'] ?? json['Errors'];
    if (errors is! List) return null;
    final messages = errors
        .whereType<Map>()
        .map((e) => e['message']?.toString() ?? '')
        .where((e) => e.trim().isNotEmpty)
        .toList(growable: false);
    if (messages.isEmpty) return null;
    return messages.join('\n');
  }
}
