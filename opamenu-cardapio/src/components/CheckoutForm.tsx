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

const formatPhone = (value: string) => {
  const numbers = value.replace(/\D/g, '');
  
  // Se não tiver números suficientes para formatar, retorna como está (apenas números)
  if (numbers.length < 10) return numbers;

  if (numbers.length === 10) {
    return numbers.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
  }
  
  // Para 11 dígitos ou mais
  return numbers.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3');
};

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
          setValidationErrors(prev => ({ ...prev, zipCode: 'CEP não encontrado' }));
        }
      } catch (error) {
        console.error('Erro ao buscar CEP', error);
        setValidationErrors(prev => ({ ...prev, zipCode: 'Erro ao buscar CEP' }));
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

  // Formatar telefone ao carregar se vier sem máscara
  useEffect(() => {
    if (checkoutData.customerPhone) {
      const formatted = formatPhone(checkoutData.customerPhone);
      if (formatted !== checkoutData.customerPhone) {
        onDataChange({ customerPhone: formatted });
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [checkoutData.customerPhone]);

  const handlePhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatPhone(e.target.value);
    onDataChange({ customerPhone: formatted });
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

          {/* Informações do Cliente */}
          <div className="space-y-6">
            <h3 className="font-black text-2xl uppercase italic tracking-tighter text-foreground flex items-center gap-3">
              <div className="bg-primary/10 p-2 rounded-xl">
                <User className="h-6 w-6 text-primary" />
              </div>
              Quem vai comer?
            </h3>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <Label htmlFor="customerName" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">
                  Nome Completo *
                </Label>
                <Input
                  id="customerName"
                  value={checkoutData.customerName}
                  onChange={(e) => onDataChange({ customerName: e.target.value })}
                  placeholder="Seu nome aqui"
                  className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold text-lg px-6 ${validationErrors.customerName ? 'border-destructive' : ''}`}
                />
                {validationErrors.customerName && (
                  <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.customerName}</p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="customerPhone" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">
                  Telefone *
                </Label>
                <Input
                  id="customerPhone"
                  value={checkoutData.customerPhone}
                  onChange={handlePhoneChange}
                  placeholder="(11) 99999-9999"
                  maxLength={15}
                  className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold text-lg px-6 ${validationErrors.customerPhone ? 'border-destructive' : ''}`}
                />
                {validationErrors.customerPhone && (
                  <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.customerPhone}</p>
                )}
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="customerEmail" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">
                E-mail (opcional)
              </Label>
              <Input
                id="customerEmail"
                type="email"
                value={checkoutData.customerEmail || ''}
                onChange={(e) => onDataChange({ customerEmail: e.target.value })}
                placeholder="seu@email.com"
                className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold px-6 ${validationErrors.customerEmail ? 'border-destructive' : ''}`}
              />
              {validationErrors.customerEmail && (
                <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.customerEmail}</p>
              )}
            </div>
          </div>

          {/* Tipo de Entrega */}
          <div className="space-y-6">
            <h3 className="font-black text-2xl uppercase italic tracking-tighter text-foreground flex items-center gap-3">
              <div className="bg-primary/10 p-2 rounded-xl">
                <Truck className="h-6 w-6 text-primary" />
              </div>
              Como quer receber?
            </h3>
            <RadioGroup
              value={checkoutData.isDelivery ? 'delivery' : 'pickup'}
              onValueChange={(value) => onDataChange({ isDelivery: value === 'delivery' })}
              className="grid gap-4"
            >
              <label
                htmlFor="delivery"
                className={`
                  flex items-center p-6 rounded-[2rem] border-2 cursor-pointer transition-all duration-300 gap-4
                  ${checkoutData.isDelivery
                    ? 'border-primary bg-primary/5 shadow-lg shadow-primary/10 scale-[1.02]'
                    : 'border-border/50 bg-white hover:border-primary/30'}  
                `}
              >
                <RadioGroupItem value="delivery" id="delivery" className="sr-only" />
                <div className="flex flex-1 items-center justify-between">
                  <div className="flex items-center gap-4">
                    <div className={`p-3 rounded-2xl ${checkoutData.isDelivery ? 'bg-primary text-white shadow-lg' : 'bg-muted text-muted-foreground'}`}>
                      <Truck className="h-6 w-6" />
                    </div>
                    <div className="flex flex-col">
                      <span className="font-black uppercase tracking-tight text-lg leading-none">Entrega</span>
                      <span className="text-[10px] font-bold uppercase tracking-widest opacity-60">Em domicílio</span>
                    </div>
                  </div>
                  <div className="text-right">
                    <span className="text-sm font-black text-primary uppercase tracking-tighter block">R$ 5,00</span>
                  </div>
                </div>
                {checkoutData.isDelivery && (
                  <div className="bg-primary rounded-full p-1 text-white animate-in zoom-in">
                    <Check className="h-4 w-4 stroke-[4]" />
                  </div>
                )}
              </label>

              <label
                htmlFor="pickup"
                className={`
                  flex items-center p-6 rounded-[2rem] border-2 cursor-pointer transition-all duration-300 gap-4
                  ${!checkoutData.isDelivery
                    ? 'border-accent bg-accent/5 shadow-lg shadow-accent/10 scale-[1.02]'
                    : 'border-border/50 bg-white hover:border-accent/30'}
                `}
              >
                <RadioGroupItem value="pickup" id="pickup" className="sr-only" />
                <div className="flex flex-1 items-center justify-between">
                  <div className="flex items-center gap-4">
                    <div className={`p-3 rounded-2xl ${!checkoutData.isDelivery ? 'bg-accent text-accent-foreground shadow-lg' : 'bg-muted text-muted-foreground'}`}>
                      <Store className="h-6 w-6" />
                    </div>
                    <div className="flex flex-col">
                      <span className="font-black uppercase tracking-tight text-lg leading-none">Retirada</span>
                      <span className="text-[10px] font-bold uppercase tracking-widest opacity-60">No local</span>
                    </div>
                  </div>
                  <div className="text-right">
                    <span className="text-sm font-black text-accent uppercase tracking-tighter block">GRÁTIS</span>
                  </div>
                </div>
                {!checkoutData.isDelivery && (
                  <div className="bg-accent rounded-full p-1 text-accent-foreground animate-in zoom-in">
                    <Check className="h-4 w-4 stroke-[4]" />
                  </div>
                )}
              </label>
            </RadioGroup>
          </div>

          {/* Endereço de Entrega */}
          {checkoutData.isDelivery && (
            <div className="space-y-6 pt-4 animate-in slide-in-from-top duration-500">
              <div className="flex items-center gap-3">
                <div className="bg-primary/10 p-2 rounded-xl">
                  <MapPin className="h-6 w-6 text-primary" />
                </div>
                <h3 className="font-black text-2xl uppercase italic tracking-tighter text-foreground">Endereço</h3>
              </div>

              <div className="space-y-6">
                {/* CEP */}
                <div className="space-y-2">
                  <Label htmlFor="zipCode" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">CEP *</Label>
                  <div className="relative">
                    <Input
                      id="zipCode"
                      value={checkoutData.zipCode || ''}
                      onChange={handleCepChange}
                      placeholder="00000-000"
                      maxLength={9}
                      className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold px-6 pr-14 ${validationErrors.zipCode ? 'border-destructive' : ''}`}
                    />
                    <div className="absolute right-5 top-1/2 -translate-y-1/2 text-primary/40">
                      {isLoadingCep ? (
                        <Loader2 className="h-5 w-5 animate-spin" />
                      ) : (
                        <Search className="h-5 w-5" />
                      )}
                    </div>
                  </div>
                  {validationErrors.zipCode && (
                    <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.zipCode}</p>
                  )}
                </div>

                {/* Rua */}
                <div className="space-y-2">
                  <Label htmlFor="street" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">Rua *</Label>
                  <Input
                    id="street"
                    value={checkoutData.street || ''}
                    onChange={(e) => updateFullAddress({ street: e.target.value })}
                    placeholder="Nome da sua rua"
                    className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold px-6 ${validationErrors.street ? 'border-destructive' : ''}`}
                    disabled={isLoadingCep}
                  />
                  {validationErrors.street && (
                    <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.street}</p>
                  )}
                </div>

                {/* Número e Complemento */}
                <div className="grid grid-cols-2 gap-6">
                  <div className="space-y-2">
                    <Label htmlFor="number" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">Número *</Label>
                    <Input
                      id="number"
                      value={checkoutData.number || ''}
                      onChange={(e) => updateFullAddress({ number: e.target.value })}
                      placeholder="Nº"
                      className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold px-6 ${validationErrors.number ? 'border-destructive' : ''}`}
                    />
                    {validationErrors.number && (
                      <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.number}</p>
                    )}
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="complement" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">Complemento</Label>
                    <Input
                      id="complement"
                      value={checkoutData.complement || ''}
                      onChange={(e) => updateFullAddress({ complement: e.target.value })}
                      placeholder="Apto/Bloco"
                      className="h-14 rounded-2xl border-2 border-border focus:border-primary font-bold px-6"
                    />
                  </div>
                </div>

                {/* Bairro, Cidade e UF */}
                <div className="space-y-6">
                  <div className="space-y-2">
                    <Label htmlFor="neighborhood" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">Bairro *</Label>
                    <Input
                      id="neighborhood"
                      value={checkoutData.neighborhood || ''}
                      onChange={(e) => updateFullAddress({ neighborhood: e.target.value })}
                      placeholder="Bairro"
                      className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold px-6 ${validationErrors.neighborhood ? 'border-destructive' : ''}`}
                      disabled={isLoadingCep}
                    />
                    {validationErrors.neighborhood && (
                      <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.neighborhood}</p>
                    )}
                  </div>

                  <div className="grid grid-cols-[1fr,100px] gap-6">
                    <div className="space-y-2">
                      <Label htmlFor="city" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">Cidade *</Label>
                      <Input
                        id="city"
                        value={checkoutData.city || ''}
                        onChange={(e) => updateFullAddress({ city: e.target.value })}
                        placeholder="Cidade"
                        className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold px-6 ${validationErrors.city ? 'border-destructive' : ''}`}
                        disabled={isLoadingCep}
                      />
                      {validationErrors.city && (
                        <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.city}</p>
                      )}
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="state" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1">UF *</Label>
                      <Input
                        id="state"
                        value={checkoutData.state || ''}
                        onChange={(e) => updateFullAddress({ state: e.target.value })}
                        placeholder="UF"
                        maxLength={2}
                        className={`h-14 rounded-2xl border-2 border-border focus:border-primary font-bold px-6 ${validationErrors.state ? 'border-destructive' : ''}`}
                        disabled={isLoadingCep}
                      />
                      {validationErrors.state && (
                        <p className="text-[10px] font-black uppercase text-destructive tracking-widest ml-4 mt-1">{validationErrors.state}</p>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Observações */}
          <div className="space-y-3">
            <Label htmlFor="notes" className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/60 ml-1 flex items-center gap-2">
              <MessageSquare className="h-4 w-4" />
              Alguma nota especial?
            </Label>
            <Textarea
              id="notes"
              value={checkoutData.notes || ''}
              onChange={(e) => onDataChange({ notes: e.target.value })}
              placeholder="Ex: Ponto de referência, tirar cebola, etc."
              rows={3}
              className="rounded-2xl border-2 border-border focus:border-primary font-medium p-6 min-h-[120px]"
            />
          </div>

          {/* Botões de Navegação */}
          <div className="flex flex-col sm:flex-row justify-between gap-4 pt-10">
            <Button
              variant="outline"
              onClick={onBack}
              className="h-16 rounded-2xl font-black uppercase italic tracking-widest gap-3 order-2 sm:order-1 flex items-center shadow-sm hover:bg-muted"
            >
              <ArrowLeft className="h-5 w-5 stroke-[3]" />
              Voltar
            </Button>

            <Button
              onClick={handleSubmit}
              disabled={isProcessing}
              className="h-16 rounded-2xl font-black uppercase italic tracking-widest gap-3 px-12 order-1 sm:order-2 bg-primary hover:bg-primary-hover text-white shadow-xl shadow-primary/20 flex items-center"
            >
              {isProcessing ? (
                <>
                  <Loader2 className="h-5 w-5 animate-spin" />
                  AGUARDE...
                </>
              ) : (
                <>
                  CONTINUAR
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

export default CheckoutForm;
