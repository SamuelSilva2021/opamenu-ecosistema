import { useState } from "react";
import { Lock, Unlock, Loader2 } from "lucide-react";
import { useCashRegister } from "../hooks/useCashRegister";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { OpenShiftDialog } from "./OpenShiftDialog";

interface CashRegisterGatekeeperProps {
    children: React.ReactNode;
}

export function CashRegisterGatekeeper({ children }: CashRegisterGatekeeperProps) {
    const [isOpenDialogOpen, setIsOpenDialogOpen] = useState(false);
    const { isLoading, isShiftOpen, openShift, isOpening } = useCashRegister();

    if (isLoading) {
        return (
            <div className="flex h-[400px] items-center justify-center">
                <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
        );
    }

    if (!isShiftOpen) {
        return (
            <div className="flex items-center justify-center h-[calc(100vh-8rem)] bg-zinc-50/50 dark:bg-zinc-950/30 rounded-xl border-2 border-dashed border-zinc-200 dark:border-zinc-800 m-4 overflow-hidden">
                <Card className="border-none shadow-none bg-white dark:bg-zinc-900 flex flex-col items-center justify-center p-8 sm:p-12 text-center w-full h-full rounded-none relative overflow-hidden">
                    {/* Background Decorative Element */}
                    <div className="absolute top-0 right-0 w-32 h-32 bg-primary/5 rounded-full -mr-16 -mt-16 blur-3xl" />
                    <div className="absolute bottom-0 left-0 w-32 h-32 bg-primary/10 rounded-full -ml-16 -mb-16 blur-3xl" />

                    <div className="relative group mb-8">
                        <div className="absolute inset-0 bg-primary/20 rounded-full blur-2xl group-hover:bg-primary/30 transition-all duration-500 scale-150" />
                        <div className="relative bg-gradient-to-br from-primary to-orange-600 p-8 rounded-full shadow-lg shadow-primary/30 transform transition-transform duration-500 group-hover:scale-110">
                            <Lock className="h-14 w-14 text-white animate-pulse" />
                        </div>
                    </div>

                    <CardHeader className="space-y-4 pb-8 w-full max-w-4xl">
                        <CardTitle className="text-4xl font-black tracking-tight text-zinc-900 dark:text-zinc-50 uppercase">
                            Caixa Fechado
                        </CardTitle>
                        <CardDescription className="text-lg text-zinc-500 dark:text-zinc-400 font-medium leading-relaxed w-full">
                            O acesso ao PDV está bloqueado. Para começar a vender e gerenciar o fluxo de dinheiro, abra um novo turno.
                        </CardDescription>
                    </CardHeader>

                    <CardContent className="w-full pb-0">
                        <Button
                            size="lg"
                            onClick={() => setIsOpenDialogOpen(true)}
                            className="w-full h-16 text-xl font-black bg-gradient-to-r from-primary to-orange-600 hover:from-orange-600 hover:to-primary text-white rounded-2xl shadow-xl shadow-primary/25 hover:shadow-primary/40 transition-all duration-300 hover:scale-[1.02] active:scale-[0.98] border-none uppercase tracking-wider"
                        >
                            <Unlock className="mr-3 h-6 w-6" />
                            Abrir Turno Agora
                        </Button>

                        <p className="mt-6 text-sm text-zinc-400 dark:text-zinc-500 flex items-center justify-center gap-2">
                            <div className="h-1.5 w-1.5 rounded-full bg-zinc-300 dark:bg-zinc-700" />
                            Controle total de entradas e saídas
                            <div className="h-1.5 w-1.5 rounded-full bg-zinc-300 dark:bg-zinc-700" />
                        </p>
                    </CardContent>

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
                </Card>
            </div>
        );
    }

    return <>{children}</>;
}
