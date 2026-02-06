import { useState } from "react";
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
import { PasswordInput } from "@/components/ui/password-input";
import { authService } from "../auth.service";
import { type LoginFormData, loginSchema } from "../validation";
import { useAuthStore } from "@/store/auth.store";
import { AuthErrorAlert } from "./AuthErrorAlert";
import type { ErrorDTO } from "../types";

export function LoginForm() {
  const navigate = useNavigate();
  const [error, setError] = useState<unknown>(null);
  const { setAccessToken, setUser } = useAuthStore();

  const form = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      usernameOrEmail: "",
      password: "",
    },
  });

  const { mutate: login, isPending } = useMutation({
    mutationFn: async (data: LoginFormData) => {
      // 1. Login
      const loginResponse = await authService.login(data);
      if (!loginResponse.succeeded) {
        throw loginResponse;
      }

      // 2. Set Token immediately to be available for the next request
      setAccessToken(loginResponse.data.accessToken, loginResponse.data.refreshToken);

      // 3. Fetch Permissions
      const permissionsResponse = await authService.getPermissions();
      if (!permissionsResponse.succeeded) {
        throw new Error(permissionsResponse.message || "Falha ao obter permissões");
      }

      return { loginResponse, permissionsResponse };
    },
    onSuccess: (data) => {
      // 4. Store User Info
      setUser(data.permissionsResponse.data);

      // 5. Navigate
      navigate("/dashboard");
    },
    onError: (err) => {
      setAccessToken("", ""); // Clear token if anything fails
      setError(err);
    },
  });

  const onSubmit = (data: LoginFormData) => {
    setError(null);
    login(data);
  };

  return (
    <div className="grid gap-6">
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
          <FormField
            control={form.control}
            name="usernameOrEmail"
            render={({ field }) => (
              <FormItem>
                <FormLabel>E-mail ou Usuário</FormLabel>
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
                  <PasswordInput placeholder="******" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <AuthErrorAlert error={error as ErrorDTO} />
          <Button type="submit" className="w-full" disabled={isPending}>
            {isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            Entrar com E-mail
          </Button>
        </form>
      </Form>
      {/* <div className="relative">
        <div className="absolute inset-0 flex items-center">
          <span className="w-full border-t" />
        </div>
        <div className="relative flex justify-center text-xs uppercase">
          <span className="bg-background px-2 text-muted-foreground">
            Ou continue com
          </span>
        </div>
      </div>
      <Button variant="outline" type="button" disabled={isPending}>
        Google (Em breve)
      </Button> */}
    </div>
  );
}
