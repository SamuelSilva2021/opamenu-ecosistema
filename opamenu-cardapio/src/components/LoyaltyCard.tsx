import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Gift } from "lucide-react";
import { LoyaltyProgramDto } from "@/types/api";

interface LoyaltyCardProps {
  program: LoyaltyProgramDto;
}

const LoyaltyCard = ({ program }: LoyaltyCardProps) => {
  return (
    <Card className="border-none shadow-sm bg-primary/5">
      <CardHeader className="pb-2">
        <CardTitle className="text-base font-bold flex items-center gap-2 text-gray-800">
          <div className="bg-primary p-1.5 rounded-full text-white">
            <Gift className="w-4 h-4" />
          </div>
          {program.name}
        </CardTitle>
      </CardHeader>
      <CardContent className="text-sm text-gray-600 space-y-2">
        {program.description && (
          <p className="whitespace-pre-line">{program.description}</p>
        )}
      </CardContent>
    </Card>
  );
};

export default LoyaltyCard;
