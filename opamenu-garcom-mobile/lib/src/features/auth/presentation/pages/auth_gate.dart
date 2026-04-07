import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../tables/presentation/pages/tables_page.dart';
import '../providers/auth_controller_provider.dart';
import 'login_page.dart';

class AuthGate extends ConsumerWidget {
  const AuthGate({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final sessionAsync = ref.watch(AuthControllerProvider.provider);

    return sessionAsync.when(
      data: (session) {
        if (session == null) return const LoginPage();
        return const TablesPage();
      },
      error: (error, stackTrace) => const LoginPage(),
      loading: () {
        return const Scaffold(
          body: Center(child: CircularProgressIndicator()),
        );
      },
    );
  }
}
