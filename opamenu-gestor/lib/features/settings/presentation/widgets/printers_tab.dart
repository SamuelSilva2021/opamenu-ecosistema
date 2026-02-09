
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import '../providers/settings_notifier.dart';

class PrintersTab extends ConsumerStatefulWidget {
  const PrintersTab({super.key});

  @override
  ConsumerState<PrintersTab> createState() => _PrintersTabState();
}

class _PrintersTabState extends ConsumerState<PrintersTab> {
  final _kitchenController = TextEditingController();
  final _counterController = TextEditingController();

  @override
  void dispose() {
    _kitchenController.dispose();
    _counterController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final settingsAsync = ref.watch(settingsProvider);

    return settingsAsync.when(
      data: (settings) {
        if (_kitchenController.text.isEmpty) _kitchenController.text = settings['kitchen'] ?? '';
        if (_counterController.text.isEmpty) _counterController.text = settings['counter'] ?? '';

        return ListView(
          padding: const EdgeInsets.all(24),
          children: [
            const Text(
              'Configuração de Impressoras',
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            const Text(
              'Defina os endereços IP das impressoras de rede para envio de pedidos.',
              style: TextStyle(color: Colors.grey),
            ),
            const SizedBox(height: 32),
            _buildPrinterInput(
              label: 'Impressora da Cozinha (KDS)',
              icon: Icons.kitchen,
              controller: _kitchenController,
              onSave: () => ref.read(settingsProvider.notifier).updatePrinterIp('kitchen', _kitchenController.text),
            ),
            const SizedBox(height: 24),
            _buildPrinterInput(
              label: 'Impressora do Caixa/Balcão',
              icon: Icons.point_of_sale,
              controller: _counterController,
              onSave: () => ref.read(settingsProvider.notifier).updatePrinterIp('counter', _counterController.text),
            ),
          ],
        );
      },
      loading: () => const Center(child: CircularProgressIndicator()),
      error: (e, _) => Center(child: Text('Erro ao carregar configurações: $e')),
    );
  }

  Widget _buildPrinterInput({
    required String label,
    required IconData icon,
    required TextEditingController controller,
    required VoidCallback onSave,
  }) {
    return Container(
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
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, color: AppColors.primary, size: 28),
              const SizedBox(width: 12),
              Text(
                label,
                style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
            ],
          ),
          const SizedBox(height: 24),
          Row(
            children: [
              Expanded(
                child: TextField(
                  controller: controller,
                  decoration: const InputDecoration(
                    labelText: 'Endereço IP (Ex: 192.168.1.100)',
                    border: OutlineInputBorder(),
                    helperText: 'Certifique-se que o dispositivo está na mesma rede Wi-Fi',
                  ),
                  keyboardType: TextInputType.number,
                ),
              ),
              const SizedBox(width: 16),
              ElevatedButton.icon(
                onPressed: () {
                  onSave();
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Configuração salva com sucesso!'),
                      backgroundColor: Colors.green,
                    ),
                  );
                },
                icon: const Icon(Icons.save),
                label: const Text('Salvar'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
