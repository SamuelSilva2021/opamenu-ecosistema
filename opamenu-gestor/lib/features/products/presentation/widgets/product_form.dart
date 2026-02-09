
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import 'package:opamenu_gestor/features/pos/domain/models/product_model.dart';
import 'package:opamenu_gestor/features/products/presentation/providers/product_notifier.dart';
import 'package:opamenu_gestor/features/products/presentation/providers/category_notifier.dart';
import 'package:opamenu_gestor/features/products/presentation/providers/additional_notifier.dart';
import 'package:opamenu_gestor/features/products/domain/models/additional_group_model.dart';
import 'package:opamenu_gestor/features/products/domain/models/category_model.dart';

class ProductForm extends ConsumerStatefulWidget {
  final ProductModel? product;

  const ProductForm({super.key, this.product});

  @override
  ConsumerState<ProductForm> createState() => _ProductFormState();
}

class _ProductFormState extends ConsumerState<ProductForm> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _nameController;
  late TextEditingController _descController;
  late TextEditingController _priceController;
  late TextEditingController _imageUrlController;
  String? _selectedCategoryId;
  List<String> _selectedAdditionalGroupIds = [];
  bool _isActive = true;

  @override
  void initState() {
    super.initState();
    _nameController = TextEditingController(text: widget.product?.name);
    _descController = TextEditingController(text: widget.product?.description);
    _priceController = TextEditingController(text: widget.product?.price.toString() ?? '0.00');
    _imageUrlController = TextEditingController(text: widget.product?.imageUrl);
    _selectedCategoryId = widget.product?.categoryId;
    _isActive = widget.product?.isActive ?? true;
    _selectedAdditionalGroupIds = widget.product?.addonGroups.map((e) => e.addonGroupId).toList() ?? [];
  }

  @override
  void dispose() {
    _nameController.dispose();
    _descController.dispose();
    _priceController.dispose();
    _imageUrlController.dispose();
    super.dispose();
  }

  void _save() {
    if (_formKey.currentState!.validate()) {
      final data = {
        'name': _nameController.text,
        'description': _descController.text,
        'price': double.tryParse(_priceController.text) ?? 0.0,
        'imageUrl': _imageUrlController.text.isEmpty ? null : _imageUrlController.text,
        'categoryId': _selectedCategoryId,
        'isActive': _isActive,
        'AditionalGroupIds': _selectedAdditionalGroupIds,
      };

      if (widget.product == null) {
        ref.read(productProvider.notifier).addProduct(data);
      } else {
        ref.read(productProvider.notifier).updateProduct(widget.product!.id, data);
      }
      Navigator.pop(context);
    }
  }

  @override
  Widget build(BuildContext context) {
    final categoriesAsync = ref.watch(categoryProvider);
    final additionalGroupsAsync = ref.watch(additionalProvider);

    return Scaffold(
      backgroundColor: Colors.white,
      appBar: AppBar(
        title: Text(widget.product == null ? 'Novo Produto' : 'Editar Produto'),
        actions: [
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
            child: ElevatedButton(
              onPressed: _save,
              style: ElevatedButton.styleFrom(
                backgroundColor: AppColors.primary,
                foregroundColor: Colors.white,
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
              ),
              child: const Text('Salvar'),
            ),
          ),
        ],
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              _buildSectionTitle('Informações Básicas'),
              const SizedBox(height: 16),
              TextFormField(
                controller: _nameController,
                decoration: const InputDecoration(
                  labelText: 'Nome do Produto',
                  border: OutlineInputBorder(),
                  prefixIcon: Icon(Icons.shopping_bag_outlined),
                ),
                validator: (v) => v == null || v.isEmpty ? 'Campo obrigatório' : null,
              ),
              const SizedBox(height: 16),
              TextFormField(
                controller: _descController,
                decoration: const InputDecoration(
                  labelText: 'Descrição',
                  border: OutlineInputBorder(),
                  prefixIcon: Icon(Icons.description_outlined),
                ),
                maxLines: 3,
              ),
              const SizedBox(height: 16),
              Row(
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _priceController,
                      decoration: const InputDecoration(
                        labelText: 'Preço (R\$)',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.attach_money),
                      ),
                      keyboardType: TextInputType.number,
                    ),
                  ),
                  const SizedBox(width: 16),
                  SwitchListTile(
                    title: const Text('Ativo'),
                    value: _isActive,
                    onChanged: (v) => setState(() => _isActive = v),
                    contentPadding: EdgeInsets.zero,
                  ),
                ],
              ),
              const SizedBox(height: 24),
              _buildSectionTitle('Imagem'),
              const SizedBox(height: 16),
              TextFormField(
                controller: _imageUrlController,
                onChanged: (v) => setState(() {}),
                decoration: const InputDecoration(
                  labelText: 'URL da Imagem',
                  border: OutlineInputBorder(),
                  prefixIcon: Icon(Icons.link),
                  helperText: 'Insira a URL direta da imagem (ex: .jpg, .png)',
                ),
              ),
              if (_imageUrlController.text.isNotEmpty)
                Padding(
                  padding: const EdgeInsets.only(top: 16),
                  child: Container(
                    height: 150,
                    width: double.infinity,
                    decoration: BoxDecoration(
                      border: Border.all(color: Colors.grey[300]!),
                      borderRadius: BorderRadius.circular(8),
                      image: DecorationImage(
                        image: NetworkImage(_imageUrlController.text),
                        fit: BoxFit.cover,
                      ),
                    ),
                  ),
                ),
              const SizedBox(height: 24),
              _buildSectionTitle('Classificação'),
              const SizedBox(height: 16),
              categoriesAsync.when(
                data: (categories) => DropdownButtonFormField<String>(
                  value: _selectedCategoryId,
                  decoration: const InputDecoration(
                    labelText: 'Categoria',
                    border: OutlineInputBorder(),
                    prefixIcon: Icon(Icons.category_outlined),
                  ),
                  items: categories.map((c) => DropdownMenuItem(
                    value: c.id,
                    child: Text(c.name),
                  )).toList(),
                  onChanged: (v) => setState(() => _selectedCategoryId = v),
                ),
                loading: () => const LinearProgressIndicator(),
                error: (e, _) => Text('Erro ao carregar categorias: $e'),
              ),
              const SizedBox(height: 24),
              _buildSectionTitle('Adicionais e Variações'),
              const SizedBox(height: 8),
              const Text(
                'Selecione os grupos de adicionais vinculados a este produto.',
                style: TextStyle(color: Colors.grey, fontSize: 12),
              ),
              const SizedBox(height: 16),
              additionalGroupsAsync.when(
                data: (groups) => Wrap(
                  spacing: 8,
                  children: groups.map((g) {
                    final isSelected = _selectedAdditionalGroupIds.contains(g.id);
                    return FilterChip(
                      label: Text(g.name),
                      selected: isSelected,
                      onSelected: (selected) {
                        setState(() {
                          if (selected) {
                            _selectedAdditionalGroupIds.add(g.id);
                          } else {
                            _selectedAdditionalGroupIds.remove(g.id);
                          }
                        });
                      },
                      selectedColor: AppColors.primary.withOpacity(0.2),
                      checkmarkColor: AppColors.primary,
                    );
                  }).toList(),
                ),
                loading: () => const LinearProgressIndicator(),
                error: (e, _) => Text('Erro ao carregar grupos: $e'),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildSectionTitle(String title) {
    return Text(
      title,
      style: const TextStyle(
        fontSize: 18,
        fontWeight: FontWeight.bold,
        color: AppColors.textPrimary,
      ),
    );
  }
}
