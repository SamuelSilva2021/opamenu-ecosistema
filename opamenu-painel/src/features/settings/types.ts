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
  paymentMethods?: string[]; // Array of payment method names/IDs
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
  paymentMethods?: string[];
  latitude?: number;
  longitude?: number;
}
