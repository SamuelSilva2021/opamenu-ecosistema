import { useState } from "react";
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

import { CashMovementType } from "../types";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { OpenShiftDialog } from "../components/OpenShiftDialog";
import { CloseShiftDialog } from "../components/CloseShiftDialog";
import { AddMovementDialog } from "../components/AddMovementDialog";
import { MovementsTable } from "../components/MovementsTable";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { useCashRegister } from "../hooks/useCashRegister";

export default function CashRegisterPage() {
    const [isOpenDialogOpen, setIsOpenDialogOpen] = useState(false);
    const [isCloseDialogOpen, setIsCloseDialogOpen] = useState(false);
    const [isMovementDialogOpen, setIsMovementDialogOpen] = useState(false);

    const {
        activeShift,
        isLoading,
        isShiftOpen,
        openShift,
        closeShift,
        addMovement,
        isOpening,
        isClosing,
        isAddingMovement
    } = useCashRegister();

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

            {isShiftOpen && activeShift ? (
                <>
                    {/* Stats Cards */}
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                        <Card className="border-none shadow-sm bg-blue-50/50 dark:bg-blue-900/10">
                            <CardHeader className="pb-2">
                                <CardDescription className="text-blue-600 dark:text-blue-400 font-medium tracking-tight uppercase text-[10px]">Fundo de Troco</CardDescription>
                                <CardTitle className="text-2xl font-bold">{activeShift.openingBalance.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="flex items-center text-[10px] text-blue-600/80 font-medium">
                                    <Calculator className="h-3 w-3 mr-1" />
                                    Informado na abertura
                                </div>
                            </CardContent>
                        </Card>

                        <Card className="border-none shadow-sm bg-emerald-50/50 dark:bg-emerald-900/10">
                            <CardHeader className="pb-2">
                                <CardDescription className="text-emerald-600 dark:text-emerald-400 font-medium tracking-tight uppercase text-[10px]">Entradas Totais</CardDescription>
                                <CardTitle className="text-2xl font-bold">{totalInbound.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="flex items-center text-[10px] text-emerald-600/80 font-medium uppercase tracking-tighter">
                                    <TrendingUp className="h-3 w-3 mr-1" />
                                    Vendas e Suprimentos
                                </div>
                            </CardContent>
                        </Card>

                        <Card className="border-none shadow-sm bg-red-50/50 dark:bg-red-900/10">
                            <CardHeader className="pb-2">
                                <CardDescription className="text-red-600 dark:text-red-400 font-medium tracking-tight uppercase text-[10px]">Saídas Totais</CardDescription>
                                <CardTitle className="text-2xl font-bold">{totalOutbound.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</CardTitle>
                            </CardHeader>
                            <CardContent>
                                <div className="flex items-center text-[10px] text-red-600/80 font-medium uppercase tracking-tighter">
                                    <TrendingDown className="h-3 w-3 mr-1" />
                                    Sangrias e Estornos
                                </div>
                            </CardContent>
                        </Card>

                        <Card className="border-none shadow-lg bg-gradient-to-br from-zinc-900 to-zinc-800 text-white relative overflow-hidden">
                            <div className="absolute top-0 right-0 w-20 h-20 bg-primary/10 rounded-full -mr-10 -mt-10 blur-2xl" />
                            <CardHeader className="pb-2 relative">
                                <CardDescription className="text-zinc-400 font-medium tracking-tight uppercase text-[10px]">Saldo Atual (Dinheiro)</CardDescription>
                                <CardTitle className="text-3xl font-black">{activeShift.expectedBalance.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</CardTitle>
                            </CardHeader>
                            <CardContent className="relative">
                                <div className="flex items-center text-[10px] text-primary font-bold uppercase tracking-widest">
                                    <DollarSign className="h-3 w-3 mr-1" />
                                    Disponível no Caixa
                                </div>
                            </CardContent>
                        </Card>
                    </div>

                    {/* Movements List */}
                    <Card className="border-none shadow-md overflow-hidden">
                        <CardHeader className="flex flex-row items-center justify-between pb-2 h-20 bg-zinc-50/50 dark:bg-zinc-900/50 border-b">
                            <div className="space-y-1">
                                <CardTitle className="text-xl">Histórico de Movimentações</CardTitle>
                                <CardDescription>Fluxo detalhado do turno atual iniciado em {new Date(activeShift.openedAt).toLocaleString('pt-BR')}</CardDescription>
                            </div>
                            <div className="bg-white dark:bg-zinc-800 p-2 rounded-xl shadow-sm border">
                                <History className="h-5 w-5 text-primary" />
                            </div>
                        </CardHeader>
                        <CardContent className="pt-6">
                            <MovementsTable movements={activeShift.movements} />
                        </CardContent>
                    </Card>
                </>
            ) : (
                <div className="flex items-center justify-center py-10">
                    <Card className="border-none shadow-2xl bg-white dark:bg-zinc-900 flex flex-col items-center justify-center p-12 sm:p-20 text-center max-w-2xl w-full rounded-3xl relative overflow-hidden">
                        {/* Background Decorative Element */}
                        <div className="absolute top-0 right-0 w-40 h-40 bg-primary/5 rounded-full -mr-20 -mt-20 blur-3xl" />
                        <div className="absolute bottom-0 left-0 w-40 h-40 bg-primary/10 rounded-full -ml-20 -mb-20 blur-3xl" />

                        <div className="relative group mb-10">
                            <div className="absolute inset-0 bg-primary/20 rounded-full blur-2xl group-hover:bg-primary/30 transition-all duration-500 scale-150" />
                            <div className="relative bg-gradient-to-br from-primary to-orange-600 p-10 rounded-full shadow-lg shadow-primary/30 transform transition-transform duration-500 group-hover:scale-110">
                                <Lock className="h-16 w-16 text-white" />
                            </div>
                        </div>

                        <CardHeader className="space-y-4 pb-10 w-full max-w-4xl">
                            <CardTitle className="text-5xl font-black tracking-tight text-zinc-900 dark:text-zinc-50 uppercase">
                                Caixa Fechado
                            </CardTitle>
                            <CardDescription className="text-xl text-zinc-500 dark:text-zinc-400 font-medium leading-relaxed w-full mx-auto">
                                Para registrar vendas no PDV e gerenciar entradas ou saídas em dinheiro, você precisa primeiro abrir seu turno de trabalho.
                            </CardDescription>
                        </CardHeader>

                        <CardContent className="w-full max-w-sm pb-0">
                            <Button
                                size="lg"
                                onClick={() => setIsOpenDialogOpen(true)}
                                className="w-full h-18 text-2xl font-black bg-gradient-to-r from-primary to-orange-600 hover:from-orange-600 hover:to-primary text-white rounded-2xl shadow-xl shadow-primary/25 hover:shadow-primary/40 transition-all duration-300 hover:scale-[1.02] active:scale-[0.98] border-none uppercase tracking-wider"
                            >
                                <Unlock className="mr-3 h-7 w-7" />
                                Abrir Turno Agora
                            </Button>

                            <p className="mt-8 text-sm text-zinc-400 dark:text-zinc-500 flex items-center justify-center gap-3">
                                <div className="h-2 w-2 rounded-full bg-zinc-300 dark:bg-zinc-700" />
                                Controle total de entradas e saídas
                                <div className="h-2 w-2 rounded-full bg-zinc-300 dark:bg-zinc-700" />
                            </p>
                        </CardContent>
                    </Card>
                </div>
            )}

            {/* Dialogs */}
            <OpenShiftDialog
                open={isOpenDialogOpen}
                onOpenChange={setIsOpenDialogOpen}
                onSubmit={(data) => {
                    openShift(data, {
                        onSuccess: () => setIsOpenDialogOpen(false)
                    });
                }}
                isLoading={isOpening}
            />

            <CloseShiftDialog
                open={isCloseDialogOpen}
                onOpenChange={setIsCloseDialogOpen}
                expectedBalance={activeShift?.expectedBalance || 0}
                onSubmit={(data) => {
                    closeShift(data, {
                        onSuccess: () => setIsCloseDialogOpen(false)
                    });
                }}
                isLoading={isClosing}
            />

            <AddMovementDialog
                open={isMovementDialogOpen}
                onOpenChange={setIsMovementDialogOpen}
                onSubmit={(data) => {
                    addMovement(data, {
                        onSuccess: () => setIsMovementDialogOpen(false)
                    });
                }}
                isLoading={isAddingMovement}
            />
        </div>
    );
}
