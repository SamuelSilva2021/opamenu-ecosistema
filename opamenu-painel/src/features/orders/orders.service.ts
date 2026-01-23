import { api } from "@/lib/axios";
import { OrderStatus, type Order, type UpdateOrderStatusRequest } from "./types";

export const ordersService = {
  getOrders: async (): Promise<Order[]> => {
    const response = await api.get<Order[]>("/orders");
    return response.data;
  },

  updateOrderStatus: async (id: string, status: OrderStatus): Promise<Order> => {
    const payload: UpdateOrderStatusRequest = { status };
    const response = await api.put<Order>(`/orders/${id}/status`, payload);
    return response.data;
  },
  
  acceptOrder: async (id: string, estimatedPreparationMinutes: number): Promise<Order> => {
    const response = await api.post<Order>(`/orders/${id}/accept`, { estimatedPreparationMinutes });
    return response.data;
  },
  
  rejectOrder: async (id: string, reason: string): Promise<Order> => {
    const response = await api.post<Order>(`/orders/${id}/reject`, { reason });
    return response.data;
  }
};
