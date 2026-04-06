import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../features/auth/presentation/pages/auth_gate.dart';
import 'app_theme.dart';

class OpaMenuGarcomApp extends ConsumerWidget {
  const OpaMenuGarcomApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return MaterialApp(
      title: 'OpaMenu Garçom',
      theme: AppTheme.light(),
      home: const AuthGate(),
      debugShowCheckedModeBanner: false,
    );
  }
}
