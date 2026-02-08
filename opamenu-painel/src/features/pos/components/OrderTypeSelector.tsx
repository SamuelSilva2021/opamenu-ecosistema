import { Button } from "@/components/ui/button";
import { OrderType } from "../types";
import { Store, Truck } from "lucide-react";
import { cn } from "@/lib/utils";

interface OrderTypeSelectorProps {
   value: OrderType;
   onChange: (value: OrderType) => void;
}

export function OrderTypeSelector({ value, onChange }: OrderTypeSelectorProps) {
   return (
      <div className="grid w-full grid-cols-2 gap-1 bg-muted p-1 rounded-lg">
         <Button
            variant="ghost"
            size="sm"
            className={cn("gap-2", value === OrderType.Counter && "bg-background shadow-sm")}
            onClick={() => onChange(OrderType.Counter)}
         >
            <Store className="h-4 w-4" />
            Balcão
         </Button>
         <Button
            variant="ghost"
            size="sm"
            className={cn("gap-2", value === OrderType.Delivery && "bg-background shadow-sm")}
            onClick={() => onChange(OrderType.Delivery)}
         >
            <Truck className="h-4 w-4" />
            Entrega
         </Button>
      </div>
   );
}
