import { useState, useEffect } from 'react';
import { loyaltyService } from '@/services/loyalty-service';
import { LoyaltyProgramDto, CustomerLoyaltySummaryDto } from '@/types/api';

export const useLoyalty = (slug?: string, customerPhone?: string) => {
    const [programs, setPrograms] = useState<LoyaltyProgramDto[]>([]);
    const [balance, setBalance] = useState<CustomerLoyaltySummaryDto | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!slug) return;

        const fetchLoyaltyData = async () => {
            setLoading(true);
            setError(null);
            try {
                console.log('Fetching loyalty programs for slug:', slug);
                const programsRes = await loyaltyService.getPrograms(slug);
                console.log('Programs response:', programsRes);
                if (programsRes.succeeded) {
                    setPrograms(programsRes.data);
                }

                if (customerPhone) {
                    console.log('Fetching customer balance for phone:', customerPhone);
                    const balanceRes = await loyaltyService.getCustomerBalance(slug, customerPhone);
                    console.log('Balance response:', balanceRes);
                    if (balanceRes.succeeded) {
                        setBalance(balanceRes.data);
                    }
                } else {
                    setBalance(null);
                }
            } catch (err) {
                console.error('Error fetching loyalty data:', err);
                setError('Não foi possível carregar as informações de fidelidade.');
            } finally {
                setLoading(false);
            }
        };

        fetchLoyaltyData();
    }, [slug, customerPhone]);

    return {
        programs,
        balance,
        loading,
        error,
        refresh: async () => {
            if (!slug) return;
            setLoading(true);
            try {
                const programsRes = await loyaltyService.getPrograms(slug);
                if (programsRes.succeeded) setPrograms(programsRes.data);
                if (customerPhone) {
                    const balanceRes = await loyaltyService.getCustomerBalance(slug, customerPhone);
                    if (balanceRes.succeeded) setBalance(balanceRes.data);
                }
            } finally {
                setLoading(false);
            }
        }
    };
};
