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
      <DialogContent className="max-w-md w-[95vw] sm:w-full rounded-[2.5rem] p-0 overflow-hidden border-none shadow-2xl flex flex-col max-h-[90vh]">
        <div className="flex flex-col h-full bg-background overflow-y-auto scrollbar-hide p-8">
          <DialogHeader className="p-0 mb-8 shrink-0 relative">
            <Button
              variant="ghost"
              size="icon"
              className="absolute -left-2 top-0 h-10 w-10 text-muted-foreground hover:bg-muted rounded-full transition-all active:scale-90"
              onClick={onBack}
            >
              <ArrowLeft className="h-5 w-5 stroke-[2.5]" />
            </Button>
            <DialogTitle className="text-2xl font-black uppercase italic tracking-tighter text-foreground text-center w-full pt-1">
              Criar Conta
            </DialogTitle>
            <DialogDescription className="text-center text-xs font-bold text-text-secondary/60 uppercase tracking-widest mt-1">
              Complete seus dados para cadastrar
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
                    <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Telefone</FormLabel>
                    <FormControl>
                      <Input
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
                            onChange={handleCepChange}
                            maxLength={9}
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
                  name="state"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Estado</FormLabel>
                      <FormControl>
                        <Input
                          placeholder="UF"
                          {...field}
                          disabled={addressFieldsDisabled}
                          className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold disabled:opacity-50 uppercase"
                        />
                      </FormControl>
                      <FormMessage className="text-[10px] font-bold" />
                    </FormItem>
                  )}
                />
              </div>

              <div className="grid grid-cols-3 gap-4">
                <FormField
                  control={form.control}
                  name="street"
                  render={({ field }) => (
                    <FormItem className="col-span-2 space-y-1.5">
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
                <FormField
                  control={form.control}
                  name="streetNumber"
                  render={({ field }) => (
                    <FormItem className="space-y-1.5">
                      <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Número</FormLabel>
                      <FormControl>
                        <Input
                          id="streetNumber"
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
              </div>

              <FormField
                control={form.control}
                name="complement"
                render={({ field }) => (
                  <FormItem className="space-y-1.5">
                    <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Complemento (Opcional)</FormLabel>
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

              <Button
                type="submit"
                className="w-full h-16 bg-primary hover:bg-primary-hover text-white rounded-2xl font-black uppercase italic tracking-wider shadow-xl shadow-primary/20 transform transition-all active:scale-95 text-base mt-6"
                disabled={isLoading}
              >
                {isLoading ? (
                  <>
                    <Loader2 className="mr-3 h-5 w-5 animate-spin" />
                    CADASTRANDO...
                  </>
                ) : (
                  "CADASTRAR AGORA"
                )}
              </Button>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}
