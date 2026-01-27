export interface OpeningHoursDay {
  open: string;
  close: string;
  isOpen: boolean;
}

export interface OpeningHours {
  monday?: OpeningHoursDay;
  tuesday?: OpeningHoursDay;
  wednesday?: OpeningHoursDay;
  thursday?: OpeningHoursDay;
  friday?: OpeningHoursDay;
  saturday?: OpeningHoursDay;
  sunday?: OpeningHoursDay;
}

export interface TenantBusinessResponseDto {
  id: string;
  name: string;
  slug: string;
  description?: string;
  logoUrl?: string;
  bannerUrl?: string;
  instagramUrl?: string;
  facebookUrl?: string;
  whatsappNumber?: string;
  phone?: string;
  email?: string;
  addressStreet?: string;
  addressNumber?: string;
  addressComplement?: string;
  addressNeighborhood?: string;
  addressCity?: string;
  addressState?: string;
  addressZipcode?: string;
  openingHours?: OpeningHours;
  paymentMethods?: string[] | { methods: string[]; pixKey?: string } | any;
  latitude?: number;
  longitude?: number;
}

export interface UpdateTenantBusinessRequestDto {
  name?: string;
  logoUrl?: string;
  bannerUrl?: string;
  description?: string;
  instagramUrl?: string;
  facebookUrl?: string;
  whatsappNumber?: string;
  phone?: string;
  email?: string;
  addressStreet?: string;
  addressNumber?: string;
  addressComplement?: string;
  addressNeighborhood?: string;
  addressCity?: string;
  addressState?: string;
  addressZipcode?: string;
  openingHours?: OpeningHours;
  paymentMethods?: string[] | { methods: string[]; pixKey?: string } | any;
  latitude?: number;
  longitude?: number;
}

export interface BankDetailsDto {
  id: string;
  tenantId: string;
  bankName?: string;
  agency?: string;
  accountNumber?: string;
  accountType?: number;
  bankId?: number;
  pixKey?: string;
  isPixKeySelected: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateBankDetailsRequestDto {
  bankName?: string;
  agency?: string;
  accountNumber?: string;
  accountType?: number;
  bankId?: number;
  pixKey?: string;
  isPixKeySelected?: boolean;
}

export interface UpdateBankDetailsRequestDto extends CreateBankDetailsRequestDto {
  id: string;
}

export const EPaymentProvider = {
  MercadoPago: 0,
  PagarMe: 1,
  Gerencianet: 2,
} as const;

export type EPaymentProvider = typeof EPaymentProvider[keyof typeof EPaymentProvider];

export const EPaymentMethod = {
  CreditCard: 0,
  DebitCard: 1,
  Pix: 2,
  Cash: 3,
  BankTransfer: 4,
  Ticket: 5
} as const;

export type EPaymentMethod = typeof EPaymentMethod[keyof typeof EPaymentMethod];

export interface TenantPaymentConfigDto {
  id?: string;
  provider: EPaymentProvider;
  paymentMethod: EPaymentMethod;
  pixKey: string;
  clientId: string;
  clientSecret: string;
  publicKey?: string;
  accessToken?: string;
  isSandbox?: boolean;
  isActive: boolean;
}
