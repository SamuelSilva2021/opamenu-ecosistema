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
