
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
    final response = await _dio.get('/api/additional-groups');
    
    if (response.statusCode == 200) {
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
    } else {
      throw Exception('Failed to load additional groups: ${response.statusCode}');
    }
  }

  Future<AdditionalGroupModel> createAdditionalGroup(Map<String, dynamic> data) async {
    final response = await _dio.post('/api/additional-groups', data: data);
    
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
    final response = await _dio.put('/api/additional-groups/$id', data: data);
    
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
    final response = await _dio.delete('/api/additional-groups/$id');
    
    if (response.statusCode != 200 && response.statusCode != 204) {
      throw Exception('Failed to delete additional group: ${response.statusCode}');
    }
  }
}
