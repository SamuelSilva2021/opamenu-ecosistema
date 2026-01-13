import 'dart:developer' as developer;
import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../domain/models/dashboard_summary_dto.dart';
import '../../../auth/data/models/api_response_model.dart';

part 'dashboard_remote_datasource.g.dart';

@riverpod
DashboardRemoteDatasource dashboardRemoteDatasource(Ref ref) {
  return DashboardRemoteDatasource(ref.watch(productsDioProvider));
}

class DashboardRemoteDatasource {
  final Dio _dio;

  DashboardRemoteDatasource(this._dio);

  Future<DashboardSummaryDto> getSummary() async {
    try {
      // Tentando rota hierárquica comum para dashboards
      final response = await _dio.get('/api/dashboard/summary');
      
      if (response.statusCode == 200) {
        if (response.data is Map<String, dynamic>) {
          if (response.data.containsKey('succeeded') || response.data.containsKey('data')) {
             try {
                final apiResponse = ApiResponseModel<DashboardSummaryDto>.fromJson(
                  response.data,
                  (json) => DashboardSummaryDto.fromJson(json as Map<String, dynamic>),
                );

                if (apiResponse.succeeded && apiResponse.data != null) {
                  return apiResponse.data!;
                }
             } catch(_) {
               // Fallback
             }
             
             if (response.data['data'] != null) {
               return DashboardSummaryDto.fromJson(response.data['data']);
             }
          }
          
          // Tenta parse direto
          return DashboardSummaryDto.fromJson(response.data);
        }
        
        throw Exception('Unexpected response format');
      } else {
        throw Exception('Failed to load dashboard summary: ${response.statusCode}');
      }
    } catch (e, stack) {
      developer.log('Error loading dashboard summary', error: e, stackTrace: stack, name: 'DashboardRemoteDatasource');
      rethrow;
    }
  }
}
