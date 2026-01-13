import { httpClient } from './http-client';
import { TenantBusinessInfo, ApiResponse } from '@/types/api';

export const getTenantInfo = async (slug: string): Promise<TenantBusinessInfo> => {
  try {
    const response = await httpClient.get<TenantBusinessInfo>(`/public/${slug}/info`);
    return response
  } catch (error) {
    console.error('Error fetching tenant info:', error);
    throw error;
  }
};
