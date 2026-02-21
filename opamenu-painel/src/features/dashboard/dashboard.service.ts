import { api } from "@/lib/axios";
import type { DashboardSummary } from "./types";

export const dashboardService = {
  getSummary: async (params?: { startDate?: string; endDate?: string }): Promise<DashboardSummary> => {
    const response = await api.get<DashboardSummary>("/dashboard/summary", { params });
    return response.data;
  },
};
