import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { cashRegisterService } from "../cash-register.service";
import { CashShiftStatus } from "../types";
import { useToast } from "@/hooks/use-toast";
import { getErrorMessage } from "@/lib/utils";

export function useCashRegister() {
    const queryClient = useQueryClient();
    const { toast } = useToast();

    const { data: activeShift, isLoading } = useQuery({
        queryKey: ["active-shift"],
        queryFn: cashRegisterService.getActiveShift,
    });

    const isShiftOpen = !!activeShift && activeShift.status === CashShiftStatus.Open;

    const openShiftMutation = useMutation({
        mutationFn: cashRegisterService.openShift,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["active-shift"] });
            toast({ variant: "success", title: "Caixa Aberto", description: "Turno iniciado com sucesso." });
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const closeShiftMutation = useMutation({
        mutationFn: cashRegisterService.closeShift,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["active-shift"] });
            toast({ variant: "success", title: "Caixa Fechado", description: "Turno encerrado com sucesso." });
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    const addMovementMutation = useMutation({
        mutationFn: cashRegisterService.addMovement,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["active-shift"] });
            toast({ variant: "success", title: "Sucesso", description: "Movimentação registrada." });
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    return {
        activeShift,
        isLoading,
        isShiftOpen,
        openShift: openShiftMutation.mutate,
        closeShift: closeShiftMutation.mutate,
        addMovement: addMovementMutation.mutate,
        isOpening: openShiftMutation.isPending,
        isClosing: closeShiftMutation.isPending,
        isAddingMovement: addMovementMutation.isPending,
    };
}
