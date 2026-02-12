import { useState } from "react";
import { ArrowLeft, ArrowRight, CreditCard, Banknote, Smartphone, Check, Wallet, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { PaymentMethod } from "@/types/checkout";

interface PaymentMethodSelectorProps {
  selectedMethod: string;
  onMethodChange: (method: string) => void;
  onBack: () => void;
  onNext: () => void;
  onPixPayment?: () => void;
  isProcessing?: boolean;
  error?: string | null;
  subtotal?: number;
  discount?: number;
  totalPrice?: number;
  availableMethods?: string[];
  hasPixIntegration?: boolean;
}

const paymentMethods: PaymentMethod[] = [
  {
    id: 'pix',
    name: 'PIX',
    type: 'pix',
    icon: '💳',
    description: 'Pagamento instantâneo'
  },
  {
    id: 'dinheiro',
    name: 'Dinheiro',
    type: 'cash',
    icon: '💵',
    description: 'Pagamento na entrega'
  },
  {
    id: 'cartao',
    name: 'Cartão',
    type: 'card',
    icon: '💳',
    description: 'Crédito ou Débito'
  }
];

const PaymentMethodSelector = ({
  selectedMethod,
  onMethodChange,
  onBack,
  onNext,
  onPixPayment,
  isProcessing = false,
  error,
  subtotal,
  discount,
  totalPrice,
  availableMethods,
  hasPixIntegration = false
}: PaymentMethodSelectorProps) => {
  const [validationError, setValidationError] = useState<string>('');

  const filteredPaymentMethods = availableMethods
    ? paymentMethods.filter(method => {
      if (method.id === 'pix') return availableMethods.includes('PIX');
      if (method.id === 'dinheiro') return availableMethods.includes('Dinheiro');
      if (method.id === 'cartao') return availableMethods.includes('Crédito') || availableMethods.includes('Débito');
      return true;
    })
    : paymentMethods;

  const handleNext = () => {
    if (!selectedMethod) {
      setValidationError('Selecione um método de pagamento');
      return;
    }
    setValidationError('');

    // Se for PIX, mostrar tela de pagamento PIX
    if (selectedMethod === 'pix' && onPixPayment) {
      onPixPayment();
    } else {
      onNext();
    }
  };

  const handleMethodChange = (method: string) => {
    onMethodChange(method);
    setValidationError('');
  };

  const getMethodIcon = (type: PaymentMethod['type']) => {
    switch (type) {
      case 'pix':
        return <Smartphone className="h-6 w-6" />;
      case 'cash':
        return <Banknote className="h-6 w-6" />;
      case 'card':
        return <CreditCard className="h-6 w-6" />;
      default:
        return <CreditCard className="h-6 w-6" />;
    }
  };

  return (
    <div className="max-w-2xl mx-auto p-0 md:p-1">
      <Card className="border-0 shadow-none md:border md:shadow-sm rounded-[2.5rem] bg-background/50 backdrop-blur-sm overflow-hidden">
        <CardContent className="p-6 md:p-8 space-y-10">
          {error && (
            <div className="p-5 rounded-2xl bg-destructive/10 text-destructive text-xs font-black uppercase tracking-widest border border-destructive/20 animate-pulse">
              {error}
            </div>
          )}

          {validationError && (
            <div className="p-5 rounded-2xl bg-destructive/10 text-destructive text-xs font-black uppercase tracking-widest border border-destructive/20 animate-pulse">
              {validationError}
            </div>
          )}

          {(subtotal !== undefined && totalPrice !== undefined) && (
            <div className="bg-muted/30 p-6 rounded-[2rem] space-y-4">
              <h3 className="font-black text-lg uppercase italic tracking-tighter text-foreground flex items-center gap-2">
                <Wallet className="h-5 w-5 text-primary" />
                Resumo do Pedido
              </h3>
              
              <div className="space-y-3">
                <div className="flex justify-between text-sm font-medium">
                  <span className="text-muted-foreground uppercase tracking-wider text-xs font-bold">Subtotal</span>
                  <span className="font-mono text-base">{new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(subtotal)}</span>
                </div>
                
                {(discount && discount > 0) ? (
                  <div className="flex justify-between text-sm font-medium text-green-600">
                    <span className="uppercase tracking-wider text-xs font-bold">Desconto</span>
                    <span className="font-mono text-base">- {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(discount)}</span>
                  </div>
                ) : null}
                
                <div className="flex justify-between items-end border-t border-border/50 pt-4 mt-2">
                  <span className="font-black text-xl uppercase italic tracking-tighter">Total</span>
                  <span className="font-black text-2xl text-primary">{new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(totalPrice)}</span>
                </div>
              </div>
            </div>
          )}

          <div className="space-y-6">
            <h3 className="font-black text-2xl uppercase italic tracking-tighter text-foreground flex items-center gap-3">
              <div className="bg-primary/10 p-2 rounded-xl">
                <CreditCard className="h-6 w-6 text-primary" />
              </div>
              Como deseja pagar?
            </h3>

            <RadioGroup value={selectedMethod} onValueChange={handleMethodChange} className="grid gap-4">
              {filteredPaymentMethods.map((method) => (
                <label
                  key={method.id}
                  htmlFor={method.id}
                  className={`
                    flex items-center p-6 rounded-[2rem] border-2 cursor-pointer transition-all duration-300 gap-4
                    ${selectedMethod === method.id
                      ? 'border-primary bg-primary/5 shadow-lg shadow-primary/10 scale-[1.02]'
                      : 'border-border/50 bg-white hover:border-primary/30'}
                  `}
                >
                  <RadioGroupItem value={method.id} id={method.id} className="sr-only" />

                  <div className="flex flex-1 items-center justify-between">
                    <div className="flex items-center gap-4">
                      <div className={`p-3 rounded-2xl ${selectedMethod === method.id ? 'bg-primary text-white shadow-lg' : 'bg-muted text-muted-foreground'}`}>
                        {getMethodIcon(method.type)}
                      </div>
                      
                      <div className="flex flex-col">
                        <span className="font-black uppercase tracking-tight text-lg leading-none">{method.name}</span>
                        <span className="text-[10px] font-bold uppercase tracking-widest opacity-60 mt-1">
                          {method.id === 'pix' && !hasPixIntegration
                            ? 'Pagar na entrega'
                            : method.description}
                        </span>
                      </div>
                    </div>
                  </div>

                  {selectedMethod === method.id && (
                    <div className="bg-primary rounded-full p-1 text-white animate-in zoom-in">
                      <Check className="h-4 w-4 stroke-[4]" />
                    </div>
                  )}
                </label>
              ))}
            </RadioGroup>
          </div>

          {/* Informações sobre o método selecionado */}
          {selectedMethod && (
            <div className="p-6 rounded-[2rem] bg-muted/30 border border-border/50 animate-in fade-in slide-in-from-bottom-4">
              {selectedMethod === 'pix' && (
                <div className="space-y-2">
                  <h4 className="font-black text-primary uppercase italic tracking-tight flex items-center gap-2">
                    <Smartphone className="h-4 w-4" />
                    {hasPixIntegration ? "Pagamento via PIX" : "Pagamento via PIX na entrega"}
                  </h4>
                  <p className="text-sm font-medium text-muted-foreground leading-relaxed">
                    {hasPixIntegration
                      ? "Após a confirmação, você receberá um QR Code para pagamento instantâneo."
                      : "O pagamento será feito via PIX no momento da entrega ou retirada do pedido."}
                  </p>
                </div>
              )}

              {selectedMethod === 'dinheiro' && (
                <div className="space-y-2">
                  <h4 className="font-black text-primary uppercase italic tracking-tight flex items-center gap-2">
                    <Banknote className="h-4 w-4" />
                    Pagamento em Dinheiro
                  </h4>
                  <p className="text-sm font-medium text-muted-foreground leading-relaxed">
                    Separe o valor para o pagamento na entrega. Se precisar de troco, avise o entregador.
                  </p>
                </div>
              )}

              {selectedMethod === 'cartao' && (
                <div className="space-y-2">
                  <h4 className="font-black text-primary uppercase italic tracking-tight flex items-center gap-2">
                    <CreditCard className="h-4 w-4" />
                    Pagamento no Cartão
                  </h4>
                  <p className="text-sm font-medium text-muted-foreground leading-relaxed">
                    Levaremos a maquininha até você. Aceitamos crédito e débito.
                  </p>
                </div>
              )}
            </div>
          )}

          {/* Botões de Navegação */}
          <div className="flex flex-col sm:flex-row justify-between gap-4 pt-4">
            <Button
              variant="outline"
              onClick={onBack}
              className="h-16 rounded-2xl font-black uppercase italic tracking-widest gap-3 order-2 sm:order-1 flex items-center shadow-sm hover:bg-muted"
            >
              <ArrowLeft className="h-5 w-5 stroke-[3]" />
              Voltar
            </Button>

            <Button
              onClick={handleNext}
              disabled={isProcessing || !selectedMethod}
              className="h-16 rounded-2xl font-black uppercase italic tracking-widest gap-3 px-8 order-1 sm:order-2 bg-primary hover:bg-primary-hover text-white shadow-xl shadow-primary/20 flex items-center"
            >
              {isProcessing ? (
                <>
                  <Loader2 className="h-5 w-5 animate-spin" />
                  PROCESSANDO...
                </>
              ) : (
                <>
                  {(selectedMethod === 'pix' && hasPixIntegration) ? 'PAGAR COM PIX' : 'FINALIZAR PEDIDO'}
                  <ArrowRight className="h-5 w-5 stroke-[3]" />
                </>
              )}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default PaymentMethodSelector;
