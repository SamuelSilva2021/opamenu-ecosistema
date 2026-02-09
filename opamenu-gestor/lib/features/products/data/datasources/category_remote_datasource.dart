
import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/constants/app_strings.dart';
import '../../../auth/data/models/api_response_model.dart';
import '../../domain/models/category_model.dart';

part 'category_remote_datasource.g.dart';

@riverpod
CategoryRemoteDataSource categoryRemoteDataSource(Ref ref) {
  return CategoryRemoteDataSource(ref.watch(productsDioProvider));
}

class CategoryRemoteDataSource {
  final Dio _dio;

  CategoryRemoteDataSource(this._dio);

  Future<List<CategoryModel>> getCategories() async {
    final response = await _dio.get('/api/categories');
    
    if (response.statusCode == 200) {
      final apiResponse = ApiResponseModel<List<CategoryModel>>.fromJson(
        response.data,
        (json) {
          if (json is List) {
            return json.map((e) => CategoryModel.fromJson(e as Map<String, dynamic>)).toList();
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
      throw Exception('Failed to load categories: ${response.statusCode}');
    }
  }

  Future<CategoryModel> createCategory(Map<String, dynamic> data) async {
    final response = await _dio.post('/api/categories', data: data);
    
    if (response.statusCode == 200 || response.statusCode == 201) {
      final apiResponse = ApiResponseModel<CategoryModel>.fromJson(
        response.data,
        (json) => CategoryModel.fromJson(json as Map<String, dynamic>),
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      } else {
        throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
      }
    } else {
      throw Exception('Failed to create category: ${response.statusCode}');
    }
  }

  Future<CategoryModel> updateCategory(String id, Map<String, dynamic> data) async {
    final response = await _dio.put('/api/categories/$id', data: data);
    
    if (response.statusCode == 200) {
      final apiResponse = ApiResponseModel<CategoryModel>.fromJson(
        response.data,
        (json) => CategoryModel.fromJson(json as Map<String, dynamic>),
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      } else {
        throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
      }
    } else {
      throw Exception('Failed to update category: ${response.statusCode}');
    }
  }

  Future<void> deleteCategory(String id) async {
    final response = await _dio.delete('/api/categories/$id');
    
    if (response.statusCode != 200 && response.statusCode != 204) {
      throw Exception('Failed to delete category: ${response.statusCode}');
    }
  }
}
