import { useState, useEffect } from "react";
import { ArrowLeft, ArrowRight, User, Phone, Mail, MapPin, MessageSquare, Truck, Store, Loader2, Search, Check } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { CheckoutData } from "@/types/checkout";

interface CheckoutFormProps {
  checkoutData: CheckoutData;
  onDataChange: (data: Partial<CheckoutData>) => void;
  onBack: () => void;
  onNext: () => void;
  isProcessing?: boolean;
  error?: string | null;
}

const CheckoutForm = ({ 
  checkoutData, 
  onDataChange, 
  onBack, 
  onNext, 
  isProcessing = false,
  error
}: CheckoutFormProps) => {
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});
  const [isLoadingCep, setIsLoadingCep] = useState(false);

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};

    if (!checkoutData.customerName.trim()) {
      errors.customerName = 'Nome é obrigatório';
    }

    if (!checkoutData.customerPhone.trim()) {
      errors.customerPhone = 'Telefone é obrigatório';
    } else if (!/^\(\d{2}\)\s\d{4,5}-\d{4}$/.test(checkoutData.customerPhone.trim())) {
      errors.customerPhone = 'Formato: (11) 99999-9999';
    }

    if (checkoutData.customerEmail && checkoutData.customerEmail.trim()) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(checkoutData.customerEmail.trim())) {
        errors.customerEmail = 'E-mail inválido';
      }
    }

    if (checkoutData.isDelivery) {
      if (!checkoutData.zipCode?.trim()) errors.zipCode = 'CEP é obrigatório';
      if (!checkoutData.street?.trim()) errors.street = 'Rua é obrigatória';
      if (!checkoutData.number?.trim()) errors.number = 'Número é obrigatório';
      if (!checkoutData.neighborhood?.trim()) errors.neighborhood = 'Bairro é obrigatório';
      if (!checkoutData.city?.trim()) errors.city = 'Cidade é obrigatória';
      if (!checkoutData.state?.trim()) errors.state = 'Estado é obrigatório';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const updateFullAddress = (data: Partial<CheckoutData>) => {
    const newData = { ...checkoutData, ...data };
    const addressParts = [
      newData.street,
      newData.number ? `${newData.number}` : null,
      newData.complement ? `(${newData.complement})` : null,
      newData.neighborhood,
      newData.city,
      newData.state,
      newData.zipCode ? `CEP: ${newData.zipCode}` : null
    ].filter(Boolean);
    
    onDataChange({ 
      ...data, 
      deliveryAddress: addressParts.join(', ') 
    });
  };

  const handleCepChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const rawCep = e.target.value.replace(/\D/g, '');
    const formattedCep = rawCep.replace(/^(\d{5})(\d)/, '$1-$2');
    
    updateFullAddress({ zipCode: formattedCep });

    if (rawCep.length === 8) {
      setIsLoadingCep(true);
      try {
        const response = await fetch(`https://viacep.com.br/ws/${rawCep}/json/`);
        const data = await response.json();
        
        if (!data.erro) {
          updateFullAddress({
            zipCode: formattedCep,
            street: data.logradouro,
            neighborhood: data.bairro,
            city: data.localidade,
            state: data.uf,
          });
          setValidationErrors(prev => {
            const newErrors = { ...prev };
            delete newErrors.zipCode;
            return newErrors;
          });
        } else {
          setValidationErrors(prev => ({...prev, zipCode: 'CEP não encontrado'}));
        }
      } catch (error) {
        console.error('Erro ao buscar CEP', error);
        setValidationErrors(prev => ({...prev, zipCode: 'Erro ao buscar CEP'}));
      } finally {
        setIsLoadingCep(false);
      }
    }
  };

  const handleSubmit = () => {
    if (validateForm()) {
      onNext();
    }
  };

  const formatPhone = (value: string) => {
    const numbers = value.replace(/\D/g, '');
    if (numbers.length <= 10) {
      return numbers.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
    }
    return numbers.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
  };

  const handlePhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatPhone(e.target.value);
    onDataChange({ customerPhone: formatted });
  };

  return (
    <div className="max-w-2xl mx-auto p-0 md:p-1">
      <Card className="border-0 shadow-none md:border md:shadow-sm rounded-none md:rounded-xl">
        <CardHeader className="p-4 md:p-6">
          <CardTitle className="flex items-center gap-2">
            <User className="h-5 w-5 text-primary" />
            Dados para Entrega
          </CardTitle>
        </CardHeader>
        
        <CardContent className="p-1 md:p-6 space-y-6">
          {error && (
            <div className="p-3 rounded-lg bg-destructive/10 text-destructive text-sm">
              {error}
            </div>
          )}

          {/* Informações do Cliente */}
          <div className="space-y-4">
            <h3 className="font-semibold text-lg">Informações de Contato</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="customerName" className="flex items-center gap-2">
                  <User className="h-4 w-4" />
                  Nome Completo *
                </Label>
                <Input
                  id="customerName"
                  value={checkoutData.customerName}
                  onChange={(e) => onDataChange({ customerName: e.target.value })}
                  placeholder="Seu nome completo"
                  className={`border-gray-300 ${validationErrors.customerName ? 'border-destructive' : ''}`}
                />
                {validationErrors.customerName && (
                  <p className="text-sm text-destructive">{validationErrors.customerName}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="customerPhone" className="flex items-center gap-2">
                  <Phone className="h-4 w-4" />
                  Telefone *
                </Label>
                <Input
                  id="customerPhone"
                  value={checkoutData.customerPhone}
                  onChange={handlePhoneChange}
                  placeholder="(11) 99999-9999"
                  maxLength={15}
                  className={`border-gray-300 ${validationErrors.customerPhone ? 'border-destructive' : ''}`}
                />
                {validationErrors.customerPhone && (
                  <p className="text-sm text-destructive">{validationErrors.customerPhone}</p>
                )}
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="customerEmail" className="flex items-center gap-2">
                <Mail className="h-4 w-4" />
                E-mail (opcional)
              </Label>
              <Input
                id="customerEmail"
                type="email"
                value={checkoutData.customerEmail || ''}
                onChange={(e) => onDataChange({ customerEmail: e.target.value })}
                placeholder="seu@email.com"
                className={`border-gray-300 ${validationErrors.customerEmail ? 'border-destructive' : ''}`}
              />
              {validationErrors.customerEmail && (
                <p className="text-sm text-destructive">{validationErrors.customerEmail}</p>
              )}
            </div>
          </div>

          {/* Tipo de Entrega */}
          <div className="space-y-4">
            <h3 className="font-semibold text-lg">Tipo de Pedido</h3>
            <RadioGroup 
              value={checkoutData.isDelivery ? 'delivery' : 'pickup'}
              onValueChange={(value) => onDataChange({ isDelivery: value === 'delivery' })}
              className="grid gap-3"
            >
              <label 
                htmlFor="delivery"
                className={`
                  flex items-center p-4 rounded-xl border-2 cursor-pointer transition-all hover:bg-muted/50 gap-3
                  ${checkoutData.isDelivery ? 'border-opamenu-orange bg-opamenu-orange/5' : 'border-border'}  
                `}
              >
                <RadioGroupItem value="delivery" id="delivery" className="sr-only" />
                <div className="flex flex-1 items-center justify-between">
                  <div className="flex items-center gap-2">
                    <Truck className={`h-5 w-5 ${checkoutData.isDelivery ? 'text-opamenu-orange' : 'text-muted-foreground'}`} />
                    <span className="font-medium">Entrega em domicílio</span>
                  </div>
                  <span className="text-sm font-medium text-muted-foreground">Taxa: R$ 5,00</span>
                </div>
                {checkoutData.isDelivery && (
                  <Check className="h-5 w-5 text-opamenu-orange" />
                )}
              </label>

              <label 
                htmlFor="pickup"
                className={`
                  flex items-center p-4 rounded-xl border-2 cursor-pointer transition-all hover:bg-muted/50 gap-3
                  ${!checkoutData.isDelivery ? 'border-opamenu-green bg-opamenu-green/5' : 'border-border'}
                `}
              >
                <RadioGroupItem value="pickup" id="pickup" className="sr-only" />
                <div className="flex flex-1 items-center justify-between">
                  <div className="flex items-center gap-2">
                    <Store className={`h-5 w-5 ${!checkoutData.isDelivery ? 'text-opamenu-green' : 'text-muted-foreground'}`} />
                    <span className="font-medium">Retirada no local</span>
                  </div>
                  <span className="text-sm font-medium text-muted-foreground">Grátis</span>
                </div>
                {!checkoutData.isDelivery && (
                  <Check className="h-5 w-5 text-opamenu-green" />
                )}
              </label>
            </RadioGroup>
          </div>

          {/* Endereço de Entrega */}
          {checkoutData.isDelivery && (
            <div className="space-y-4">
              <div className="flex items-center gap-2">
                <MapPin className="h-5 w-5 text-opamenu-orange" />    
                <h3 className="font-semibold text-lg">Endereço de Entrega</h3>
              </div>
              
              <div className="space-y-4">
                {/* CEP */}
                <div className="space-y-2">
                  <Label htmlFor="zipCode">CEP *</Label>
                  <div className="relative">
                    <Input
                      id="zipCode"
                      value={checkoutData.zipCode || ''}
                      onChange={handleCepChange}
                      placeholder="00000-000"
                      maxLength={9}
                      className={`border-gray-300 ${validationErrors.zipCode ? 'border-destructive pr-10' : 'pr-10'}`}
                    />
                    <div className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground">
                      {isLoadingCep ? (
                        <Loader2 className="h-4 w-4 animate-spin" />
                      ) : (
                        <Search className="h-4 w-4" />
                      )}
                    </div>
                  </div>
                  {validationErrors.zipCode && (
                    <p className="text-sm text-destructive">{validationErrors.zipCode}</p>
                  )}
                </div>

                {/* Rua */}
                <div className="space-y-2">
                  <Label htmlFor="street">Rua *</Label>
                  <Input
                    id="street"
                    value={checkoutData.street || ''}
                    onChange={(e) => updateFullAddress({ street: e.target.value })}
                    placeholder="Nome da rua"
                    className={`border-gray-300 ${validationErrors.street ? 'border-destructive' : ''}`}
                    disabled={isLoadingCep}
                  />
                  {validationErrors.street && (
                    <p className="text-sm text-destructive">{validationErrors.street}</p>
                  )}
                </div>

                {/* Número e Complemento */}
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="number">Número *</Label>
                    <Input
                      id="number"
                      value={checkoutData.number || ''}
                      onChange={(e) => updateFullAddress({ number: e.target.value })}
                      placeholder="123"
                      className={`border-gray-300 ${validationErrors.number ? 'border-destructive' : ''}`}
                    />
                    {validationErrors.number && (
                      <p className="text-sm text-destructive">{validationErrors.number}</p>
                    )}
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="complement">Complemento</Label>
                    <Input
                      id="complement"
                      value={checkoutData.complement || ''}
                      onChange={(e) => updateFullAddress({ complement: e.target.value })}
                      placeholder="Apto 101"
                      className="border-gray-300"
                    />
                  </div>
                </div>

                {/* Bairro, Cidade e UF */}
                <div className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor="neighborhood">Bairro *</Label>
                    <Input
                      id="neighborhood"
                      value={checkoutData.neighborhood || ''}
                      onChange={(e) => updateFullAddress({ neighborhood: e.target.value })}
                      placeholder="Bairro"
                      className={`border-gray-300 ${validationErrors.neighborhood ? 'border-destructive' : ''}`}
                      disabled={isLoadingCep}
                    />
                    {validationErrors.neighborhood && (
                      <p className="text-sm text-destructive">{validationErrors.neighborhood}</p>
                    )}
                  </div>

                  <div className="grid grid-cols-[1fr,80px] gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="city">Cidade *</Label>
                      <Input
                        id="city"
                        value={checkoutData.city || ''}
                        onChange={(e) => updateFullAddress({ city: e.target.value })}
                        placeholder="Cidade"
                        className={`border-gray-300 ${validationErrors.city ? 'border-destructive' : ''}`}
                        disabled={isLoadingCep}
                      />
                      {validationErrors.city && (
                        <p className="text-sm text-destructive">{validationErrors.city}</p>
                      )}
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="state">UF *</Label>
                      <Input
                        id="state"
                        value={checkoutData.state || ''}
                        onChange={(e) => updateFullAddress({ state: e.target.value })}
                        placeholder="UF"
                        maxLength={2}
                        className={`border-gray-300 ${validationErrors.state ? 'border-destructive' : ''}`}
                        disabled={isLoadingCep}
                      />
                      {validationErrors.state && (
                        <p className="text-sm text-destructive">{validationErrors.state}</p>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Observações */}
          <div className="space-y-2">
            <Label htmlFor="notes" className="flex items-center gap-2">
              <MessageSquare className="h-4 w-4" />
              Observações (opcional)
            </Label>
            <Textarea
              id="notes"
              value={checkoutData.notes || ''}
              onChange={(e) => onDataChange({ notes: e.target.value })}
              placeholder="Observações especiais, ponto de referência, etc."
              rows={3}
              className="border-gray-300"
            />
          </div>

          {/* Botões de Navegação */}
          <div className="flex justify-between pt-4">
            <Button 
              variant="outline" 
              onClick={onBack}
              className="flex items-center gap-2"
            >
              <ArrowLeft className="h-4 w-4" />
              Voltar ao Carrinho
            </Button>
            
            <Button 
              onClick={handleSubmit}
              disabled={isProcessing}
              className="flex items-center gap-2 bg-opamenu-orange hover:bg-opamenu-orange/90"
            >
              {isProcessing ? 'Processando...' : 'Continuar'}
              <ArrowRight className="h-4 w-4" />
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default CheckoutForm;
