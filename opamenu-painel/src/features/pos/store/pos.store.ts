import { create } from "zustand";
import { persist } from "zustand/middleware";
import { OrderType } from "../types";
import type { CartItem, CartItemAddon } from "../types";
import type { Product } from "../../products/types";
import type { Customer } from "../../customers/types";

interface POSState {
  items: CartItem[];
  customer: Customer | null;
  orderType: OrderType;
  tableId?: string;
  deliveryFee: number;
  discount: number;
  couponCode?: string;

  addItem: (product: Product, quantity: number, notes?: string, addons?: CartItemAddon[]) => void;
  removeItem: (tempId: string) => void;
  updateItemQuantity: (tempId: string, quantity: number) => void;
  setCustomer: (customer: Customer | null) => void;
  setOrderType: (type: OrderType) => void;
  setTableId: (id?: string) => void;
  setDeliveryFee: (fee: number) => void;
  setDiscount: (amount: number) => void;
  setCouponCode: (code?: string) => void;
  clearCart: () => void;
  
  getSubtotal: () => number;
  getTotal: () => number;
}

export const usePOSStore = create<POSState>()(
  persist(
    (set, get) => ({
      items: [],
      customer: null,
      orderType: OrderType.Counter,
      deliveryFee: 0,
      discount: 0,
      
      addItem: (product, quantity, notes, addons = []) => {
        const addonsTotal = addons.reduce((acc, addon) => acc + (addon.price * addon.quantity), 0);
        const itemTotal = (product.price * quantity) + addonsTotal;
        
        const newItem: CartItem = {
          tempId: crypto.randomUUID(),
          product,
          quantity,
          notes,
          addons,
          totalPrice: itemTotal
        };
        
        set((state) => ({ items: [...state.items, newItem] }));
      },
      
      removeItem: (tempId) => {
        set((state) => ({ items: state.items.filter(i => i.tempId !== tempId) }));
      },
      
      updateItemQuantity: (tempId, quantity) => {
        if (quantity <= 0) {
            get().removeItem(tempId);
            return;
        }
        
        set((state) => {
          const items = state.items.map(item => {
            if (item.tempId === tempId) {
               const addonsTotal = item.addons.reduce((acc, addon) => acc + (addon.price * addon.quantity), 0);
               return {
                 ...item,
                 quantity,
                 totalPrice: (item.product.price * quantity) + addonsTotal
               };
            }
            return item;
          });
          return { items };
        });
      },
      
      setCustomer: (customer) => set({ customer }),
      setOrderType: (orderType) => set({ orderType }),
      setTableId: (tableId) => set({ tableId }),
      setDeliveryFee: (deliveryFee) => set({ deliveryFee }),
      setDiscount: (discount) => set({ discount }),
      setCouponCode: (couponCode) => set({ couponCode }),
      clearCart: () => set({ items: [], customer: null, orderType: OrderType.Counter, tableId: undefined, deliveryFee: 0, discount: 0, couponCode: undefined }),
      
      getSubtotal: () => {
        return get().items.reduce((acc, item) => acc + item.totalPrice, 0);
      },
      
      getTotal: () => {
        const subtotal = get().getSubtotal();
        const { deliveryFee, discount } = get();
        return Math.max(0, subtotal + deliveryFee - discount);
      }
    }),
    {
      name: "pos-cart-storage",
    }
  )
);
