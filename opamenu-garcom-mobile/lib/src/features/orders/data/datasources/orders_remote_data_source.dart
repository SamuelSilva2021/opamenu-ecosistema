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
import '../models/create_table_order_request_model.dart';
import '../models/order_model.dart';
import 'orders_remote_data_source_contract.dart';

class OrdersRemoteDataSource implements OrdersRemoteDataSourceContract {
  final ApiHttpClientContract client;
  final AppEnvironment environment;

  const OrdersRemoteDataSource({
    required this.client,
    required this.environment,
  });

  @override
  Future<Result<OrderModel>> createTableOrder({
    required String accessToken,
    required CreateTableOrderRequestModel request,
  }) async {
    final uri = environment.coreBaseUri.resolve('/api/orders');
    final response = await client.postJson(
      uri,
      headers: {
        'Authorization': 'Bearer $accessToken',
      },
      body: request.toJson(),
    );

    if (response is FailureResult<ApiHttpResponse>) {
      return FailureResult(response.failure);
    }

    final http = (response as SuccessResult<ApiHttpResponse>).value;
    if (http.statusCode == 401 || http.statusCode == 403) {
      return const FailureResult(UnauthorizedFailure('Não autorizado'));
    }

    final decoded = _decodeJson(http.body);
    if (decoded == null) {
      return FailureResult(UnexpectedFailure('Resposta inválida do servidor'));
    }

    final map = decoded is Map ? decoded.cast<String, Object?>() : null;
    if (map == null) return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));

    final succeeded = map['succeeded'] ?? map['Succeeded'];
    if (succeeded is bool && !succeeded) {
      return FailureResult(ValidationFailure(_extractErrors(map) ?? 'Falha ao criar pedido'));
    }

    final data = map['data'] ?? map['Data'];
    if (data is Map) {
      return SuccessResult(OrderModel.fromJson(data.cast<String, Object?>()));
    }

    return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));
  }

  Object? _decodeJson(String body) {
    try {
      return jsonDecode(body);
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
