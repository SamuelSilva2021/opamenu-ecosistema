import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/theme/app_colors.dart';
import '../../domain/models/create_order_request_dto.dart';
import '../../domain/enums/delivery_type.dart';
import '../../../tables/presentation/controllers/tables_controller.dart';
import '../../../tables/data/models/table_response_dto.dart';
import '../providers/cart_notifier.dart';
import '../providers/checkout_provider.dart';

class CheckoutPage extends ConsumerStatefulWidget {
  const CheckoutPage({super.key});

  @override
  ConsumerState<CheckoutPage> createState() => _CheckoutPageState();
}

class _CheckoutPageState extends ConsumerState<CheckoutPage> {
  final _formKey = GlobalKey<FormState>();

  // Customer Controllers
  late TextEditingController _nameController;
  late TextEditingController _phoneController;
  late TextEditingController _emailController;

  // Address Controllers
  late TextEditingController _zipController;
  late TextEditingController _streetController;
  late TextEditingController _numberController;
  late TextEditingController _complementController;
  late TextEditingController _neighborhoodController;
  late TextEditingController _cityController;
  late TextEditingController _stateController;

  @override
  void initState() {
    super.initState();
    final state = ref.read(checkoutProvider);
    
    _nameController = TextEditingController(text: state.customerName);
    _phoneController = TextEditingController(text: state.customerPhone);
    _emailController = TextEditingController(text: state.customerEmail);

    _zipController = TextEditingController(text: state.address.zipCode);
    _streetController = TextEditingController(text: state.address.street);
    _numberController = TextEditingController(text: state.address.number);
    _complementController = TextEditingController(text: state.address.complement);
    _neighborhoodController = TextEditingController(text: state.address.neighborhood);
    _cityController = TextEditingController(text: state.address.city);
    _stateController = TextEditingController(text: state.address.state);
  }

  @override
  void dispose() {
    _nameController.dispose();
    _phoneController.dispose();
    _emailController.dispose();
    _zipController.dispose();
    _streetController.dispose();
    _numberController.dispose();
    _complementController.dispose();
    _neighborhoodController.dispose();
    _cityController.dispose();
    _stateController.dispose();
    super.dispose();
  }

  void _onAddressChanged() {
    final address = AddressDto(
      zipCode: _zipController.text,
      street: _streetController.text,
      number: _numberController.text,
      complement: _complementController.text.isEmpty ? null : _complementController.text,
      neighborhood: _neighborhoodController.text,
      city: _cityController.text,
      state: _stateController.text,
    );
    ref.read(checkoutProvider.notifier).updateAddress(address);
  }

  @override
  Widget build(BuildContext context) {
    final checkoutState = ref.watch(checkoutProvider);
    final cartItems = ref.watch(cartProvider);
    ref.listen(checkoutProvider, (previous, next) {
      if (previous?.address != next.address) {
        if (_streetController.text != next.address.street) _streetController.text = next.address.street;
        if (_neighborhoodController.text != next.address.neighborhood) _neighborhoodController.text = next.address.neighborhood;
        if (_cityController.text != next.address.city) _cityController.text = next.address.city;
        if (_stateController.text != next.address.state) _stateController.text = next.address.state;
        if (_zipController.text != next.address.zipCode && next.address.zipCode.isNotEmpty) {
        }
      }

      if (next.isSuccess && !(previous?.isSuccess ?? false)) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Pedido criado com sucesso!')),
        );
        ref.read(cartProvider.notifier).clearCart();
        ref.read(checkoutProvider.notifier).reset();
        Navigator.pop(context);
      }
      if (next.errorMessage != null && next.errorMessage != previous?.errorMessage) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro: ${next.errorMessage}'), backgroundColor: Colors.red),
        );
      }
    });

    return Column(
      children: [
        Container(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 16),
          color: Colors.white,
          child: Row(
            children: [
              IconButton(
                icon: const Icon(Icons.arrow_back),
                onPressed: () => context.pop(),
                color: AppColors.textPrimary,
              ),
              const SizedBox(width: 16),
              const Text(
                'Finalizar Pedido',
                style: TextStyle(
                  fontSize: 20,
                  fontWeight: FontWeight.bold,
                  color: AppColors.textPrimary,
                ),
              ),
            ],
          ),
        ),
        Expanded(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(24),
            child: Form(
          key: _formKey,
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Left Column: Form
              Expanded(
                flex: 2,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    _buildSectionTitle('Dados do Cliente'),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        Expanded(
                          child: _buildTextField(
                            controller: _nameController,
                            label: checkoutState.deliveryType == DeliveryType.table 
                                ? 'Nome do Cliente' 
                                : 'Nome do Cliente *',
                            onChanged: (v) => ref.read(checkoutProvider.notifier).updateCustomerInfo(name: v),
                            validator: (v) {
                              if (checkoutState.deliveryType == DeliveryType.table) return null;
                              return (v == null || v.isEmpty) ? 'Obrigatório' : null;
                            },
                          ),
                        ),
                        const SizedBox(width: 16),
                        Expanded(
                          child: _buildTextField(
                            controller: _phoneController,
                            label: checkoutState.deliveryType == DeliveryType.table 
                                ? 'Telefone' 
                                : 'Telefone *',
                            onChanged: (v) => ref.read(checkoutProvider.notifier).updateCustomerInfo(phone: v),
                            validator: (v) {
                              if (checkoutState.deliveryType == DeliveryType.table) return null;
                              return (v == null || v.length < 10) ? 'Inválido' : null;
                            },
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    _buildTextField(
                      controller: _emailController,
                      label: 'Email (Opcional)',
                      onChanged: (v) => ref.read(checkoutProvider.notifier).updateCustomerInfo(email: v),
                    ),
                    
                    const SizedBox(height: 32),
                    _buildSectionTitle('Tipo de Entrega'),
                    const SizedBox(height: 16),
                    Row(
                      children: [
                        _buildDeliveryOption(
                          label: 'Retirada',
                          value: DeliveryType.pickup,
                          groupValue: checkoutState.deliveryType,
                          icon: Icons.store,
                          onChanged: (v) => ref.read(checkoutProvider.notifier).setDeliveryMode(v),
                        ),
                        const SizedBox(width: 16),
                        _buildDeliveryOption(
                          label: 'Delivery',
                          value: DeliveryType.delivery,
                          groupValue: checkoutState.deliveryType,
                          icon: Icons.delivery_dining,
                          onChanged: (v) => ref.read(checkoutProvider.notifier).setDeliveryMode(v),
                        ),
                        const SizedBox(width: 16),
                        _buildDeliveryOption(
                          label: 'Mesa',
                          value: DeliveryType.table,
                          groupValue: checkoutState.deliveryType,
                          icon: Icons.table_restaurant,
                          onChanged: (v) => ref.read(checkoutProvider.notifier).setDeliveryMode(v),
                        ),
                      ],
                    ),

                    if (checkoutState.deliveryType == DeliveryType.table) ...[
                      const SizedBox(height: 32),
                      _buildSectionTitle('Selecione a Mesa'),
                      const SizedBox(height: 16),
                      Consumer(
                        builder: (context, ref, child) {
                          final tablesAsync = ref.watch(tablesControllerProvider);
                          
                          return tablesAsync.when(
                            data: (pagedResponse) {
                              final tables = pagedResponse.data ?? [];
                              if (tables.isEmpty) {
                                return const Padding(
                                  padding: EdgeInsets.all(16.0),
                                  child: Text('Nenhuma mesa cadastrada.', style: TextStyle(color: Colors.grey)),
                                );
                              }
                              
                              return GridView.builder(
                                shrinkWrap: true,
                                physics: const NeverScrollableScrollPhysics(),
                                gridDelegate: const SliverGridDelegateWithFixedCrossAxisCount(
                                  crossAxisCount: 4,
                                  crossAxisSpacing: 12,
                                  mainAxisSpacing: 12,
                                  childAspectRatio: 1.4,
                                ),
                                itemCount: tables.length,
                                itemBuilder: (context, index) {
                                  final table = tables[index];
                                  final isSelected = checkoutState.tableId == table.id;
                                  
                                  return InkWell(
                                    onTap: () {
                                      ref.read(checkoutProvider.notifier).setTableId(table.id);
                                    },
                                    borderRadius: BorderRadius.circular(12),
                                    child: Container(
                                      decoration: BoxDecoration(
                                        color: isSelected ? AppColors.primary : Colors.white,
                                        border: Border.all(
                                          color: isSelected ? AppColors.primary : Colors.grey[300]!,
                                          width: 2,
                                        ),
                                        borderRadius: BorderRadius.circular(12),
                                        boxShadow: [
                                          if (!isSelected)
                                            BoxShadow(
                                              color: Colors.black.withOpacity(0.05),
                                              blurRadius: 4,
                                              offset: const Offset(0, 2),
                                            ),
                                        ],
                                      ),
                                      child: Column(
                                        mainAxisAlignment: MainAxisAlignment.center,
                                        children: [
                                          Icon(
                                            Icons.table_restaurant,
                                            color: isSelected ? Colors.white : Colors.grey[600],
                                            size: 28,
                                          ),
                                          const SizedBox(height: 8),
                                          Text(
                                            table.name,
                                            style: TextStyle(
                                              color: isSelected ? Colors.white : AppColors.textPrimary,
                                              fontWeight: FontWeight.bold,
                                              fontSize: 14,
                                            ),
                                            textAlign: TextAlign.center,
                                            maxLines: 1,
                                            overflow: TextOverflow.ellipsis,
                                          ),
                                          Text(
                                            '${table.capacity} lug.',
                                            style: TextStyle(
                                              color: isSelected ? Colors.white.withOpacity(0.9) : Colors.grey[500],
                                              fontSize: 11,
                                            ),
                                          ),
                                        ],
                                      ),
                                    ),
                                  );
                                },
                              );
                            },
                            loading: () => const Center(child: Padding(
                              padding: EdgeInsets.all(16.0),
                              child: CircularProgressIndicator(),
                            )),
                            error: (e, s) => Padding(
                              padding: const EdgeInsets.all(16.0),
                              child: Text('Erro ao carregar mesas: $e', style: const TextStyle(color: Colors.red)),
                            ),
                          );
                        },
                      ),
                    ],

                    if (checkoutState.deliveryType == DeliveryType.delivery) ...[
                      const SizedBox(height: 32),
                      _buildSectionTitle('Endereço de Entrega'),
                      const SizedBox(height: 16),
                      Row(
                        children: [
                          Expanded(
                            flex: 1,
                            child: _buildTextField(
                              controller: _zipController,
                              label: 'CEP',
                              onChanged: (v) {
                                _onAddressChanged();
                                if (v.replaceAll(RegExp(r'[^0-9]'), '').length == 8) {
                                  ref.read(checkoutProvider.notifier).searchCep(v);
                                }
                              },
                              suffixIcon: checkoutState.isSearchingCep 
                                ? const SizedBox(width: 16, height: 16, child: Center(child: CircularProgressIndicator(strokeWidth: 2))) 
                                : null,
                            ),
                          ),
                          const SizedBox(width: 16),
                          Expanded(
                            flex: 2,
                            child: _buildTextField(
                              controller: _cityController,
                              label: 'Cidade *',
                              onChanged: (_) => _onAddressChanged(),
                              validator: (v) => (v == null || v.isEmpty) ? 'Obrigatório' : null,
                            ),
                          ),
                          const SizedBox(width: 16),
                          Expanded(
                            flex: 1,
                            child: _buildTextField(
                              controller: _stateController,
                              label: 'UF *',
                              onChanged: (_) => _onAddressChanged(),
                              validator: (v) => (v == null || v.length != 2) ? '2 letras' : null,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 16),
                      Row(
                        children: [
                          Expanded(
                            flex: 3,
                            child: _buildTextField(
                              controller: _streetController,
                              label: 'Rua *',
                              onChanged: (_) => _onAddressChanged(),
                              validator: (v) => (v == null || v.isEmpty) ? 'Obrigatório' : null,
                            ),
                          ),
                          const SizedBox(width: 16),
                          Expanded(
                            flex: 1,
                            child: _buildTextField(
                              controller: _numberController,
                              label: 'Número *',
                              onChanged: (_) => _onAddressChanged(),
                              validator: (v) => (v == null || v.isEmpty) ? 'Obrigatório' : null,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 16),
                      Row(
                        children: [
                          Expanded(
                            child: _buildTextField(
                              controller: _neighborhoodController,
                              label: 'Bairro *',
                              onChanged: (_) => _onAddressChanged(),
                              validator: (v) => (v == null || v.isEmpty) ? 'Obrigatório' : null,
                            ),
                          ),
                          const SizedBox(width: 16),
                          Expanded(
                            child: _buildTextField(
                              controller: _complementController,
                              label: 'Complemento',
                              onChanged: (_) => _onAddressChanged(),
                            ),
                          ),
                        ],
                      ),
                    ],
                  ],
                ),
              ),
              
              const SizedBox(width: 32),
              
              // Right Column: Summary
              Expanded(
                flex: 1,
                child: Container(
                  padding: const EdgeInsets.all(24),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(16),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.black.withValues(alpha: 0.05),
                        blurRadius: 10,
                        offset: const Offset(0, 4),
                      ),
                    ],
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Resumo do Pedido',
                        style: Theme.of(context).textTheme.titleLarge?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 24),
                      ListView.separated(
                        shrinkWrap: true,
                        physics: const NeverScrollableScrollPhysics(),
                        itemCount: cartItems.length,
                        separatorBuilder: (_, __) => const Divider(),
                        itemBuilder: (context, index) {
                          final item = cartItems[index];
                          return Row(
                            children: [
                              Container(
                                width: 50,
                                height: 50,
                                decoration: BoxDecoration(
                                  borderRadius: BorderRadius.circular(8),
                                  color: Colors.grey[200],
                                  image: item.product.imageUrl != null
                                      ? DecorationImage(
                                          image: NetworkImage(item.product.imageUrl!),
                                          fit: BoxFit.cover,
                                        )
                                      : null,
                                ),
                              ),
                              const SizedBox(width: 12),
                              Expanded(
                                child: Column(
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(
                                      item.product.name,
                                      style: const TextStyle(fontWeight: FontWeight.bold),
                                    ),
                                    Text(
                                      'x${item.quantity}',
                                      style: const TextStyle(color: Colors.grey),
                                    ),
                                  ],
                                ),
                              ),
                              Text(
                                'R\$ ${item.totalPrice.toStringAsFixed(2)}',
                                style: const TextStyle(fontWeight: FontWeight.bold),
                              ),
                            ],
                          );
                        },
                      ),
                      const Divider(height: 32),
                      _SummaryRow(
                        label: 'Subtotal',
                        value: cartItems.fold(0, (sum, item) => sum + item.totalPrice),
                      ),
                      const SizedBox(height: 8),
                      // TODO: Implement delivery fee logic
                      const _SummaryRow(label: 'Taxa de Entrega', value: 0.0),
                      const Divider(height: 32),
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          const Text(
                            'Total',
                            style: TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          Text(
                            'R\$ ${cartItems.fold<double>(0, (sum, item) => sum + item.totalPrice).toStringAsFixed(2)}',
                            style: const TextStyle(
                              fontSize: 18,
                              fontWeight: FontWeight.bold,
                              color: AppColors.primary,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 32),
                      SizedBox(
                        width: double.infinity,
                        height: 50,
                        child: ElevatedButton(
                          onPressed: checkoutState.isLoading
                              ? null
                              : () {
                                  if (_formKey.currentState!.validate()) {
                                    ref.read(checkoutProvider.notifier).submitOrder(cartItems);
                                  }
                                },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            foregroundColor: Colors.white,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                            ),
                          ),
                          child: checkoutState.isLoading
                              ? const CircularProgressIndicator(color: Colors.white)
                              : const Text('Confirmar Pedido'),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    ),
  ],
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

  Widget _buildTextField({
    required TextEditingController controller,
    required String label,
    required Function(String) onChanged,
    String? Function(String?)? validator,
    Widget? suffixIcon,
    bool readOnly = false,
  }) {
    return TextFormField(
      controller: controller,
      onChanged: onChanged,
      validator: validator,
      readOnly: readOnly,
      decoration: InputDecoration(
        labelText: label,
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: Colors.grey[300]!),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(8),
          borderSide: BorderSide(color: Colors.grey[300]!),
        ),
        filled: true,
        fillColor: readOnly ? Colors.grey[100] : Colors.white,
        suffixIcon: suffixIcon,
      ),
    );
  }

  Widget _buildDeliveryOption({
    required String label,
    required DeliveryType value,
    required DeliveryType groupValue,
    required IconData icon,
    required Function(DeliveryType) onChanged,
  }) {
    final isSelected = value == groupValue;
    return Expanded(
      child: InkWell(
        onTap: () => onChanged(value),
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 16),
          decoration: BoxDecoration(
            color: isSelected ? AppColors.primary.withValues(alpha: 0.1) : Colors.white,
            border: Border.all(
              color: isSelected ? AppColors.primary : Colors.grey[300]!,
              width: 2,
            ),
            borderRadius: BorderRadius.circular(12),
          ),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(icon, color: isSelected ? AppColors.primary : Colors.grey),
              const SizedBox(width: 8),
              Flexible(
                child: Text(
                  label,
                  style: TextStyle(
                    fontWeight: FontWeight.bold,
                    color: isSelected ? AppColors.primary : Colors.grey[700],
                    fontSize: 14,
                  ),
                  overflow: TextOverflow.ellipsis,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}

class _SummaryRow extends StatelessWidget {
  final String label;
  final double value;

  const _SummaryRow({required this.label, required this.value});

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: const TextStyle(color: Colors.grey)),
        Text('R\$ ${value.toStringAsFixed(2)}', style: const TextStyle(fontWeight: FontWeight.bold)),
      ],
    );
  }
}
