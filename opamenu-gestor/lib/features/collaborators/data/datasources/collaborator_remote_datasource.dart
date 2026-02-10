import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:opamenu_gestor/core/network/api_client.dart';
import 'package:opamenu_gestor/core/constants/app_strings.dart';
import 'package:opamenu_gestor/features/auth/data/models/api_response_model.dart';
import 'package:opamenu_gestor/features/collaborators/domain/models/collaborator_model.dart';

part 'collaborator_remote_datasource.g.dart';

@riverpod
CollaboratorRemoteDataSource collaboratorRemoteDataSource(Ref ref) {
  return CollaboratorRemoteDataSource(ref.watch(productsDioProvider));
}

class CollaboratorRemoteDataSource {
  final Dio _dio;

  CollaboratorRemoteDataSource(this._dio);

  Future<List<CollaboratorModel>> getCollaborators() async {
    final response = await _dio.get('/api/collaborators');
    return _parseListResponse(response);
  }

  Future<CollaboratorModel> createCollaborator(Map<String, dynamic> data) async {
    final response = await _dio.post('/api/collaborators', data: data);
    return _parseSingleResponse(response);
  }

  Future<CollaboratorModel> updateCollaborator(String id, Map<String, dynamic> data) async {
    final response = await _dio.put('/api/collaborators/$id', data: data);
    return _parseSingleResponse(response);
  }

  Future<bool> deleteCollaborator(String id) async {
    final response = await _dio.delete('/api/collaborators/$id');
    if (response.statusCode == 200) {
      return true;
    }
    throw Exception('Falha ao excluir colaborador');
  }

  List<CollaboratorModel> _parseListResponse(Response response) {
    if (response.statusCode == 200) {
      final apiResponse = ApiResponseModel<List<CollaboratorModel>>.fromJson(
        response.data,
        (json) {
          if (json is List) {
            return json.map((e) => CollaboratorModel.fromJson(e as Map<String, dynamic>)).toList();
          }
          return [];
        },
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      }
      
      throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
    }
    throw Exception('Failed to load collaborators');
  }

  CollaboratorModel _parseSingleResponse(Response response) {
    if (response.statusCode == 200 || response.statusCode == 201) {
      final apiResponse = ApiResponseModel<CollaboratorModel>.fromJson(
        response.data,
        (json) => CollaboratorModel.fromJson(json as Map<String, dynamic>),
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      }
      
      throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
    }
    throw Exception('Failed to process collaborator request');
  }
}
