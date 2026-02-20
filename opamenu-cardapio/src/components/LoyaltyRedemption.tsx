import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Slider } from "@/components/ui/slider";
import { Button } from "@/components/ui/button";
import { Gift, AlertCircle, ChevronDown, CheckCircle2 } from "lucide-react";
import { LoyaltyProgramDto, CustomerResponseDto } from "@/types/api";
import { useCart, useLoyalty } from "@/hooks";
import { useParams } from "react-router-dom";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";

interface LoyaltyRedemptionProps {
  program?: LoyaltyProgramDto; // Legado, mantido para compatibilidade se necessário
  customer: CustomerResponseDto;
}

export const LoyaltyRedemption = ({ program: initialProgram, customer }: LoyaltyRedemptionProps) => {
  const { slug } = useParams<{ slug: string }>();
  const { subtotal, applyLoyaltyPoints, removeLoyaltyPoints, loyaltyPointsUsed, loyaltyProgramId } = useCart();
  const { balance, programs, loading } = useLoyalty(slug, customer.phone);

  // Encontrar o programa selecionado atualmente ou o primeiro disponível com saldo
  const [selectedProgram, setSelectedProgram] = useState<LoyaltyProgramDto | null>(null);
  const [pointsToUse, setPointsToUse] = useState<number>(loyaltyPointsUsed);

  // Filtrar programas que o usuário realmente tem saldo
  const activeBalances = balance?.balances?.filter(b => b.balance > 0) || [];
  const availablePrograms = programs.filter(p => activeBalances.some(b => b.programId === p.id));

  useEffect(() => {
    if (availablePrograms.length > 0 && !selectedProgram) {
      // Se já houver um programa aplicado no carrinho, seleciona ele
      if (loyaltyProgramId) {
        const current = availablePrograms.find(p => p.id === loyaltyProgramId);
        if (current) setSelectedProgram(current);
      } else {
        // Senão seleciona o primeiro com saldo
        setSelectedProgram(availablePrograms[0]);
      }
    }
  }, [availablePrograms, selectedProgram, loyaltyProgramId]);

  useEffect(() => {
    // Se mudou o programa selecionado, reseta os pontos a serem usados (ou ajusta ao que já está no carrinho se for o mesmo programa)
    if (selectedProgram?.id === loyaltyProgramId) {
      setPointsToUse(loyaltyPointsUsed);
    } else {
      setPointsToUse(0);
    }
  }, [selectedProgram, loyaltyProgramId, loyaltyPointsUsed]);

  const pointsBalance = selectedProgram ? (balance?.balances?.find(b => b.programId === selectedProgram.id)?.balance || 0) : 0;
  const VALUE_PER_POINT = (selectedProgram?.currencyValue && selectedProgram.currencyValue > 0) ? selectedProgram.currencyValue : 1.0;

  const maxRedeemablePoints = Math.min(
    pointsBalance,
    Math.floor(subtotal / VALUE_PER_POINT)
  );

  const handleApply = () => {
    if (selectedProgram) {
      applyLoyaltyPoints(pointsToUse, VALUE_PER_POINT, selectedProgram.id);
    }
  };

  const handleRemove = () => {
    removeLoyaltyPoints();
    setPointsToUse(0);
  };

  const discountValue = pointsToUse * VALUE_PER_POINT;

  if (loading) return <div className="p-4 text-center text-sm text-gray-500">Carregando fidelidade...</div>;
  if (availablePrograms.length === 0) return null;

  return (
    <Card className="border-none shadow-sm bg-primary/5 rounded-[2rem] overflow-hidden">
      <CardHeader className="pb-2">
        <CardTitle className="text-base font-black uppercase italic tracking-tight flex items-center gap-2 text-primary">
          <Gift className="w-5 h-5" />
          Programa de Fidelidade
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">

        {/* Seletor de Programa (se tiver mais de um) */}
        {availablePrograms.length > 1 && (
          <div className="space-y-2">
            <label className="text-xs font-bold uppercase tracking-wider text-gray-500">Selecione o programa:</label>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="outline" className="w-full justify-between h-12 rounded-xl border-gray-200 bg-white font-bold">
                  {selectedProgram?.name || "Escolha um programa"}
                  <ChevronDown className="w-4 h-4 opacity-50" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent className="w-[calc(100vw-3rem)] max-w-md rounded-xl shadow-xl">
                {availablePrograms.map((p) => (
                  <DropdownMenuItem
                    key={p.id}
                    onClick={() => setSelectedProgram(p)}
                    className="flex justify-between py-3 cursor-pointer"
                  >
                    <span className="font-semibold">{p.name}</span>
                    <span className="text-primary font-bold">{balance?.balances?.find(b => b.programId === p.id)?.balance} pts</span>
                  </DropdownMenuItem>
                ))}
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        )}

        {selectedProgram && (
          <div className="bg-white/50 p-4 rounded-2xl border border-white space-y-4">
            <div className="flex justify-between items-center text-sm">
              <span className="text-gray-600 font-medium">
                Saldo em <span className="font-bold text-gray-900">{selectedProgram.name}</span>:
              </span>
              <span className="font-black text-primary text-lg">{pointsBalance} pontos</span>
            </div>

            {maxRedeemablePoints > 0 ? (
              <>
                <div className="space-y-4">
                  <div className="flex justify-between text-sm font-bold">
                    <span>Pontos para usar:</span>
                    <span className="text-primary">{pointsToUse}</span>
                  </div>

                  <Slider
                    value={[pointsToUse]}
                    max={maxRedeemablePoints}
                    step={1}
                    onValueChange={(vals) => setPointsToUse(vals[0])}
                    className="py-2"
                  />

                  <div className="flex justify-between items-center p-3 bg-green-50 rounded-xl border border-green-100">
                    <span className="text-xs font-bold text-green-700 uppercase">Desconto aplicado:</span>
                    <span className="font-black text-green-600">
                      {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(discountValue)}
                    </span>
                  </div>
                </div>

                <div className="flex gap-2 justify-end pt-2">
                  {loyaltyProgramId === selectedProgram.id && loyaltyPointsUsed > 0 && (
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={handleRemove}
                      className="text-red-500 border-red-200 hover:bg-red-50 h-11 rounded-xl px-6 font-bold"
                    >
                      Remover
                    </Button>
                  )}
                  <Button
                    size="sm"
                    onClick={handleApply}
                    disabled={pointsToUse === 0 || (pointsToUse === loyaltyPointsUsed && loyaltyProgramId === selectedProgram.id)}
                    className="bg-primary hover:bg-primary/90 text-white h-11 rounded-xl px-6 font-bold shadow-lg shadow-primary/20 flex gap-2"
                  >
                    {loyaltyProgramId === selectedProgram.id && loyaltyPointsUsed > 0 ? (
                      <><CheckCircle2 className="w-4 h-4" /> Aplicado</>
                    ) : (
                      "Aplicar Desconto"
                    )}
                  </Button>
                </div>
              </>
            ) : (
              <div className="text-sm text-gray-500 flex items-center gap-2 p-3 bg-white/80 rounded-xl border border-gray-100">
                <AlertCircle className="w-4 h-4 text-orange-400" />
                <span>O valor do pedido é menor que 1 ponto ou saldo zerado.</span>
              </div>
            )}
          </div>
        )}
      </CardContent>
    </Card>
  );
};
