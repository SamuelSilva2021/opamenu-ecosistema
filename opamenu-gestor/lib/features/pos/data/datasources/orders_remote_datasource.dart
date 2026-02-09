import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../domain/models/create_order_request_dto.dart';
import '../../domain/models/order_response_dto.dart';
import '../models/paged_response_model.dart';
import 'dart:developer' as developer;

part 'orders_remote_datasource.g.dart';

@riverpod
OrdersRemoteDataSource ordersRemoteDataSource(Ref ref) {
  return OrdersRemoteDataSource(ref.watch(productsDioProvider));
}

class OrdersRemoteDataSource {
  final Dio _dio;

  OrdersRemoteDataSource(this._dio);

  Future<PagedResponseModel<List<OrderResponseDto>>> getOrders({int page = 1, int pageSize = 10}) async {
    try {
      final response = await _dio.get(
        '/api/orders',
        queryParameters: {
          'page': page,
          'pageSize': pageSize,
        },
      );

      if (response.statusCode == 200) {
        if (response.data is Map<String, dynamic>) {
          try {
            final apiResponse = PagedResponseModel<List<OrderResponseDto>>.fromJson(
              response.data,
              (json) {
                if (json is List) {
                  return json
                      .map((e) => OrderResponseDto.fromJson(e as Map<String, dynamic>))
                      .toList();
                }
                return [];
              },
            );

            if (apiResponse.succeeded) {
              return apiResponse;
            }
          } catch (e) {
            developer.log('Error parsing PagedResponseModel', error: e, name: 'OrdersRemoteDataSource');
          }
        } else if (response.data is List) {
          // API retornou uma lista simples de pedidos (sem metadados de paginação)
          final list = (response.data as List)
              .map((e) => OrderResponseDto.fromJson(e as Map<String, dynamic>))
              .toList();

          return PagedResponseModel<List<OrderResponseDto>>(
            succeeded: true,
            data: list,
            errors: null,
            code: response.statusCode,
            totalItems: list.length,
            totalPages: 1,
            currentPage: page,
            pageSize: pageSize,
          );
        }

        throw Exception('Unexpected response format or failed request');
      } else {
        throw Exception('Failed to load orders: ${response.statusCode}');
      }
    } catch (e, stack) {
      developer.log('Error loading orders', error: e, stackTrace: stack, name: 'OrdersRemoteDataSource');
      rethrow;
    }
  }

  Future<OrderResponseDto> createOrder(CreateOrderRequestDto order) async {
    try {
      final response = await _dio.post(
        '/api/orders',
        data: order.toJson(),
      );

      if (response.statusCode != 200 && response.statusCode != 201) {
        throw Exception('Failed to create order: ${response.statusCode} - ${response.statusMessage}');
      }
      
      if (response.data is Map<String, dynamic> && response.data['data'] != null) {
          return OrderResponseDto.fromJson(response.data['data']);
      }
      return OrderResponseDto.fromJson(response.data);
    } catch (e, stack) {
      developer.log('Error creating order', error: e, stackTrace: stack, name: 'OrdersRemoteDataSource');
      rethrow;
    }
  }

  Future<OrderResponseDto> addItemsToOrder(String orderId, List<CreateOrderItemRequestDto> items) async {
    try {
      final response = await _dio.post(
        '/api/orders/$orderId/items',
        data: items.map((e) => e.toJson()).toList(),
      );
      if (response.data is Map<String, dynamic> && response.data['data'] != null) {
         return OrderResponseDto.fromJson(response.data['data']);
      }
      return OrderResponseDto.fromJson(response.data);
    } catch (e, stack) {
      developer.log('Error adding items to order', error: e, stackTrace: stack, name: 'OrdersRemoteDataSource');
      rethrow;
    }
  }
}
