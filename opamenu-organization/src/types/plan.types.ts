export type BillingCycle = 'Mensal' | 'Anual' | 'Semestral' | 'Semanal' | 'Diario';
export type PlanStatus = 'Ativo' | 'Inativo' | 'Pendente' | 'Cancelado';

export interface Plan {
  id: string;
  name: string;
  slug: string;
  description?: string;
  price: number;
  billingCycle: BillingCycle;
  maxUsers: number;
  maxStorageGb: number;
  features?: string;
  status: PlanStatus;
  sortOrder: number;
  createdAt: string;
  updatedAt?: string;
  totalSubscriptions: number;
  activeSubscriptions: number;
  monthlyRecurringRevenue: number;
  isTrial?: boolean;
  trialPeriodDays?: number;
}

export interface CreatePlanDTO {
  name: string;
  slug: string;
  description?: string;
  price: number;
  billingCycle: BillingCycle;
  maxUsers: number;
  maxStorageGb: number;
  features?: string;
  status: PlanStatus;
  sortOrder: number;
  isTrial?: boolean;
  trialPeriodDays?: number;
}

export interface UpdatePlanDTO {
  id: string;
  name?: string;
  slug?: string;
  description?: string;
  price?: number;
  billingCycle?: BillingCycle;
  maxUsers?: number;
  maxStorageGb?: number;
  features?: string;
  status?: PlanStatus;
  sortOrder?: number;
  isTrial?: boolean;
  trialPeriodDays?: number;
}

export interface PlanFilter {
  name?: string;
  isActive?: boolean;
  status?: PlanStatus;
  page: number;
  pageSize: number;
}

export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

export interface PlanListResponse {
  items: Plan[];
  page: number;
  limit: number;
  total: number;
  totalPages: number;
  succeeded: boolean;
  code: number;
  currentPage: number;
  pageSize: number;
}

export interface ErrorDTO {
  message: string;
}

export interface ApiResponse<T> {
  succeeded: boolean;
  data: T;
  errors: ErrorDTO[];
}
