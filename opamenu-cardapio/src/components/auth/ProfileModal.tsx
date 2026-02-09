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
      <DialogContent className="max-w-md w-[95vw] sm:w-full rounded-[2.5rem] p-0 overflow-hidden border-none shadow-2xl flex flex-col max-h-[90vh]">
        <div className="flex flex-col h-full bg-background overflow-y-auto scrollbar-hide p-8">
          <DialogHeader className="p-0 mb-8 shrink-0">
            <DialogTitle className="text-2xl font-black uppercase italic tracking-tighter text-foreground text-center">
              Meu Perfil
            </DialogTitle>
            <DialogDescription className="text-center text-xs font-bold text-text-secondary/60 uppercase tracking-widest mt-1">
              Gerencie suas informações pessoais
            </DialogDescription>
          </DialogHeader>

          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-5">
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem className="space-y-1.5">
                    <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Nome Completo</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="Seu nome"
                        {...field}
                        className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold"
                      />
                    </FormControl>
                    <FormMessage className="text-[10px] font-bold" />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="phone"
                render={({ field }) => (
                  <FormItem className="space-y-1.5">
                    <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Celular</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="(00) 00000-0000"
                        {...field}
                        disabled
                        className="h-14 rounded-2xl border-2 border-border/20 bg-muted/30 px-5 font-bold text-muted-foreground cursor-not-allowed"
                      />
                    </FormControl>
                    <FormMessage className="text-[10px] font-bold" />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem className="space-y-1.5">
                    <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">E-mail (Opcional)</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="seu@email.com"
                        type="email"
                        {...field}
                        className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold"
                      />
                    </FormControl>
                    <FormMessage className="text-[10px] font-bold" />
                  </FormItem>
                )}
              />

              <div className="grid grid-cols-2 gap-4">
                <FormField
                  control={form.control}
                  name="postalCode"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">CEP</FormLabel>
                      <FormControl>
                        <div className="relative">
                          <Input
                            placeholder="00000-000"
                            {...field}
                            onChange={(e) => {
                              field.onChange(e);
                              handleCepChange(e);
                            }}
                            className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold pr-10"
                          />
                          {isCepLoading && (
                            <div className="absolute right-4 top-1/2 -translate-y-1/2">
                              <Loader2 className="h-4 w-4 animate-spin text-primary" />
                            </div>
                          )}
                        </div>
                      </FormControl>
                      <FormMessage className="text-[10px] font-bold" />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="streetNumber"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Número</FormLabel>
                      <FormControl>
                        <Input
                          id="profileStreetNumber"
                          placeholder="123"
                          {...field}
                          disabled={addressFieldsDisabled}
                          className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold disabled:opacity-50"
                        />
                      </FormControl>
                      <FormMessage className="text-[10px] font-bold" />
                    </FormItem>
                  )}
                />
              </div>

              <FormField
                control={form.control}
                name="street"
                render={({ field }) => (
                  <FormItem className="space-y-1.5">
                    <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Endereço</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="Rua..."
                        {...field}
                        disabled={addressFieldsDisabled}
                        className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold disabled:opacity-50"
                      />
                    </FormControl>
                    <FormMessage className="text-[10px] font-bold" />
                  </FormItem>
                )}
              />

              <div className="grid grid-cols-2 gap-4">
                <FormField
                  control={form.control}
                  name="neighborhood"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Bairro</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Bairro"
                          {...field}
                          disabled={addressFieldsDisabled}
                          className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold disabled:opacity-50"
                        />
                      </FormControl>
                      <FormMessage className="text-[10px] font-bold" />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="complement"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Complemento</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Apto, Bloco..."
                          {...field}
                          disabled={addressFieldsDisabled}
                          className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold disabled:opacity-50"
                        />
                      </FormControl>
                      <FormMessage className="text-[10px] font-bold" />
                    </FormItem>
                  )}
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <FormField
                  control={form.control}
                  name="city"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Cidade</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="Cidade"
                          {...field}
                          disabled={addressFieldsDisabled}
                          className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold disabled:opacity-50"
                        />
                      </FormControl>
                      <FormMessage className="text-[10px] font-bold" />
                    </FormItem>
                  )}
                />
                <FormField
                  control={form.control}
                  name="state"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">UF</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="UF"
                          {...field}
                          maxLength={2}
                          disabled={addressFieldsDisabled}
                          className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold disabled:opacity-50 uppercase"
                        />
                      </FormControl>
                      <FormMessage className="text-[10px] font-bold" />
                    </FormItem>
                  )}
                />
              </div>

              <Button
                type="submit"
                className="w-full h-14 bg-primary hover:bg-primary-hover text-white rounded-2xl font-black uppercase italic tracking-wider shadow-xl shadow-primary/20 transform transition-all active:scale-95 text-base mt-6"
                disabled={isLoading}
              >
                {isLoading ? (
                  <>
                    <Loader2 className="mr-3 h-5 w-5 animate-spin" />
                    Salvando...
                  </>
                ) : (
                  "Salvar Alterações"
                )}
              </Button>

              <div className="relative my-8">
                <div className="absolute inset-0 flex items-center">
                  <span className="w-full border-t border-border/50" />
                </div>
                <div className="relative flex justify-center text-[10px] font-black uppercase tracking-[0.3em]">
                  <span className="bg-background px-4 text-text-secondary/30 leading-none">ou</span>
                </div>
              </div>

              <Button
                type="button"
                variant="outline"
                className="w-full h-14 rounded-2xl font-black uppercase italic tracking-widest text-xs text-red-600 border-red-100 hover:bg-red-50 hover:text-red-700 hover:border-red-200 transition-all active:scale-95"
                onClick={handleLogout}
              >
                <LogOut className="mr-3 h-4 w-4 stroke-[2.5]" />
                Sair da conta
              </Button>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}
