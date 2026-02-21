import { api } from "@/lib/axios";
import type {
    CashShift,
    CashMovement,
    OpenCashShiftRequest,
    CloseCashShiftRequest,
    AddCashMovementRequest
} from "./types";

export const cashRegisterService = {
    getActiveShift: async (): Promise<CashShift | null> => {
        const response = await api.get<CashShift | null>("/cash-register/active");
        return response.data;
    },

    openShift: async (request: OpenCashShiftRequest): Promise<CashShift> => {
        const response = await api.post<CashShift>("/cash-register/open", request);
        return response.data;
    },

    closeShift: async (request: CloseCashShiftRequest): Promise<CashShift> => {
        const response = await api.post<CashShift>("/cash-register/close", request);
        return response.data;
    },

    addMovement: async (request: AddCashMovementRequest): Promise<CashMovement> => {
        const response = await api.post<CashMovement>("/cash-register/movement", request);
        return response.data;
    },

    getHistory: async (count: number = 20): Promise<CashShift[]> => {
        const response = await api.get<CashShift[]>("/cash-register/history", { params: { count } });
        return response.data;
    }
};
