import { useState, useEffect } from "react";
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
import { Loader2, LogOut } from "lucide-react";

const formSchema = z.object({
  name: z.string().min(3, "Nome deve ter pelo menos 3 caracteres"),
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

interface ProfileModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function ProfileModal({ isOpen, onClose }: ProfileModalProps) {
  const { customer, updateProfile, logout, isLoading } = useCustomer();
  const [isCepLoading, setIsCepLoading] = useState(false);
  const [addressFieldsDisabled, setAddressFieldsDisabled] = useState(false);
  
  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: "",
      phone: "",
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

  // Preencher formulário quando customer mudar ou modal abrir
  useEffect(() => {
    if (customer && isOpen) {
      form.reset({
        name: customer.name || "",
        phone: customer.phone || "",
        email: customer.email || "",
        postalCode: customer.postalCode || "",
        street: customer.street || "",
        streetNumber: customer.streetNumber || "",
        complement: customer.complement || "",
        neighborhood: customer.neighborhood || "",
        city: customer.city || "",
        state: customer.state || "",
      });
      // Habilitar campos de endereço se já tiver CEP
      if (customer.postalCode) {
        setAddressFieldsDisabled(false);
      }
    }
  }, [customer, isOpen, form]);

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
          const numberInput = document.getElementById("profileStreetNumber");
          if (numberInput) numberInput.focus();
        }
        
        setAddressFieldsDisabled(false);
      } catch (error) {
        console.error("Erro ao buscar CEP", error);
        setAddressFieldsDisabled(false);
      } finally {
        setIsCepLoading(false);
      }
    } else {
        if (cep.length < 8) {
            // Se apagou o CEP, talvez travar? Por enquanto deixamos livre se já tinha antes.
            // Mas seguindo a lógica do cadastro:
            // setAddressFieldsDisabled(true); 
            // Porém na edição o usuário já tem dados, então melhor não travar agressivamente.
        }
    }
  };

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    if (!customer) return;

    try {
        const cleanPhone = values.phone.replace(/\D/g, "");
        let formattedPhone = cleanPhone;
        // Manter formatação existente se não mudou, ou reformatar se mudou (mas phone é readonly)
        if (values.phone === customer.phone) {
            formattedPhone = customer.phone;
        } else {
             if (cleanPhone.length === 11) {
                formattedPhone = `(${cleanPhone.slice(0, 2)}) ${cleanPhone.slice(2, 7)}-${cleanPhone.slice(7)}`;
            } else if (cleanPhone.length === 10) {
                formattedPhone = `(${cleanPhone.slice(0, 2)}) ${cleanPhone.slice(2, 6)}-${cleanPhone.slice(6)}`;
            }
        }

        const cleanCep = values.postalCode?.replace(/\D/g, "");

        await updateProfile({
            id: customer.id,
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
        onClose();
    } catch (error) {
        // Error handled in context
    }
  };

  const handleLogout = () => {
    logout();
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-md max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle className="text-center text-xl font-bold">Meu Perfil</DialogTitle>
          <DialogDescription className="text-center">
            Gerencie suas informações pessoais
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4 py-4">
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
                  <FormLabel>Celular</FormLabel>
                  <FormControl>
                    <Input 
                        placeholder="(00) 00000-0000" 
                        {...field} 
                        disabled 
                        className="bg-gray-100"
                    />
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
                    <Input placeholder="seu@email.com" type="email" {...field} />
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
                                onChange={(e) => {
                                    field.onChange(e);
                                    handleCepChange(e);
                                }}
                            />
                            {isCepLoading && (
                                <Loader2 className="h-4 w-4 animate-spin absolute right-3 top-3 text-gray-400" />
                            )}
                        </div>
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
                            <Input 
                                id="profileStreetNumber"
                                placeholder="123" 
                                {...field} 
                                disabled={addressFieldsDisabled}
                            />
                        </FormControl>
                        <FormMessage />
                        </FormItem>
                    )}
                />
            </div>

            <FormField
              control={form.control}
              name="street"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Endereço</FormLabel>
                  <FormControl>
                    <Input placeholder="Rua..." {...field} disabled={addressFieldsDisabled} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

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
                    name="complement"
                    render={({ field }) => (
                        <FormItem>
                        <FormLabel>Complemento</FormLabel>
                        <FormControl>
                            <Input placeholder="Apto, Bloco..." {...field} disabled={addressFieldsDisabled} />
                        </FormControl>
                        <FormMessage />
                        </FormItem>
                    )}
                />
            </div>

            <div className="grid grid-cols-2 gap-4">
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
                 <FormField
                    control={form.control}
                    name="state"
                    render={({ field }) => (
                        <FormItem>
                        <FormLabel>UF</FormLabel>
                        <FormControl>
                            <Input placeholder="UF" {...field} maxLength={2} disabled={addressFieldsDisabled} />
                        </FormControl>
                        <FormMessage />
                        </FormItem>
                    )}
                />
            </div>

            <Button type="submit" className="w-full h-12 text-base mt-6" disabled={isLoading}>
              {isLoading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Salvando...
                </>
              ) : (
                "Salvar Alterações"
              )}
            </Button>
            
            <div className="relative my-4">
                <div className="absolute inset-0 flex items-center">
                    <span className="w-full border-t border-gray-200" />
                </div>
                <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-white px-2 text-gray-500">Ou</span>
                </div>
            </div>

            <Button 
                type="button" 
                variant="outline" 
                className="w-full h-12 text-base text-red-600 border-red-200 hover:bg-red-50 hover:text-red-700"
                onClick={handleLogout}
            >
                <LogOut className="mr-2 h-4 w-4" />
                Sair da conta
            </Button>

          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
