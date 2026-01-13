import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { Clock, User } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import type { Order } from "../types";
import { formatDistanceToNow } from "date-fns";
import { ptBR } from "date-fns/locale";

interface OrderCardProps {
  order: Order;
}

export function OrderCard({ order }: OrderCardProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: order.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <div ref={setNodeRef} style={style} {...attributes} {...listeners} className="mb-3">
      <Card className="cursor-grab active:cursor-grabbing hover:shadow-md transition-shadow">
        <CardHeader className="p-4 pb-2">
          <div className="flex justify-between items-start">
            <CardTitle className="text-sm font-medium">#{order.id}</CardTitle>
            <Badge variant="outline" className="text-xs">
              {order.isDelivery ? "Delivery" : "Retirada"}
            </Badge>
          </div>
          <div className="flex items-center text-xs text-muted-foreground mt-1">
            <User className="h-3 w-3 mr-1" />
            <span className="truncate max-w-[120px]">{order.customerName}</span>
          </div>
        </CardHeader>
        <CardContent className="p-4 pt-2">
          <div className="space-y-2">
            <div className="text-sm text-muted-foreground line-clamp-2">
              {order.items.map((item) => (
                <div key={item.id} className="flex justify-between text-xs">
                  <span>{item.quantity}x {item.productName}</span>
                </div>
              ))}
            </div>
            
            <div className="flex justify-between items-center pt-2 border-t mt-2">
              <div className="flex items-center text-xs text-muted-foreground" title={new Date(order.createdAt).toLocaleString()}>
                <Clock className="h-3 w-3 mr-1" />
                {formatDistanceToNow(new Date(order.createdAt), { addSuffix: true, locale: ptBR })}
              </div>
              <span className="font-bold text-sm">
                {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(order.total)}
              </span>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
