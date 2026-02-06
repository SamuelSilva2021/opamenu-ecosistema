import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { ArrowLeft, User, CreditCard, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";
import CheckoutForm from "@/components/CheckoutForm";
import PaymentMethodSelector from "@/components/PaymentMethodSelector";
import OrderConfirmation from "@/components/OrderConfirmation";
import PixPayment from "@/components/PixPayment";
import { CheckoutSteps } from "@/types/checkout";
import { useCheckout } from "@/hooks/use-checkout";
import { useCart } from "@/hooks/use-cart";
import { useCustomer } from "@/hooks/use-customer";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/accordion";
import { TenantBusinessInfo } from "@/types/api";

interface CheckoutPageProps {
  onBackToMenu: () => void;
  tenant?: TenantBusinessInfo;
}

const CheckoutPage = ({ onBackToMenu, tenant }: CheckoutPageProps) => {
  const { slug } = useParams<{ slug: string }>();
  const [selectedPaymentMethod, setSelectedPaymentMethod] = useState<string>('');
  const [activeAccordionItem, setActiveAccordionItem] = useState<string>("step-1");

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

  useEffect(() => {
    setHasAppliedCustomerData(false);
  }, [customer?.id]);

  useEffect(() => {
    if (customer && !hasAppliedCustomerData) {
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
          isDelivery: !!(customer.street && customer.streetNumber && customer.neighborhood)
        });
      }
      setHasAppliedCustomerData(true);
    }
  }, [customer, hasAppliedCustomerData, updateCheckoutData, checkoutData.customerName, checkoutData.customerPhone]);

  // Sincronizar passo do hook com o acordeão
  useEffect(() => {
    if (currentStep === CheckoutSteps.CUSTOMER_INFO) {
      setActiveAccordionItem("step-1");
    } else if (currentStep === CheckoutSteps.PAYMENT) {
      setActiveAccordionItem("step-2");
    }
  }, [currentStep]);

  const handlePaymentMethodNext = async () => {
    updateCheckoutData({
      paymentMethod: selectedPaymentMethod
    });

    const success = await processOrder(selectedPaymentMethod);

    if (success) {
      setCurrentStep(CheckoutSteps.CONFIRMATION);
    }
  };

  const handlePixPaymentFlow = async () => {
    setIsLocalProcessing(true);
    try {
      updateCheckoutData({
        paymentMethod: selectedPaymentMethod
      });

      const order = await processOrder(selectedPaymentMethod, true);

      if (order) {
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

  if (currentStep === CheckoutSteps.CONFIRMATION && lastOrder && !showPixPayment) {
    return (
      <div className="container mx-auto py-0 md:py-6">
        <OrderConfirmation
          order={lastOrder}
          onBackToMenu={onBackToMenu}
          onNewOrder={handleNewOrder}
        />
      </div>
    );
  }

  if (showPixPayment && lastOrder) {
    return (
      <div className="container mx-auto py-0 md:py-6">
        <PixPayment
          orderId={lastOrder.id?.toString() || ''}
          amount={lastOrder.total || 0}
          pixKey={tenant?.pixKey}
          merchantName={tenant?.name}
          merchantCity={tenant?.addressCity}
          qrCodePayload={qrCodePayload}
          slug={slug}
          onPaymentConfirmed={confirmPixPayment}
          onCancel={() => {
            setCurrentStep(CheckoutSteps.PAYMENT);
            resetCheckout();
          }}
        />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <div className="bg-white border-b sticky top-0 z-10">
        <div className="container mx-auto px-4 py-4">
          <div className="flex items-center gap-4">
            <Button
              variant="ghost"
              size="icon"
              onClick={onBackToMenu}
              className="md:hidden"
            >
              <ArrowLeft className="h-5 w-5" />
            </Button>
            <h1 className="text-xl font-semibold">Finalizar Pedido</h1>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="container mx-auto py-6 px-4">
        <Accordion
          type="single"
          collapsible
          value={activeAccordionItem}
          onValueChange={setActiveAccordionItem}
          className="max-w-3xl mx-auto space-y-4"
        >
          {/* Step 1: Entrega e Identificação */}
          <AccordionItem value="step-1" className="border rounded-xl bg-card overflow-hidden">
            <AccordionTrigger className="px-6 py-4 hover:no-underline">
              <div className="flex items-center gap-3">
                <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${currentStep === CheckoutSteps.CUSTOMER_INFO || activeAccordionItem === "step-1"
                  ? 'bg-primary text-white'
                  : 'bg-muted text-muted-foreground'
                  }`}>
                  1
                </div>
                <div className="text-left">
                  <span className="font-semibold block">Entrega e Identificação</span>
                  {checkoutData.customerName && activeAccordionItem !== "step-1" && (
                    <span className="text-xs text-muted-foreground">{checkoutData.customerName} - {checkoutData.customerPhone}</span>
                  )}
                </div>
              </div>
            </AccordionTrigger>
            <AccordionContent className="px-0 pb-0 border-t">
              <CheckoutForm
                checkoutData={checkoutData}
                onDataChange={updateCheckoutData}
                onBack={onBackToMenu}
                onNext={() => {
                  setCurrentStep(CheckoutSteps.PAYMENT);
                  setActiveAccordionItem("step-2");
                }}
                isProcessing={isProcessing}
                error={error}
              />
            </AccordionContent>
          </AccordionItem>

          {/* Step 2: Forma de Pagamento */}
          <AccordionItem value="step-2" className="border rounded-xl bg-card overflow-hidden" disabled={!checkoutData.customerName || !checkoutData.customerPhone}>
            <AccordionTrigger className="px-6 py-4 hover:no-underline">
              <div className="flex items-center gap-3">
                <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${activeAccordionItem === "step-2"
                  ? 'bg-primary text-white'
                  : 'bg-muted text-muted-foreground'
                  }`}>
                  2
                </div>
                <div className="text-left">
                  <span className="font-semibold block">Forma de Pagamento</span>
                  {selectedPaymentMethod && activeAccordionItem !== "step-2" && (
                    <span className="text-xs text-muted-foreground">Método: {selectedPaymentMethod}</span>
                  )}
                </div>
              </div>
            </AccordionTrigger>
            <AccordionContent className="px-0 pb-0 border-t">
              <PaymentMethodSelector
                selectedMethod={selectedPaymentMethod}
                onMethodChange={setSelectedPaymentMethod}
                onBack={() => setActiveAccordionItem("step-1")}
                onNext={handlePaymentMethodNext}
                onPixPayment={handlePixPaymentFlow}
                isProcessing={isProcessing || isLocalProcessing}
                error={error}
                subtotal={subtotal}
                discount={discount}
                totalPrice={totalPrice}
              />
            </AccordionContent>
          </AccordionItem>
        </Accordion>
      </div>
    </div>
  );
};

export default CheckoutPage;
