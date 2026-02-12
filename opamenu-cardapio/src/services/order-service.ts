import { httpClient, withApiErrorHandling } from './http-client';
import { API_ENDPOINTS } from '@/config/api';
import { 
  Order, 
  CreateOrderRequest, 
  OrderStatus, 
  ApiResponse, 
  CartItem, 
  AddressDto, 
  SelectedAddon,
  CancelOrderRequest,
  UpdateOrderPaymentRequest,
  UpdateOrderDeliveryTypeRequest,
  PixResponseDto,
  EOrderType
} from '@/types/api';

// Serviço para gerenciar pedidos
export class OrderService {
  // Criar um novo pedido
  async createOrder(orderData: CreateOrderRequest, slug?: string): Promise<Order> {
    return withApiErrorHandling(async () => {
      const endpoint = slug ? API_ENDPOINTS.PUBLIC.ORDERS(slug) : API_ENDPOINTS.ORDERS;
      const response = await httpClient.post<Order>(
        endpoint,
        orderData
      );
      
      return response;
    });
  }

  // Cancelar pedido
  async cancelOrder(orderId: string, data: CancelOrderRequest, slug?: string): Promise<Order> {
    return withApiErrorHandling(async () => {
      // Ajuste de endpoint: Assumindo que o endpoint é api/orders/{id}/cancel
      const endpoint = slug ? `/public/${slug}/orders/${orderId}/cancel` : `/orders/${orderId}/cancel`;
      const response = await httpClient.put<Order>(endpoint, data);
      return response;
    });
  }

  // Atualizar método de pagamento
  async updatePaymentMethod(orderId: string, data: UpdateOrderPaymentRequest, slug?: string): Promise<Order> {
    return withApiErrorHandling(async () => {
      const endpoint = slug ? `/public/${slug}/orders/${orderId}/payment` : `/orders/${orderId}/payment`;
      const response = await httpClient.put<Order>(endpoint, data);
      return response;
    });
  }

  // Atualizar tipo de entrega
  async updateDeliveryType(orderId: string, data: UpdateOrderDeliveryTypeRequest, slug?: string): Promise<Order> {
    return withApiErrorHandling(async () => {
      const endpoint = slug ? `/public/${slug}/orders/${orderId}/delivery-type` : `/orders/${orderId}/delivery-type`;
      const response = await httpClient.put<Order>(endpoint, data);
      return response;
    });
  }

  // Buscar pedido por ID
  async getOrderById(id: string, slug?: string): Promise<Order> {
    return withApiErrorHandling(async () => {
      const endpoint = slug ? API_ENDPOINTS.PUBLIC.ORDER(slug, id) : API_ENDPOINTS.ORDER_BY_ID(id);
      const response = await httpClient.get<Order>(endpoint);
      
      return response;
    });
  }

  // Buscar pedidos do cliente (se implementado no backend)
  async getCustomerOrders(customerPhone: string): Promise<Order[]> {
    return withApiErrorHandling(async () => {
      return httpClient.get<Order[]>(
        API_ENDPOINTS.ORDERS,
        { customerPhone }
      );
    });
  }

  // Buscar pedidos públicos do cliente
  async getPublicOrdersByCustomerId(slug: string, customerId: string): Promise<Order[]> {
    return withApiErrorHandling(async () => {
      return httpClient.get<Order[]>(
        API_ENDPOINTS.PUBLIC.ORDERS_BY_CUSTOMER(slug, customerId)
      );
    });
  }

  // Verificar status do pedido
  async checkOrderStatus(orderId: string, slug?: string): Promise<OrderStatus> {
    const order = await this.getOrderById(orderId, slug);
    return order.status;
  }

  // Gerar pagamento PIX
  async generatePixPayment(orderId: string, slug: string): Promise<PixResponseDto> {
    return withApiErrorHandling(async () => {
      const endpoint = API_ENDPOINTS.PUBLIC.PIX(slug, orderId);
      const response = await httpClient.post<PixResponseDto>(endpoint, {});
      return response;
    });
  }
}

// Instância do serviço
export const orderService = new OrderService();

// Utilitários para pedidos
export const getOrderStatusText = (status: OrderStatus): string => {
  const statusMap: Record<OrderStatus, string> = {
    [OrderStatus.Pending]: 'Aguardando confirmação',
    [OrderStatus.Preparing]: 'Preparando',
    [OrderStatus.Ready]: 'Pronto para retirada',
    [OrderStatus.OutForDelivery]: 'Saiu para entrega',
    [OrderStatus.Delivered]: 'Entregue',
    [OrderStatus.Cancelled]: 'Cancelado',
    [OrderStatus.Rejected]: 'Rejeitado',
  };
  
  return statusMap[status] || 'Status desconhecido';
};

export const parseOrderStatus = (status: string | number): OrderStatus => {
  if (typeof status === 'number') return status;
  
  // Tenta converter string numérica "4" -> 4
  const parsed = Number(status);
  if (!isNaN(parsed)) return parsed;

  const lower = status.toLowerCase();
  if (lower === 'pending') return OrderStatus.Pending;
  if (lower === 'preparing') return OrderStatus.Preparing;
  if (lower === 'ready') return OrderStatus.Ready;
  if (lower === 'outfordelivery') return OrderStatus.OutForDelivery;
  if (lower === 'delivered') return OrderStatus.Delivered;
  if (lower === 'cancelled') return OrderStatus.Cancelled;
  if (lower === 'rejected') return OrderStatus.Rejected;

  return OrderStatus.Pending;
};

export const getOrderStatusColor = (status: OrderStatus): string => {
  const colorMap: Record<OrderStatus, string> = {
    [OrderStatus.Pending]: 'text-yellow-600',
    [OrderStatus.Preparing]: 'text-orange-600',
    [OrderStatus.Ready]: 'text-green-600',
    [OrderStatus.OutForDelivery]: 'text-orange-800',
    [OrderStatus.Delivered]: 'text-green-800',
    [OrderStatus.Cancelled]: 'text-red-600',
    [OrderStatus.Rejected]: 'text-red-800',
  };
  
  return colorMap[status] || 'text-gray-600';
};

export const calculateOrderTotals = (items: any[], deliveryFee: number = 0) => {
  const subtotal = items.reduce((total, item) => {
    return total + (item.unitPrice * item.quantity);
  }, 0);
  
  const total = subtotal + deliveryFee;
  
  return {
    subtotal,
    deliveryFee,
    total,
  };
};

// Validações para criação de pedidos
export const validateOrderData = (orderData: CreateOrderRequest): string[] => {
  const errors: string[] = [];
  
  // Validar dados do cliente
  if (!orderData.customerName?.trim()) {
    errors.push('Nome do cliente é obrigatório');
  }
  
  if (!orderData.customerPhone?.trim()) {
    errors.push('Telefone do cliente é obrigatório');
  } else if (!/^\(\d{2}\)\s\d{4,5}-\d{4}$/.test(orderData.customerPhone)) {
    errors.push('Formato de telefone inválido. Use (11) 99999-9999');
  }
  
  if (orderData.customerEmail && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(orderData.customerEmail)) {
    errors.push('Email inválido');
  }
  
  // Validar itens do pedido
  if (!orderData.items || orderData.items.length === 0) {
    errors.push('Pedido deve ter pelo menos um item');
  }
  
  orderData.items.forEach((item, index) => {
    if (!item.productId || item.productId.length === 0) {
      errors.push(`Item ${index + 1}: ID do produto inválido`);
    }
    
    if (!item.quantity || item.quantity <= 0) {
      errors.push(`Item ${index + 1}: Quantidade deve ser maior que zero`);
    }
  });
  
  // Validar endereço de entrega se necessário
  if (orderData.isDelivery) {
    if (!orderData.deliveryAddress) {
      errors.push('Endereço de entrega é obrigatório para delivery');
    } else {
      const addr = orderData.deliveryAddress;
      if (!addr.street?.trim() || !addr.number?.trim() || !addr.neighborhood?.trim() || !addr.city?.trim() || !addr.state?.trim() || !addr.zipCode?.trim()) {
        errors.push('Endereço de entrega incompleto');
      }
    }
  }
  
  return errors;
};

// Formatação de dados para envio
export const formatOrderForAPI = (
  cartItems: CartItem[],
  customerData: { name: string; phone: string; email?: string },
  isDelivery: boolean = false,
  deliveryAddress?: AddressDto,
  notes?: string
): CreateOrderRequest => {
  return {
    customerName: customerData.name.trim(),
    customerPhone: customerData.phone.trim(),
    customerEmail: customerData.email?.trim() || undefined,
    items: cartItems.map(item => ({
      productId: item.productId,
      quantity: item.quantity,
      notes: item.notes?.trim() || undefined,
      addons: item.selectedAddons ? item.selectedAddons.map((addon: SelectedAddon) => ({
        addonId: addon.addonId,
        quantity: addon.quantity
      })) : []
    })),
    deliveryAddress: isDelivery ? deliveryAddress : undefined,
    notes: notes?.trim() || undefined,
    isDelivery,
    orderType: isDelivery ? EOrderType.Delivery : EOrderType.Counter
  };
};

// Polling para acompanhar status do pedido
export class OrderStatusTracker {
  private intervalId: NodeJS.Timeout | null = null;
  private callbacks: ((status: OrderStatus) => void)[] = [];

  startTracking(orderId: string, slug?: string, intervalMs: number = 30000): void {
    this.stopTracking();
    this.intervalId = setInterval(async () => {
      try {
        const status = await orderService.checkOrderStatus(orderId, slug);
        this.callbacks.forEach(callback => callback(status));
        
        // Parar tracking se pedido foi entregue ou cancelado
        if (status === OrderStatus.Delivered || status === OrderStatus.Cancelled) {
          this.stopTracking();
        }
      } catch (error) {
        console.error('Error tracking order status:', error);
      }
    }, intervalMs);
  }

  stopTracking(): void {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }

  onStatusChange(callback: (status: OrderStatus) => void): void {
    this.callbacks.push(callback);
  }

  removeCallback(callback: (status: OrderStatus) => void): void {
    this.callbacks = this.callbacks.filter(cb => cb !== callback);
  }
}

export const orderStatusTracker = new OrderStatusTracker();
