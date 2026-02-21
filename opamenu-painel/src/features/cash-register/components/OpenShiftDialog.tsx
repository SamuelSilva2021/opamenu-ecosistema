import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Loader2 } from "lucide-react";

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

const formSchema = z.object({
    openingBalance: z.coerce.number().min(0, "O valor não pode ser negativo"),
});

type FormValues = z.infer<typeof formSchema>;

interface OpenShiftDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    onSubmit: (data: { openingBalance: number }) => void;
    isLoading?: boolean;
}

export function OpenShiftDialog({
    open,
    onOpenChange,
    onSubmit,
    isLoading,
}: OpenShiftDialogProps) {
    const form = useForm<FormValues>({
        resolver: zodResolver(formSchema) as any,
        defaultValues: {
            openingBalance: 0,
        },
    });

    const handleSubmit = (values: FormValues) => {
        onSubmit(values);
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Abrir Caixa</DialogTitle>
                    <DialogDescription>
                        Informe o saldo inicial para começar o turno de trabalho.
                    </DialogDescription>
                </DialogHeader>

                <Form {...form}>
                    <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
                        <FormField
                            control={form.control}
                            name="openingBalance"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Saldo Inicial (Troco)</FormLabel>
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

                        <DialogFooter>
                            <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                                Cancelar
                            </Button>
                            <Button type="submit" disabled={isLoading}>
                                {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                                Confirmar Abertura
                            </Button>
                        </DialogFooter>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    );
}
