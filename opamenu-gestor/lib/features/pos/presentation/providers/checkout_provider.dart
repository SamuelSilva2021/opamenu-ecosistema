import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/models/create_order_request_dto.dart';
import '../../domain/models/cart_item_model.dart';
import '../../data/repositories/orders_repository.dart';
import '../../data/datasources/viacep_datasource.dart';
import 'package:flutter/foundation.dart';
import '../../domain/enums/delivery_type.dart';

part 'checkout_provider.g.dart';

class CheckoutState {
  final DeliveryType deliveryType;
  final int? tableId;
  final String customerName;
  final String customerPhone;
  final String? customerEmail;
  final AddressDto address;
  final bool isLoading;
  final bool isSearchingCep;
  final String? errorMessage;
  final bool isSuccess;

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
  });

  bool get isDelivery => deliveryType == DeliveryType.delivery;

  CheckoutState copyWith({
    DeliveryType? deliveryType,
    int? tableId,
    String? customerName,
    String? customerPhone,
    String? customerEmail,
    AddressDto? address,
    bool? isLoading,
    bool? isSearchingCep,
    String? errorMessage,
    bool? isSuccess,
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
      errorMessage: errorMessage, // Reset error if not provided (or handle differently)
      isSuccess: isSuccess ?? this.isSuccess,
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

  void setTableId(int? tableId) {
    state = state.copyWith(tableId: tableId);
  }

  void updateCustomerInfo({String? name, String? phone, String? email}) {
    state = state.copyWith(
      customerName: name ?? state.customerName,
      customerPhone: phone ?? state.customerPhone,
      customerEmail: email ?? state.customerEmail,
    );
  }

  void updateAddress(AddressDto address) {
    state = state.copyWith(address: address);
  }

  Future<void> searchCep(String cep) async {
    // Only search if 8 digits
    final cleanCep = cep.replaceAll(RegExp(r'[^0-9]'), '');
    if (cleanCep.length != 8) return;

    // Prevent multiple calls for same CEP or while loading
    if (state.isSearchingCep) return;
    
    state = state.copyWith(isSearchingCep: true);

    try {
      final result = await _viaCepDatasource.fetchAddress(cleanCep);
      
      if (result != null) {
        // Keep existing number/complement if user already typed them
        final currentAddress = state.address;
        
        final newAddress = AddressDto(
          zipCode: cep, // Keep original formatting if provided
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
