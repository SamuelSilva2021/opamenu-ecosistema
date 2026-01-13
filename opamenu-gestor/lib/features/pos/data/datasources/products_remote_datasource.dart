import 'dart:developer' as developer;
import 'package:dio/dio.dart';
import 'package:opamenu_gestor/core/constants/app_strings.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../domain/models/product_model.dart';
import '../../../auth/data/models/api_response_model.dart';

part 'products_remote_datasource.g.dart';

@riverpod
ProductsRemoteDataSource productsRemoteDataSource(Ref ref) {
  return ProductsRemoteDataSource(ref.watch(productsDioProvider));
}

class ProductsRemoteDataSource {
  final Dio _dio;

  ProductsRemoteDataSource(this._dio);

  Future<List<ProductModel>> getProducts() async {
    try {
      final response = await _dio.get('/api/products');
      
      if (response.statusCode == 200) {
        // Verifica se a resposta é um Map (provável ApiResponseModel) ou Lista direta
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
          } catch (e) {
            // Fallback: Tenta acessar 'data' diretamente se falhar o parse do wrapper completo
            if (response.data['data'] is List) {
               return (response.data['data'] as List)
                   .map((e) => ProductModel.fromJson(e as Map<String, dynamic>))
                   .toList();
            }
            rethrow;
          }
        } else if (response.data is List) {
          return (response.data as List)
              .map((json) => ProductModel.fromJson(json as Map<String, dynamic>))
              .toList();
        } else {
           throw Exception('${AppStrings.unexpectedResponseFormat}: ${response.data.runtimeType}');
        }
      } else {
        throw Exception('${AppStrings.failedToLoadProducts}: ${response.statusCode}');
      }
    } catch (e, stack) {
      developer.log(AppStrings.failedToLoadProducts, error: e, stackTrace: stack, name: 'ProductsRemoteDataSource');
      throw Exception('${AppStrings.failedToLoadProducts}: $e');
    }
  }
}
