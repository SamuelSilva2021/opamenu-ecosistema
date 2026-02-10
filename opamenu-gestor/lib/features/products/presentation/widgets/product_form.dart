
import 'dart:io';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';
import 'package:opamenu_gestor/core/theme/app_colors.dart';
import 'package:opamenu_gestor/core/utils/image_compressor.dart';
import 'package:opamenu_gestor/features/pos/domain/models/product_model.dart';
import 'package:opamenu_gestor/features/products/data/datasources/file_remote_datasource.dart';
import 'package:opamenu_gestor/features/products/presentation/providers/product_notifier.dart';
import 'package:opamenu_gestor/features/products/presentation/providers/category_notifier.dart';
import 'package:opamenu_gestor/features/products/presentation/providers/additional_notifier.dart';

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
  File? _imageFile;
  bool _isUploading = false;

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

  Future<void> _pickImage() async {
    final picker = ImagePicker();
    final image = await picker.pickImage(source: ImageSource.gallery);
    if (image != null) {
      final originalFile = File(image.path);
      
      // Comprime a imagem antes de salvar no estado
      final compressedFile = await ImageCompressor.compressFile(originalFile);
      
      setState(() {
        _imageFile = compressedFile ?? originalFile;
        _imageUrlController.text = ''; // Clear URL if local file is selected
      });
    }
  }

  Future<void> _save() async {
    if (_formKey.currentState!.validate()) {
      setState(() => _isUploading = true);
      
      try {
        String? imageUrl = _imageUrlController.text.trim();
        if (imageUrl.isEmpty) imageUrl = null;

        if (_imageFile != null) {
          imageUrl = await ref.read(fileRemoteDataSourceProvider).uploadImage(_imageFile!);
        }

        final data = {
          'name': _nameController.text,
          'description': _descController.text,
          'price': double.tryParse(_priceController.text) ?? 0.0,
          'imageUrl': imageUrl,
          'categoryId': _selectedCategoryId,
          'isActive': _isActive,
          'AditionalGroupIds': _selectedAdditionalGroupIds,
        };

        if (widget.product == null) {
          await ref.read(productProvider.notifier).addProduct(data);
        } else {
          await ref.read(productProvider.notifier).updateProduct(widget.product!.id, data);
        }

        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Produto salvo com sucesso!'),
              backgroundColor: Colors.green,
            ),
          );
          Navigator.pop(context);
        }
      } catch (e) {
        if (mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text('Erro ao salvar produto: $e'),
              backgroundColor: Colors.red,
            ),
          );
        }
      } finally {
        if (mounted) setState(() => _isUploading = false);
      }
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
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    child: TextFormField(
                      controller: _priceController,
                      decoration: const InputDecoration(
                        labelText: 'Preço (R\$)',
                        border: OutlineInputBorder(),
                        prefixIcon: Icon(Icons.attach_money),
                      ),
                      keyboardType: const TextInputType.numberWithOptions(decimal: true),
                    ),
                  ),
                  const SizedBox(width: 16),
                  Container(
                    height: 56, // Match input height
                    padding: const EdgeInsets.symmetric(horizontal: 12),
                    decoration: BoxDecoration(
                      border: Border.all(color: Colors.grey),
                      borderRadius: BorderRadius.circular(4),
                    ),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Text('Ativo', style: TextStyle(fontSize: 16)),
                        const SizedBox(width: 8),
                        Switch(
                          value: _isActive,
                          onChanged: (v) => setState(() => _isActive = v),
                          activeColor: AppColors.primary,
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 24),
              _buildSectionTitle('Imagem do Produto'),
              const SizedBox(height: 16),
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Expanded(
                    flex: 2,
                    child: Column(
                      children: [
                        TextFormField(
                          controller: _imageUrlController,
                          onChanged: (v) {
                            if (v.isNotEmpty) setState(() => _imageFile = null);
                          },
                          decoration: const InputDecoration(
                            labelText: 'URL da Imagem',
                            border: OutlineInputBorder(),
                            prefixIcon: Icon(Icons.link),
                          ),
                        ),
                        const SizedBox(height: 12),
                        const Text('OU', style: TextStyle(fontWeight: FontWeight.bold, color: Colors.grey)),
                        const SizedBox(height: 12),
                        SizedBox(
                          width: double.infinity,
                          child: OutlinedButton.icon(
                            onPressed: _isUploading ? null : _pickImage,
                            icon: const Icon(Icons.image_search),
                            label: const Text('Selecionar do Dispositivo'),
                            style: OutlinedButton.styleFrom(
                              padding: const EdgeInsets.all(16),
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(width: 24),
                  Expanded(
                    child: AspectRatio(
                      aspectRatio: 1,
                      child: Container(
                        decoration: BoxDecoration(
                          color: Colors.grey[100],
                          border: Border.all(color: Colors.grey[300]!),
                          borderRadius: BorderRadius.circular(12),
                        ),
                        clipBehavior: Clip.antiAlias,
                        child: _imageFile != null
                            ? Image.file(_imageFile!, fit: BoxFit.cover)
                            : _imageUrlController.text.isNotEmpty
                                ? Image.network(
                                    _imageUrlController.text,
                                    fit: BoxFit.cover,
                                    errorBuilder: (_, __, ___) => const Icon(Icons.broken_image, size: 40, color: Colors.grey),
                                  )
                                : const Icon(Icons.add_a_photo_outlined, size: 40, color: Colors.grey),
                      ),
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 24),
              _buildSectionTitle('Classificação'),
              const SizedBox(height: 16),
              categoriesAsync.when(
                data: (categories) {
                  // Ensure selected value exists in list or is null
                  final validValue = categories.any((c) => c.id == _selectedCategoryId) 
                      ? _selectedCategoryId 
                      : null;
                      
                  return DropdownButtonFormField<String>(
                    value: validValue,
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
                    validator: (v) => v == null ? 'Selecione uma categoria' : null,
                  );
                },
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
                      selectedColor: AppColors.primary.withValues(alpha: 0.2),
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
