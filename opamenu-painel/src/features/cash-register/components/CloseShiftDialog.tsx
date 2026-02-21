import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Loader2, AlertCircle } from "lucide-react";

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
} from "@/components/ui/form";
import { CurrencyInput } from "@/components/ui/currency-input";
import { Separator } from "@/components/ui/separator";

const formSchema = z.object({
    closingBalance: z.coerce.number().min(0, "O valor não pode ser negativo"),
});

type FormValues = z.infer<typeof formSchema>;

interface CloseShiftDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    onSubmit: (data: FormValues) => void;
    isLoading?: boolean;
    expectedBalance: number;
}

export function CloseShiftDialog({
    open,
    onOpenChange,
    onSubmit,
    isLoading,
    expectedBalance,
}: CloseShiftDialogProps) {
    const form = useForm<FormValues>({
        resolver: zodResolver(formSchema) as any,
        defaultValues: {
            closingBalance: 0,
        },
    });

    const handleSubmit = (values: FormValues) => {
        onSubmit(values);
    };

    const closingBalance = form.watch("closingBalance");
    const difference = closingBalance - expectedBalance;

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Fechar Caixa</DialogTitle>
                    <DialogDescription>
                        Confira os valores e informe o saldo final em dinheiro.
                    </DialogDescription>
                </DialogHeader>

                <div className="space-y-4 py-4">
                    <div className="flex justify-between items-center text-sm">
                        <span className="text-muted-foreground">Saldo Esperado:</span>
                        <span className="font-semibold">{expectedBalance.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</span>
                    </div>

                    <Separator />

                    <Form {...form}>
                        <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-6">
                            <FormField
                                control={form.control}
                                name="closingBalance"
                                render={({ field }) => (
                                    <FormItem>
                                        <FormLabel>Saldo Final (Contado no Caixa)</FormLabel>
                                        <FormControl>
                                            <CurrencyInput
                                                placeholder="R$ 0,00"
                                                value={field.value}
                                                onChange={field.onChange}
                                            />
                                        </FormControl>
                                        <FormMessage />
                                    </FormItem>
                                )}
                            />

                            {Math.abs(difference) > 0.01 && (
                                <div className={`flex items-center gap-2 p-3 rounded-md text-sm ${difference > 0 ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'}`}>
                                    <AlertCircle className="h-4 w-4" />
                                    <span>
                                        Diferença: {difference.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                                        ({difference > 0 ? 'Sobra' : 'Quebra'})
                                    </span>
                                </div>
                            )}

                            <DialogFooter>
                                <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                                    Cancelar
                                </Button>
                                <Button type="submit" variant="destructive" disabled={isLoading}>
                                    {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                                    Encerrar Turno
                                </Button>
                            </DialogFooter>
                        </form>
                    </Form>
                </div>
            </DialogContent>
        </Dialog>
    );
}
