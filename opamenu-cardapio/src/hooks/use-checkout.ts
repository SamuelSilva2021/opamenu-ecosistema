import { useState, useCallback, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import { CheckoutData, CheckoutSteps } from '@/types/checkout';
import { useOrder } from './use-order';
import { useCart } from './use-cart';
import { OrderRequest, EOrderType } from '@/types/cart';
import { Order } from '@/types/api';
import { orderService } from '@/services/order-service';

export interface CheckoutHookReturn {
  currentStep: CheckoutSteps;
  checkoutData: CheckoutData;
  isProcessing: boolean;
  error: string | null;
  lastOrder: Order | null;
  showPixPayment: boolean;
  qrCodePayload: string | undefined;
  setCurrentStep: (step: CheckoutSteps) => void;
  updateCheckoutData: (data: Partial<CheckoutData>) => void;
  processOrder: (paymentMethodOverride?: string, skipConfirmationNavigation?: boolean) => Promise<Order | null>;
  resetCheckout: () => void;
  handlePixPayment: (order?: Order) => Promise<void>;
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

const CHECKOUT_STORAGE_KEY = 'opamenu-checkout-data';

export const useCheckout = (): CheckoutHookReturn => {
  const { neighborhood, slug: urlSlug } = useParams<{ neighborhood: string; slug: string }>();
  const slug = (neighborhood && urlSlug) ? `${neighborhood}/${urlSlug}` : urlSlug;
  const storageKey = slug ? `${CHECKOUT_STORAGE_KEY}-${slug.replace('/', '_')}` : CHECKOUT_STORAGE_KEY;

  const [currentStep, setCurrentStep] = useState<CheckoutSteps>(CheckoutSteps.CUSTOMER_INFO);

  const [checkoutData, setCheckoutData] = useState<CheckoutData>(() => {
    try {
      const saved = localStorage.getItem(storageKey);
      if (saved) {
        return { ...initialCheckoutData, ...JSON.parse(saved) };
      }
    } catch (error) {
      console.error('Error loading checkout data from localStorage:', error);
    }
    return initialCheckoutData;
  });

  const [isProcessing, setIsProcessing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [lastOrder, setLastOrder] = useState<Order | null>(null);
  const [showPixPayment, setShowPixPayment] = useState(false);
  const [qrCodePayload, setQrCodePayload] = useState<string | undefined>(undefined);

  const { createOrder } = useOrder();
  const { items: cartItems, clearCart, coupon, loyaltyPointsUsed, loyaltyProgramId, loyaltyDiscount } = useCart();

  // Persistir dados no localStorage sempre que mudarem
  useEffect(() => {
    try {
      localStorage.setItem(storageKey, JSON.stringify(checkoutData));
    } catch (error) {
      console.error('Error saving checkout data to localStorage:', error);
    }
  }, [checkoutData, storageKey]);

  const updateCheckoutData = useCallback((data: Partial<CheckoutData>) => {
    setCheckoutData(prev => ({ ...prev, ...data }));
    setError(null);
  }, []);

  // Buscar taxa de entrega quando cidade/bairro mudar
  useEffect(() => {
    if (checkoutData.isDelivery && checkoutData.city && slug) {
      const fetchFee = async () => {
        try {
          const fee = await orderService.getDeliveryFee(slug, checkoutData.city!, checkoutData.neighborhood);
          // Só atualiza se o valor for diferente para evitar loops
          if (fee !== checkoutData.deliveryFee) {
            updateCheckoutData({ deliveryFee: fee ?? 0 });
          }
        } catch (error) {
          console.error('Error fetching delivery fee:', error);
        }
      };
      
      const timer = setTimeout(fetchFee, 500); // Debounce
      return () => clearTimeout(timer);
    } else if (!checkoutData.isDelivery && checkoutData.deliveryFee !== 0) {
      updateCheckoutData({ deliveryFee: 0 });
    }
  }, [checkoutData.isDelivery, checkoutData.city, checkoutData.neighborhood, slug, checkoutData.deliveryFee, updateCheckoutData]);

  const processOrder = useCallback(async (paymentMethodOverride?: string, skipConfirmationNavigation: boolean = false): Promise<Order | null> => {
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
        orderType: checkoutData.isDelivery ? EOrderType.Delivery : EOrderType.Counter,
        paymentMethod: paymentMethod as 'dinheiro' | 'cartao' | 'pix',
        notes: checkoutData.notes?.trim(),
        couponCode: coupon?.code,
        loyaltyPointsUsed: loyaltyPointsUsed > 0 ? loyaltyPointsUsed : undefined,
        loyaltyProgramId: loyaltyPointsUsed > 0 ? loyaltyProgramId : undefined,
        loyaltyDiscount: loyaltyDiscount && loyaltyDiscount > 0 ? loyaltyDiscount : undefined,
        deliveryFee: checkoutData.isDelivery ? (checkoutData.deliveryFee ?? 0) : 0,
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

        // Limpar dados do localStorage após sucesso
        localStorage.removeItem(storageKey);

        // Limpar o carrinho APÓS o pedido ser criado com sucesso
        clearCart();

        if (!skipConfirmationNavigation) {
          setCurrentStep(CheckoutSteps.CONFIRMATION);
        }

        return order;
      }

      return null;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao processar pedido');
      return null;
    } finally {
      setIsProcessing(false);
    }
  }, [checkoutData, cartItems, createOrder, clearCart, storageKey, coupon, loyaltyPointsUsed, loyaltyProgramId, loyaltyDiscount]);

  const resetCheckout = useCallback(() => {
    setCurrentStep(CheckoutSteps.CUSTOMER_INFO);
    setCheckoutData(initialCheckoutData);
    setIsProcessing(false);
    setError(null);
    setLastOrder(null);
    setShowPixPayment(false);

    // Limpar dados do localStorage
    localStorage.removeItem(storageKey);
  }, [storageKey]);

  const handlePixPayment = useCallback(async (orderOverride?: Order) => {
    const order = orderOverride || lastOrder;

    if (!order?.id || !slug) {
      setShowPixPayment(true);
      return;
    }

    try {
      setIsProcessing(true);
      const pixData = await orderService.generatePixPayment(order.id, slug);
      setQrCodePayload(pixData.qrCode);
      setShowPixPayment(true);
    } catch (error) {
      console.error('Error generating PIX:', error);
      // Se não houver integração PIX ou ocorrer erro na geração, avançamos para a confirmação
      // O lojista receberá o pedido com método PIX e poderá tratar manualmente
      setCurrentStep(CheckoutSteps.CONFIRMATION);
    } finally {
      setIsProcessing(false);
    }
  }, [lastOrder, slug]);

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
    qrCodePayload,
    setCurrentStep,
    updateCheckoutData,
    processOrder,
    resetCheckout,
    handlePixPayment,
    confirmPixPayment
  };
};
