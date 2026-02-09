
import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/constants/app_strings.dart';
import '../../../auth/data/models/api_response_model.dart';
import '../../domain/models/user_model.dart';
import '../../domain/models/role_model.dart';

part 'users_remote_datasource.g.dart';

@riverpod
UsersRemoteDataSource usersRemoteDataSource(Ref ref) {
  return UsersRemoteDataSource(ref.watch(dioProvider));
}

class UsersRemoteDataSource {
  final Dio _dio;

  UsersRemoteDataSource(this._dio);

  Future<List<UserModel>> getUsers() async {
    final response = await _dio.get('/api/users');
    
    if (response.statusCode == 200) {
      final apiResponse = ApiResponseModel<List<UserModel>>.fromJson(
        response.data,
        (json) {
          if (json is List) {
            return json.map((e) => UserModel.fromJson(e as Map<String, dynamic>)).toList();
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
      throw Exception('Failed to load users: ${response.statusCode}');
    }
  }

  Future<List<RoleModel>> getRoles() async {
    final response = await _dio.get('/api/roles');
    
    if (response.statusCode == 200) {
      final apiResponse = ApiResponseModel<List<RoleModel>>.fromJson(
        response.data,
        (json) {
          if (json is List) {
            return json.map((e) => RoleModel.fromJson(e as Map<String, dynamic>)).toList();
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
      throw Exception('Failed to load roles: ${response.statusCode}');
    }
  }

  Future<void> createUser(Map<String, dynamic> data) async {
    final response = await _dio.post('/api/users', data: data);
    
    if (response.statusCode != 200 && response.statusCode != 201) {
      throw Exception('Failed to create user: ${response.statusCode}');
    }
  }

  Future<void> updateUser(String id, Map<String, dynamic> data) async {
    final response = await _dio.put('/api/users/$id', data: data);
    
    if (response.statusCode != 200) {
      throw Exception('Failed to update user: ${response.statusCode}');
    }
  }

  Future<void> deleteUser(String id) async {
    final response = await _dio.delete('/api/users/$id');
    
    if (response.statusCode != 200 && response.statusCode != 204) {
      throw Exception('Failed to delete user: ${response.statusCode}');
    }
  }
}
