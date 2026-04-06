import 'dart:io';

import 'package:flutter/foundation.dart';

class AppEnvironment {
  final Uri authBaseUri;
  final Uri coreBaseUri;
  final bool allowInsecureCertificates;
  final String name;

  const AppEnvironment({
    required this.authBaseUri,
    required this.coreBaseUri,
    required this.allowInsecureCertificates,
    required this.name,
  });

  static AppEnvironment current() {
    const env = String.fromEnvironment('APP_ENV', defaultValue: '');
    const authBaseUrl = String.fromEnvironment('AUTH_BASE_URL', defaultValue: '');
    const coreBaseUrl = String.fromEnvironment('CORE_BASE_URL', defaultValue: '');

    final isProd = env.toLowerCase() == 'prod' || (env.isEmpty && kReleaseMode);

    if (isProd) {
      final authUri = _parseBaseUriOrFallback(
        authBaseUrl,
        fallback: Uri.parse('https://prod-auth.opamenu.invalid'),
      );
      final coreUri = _parseBaseUriOrFallback(
        coreBaseUrl,
        fallback: Uri.parse('https://prod-core.opamenu.invalid'),
      );

      return AppEnvironment(
        authBaseUri: authUri,
        coreBaseUri: coreUri,
        allowInsecureCertificates: false,
        name: 'prod',
      );
    }

    final authUri = _parseBaseUriOrFallback(
      authBaseUrl,
      fallback: Uri.parse('https://localhost:7019'),
    );
    final coreUri = _parseBaseUriOrFallback(
      coreBaseUrl,
      fallback: Uri.parse('https://localhost:7243'),
    );

    return AppEnvironment(
      authBaseUri: _normalizeLocalhost(authUri),
      coreBaseUri: _normalizeLocalhost(coreUri),
      allowInsecureCertificates: true,
      name: env.isEmpty ? 'local' : env.toLowerCase(),
    );
  }

  static Uri _parseBaseUriOrFallback(String value, {required Uri fallback}) {
    if (value.trim().isEmpty) return fallback;
    return Uri.parse(value);
  }

  static Uri _normalizeLocalhost(Uri uri) {
    if (kIsWeb) return uri;
    if (!Platform.isAndroid) return uri;
    if (uri.host != 'localhost') return uri;

    return uri.replace(host: '10.0.2.2');
  }
}

