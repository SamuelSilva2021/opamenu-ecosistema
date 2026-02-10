
import 'dart:io';
import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../../../core/network/api_client.dart';
import '../../../auth/data/models/api_response_model.dart';

part 'file_remote_datasource.g.dart';

@riverpod
FileRemoteDataSource fileRemoteDataSource(Ref ref) {
  return FileRemoteDataSource(ref.watch(productsDioProvider));
}

class FileRemoteDataSource {
  final Dio _dio;

  FileRemoteDataSource(this._dio);

  Future<String> uploadImage(File file, {String folder = 'products'}) async {
    final fileName = file.path.split(Platform.pathSeparator).last;
    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(file.path, filename: fileName),
      'folder': folder,
    });

    final response = await _dio.post(
      '/api/files/upload',
      data: formData,
    );

    if (response.statusCode == 200 || response.statusCode == 201) {
      // Check for direct response (unwrapped)
      if (response.data is Map<String, dynamic>) {
        final data = response.data as Map<String, dynamic>;
        
        // Try common field names for file URL
        if (data.containsKey('fileUrl')) return data['fileUrl'] as String;
        if (data.containsKey('filePath')) return data['filePath'] as String;
        if (data.containsKey('url')) return data['url'] as String;
        
        // Check if wrapped in 'data' field but not ApiResponseModel structure
        if (data.containsKey('data') && data['data'] is Map) {
           final innerData = data['data'] as Map<String, dynamic>;
           if (innerData.containsKey('fileUrl')) return innerData['fileUrl'] as String;
           if (innerData.containsKey('filePath')) return innerData['filePath'] as String;
        }
      }

      // Try parsing as ApiResponseModel (wrapped)
      try {
        final apiResponse = ApiResponseModel<Map<String, dynamic>>.fromJson(
          response.data,
          (json) => json as Map<String, dynamic>,
        );

        if (apiResponse.succeeded && apiResponse.data != null) {
          final data = apiResponse.data!;
          if (data.containsKey('fileUrl')) return data['fileUrl'] as String;
          if (data.containsKey('filePath')) return data['filePath'] as String;
          if (data.containsKey('url')) return data['url'] as String;
        }
      } catch (_) {
        // Ignore parsing error and fall through
      }
      
      // If we reached here, we couldn't find the URL
      // Let's inspect the response to help debugging if needed
      throw Exception('Formato de resposta de upload desconhecido: ${response.data}');
    } else {
      throw Exception('Falha no upload: ${response.statusCode}');
    }
  }
}
