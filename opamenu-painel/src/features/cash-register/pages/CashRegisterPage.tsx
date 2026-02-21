import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
    Lock,
    Unlock,
    History,
    Info,
    DollarSign,
    TrendingUp,
    TrendingDown,
    Calculator,
    Plus
} from "lucide-react";

import { cashRegisterService } from "../cash-register.service";
import { CashShiftStatus, CashMovementType } from "../types";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { useToast } from "@/hooks/use-toast";
import { getErrorMessage } from "@/lib/utils";
import { OpenShiftDialog } from "../components/OpenShiftDialog";
import { CloseShiftDialog } from "../components/CloseShiftDialog";
import { AddMovementDialog } from "../components/AddMovementDialog";
import { MovementsTable } from "../components/MovementsTable";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";

export default function CashRegisterPage() {
    const [isOpenDialogOpen, setIsOpenDialogOpen] = useState(false);
    const [isCloseDialogOpen, setIsCloseDialogOpen] = useState(false);
    const [isMovementDialogOpen, setIsMovementDialogOpen] = useState(false);

    const queryClient = useQueryClient();
    const { toast } = useToast();

    const { data: activeShift, isLoading } = useQuery({
        queryKey: ["active-shift"],
        queryFn: cashRegisterService.getActiveShift,
    });

    const openShiftMutation = useMutation({
        mutationFn: cashRegisterService.openShift,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ["active-shift"] });
            toast({ variant: "success", title: "Caixa Aberto", description: "Turno iniciado com sucesso." });
            setIsOpenDialogOpen(false);
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
            setIsCloseDialogOpen(false);
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
            setIsMovementDialogOpen(false);
        },
        onError: (error) => {
            toast({ title: "Erro", description: getErrorMessage(error), variant: "destructive" });
        },
    });

    if (isLoading) {
        return (
            <div className="space-y-6">
                <div className="flex justify-between items-center">
                    <Skeleton className="h-10 w-48" />
                    <Skeleton className="h-10 w-32" />
                </div>
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    {[1, 2, 3, 4].map(i => <Skeleton key={i} className="h-32 w-full" />)}
                </div>
                <Skeleton className="h-[400px] w-full" />
            </div>
        );
    }

    const isShiftOpen = activeShift && activeShift.status === CashShiftStatus.Open;

    const totalInbound = activeShift?.movements
        .filter(m => m.type === CashMovementType.Inbound || m.type === CashMovementType.OrderPayment || m.type === CashMovementType.Opening)
        .reduce((acc, m) => acc + m.amount, 0) || 0;

    const totalOutbound = activeShift?.movements
        .filter(m => m.type === CashMovementType.Outbound || m.type === CashMovementType.Reversed)
        .reduce((acc, m) => acc + m.amount, 0) || 0;

    return (
        <div className="space-y-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
            {/* Header */}
            <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50 flex items-center gap-2">
                        Fluxo de Caixa
                        {isShiftOpen ? (
                            <Badge variant="outline" className="bg-emerald-50 text-emerald-700 border-emerald-200 ml-2 animate-pulse">
                                Aberto
                            </Badge>
                        ) : (
                            <Badge variant="outline" className="bg-zinc-100 text-zinc-600 border-zinc-200 ml-2">
                                Fechado
                            </Badge>
                        )}
                    </h2>
                    <p className="text-muted-foreground mt-1 flex items-center gap-1">
                        <Info className="h-3.5 w-3.5" />
                        Controle de entradas e saídas do terminal atual.
                    </p>
                </div>

                <div className="flex gap-2 w-full sm:w-auto">
                    {isShiftOpen ? (
                        <>
                            <Button variant="outline" onClick={() => setIsMovementDialogOpen(true)}>
                                <Plus className="mr-2 h-4 w-4" />
                                Movimentar
                            </Button>
                            <Button variant="destructive" onClick={() => setIsCloseDialogOpen(true)}>
                                <Lock className="mr-2 h-4 w-4" />
                                Fechar Caixa
                            </Button>
                        </>
                    ) : (
                        <Button onClick={() => setIsOpenDialogOpen(true)} className="bg-emerald-600 hover:bg-emerald-700">
                            <Unlock className="mr-2 h-4 w-4" />
                            Abrir Caixa
                        </Button>
                    )}
                </div>
            </div>

            {isShiftOpen ? (
                <>
                    {/* Stats Cards */}
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                        <Card className="border-none shadow-sm bg-blue-50/50 dark:bg-blue-900/10">
                            <CardHeader className="pb-2">
                                <CardDescription className="text-blue-600 dark:text-blue-400 font-medium">Fundo de Troco</CardDescription>
                                <CardTitle className="text-2xl font-bold">{activeShift.openingBalance.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="flex items-center text-xs text-blue-600/80">
                                    <Calculator className="h-3 w-3 mr-1" />
                                    Informado na abertura
                                </div>
                            </CardContent>
                        </Card>

                        <Card className="border-none shadow-sm bg-emerald-50/50 dark:bg-emerald-900/10">
                            <CardHeader className="pb-2">
                                <CardDescription className="text-emerald-600 dark:text-emerald-400 font-medium">Entradas Totais</CardDescription>
                                <CardTitle className="text-2xl font-bold">{totalInbound.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="flex items-center text-xs text-emerald-600/80 uppercase tracking-tighter font-bold">
                                    <TrendingUp className="h-3 w-3 mr-1" />
                                    Vendas e Suprimentos
                                </div>
                            </CardContent>
                        </Card>

                        <Card className="border-none shadow-sm bg-red-50/50 dark:bg-red-900/10">
                            <CardHeader className="pb-2">
                                <CardDescription className="text-red-600 dark:text-red-400 font-medium">Saídas Totais</CardDescription>
                                <CardTitle className="text-2xl font-bold">{totalOutbound.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="flex items-center text-xs text-red-600/80 uppercase tracking-tighter font-bold">
                                    <TrendingDown className="h-3 w-3 mr-1" />
                                    Sangrias e Estornos
                                </div>
                            </CardContent>
                        </Card>

                        <Card className="border-none shadow-md bg-zinc-900 text-white">
                            <CardHeader className="pb-2">
                                <CardDescription className="text-zinc-400 font-medium">Saldo Atual (Esperado)</CardDescription>
                                <CardTitle className="text-3xl font-black">{activeShift.expectedBalance.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="flex items-center text-xs text-emerald-400 font-bold uppercase tracking-widest">
                                    <DollarSign className="h-3 w-3 mr-1" />
                                    Em Dinheiro no Caixa
                                </div>
                            </CardContent>
                        </Card>
                    </div>

                    {/* Movements List */}
                    <Card className="border-none shadow-md">
                        <CardHeader className="flex flex-row items-center justify-between pb-2 h-20">
                            <div className="space-y-1">
                                <CardTitle>Histórico de Movimentações</CardTitle>
                                <CardDescription>Fluxo detalhado do turno atual iniciado em {new Date(activeShift.openedAt).toLocaleString('pt-BR')}</CardDescription>
                            </div>
                            <History className="h-5 w-5 text-muted-foreground" />
                        </CardHeader>
                        <CardContent>
                            <MovementsTable movements={activeShift.movements} />
                        </CardContent>
                    </Card>
                </>
            ) : (
                <Card className="border-none shadow-lg bg-zinc-50 dark:bg-zinc-900 border-2 border-dashed flex flex-col items-center justify-center py-20 text-center">
                    <div className="bg-zinc-200 dark:bg-zinc-800 p-6 rounded-full mb-6">
                        <Lock className="h-12 w-12 text-zinc-400" />
                    </div>
                    <CardHeader>
                        <CardTitle className="text-2xl">Caixa Fechado</CardTitle>
                        <CardDescription className="max-w-md">
                            Para registrar vendas no PDV e gerenciar entradas ou saídas em dinheiro, você precisa primeiro abrir seu turno de trabalho.
                        </CardDescription>
                    </CardHeader>
                    <CardContent>
                        <Button size="lg" onClick={() => setIsOpenDialogOpen(true)} className="bg-emerald-600 hover:bg-emerald-700 h-12 px-8 text-lg font-bold shadow-lg shadow-emerald-200/50 dark:shadow-none transition-all hover:scale-105">
                            <Unlock className="mr-2 h-5 w-5" />
                            Abrir Turno Agora
                        </Button>
                    </CardContent>
                </Card>
            )}

            {/* Dialogs */}
            <OpenShiftDialog
                open={isOpenDialogOpen}
                onOpenChange={setIsOpenDialogOpen}
                onSubmit={(data) => openShiftMutation.mutate(data)}
                isLoading={openShiftMutation.isPending}
            />

            <CloseShiftDialog
                open={isCloseDialogOpen}
                onOpenChange={setIsCloseDialogOpen}
                expectedBalance={activeShift?.expectedBalance || 0}
                onSubmit={(data) => closeShiftMutation.mutate(data)}
                isLoading={closeShiftMutation.isPending}
            />

            <AddMovementDialog
                open={isMovementDialogOpen}
                onOpenChange={setIsMovementDialogOpen}
                onSubmit={(data) => addMovementMutation.mutate(data)}
                isLoading={addMovementMutation.isPending}
            />
        </div>
    );
}
