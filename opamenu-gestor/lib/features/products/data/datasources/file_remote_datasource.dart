
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
    final fileName = file.path.split('/').last;
    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(file.path, filename: fileName),
      'folder': folder,
    });

    final response = await _dio.post(
      '/api/Files/upload',
      data: formData,
    );

    if (response.statusCode == 200 || response.statusCode == 201) {
      final apiResponse = ApiResponseModel<Map<String, dynamic>>.fromJson(
        response.data,
        (json) => json as Map<String, dynamic>,
      );

      if (apiResponse.succeeded && apiResponse.data != null) {
        // The API returns FileUploadResult which has FilePath
        return apiResponse.data!['filePath'] as String;
      } else {
        throw Exception(apiResponse.errors?.firstOrNull?.message ?? 'Erro ao fazer upload da imagem');
      }
    } else {
      throw Exception('Falha no upload: ${response.statusCode}');
    }
  }
}
