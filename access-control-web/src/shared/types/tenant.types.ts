// Tipos de Tenant baseados na API
export interface Tenant {
  id: string;
  name: string;
  slug: string;
  domain?: string;
  status: string;
  cnpjCpf?: string;
  razaoSocial?: string;
  inscricaoEstadual?: string;
  inscricaoMunicipal?: string;
  phone?: string;
  email?: string;
  website?: string;
  
  // Endereço
  addressStreet?: string;
  addressNumber?: string;
  addressComplement?: string;
  addressNeighborhood?: string;
  addressCity?: string;
  addressState?: string;
  addressZipcode?: string;
  addressCountry?: string;

  // Endereço de Cobrança
  billingStreet?: string;
  billingNumber?: string;
  billingComplement?: string;
  billingNeighborhood?: string;
  billingCity?: string;
  billingState?: string;
  billingZipcode?: string;
  billingCountry?: string;
  
  // Representante Legal
  legalRepresentativeName?: string;
  legalRepresentativeCpf?: string;
  legalRepresentativeEmail?: string;
  legalRepresentativePhone?: string;
  
  // Assinatura
  activeSubscriptionId?: string;
  
  // Datas
  createdAt: string;
  updatedAt?: string;
  
  // Configurações
  settings: Record<string, any>;
}

export interface TenantSummary {
  id: string;
  name: string;
  slug: string;
  domain?: string;
  status: string;
  email?: string;
  phone?: string;
  createdAt: string;
  updatedAt?: string;
  activeSubscriptionId?: string;
}

export interface CreateTenantRequest {
  name: string;
  slug: string;
  domain?: string;
  email?: string;
  phone?: string;
  cnpjCpf?: string;
  razaoSocial?: string;
}

export interface TenantFilters {
  name?: string;
  slug?: string;
  domain?: string;
  email?: string;
  phone?: string;
  status?: string;
}