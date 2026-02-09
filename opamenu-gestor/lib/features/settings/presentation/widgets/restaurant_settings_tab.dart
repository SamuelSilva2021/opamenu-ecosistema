
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import '../providers/settings_notifier.dart';

class RestaurantSettingsTab extends ConsumerStatefulWidget {
  const RestaurantSettingsTab({super.key});

  @override
  ConsumerState<RestaurantSettingsTab> createState() => _RestaurantSettingsTabState();
}

class _RestaurantSettingsTabState extends ConsumerState<RestaurantSettingsTab> {
  final _nameController = TextEditingController();
  final _addressController = TextEditingController();
  final _phoneController = TextEditingController();

  @override
  void dispose() {
    _nameController.dispose();
    _addressController.dispose();
    _phoneController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final settingsAsync = ref.watch(settingsProvider);

    return settingsAsync.when(
      data: (settings) {
        if (_nameController.text.isEmpty) _nameController.text = settings['name'] ?? '';
        if (_addressController.text.isEmpty) _addressController.text = settings['address'] ?? '';
        if (_phoneController.text.isEmpty) _phoneController.text = settings['phone'] ?? '';

        return ListView(
          padding: const EdgeInsets.all(24),
          children: [
            const Text(
              'Dados do Restaurante',
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            const Text(
              'Essas informações serão exibidas no cabeçalho dos comprovantes impressos.',
              style: TextStyle(color: Colors.grey),
            ),
            const SizedBox(height: 32),
            Container(
              padding: const EdgeInsets.all(24),
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(12),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withOpacity(0.05),
                    blurRadius: 10,
                    offset: const Offset(0, 4),
                  ),
                ],
              ),
              child: Column(
                children: [
                  TextField(
                    controller: _nameController,
                    decoration: const InputDecoration(
                      labelText: 'Nome do Estabelecimento',
                      border: OutlineInputBorder(),
                      prefixIcon: Icon(Icons.store),
                    ),
                  ),
                  const SizedBox(height: 16),
                  TextField(
                    controller: _addressController,
                    decoration: const InputDecoration(
                      labelText: 'Endereço Completo',
                      border: OutlineInputBorder(),
                      prefixIcon: Icon(Icons.location_on),
                    ),
                  ),
                  const SizedBox(height: 16),
                  TextField(
                    controller: _phoneController,
                    decoration: const InputDecoration(
                      labelText: 'Telefone',
                      border: OutlineInputBorder(),
                      prefixIcon: Icon(Icons.phone),
                    ),
                  ),
                  const SizedBox(height: 32),
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton.icon(
                      onPressed: () {
                        ref.read(settingsProvider.notifier).updateRestaurantInfo(
                          name: _nameController.text,
                          address: _addressController.text,
                          phone: _phoneController.text,
                        );
                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text('Dados salvos com sucesso!'),
                            backgroundColor: Colors.green,
                          ),
                        );
                      },
                      icon: const Icon(Icons.save),
                      label: const Text('Salvar Alterações'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        );
      },
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (e, _) => Center(child: Text('Erro: $e')),
    );
  }
}
