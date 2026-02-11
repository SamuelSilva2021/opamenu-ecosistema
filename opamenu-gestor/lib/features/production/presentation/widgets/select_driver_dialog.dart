import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/features/collaborators/presentation/providers/collaborators_provider.dart';
import 'package:opamenu_gestor/features/collaborators/domain/models/collaborator_model.dart';

class SelectDriverDialog extends ConsumerStatefulWidget {
  const SelectDriverDialog({super.key});

  @override
  ConsumerState<SelectDriverDialog> createState() => _SelectDriverDialogState();
}

class _SelectDriverDialogState extends ConsumerState<SelectDriverDialog> {
  CollaboratorModel? _selectedDriver;
  final TextEditingController _searchController = TextEditingController();
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _searchController.addListener(() {
      setState(() {
        _searchQuery = _searchController.text.toLowerCase();
      });
    });
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final collaboratorsAsync = ref.watch(collaboratorsProvider);

    return AlertDialog(
      title: const Text('Selecionar Entregador'),
      content: SizedBox(
        width: 400,
        height: 400,
        child: Column(
          children: [
            TextField(
              controller: _searchController,
              decoration: InputDecoration(
                hintText: 'Buscar entregador...',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(8),
                ),
                contentPadding: const EdgeInsets.symmetric(horizontal: 12, vertical: 8),
              ),
            ),
            const SizedBox(height: 16),
            Expanded(
              child: collaboratorsAsync.when(
                data: (collaborators) {
                  // Filtrar apenas ativos e aplicar busca
                  final activeDrivers = collaborators.where((c) {
                    if (!c.active) return false;
                    final matchesSearch = c.name.toLowerCase().contains(_searchQuery) ||
                        (c.phone?.contains(_searchQuery) ?? false);
                    return matchesSearch;
                  }).toList();

                  if (activeDrivers.isEmpty) {
                    return const Center(child: Text('Nenhum entregador disponível.'));
                  }

                  return ListView.separated(
                    itemCount: activeDrivers.length,
                    separatorBuilder: (context, index) => const Divider(height: 1),
                    itemBuilder: (context, index) {
                      final driver = activeDrivers[index];
                      final isSelected = _selectedDriver?.id == driver.id;

                      return ListTile(
                        leading: CircleAvatar(
                          backgroundColor: driver.type == 1 
                              ? Colors.blue.withOpacity(0.1)
                              : Colors.orange.withOpacity(0.1),
                          child: Icon(
                            driver.type == 1 ? Icons.badge : Icons.motorcycle,
                            color: driver.type == 1 ? Colors.blue : Colors.orange,
                            size: 20,
                          ),
                        ),
                        title: Text(driver.name),
                        subtitle: Text(
                          driver.typeLabel + (driver.phone != null ? ' • ${driver.phone}' : ''),
                          style: TextStyle(color: Colors.grey[600], fontSize: 12),
                        ),
                        trailing: isSelected 
                            ? const Icon(Icons.check_circle, color: Colors.green)
                            : null,
                        onTap: () {
                          setState(() {
                            _selectedDriver = driver;
                          });
                        },
                        selected: isSelected,
                        selectedTileColor: Colors.blue.withOpacity(0.05),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8),
                          side: isSelected 
                              ? const BorderSide(color: Colors.blue, width: 1)
                              : BorderSide.none,
                        ),
                      );
                    },
                  );
                },
                loading: () => const Center(child: CircularProgressIndicator()),
                error: (e, s) => Center(child: Text('Erro ao carregar entregadores: $e')),
              ),
            ),
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.pop(context),
          child: const Text('Cancelar'),
        ),
        ElevatedButton(
          onPressed: _selectedDriver == null
              ? null
              : () {
                  Navigator.pop(context, _selectedDriver);
                },
          child: const Text('Confirmar Saída'),
        ),
      ],
    );
  }
}
