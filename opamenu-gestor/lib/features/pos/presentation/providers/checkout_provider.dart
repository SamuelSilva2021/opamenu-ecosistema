import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/models/create_order_request_dto.dart';
import '../../domain/models/cart_item_model.dart';
import '../../data/repositories/orders_repository.dart';
import '../../data/repositories/customers_repository.dart';
import '../../data/datasources/viacep_datasource.dart';
import 'package:flutter/foundation.dart';
import '../../domain/enums/delivery_type.dart';

part 'checkout_provider.g.dart';

class CheckoutState {
  final DeliveryType deliveryType;
  final String? tableId;
  final String customerName;
  final String customerPhone;
  final String? customerEmail;
  final AddressDto address;
  final bool isLoading;
  final bool isSearchingCep;
  final String? errorMessage;
  final bool isSuccess;
  final double deliveryFee;

  CheckoutState({
    this.deliveryType = DeliveryType.pickup,
    this.tableId,
    this.customerName = '',
    this.customerPhone = '',
    this.customerEmail,
    required this.address,
    this.isLoading = false,
    this.isSearchingCep = false,
    this.errorMessage,
    this.isSuccess = false,
    this.deliveryFee = 0.0,
  });

  bool get isDelivery => deliveryType == DeliveryType.delivery;

  CheckoutState copyWith({
    DeliveryType? deliveryType,
    String? tableId,
    String? customerName,
    String? customerPhone,
    String? customerEmail,
    AddressDto? address,
    bool? isLoading,
    bool? isSearchingCep,
    String? errorMessage,
    bool? isSuccess,
    double? deliveryFee,
  }) {
    return CheckoutState(
      deliveryType: deliveryType ?? this.deliveryType,
      tableId: tableId ?? this.tableId,
      customerName: customerName ?? this.customerName,
      customerPhone: customerPhone ?? this.customerPhone,
      customerEmail: customerEmail ?? this.customerEmail,
      address: address ?? this.address,
      isLoading: isLoading ?? this.isLoading,
      isSearchingCep: isSearchingCep ?? this.isSearchingCep,
      errorMessage: errorMessage, //Reset mensagem de erro ao submeter
      isSuccess: isSuccess ?? this.isSuccess,
      deliveryFee: deliveryFee ?? this.deliveryFee,
    );
  }
  
  factory CheckoutState.initial() {
    return CheckoutState(address: AddressDto());
  }
}

@riverpod
class Checkout extends _$Checkout {
  late final ViaCepDatasource _viaCepDatasource;

  @override
  CheckoutState build() {
    _viaCepDatasource = ViaCepDatasource();
    return CheckoutState.initial();
  }

  void setDeliveryMode(DeliveryType type) {
    state = state.copyWith(deliveryType: type);
  }

  void setTableId(String? tableId) {
    state = state.copyWith(tableId: tableId);
  }

  void updateCustomerInfo({String? name, String? phone, String? email}) {
    state = state.copyWith(
      customerName: name ?? state.customerName,
      customerPhone: phone != null ? phone.replaceAll(RegExp(r'[^0-9]'), '') : state.customerPhone,
      customerEmail: email ?? state.customerEmail,
    );
  }

  void updateAddress(AddressDto address) {
    state = state.copyWith(address: address);
  }

  void setDeliveryFee(double fee) {
    state = state.copyWith(deliveryFee: fee);
  }

  Future<void> searchCep(String cep) async {
    // Busca o endereço apenas se tiver 8 dígitos numéricos
    final cleanCep = cep.replaceAll(RegExp(r'[^0-9]'), '');
    if (cleanCep.length != 8) return;

    // Previne múltiplas chamadas para o mesmo CEP ou enquanto está carregando
    if (state.isSearchingCep) return;
    
    state = state.copyWith(isSearchingCep: true);

    try {
      final result = await _viaCepDatasource.fetchAddress(cleanCep);
      
      if (result != null) {
        final currentAddress = state.address;
        
        final newAddress = AddressDto(
          zipCode: cep, //Sem formatação específica, apenas números
          street: result.logradouro,
          neighborhood: result.bairro,
          city: result.localidade,
          state: result.uf,
          number: currentAddress.number,
          complement: currentAddress.complement,
        );
        
        state = state.copyWith(
          address: newAddress,
          isSearchingCep: false,
        );
      } else {
         state = state.copyWith(isSearchingCep: false);
      }
    } catch (e) {
      state = state.copyWith(isSearchingCep: false);
    }
  }

  Future<void> searchCustomerByPhone(String phone) async {
    final cleanPhone = phone.replaceAll(RegExp(r'[^0-9]'), '');
    if (cleanPhone.length < 10) return;

    state = state.copyWith(isLoading: true);

    try {
      // Busca o cliente primeiro no repositório de clientes
      final repository = ref.read(customersRepositoryProvider);
      final customer = await repository.getCustomerByPhone(cleanPhone);
      
      if (customer != null) {
        // Se o cliente tiver endereço, preenche o endereço
        AddressDto? address;
        if (customer.street != null) {
          address = AddressDto(
            street: customer.street!,
            number: customer.streetNumber ?? '',
            neighborhood: customer.neighborhood ?? '',
            city: customer.city ?? '',
            state: customer.state ?? '',
            zipCode: customer.postalCode ?? '',
            complement: customer.complement,
          );
        }

        state = state.copyWith(
          customerName: customer.name,
          customerEmail: customer.email,
          address: address ?? state.address,
          isLoading: false,
        );
      } else {
        // Se não encontrar, apenas para de carregar
        state = state.copyWith(isLoading: false);
      }
    } catch (e) {
      // Se ocorrer um erro, apenas para de carregar
      state = state.copyWith(isLoading: false);
    }
  }

  Future<void> submitOrder(List<CartItemModel> cartItems) async {
    if (cartItems.isEmpty) {
      state = state.copyWith(errorMessage: 'Carrinho vazio');
      return;
    }

    state = state.copyWith(isLoading: true, errorMessage: null);

    try {
      final itemsDto = cartItems.map((item) {
        return CreateOrderItemRequestDto(
          productId: item.product.id,
          quantity: item.quantity,
          notes: null, // TODO: Add notes per item support in UI
          addons: [], // TODO: Add addons support in UI
        );
      }).toList();

      final orderDto = CreateOrderRequestDto(
        customerName: state.customerName.isEmpty ? null : state.customerName,
        customerPhone: state.customerPhone.isEmpty ? null : state.customerPhone,
        customerEmail: state.customerEmail,
        deliveryAddress: state.isDelivery ? state.address : null,
        isDelivery: state.isDelivery,
        tableId: state.deliveryType == DeliveryType.table ? state.tableId : null,
        orderType: state.deliveryType.value,
        items: itemsDto,
        deliveryFee: state.isDelivery ? state.deliveryFee : null,
      );

      await ref.read(ordersRepositoryProvider).createOrder(orderDto);
      
      if (kDebugMode) {
        print('Order Submitted: ${orderDto.toJson()}');
      }

      state = state.copyWith(isLoading: false, isSuccess: true);
    } catch (e) {
      state = state.copyWith(isLoading: false, errorMessage: e.toString());
    }
  }
  
  void reset() {
    state = CheckoutState.initial();
  }
}
