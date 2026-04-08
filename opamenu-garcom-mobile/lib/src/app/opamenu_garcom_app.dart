import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../features/auth/presentation/pages/auth_gate.dart';
import '../features/auth/presentation/providers/auth_controller_provider.dart';
import 'app_theme.dart';

class OpaMenuGarcomApp extends ConsumerWidget {
  static final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();
  static final GlobalKey<ScaffoldMessengerState> scaffoldMessengerKey =
      GlobalKey<ScaffoldMessengerState>();

  const OpaMenuGarcomApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    ref.listen(AuthControllerProvider.provider, (previous, next) {
      final wasLoggedIn = previous?.asData?.value != null;
      final isLoggedOut = next.asData?.value == null;
      if (!wasLoggedIn || !isLoggedOut) return;

      navigatorKey.currentState?.popUntil((route) => route.isFirst);
    });

    return MaterialApp(
      title: 'OpaMenu Garçom',
      theme: AppTheme.light(),
      home: const AuthGate(),
      navigatorKey: navigatorKey,
      scaffoldMessengerKey: scaffoldMessengerKey,
      debugShowCheckedModeBanner: false,
    );
  }
}
