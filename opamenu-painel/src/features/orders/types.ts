export const OrderStatus = {
  Pending: 0,
  Confirmed: 1,
  Preparing: 2,
  Ready: 3,
  OutForDelivery: 4,
  Delivered: 5,
  Cancelled: 6,
  Rejected: 7
} as const;

export type OrderStatus = typeof OrderStatus[keyof typeof OrderStatus];

export interface OrderItemAddon {
  id: number;
  addonId: number;
  addonName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

export interface OrderItem {
  id: number;
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  notes?: string;
  addons: OrderItemAddon[];
}

export interface Order {
  id: number;
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  deliveryAddress: string;
  subtotal: number;
  deliveryFee: number;
  discountAmount: number;
  couponCode?: string;
  total: number;
  status: OrderStatus;
  createdAt: string;
  updatedAt: string;
  isDelivery: boolean;
  notes?: string;
  estimatedPreparationMinutes?: number;
  estimatedDeliveryTime?: string;
  queuePosition?: number;
  items: OrderItem[];
}

export interface UpdateOrderStatusRequest {
  status: OrderStatus;
}
