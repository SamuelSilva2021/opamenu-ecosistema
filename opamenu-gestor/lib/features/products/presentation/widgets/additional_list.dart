
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../providers/additional_notifier.dart';
import '../../domain/models/additional_group_model.dart';
import '../../../../core/theme/app_colors.dart';

class AdditionalList extends ConsumerWidget {
  const AdditionalList({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final groupsAsync = ref.watch(additionalProvider);

    return Scaffold(
      backgroundColor: const Color(0xFFF9FAFB),
      body: groupsAsync.when(
        data: (groups) {
          if (groups.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.add_circle_outline, size: 64, color: Colors.grey[400]),
                  const SizedBox(height: 16),
                  Text(
                    'Nenhum grupo de adicionais cadastrado',
                    style: TextStyle(color: Colors.grey[600], fontSize: 16),
                  ),
                ],
              ),
            );
          }
          return ListView.builder(
            padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
            itemCount: groups.length,
            itemBuilder: (context, index) {
              final group = groups[index];
              return Container(
                margin: const EdgeInsets.only(bottom: 16),
                decoration: BoxDecoration(
                  color: Colors.white,
                  borderRadius: BorderRadius.circular(16),
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withOpacity(0.05),
                      blurRadius: 10,
                      offset: const Offset(0, 4),
                    ),
                  ],
                ),
                child: ExpansionTile(
                  leading: CircleAvatar(
                    backgroundColor: AppColors.primary.withOpacity(0.1),
                    child: const Icon(Icons.add_circle, color: AppColors.primary),
                  ),
                  title: Text(
                    group.name,
                    style: const TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
                  ),
                  subtitle: Text(
                    '${group.additionals.length} itens • Sel. ${group.minSelection}-${group.maxSelection}',
                    style: TextStyle(color: Colors.grey[600]),
                  ),
                  children: [
                    ...group.additionals.map((item) => ListTile(
                          title: Text(item.name),
                          trailing: Text('R\$ ${item.price.toStringAsFixed(2)}'),
                        )),
                    Padding(
                      padding: const EdgeInsets.all(16.0),
                      child: Row(
                        mainAxisAlignment: MainAxisAlignment.end,
                        children: [
                          TextButton.icon(
                            icon: const Icon(Icons.edit_outlined, size: 18),
                            label: const Text('Editar'),
                            onPressed: () => _showGroupDialog(context, ref, group: group),
                            style: TextButton.styleFrom(foregroundColor: Colors.orange),
                          ),
                          const SizedBox(width: 8),
                          TextButton.icon(
                            icon: const Icon(Icons.delete_outline, size: 18),
                            label: const Text('Excluir'),
                            onPressed: () => _confirmDelete(context, ref, group),
                            style: TextButton.styleFrom(foregroundColor: Colors.red),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              );
            },
          );
        },
        loading: () => const Center(child: CircularProgressIndicator(color: AppColors.primary)),
        error: (error, _) => Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(Icons.error_outline, color: Colors.red, size: 48),
              const SizedBox(height: 16),
              Text('Erro: $error', style: const TextStyle(color: Colors.red)),
              TextButton(
                onPressed: () => ref.refresh(additionalProvider),
                child: const Text('Tentar novamente'),
              ),
            ],
          ),
        ),
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => _showGroupDialog(context, ref),
        backgroundColor: AppColors.primary,
        icon: const Icon(Icons.add, color: Colors.white),
        label: const Text('Novo Grupo', style: TextStyle(color: Colors.white)),
      ),
    );
  }

  Future<void> _showGroupDialog(BuildContext context, WidgetRef ref, {AdditionalGroupModel? group}) async {
    final nameController = TextEditingController(text: group?.name);
    final descController = TextEditingController(text: group?.description);

    return showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(group == null ? 'Novo Grupo de Adicionais' : 'Editar Grupo'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextField(
              controller: nameController,
              decoration: const InputDecoration(labelText: 'Nome do Grupo'),
            ),
            const SizedBox(height: 16),
            TextField(
              controller: descController,
              decoration: const InputDecoration(labelText: 'Descrição (opcional)'),
            ),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Cancelar'),
          ),
          ElevatedButton(
            onPressed: () {
              if (group == null) {
                ref.read(additionalProvider.notifier).addGroup(
                  nameController.text,
                  descController.text,
                );
              } else {
                ref.read(additionalProvider.notifier).updateGroup(
                  group.copyWith(
                    name: nameController.text,
                    description: descController.text,
                  ),
                );
              }
              Navigator.pop(context);
            },
            child: const Text('Salvar'),
          ),
        ],
      ),
    );
  }

  Future<void> _confirmDelete(BuildContext context, WidgetRef ref, AdditionalGroupModel group) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Excluir Grupo'),
        content: Text('Deseja realmente excluir o grupo "${group.name}"?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancelar'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Excluir', style: TextStyle(color: Colors.red)),
          ),
        ],
      ),
    );

    if (confirmed == true) {
      ref.read(additionalProvider.notifier).deleteGroup(group.id);
    }
  }
}

// Extension to help with copies if needed (simplified for now)
extension on AdditionalGroupModel {
  AdditionalGroupModel copyWith({String? name, String? description}) {
    return AdditionalGroupModel(
      id: id,
      name: name ?? this.name,
      description: description ?? this.description,
      minSelection: minSelection,
      maxSelection: maxSelection,
      isRequired: isRequired,
      isActive: isActive,
      additionals: additionals,
    );
  }
}
