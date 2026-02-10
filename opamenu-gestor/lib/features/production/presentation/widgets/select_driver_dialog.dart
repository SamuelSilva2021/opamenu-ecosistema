import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/features/users/presentation/providers/users_notifier.dart';
import 'package:opamenu_gestor/features/users/domain/models/user_model.dart';

class SelectDriverDialog extends ConsumerStatefulWidget {
  const SelectDriverDialog({super.key});

  @override
  ConsumerState<SelectDriverDialog> createState() => _SelectDriverDialogState();
}

class _SelectDriverDialogState extends ConsumerState<SelectDriverDialog> {
  UserModel? _selectedDriver;

  @override
  Widget build(BuildContext context) {
    final usersAsync = ref.watch(usersProvider);

    return AlertDialog(
      title: const Text('Selecionar Entregador'),
      content: SizedBox(
        width: 400,
        child: usersAsync.when(
          data: (users) {
            // Filtrar usuários que podem ser entregadores (MVP: Mostra todos ativos)
            // Futuro: Filtrar por Role "Driver"
            final activeUsers = users.where((u) => u.isActive).toList();

            if (activeUsers.isEmpty) {
              return const Center(child: Text('Nenhum entregador disponível.'));
            }

            return ListView.builder(
              shrinkWrap: true,
              itemCount: activeUsers.length,
              itemBuilder: (context, index) {
                final user = activeUsers[index];
                final isSelected = _selectedDriver?.id == user.id;

                return ListTile(
                  leading: CircleAvatar(
                    child: Text(user.username.substring(0, 1).toUpperCase()),
                  ),
                  title: Text(user.username),
                  subtitle: user.phoneNumber != null ? Text(user.phoneNumber!) : null,
                  trailing: isSelected 
                      ? const Icon(Icons.check_circle, color: Colors.green)
                      : null,
                  onTap: () {
                    setState(() {
                      _selectedDriver = user;
                    });
                  },
                  selected: isSelected,
                );
              },
            );
          },
          loading: () => const Center(child: CircularProgressIndicator()),
          error: (e, s) => Center(child: Text('Erro ao carregar usuários: $e')),
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
