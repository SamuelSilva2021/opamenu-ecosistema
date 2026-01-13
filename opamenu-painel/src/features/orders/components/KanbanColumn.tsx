import { useDroppable } from "@dnd-kit/core";
import type { Order } from "../types";
import { OrderCard } from "./OrderCard";
import { SortableContext, verticalListSortingStrategy } from "@dnd-kit/sortable";

interface KanbanColumnProps {
  id: string; // This will be the status string/number
  title: string;
  orders: Order[];
  color: string;
}

export function KanbanColumn({ id, title, orders, color }: KanbanColumnProps) {
  const { setNodeRef } = useDroppable({
    id: id,
  });

  return (
    <div className="flex flex-col h-full min-w-[280px] w-full bg-muted/30 rounded-lg p-2">
      <div className={`flex items-center justify-between p-3 mb-2 rounded-md ${color} text-white`}>
        <h3 className="font-semibold text-sm">{title}</h3>
        <span className="text-xs bg-white/20 px-2 py-0.5 rounded-full">
          {orders.length}
        </span>
      </div>
      
      <div ref={setNodeRef} className="flex-1 overflow-y-auto px-1">
        <SortableContext items={orders.map(o => o.id)} strategy={verticalListSortingStrategy}>
          {orders.map((order) => (
            <OrderCard key={order.id} order={order} />
          ))}
          {orders.length === 0 && (
            <div className="h-20 flex items-center justify-center text-xs text-muted-foreground border-2 border-dashed rounded-md">
              Arraste pedidos aqui
            </div>
          )}
        </SortableContext>
      </div>
    </div>
  );
}
