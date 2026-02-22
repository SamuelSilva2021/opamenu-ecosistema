import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../domain/models/cash_shift_response_dto.dart';
import '../../domain/models/cash_movement_response_dto.dart';
import '../../domain/models/cash_register_requests.dart';
import 'dart:developer' as developer;

part 'cash_register_remote_datasource.g.dart';

@riverpod
CashRegisterRemoteDataSource cashRegisterRemoteDataSource(Ref ref) {
  return CashRegisterRemoteDataSource(ref.watch(productsDioProvider));
}

class CashRegisterRemoteDataSource {
  final Dio _dio;

  CashRegisterRemoteDataSource(this._dio);

  Future<CashShiftResponseDto?> getActiveShift() async {
    try {
      final response = await _dio.get('/api/cash-register/active');
      developer.log('Active shift response: status=${response.statusCode}, data=${response.data}', name: 'CashRegisterRemoteDataSource');
      
      if (response.statusCode == 200) {
        final data = _extractData(response.data);
        if (data == null) return null;
        return CashShiftResponseDto.fromJson(data);
      }
      return null;
    } catch (e, stack) {
      developer.log('Error getting active shift', error: e, stackTrace: stack, name: 'CashRegisterRemoteDataSource');
      if (e is DioException && e.response?.statusCode == 404) {
        return null; // Not found often means no active shift
      }
      rethrow;
    }
  }

  Future<CashShiftResponseDto> openShift(OpenCashShiftRequestDto request) async {
    try {
      final response = await _dio.post(
        '/api/cash-register/open',
        data: request.toJson(),
      );

      if (response.statusCode != 200 && response.statusCode != 201) {
        throw Exception('Failed to open shift: ${response.statusCode}');
      }
      
      final data = _extractData(response.data);
      if (data == null) throw Exception('Resposta da API está vazia');
      return CashShiftResponseDto.fromJson(data);
    } catch (e, stack) {
      developer.log('Error opening shift', error: e, stackTrace: stack, name: 'CashRegisterRemoteDataSource');
      rethrow;
    }
  }

  Future<CashShiftResponseDto> closeShift(CloseCashShiftRequestDto request) async {
    try {
      final response = await _dio.post(
        '/api/cash-register/close',
        data: request.toJson(),
      );

      if (response.statusCode != 200) {
        throw Exception('Failed to close shift: ${response.statusCode}');
      }
      
      final data = _extractData(response.data);
      if (data == null) throw Exception('Resposta da API está vazia');
      return CashShiftResponseDto.fromJson(data);
    } catch (e, stack) {
      developer.log('Error closing shift', error: e, stackTrace: stack, name: 'CashRegisterRemoteDataSource');
      rethrow;
    }
  }

  Future<CashMovementResponseDto> addMovement(AddCashMovementRequestDto request) async {
    try {
      final response = await _dio.post(
        '/api/cash-register/movement',
        data: request.toJson(),
      );

      if (response.statusCode != 200 && response.statusCode != 201) {
        throw Exception('Failed to add movement: ${response.statusCode}');
      }
      
      final data = _extractData(response.data);
      if (data == null) throw Exception('API returned empty data');
      return CashMovementResponseDto.fromJson(data);
    } catch (e, stack) {
      developer.log('Error adding movement', error: e, stackTrace: stack, name: 'CashRegisterRemoteDataSource');
      rethrow;
    }
  }

  String _getErrorMessage(DioException e) {
    if (e.response?.data is List && (e.response?.data as List).isNotEmpty) {
      final errors = e.response?.data as List;
      final firstError = errors.first;
      if (firstError is Map && firstError.containsKey('message')) {
        return firstError['message'];
      }
    }
    return e.message ?? 'Erro desconhecido na API';
  }

  Map<String, dynamic>? _extractData(dynamic data) {
    if (data == null) return null;
    if (data is Map<String, dynamic>) {
      if (data.containsKey('data')) {
        final innerData = data['data'];
        if (innerData == null) return null;
        if (innerData is Map<String, dynamic>) return innerData;
        throw Exception('Conteúdo "data" não é um objeto (Map)');
      }
      return data;
    }
    return null;
  }
}
