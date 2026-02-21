import { useState } from "react";
import { PermissionGate } from "@/components/auth/PermissionGate";
import { ProductGrid } from "../components/ProductGrid";
import { CartSidebar } from "../components/CartSidebar";
import { CashRegisterGatekeeper } from "@/features/cash-register/components/CashRegisterGatekeeper";
import { Button } from "@/components/ui/button";
import { Lock, Plus, DollarSign } from "lucide-react";
import { CloseShiftDialog } from "@/features/cash-register/components/CloseShiftDialog";
import { AddMovementDialog } from "@/features/cash-register/components/AddMovementDialog";
import { useCashRegister } from "@/features/cash-register/hooks/useCashRegister";

export function POSPage() {
  const [isCloseDialogOpen, setIsCloseDialogOpen] = useState(false);
  const [isMovementDialogOpen, setIsMovementDialogOpen] = useState(false);
  const {
    activeShift,
    closeShift,
    addMovement,
    isClosing,
    isAddingMovement
  } = useCashRegister();

  return (
    <PermissionGate module="PDV" operation="READ" fallback={
      <div className="flex h-[400px] items-center justify-center">
        <p className="text-muted-foreground">Você não tem permissão para acessar o PDV.</p>
      </div>
    }>
      <CashRegisterGatekeeper>
        <div className="flex flex-col h-[calc(100vh-8rem)] gap-4">
          {/* POS Header with Cash Actions */}
          <div className="flex items-center justify-between bg-white/80 dark:bg-zinc-900/80 backdrop-blur-md p-4 rounded-2xl border shadow-lg">
            <div className="flex items-center gap-4">
              <div className="relative group">
                <div className="absolute inset-0 bg-primary/20 rounded-full blur-md group-hover:bg-primary/30 transition-all scale-150" />
                <div className="relative bg-gradient-to-br from-primary to-orange-600 p-2.5 rounded-full shadow-md shadow-primary/20">
                  <DollarSign className="h-5 w-5 text-white" />
                </div>
              </div>
              <div>
                <h3 className="font-black text-xs uppercase tracking-widest text-zinc-400 dark:text-zinc-500">Caixa em Operação</h3>
                <p className="text-sm font-bold text-zinc-900 dark:text-zinc-100">
                  Saldo: <span className="text-primary font-black">{(activeShift?.expectedBalance ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}</span>
                </p>
              </div>
            </div>
            <div className="flex gap-3">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setIsMovementDialogOpen(true)}
                className="rounded-xl border-zinc-200 dark:border-zinc-800 hover:bg-zinc-50 dark:hover:bg-zinc-800 font-bold text-xs uppercase tracking-tight h-10 px-4 transition-all"
              >
                <Plus className="mr-2 h-4 w-4 text-primary" />
                Sangria / Suprimento
              </Button>
              <Button
                variant="destructive"
                size="sm"
                onClick={() => setIsCloseDialogOpen(true)}
                className="rounded-xl bg-red-50 text-red-600 hover:bg-red-100 dark:bg-red-900/20 dark:text-red-400 dark:hover:bg-red-900/30 border-none font-bold text-xs uppercase tracking-tight h-10 px-4 transition-all"
              >
                <Lock className="mr-2 h-4 w-4" />
                Fechar Caixa
              </Button>
            </div>
          </div>

          <div className="flex-1 flex overflow-hidden rounded-lg border bg-background shadow-sm">
            <div className="flex-1 overflow-hidden bg-gray-50/50 dark:bg-zinc-900/50">
              <ProductGrid />
            </div>
            <div className="w-[400px] border-l h-full bg-background">
              <CartSidebar />
            </div>
          </div>
        </div>

        {/* Dialogs */}
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
      </CashRegisterGatekeeper>
    </PermissionGate>
  );
}
