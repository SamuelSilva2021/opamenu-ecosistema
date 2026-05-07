export const OrderStatus = {
  Pending: 0,
  Preparing: 1,
  Ready: 2,
  OutForDelivery: 3,
  Delivered: 4,
  Cancelled: 5,
  Rejected: 6
} as const;

export type OrderStatus = typeof OrderStatus[keyof typeof OrderStatus];

export interface OrderItemAditional {
  id: string;
  aditionalId: string;
  aditionalName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
}

export interface OrderItem {
  id: string;
  productId: string;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  notes?: string;
  aditionals: OrderItemAditional[];
}

export interface Order {
  id: string;
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  deliveryAddress: string;
  subtotal: number;
  deliveryFee: number;
  discountAmount: number;
  loyaltyDiscountAmount: number;
  loyaltyPointsUsed: number;
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
  orderNumber: number;
  items: OrderItem[];
}

export interface UpdateOrderStatusRequest {
  status: OrderStatus;
}
