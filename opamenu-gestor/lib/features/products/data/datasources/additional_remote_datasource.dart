
import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/constants/app_strings.dart';
import '../../../auth/data/models/api_response_model.dart';
import '../../domain/models/additional_group_model.dart';

part 'additional_remote_datasource.g.dart';

@riverpod
AdditionalRemoteDataSource additionalRemoteDataSource(Ref ref) {
  return AdditionalRemoteDataSource(ref.watch(productsDioProvider));
}

class AdditionalRemoteDataSource {
  final Dio _dio;

  AdditionalRemoteDataSource(this._dio);

  Future<List<AdditionalGroupModel>> getAdditionalGroups() async {
    final response = await _dio.get('/api/aditionalgroups');
    
    if (response.statusCode == 200) {
      // Check if response is directly a List (new backend behavior)
      if (response.data is List) {
        return (response.data as List)
            .map((e) => AdditionalGroupModel.fromJson(e as Map<String, dynamic>))
            .toList();
      }

      // Check if response is ApiResponseModel (old behavior or wrapped)
      if (response.data is Map<String, dynamic>) {
        try {
          final apiResponse = ApiResponseModel<List<AdditionalGroupModel>>.fromJson(
            response.data,
            (json) {
              if (json is List) {
                return json.map((e) => AdditionalGroupModel.fromJson(e as Map<String, dynamic>)).toList();
              }
              return [];
            },
          );

          if (apiResponse.succeeded && apiResponse.data != null) {
            return apiResponse.data!;
          } else {
            throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
          }
        } catch (_) {
          // Fallback
          if (response.data['data'] is List) {
             return (response.data['data'] as List)
                .map((e) => AdditionalGroupModel.fromJson(e as Map<String, dynamic>))
                .toList();
          }
          rethrow;
        }
      }

      throw Exception('Formato de resposta inesperado');
    } else {
      throw Exception('Failed to load additional groups: ${response.statusCode}');
    }
  }

  Future<AdditionalGroupModel> createAdditionalGroup(Map<String, dynamic> data) async {
    final response = await _dio.post('/api/aditionalgroups', data: data);
    
    if (response.statusCode == 200 || response.statusCode == 201) {
      final apiResponse = ApiResponseModel<AdditionalGroupModel>.fromJson(
        response.data,
        (json) => AdditionalGroupModel.fromJson(json as Map<String, dynamic>),
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      } else {
        throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
      }
    } else {
      throw Exception('Failed to create additional group: ${response.statusCode}');
    }
  }

  Future<AdditionalGroupModel> updateAdditionalGroup(String id, Map<String, dynamic> data) async {
    final response = await _dio.put('/api/aditionalgroups/$id', data: data);
    
    if (response.statusCode == 200) {
      final apiResponse = ApiResponseModel<AdditionalGroupModel>.fromJson(
        response.data,
        (json) => AdditionalGroupModel.fromJson(json as Map<String, dynamic>),
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      } else {
        throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
      }
    } else {
      throw Exception('Failed to update additional group: ${response.statusCode}');
    }
  }

  Future<void> deleteAdditionalGroup(String id) async {
    final response = await _dio.delete('/api/aditionalgroups/$id');
    
    if (response.statusCode != 200 && response.statusCode != 204) {
      throw Exception('Failed to delete additional group: ${response.statusCode}');
    }
  }

  // --- ITEMS (Aditionals) ---

  Future<void> createAdditional(Map<String, dynamic> data) async {
    final response = await _dio.post('/api/aditionals', data: data);
    
    if (response.statusCode != 200 && response.statusCode != 201) {
      throw Exception('Failed to create additional item: ${response.statusCode}');
    }
  }

  Future<void> updateAdditional(String id, Map<String, dynamic> data) async {
    final response = await _dio.put('/api/aditionals/$id', data: data);
    
    if (response.statusCode != 200) {
      throw Exception('Failed to update additional item: ${response.statusCode}');
    }
  }

  Future<void> deleteAdditional(String id) async {
    final response = await _dio.delete('/api/aditionals/$id');
    
    if (response.statusCode != 200 && response.statusCode != 204) {
      throw Exception('Failed to delete additional item: ${response.statusCode}');
    }
  }
}
