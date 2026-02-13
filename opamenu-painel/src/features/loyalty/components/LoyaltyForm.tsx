import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Loader2, X } from "lucide-react";
import { useQuery } from "@tanstack/react-query";

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
import { CurrencyInput } from "@/components/ui/currency-input";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Label } from "@/components/ui/label";
import { useToast } from "@/hooks/use-toast";
import { loyaltyService } from "../loyalty.service";
import { categoriesService } from "../../categories/categories.service";
import { productsService } from "../../products/products.service";
import { ELoyaltyProgramType, ELoyaltyRewardType, type LoyaltyProgram } from "../types";

const formSchema = z.object({
  name: z.string().min(3, "Nome é obrigatório"),
  description: z.string().optional(),
  type: z.nativeEnum(ELoyaltyProgramType),
  pointsPerCurrency: z.coerce.number().min(0.1, "Deve ser maior que 0"),
  currencyValue: z.coerce.number().min(0, "Não pode ser negativo"),
  minOrderValue: z.coerce.number().min(0, "Não pode ser negativo"),
  pointsValidityDays: z.union([
    z.coerce.number().min(1, "Mínimo 1 dia"),
    z.literal(""),
    z.undefined(),
    z.null()
  ]).transform(val => (val === "" || val === null || val === undefined) ? undefined : Number(val)),
  isActive: z.boolean().default(true),
  targetCount: z.coerce.number().optional().nullable(),
  rewardType: z.nativeEnum(ELoyaltyRewardType).optional().nullable(),
  rewardValue: z.coerce.number().optional().nullable(),
  filters: z.array(z.object({
    productId: z.string().optional().nullable(),
    categoryId: z.string().optional().nullable()
  })).default([]),
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
      type: ELoyaltyProgramType.PointsPerValue,
      pointsPerCurrency: 1,
      currencyValue: 0,
      minOrderValue: 0,
      pointsValidityDays: undefined,
      isActive: true,
      targetCount: null,
      rewardType: ELoyaltyRewardType.PercentageDiscount,
      rewardValue: 0,
      filters: [],
    },
  });

  const type = form.watch("type");
  const currencyValue = form.watch("currencyValue");
  const pointsPerCurrency = form.watch("pointsPerCurrency");
  const selectedFilters = form.watch("filters");

  // Fetch Categories and Products for filters
  const { data: categories } = useQuery<any[]>({
    queryKey: ["categories"],
    queryFn: categoriesService.getCategories,
  });

  const { data: productsResult } = useQuery<any>({
    queryKey: ["products-list"],
    queryFn: () => productsService.getProducts({}),
  });

  const products = Array.isArray(productsResult) ? productsResult : (productsResult as any)?.data?.items || [];

  useEffect(() => {
    if (type === ELoyaltyProgramType.PointsPerValue) {
      const formattedCurrency = new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL",
      }).format(currencyValue || 0);

      const desc = `A cada ${formattedCurrency} em compra você ganha ${pointsPerCurrency || 0} ponto para trocar por recompensas.`;
      form.setValue("description", desc);
    } else if (type === ELoyaltyProgramType.OrderCount) {
      form.setValue("description", "Cada pedido realizado conta 1 ponto para o programa.");
    } else if (type === ELoyaltyProgramType.ItemCount) {
      form.setValue("description", "Cada item (dos produtos selecionados) conta 1 ponto para o programa.");
    }
  }, [type, currencyValue, pointsPerCurrency, form]);

  useEffect(() => {
    if (initialData) {
      form.reset({
        name: initialData.name,
        description: initialData.description || "",
        type: initialData.type,
        pointsPerCurrency: initialData.pointsPerCurrency,
        currencyValue: initialData.currencyValue,
        minOrderValue: initialData.minOrderValue,
        pointsValidityDays: initialData.pointsValidityDays,
        isActive: initialData.isActive,
        targetCount: initialData.targetCount,
        rewardType: initialData.rewardType,
        rewardValue: initialData.rewardValue,
        filters: initialData.filters || [],
      });
    }
  }, [initialData, form]);

  const removeFilter = (index: number) => {
    const current = [...selectedFilters];
    current.splice(index, 1);
    form.setValue("filters", current);
  };

  const addFilter = (productId?: string, categoryId?: string) => {
    if (!productId && !categoryId) return;

    // Check for duplicates
    const exists = selectedFilters.some(f =>
      (productId && f.productId === productId) || (categoryId && f.categoryId === categoryId)
    );

    if (!exists) {
      form.setValue("filters", [...selectedFilters, { productId, categoryId }]);
    }
  };

  return (
    <div className="space-y-6 py-4">
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit as any)} className="space-y-6">
          <FormField
            control={form.control as any}
            name="isActive"
            render={({ field }) => (
              <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4 bg-muted/5">
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
                      if (initialData?.id) {
                        try {
                          await loyaltyService.toggleStatus(initialData.id, checked);
                          toast({ title: "Sucesso", description: `Programa ${checked ? "ativado" : "desativado"} com sucesso.`, variant: "success" });
                        } catch (error) {
                          field.onChange(!checked);
                          toast({ title: "Erro", description: "Erro ao alterar status.", variant: "destructive" });
                        }
                      }
                    }}
                  />
                </FormControl>
              </FormItem>
            )}
          />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
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
              name="type"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Tipo de Acúmulo</FormLabel>
                  <Select
                    onValueChange={(val) => field.onChange(parseInt(val))}
                    value={field.value.toString()}
                    disabled={readOnly}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Selecione o tipo" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      <SelectItem value={ELoyaltyProgramType.PointsPerValue.toString()}>Pontos por Valor Gasto</SelectItem>
                      <SelectItem value={ELoyaltyProgramType.OrderCount.toString()}>Quantidade de Pedidos</SelectItem>
                      <SelectItem value={ELoyaltyProgramType.ItemCount.toString()}>Quantidade de Itens/Produtos</SelectItem>
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          {type === ELoyaltyProgramType.PointsPerValue && (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 border p-6 rounded-lg bg-muted/20">
              <FormField
                control={form.control as any}
                name="currencyValue"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Moeda Base (R$)</FormLabel>
                    <FormControl>
                      <CurrencyInput value={field.value} onChange={field.onChange} disabled={readOnly} />
                    </FormControl>
                    <FormDescription>Valor gasto para ganhar pontos.</FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control as any}
                name="pointsPerCurrency"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Pontos Ganhos</FormLabel>
                    <FormControl>
                      <Input type="number" step="0.1" {...field} disabled={readOnly} />
                    </FormControl>
                    <FormDescription>Pontos ganhos por cada moeda base.</FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
          )}

          {type === ELoyaltyProgramType.ItemCount && (
            <div className="space-y-4 border p-6 rounded-lg bg-muted/20">
              <div className="flex items-end gap-2">
                <div className="flex-1 space-y-2">
                  <Label>Adicionar Filtro (Produto ou Categoria)</Label>
                  <div className="grid grid-cols-2 gap-2">
                    <Select onValueChange={(val) => addFilter(val, undefined)} disabled={readOnly}>
                      <SelectTrigger><SelectValue placeholder="Produto" /></SelectTrigger>
                      <SelectContent>
                        {products?.map((p: any) => (
                          <SelectItem key={p.id} value={p.id}>{p.name}</SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <Select onValueChange={(val) => addFilter(undefined, val)} disabled={readOnly}>
                      <SelectTrigger><SelectValue placeholder="Categoria" /></SelectTrigger>
                      <SelectContent>
                        {categories?.map((c: any) => (
                          <SelectItem key={c.id} value={c.id}>{c.name}</SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              </div>
              <div className="flex flex-wrap gap-2">
                {selectedFilters.map((filter, index) => {
                  const prod = products?.find((p: any) => p.id === filter.productId);
                  const cat = categories?.find((c: any) => c.id === filter.categoryId);
                  return (
                    <Badge key={index} variant="secondary" className="pl-3 pr-1 py-1 gap-1">
                      {prod ? `Prod: ${prod.name}` : cat ? `Cat: ${cat.name}` : "Desconhecido"}
                      {!readOnly && (
                        <button type="button" onClick={() => removeFilter(index)} className="rounded-full hover:bg-muted p-0.5">
                          <X className="h-3 w-3" />
                        </button>
                      )}
                    </Badge>
                  );
                })}
                {selectedFilters.length === 0 && <span className="text-sm text-muted-foreground">Nenhum filtro aplicado. O programa valerá para todos os itens.</span>}
              </div>
            </div>
          )}

          <Separator />

          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            <FormField
              control={form.control as any}
              name="minOrderValue"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Valor Mínimo do Pedido (R$)</FormLabel>
                  <FormControl>
                    <CurrencyInput value={field.value} onChange={field.onChange} disabled={readOnly} />
                  </FormControl>
                  <FormDescription>Valor mínimo para pontuar no pedido.</FormDescription>
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
                      onChange={(e) => field.onChange(e.target.value === "" ? undefined : Number(e.target.value))}
                      disabled={readOnly}
                    />
                  </FormControl>
                  <FormDescription>Deixe em branco para não expirar.</FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />
          </div>

          <Separator />

          <div className="space-y-4">
            <h3 className="text-lg font-semibold tracking-tight">Meta e Recompensa</h3>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6 border p-6 rounded-lg bg-primary/5">
              <FormField
                control={form.control as any}
                name="targetCount"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Meta (Pontos)</FormLabel>
                    <FormControl>
                      <Input type="number" {...field} value={field.value ?? ""} disabled={readOnly} />
                    </FormControl>
                    <FormDescription>Pontos para liberar prêmio.</FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control as any}
                name="rewardType"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Recompensa</FormLabel>
                    <Select
                      onValueChange={(val) => field.onChange(parseInt(val))}
                      value={field.value?.toString() ?? ELoyaltyRewardType.PercentageDiscount.toString()}
                      disabled={readOnly}
                    >
                      <FormControl>
                        <SelectTrigger><SelectValue /></SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value={ELoyaltyRewardType.PercentageDiscount.toString()}>Desconto (%)</SelectItem>
                        <SelectItem value={ELoyaltyRewardType.FixedValueDiscount.toString()}>Desconto Fixo (R$)</SelectItem>
                        <SelectItem value={ELoyaltyRewardType.FreeProduct.toString()}>Produto Grátis</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control as any}
                name="rewardValue"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Valor/ID da Recompensa</FormLabel>
                    <FormControl>
                      <Input type="number" step="0.01" {...field} value={field.value ?? ""} disabled={readOnly} />
                    </FormControl>
                    <FormDescription>Valor do desc. ou produto.</FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>
          </div>

          <FormField
            control={form.control as any}
            name="description"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Descrição Automática</FormLabel>
                <FormControl>
                  <Textarea className="resize-none" {...field} disabled={true} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          {!readOnly && (
            <div className="flex justify-end pt-4">
              <Button type="submit" size="lg" className="px-8" disabled={isLoading}>
                {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                Salvar Programa de Fidelidade
              </Button>
            </div>
          )}
        </form>
      </Form>
    </div>
  );
}
