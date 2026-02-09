
import 'dart:io';

import 'package:dio/dio.dart';
import 'package:dio/io.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../config/env_config.dart';
import '../../features/auth/data/repositories/auth_repository_impl.dart';
import '../../features/auth/presentation/providers/auth_notifier.dart';

part 'api_client.g.dart';

@riverpod
Dio dio(Ref ref) {
  final dio = Dio(
    BaseOptions(
      baseUrl: EnvConfig.authBaseUrl,
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
    ),
  );

  if (!kIsWeb && kDebugMode) {
    (dio.httpClientAdapter as IOHttpClientAdapter).createHttpClient = () {
      final client = HttpClient();
      client.badCertificateCallback = (cert, host, port) => true;
      return client;
    };
  }

  // Add LogInterceptor
  dio.interceptors.add(LogInterceptor(
    requestBody: true,
    responseBody: true,
    logPrint: (object) {
      if (kDebugMode) {
        print(object);
      }
    },
  ));

  // Add AuthInterceptor for 401 handling
  dio.interceptors.add(AuthInterceptor(ref, dio));

  return dio;
}

@riverpod
Dio productsDio(Ref ref) {
  final dio = Dio(
    BaseOptions(
      baseUrl: EnvConfig.apiBaseUrl,
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 10),
    ),
  );

  // Allow self-signed certificates for localhost development
  if (!kIsWeb && kDebugMode) {
    (dio.httpClientAdapter as IOHttpClientAdapter).createHttpClient = () {
      final client = HttpClient();
      client.badCertificateCallback = (cert, host, port) => true;
      return client;
    };
  }

  // Add LogInterceptor
  dio.interceptors.add(LogInterceptor(
    requestBody: true,
    responseBody: true,
    logPrint: (object) {
      if (kDebugMode) {
        print(object);
      }
    },
  ));

  // Add AuthInterceptor for 401 handling
  dio.interceptors.add(AuthInterceptor(ref, dio));

  return dio;
}

class AuthInterceptor extends Interceptor {
  final Ref _ref;
  final Dio _dio;
  bool _isRefreshing = false;

  AuthInterceptor(this._ref, this._dio);

  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) async {
    const storage = FlutterSecureStorage();
    final token = await storage.read(key: 'access_token');
    if (token != null) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    return handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    if (err.response?.statusCode == 401) {
      if (_isRefreshing) {
        return handler.next(err);
      }

      _isRefreshing = true;
      const storage = FlutterSecureStorage();
      final refreshToken = await storage.read(key: 'refresh_token');

      if (refreshToken != null) {
        try {
          final repository = _ref.read(authRepositoryProvider);
          final result = await repository.refreshToken(refreshToken);

          return await result.fold(
            (error) async {
              _isRefreshing = false;
              await _handleLogout();
              return handler.next(err);
            },
            (response) async {
              await storage.write(key: 'access_token', value: response.accessToken);
              await storage.write(key: 'refresh_token', value: response.refreshToken);
              _isRefreshing = false;

              // Retry original request
              final options = err.requestOptions;
              options.headers['Authorization'] = 'Bearer ${response.accessToken}';
              
              final retryResponse = await _dio.fetch(options);
              return handler.resolve(retryResponse);
            },
          );
        } catch (e) {
          _isRefreshing = false;
          await _handleLogout();
          return handler.next(err);
        }
      } else {
        _isRefreshing = false;
        await _handleLogout();
      }
    }
    return handler.next(err);
  }

  Future<void> _handleLogout() async {
    const storage = FlutterSecureStorage();
    await storage.deleteAll();
    _ref.invalidate(authProvider);
  }
}
