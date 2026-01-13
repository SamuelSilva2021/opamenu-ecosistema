import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { format } from "date-fns";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
  DialogDescription,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
  FormDescription,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { DiscountType } from "../types";
import type { Coupon, CreateCouponRequest, UpdateCouponRequest } from "../types";

const formSchema = z.object({
  code: z.string().min(3, "Código deve ter pelo menos 3 caracteres"),
  description: z.string().optional(),
  discountType: z.coerce.number().refine((val) => val === 1 || val === 2, {
    message: "Selecione um tipo de desconto",
  }),
  discountValue: z.coerce.number().min(0.01, "Valor deve ser maior que zero"),
  minOrderValue: z.preprocess(
    (val) => (val === "" ? undefined : val),
    z.coerce.number().min(0).optional()
  ),
  maxDiscountValue: z.preprocess(
    (val) => (val === "" ? undefined : val),
    z.coerce.number().min(0).optional()
  ),
  usageLimit: z.preprocess(
    (val) => (val === "" ? undefined : val),
    z.coerce.number().min(0).optional()
  ),
  startDate: z.string().optional(),
  expirationDate: z.string().optional(),
  isActive: z.boolean().default(true),
  firstOrderOnly: z.boolean().default(false),
});

interface CouponFormProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onSubmit: (data: CreateCouponRequest | UpdateCouponRequest) => void;
  initialData?: Coupon | null;
  isLoading?: boolean;
}

export function CouponForm({
  open,
  onOpenChange,
  onSubmit,
  initialData,
  isLoading,
}: CouponFormProps) {
  const form = useForm<any>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      code: "",
      description: "",
      discountType: DiscountType.Percentage,
      discountValue: 0,
      minOrderValue: undefined,
      maxDiscountValue: undefined,
      usageLimit: undefined,
      startDate: "",
      expirationDate: "",
      isActive: true,
      firstOrderOnly: false,
    },
  });

  const discountType = form.watch("discountType");

  useEffect(() => {
    if (initialData) {
      form.reset({
        code: initialData.code,
        description: initialData.description || "",
        discountType: initialData.discountType,
        discountValue: initialData.discountValue,
        minOrderValue: initialData.minOrderValue || undefined,
        maxDiscountValue: initialData.maxDiscountValue || undefined,
        usageLimit: initialData.usageLimit || undefined,
        startDate: initialData.startDate ? format(new Date(initialData.startDate), "yyyy-MM-dd'T'HH:mm") : "",
        expirationDate: initialData.expirationDate ? format(new Date(initialData.expirationDate), "yyyy-MM-dd'T'HH:mm") : "",
        isActive: initialData.isActive,
        firstOrderOnly: initialData.firstOrderOnly,
      });
    } else {
      form.reset({
        code: "",
        description: "",
        discountType: DiscountType.Percentage,
        discountValue: 0,
        minOrderValue: undefined,
        maxDiscountValue: undefined,
        usageLimit: undefined,
        startDate: "",
        expirationDate: "",
        isActive: true,
        firstOrderOnly: false,
      });
    }
  }, [initialData, form, open]);

  const handleSubmit = (values: z.infer<typeof formSchema>) => {
    const payload: any = {
      ...values,
      startDate: values.startDate ? new Date(values.startDate).toISOString() : undefined,
      expirationDate: values.expirationDate ? new Date(values.expirationDate).toISOString() : undefined,
    };
    onSubmit(payload);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[600px] max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>
            {initialData ? "Editar Cupom" : "Novo Cupom"}
          </DialogTitle>
          <DialogDescription>
            {initialData 
              ? "Faça as alterações necessárias nos dados do cupom." 
              : "Preencha os dados abaixo para criar um novo cupom de desconto."}
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="code"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Código</FormLabel>
                    <FormControl>
                      <Input 
                        placeholder="Ex: PROMO10" 
                        {...field} 
                        className="uppercase" 
                        onChange={e => field.onChange(e.target.value.toUpperCase())} 
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="discountType"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Tipo de Desconto</FormLabel>
                    <Select
                      onValueChange={(val) => field.onChange(Number(val))}
                      value={field.value?.toString()}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Selecione o tipo" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="1">Porcentagem (%)</SelectItem>
                        <SelectItem value="2">Valor Fixo (R$)</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Descrição (Opcional)</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Descrição interna ou para o cliente"
                      className="resize-none"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="discountValue"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Valor {discountType === DiscountType.Percentage ? "% de desconto" : "R$ de desconto"}</FormLabel>
                    <FormControl>
                      <Input type="number" step="0.01" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="minOrderValue"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Valor Mínimo do Pedido</FormLabel>
                    <FormControl>
                      <Input 
                        type="number" 
                        step="0.01" 
                        placeholder="Valor mínimo para aplicar" 
                        {...field} 
                        value={field.value ?? ""} 
                        onChange={e => field.onChange(e.target.value)} 
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="maxDiscountValue"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Desconto Máximo</FormLabel>
                    <FormControl>
                      <Input 
                        type="number" 
                        step="0.01" 
                        placeholder="Limite do desconto" 
                        {...field} 
                        value={field.value ?? ""} 
                        onChange={e => field.onChange(e.target.value)} 
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="usageLimit"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Limite de Uso</FormLabel>
                    <FormControl>
                      <Input 
                        type="number" 
                        step="1" 
                        placeholder="Total de usos permitidos" 
                        {...field} 
                        value={field.value ?? ""} 
                        onChange={e => field.onChange(e.target.value)} 
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="startDate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data de Início</FormLabel>
                    <FormControl>
                      <Input 
                        type="datetime-local" 
                        {...field} 
                        value={field.value ?? ""} 
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="expirationDate"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Data de Expiração</FormLabel>
                    <FormControl>
                      <Input 
                        type="datetime-local" 
                        {...field} 
                        value={field.value ?? ""} 
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="flex flex-col gap-4 pt-2">
              <FormField
                control={form.control}
                name="firstOrderOnly"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-3 shadow-sm">
                    <div className="space-y-0.5">
                      <FormLabel className="text-base">Primeira Compra</FormLabel>
                      <FormDescription>
                        Válido apenas para a primeira compra do cliente
                      </FormDescription>
                    </div>
                    <FormControl>
                      <Switch
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="isActive"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-3 shadow-sm">
                    <div className="space-y-0.5">
                      <FormLabel className="text-base">Ativo</FormLabel>
                      <FormDescription>
                        Se desativado, o cupom não poderá ser utilizado
                      </FormDescription>
                    </div>
                    <FormControl>
                      <Switch
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                  </FormItem>
                )}
              />
            </div>

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                Cancelar
              </Button>
              <Button type="submit" disabled={isLoading}>
                {isLoading ? "Salvando..." : "Salvar"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
