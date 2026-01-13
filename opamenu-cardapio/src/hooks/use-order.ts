import { useState, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { Order, OrderStatus, CreateOrderRequest } from '@/types/api';
import { OrderRequest } from '@/types/cart';
import { orderService } from '@/services/order-service';
import { handleApiError } from '@/services/http-client';

export interface OrderHookReturn {
  currentOrder: Order | null;
  orderHistory: Order[];
  loading: boolean;
  error: string | null;
  createOrder: (orderData: OrderRequest) => Promise<Order | null>;
  getOrderById: (orderId: number) => Promise<Order | null>;
  clearError: () => void;
  clearCurrentOrder: () => void;
}

export const useOrder = (): OrderHookReturn => {
  const { slug } = useParams<{ slug: string }>();
  const [currentOrder, setCurrentOrder] = useState<Order | null>(null);
  const [orderHistory, setOrderHistory] = useState<Order[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createOrder = useCallback(async (orderData: OrderRequest): Promise<Order | null> => {
    try {
      setLoading(true);
      setError(null);

      // Converter OrderRequest para CreateOrderRequest (formato do backend C#)
      const createRequest: CreateOrderRequest = {
        customerName: orderData.customerName,
        customerPhone: orderData.customerPhone,
        customerEmail: orderData.customerEmail && orderData.customerEmail.trim() ? orderData.customerEmail.trim() : undefined,
        deliveryAddress: orderData.isDelivery && orderData.deliveryAddress ? orderData.deliveryAddress : undefined,
        notes: orderData.notes || '',
        couponCode: orderData.couponCode,
        isDelivery: orderData.isDelivery || false,
        items: orderData.items.map(item => ({
          productId: item.productId,
          quantity: item.quantity,
          notes: item.notes || '',
          addons: item.selectedAddons ? item.selectedAddons.map(addon => ({
            addonId: addon.addonId,
            quantity: addon.quantity
          })) : []
        }))
      };

      const order = await orderService.createOrder(createRequest, slug);
      setCurrentOrder(order);
      
      // Adicionar ao histórico
      setOrderHistory(prev => [order, ...prev]);
      
      return order;
    } catch (err) {
      console.error('Error creating order:', err);
      setError(handleApiError(err));
      return null;
    } finally {
      setLoading(false);
    }
  }, [slug]);

  const getOrderById = useCallback(async (orderId: number): Promise<Order | null> => {
    try {
      setError(null);
      const order = await orderService.getOrderById(orderId, slug);
      
      // Atualizar ordem atual se for a mesma
      if (currentOrder?.id === orderId) {
        setCurrentOrder(order);
      }
      
      return order;
    } catch (err) {
      console.error('Error getting order:', err);
      setError(handleApiError(err));
      return null;
    }
  }, [currentOrder?.id]);

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  const clearCurrentOrder = useCallback(() => {
    setCurrentOrder(null);
  }, []);

  return {
    currentOrder,
    orderHistory,
    loading,
    error,
    createOrder,
    getOrderById,
    clearError,
    clearCurrentOrder,
  };
};
