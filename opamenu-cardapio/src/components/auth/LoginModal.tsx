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
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="text-center text-xl font-bold">Informe seu número de telefone</DialogTitle>
          <DialogDescription className="text-center">
            Ele é importante para falarmos com você caso necessário
          </DialogDescription>
        </DialogHeader>
        
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <FormField
              control={form.control}
              name="phoneNumber"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-[#FF4500] font-bold">Telefone</FormLabel>
                  <FormControl>
                    <Input 
                        placeholder="(00) 90000-0000" 
                        {...field} 
                        onChange={(e) => {
                            const formatted = formatPhone(e.target.value);
                            field.onChange(formatted);
                        }}
                        maxLength={15}
                        className="text-lg h-12 border-[#FF4500] focus-visible:ring-[#FF4500]"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            
            <Button 
                type="submit" 
                className="w-full bg-[#FF4500] hover:bg-[#FF4500]/90 text-white font-bold h-12 text-lg"
                disabled={isLoading}
            >
              {isLoading ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : "CONFIRMAR"}
            </Button>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
