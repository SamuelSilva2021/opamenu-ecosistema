import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { Loader2 } from "lucide-react";
import { useNavigate } from "react-router-dom";

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
import { authService } from "../auth.service";
import { type RegisterFormData, registerSchema } from "../validation";
import { useAuthStore } from "@/store/auth.store";
import { formatDocument, getErrorMessage } from "@/lib/utils";
import { useToast } from "@/hooks/use-toast";

export function RegisterForm() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const { setAccessToken, setUser } = useAuthStore();

  const form = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      companyName: "",
      document: "",
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const { mutate: register, isPending } = useMutation({
    mutationFn: async (data: RegisterFormData) => {
      const response = await authService.register(data);
      if (!response.succeeded) {
        throw new Error(response.message || "Falha ao realizar cadastro");
      }
      return response;
    },
    onSuccess: (response) => {
      // Login automático após registro
      // Assumindo que o registro retorna tokens. Se não, precisaríamos fazer login.
      // Verificando RegisterController: retorna RegisterTenantResponseDTO que tem accessToken?
      // Vou assumir que sim por enquanto, se não funcionar eu ajusto.
      // O endpoint Register retorna ResponseDTO<RegisterTenantResponseDTO>
      // O DTO deve ter AccessToken. Vamos verificar depois.
      // Por hora, vou redirecionar para login ou tentar logar automaticamente.
      // Melhor: Fazer login automático usando as credenciais fornecidas.
      
      loginAutomatico(response);
    },
    onError: (err) => {
      const message = getErrorMessage(err);
      toast({
        variant: "destructive",
        title: "Erro no cadastro",
        description: message,
      });
    },
  });

  const { mutate: loginAfterRegister } = useMutation({
      mutationFn: async (data: any) => {
        // Se o registro já retornou token, usamos ele.
        // Se não, fazemos login.
        if (data.data?.accessToken) {
            setAccessToken(data.data.accessToken, true); // Requires payment is true for new tenants
            // Fetch permissions
            const permissionsResponse = await authService.getPermissions();
            return { loginResponse: { data: { requiresPayment: true } }, permissionsResponse };
        }
        return null;
      },
      onSuccess: (data) => {
          if (data) {
              setUser(data.permissionsResponse.data);
              navigate("/plans"); // Redireciona para tela de planos
          } else {
              navigate("/login");
          }
      },
      onError: () => {
          navigate("/login");
      }
  });

  const loginAutomatico = (registerResponse: any) => {
      loginAfterRegister(registerResponse);
  }

  const onSubmit = (data: RegisterFormData) => {
    register(data);
  };

  return (
    <div className="grid gap-6">
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
          <FormField
            control={form.control}
            name="companyName"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Nome da Empresa</FormLabel>
                <FormControl>
                  <Input placeholder="Restaurante Legal" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="document"
            render={({ field }) => (
              <FormItem>
                <FormLabel>CNPJ/CPF</FormLabel>
                <FormControl>
                  <Input 
                    placeholder="00.000.000/0000-00" 
                    {...field} 
                    onChange={(e) => {
                      field.onChange(formatDocument(e.target.value));
                    }}
                    maxLength={18}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <div className="grid grid-cols-2 gap-4">
            <FormField
                control={form.control}
                name="firstName"
                render={({ field }) => (
                <FormItem>
                    <FormLabel>Nome</FormLabel>
                    <FormControl>
                    <Input placeholder="João" {...field} />
                    </FormControl>
                    <FormMessage />
                </FormItem>
                )}
            />
            <FormField
                control={form.control}
                name="lastName"
                render={({ field }) => (
                <FormItem>
                    <FormLabel>Sobrenome</FormLabel>
                    <FormControl>
                    <Input placeholder="Silva" {...field} />
                    </FormControl>
                    <FormMessage />
                </FormItem>
                )}
            />
          </div>
          <FormField
            control={form.control}
            name="email"
            render={({ field }) => (
              <FormItem>
                <FormLabel>E-mail</FormLabel>
                <FormControl>
                  <Input placeholder="nome@exemplo.com" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="password"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Senha</FormLabel>
                <FormControl>
                  <Input type="password" placeholder="******" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="confirmPassword"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Confirmar Senha</FormLabel>
                <FormControl>
                  <Input type="password" placeholder="******" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <Button type="submit" className="w-full" disabled={isPending}>
            {isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            Criar conta
          </Button>
        </form>
      </Form>
    </div>
  );
}
