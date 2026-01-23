import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useCustomer } from "@/hooks/use-customer";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Loader2, ArrowLeft, Search } from "lucide-react";

const formSchema = z.object({
  name: z.string().min(3, "Nome é obrigatório"),
  phone: z.string().min(10, "Telefone inválido"),
  email: z.string().email("E-mail inválido").optional().or(z.literal("")),
  postalCode: z.string().optional(),
  street: z.string().optional(),
  streetNumber: z.string().optional(),
  complement: z.string().optional(),
  neighborhood: z.string().optional(),
  city: z.string().optional(),
  state: z.string().optional(),
});

interface RegisterFormProps {
  isOpen: boolean;
  onClose: () => void;
  initialPhone: string;
  onSuccess: () => void;
  onBack: () => void;
}

export function RegisterForm({ isOpen, onClose, initialPhone, onSuccess, onBack }: RegisterFormProps) {
  const { register, isLoading } = useCustomer();
  const [isCepLoading, setIsCepLoading] = useState(false);
  const [addressFieldsDisabled, setAddressFieldsDisabled] = useState(true);
  
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "",
      phone: initialPhone,
      email: "",
      postalCode: "",
      street: "",
      streetNumber: "",
      complement: "",
      neighborhood: "",
      city: "",
      state: "",
    },
  });

  const handleCepChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    let cep = e.target.value.replace(/\D/g, "");
    if (cep.length > 8) cep = cep.slice(0, 8);
    
    // Mask CEP
    const maskedCep = cep.replace(/^(\d{5})(\d)/, "$1-$2");
    form.setValue("postalCode", maskedCep);

    if (cep.length === 8) {
      setIsCepLoading(true);
      try {
        const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`);
        const data = await response.json();

        if (!data.erro) {
          form.setValue("street", data.logradouro);
          form.setValue("neighborhood", data.bairro);
          form.setValue("city", data.localidade);
          form.setValue("state", data.uf);
          // Focus on number field
          const numberInput = document.getElementById("streetNumber");
          if (numberInput) numberInput.focus();
        }
        
        // Always unlock fields after search, regardless of result
        setAddressFieldsDisabled(false);
      } catch (error) {
        console.error("Erro ao buscar CEP", error);
        // Unlock fields on error too, so user can type manually
        setAddressFieldsDisabled(false);
      } finally {
        setIsCepLoading(false);
      }
    } else {
        // If CEP is invalid/incomplete, maybe lock again?
        // User instruction: "Deixei os demais campos travados até o retorno do via cep"
        // So we keep them locked if CEP is not fully typed/searched yet
        if (cep.length < 8) {
            setAddressFieldsDisabled(true);
        }
    }
  };

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    try {
        const cleanPhone = values.phone.replace(/\D/g, "");
        let formattedPhone = cleanPhone;
        if (cleanPhone.length === 11) {
            formattedPhone = `(${cleanPhone.slice(0, 2)}) ${cleanPhone.slice(2, 7)}-${cleanPhone.slice(7)}`;
        } else if (cleanPhone.length === 10) {
            formattedPhone = `(${cleanPhone.slice(0, 2)}) ${cleanPhone.slice(2, 6)}-${cleanPhone.slice(6)}`;
        }

        const cleanCep = values.postalCode?.replace(/\D/g, "");

        await register({
            name: values.name,
            phone: formattedPhone,
            email: values.email || undefined,
            postalCode: cleanCep,
            street: values.street,
            streetNumber: values.streetNumber,
            complement: values.complement,
            neighborhood: values.neighborhood,
            city: values.city,
            state: values.state,
        });
        onSuccess();
    } catch (error) {
        // Error handled in context
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-md max-h-[90vh] overflow-y-auto">
        <DialogHeader className="flex flex-row items-center gap-2">
           <Button variant="ghost" size="icon" className="absolute left-4 top-4" onClick={onBack}>
               <ArrowLeft className="h-4 w-4" />
           </Button>
          <DialogTitle className="text-center text-xl font-bold w-full pt-2">Criar Conta</DialogTitle>
        </DialogHeader>
        <DialogDescription className="text-center">
            Complete seus dados para finalizar o cadastro
        </DialogDescription>
        
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4 pt-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nome Completo</FormLabel>
                  <FormControl>
                    <Input placeholder="Seu nome" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="phone"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Telefone</FormLabel>
                  <FormControl>
                    <Input {...field} disabled />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>E-mail (Opcional)</FormLabel>
                  <FormControl>
                    <Input placeholder="seu@email.com" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-2 gap-4">
                <FormField
                  control={form.control}
                  name="postalCode"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>CEP</FormLabel>
                      <FormControl>
                        <div className="relative">
                            <Input 
                                placeholder="00000-000" 
                                {...field} 
                                onChange={handleCepChange}
                                maxLength={9}
                            />
                            {isCepLoading && (
                                <div className="absolute right-3 top-2.5">
                                    <Loader2 className="h-4 w-4 animate-spin text-muted-foreground" />
                                </div>
                            )}
                        </div>
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                 <FormField
                  control={form.control}
                  name="state"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Estado</FormLabel>
                      <FormControl>
                        <Input placeholder="UF" {...field} disabled={addressFieldsDisabled} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
            </div>

            <div className="grid grid-cols-3 gap-4">
                <FormField
                  control={form.control}
                  name="street"
                  render={({ field }) => (
                    <FormItem className="col-span-2">
                      <FormLabel>Endereço</FormLabel>
                      <FormControl>
                        <Input placeholder="Rua..." {...field} disabled={addressFieldsDisabled} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="streetNumber"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Número</FormLabel>
                      <FormControl>
                        <Input id="streetNumber" placeholder="123" {...field} disabled={addressFieldsDisabled} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
            </div>

            <div className="grid grid-cols-2 gap-4">
                <FormField
                  control={form.control}
                  name="neighborhood"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Bairro</FormLabel>
                      <FormControl>
                        <Input placeholder="Bairro" {...field} disabled={addressFieldsDisabled} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="city"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Cidade</FormLabel>
                      <FormControl>
                        <Input placeholder="Cidade" {...field} disabled={addressFieldsDisabled} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
            </div>

             <FormField
              control={form.control}
              name="complement"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Complemento (Opcional)</FormLabel>
                  <FormControl>
                    <Input placeholder="Apto, Bloco..." {...field} disabled={addressFieldsDisabled} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            
            <Button 
                type="submit" 
                className="w-full bg-primary hover:bg-primary/90 text-white font-bold h-12 text-lg mt-6"
                disabled={isLoading}
            >
              {isLoading ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : "CADASTRAR"}
            </Button>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
