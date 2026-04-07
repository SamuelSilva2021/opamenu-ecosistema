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
import '../models/create_tab_item_request_model.dart';
import '../models/tab_item_model.dart';
import '../models/tab_model.dart';
import '../models/update_tab_request_model.dart';
import 'tabs_remote_data_source_contract.dart';

class TabsRemoteDataSource implements TabsRemoteDataSourceContract {
  final ApiHttpClientContract client;
  final AppEnvironment environment;

  const TabsRemoteDataSource({
    required this.client,
    required this.environment,
  });

  @override
  Future<Result<List<TabModel>>> getTabs({
    required String accessToken,
    required String tableId,
  }) async {
    final uri = environment.coreBaseUri.resolve('/api/tables/$tableId/tabs');
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
      return const SuccessResult(<TabModel>[]);
    }

    final decoded = _decodeJson(http.body);
    if (decoded == null) {
      if (http.statusCode < 200 || http.statusCode >= 300) {
        return FailureResult(
          UnexpectedFailure('Erro HTTP ${http.statusCode} ao carregar comandas'),
        );
      }
      return FailureResult(UnexpectedFailure('Resposta inválida do servidor'));
    }

    final list = _extractList(decoded);
    if (list != null) {
      final models = list
          .whereType<Map>()
          .map((e) => TabModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false);
      return SuccessResult(models);
    }

    final map = decoded is Map ? decoded.cast<String, Object?>() : null;
    if (map == null) {
      final bodySnippet = http.body.length > 300 ? http.body.substring(0, 300) : http.body;
      return FailureResult(
        UnexpectedFailure('Formato de resposta inesperado (HTTP ${http.statusCode}): $bodySnippet'),
      );
    }

    final succeeded = map['succeeded'] ?? map['Succeeded'];
    if (succeeded is bool && !succeeded) {
      return FailureResult(ValidationFailure(_extractErrors(map) ?? 'Falha ao carregar comandas'));
    }

    final rawData = map['data'] ?? map['Data'];
    if (succeeded is bool && succeeded && rawData == null) {
      return const SuccessResult(<TabModel>[]);
    }

    final bodySnippet = http.body.length > 300 ? http.body.substring(0, 300) : http.body;
    return FailureResult(
      UnexpectedFailure('Formato de resposta inesperado (HTTP ${http.statusCode}): $bodySnippet'),
    );
  }

  @override
  Future<Result<TabModel>> openTab({
    required String accessToken,
    required String tableId,
    String? name,
  }) async {
    final uri = environment.coreBaseUri.resolve('/api/tables/$tableId/tabs');
    final response = await client.postJson(
      uri,
      headers: {
        'Authorization': 'Bearer $accessToken',
      },
      body: {
        if (name != null && name.trim().isNotEmpty) 'name': name.trim(),
      },
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
      return FailureResult(ValidationFailure(_extractErrors(map) ?? 'Falha ao abrir comanda'));
    }

    final data = map['data'] ?? map['Data'];
    if (data is Map) {
      return SuccessResult(TabModel.fromJson(data.cast<String, Object?>()));
    }

    return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));
  }

  @override
  Future<Result<bool>> deleteTab({
    required String accessToken,
    required String tabId,
  }) async {
    final uri = environment.coreBaseUri.resolve('/api/tabs/$tabId');
    final response = await client.delete(
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

    final normalizedBody = http.body.trim().toLowerCase();
    if (http.statusCode >= 200 && http.statusCode < 300) {
      if (normalizedBody == 'true') return const SuccessResult(true);
      if (normalizedBody == 'false') return const SuccessResult(false);
    }

    final decoded = _decodeJson(http.body);
    if (decoded == null) {
      if (http.statusCode >= 200 && http.statusCode < 300) {
        return const SuccessResult(true);
      }
      return FailureResult(UnexpectedFailure('Erro HTTP ${http.statusCode} ao excluir comanda'));
    }

    if (decoded is bool) return SuccessResult(decoded);

    final map = decoded is Map ? decoded.cast<String, Object?>() : null;
    if (map == null) return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));

    final succeeded = map['succeeded'] ?? map['Succeeded'];
    if (succeeded is bool && !succeeded) {
      return FailureResult(ValidationFailure(_extractErrors(map) ?? 'Falha ao excluir comanda'));
    }

    final data = map['data'] ?? map['Data'];
    if (data is bool) return SuccessResult(data);

    if (http.statusCode >= 200 && http.statusCode < 300) {
      return const SuccessResult(true);
    }

    return FailureResult(UnexpectedFailure('Erro HTTP ${http.statusCode} ao excluir comanda'));
  }

  @override
  Future<Result<List<TabItemModel>>> getTabItems({
    required String accessToken,
    required String tabId,
  }) async {
    final uri = environment.coreBaseUri.resolve('/api/tabs/$tabId/items');
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
      return const SuccessResult(<TabItemModel>[]);
    }

    final decoded = _decodeJson(http.body);
    if (decoded == null) {
      if (http.statusCode < 200 || http.statusCode >= 300) {
        return FailureResult(
          UnexpectedFailure('Erro HTTP ${http.statusCode} ao carregar itens da comanda'),
        );
      }
      return FailureResult(UnexpectedFailure('Resposta inválida do servidor'));
    }

    final list = _extractList(decoded);
    if (list != null) {
      final models = list
          .whereType<Map>()
          .map((e) => TabItemModel.fromJson(e.cast<String, Object?>()))
          .toList(growable: false);
      return SuccessResult(models);
    }

    final map = decoded is Map ? decoded.cast<String, Object?>() : null;
    if (map == null) return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));

    final succeeded = map['succeeded'] ?? map['Succeeded'];
    if (succeeded is bool && !succeeded) {
      return FailureResult(ValidationFailure(_extractErrors(map) ?? 'Falha ao carregar itens'));
    }

    final rawData = map['data'] ?? map['Data'];
    if (succeeded is bool && succeeded && rawData == null) {
      return const SuccessResult(<TabItemModel>[]);
    }

    return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));
  }

  @override
  Future<Result<bool>> addTabItems({
    required String accessToken,
    required String tabId,
    required List<CreateTabItemRequestModel> items,
  }) async {
    final uri = environment.coreBaseUri.resolve('/api/tabs/$tabId/items');
    final response = await client.postJsonList(
      uri,
      headers: {
        'Authorization': 'Bearer $accessToken',
      },
      body: items.map((e) => e.toJson()).toList(growable: false),
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
      if (http.statusCode >= 200 && http.statusCode < 300) {
        return const SuccessResult(true);
      }
      return FailureResult(
        UnexpectedFailure('Erro HTTP ${http.statusCode} ao incluir itens na comanda'),
      );
    }

    final map = decoded is Map ? decoded.cast<String, Object?>() : null;
    if (map == null) return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));

    final succeeded = map['succeeded'] ?? map['Succeeded'];
    if (succeeded is bool && !succeeded) {
      return FailureResult(ValidationFailure(_extractErrors(map) ?? 'Falha ao incluir itens'));
    }

    if (http.statusCode >= 200 && http.statusCode < 300) {
      return const SuccessResult(true);
    }

    return FailureResult(UnexpectedFailure('Erro HTTP ${http.statusCode} ao incluir itens na comanda'));
  }

  @override
  Future<Result<TabModel>> updateTab({
    required String accessToken,
    required String tabId,
    required UpdateTabRequestModel request,
  }) async {
    final uri = environment.coreBaseUri.resolve('/api/tabs/$tabId');
    final response = await client.putJson(
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
      return FailureResult(ValidationFailure(_extractErrors(map) ?? 'Falha ao atualizar comanda'));
    }

    final data = map['data'] ?? map['Data'];
    if (data is Map) {
      return SuccessResult(TabModel.fromJson(data.cast<String, Object?>()));
    }

    return FailureResult(UnexpectedFailure('Formato de resposta inesperado'));
  }

  List<Object?>? _extractList(Object decoded) {
    if (decoded is List) return decoded;
    if (decoded is! Map) return null;

    final values = decoded[r'$values'];
    if (values is List) return values;

    final data = decoded['data'] ?? decoded['Data'];
    if (data is List) return data;
    if (data is Map) {
      final dataValues = data[r'$values'];
      if (dataValues is List) return dataValues;
      final items = data['items'] ?? data['Items'];
      if (items is List) return items;
      if (items is Map) {
        final itemsValues = items[r'$values'];
        if (itemsValues is List) return itemsValues;
      }
    }

    return null;
  }

  Object? _decodeJson(String body) {
    try {
      final normalized = body.startsWith('\uFEFF') ? body.substring(1) : body;
      final decoded = jsonDecode(normalized);
      if (decoded is! String) return decoded;

      final inner = decoded.trim();
      if (inner.isEmpty) return decoded;
      final looksJsonObject = inner.startsWith('{') && inner.endsWith('}');
      final looksJsonArray = inner.startsWith('[') && inner.endsWith(']');
      final looksJsonLiteral = inner == 'true' || inner == 'false' || inner == 'null';
      if (!looksJsonObject && !looksJsonArray && !looksJsonLiteral) return decoded;

      try {
        return jsonDecode(inner);
      } catch (_) {
        return decoded;
      }
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
