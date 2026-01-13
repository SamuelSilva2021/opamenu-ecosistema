export interface CheckoutData {
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  deliveryAddress?: string; // Mantido para compatibilidade, mas idealmente construído a partir dos campos abaixo
  zipCode?: string;
  street?: string;
  number?: string;
  complement?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  isDelivery: boolean;
  notes?: string;
  paymentMethod?: string;
}

export interface PaymentMethod {
  id: string;
  name: string;
  type: 'pix' | 'card' | 'cash';
  icon: string;
  description: string;
}

export interface CheckoutStep {
  id: string;
  name: string;
  completed: boolean;
}

export enum CheckoutSteps {
  CART = 'cart',
  CUSTOMER_INFO = 'customer-info',
  PAYMENT = 'payment',
  CONFIRMATION = 'confirmation'
}
