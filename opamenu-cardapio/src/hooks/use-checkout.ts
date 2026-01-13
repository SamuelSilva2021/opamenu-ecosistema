import { useState, useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { CheckoutData, CheckoutSteps } from '@/types/checkout';
import { useOrder } from './use-order';
import { useCart } from './use-cart';
import { OrderRequest } from '@/types/cart';
import { Order } from '@/types/api';

export interface CheckoutHookReturn {
  currentStep: CheckoutSteps;
  checkoutData: CheckoutData;
  isProcessing: boolean;
  error: string | null;
  lastOrder: Order | null;
  showPixPayment: boolean;
  setCurrentStep: (step: CheckoutSteps) => void;
  updateCheckoutData: (data: Partial<CheckoutData>) => void;
  processOrder: (paymentMethodOverride?: string) => Promise<boolean>;
  resetCheckout: () => void;
  handlePixPayment: () => void;
  confirmPixPayment: () => void;
}

const initialCheckoutData: CheckoutData = {
  customerName: '',
  customerPhone: '',
  customerEmail: '',
  deliveryAddress: '',
  zipCode: '',
  street: '',
  number: '',
  complement: '',
  neighborhood: '',
  city: '',
  state: '',
  isDelivery: true,
  notes: ''
};

export const useCheckout = (): CheckoutHookReturn => {
  const { slug } = useParams<{ slug: string }>();
  const [currentStep, setCurrentStep] = useState<CheckoutSteps>(CheckoutSteps.CUSTOMER_INFO);
  const [checkoutData, setCheckoutData] = useState<CheckoutData>(initialCheckoutData);
  const [isProcessing, setIsProcessing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [lastOrder, setLastOrder] = useState<Order | null>(null);
  const [showPixPayment, setShowPixPayment] = useState(false);

  const { createOrder } = useOrder();
  const { items: cartItems, clearCart, coupon } = useCart(slug);

  const updateCheckoutData = useCallback((data: Partial<CheckoutData>) => {
    setCheckoutData(prev => ({ ...prev, ...data }));
    setError(null);
  }, []);

  const processOrder = useCallback(async (paymentMethodOverride?: string): Promise<boolean> => {
    try {
      setIsProcessing(true);
      setError(null);

      // Validar dados
      if (!checkoutData.customerName.trim()) {
        throw new Error('Nome é obrigatório');
      }
      if (!checkoutData.customerPhone.trim()) {
        throw new Error('Telefone é obrigatório');
      }
      if (checkoutData.isDelivery) {
        if (!checkoutData.zipCode || !checkoutData.street || !checkoutData.number || !checkoutData.neighborhood || !checkoutData.city || !checkoutData.state) {
          throw new Error('Endereço completo é obrigatório');
        }
      }
      if (cartItems.length === 0) {
        throw new Error('Carrinho está vazio');
      }

      const paymentMethod = paymentMethodOverride || checkoutData.paymentMethod || 'dinheiro';

      // Converter dados para o formato da API
      const orderRequest: OrderRequest = {
        customerName: checkoutData.customerName.trim(),
        customerPhone: checkoutData.customerPhone.trim(),
        customerEmail: checkoutData.customerEmail?.trim(),
        deliveryAddress: checkoutData.isDelivery ? {
          zipCode: checkoutData.zipCode!,
          street: checkoutData.street!,
          number: checkoutData.number!,
          complement: checkoutData.complement,
          neighborhood: checkoutData.neighborhood!,
          city: checkoutData.city!,
          state: checkoutData.state!
        } : undefined,
        isDelivery: checkoutData.isDelivery,
        paymentMethod: paymentMethod as 'dinheiro' | 'cartao' | 'pix',
        notes: checkoutData.notes?.trim(),
        couponCode: coupon?.code,
        items: cartItems.map(item => ({
          productId: item.product.id,
          quantity: item.quantity,
          unitPrice: item.product.price,
          notes: item.notes,
          selectedAddons: item.selectedAddons?.map(addon => ({
            addonId: addon.addonId,
            quantity: addon.quantity
          }))
        }))
      };

      const order = await createOrder(orderRequest);
      
      if (order) {
        // Armazenar o pedido no estado local do checkout
        setLastOrder(order);
        
        // Limpar o carrinho APÓS o pedido ser criado com sucesso
        clearCart();
        setCurrentStep(CheckoutSteps.CONFIRMATION);
        return true;
      }
      
      return false;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao processar pedido');
      return false;
    } finally {
      setIsProcessing(false);
    }
  }, [checkoutData, cartItems, createOrder, clearCart]);

  const resetCheckout = useCallback(() => {
    setCurrentStep(CheckoutSteps.CUSTOMER_INFO);
    setCheckoutData(initialCheckoutData);
    setIsProcessing(false);
    setError(null);
    setLastOrder(null);
    setShowPixPayment(false);
    // Carrinho já foi limpo após o pedido - não limpar novamente
  }, []);

  const handlePixPayment = useCallback(() => {
    setShowPixPayment(true);
  }, []);

  const confirmPixPayment = useCallback(() => {
    setShowPixPayment(false);
    setCurrentStep(CheckoutSteps.CONFIRMATION);
  }, []);

  return {
    currentStep,
    checkoutData,
    isProcessing,
    error,
    lastOrder,
    showPixPayment,
    setCurrentStep,
    updateCheckoutData,
    processOrder,
    resetCheckout,
    handlePixPayment,
    confirmPixPayment
  };
};
