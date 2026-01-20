import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { ordersService } from "../orders.service";
import { OrdersKanban } from "../components/OrdersKanban";
import { OrderStatus, type Order } from "../types";
import { Loader2, RefreshCcw } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useToast } from "@/hooks/use-toast";
import { usePermission } from "@/hooks/usePermission";

export default function OrdersPage() {
  const { can } = usePermission();
  const queryClient = useQueryClient();
  const { toast } = useToast();
  
  const canUpdate = can("ORDER", "UPDATE");

  const { data: orders = [], isLoading, isFetching } = useQuery({
    queryKey: ["orders"],
    queryFn: ordersService.getOrders,
    refetchInterval: 30000, // Poll every 30s
  });

  const updateStatusMutation = useMutation({
    mutationFn: ({ id, status }: { id: number; status: OrderStatus }) =>
      ordersService.updateOrderStatus(id, status),
    onMutate: async ({ id, status }) => {
      // Optimistic update
      await queryClient.cancelQueries({ queryKey: ["orders"] });
      const previousOrders = queryClient.getQueryData<Order[]>(["orders"]);

      queryClient.setQueryData<Order[]>(["orders"], (old) =>
        old ? old.map((order) =>
          order.id === id ? { ...order, status } : order
        ) : []
      );

      return { previousOrders };
    },
    onError: (_err, _newTodo, context: { previousOrders: Order[] | undefined } | undefined) => {
      if (context?.previousOrders) {
        queryClient.setQueryData(["orders"], context.previousOrders);
      }
      toast({
        title: "Erro ao atualizar status",
        description: "Não foi possível mover o pedido.",
        variant: "destructive",
      });
    },
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ["orders"] });
    },
  });

  const handleOrderMove = (orderId: number, newStatus: OrderStatus) => {
    updateStatusMutation.mutate({ id: orderId, status: newStatus });
  };

  return (
    <div className="flex-1 space-y-4 p-8 pt-6">
      <div className="flex items-center justify-between space-y-2">
        <h2 className="text-3xl font-bold tracking-tight">Pedidos</h2>
        <div className="flex items-center space-x-2">
          <Button 
            variant="outline" 
            size="sm" 
            onClick={() => queryClient.invalidateQueries({ queryKey: ["orders"] })}
            disabled={isFetching}
          >
            <RefreshCcw className={`mr-2 h-4 w-4 ${isFetching ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
        </div>
      </div>

      {isLoading ? (
        <div className="flex h-[400px] items-center justify-center">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
        </div>
      ) : (
        <OrdersKanban orders={orders} onOrderMove={handleOrderMove} readOnly={!canUpdate} />
      )}
    </div>
  );
}
