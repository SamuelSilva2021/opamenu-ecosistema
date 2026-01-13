import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Gift } from "lucide-react";

const LoyaltyCard = () => {
  return (
    <Card className="border-none shadow-sm bg-orange-50/50">
      <CardHeader className="pb-2">
        <CardTitle className="text-base font-bold flex items-center gap-2 text-gray-800">
          <div className="bg-[#FF4500] p-1.5 rounded-full text-white">
            <Gift className="w-4 h-4" />
          </div>
          Programa de fidelidade
        </CardTitle>
      </CardHeader>
      <CardContent className="text-sm text-gray-600 space-y-2">
        <p>
          A cada <span className="font-bold">R$ 1,00</span> em compras você ganha <span className="font-bold">1 ponto</span> que pode ser trocado por prêmios.
        </p>
        <p className="text-xs text-gray-500">
          Os novos clientes ganham automaticamente <span className="font-bold text-gray-700">10 pontos</span>.
        </p>
      </CardContent>
    </Card>
  );
};

export default LoyaltyCard;
