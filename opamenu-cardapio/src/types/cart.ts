import { Product, SelectedAddon, EOrderType } from '@/types/api';

export interface CartItem {
  cartItemId?: string; // Identificador único do item no carrinho
  product: Product;
  quantity: number;
  selectedAddons?: SelectedAddon[]; // Adicionais selecionados
  unitPrice: number; // Preço unitário (produto + adicionais)
  totalPrice: number; // Preço total (unitPrice * quantity)
  notes?: string; // Observações do item
}

export interface CartSummary {
  items: CartItem[];
  totalItems: number;
  totalPrice: number;
  subtotal: number;
  discount?: number;
  tax?: number;
  deliveryFee?: number;
}

export interface Address {
  zipCode: string;
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  state: string;
}

export interface OrderRequest {
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  deliveryAddress?: Address;
  orderType: EOrderType;
  paymentMethod: 'dinheiro' | 'cartao' | 'pix';
  couponCode?: string;
  items: {
    productId: string;
    quantity: number;
    unitPrice: number;
    selectedAddons?: {
      addonId: string;
      quantity: number;
    }[];
    notes?: string;
  }[];
  notes?: string;
}
export {
  EOrderType // Preço unitário (produto + adicionais)
};

