import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/presentation/widgets/permission_gate.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import 'package:opamenu_gestor/features/collaborators/domain/models/collaborator_model.dart';
import 'package:opamenu_gestor/features/collaborators/presentation/providers/collaborators_provider.dart';
import 'package:opamenu_gestor/features/collaborators/presentation/widgets/collaborator_form_dialog.dart';

class CollaboratorsPage extends ConsumerStatefulWidget {
  const CollaboratorsPage({super.key});

  @override
  ConsumerState<CollaboratorsPage> createState() => _CollaboratorsPageState();
}

class _CollaboratorsPageState extends ConsumerState<CollaboratorsPage> with SingleTickerProviderStateMixin {
  late TabController _tabController;
  final TextEditingController _searchController = TextEditingController();
  String _searchQuery = '';

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 3, vsync: this);
    _searchController.addListener(() {
      setState(() {
        _searchQuery = _searchController.text.toLowerCase();
      });
    });
  }

  @override
  void dispose() {
    _tabController.dispose();
    _searchController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final collaboratorsAsync = ref.watch(collaboratorsProvider);

    return Scaffold(
      backgroundColor: const Color(0xFFF9FAFB),
      body: Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header with Title and Add Button
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const Text(
                  'Gestão de Colaboradores',
                  style: TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                    color: AppColors.textPrimary,
                  ),
                ),
                PermissionGate(
                  module: 'USER_ACCOUNT',
                  operation: 'CREATE',
                  child: ElevatedButton.icon(
                    onPressed: () => showDialog(
                      context: context,
                      builder: (context) => const CollaboratorFormDialog(),
                    ),
                    icon: const Icon(Icons.person_add),
                    label: const Text('Novo Colaborador'),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: AppColors.primary,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(horizontal: 20, vertical: 16),
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 24),
            
            // Search and Filters
            Container(
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
                  Padding(
                    padding: const EdgeInsets.all(16.0),
                    child: TextField(
                      controller: _searchController,
                      decoration: InputDecoration(
                        hintText: 'Buscar por nome, telefone ou função...',
                        prefixIcon: const Icon(Icons.search),
                        border: OutlineInputBorder(
                          borderRadius: BorderRadius.circular(8),
                          borderSide: BorderSide.none,
                        ),
                        filled: true,
                        fillColor: Colors.grey[100],
                        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                      ),
                    ),
                  ),
                  TabBar(
                    controller: _tabController,
                    labelColor: AppColors.primary,
                    unselectedLabelColor: Colors.grey,
                    indicatorColor: AppColors.primary,
                    tabs: const [
                      Tab(text: 'Todos'),
                      Tab(text: 'Internos (Funcionários)'),
                      Tab(text: 'Externos (Parceiros)'),
                    ],
                  ),
                ],
              ),
            ),
            const SizedBox(height: 24),

            // List
            Expanded(
              child: Container(
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
                child: collaboratorsAsync.when(
                  data: (collaborators) {
                    // Filter logic
                    var filteredList = collaborators.where((c) {
                      final matchesSearch = c.name.toLowerCase().contains(_searchQuery) ||
                          (c.phone?.contains(_searchQuery) ?? false) ||
                          (c.role?.toLowerCase().contains(_searchQuery) ?? false);
                      return matchesSearch;
                    }).toList();

                    return TabBarView(
                      controller: _tabController,
                      children: [
                        // Tab 1: Todos
                        _buildList(context, ref, filteredList),
                        
                        // Tab 2: Internos (Type 1)
                        _buildList(context, ref, filteredList.where((c) => c.type == 1).toList()),
                        
                        // Tab 3: Externos (Type 2)
                        _buildList(context, ref, filteredList.where((c) => c.type == 2).toList()),
                      ],
                    );
                  },
                  loading: () => const Center(child: CircularProgressIndicator()),
                  error: (error, _) => Center(child: Text('Erro: $error')),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildList(BuildContext context, WidgetRef ref, List<CollaboratorModel> list) {
    if (list.isEmpty) {
      return const Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.search_off, size: 48, color: Colors.grey),
            SizedBox(height: 16),
            Text('Nenhum colaborador encontrado', style: TextStyle(color: Colors.grey)),
          ],
        ),
      );
    }

    return ListView.separated(
      padding: const EdgeInsets.all(16),
      itemCount: list.length,
      separatorBuilder: (context, index) => const Divider(),
      itemBuilder: (context, index) {
        final collaborator = list[index];
        return ListTile(
          leading: CircleAvatar(
            backgroundColor: collaborator.type == 1 
                ? Colors.blue.withOpacity(0.1)
                : Colors.orange.withOpacity(0.1),
            child: Icon(
              collaborator.type == 1 ? Icons.badge : Icons.motorcycle,
              color: collaborator.type == 1 ? Colors.blue : Colors.orange,
            ),
          ),
          title: Text(
            collaborator.name,
            style: const TextStyle(fontWeight: FontWeight.bold),
          ),
          subtitle: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              if (collaborator.phone != null) Text(collaborator.phone!),
              if (collaborator.role != null) Text(collaborator.role!),
              const SizedBox(height: 4),
              Row(
                children: [
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color: (collaborator.type == 1 ? Colors.blue : Colors.orange).withOpacity(0.1),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      collaborator.typeLabel,
                      style: TextStyle(
                        color: collaborator.type == 1 ? Colors.blue : Colors.orange,
                        fontSize: 12,
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color: (collaborator.active ? Colors.green : Colors.red).withOpacity(0.1),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Text(
                      collaborator.active ? 'Ativo' : 'Inativo',
                      style: TextStyle(
                        color: collaborator.active ? Colors.green : Colors.red,
                        fontSize: 12,
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
          trailing: Row(
            mainAxisSize: MainAxisSize.min,
            children: [
              PermissionGate(
                module: 'USER_ACCOUNT',
                operation: 'UPDATE',
                child: IconButton(
                  icon: const Icon(Icons.edit_outlined, color: Colors.orange),
                  onPressed: () => showDialog(
                    context: context,
                    builder: (context) => CollaboratorFormDialog(collaborator: collaborator),
                  ),
                ),
              ),
              PermissionGate(
                module: 'USER_ACCOUNT',
                operation: 'DELETE',
                child: IconButton(
                  icon: const Icon(Icons.delete_outline, color: Colors.red),
                  onPressed: () => _confirmDelete(context, ref, collaborator),
                ),
              ),
            ],
          ),
        );
      },
    );
  }

  Future<void> _confirmDelete(BuildContext context, WidgetRef ref, CollaboratorModel collaborator) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Confirmar exclusão'),
        content: Text('Deseja realmente excluir o colaborador "${collaborator.name}"?'),
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
      ref.read(collaboratorsProvider.notifier).deleteCollaborator(collaborator.id);
    }
  }
}
