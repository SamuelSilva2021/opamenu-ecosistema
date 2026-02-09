
import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../../../core/constants/app_strings.dart';
import '../../../auth/data/models/api_response_model.dart';
import '../../../pos/domain/models/product_model.dart';

part 'product_remote_datasource.g.dart';

@riverpod
ProductRemoteDataSource productRemoteDataSource(Ref ref) {
  return ProductRemoteDataSource(ref.watch(productsDioProvider));
}

class ProductRemoteDataSource {
  final Dio _dio;

  ProductRemoteDataSource(this._dio);

  Future<List<ProductModel>> getProducts() async {
    final response = await _dio.get('/api/products');
    
    if (response.statusCode == 200) {
      if (response.data is List) {
        return (response.data as List)
            .map((e) => ProductModel.fromJson(e as Map<String, dynamic>))
            .toList();
      }

      if (response.data is Map<String, dynamic>) {
        try {
          final apiResponse = ApiResponseModel<List<ProductModel>>.fromJson(
            response.data,
            (json) {
              if (json is List) {
                return json.map((e) => ProductModel.fromJson(e as Map<String, dynamic>)).toList();
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
          if (response.data['data'] is List) {
             return (response.data['data'] as List)
                .map((e) => ProductModel.fromJson(e as Map<String, dynamic>))
                .toList();
          }
          rethrow;
        }
      }
      
      throw Exception('Formato de resposta inesperado');
    } else {
      throw Exception('Failed to load products: ${response.statusCode}');
    }
  }

  Future<ProductModel> createProduct(Map<String, dynamic> data) async {
    final response = await _dio.post('/api/products', data: data);
    
    if (response.statusCode == 200 || response.statusCode == 201) {
      final apiResponse = ApiResponseModel<ProductModel>.fromJson(
        response.data,
        (json) => ProductModel.fromJson(json as Map<String, dynamic>),
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      } else {
        throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
      }
    } else {
      throw Exception('Failed to create product: ${response.statusCode}');
    }
  }

  Future<ProductModel> updateProduct(String id, Map<String, dynamic> data) async {
    final response = await _dio.put('/api/products/$id', data: data);
    
    if (response.statusCode == 200) {
      final apiResponse = ApiResponseModel<ProductModel>.fromJson(
        response.data,
        (json) => ProductModel.fromJson(json as Map<String, dynamic>),
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        return apiResponse.data!;
      } else {
        throw Exception(apiResponse.errors?.firstOrNull?.message ?? AppStrings.unknownErrorFromApi);
      }
    } else {
      throw Exception('Failed to update product: ${response.statusCode}');
    }
  }

  Future<void> deleteProduct(String id) async {
    final response = await _dio.delete('/api/products/$id');
    
    if (response.statusCode != 200 && response.statusCode != 204) {
      throw Exception('Failed to delete product: ${response.statusCode}');
    }
  }
}
