import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import CheckoutForm from "@/components/CheckoutForm";
import PaymentMethodSelector from "@/components/PaymentMethodSelector";
import OrderConfirmation from "@/components/OrderConfirmation";
import PixPayment from "@/components/PixPayment";
import { CheckoutSteps } from "@/types/checkout";
import { useCheckout } from "@/hooks/use-checkout";
import { useCart } from "@/hooks/use-cart";
import { useCustomer } from "@/hooks/use-customer";

import { TenantBusinessInfo } from "@/types/api";

interface CheckoutPageProps {
  onBackToMenu: () => void;
  tenant?: TenantBusinessInfo;
}

const CheckoutPage = ({ onBackToMenu, tenant }: CheckoutPageProps) => {
  const { slug } = useParams<{ slug: string }>();
  const [selectedPaymentMethod, setSelectedPaymentMethod] = useState<string>('');
  
  const {
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
  } = useCheckout();

  const { totalItems, totalPrice, subtotal, discount } = useCart();
  const { customer } = useCustomer();
  const [hasAppliedCustomerData, setHasAppliedCustomerData] = useState(false);
  const [isLocalProcessing, setIsLocalProcessing] = useState(false);

  // Resetar flag quando o customer mudar (ex: login/logout)
  useEffect(() => {
    setHasAppliedCustomerData(false);
  }, [customer?.id]);

  // Preencher dados do cliente automaticamente se estiver logado
  useEffect(() => {
    if (customer && !hasAppliedCustomerData) {
      // Verificar se os campos principais estão vazios antes de preencher
      // para evitar sobrescrever dados que o usuário possa ter começado a digitar
      // antes do customer carregar (embora improvável com a flag)
      if (!checkoutData.customerName && !checkoutData.customerPhone) {
        updateCheckoutData({
          customerName: customer.name || '',
          customerPhone: customer.phone || '',
          customerEmail: customer.email || '',
          zipCode: customer.postalCode || '',
          street: customer.street || '',
          number: customer.streetNumber || '',
          complement: customer.complement || '',
          neighborhood: customer.neighborhood || '',
          city: customer.city || '',
          state: customer.state || '',
          // Se tiver endereço completo, sugere entrega
          isDelivery: !!(customer.street && customer.streetNumber && customer.neighborhood)
        });
      }
      setHasAppliedCustomerData(true);
    }
  }, [customer, hasAppliedCustomerData, updateCheckoutData, checkoutData.customerName, checkoutData.customerPhone]);

  // Scroll para o topo sempre que mudar de etapa
  useEffect(() => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }, [currentStep]);

  // Comentando temporariamente - problema de sincronização entre hooks
  /*
  // Se não há itens no carrinho e não estamos na confirmação, voltar ao menu
  // Mas só se ainda não processamos um pedido
  useEffect(() => {
    if (totalItems === 0 && currentStep === CheckoutSteps.CUSTOMER_INFO && !currentOrder) {
      onBackToMenu();
    }
  }, [totalItems, currentStep, currentOrder, onBackToMenu]);
  */

  const handlePaymentMethodNext = async () => {
    // Atualizar dados com método de pagamento selecionado
    updateCheckoutData({ 
      paymentMethod: selectedPaymentMethod
    });

    // Processar o pedido
    const success = await processOrder(selectedPaymentMethod);
    
    if (success) {
      setCurrentStep(CheckoutSteps.CONFIRMATION);
    }
  };

  const handlePixPaymentFlow = async () => {
    setIsLocalProcessing(true);
    try {
      // Atualizar dados com método de pagamento PIX
      updateCheckoutData({ 
        paymentMethod: selectedPaymentMethod
      });
      
      // Processar o pedido primeiro, mas não avançar para confirmação ainda
      // Aguardaremos a geração do QR Code
      const order = await processOrder(selectedPaymentMethod, true);
      
      if (order) {
        // Mostrar tela PIX (gera o QR Code e então mostra a tela)
        await handlePixPayment(order);
      }
    } finally {
      setIsLocalProcessing(false);
    }
  };

  const handleNewOrder = () => {
    resetCheckout();
    onBackToMenu();
  };

  const renderStep = () => {
    // Se está mostrando PIX, renderizar componente PIX
    if (showPixPayment && lastOrder) {
      return (
        <PixPayment
          orderId={lastOrder.id?.toString() || ''}
          amount={lastOrder.total || 0}
          pixKey={tenant?.pixKey}
          merchantName={tenant?.name}
          merchantCity={tenant?.addressCity}
          qrCodePayload={qrCodePayload}
          onPaymentConfirmed={confirmPixPayment}
          onCancel={() => {
            setCurrentStep(CheckoutSteps.PAYMENT);
            resetCheckout();
          }}
        />
      );
    }

    switch (currentStep) {
      case CheckoutSteps.CUSTOMER_INFO:
        return (
          <CheckoutForm
            checkoutData={checkoutData}
            onDataChange={updateCheckoutData}
            onBack={onBackToMenu}
            onNext={() => setCurrentStep(CheckoutSteps.PAYMENT)}
            isProcessing={isProcessing}
            error={error}
          />
        );

      case CheckoutSteps.PAYMENT:
        return (
          <PaymentMethodSelector
            selectedMethod={selectedPaymentMethod}
            onMethodChange={setSelectedPaymentMethod}
            onBack={() => setCurrentStep(CheckoutSteps.CUSTOMER_INFO)}
            onNext={handlePaymentMethodNext}
            onPixPayment={handlePixPaymentFlow}
            isProcessing={isProcessing || isLocalProcessing}
            error={error}
            subtotal={subtotal}
            discount={discount}
            totalPrice={totalPrice}
          />
        );

      case CheckoutSteps.CONFIRMATION:
        return (
          <OrderConfirmation
            order={lastOrder}
            onBackToMenu={onBackToMenu}
            onNewOrder={handleNewOrder}
          />
        );

      default:
        return null;
    }
  };

  const getStepTitle = () => {
    switch (currentStep) {
      case CheckoutSteps.CUSTOMER_INFO:
        return 'Dados para Entrega';
      case CheckoutSteps.PAYMENT:
        return 'Método de Pagamento';
      case CheckoutSteps.CONFIRMATION:
        return 'Pedido';
      default:
        return 'Checkout';
    }
  };

  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <div className="bg-white border-b sticky top-0 z-10">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              {currentStep !== CheckoutSteps.CONFIRMATION && (
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => {
                    if (currentStep === CheckoutSteps.PAYMENT) {
                      setCurrentStep(CheckoutSteps.CUSTOMER_INFO);
                    } else {
                      onBackToMenu();
                    }
                  }}
                  className="md:hidden"
                >
                  <ArrowLeft className="h-5 w-5" />
                </Button>
              )}
              <h1 className="text-xl font-semibold">{getStepTitle()}</h1>
            </div>
            
            {/* Progress Indicator */}
            {currentStep !== CheckoutSteps.CONFIRMATION && (
              <div className="hidden md:flex items-center gap-2">
                <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${
                  currentStep === CheckoutSteps.CUSTOMER_INFO 
                    ? 'bg-opamenu-green text-white' 
                    : 'bg-gray-200 text-gray-600'
                }`}>
                  1
                </div>
                <div className="w-8 h-0.5 bg-gray-200"></div>
                <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${
                  currentStep === CheckoutSteps.PAYMENT 
                    ? 'bg-opamenu-green text-white' 
                    : 'bg-gray-200 text-gray-600'
                }`}>
                  2
                </div>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="container mx-auto py-0 md:py-6">
        {renderStep()}
      </div>
    </div>
  );
};

export default CheckoutPage;
