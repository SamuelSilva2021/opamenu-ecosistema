import type { Subscription } from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

export class SubscriptionService {
  static async getByTenantId(tenantId: string): Promise<Subscription | null> {
    const dto = await httpClient.get<Subscription>(
      API_ENDPOINTS.SUBSCRIPTION_BY_TENANT(tenantId),
    );

    if (!dto) {
      return null;
    }

    const emptyGuid = '00000000-0000-0000-0000-000000000000';

    const isEmptySubscription =
      !dto.id || dto.id === emptyGuid;

    if (isEmptySubscription) {
      return null;
    }

    return dto;
  }
}
