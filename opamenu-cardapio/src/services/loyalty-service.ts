import { httpClient } from './http-client';
import { API_ENDPOINTS } from '@/config/api';
import { ApiResponse, LoyaltyProgramDto, CustomerLoyaltySummaryDto } from '@/types/api';

export const loyaltyService = {
    /**
     * Obtém todos os programas de fidelidade ativos para uma loja específica
     */
    getPrograms: async (slug: string) => {
        const url = API_ENDPOINTS.PUBLIC.LOYALTY.PROGRAMS(slug);
        return httpClient.get<ApiResponse<LoyaltyProgramDto[]>>(url);
    },

    /**
     * Obtém o saldo de fidelidade de um cliente para uma loja específica
     */
    getCustomerBalance: async (slug: string, phone: string) => {
        const url = API_ENDPOINTS.PUBLIC.LOYALTY.BALANCE(slug, phone);
        return httpClient.get<ApiResponse<CustomerLoyaltySummaryDto>>(url);
    }
};
