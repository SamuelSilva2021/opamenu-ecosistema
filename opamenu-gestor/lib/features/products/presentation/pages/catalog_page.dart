
import 'package:flutter/material.dart';
import 'package:opamenu_gestor/features/products/presentation/widgets/category_list.dart';
import 'package:opamenu_gestor/features/products/presentation/widgets/additional_list.dart';
import 'package:opamenu_gestor/features/products/presentation/widgets/product_list.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';

class CatalogPage extends StatefulWidget {
  const CatalogPage({super.key});

  @override
  State<CatalogPage> createState() => _CatalogPageState();
}

class _CatalogPageState extends State<CatalogPage> with SingleTickerProviderStateMixin {
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
        backgroundColor: Colors.white,
        elevation: 0,
        title: const Text(
          'Gestão de Catálogo',
          style: TextStyle(color: AppColors.textPrimary, fontWeight: FontWeight.bold),
        ),
        bottom: TabBar(
          controller: _tabController,
          labelColor: AppColors.primary,
          unselectedLabelColor: Colors.grey,
          indicatorColor: AppColors.primary,
          tabs: const [
            Tab(text: 'Produtos', icon: Icon(Icons.inventory_2)),
            Tab(text: 'Categorias', icon: Icon(Icons.category)),
            Tab(text: 'Adicionais', icon: Icon(Icons.add_circle)),
          ],
        ),
      ),
      body: TabBarView(
        controller: _tabController,
        children: const [
          ProductList(),
          CategoryList(),
          AdditionalList(),
        ],
      ),
    );
  }
}
