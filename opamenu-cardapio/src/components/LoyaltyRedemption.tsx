import { useState, useEffect, useMemo } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Slider } from "@/components/ui/slider";
import { Button } from "@/components/ui/button";
import { Gift, AlertCircle, ChevronDown, CheckCircle2 } from "lucide-react";
import { LoyaltyProgramDto, CustomerResponseDto, ELoyaltyProgramType, ELoyaltyRewardType } from "@/types/api";
import { useCart, useLoyalty } from "@/hooks";
import { useParams } from "react-router-dom";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";

interface LoyaltyRedemptionProps {
  program?: LoyaltyProgramDto; // Legado, mantido para compatibilidade se necessário
  customer: CustomerResponseDto;
}

export const LoyaltyRedemption = ({ program: initialProgram, customer }: LoyaltyRedemptionProps) => {
  const { slug } = useParams<{ slug: string }>();
  const { items, subtotal, applyLoyaltyPoints, removeLoyaltyPoints, loyaltyPointsUsed, loyaltyProgramId, loyaltyDiscount } = useCart();
  const { balance, programs, loading } = useLoyalty(slug, customer.phone);

  // Helper function to calculate eligible subtotal for a program based on its filters
  const calculateEligibleSubtotal = (program: LoyaltyProgramDto) => {
    if (!program.filters || program.filters.length === 0) {
      return subtotal; // No filters, all items are eligible
    }

    const eligibleItemsValue = items.reduce((acc, item) => {
      const matchFilter = program.filters!.some(f => {
        const productMatch = f.productId ? f.productId.toLowerCase() === item.product.id.toLowerCase() : false;
        const categoryMatch = f.categoryId ? f.categoryId.toLowerCase() === item.product.categoryId.toLowerCase() : false;
        return productMatch || categoryMatch;
      });

      let isEligible = true;
      if (program.type === ELoyaltyProgramType.PointsPerValue) {
        // For PointsPerValue, filters are EXCLUSIONS (e.g. valid for everything EXCEPT pizzas)
        isEligible = !matchFilter;
      } else if (program.type === ELoyaltyProgramType.ItemCount) {
        // For ItemCount, filters are INCLUSIONS (e.g. valid ONLY for pizzas)
        isEligible = matchFilter;
      }

      return isEligible ? acc + item.totalPrice : acc;
    }, 0);

    return eligibleItemsValue;
  };

  // Encontrar o programa selecionado atualmente ou o primeiro disponível com saldo
  const [selectedProgram, setSelectedProgram] = useState<LoyaltyProgramDto | null>(null);
  const [pointsToUse, setPointsToUse] = useState<number>(loyaltyPointsUsed);

  // Filtrar programas que o usuário realmente tem saldo E que têm itens elegíveis no carrinho
  const activeBalances = balance?.balances?.filter(b => b.balance > 0) || [];
  const availablePrograms = programs.filter(p => {
    const hasBalance = activeBalances.some(b => b.programId === p.id);
    const eligibleSubtotal = calculateEligibleSubtotal(p);
    return hasBalance && eligibleSubtotal > 0;
  });

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
    } else if (availablePrograms.length === 0 && selectedProgram) {
      // Se o carrinho mudou e não há mais programas válidos
      setSelectedProgram(null);
      if (loyaltyPointsUsed > 0) {
        removeLoyaltyPoints();
      }
    }
  }, [availablePrograms, selectedProgram, loyaltyProgramId, loyaltyPointsUsed, removeLoyaltyPoints]);

  useEffect(() => {
    // Se mudou o programa selecionado, reseta os pontos a serem usados (ou ajusta ao que já está no carrinho se for o mesmo programa)
    if (selectedProgram?.id === loyaltyProgramId) {
      setPointsToUse(loyaltyPointsUsed);
    } else {
      setPointsToUse(0);
    }
  }, [selectedProgram, loyaltyProgramId, loyaltyPointsUsed]);

  const pointsBalance = selectedProgram ? (balance?.balances?.find(b => b.programId === selectedProgram.id)?.balance || 0) : 0;
  const VALUE_PER_POINT = (selectedProgram?.rewardValue && selectedProgram.rewardValue > 0) ? selectedProgram.rewardValue : 1.0;
  const eligibleSubtotal = selectedProgram ? calculateEligibleSubtotal(selectedProgram) : 0;

  const maxRedeemablePoints = Math.min(
    pointsBalance,
    Math.floor(eligibleSubtotal / VALUE_PER_POINT)
  );

  useEffect(() => {
    // Safety check if cart items change making maxRedeemablePoints less than what is currently applied
    if (selectedProgram?.id === loyaltyProgramId && pointsToUse > maxRedeemablePoints) {
      setPointsToUse(maxRedeemablePoints);
      // Let the discountValue effect handle the applyLoyaltyPoints call for consistency
    }
  }, [maxRedeemablePoints, pointsToUse, selectedProgram, loyaltyProgramId]);

  const handleApply = () => {
    if (selectedProgram) {
      applyLoyaltyPoints(pointsToUse, discountValue, selectedProgram.id);
    }
  };

  const handleRemove = () => {
    removeLoyaltyPoints();
    setPointsToUse(0);
  };

  // Helper calculation for discount value based on program type
  const calculatedDiscount = useMemo(() => {
    if (!selectedProgram) return 0;

    // ItemCount + FreeProduct = Free item of the same category (most expensive)
    if (selectedProgram.type === ELoyaltyProgramType.ItemCount &&
      selectedProgram.rewardType === ELoyaltyRewardType.FreeProduct &&
      selectedProgram.targetCount && selectedProgram.targetCount > 0) {

      const numFreeItems = Math.floor(pointsToUse / selectedProgram.targetCount);
      if (numFreeItems <= 0) return 0;

      // Pegar todos os itens elegíveis do carrinho
      const eligibleItems = items.filter(item => {
        if (!selectedProgram.filters || selectedProgram.filters.length === 0) return true;
        return selectedProgram.filters.some(f => {
          const productMatch = f.productId ? f.productId.toLowerCase() === item.product.id.toLowerCase() : false;
          const categoryMatch = f.categoryId ? f.categoryId.toLowerCase() === item.product.categoryId.toLowerCase() : false;
          return productMatch || categoryMatch;
        });
      });

      // Expand items by quantity and sort by price descending
      const expandedPrices = eligibleItems.flatMap(item =>
        Array(item.quantity).fill(item.unitPrice)
      ).sort((a, b) => b - a);

      // Sum the prices of the N most expensive items
      return expandedPrices.slice(0, numFreeItems).reduce((sum, price) => sum + price, 0);
    }

    // Default: Points * ValuePerPoint
    return pointsToUse * VALUE_PER_POINT;
  }, [selectedProgram, pointsToUse, items, VALUE_PER_POINT]);

  const discountValue = calculatedDiscount;

  // Auto-reapply discount if cart changes and it's a dynamic discount (FreeProduct)
  useEffect(() => {
    if (selectedProgram?.id === loyaltyProgramId &&
      selectedProgram.type === ELoyaltyProgramType.ItemCount &&
      selectedProgram.rewardType === ELoyaltyRewardType.FreeProduct) {

      if (discountValue !== loyaltyDiscount) {
        applyLoyaltyPoints(pointsToUse, discountValue, selectedProgram.id);
      }
    }
  }, [items, discountValue, loyaltyDiscount, loyaltyProgramId, selectedProgram, pointsToUse, applyLoyaltyPoints]);

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
                  {selectedProgram.type === ELoyaltyProgramType.ItemCount &&
                    selectedProgram.rewardType === ELoyaltyRewardType.FreeProduct ? (
                    <div className="flex flex-col gap-3">
                      <div className="flex justify-between items-center bg-white p-3 rounded-xl border border-gray-100 h-14">
                        <span className="text-xs font-bold uppercase text-gray-400">Produtos para resgatar:</span>
                        <div className="flex items-center gap-4">
                          <Button
                            variant="outline"
                            size="icon"
                            className="h-8 w-8 rounded-full"
                            onClick={() => setPointsToUse(p => Math.max(0, p - (selectedProgram.targetCount || 1)))}
                            disabled={pointsToUse === 0}
                          >
                            -
                          </Button>
                          <span className="font-black text-lg w-4 text-center">
                            {Math.floor(pointsToUse / (selectedProgram.targetCount || 1))}
                          </span>
                          <Button
                            variant="outline"
                            size="icon"
                            className="h-8 w-8 rounded-full"
                            onClick={() => setPointsToUse(p => Math.min(pointsBalance, p + (selectedProgram.targetCount || 1)))}
                            disabled={pointsToUse + (selectedProgram.targetCount || 1) > maxRedeemablePoints}
                          >
                            +
                          </Button>
                        </div>
                      </div>
                      <p className="text-[10px] text-gray-400 text-center px-2">
                        Meta: <b>{selectedProgram.targetCount} pontos</b> por produto grátis.
                      </p>
                    </div>
                  ) : (
                    <>
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
                    </>
                  )}

                  <div className="flex justify-between items-center p-3 bg-green-50 rounded-xl border border-green-100">
                    <span className="text-xs font-bold text-green-700 uppercase">
                      {selectedProgram.rewardType === ELoyaltyRewardType.FreeProduct ? "Valor dos itens grátis:" : "Desconto aplicado:"}
                    </span>
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
