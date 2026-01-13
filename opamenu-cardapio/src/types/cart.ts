import { Product, SelectedAddon } from '@/types/api';

export interface CartItem {
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
  isDelivery?: boolean;
  paymentMethod: 'dinheiro' | 'cartao' | 'pix';
  couponCode?: string;
  items: {
    productId: number;
    quantity: number;
    unitPrice: number;
    selectedAddons?: {
      addonId: number;
      quantity: number;
    }[];
    notes?: string;
  }[];
  notes?: string;
}
