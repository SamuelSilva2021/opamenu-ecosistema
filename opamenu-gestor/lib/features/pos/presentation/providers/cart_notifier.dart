import 'package:riverpod_annotation/riverpod_annotation.dart';
import '../../domain/models/cart_item_model.dart';
import '../../domain/models/product_model.dart';

part 'cart_notifier.g.dart';

@riverpod
class CartNotifier extends _$CartNotifier {
  @override
  List<CartItemModel> build() {
    return [];
  }

  void addProduct(ProductModel product) {
    final existingIndex = state.indexWhere((item) => item.product.id == product.id);
    if (existingIndex >= 0) {
      final existingItem = state[existingIndex];
      final updatedItem = existingItem.copyWith(quantity: existingItem.quantity + 1);
      final newState = [...state];
      newState[existingIndex] = updatedItem;
      state = newState;
    } else {
      state = [...state, CartItemModel(product: product)];
    }
  }

  void removeProduct(ProductModel product) {
    final existingIndex = state.indexWhere((item) => item.product.id == product.id);
    if (existingIndex >= 0) {
      final existingItem = state[existingIndex];
      if (existingItem.quantity > 1) {
        final updatedItem = existingItem.copyWith(quantity: existingItem.quantity - 1);
        final newState = [...state];
        newState[existingIndex] = updatedItem;
        state = newState;
      } else {
        final newState = [...state];
        newState.removeAt(existingIndex);
        state = newState;
      }
    }
  }
  
  void deleteItem(ProductModel product) {
    state = state.where((item) => item.product.id != product.id).toList();
  }

  void clearCart() {
    state = [];
  }
}
