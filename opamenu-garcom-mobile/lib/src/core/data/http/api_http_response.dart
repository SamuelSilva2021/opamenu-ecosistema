class ApiHttpResponse {
  final int statusCode;
  final Map<String, String> headers;
  final String body;

  const ApiHttpResponse({
    required this.statusCode,
    required this.headers,
    required this.body,
  });
}

