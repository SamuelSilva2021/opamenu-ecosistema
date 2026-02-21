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
import { Input } from "@/components/ui/input";
import { CurrencyInput } from "@/components/ui/currency-input";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
import { CashMovementType } from "../types";

const formSchema = z.object({
    type: z.enum([CashMovementType.Inbound, CashMovementType.Outbound]),
    amount: z.coerce.number().min(0.01, "O valor deve ser maior que zero"),
    description: z.string().min(3, "Descrição muito curta"),
});

type FormValues = z.infer<typeof formSchema>;

interface AddMovementDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    onSubmit: (data: FormValues) => void;
    isLoading?: boolean;
}

export function AddMovementDialog({
    open,
    onOpenChange,
    onSubmit,
    isLoading,
}: AddMovementDialogProps) {
    const form = useForm<FormValues>({
        resolver: zodResolver(formSchema) as any,
        defaultValues: {
            type: CashMovementType.Outbound,
            amount: 0,
            description: "",
        },
    });

    const handleSubmit = (values: FormValues) => {
        onSubmit(values);
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Nova Movimentação</DialogTitle>
                    <DialogDescription>
                        Registre uma entrada (suprimento) ou saída (sangria) manual.
                    </DialogDescription>
                </DialogHeader>

                <Form {...form}>
                    <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
                        <FormField
                            control={form.control}
                            name="type"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Tipo de Movimentação</FormLabel>
                                    <Select
                                        onValueChange={field.onChange}
                                        defaultValue={field.value}
                                    >
                                        <FormControl>
                                            <SelectTrigger>
                                                <SelectValue placeholder="Selecione o tipo" />
                                            </SelectTrigger>
                                        </FormControl>
                                        <SelectContent>
                                            <SelectItem value={CashMovementType.Outbound}>Saída (Sangria)</SelectItem>
                                            <SelectItem value={CashMovementType.Inbound}>Entrada (Suprimento)</SelectItem>
                                        </SelectContent>
                                    </Select>
                                    <FormMessage />
                                </FormItem>
                            )}
                        />

                        <FormField
                            control={form.control}
                            name="amount"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Valor</FormLabel>
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

                        <FormField
                            control={form.control}
                            name="description"
                            render={({ field }) => (
                                <FormItem>
                                    <FormLabel>Descrição / Motivo</FormLabel>
                                    <FormControl>
                                        <Input placeholder="Ex: Pagamento de fornecedor..." {...field} />
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
                                Confirmar
                            </Button>
                        </DialogFooter>
                    </form>
                </Form>
            </DialogContent>
        </Dialog>
    );
}
