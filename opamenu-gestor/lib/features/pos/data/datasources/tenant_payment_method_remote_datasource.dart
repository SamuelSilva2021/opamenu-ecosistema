import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../domain/models/tenant_payment_method_model.dart';
import '../../../../core/constants/app_strings.dart';
import '../../../auth/data/models/api_response_model.dart';

part 'tenant_payment_method_remote_datasource.g.dart';

@riverpod
TenantPaymentMethodRemoteDataSource tenantPaymentMethodRemoteDataSource(Ref ref) {
  return TenantPaymentMethodRemoteDataSource(ref.watch(productsDioProvider));
}

class TenantPaymentMethodRemoteDataSource {
  final Dio _dio;

  TenantPaymentMethodRemoteDataSource(this._dio);

  Future<List<TenantPaymentMethodModel>> getPaymentMethods() async {
    final response = await _dio.get('/api/tenant-payment-methods');

    if (response.statusCode == 200) {
      if (response.data is List) {
        return (response.data as List)
            .map((e) => TenantPaymentMethodModel.fromJson(e as Map<String, dynamic>))
            .toList();
      }

      if (response.data is Map<String, dynamic>) {
        try {
          final apiResponse = ApiResponseModel<List<TenantPaymentMethodModel>>.fromJson(
            response.data,
            (json) {
              if (json is List) {
                return json
                    .map((e) => TenantPaymentMethodModel.fromJson(e as Map<String, dynamic>))
                    .toList();
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
          // Fallback for wrapped data in 'data' field but not full ApiResponseModel
           if (response.data['data'] is List) {
             return (response.data['data'] as List)
                .map((e) => TenantPaymentMethodModel.fromJson(e as Map<String, dynamic>))
                .toList();
          }
          rethrow;
        }
      }
      
      throw Exception('Unexpected response format');
    } else {
      throw Exception('Failed to load payment methods: ${response.statusCode}');
    }
  }
}
