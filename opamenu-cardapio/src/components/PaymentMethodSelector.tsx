import { useState } from "react";
import { ArrowLeft, ArrowRight, CreditCard, Banknote, Smartphone, Check } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Label } from "@/components/ui/label";
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
}

const paymentMethods: PaymentMethod[] = [
  {
    id: 'pix',
    name: 'PIX',
    type: 'pix',
    icon: '💳',
    description: 'Pagamento instantâneo via PIX'
  },
  {
    id: 'dinheiro',
    name: 'Dinheiro',
    type: 'cash',
    icon: '💵',
    description: 'Pagamento em dinheiro na entrega/retirada'
  },
  {
    id: 'cartao',
    name: 'Cartão',
    type: 'card',
    icon: '💳',
    description: 'Cartão de crédito ou débito'
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
  totalPrice
}: PaymentMethodSelectorProps) => {
  const [validationError, setValidationError] = useState<string>('');

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
        return <Smartphone className="h-6 w-6 text-blue-600" />;
      case 'cash':
        return <Banknote className="h-6 w-6 text-green-600" />;
      case 'card':
        return <CreditCard className="h-6 w-6 text-purple-600" />;
      default:
        return <CreditCard className="h-6 w-6" />;
    }
  };

  return (
    <div className="max-w-2xl mx-auto p-1">
      <Card className="border-0 shadow-none md:border md:shadow-sm rounded-none md:rounded-xl">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <CreditCard className="h-5 w-5 text-opamenu-green" />
            Método de Pagamento
          </CardTitle>
        </CardHeader>
        
        <CardContent className="p-1 md:p-6 space-y-6">
          {error && (
            <div className="p-3 rounded-lg bg-destructive/10 text-destructive text-sm">
              {error}
            </div>
          )}

          {validationError && (
            <div className="p-1 rounded-lg bg-destructive/10 text-destructive text-sm">
              {validationError}
            </div>
          )}

          {(subtotal !== undefined && totalPrice !== undefined) && (
            <div className="bg-muted/30 p-4 rounded-lg space-y-2">
              <h3 className="font-semibold text-lg mb-2">Resumo do Pedido</h3>
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">Subtotal</span>
                <span>{new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(subtotal)}</span>
              </div>
              {(discount && discount > 0) ? (
                <div className="flex justify-between text-sm text-green-600">
                  <span>Desconto</span>
                  <span>- {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(discount)}</span>
                </div>
              ) : null}
              <div className="flex justify-between font-bold text-lg border-t pt-2 mt-2">
                <span>Total</span>
                <span>{new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(totalPrice)}</span>
              </div>
            </div>
          )}

          <div className="space-y-4">
            <h3 className="font-semibold text-lg">Escolha como deseja pagar</h3>
            
            <RadioGroup value={selectedMethod} onValueChange={handleMethodChange}>
              {paymentMethods.map((method) => (
                <div
                  key={method.id}
                  className={`flex items-center gap-3 p-4 rounded-lg border-2 transition-all hover:bg-muted/50 ${
                    selectedMethod === method.id 
                      ? 'border-opamenu-green bg-opamenu-green/5' 
                      : 'border-border'
                  }`}
                >
                  <RadioGroupItem value={method.id} id={method.id} className="sr-only" />
                  
                  <div className="flex items-center gap-3 flex-1">
                    {getMethodIcon(method.type)}
                    
                    <div className="flex-1">
                      <Label htmlFor={method.id} className="cursor-pointer">
                        <div className="font-medium text-base">{method.name}</div>
                        <div className="text-sm text-muted-foreground">
                          {method.description}
                        </div>
                      </Label>
                    </div>

                    {selectedMethod === method.id && (
                      <Check className="h-5 w-5 text-opamenu-green" />
                    )}
                  </div>
                </div>
              ))}
            </RadioGroup>
          </div>

          {/* Informações sobre o método selecionado */}
          {selectedMethod && (
            <div className="p-4 rounded-lg bg-muted/30">
              {selectedMethod === 'pix' && (
                <div className="space-y-2">
                  <h4 className="font-medium text-opamenu-green">Pagamento via PIX</h4>
                  <p className="text-sm text-muted-foreground">
                    Após a confirmação do pedido, você receberá um QR Code para pagamento.
                    O pagamento é processado instantaneamente.
                  </p>
                </div>
              )}
              
              {selectedMethod === 'dinheiro' && (
                <div className="space-y-2">
                  <h4 className="font-medium text-green-600">Pagamento em Dinheiro</h4>
                  <p className="text-sm text-muted-foreground">
                    O pagamento será feito na entrega ou retirada do pedido.
                    Tenha o valor exato ou informe se precisará de troco.
                  </p>
                </div>
              )}
              
              {selectedMethod === 'cartao' && (
                <div className="space-y-2">
                  <h4 className="font-medium text-purple-600">Pagamento no Cartão</h4>
                  <p className="text-sm text-muted-foreground">
                    O pagamento será processado no cartão de crédito ou débito
                    na entrega ou retirada do pedido.
                  </p>
                </div>
              )}
            </div>
          )}

          {/* Botões de Navegação */}
          <div className="flex justify-between pt-4">
            <Button 
              variant="outline" 
              onClick={onBack}
              className="flex items-center gap-2"
            >
              <ArrowLeft className="h-4 w-4" />
              Voltar
            </Button>
            
            <Button 
              onClick={handleNext}
              disabled={isProcessing || !selectedMethod}
              className="flex items-center gap-2 bg-opamenu-green hover:bg-opamenu-green/90"
            >
              {isProcessing ? 'Processando...' : selectedMethod === 'pix' ? 'Pagar com PIX' : 'Finalizar Pedido'}
              <ArrowRight className="h-4 w-4" />
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default PaymentMethodSelector;
