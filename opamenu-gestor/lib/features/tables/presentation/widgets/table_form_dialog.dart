import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/presentation/widgets/app_loader.dart';
import '../../data/models/table_response_dto.dart';
import '../controllers/tables_controller.dart';

class TableFormDialog extends ConsumerStatefulWidget {
  final TableResponseDto? table;

  const TableFormDialog({super.key, this.table});

  @override
  ConsumerState<TableFormDialog> createState() => _TableFormDialogState();
}

class _TableFormDialogState extends ConsumerState<TableFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _nameController;
  late TextEditingController _capacityController;
  bool _isActive = true;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.table?.name ?? '');
    _capacityController = TextEditingController(text: widget.table?.capacity.toString() ?? '4');
    _isActive = widget.table?.isActive ?? true;
  }

  @override
  void dispose() {
    _nameController.dispose();
    _capacityController.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    if (_formKey.currentState?.validate() ?? false) {
      LoadingOverlay.show(context, message: 'Salvando mesa...');
      try {
        final name = _nameController.text.trim();
        final capacity = int.parse(_capacityController.text.trim());

        if (widget.table == null) {
          // Create
          await ref.read(tablesControllerProvider.notifier).createTable(name, capacity);
        } else {
          // Update
          await ref.read(tablesControllerProvider.notifier).updateTable(
                widget.table!.id,
                name,
                capacity,
                _isActive,
              );
        }

        if (mounted) {
          LoadingOverlay.hide(context);
          Navigator.of(context).pop();
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Mesa salva com sucesso!')),
          );
        }
      } catch (e) {
        if (mounted) {
          LoadingOverlay.hide(context);
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Erro ao salvar mesa: $e'), backgroundColor: Colors.red),
          );
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(widget.table == null ? 'Nova Mesa' : 'Editar Mesa'),
      content: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextFormField(
              controller: _nameController,
              decoration: const InputDecoration(
                labelText: 'Nome da Mesa',
                hintText: 'Ex: Mesa 1',
              ),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Por favor, informe o nome';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _capacityController,
              decoration: const InputDecoration(
                labelText: 'Capacidade',
                hintText: 'Ex: 4',
              ),
              keyboardType: TextInputType.number,
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Por favor, informe a capacidade';
                }
                if (int.tryParse(value) == null) {
                  return 'Informe um número válido';
                }
                return null;
              },
            ),
            if (widget.table != null) ...[
              const SizedBox(height: 16),
              SwitchListTile(
                title: const Text('Ativa'),
                value: _isActive,
                onChanged: (value) {
                  setState(() {
                    _isActive = value;
                  });
                },
              ),
            ],
          ],
        ),
      ),
      actions: [
        TextButton(
          onPressed: () => Navigator.of(context).pop(),
          child: const Text('Cancelar'),
        ),
        ElevatedButton(
          onPressed: _save,
          child: const Text('Salvar'),
        ),
      ],
    );
  }
}
