import type { Product } from "../products/types";

export const OrderType = {
  Delivery: "Delivery",
  Counter: "Counter",
  Table: "Table"
} as const;

export type OrderType = typeof OrderType[keyof typeof OrderType];

export interface CartItemAddon {
  addonId: string;
  name: string;
  price: number;
  quantity: number;
}

export interface CartItem {
  tempId: string;
  product: Product;
  quantity: number;
  notes?: string;
  addons: CartItemAddon[];
  totalPrice: number;
}
