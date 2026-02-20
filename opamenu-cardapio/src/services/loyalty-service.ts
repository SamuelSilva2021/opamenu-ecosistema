import { httpClient } from './http-client';
import { API_ENDPOINTS } from '@/config/api';
import { ApiResponse, LoyaltyProgramDto, CustomerLoyaltySummaryDto } from '@/types/api';

export const loyaltyService = {
    /**
     * Obtém todos os programas de fidelidade ativos para uma loja específica
     */
    getPrograms: async (slug: string) => {
        const url = API_ENDPOINTS.PUBLIC.LOYALTY.PROGRAMS(slug);
        const data = await httpClient.get<LoyaltyProgramDto[]>(url);
        return {
            data,
            succeeded: true
        } as ApiResponse<LoyaltyProgramDto[]>;
    },

    /**
     * Obtém o saldo de fidelidade de um cliente para uma loja específica
     */
    getCustomerBalance: async (slug: string, phone: string) => {
        const url = API_ENDPOINTS.PUBLIC.LOYALTY.BALANCE(slug, phone);
        const data = await httpClient.get<CustomerLoyaltySummaryDto>(url);
        return {
            data,
            succeeded: true
        } as ApiResponse<CustomerLoyaltySummaryDto>;
    }
};
