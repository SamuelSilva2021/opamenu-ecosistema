import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import 'package:opamenu_gestor/features/collaborators/domain/models/collaborator_model.dart';
import 'package:opamenu_gestor/features/collaborators/presentation/providers/collaborators_provider.dart';
import 'package:flutter/services.dart';
import 'package:opamenu_gestor/core/utils/phone_utils.dart';

class CollaboratorFormDialog extends ConsumerStatefulWidget {
  final CollaboratorModel? collaborator;

  const CollaboratorFormDialog({super.key, this.collaborator});

  @override
  ConsumerState<CollaboratorFormDialog> createState() => _CollaboratorFormDialogState();
}

class _CollaboratorFormDialogState extends ConsumerState<CollaboratorFormDialog> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _nameController;
  late TextEditingController _phoneController;
  late TextEditingController _roleController;
  int _type = 2; // Default to External
  bool _isActive = true;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.collaborator?.name);
    _phoneController = TextEditingController(text: widget.collaborator?.phone != null ? PhoneUtils.formatDisplay(widget.collaborator!.phone!) : null);
    _roleController = TextEditingController(text: widget.collaborator?.role);
    _type = widget.collaborator?.type ?? 2;
    _isActive = widget.collaborator?.active ?? true;
  }

  @override
  void dispose() {
    _nameController.dispose();
    _phoneController.dispose();
    _roleController.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    if (_formKey.currentState!.validate()) {
      final data = {
        'name': _nameController.text,
        'phone': PhoneUtils.sanitize(_phoneController.text),
        'role': _roleController.text,
        'type': _type,
        'active': _isActive,
      };

      try {
        if (widget.collaborator == null) {
          await ref.read(collaboratorsProvider.notifier).createCollaborator(data);
        } else {
          await ref.read(collaboratorsProvider.notifier).updateCollaborator(widget.collaborator!.id, data);
        }
        
        if (mounted) {
          Navigator.pop(context);
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Colaborador salvo com sucesso!'), backgroundColor: Colors.green),
          );
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text('Erro ao salvar colaborador: $e'), backgroundColor: Colors.red),
          );
        }
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Dialog(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Container(
        width: 500,
        padding: const EdgeInsets.all(24),
        child: Form(
          key: _formKey,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                widget.collaborator == null ? 'Novo Colaborador' : 'Editar Colaborador',
                style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 24),
              TextFormField(
                controller: _nameController,
                decoration: const InputDecoration(
                  labelText: 'Nome Completo',
                  border: OutlineInputBorder(),
                  prefixIcon: Icon(Icons.person_outline),
                ),
                validator: (v) => v == null || v.isEmpty ? 'Obrigatório' : null,
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _phoneController,
                      decoration: const InputDecoration(
                        labelText: 'Telefone',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.phone_outlined),
                      ),
                      keyboardType: TextInputType.phone,
                      inputFormatters: [PhoneMaskTextInputFormatter()],
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: TextFormField(
                      controller: _roleController,
                      decoration: const InputDecoration(
                        labelText: 'Função (ex: Entregador)',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.work_outline),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 16),
              DropdownButtonFormField<int>(
                value: _type,
                decoration: const InputDecoration(
                  labelText: 'Tipo de Vínculo',
                  border: OutlineInputBorder(),
                  prefixIcon: Icon(Icons.category_outlined),
                ),
                items: const [
                  DropdownMenuItem(value: 1, child: Text('Interno (Funcionário)')),
                  DropdownMenuItem(value: 2, child: Text('Externo (Parceiro/Terceiro)')),
                ],
                onChanged: (v) => setState(() => _type = v!),
              ),
              const SizedBox(height: 16),
              SwitchListTile(
                title: const Text('Colaborador Ativo'),
                value: _isActive,
                onChanged: (v) => setState(() => _isActive = v),
                contentPadding: EdgeInsets.zero,
              ),
              const SizedBox(height: 32),
              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  TextButton(
                    onPressed: () => Navigator.pop(context),
                    child: const Text('Cancelar'),
                  ),
                  const SizedBox(width: 16),
                  ElevatedButton(
                    onPressed: _save,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.primary,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
                    ),
                    child: const Text('Salvar'),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }
}
