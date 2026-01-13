import { useState } from "react";
import {
  DndContext,
  DragOverlay,
  useSensor,
  useSensors,
  PointerSensor,
  type DragStartEvent,
  type DragEndEvent,
  closestCorners,
} from "@dnd-kit/core";
import { OrderStatus, type Order } from "../types";
import { KanbanColumn } from "./KanbanColumn";
import { OrderCard } from "./OrderCard";
import { createPortal } from "react-dom";

interface OrdersKanbanProps {
  orders: Order[];
  onOrderMove: (orderId: number, newStatus: OrderStatus) => void;
}

const COLUMNS = [
  { id: OrderStatus.Pending, title: "Recebido", color: "bg-blue-500" },
  { id: OrderStatus.Confirmed, title: "Confirmado", color: "bg-indigo-500" },
  { id: OrderStatus.Preparing, title: "Em Preparo", color: "bg-orange-500" },
  { id: OrderStatus.Ready, title: "Pronto", color: "bg-green-500" },
  { id: OrderStatus.Delivered, title: "Entregue/Retirado", color: "bg-slate-500" },
];

export function OrdersKanban({ orders, onOrderMove }: OrdersKanbanProps) {
  const [activeOrder, setActiveOrder] = useState<Order | null>(null);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: {
        distance: 8,
      },
    })
  );

  function handleDragStart(event: DragStartEvent) {
    const { active } = event;
    const order = orders.find((o) => o.id === active.id);
    if (order) {
      setActiveOrder(order);
    }
  }

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;

    if (!over) {
      setActiveOrder(null);
      return;
    }

    const activeId = active.id;
    const overId = over.id; // This could be an Order ID or a Column ID (Status)

    // Find the order that was dragged
    const activeOrder = orders.find((o) => o.id === activeId);
    if (!activeOrder) return;

    // Check if dropped over a column (status) directly
    // overId is string "0", "1", "2" etc from Column ID
    // or number 123 from Order ID
    
    let newStatus: OrderStatus | undefined;

    // If dropped on a column container
    if (COLUMNS.some(c => c.id.toString() === overId.toString())) {
      newStatus = Number(overId) as OrderStatus;
    } 
    // If dropped on another card
    else {
      const overOrder = orders.find(o => o.id === overId);
      if (overOrder) {
        newStatus = overOrder.status;
      }
    }

    if (newStatus !== undefined && newStatus !== activeOrder.status) {
      onOrderMove(activeOrder.id, newStatus);
    }

    setActiveOrder(null);
  }

  return (
    <DndContext
      sensors={sensors}
      collisionDetection={closestCorners}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
    >
      <div className="flex h-[calc(100vh-220px)] gap-4 overflow-x-auto pb-4">
        {COLUMNS.map((col) => (
          <KanbanColumn
            key={col.id}
            id={col.id.toString()}
            title={col.title}
            color={col.color}
            orders={orders.filter((o) => o.status === col.id)}
          />
        ))}
      </div>

      {createPortal(
        <DragOverlay>
          {activeOrder ? <OrderCard order={activeOrder} /> : null}
        </DragOverlay>,
        document.body
      )}
    </DndContext>
  );
}
