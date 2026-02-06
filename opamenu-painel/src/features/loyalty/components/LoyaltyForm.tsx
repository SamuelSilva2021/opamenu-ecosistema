import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Loader2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { loyaltyService } from "../loyalty.service";
import type { LoyaltyProgram } from "../types";

const formSchema = z.object({
  name: z.string().min(3, "Nome é obrigatório"),
  description: z.string().optional(),
  pointsPerCurrency: z.coerce.number().min(0.1, "Deve ser maior que 0"),
  currencyValue: z.coerce.number().min(0.1, "Deve ser maior que 0"),
  minOrderValue: z.coerce.number().min(0, "Não pode ser negativo"),
  // Aceita string vazia ou undefined e converte para number | undefined
  pointsValidityDays: z.union([
    z.coerce.number().min(1, "Mínimo 1 dia"),
    z.literal(""),
    z.undefined(),
    z.null()
  ]).transform(val => (val === "" || val === null || val === undefined) ? undefined : Number(val)),
  isActive: z.boolean().default(true),
});

type FormValues = z.infer<typeof formSchema>;

interface LoyaltyFormProps {
  initialData?: LoyaltyProgram | null;
  onSubmit: (data: FormValues) => void;
  isLoading?: boolean;
  readOnly?: boolean;
}

export function LoyaltyForm({ initialData, onSubmit, isLoading, readOnly }: LoyaltyFormProps) {
  const { toast } = useToast();
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema) as any,
    defaultValues: {
      name: "",
      description: "",
      pointsPerCurrency: 1,
      currencyValue: 0,
      minOrderValue: 0,
      pointsValidityDays: undefined,
      isActive: true,
    },
  });

  useEffect(() => {
    if (initialData) {
      form.reset({
        name: initialData.name,
        description: initialData.description || "",
        pointsPerCurrency: initialData.pointsPerCurrency,
        currencyValue: initialData.currencyValue,
        minOrderValue: initialData.minOrderValue,
        pointsValidityDays: initialData.pointsValidityDays,
        isActive: initialData.isActive,
      });
    }
  }, [initialData, form]);

  return (
    <Card>
      <CardHeader>
        <CardTitle>Configuração do Programa</CardTitle>
        <CardDescription>
          Defina como seus clientes ganham pontos.
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit as any)} className="space-y-6">
            <FormField
              control={form.control as any}
              name="isActive"
              render={({ field }) => (
                <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                  <div className="space-y-0.5">
                    <FormLabel className="text-base">Programa Ativo</FormLabel>
                    <FormDescription>
                      Ative ou desative o acúmulo de pontos para novos pedidos.
                    </FormDescription>
                  </div>
                  <FormControl>
                    <Switch
                      disabled={readOnly}
                      checked={field.value}
                      onCheckedChange={async (checked) => {
                        field.onChange(checked);

                        // Se já existe um ID (edição), chama o endpoint de toggle imediatamente
                        if (initialData?.id) {
                          try {
                            await loyaltyService.toggleStatus(initialData.id, checked);
                            toast({
                              title: "Sucesso",
                              description: `Programa ${checked ? "ativado" : "desativado"} com sucesso.`,
                              variant: "success",
                            });
                          } catch (error) {
                            field.onChange(!checked); // Reverte o switch em caso de erro
                            toast({
                              title: "Erro",
                              description: "Erro ao alterar status do programa.",
                              variant: "destructive",
                            });
                          }
                        }
                      }}
                    />
                  </FormControl>
                </FormItem>
              )}
            />

            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <FormField
                control={form.control as any}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Nome do Programa</FormLabel>
                    <FormControl>
                      <Input placeholder="Ex: Clube de Vantagens" {...field} disabled={readOnly} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control as any}
                name="currencyValue"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>R$ - Moeda por ponto</FormLabel>
                    <FormControl>
                      <Input step="0.1" {...field} placeholder="Ex: 1 real valeu 1 ponto" disabled={readOnly} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control as any}
                name="pointsPerCurrency"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Pontos por moeda</FormLabel>
                    <FormControl>
                      <Input type="number" step="0.1" {...field} disabled={readOnly} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control as any}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Descrição</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Descreva os benefícios do programa..."
                      className="resize-none"
                      {...field}
                      disabled={readOnly}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control as any}
                name="minOrderValue"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Valor Mínimo do Pedido (R$)</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        step="0.01"
                        {...field}
                        onChange={e => field.onChange(Number(e.target.value))}
                        disabled={readOnly}
                      />
                    </FormControl>
                    <FormDescription>
                      Valor mínimo para começar a pontuar.
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control as any}
                name="pointsValidityDays"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Validade dos Pontos (dias)</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        {...field}
                        value={field.value ?? ""}
                        onChange={(e) => {
                          const val = e.target.value === "" ? undefined : Number(e.target.value);
                          field.onChange(val);
                        }}
                        disabled={readOnly}
                      />
                    </FormControl>
                    <FormDescription>
                      Deixe em branco para não expirar.
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {!readOnly && (
              <div className="flex justify-end">
                <Button type="submit" disabled={isLoading}>
                  {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                  Salvar Configurações
                </Button>
              </div>
            )}
          </form>
        </Form>
      </CardContent>
    </Card>
  );
}
