import { Card, CardContent } from "@/components/ui/card";
import { Bike, ChevronRight, MapPin } from "lucide-react";
import { TenantBusinessInfo, CustomerResponseDto } from "@/types/api";

interface DeliveryInfoProps {
    tenant: TenantBusinessInfo;
    customer?: CustomerResponseDto | null;
}

const DeliveryInfo = ({ tenant, customer }: DeliveryInfoProps) => {
  const addressDisplay = customer?.street 
    ? `${customer.street}, ${customer.streetNumber}` 
    : (tenant.addressStreet ? `${tenant.addressStreet}, ${tenant.addressNumber}` : "Endereço de entrega");

  return (
    <Card className="border-none shadow-sm">
      <CardContent className="p-4">
        <div className="flex items-start justify-between cursor-pointer hover:bg-gray-50 transition-colors -m-2 p-2 rounded-lg">
          <div className="flex gap-3">
             <div className="mt-1 text-gray-400">
                 <Bike className="w-5 h-5" />
             </div>
             <div className="space-y-1">
                 <p className="text-sm font-semibold text-gray-800">
                     {addressDisplay}
                 </p>
                 <p className="text-xs text-gray-500 flex items-center gap-2">
                     Entrega em 40-50 min <span className="w-1 h-1 bg-gray-300 rounded-full"></span> R$ 5,00
                 </p>
             </div>
          </div>
          <ChevronRight className="w-5 h-5 text-primary mt-2" />
        </div>
      </CardContent>
    </Card>
  );
};

export default DeliveryInfo;
