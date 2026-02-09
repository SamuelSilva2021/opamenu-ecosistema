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

part 'app_router.g.dart';

final rootNavigatorKey = GlobalKey<NavigatorState>();
final shellNavigatorKey = GlobalKey<NavigatorState>();

@riverpod
GoRouter goRouter(Ref ref) {
  final permissionsAsync = ref.watch(permissionsProvider);

  return GoRouter(
    navigatorKey: rootNavigatorKey,
    initialLocation: '/login',
    redirect: (context, state) {
      final permissions = permissionsAsync.value ?? [];
      final path = state.uri.path;

      // Allow login access
      if (path == '/login') return null;

      // Check if user has permission for the module
      final requiredModule = _routePermissions.entries
          .where((e) => path.startsWith(e.key))
          .map((e) => e.value)
          .firstOrNull;

      if (requiredModule != null) {
        final hasAccess = permissions.any((p) => p.module == requiredModule);
        if (!hasAccess && permissions.isNotEmpty) {
          return '/dashboard';
        }
      }

      return null;
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

const _routePermissions = {
  '/dashboard': 'DASHBOARD',
  '/pos': 'POS',
  '/orders': 'ORDERS',
  '/tables': 'TABLES',
  '/products': 'PRODUCTS',
  '/users': 'USERS',
  '/settings': 'SETTINGS',
};
