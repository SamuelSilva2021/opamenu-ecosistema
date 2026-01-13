import 'package:dio/dio.dart';

class ViaCepResponseDto {
  final String cep;
  final String logradouro;
  final String complemento;
  final String bairro;
  final String localidade;
  final String uf;
  final bool erro;

  ViaCepResponseDto({
    required this.cep,
    required this.logradouro,
    required this.complemento,
    required this.bairro,
    required this.localidade,
    required this.uf,
    this.erro = false,
  });

  factory ViaCepResponseDto.fromJson(Map<String, dynamic> json) {
    return ViaCepResponseDto(
      cep: json['cep'] ?? '',
      logradouro: json['logradouro'] ?? '',
      complemento: json['complemento'] ?? '',
      bairro: json['bairro'] ?? '',
      localidade: json['localidade'] ?? '',
      uf: json['uf'] ?? '',
      erro: json['erro'] == true || json['erro'] == 'true',
    );
  }
}

class ViaCepDatasource {
  final Dio _dio;

  ViaCepDatasource([Dio? dio]) : _dio = dio ?? Dio();

  Future<ViaCepResponseDto?> fetchAddress(String cep) async {
    try {
      final cleanCep = cep.replaceAll(RegExp(r'[^0-9]'), '');
      if (cleanCep.length != 8) return null;

      final response = await _dio.get('https://viacep.com.br/ws/$cleanCep/json/');
      
      if (response.statusCode == 200) {
        final dto = ViaCepResponseDto.fromJson(response.data);
        if (dto.erro) return null;
        return dto;
      }
      return null;
    } catch (e) {
      return null;
    }
  }
}
