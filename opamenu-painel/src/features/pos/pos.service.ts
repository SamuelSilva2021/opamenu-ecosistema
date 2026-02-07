import { api } from "@/lib/axios";
import type { CartItem, OrderType } from "./types";
import type { Customer } from "../customers/types";

interface CreateOrderPayload {
    customerName: string;
    customerPhone: string;
    customerEmail?: string;
    deliveryAddress?: {
        street: string;
        number: string;
        neighborhood: string;
        city: string;
        state: string;
        zipCode: string;
        complement?: string;
    };
    isDelivery: boolean;
    orderType: OrderType;
    tableId?: string;
    notes?: string;
    couponCode?: string;
    deliveryFee?: number;
    items: {
        productId: string;
        quantity: number;
        notes?: string;
        aditionals: {
            aditionalId: string;
            quantity: number;
        }[];
    }[];
}

export const posService = {
    createOrder: async (
        items: CartItem[],
        customer: Customer | null,
        orderType: OrderType,
        deliveryFee: number,
        couponCode?: string,
        tableId?: string,
        deliveryAddress?: any,
        tempCustomer?: { name: string; phone: string }
    ) => {

        const name = customer?.name || tempCustomer?.name || "Cliente Balcão";
        const phone = customer?.phone || tempCustomer?.phone || "99999999999";

        const payload: CreateOrderPayload = {
            customerName: name,
            customerPhone: phone,
            customerEmail: customer?.email,
            isDelivery: orderType === "Delivery",
            orderType: orderType,
            tableId: orderType === "Table" ? tableId : undefined,
            couponCode: couponCode,
            deliveryFee: orderType === "Delivery" ? deliveryFee : 0,
            items: items.map(item => ({
                productId: item.product.id,
                quantity: item.quantity,
                notes: item.notes,
                aditionals: item.aditionals.map(aditional => ({
                    aditionalId: aditional.aditionalId,
                    quantity: aditional.quantity
                }))
            })),
            deliveryAddress: orderType === "Delivery" ? deliveryAddress : undefined
        };

        const response = await api.post("/orders", payload);
        return response.data;
    }
};
