import React, { useState } from "react";
import { Input } from "@/components/ui/input";
import { Loader2 } from "lucide-react";
import { toast } from "@/hooks/use-toast";

export interface AddressData {
  logradouro: string;
  bairro: string;
  localidade: string;
  uf: string;
  complemento?: string;
  erro?: boolean;
}

interface ZipCodeInputProps extends Omit<React.ComponentProps<"input">, "onChange"> {
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  onAddressLoaded?: (address: AddressData) => void;
  onLoadingChange?: (isLoading: boolean) => void;
}

export function ZipCodeInput({ 
  value, 
  onChange, 
  onAddressLoaded, 
  onLoadingChange, 
  className, 
  ...props 
}: ZipCodeInputProps) {
  const [isLoading, setIsLoading] = useState(false);

  const handleBlur = async (e: React.FocusEvent<HTMLInputElement>) => {
    const cep = e.target.value.replace(/\D/g, "");
    
    if (cep.length === 8) {
      setIsLoading(true);
      onLoadingChange?.(true);
      
      try {
        const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
        const data = await response.json();

        if (!data.erro) {
          onAddressLoaded?.(data);
        } else {
          toast({
            title: "CEP não encontrado",
            description: "Verifique o CEP informado.",
            variant: "destructive",
          });
        }
      } catch (error) {
        toast({
          title: "Erro na busca",
          description: "Não foi possível buscar o endereço.",
          variant: "destructive",
        });
      } finally {
        setIsLoading(false);
        onLoadingChange?.(false);
      }
    }
    
    props.onBlur?.(e);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    let newValue = e.target.value.replace(/\D/g, "");
    if (newValue.length > 8) newValue = newValue.slice(0, 8);
    if (newValue.length > 5) {
      newValue = newValue.replace(/^(\d{5})(\d)/, "$1-$2");
    }
    
    e.target.value = newValue;
    onChange(e);
  };

  return (
    <div className="relative">
      <Input 
        {...props}
        value={value}
        onChange={handleChange}
        onBlur={handleBlur}
        placeholder="00000-000"
        maxLength={9}
        className={className}
      />
      {isLoading && (
        <div className="absolute right-2 top-2.5">
          <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
        </div>
      )}
    </div>
  );
}
