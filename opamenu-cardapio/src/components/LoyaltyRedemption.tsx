import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Slider } from "@/components/ui/slider";
import { Button } from "@/components/ui/button";
import { Gift, AlertCircle } from "lucide-react";
import { LoyaltyProgramDto, CustomerResponseDto } from "@/types/api";
import { useCart, useLoyalty } from "@/hooks";
import { useParams } from "react-router-dom";

interface LoyaltyRedemptionProps {
  program: LoyaltyProgramDto;
  customer: CustomerResponseDto;
}

export const LoyaltyRedemption = ({ program, customer }: LoyaltyRedemptionProps) => {
  const { slug } = useParams<{ slug: string }>();
  const { subtotal, applyLoyaltyPoints, removeLoyaltyPoints, loyaltyPointsUsed } = useCart();
  const { balance } = useLoyalty(slug, customer.phone);
  const [pointsToUse, setPointsToUse] = useState<number>(loyaltyPointsUsed);

  const pointsBalance = balance?.balance || 0;

  // Valor de resgate: 1 ponto = R$ 1.00 (Configuração temporária)
  // Idealmente isso viria do LoyaltyProgramDto (ex: redemptionValue)
  const VALUE_PER_POINT = program.currencyValue || 1.0;

  // Calcula o máximo de pontos que podem ser usados
  // Não pode usar mais que o saldo
  // Não pode usar mais que o valor total do pedido (subtotal)
  const maxRedeemablePoints = Math.min(
    pointsBalance,
    Math.floor(subtotal / VALUE_PER_POINT)
  );

  useEffect(() => {
    setPointsToUse(loyaltyPointsUsed);
  }, [loyaltyPointsUsed]);

  const handleApply = () => {
    applyLoyaltyPoints(pointsToUse, VALUE_PER_POINT);
  };

  const handleRemove = () => {
    removeLoyaltyPoints();
    setPointsToUse(0);
  };

  const discountValue = pointsToUse * VALUE_PER_POINT;

  // Se o programa não estiver ativo ou cliente sem saldo, não exibe nada
  if (!program.isActive || pointsBalance <= 0) {
    return null;
  }

  return (
    <Card className="border-none shadow-sm bg-primary/5 rounded-[2rem] overflow-hidden">
      <CardHeader className="pb-2">
        <CardTitle className="text-base font-black uppercase italic tracking-tight flex items-center gap-2 text-primary">
          <Gift className="w-5 h-5" />
          Programa de Fidelidade
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex justify-between items-center text-sm">
          <span className="text-gray-600">Seu saldo:</span>
          <span className="font-bold text-gray-900">{pointsBalance} pontos</span>
        </div>

        {maxRedeemablePoints > 0 ? (
          <>
            <div className="space-y-4">
              <div className="flex justify-between text-sm">
                <span>Usar pontos:</span>
                <span className="font-bold">{pointsToUse}</span>
              </div>

              <Slider
                value={[pointsToUse]}
                max={maxRedeemablePoints}
                step={1}
                onValueChange={(vals) => setPointsToUse(vals[0])}
                className="py-2"
              />

              <div className="flex justify-between items-center">
                <div className="text-xs text-gray-500">
                  Desconto: <span className="font-semibold text-green-600">
                    {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(discountValue)}
                  </span>
                </div>
              </div>
            </div>

            <div className="flex gap-2 justify-end pt-2">
              {loyaltyPointsUsed > 0 && (
                <Button
                  variant="outline"
                  size="sm"
                  onClick={handleRemove}
                  className="text-red-500 border-red-200 hover:bg-red-50 h-10 rounded-xl px-4 font-bold"
                >
                  Remover
                </Button>
              )}
              <Button
                size="sm"
                onClick={handleApply}
                disabled={pointsToUse === 0 || pointsToUse === loyaltyPointsUsed}
                className="bg-primary hover:bg-primary/90 text-white h-10 rounded-xl px-4 font-bold shadow-lg shadow-primary/20"
              >
                Aplicar Desconto
              </Button>
            </div>
          </>
        ) : (
          <div className="text-sm text-gray-500 flex items-center gap-2 p-2 bg-white/50 rounded-md border border-gray-100">
            <AlertCircle className="w-4 h-4 text-orange-400" />
            <span>O valor do pedido é menor que 1 ponto.</span>
          </div>
        )}
      </CardContent>
    </Card>
  );
};
