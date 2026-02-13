import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Gift, Wallet, ArrowRight } from "lucide-react";
import { LoyaltyProgramDto, CustomerResponseDto } from "@/types/api";
import { Link, useParams } from "react-router-dom";

interface LoyaltyCardProps {
  program: LoyaltyProgramDto;
  customer?: CustomerResponseDto | null;
}

const LoyaltyCard = ({ program, customer }: LoyaltyCardProps) => {
  const { slug } = useParams<{ slug: string }>();

  return (
    <Card className="border-none shadow-sm bg-primary/5">
      <CardHeader className="pb-2">
        <CardTitle className="text-base font-bold flex items-center justify-between text-gray-800">
          <div className="flex items-center gap-2">
            <div className="bg-primary p-1.5 rounded-full text-white">
              <Gift className="w-4 h-4" />
            </div>
            {program.name}
          </div>
          {customer && customer.loyaltyBalance !== undefined && (
            <div className="flex items-center gap-1.5 text-primary bg-white px-2 py-1 rounded-full shadow-sm text-sm">
              <Wallet className="w-3 h-3" />
              <span>{customer.loyaltyBalance} pts</span>
            </div>
          )}
        </CardTitle>
      </CardHeader>
      <CardContent className="text-sm text-gray-600 space-y-2">
        {program.description && (
          <p className="whitespace-pre-line line-clamp-2">{program.description}</p>
        )}
        <Link
          to={`/${slug}/loyalty`}
          className="flex items-center justify-between mt-4 text-xs font-black uppercase tracking-widest text-primary hover:opacity-80 transition-opacity"
        >
          Ver detalhes e metas
          <ArrowRight className="w-3 h-3" />
        </Link>
      </CardContent>
    </Card>
  );
};

export default LoyaltyCard;
