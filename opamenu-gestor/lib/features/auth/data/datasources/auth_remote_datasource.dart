
import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../models/api_response_model.dart';
import '../models/login_request_model.dart';
import '../models/login_response_model.dart';

part 'auth_remote_datasource.g.dart';

@riverpod
AuthRemoteDataSource authRemoteDataSource(Ref ref) {
  return AuthRemoteDataSource(ref.watch(dioProvider));
}

class AuthRemoteDataSource {
  final Dio _dio;

  AuthRemoteDataSource(this._dio);

  Future<LoginResponseModel> login(LoginRequestModel request) async {
    try {
      final response = await _dio.post(
        '/api/auth/login',
        data: request.toJson(),
      );

      final apiResponse = ApiResponseModel<LoginResponseModel>.fromJson(
        response.data,
        (json) => LoginResponseModel.fromJson(json as Map<String, dynamic>),
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      } else {
        throw Exception(apiResponse.errors?.first.message ?? 'Unknown error');
      }
    } on DioException catch (e) {
      if (e.response?.data != null) {
        try {
          final apiResponse = ApiResponseModel<LoginResponseModel>.fromJson(
            e.response!.data,
            (json) => LoginResponseModel.fromJson(json as Map<String, dynamic>),
          );
          throw Exception(apiResponse.errors?.first.message ?? e.message);
        } catch (_) {
          throw Exception(e.message);
        }
      }
      throw Exception(e.message);
    }
  }
}
