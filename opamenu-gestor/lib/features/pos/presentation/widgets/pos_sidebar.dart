import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:opamenu_gestor/core/presentation/widgets/permission_gate.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import 'package:opamenu_gestor/features/auth/presentation/providers/auth_notifier.dart';

class PosSidebar extends ConsumerWidget {
  const PosSidebar({super.key});

  int _getCurrentIndex(String location) {
    if (location.startsWith('/dashboard')) return 0;
    if (location.startsWith('/pos')) return 1;
    if (location.startsWith('/checkout')) return 1; // Mantém selecionado se estiver no checkout
    if (location.startsWith('/orders')) return 2;
    if (location.startsWith('/tables')) return 3;
    if (location.startsWith('/products')) return 4;
    if (location.startsWith('/notifications')) return 5;
    if (location.startsWith('/users')) return 6;
    if (location.startsWith('/messages')) return 7;
    if (location.startsWith('/settings')) return 8;
    return -1;
  }

  void _navigateTo(BuildContext context, int index) {
    final String location = GoRouterState.of(context).uri.toString();
    final int currentIndex = _getCurrentIndex(location);
    if (index == currentIndex) return;

    switch (index) {
      case 0:
        context.go('/dashboard');
        break;
      case 1:
        context.go('/pos');
        break;
      case 2:
        context.go('/orders');
        break;
      case 3:
        context.go('/tables');
        break;
      case 4:
        context.go('/products');
        break;
      case 5:
        context.go('/notifications');
        break;
      case 6:
        context.go('/users');
        break;
      case 7:
        context.go('/messages');
        break;
      case 8:
        context.go('/settings');
        break;
    }
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final String location = GoRouterState.of(context).uri.toString();
    final int currentIndex = _getCurrentIndex(location);

    return Container(
      width: 250, // Aumentado para caber o texto
      color: Colors.white,
      child: Column(
        children: [
          const SizedBox(height: 24),
          // Logo placeholder com Nome do App
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 24),
            child: Row(
              children: [
                Container(
                  padding: const EdgeInsets.all(8),
                  decoration: BoxDecoration(
                    color: AppColors.primary.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: const Icon(Icons.grid_view_rounded, color: AppColors.primary, size: 28),
                ),
                const SizedBox(width: 12),
                const Text(
                  'Opamenu',
                  style: TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                    color: AppColors.textPrimary,
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: 32),
          Expanded(
            child: SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Padding(
                    padding: EdgeInsets.only(left: 12, bottom: 8),
                    child: Text(
                      'MENU PRINCIPAL',
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        color: Colors.grey,
                        letterSpacing: 1.2,
                      ),
                    ),
                  ),
                  PermissionGate(
                    module: 'DASHBOARD',
                    child: _SidebarItem(
                      icon: Icons.dashboard_rounded,
                      label: 'Dashboard',
                      isSelected: currentIndex == 0,
                      onTap: () => _navigateTo(context, 0),
                    ),
                  ),
                  PermissionGate(
                    module: 'PDV',
                    child: _SidebarItem(
                      icon: Icons.point_of_sale_rounded,
                      label: 'Checkout',
                      isSelected: currentIndex == 1,
                      onTap: () => _navigateTo(context, 1),
                    ),
                  ),
                  PermissionGate(
                    module: 'ORDER',
                    child: _SidebarItem(
                      icon: Icons.receipt_long_rounded,
                      label: 'Pedidos',
                      isSelected: currentIndex == 2,
                      onTap: () => _navigateTo(context, 2),
                    ),
                  ),
                  PermissionGate(
                    module: 'TABLE',
                    child: _SidebarItem(
                      icon: Icons.table_restaurant_rounded,
                      label: 'Mesas',
                      isSelected: currentIndex == 3,
                      onTap: () => _navigateTo(context, 3),
                    ),
                  ),
                  PermissionGate(
                    module: 'PRODUCT',
                    child: _SidebarItem(
                      icon: Icons.inventory_2_rounded,
                      label: 'Produtos',
                      isSelected: currentIndex == 4,
                      onTap: () => _navigateTo(context, 4),
                    ),
                  ),
                  
                  const SizedBox(height: 24),
                  const Padding(
                    padding: EdgeInsets.only(left: 12, bottom: 8),
                    child: Text(
                      'GERAL',
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        color: Colors.grey,
                        letterSpacing: 1.2,
                      ),
                    ),
                  ),
                  
                  _SidebarItem(
                    icon: Icons.notifications_rounded,
                    label: 'Notificações',
                    isSelected: currentIndex == 5,
                    onTap: () => _navigateTo(context, 5),
                  ),
                  PermissionGate(
                    module: 'USER',
                    child: _SidebarItem(
                      icon: Icons.people_rounded,
                      label: 'Usuários',
                      isSelected: currentIndex == 6,
                      onTap: () => _navigateTo(context, 6),
                    ),
                  ),
                  _SidebarItem(
                    icon: Icons.chat_bubble_rounded,
                    label: 'Mensagens',
                    isSelected: currentIndex == 7,
                    onTap: () => _navigateTo(context, 7),
                  ),
                  PermissionGate(
                    module: 'SETTINGS',
                    child: _SidebarItem(
                      icon: Icons.settings_rounded,
                      label: 'Configurações',
                      isSelected: currentIndex == 8,
                      onTap: () => _navigateTo(context, 8),
                    ),
                  ),
                ],
              ),
            ),
          ),
          // User profile snippet at bottom
          Container(
            padding: const EdgeInsets.all(16),
            margin: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.grey[50],
              borderRadius: BorderRadius.circular(12),
            ),
            child: Row(
              children: [
                const CircleAvatar(
                  radius: 16,
                  backgroundColor: AppColors.primary,
                  child: Icon(Icons.person, color: Colors.white, size: 20),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        'Admin',
                        style: TextStyle(
                          fontWeight: FontWeight.bold,
                          fontSize: 14,
                        ),
                      ),
                      Text(
                        'admin@opamenu.com',
                        style: TextStyle(
                          color: Colors.grey[600],
                          fontSize: 10,
                        ),
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                      ),
                    ],
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.logout_rounded, color: Colors.redAccent, size: 20),
                  onPressed: () => ref.read(authProvider.notifier).logout(),
                  tooltip: 'Sair',
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _SidebarItem extends StatelessWidget {
  final IconData icon;
  final String label;
  final bool isSelected;
  final VoidCallback? onTap;

  const _SidebarItem({
    required this.icon,
    required this.label,
    this.isSelected = false,
    this.onTap,
  });

  @override
  Widget build(BuildContext context) {
    return Material(
      color: Colors.transparent,
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Container(
          margin: const EdgeInsets.symmetric(vertical: 4),
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
          decoration: isSelected
              ? BoxDecoration(
                  color: AppColors.primary,
                  borderRadius: BorderRadius.circular(12),
                  boxShadow: [
                    BoxShadow(
                      color: AppColors.primary.withValues(alpha: 0.3),
                      blurRadius: 8,
                      offset: const Offset(0, 4),
                    ),
                  ],
                )
              : null,
          child: Row(
            children: [
              Icon(
                icon,
                color: isSelected ? Colors.white : AppColors.textSecondary,
                size: 22,
              ),
              const SizedBox(width: 12),
              Text(
                label,
                style: TextStyle(
                  color: isSelected ? Colors.white : AppColors.textSecondary,
                  fontWeight: isSelected ? FontWeight.bold : FontWeight.w500,
                  fontSize: 14,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
