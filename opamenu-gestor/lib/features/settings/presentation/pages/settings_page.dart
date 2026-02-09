
import 'package:flutter/material.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import '../widgets/printers_tab.dart';
import '../widgets/restaurant_settings_tab.dart';

class SettingsPage extends StatefulWidget {
  const SettingsPage({super.key});

  @override
  State<SettingsPage> createState() => _SettingsPageState();
}

class _SettingsPageState extends State<SettingsPage> with SingleTickerProviderStateMixin {
  late TabController _tabController;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color(0xFFF9FAFB),
      appBar: AppBar(
        title: const Text(
          'Configurações',
          style: TextStyle(fontWeight: FontWeight.bold, color: AppColors.textPrimary),
        ),
        backgroundColor: Colors.white,
        elevation: 0,
        bottom: TabBar(
          controller: _tabController,
          labelColor: AppColors.primary,
          unselectedLabelColor: Colors.grey,
          indicatorColor: AppColors.primary,
          tabs: const [
            Tab(icon: Icon(Icons.store), text: 'Restaurante'),
            Tab(icon: Icon(Icons.print), text: 'Impressoras'),
            Tab(icon: Icon(Icons.security), text: 'Segurança'),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: const [
          RestaurantSettingsTab(),
          PrintersTab(),
          Center(child: Text('Em breve: Configurações de Segurança')),
        ],
      ),
    );
  }
}
