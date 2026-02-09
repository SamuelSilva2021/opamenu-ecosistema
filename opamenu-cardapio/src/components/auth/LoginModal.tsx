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
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { RegisterForm } from "./RegisterForm";

const formSchema = z.object({
  phoneNumber: z.string().min(10, "Telefone inválido").max(15, "Telefone inválido"),
});

interface LoginModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function LoginModal({ isOpen, onClose }: LoginModalProps) {
  const { login, isLoading } = useCustomer();
  const [showRegister, setShowRegister] = useState(false);
  const [enteredPhone, setEnteredPhone] = useState("");

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      phoneNumber: "",
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    try {
      const cleanPhone = values.phoneNumber.replace(/\D/g, "");
      let formattedPhone = cleanPhone;
      if (cleanPhone.length === 11) {
        formattedPhone = `(${cleanPhone.slice(0, 2)}) ${cleanPhone.slice(2, 7)}-${cleanPhone.slice(7)}`;
      } else if (cleanPhone.length === 10) {
        formattedPhone = `(${cleanPhone.slice(0, 2)}) ${cleanPhone.slice(2, 6)}-${cleanPhone.slice(6)}`;
      }

      const success = await login(formattedPhone);

      if (success) {
        onClose();
      } else {
        setEnteredPhone(formattedPhone);
        setShowRegister(true);
      }
    } catch (error) {
      // Error is handled in context
    }
  };

  const formatPhone = (value: string) => {
    const v = value.replace(/\D/g, "");
    if (v.length > 11) return value.slice(0, 15);

    if (v.length > 10) return `(${v.slice(0, 2)}) ${v.slice(2, 7)}-${v.slice(7)}`;
    if (v.length > 6) return `(${v.slice(0, 2)}) ${v.slice(2, 6)}-${v.slice(6)}`;
    if (v.length > 2) return `(${v.slice(0, 2)}) ${v.slice(2)}`;
    if (v.length > 0) return `(${v}`;
    return "";
  };

  const handleRegisterSuccess = () => {
    setShowRegister(false);
    onClose();
  };

  if (showRegister) {
    return (
      <RegisterForm
        isOpen={isOpen}
        onClose={onClose}
        initialPhone={enteredPhone}
        onSuccess={handleRegisterSuccess}
        onBack={() => setShowRegister(false)}
      />
    );
  }

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-md w-[95vw] sm:w-full rounded-[2.5rem] p-0 overflow-hidden border-none shadow-2xl">
        <div className="bg-background p-8 pt-10">
          <DialogHeader className="p-0 mb-8 shrink-0">
            <DialogTitle className="text-2xl font-black uppercase italic tracking-tighter text-foreground text-center leading-tight px-4">
              Informe seu número de telefone
            </DialogTitle>
            <DialogDescription className="text-center text-xs font-bold text-text-secondary/60 uppercase tracking-widest mt-2 px-8">
              Ele é importante para falarmos com você caso necessário
            </DialogDescription>
          </DialogHeader>

          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              <FormField
                control={form.control}
                name="phoneNumber"
                render={({ field }) => (
                  <FormItem className="space-y-2">
                    <FormLabel className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Telefone</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="(00) 90000-0000"
                        {...field}
                        onChange={(e) => {
                          const formatted = formatPhone(e.target.value);
                          field.onChange(formatted);
                        }}
                        maxLength={15}
                        className="h-16 text-xl rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-6 font-black tracking-wider"
                      />
                    </FormControl>
                    <FormMessage className="text-[10px] font-bold" />
                  </FormItem>
                )}
              />

              <Button
                type="submit"
                className="w-full h-16 bg-primary hover:bg-primary-hover text-white rounded-2xl font-black uppercase italic tracking-wider shadow-xl shadow-primary/20 transform transition-all active:scale-95 text-lg"
                disabled={isLoading}
              >
                {isLoading ? (
                  <>
                    <Loader2 className="mr-3 h-5 w-5 animate-spin" />
                    Buscando...
                  </>
                ) : (
                  "CONFIRMAR"
                )}
              </Button>
            </form>
          </Form>
        </div>
      </DialogContent>
    </Dialog>
  );
}
