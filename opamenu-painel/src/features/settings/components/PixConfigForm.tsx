import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Loader2, Save } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { useToast } from "@/hooks/use-toast";
import { settingsService } from "../settings.service";
import { EPaymentMethod, EPaymentProvider } from "../types";

const formSchema = z.object({
  provider: z.string(), 
  pixKey: z.string().min(1, "Chave PIX é obrigatória"),
  clientId: z.string().min(1, "Client ID é obrigatório"),
  clientSecret: z.string().min(1, "Client Secret é obrigatório"),
  publicKey: z.string().optional(),
  accessToken: z.string().optional(),
  isSandbox: z.boolean(),
  isActive: z.boolean(),
});

type FormValues = z.infer<typeof formSchema>;

export function PixConfigForm() {
  const { toast } = useToast();
  const queryClient = useQueryClient();

  const { data: config, isLoading } = useQuery({
    queryKey: ["pix-config"],
    queryFn: settingsService.getPixConfig,
    staleTime: 1000 * 60 * 5, // 5 minutes
    refetchOnWindowFocus: false,
  });

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      provider: "0", // MercadoPago default
      pixKey: "",
      clientId: "",
      clientSecret: "",
      publicKey: "",
      accessToken: "",
      isSandbox: true,
      isActive: true,
    },
  });

  useEffect(() => {
    if (config) {
      form.reset({
        provider: config.provider.toString(),
        pixKey: config.pixKey,
        clientId: config.clientId,
        clientSecret: config.clientSecret,
        publicKey: config.publicKey || "",
        accessToken: config.accessToken || "",
        isSandbox: config.isSandbox || false,
        isActive: config.isActive,
      });
    }
  }, [config, form]);

  const mutation = useMutation({
    mutationFn: settingsService.upsertPixConfig,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pix-config"] });
      toast({ title: "Sucesso", description: "Configuração PIX salva." });
    },
    onError: () => toast({ title: "Erro", description: "Falha ao salvar configuração.", variant: "destructive" }),
  });

  const onSubmit = (data: FormValues) => {
    mutation.mutate({
      ...data,
      provider: parseInt(data.provider) as EPaymentProvider,
      paymentMethod: EPaymentMethod.Pix,
    });
  };

  if (isLoading) {
    return <div className="flex h-full items-center justify-center"><Loader2 className="h-8 w-8 animate-spin" /></div>;
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Configuração PIX</CardTitle>
        <CardDescription>Configure o provedor para recebimento via PIX automático.</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          <div className="space-y-2">
            <Label>Provedor</Label>
            <Select 
              onValueChange={(val) => form.setValue("provider", val)} 
              defaultValue={form.watch("provider")}
            >
              <SelectTrigger>
                <SelectValue placeholder="Selecione o provedor" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="0">Mercado Pago</SelectItem>
                <SelectItem value="2">Gerencianet (Efí)</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <Label>Chave PIX</Label>
            <Input {...form.register("pixKey")} placeholder="CPF, Email, Telefone ou Aleatória" />
            {form.formState.errors.pixKey && <p className="text-sm text-destructive">{form.formState.errors.pixKey.message}</p>}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label>Client ID</Label>
              <Input {...form.register("clientId")} type="password" />
              {form.formState.errors.clientId && <p className="text-sm text-destructive">{form.formState.errors.clientId.message}</p>}
            </div>
            <div className="space-y-2">
              <Label>Client Secret</Label>
              <Input {...form.register("clientSecret")} type="password" />
              {form.formState.errors.clientSecret && <p className="text-sm text-destructive">{form.formState.errors.clientSecret.message}</p>}
            </div>
            <div className="space-y-2">
              <Label>Public Key (Mercado Pago)</Label>
              <Input {...form.register("publicKey")} />
              {form.formState.errors.publicKey && <p className="text-sm text-destructive">{form.formState.errors.publicKey.message}</p>}
            </div>
            <div className="space-y-2">
              <Label>Access Token (Mercado Pago)</Label>
              <Input {...form.register("accessToken")} type="password" />
              {form.formState.errors.accessToken && <p className="text-sm text-destructive">{form.formState.errors.accessToken.message}</p>}
            </div>
          </div>

          <div className="flex items-center space-x-2">
            <Switch 
              checked={form.watch("isSandbox")}
              onCheckedChange={(checked) => form.setValue("isSandbox", checked)}
            />
            <Label>Modo Sandbox (Teste)</Label>
          </div>

          <div className="flex items-center space-x-2">
            <Switch 
              checked={form.watch("isActive")}
              onCheckedChange={(checked) => form.setValue("isActive", checked)}
            />
            <Label>Ativo</Label>
          </div>

          <div className="flex justify-end">
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              <Save className="mr-2 h-4 w-4" />
              Salvar Configuração
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
