import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import 'dart:developer' as developer;

part 'customers_remote_datasource.g.dart';

@riverpod
CustomersRemoteDataSource customersRemoteDataSource(Ref ref) {
  return CustomersRemoteDataSource(ref.watch(productsDioProvider));
}

class CustomersRemoteDataSource {
  final Dio _dio;

  CustomersRemoteDataSource(this._dio);

  Future<Map<String, dynamic>?> getCustomerByPhone(String phone) async {
    try {
      final response = await _dio.get('/api/customers/phone/$phone');
      
      if (response.statusCode == 200) {
        if (response.data is Map) {
          // Se a resposta tiver um campo 'data', retorna o valor desse campo
          if (response.data['data'] != null) {
            return response.data['data'] as Map<String, dynamic>;
          }
          // Se a resposta for o objeto em si (como visto em alguns endpoints)
          return response.data as Map<String, dynamic>;
        }
        return null;
      }
      return null;
    } catch (e, stack) {
      // Se a resposta for 404, significa que o cliente não foi encontrado, o que é normal
      if (e is DioException && e.response?.statusCode == 404) {
        return null;
      }
      developer.log('Error loading customer by phone', error: e, stackTrace: stack, name: 'CustomersRemoteDataSource');
      rethrow;
    }
  }
}
