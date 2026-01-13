import { api } from "@/lib/axios";
import type { 
  SubscriptionStatusResponseDto, 
  CancelSubscriptionRequestDto, 
  ChangePlanRequestDto 
} from "./types";

export const subscriptionService = {
  getStatus: async () => {
    const response = await api.get<SubscriptionStatusResponseDto>("/subscription/status");
    return response.data;
  },

  cancelSubscription: async (data: CancelSubscriptionRequestDto) => {
    const response = await api.post<boolean>("/subscription/cancel", data);
    return response.data;
  },

  changePlan: async (data: ChangePlanRequestDto) => {
    const response = await api.post<boolean>("/subscription/change-plan", data);
    return response.data;
  },

  getBillingPortalUrl: async () => {
    const response = await api.get<string>("/subscription/billing-portal");
    return response.data;
  }
};
