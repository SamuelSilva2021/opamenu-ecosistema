import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../../../features/pos/data/models/paged_response_model.dart';
import '../../../../features/pos/domain/models/order_response_dto.dart';
import '../models/create_table_request_dto.dart';
import '../models/table_response_dto.dart';
import '../models/update_table_request_dto.dart';

part 'tables_remote_datasource.g.dart';

@riverpod
TablesRemoteDataSource tablesRemoteDataSource(Ref ref) {
  return TablesRemoteDataSource(ref.watch(productsDioProvider));
}

class TablesRemoteDataSource {
  final Dio _dio;

  TablesRemoteDataSource(this._dio);

  Future<PagedResponseModel<List<TableResponseDto>>> getTables({
    int page = 1,
    int pageSize = 10,
  }) async {
    try {
      final response = await _dio.get(
        '/api/tables',
        queryParameters: {
          'pageNumber': page,
          'pageSize': pageSize,
        },
      );

      return PagedResponseModel<List<TableResponseDto>>.fromJson(
        response.data,
        (json) {
          if (json is List) {
            return json
                .map((e) => TableResponseDto.fromJson(e as Map<String, dynamic>))
                .toList();
          }
          return [];
        },
      );
    } catch (e) {
      rethrow;
    }
  }

  Future<TableResponseDto> getTable(int id) async {
    try {
      final response = await _dio.get('/api/tables/$id');
      return TableResponseDto.fromJson(response.data);
    } catch (e) {
      rethrow;
    }
  }

  Future<TableResponseDto> createTable(CreateTableRequestDto dto) async {
    try {
      final response = await _dio.post(
        '/api/tables',
        data: dto.toJson(),
      );
      return TableResponseDto.fromJson(response.data);
    } catch (e) {
      rethrow;
    }
  }

  Future<TableResponseDto> updateTable(int id, UpdateTableRequestDto dto) async {
    try {
      final response = await _dio.put(
        '/api/tables/$id',
        data: dto.toJson(),
      );
      return TableResponseDto.fromJson(response.data);
    } catch (e) {
      rethrow;
    }
  }

  Future<bool> deleteTable(int id) async {
    try {
      final response = await _dio.delete('/api/tables/$id');
      return response.data as bool;
    } catch (e) {
      rethrow;
    }
  }

  Future<String> generateQrCode(int id) async {
    try {
      final response = await _dio.post('/api/tables/$id/qrcode');
      return response.data as String;
    } catch (e) {
      rethrow;
    }
  }

  Future<OrderResponseDto?> getActiveOrder(int id) async {
    try {
      final response = await _dio.get('/api/tables/$id/order');
      if (response.data is Map<String, dynamic> && response.data['data'] != null) {
          return OrderResponseDto.fromJson(response.data['data']);
      }
      return null;
    } catch (e) {
      if (e is DioException && e.response?.statusCode == 404) {
        return null;
      }
      rethrow;
    }
  }

  Future<OrderResponseDto> closeAccount(int id) async {
    try {
      final response = await _dio.post('/api/tables/$id/close');
      if (response.data is Map<String, dynamic> && response.data['data'] != null) {
          return OrderResponseDto.fromJson(response.data['data']);
      }
      return OrderResponseDto.fromJson(response.data);
    } catch (e) {
      rethrow;
    }
  }

}
