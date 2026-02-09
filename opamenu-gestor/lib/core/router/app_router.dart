import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:opamenu_gestor/core/presentation/layouts/main_layout.dart';
import '../../features/auth/presentation/pages/login_page.dart';
import '../../features/pos/presentation/pages/pos_page.dart';
import '../../features/dashboard/presentation/pages/dashboard_page.dart';
import '../../features/pos/presentation/pages/checkout_page.dart';
import '../../features/pos/presentation/pages/orders_page.dart';
import '../../features/tables/presentation/pages/tables_page.dart';
import '../../features/products/presentation/pages/catalog_page.dart';
import 'package:opamenu_gestor/core/presentation/providers/permissions_provider.dart';
import 'package:opamenu_gestor/core/presentation/pages/placeholder_page.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/features/auth/presentation/providers/auth_notifier.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

part 'app_router.g.dart';

final rootNavigatorKey = GlobalKey<NavigatorState>();
final shellNavigatorKey = GlobalKey<NavigatorState>();

@riverpod
GoRouter goRouter(Ref ref) {
  return GoRouter(
    navigatorKey: rootNavigatorKey,
    initialLocation: '/login',
    refreshListenable: _RiverpodListenable(ref, authProvider),
    redirect: (context, state) async {
      final authState = ref.read(authProvider);
      final permissionsAsync = ref.read(permissionsProvider);
      final path = state.uri.path;
      final isLoggingIn = path == '/login';

      if (authState.isLoading) return null;

      const storage = FlutterSecureStorage();
      final token = await storage.read(key: 'access_token');
      final isAuthenticated = token != null;

      if (!isAuthenticated) {
        return isLoggingIn ? null : '/login';
      }

      if (isLoggingIn) return '/pos';

      return permissionsAsync.maybeWhen(
        data: (permissions) {
          final requiredModule = _routePermissions.entries
              .where((e) => path.startsWith(e.key))
              .map((e) => e.value)
              .firstOrNull;

          if (requiredModule != null) {
            final hasAccess = permissions.any((p) => p.module == requiredModule);
            if (!hasAccess) {
              for (final entry in _routePermissions.entries) {
                if (permissions.any((p) => p.module == entry.value)) {
                  return entry.key;
                }
              }
              return '/login';
            }
          }
          return null;
        },
        orElse: () => null,
      );
    },
    routes: [
      GoRoute(
        path: '/login',
        builder: (context, state) => const LoginPage(),
      ),
      ShellRoute(
        navigatorKey: shellNavigatorKey,
        builder: (context, state, child) {
          return MainLayout(child: child);
        },
        routes: [
          GoRoute(
            path: '/pos',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: PosPage(),
            ),
          ),
          GoRoute(
            path: '/dashboard',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: DashboardPage(),
            ),
          ),
          GoRoute(
            path: '/checkout',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: CheckoutPage(),
            ),
          ),
          GoRoute(
            path: '/orders',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: OrdersPage(),
            ),
          ),
          GoRoute(
            path: '/tables',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: TablesPage(),
            ),
          ),
          GoRoute(
            path: '/products',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: CatalogPage(),
            ),
          ),
          GoRoute(
            path: '/notifications',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: PlaceholderPage(title: 'Notificações'),
            ),
          ),
          GoRoute(
            path: '/users',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: PlaceholderPage(title: 'Usuários'),
            ),
          ),
          GoRoute(
            path: '/messages',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: PlaceholderPage(title: 'Mensagens'),
            ),
          ),
          GoRoute(
            path: '/settings',
            pageBuilder: (context, state) => const NoTransitionPage(
              child: PlaceholderPage(title: 'Configurações'),
            ),
          ),
        ],
      ),
    ],
  );
}

class _RiverpodListenable extends ChangeNotifier {
  _RiverpodListenable(Ref ref, ProviderListenable locator) {
    ref.listen(locator, (_, __) => notifyListeners());
  }
}

const _routePermissions = {
  '/dashboard': 'DASHBOARD',
  '/pos': 'PDV',
  '/orders': 'ORDER',
  '/tables': 'TABLE',
  '/products': 'PRODUCT',
  '/users': 'USER',
  '/settings': 'SETTINGS',
};
