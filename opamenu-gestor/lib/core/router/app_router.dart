import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../core/presentation/layouts/main_layout.dart';
import '../../features/auth/presentation/pages/login_page.dart';
import '../../features/pos/presentation/pages/pos_page.dart';
import '../../features/dashboard/presentation/pages/dashboard_page.dart';
import '../../features/pos/presentation/pages/checkout_page.dart';
import '../../features/pos/presentation/pages/orders_page.dart';
import '../../features/tables/presentation/pages/tables_page.dart';
import '../presentation/pages/placeholder_page.dart';

part 'app_router.g.dart';

final rootNavigatorKey = GlobalKey<NavigatorState>();
final shellNavigatorKey = GlobalKey<NavigatorState>();

@riverpod
GoRouter goRouter(Ref ref) {
  return GoRouter(
    navigatorKey: rootNavigatorKey,
    initialLocation: '/login',
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
              child: PlaceholderPage(title: 'Produtos'),
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
