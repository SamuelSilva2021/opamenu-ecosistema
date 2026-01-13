// --- API DTOs (Data Transfer Objects) ---
// Estes tipos refletem exatamente o retorno da API

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
  openingHours?: string | object; 
  paymentMethods?: string | object;
  latitude?: number;
  longitude?: number;
}

export interface CustomerResponseDto {
  id: string;
  name: string;
  phone: string;
  email?: string;
  postalCode?: string;
  street?: string;
  streetNumber?: string;
  neighborhood?: string;
  city?: string;
  state?: string;
  complement?: string;
  createdAt: string;
  updatedAt: string;
}

export interface AddonResponseDto {
  id: number;
  name: string;
  description?: string;
  price: number;
  imageUrl?: string;
  displayOrder: number;
  addonGroupId: number;
  isActive: boolean;
}

export interface AddonGroupResponseDto {
  id: number;
  name: string;
  description?: string;
  type: AddonGroupType;
  minSelections?: number;
  maxSelections?: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
  addons: AddonResponseDto[];
}

export interface ProductAddonGroupResponseDto {
  id: number;
  productId: number;
  addonGroupId: number;
  addonGroup: AddonGroupResponseDto;
  displayOrder: number;
  isRequired: boolean;
  minSelectionsOverride?: number;
  maxSelectionsOverride?: number;
}

export interface ProductDto {
  id: number;
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  imageUrl?: string;
  isActive: boolean;
  displayOrder: number;
  createdAt: string;
  updatedAt: string;
  categoryName: string;
  addonGroups: ProductAddonGroupResponseDto[];
}

export interface CategoryResponseDto {
  id: number;
  name: string;
  description?: string;
  displayOrder: number;
  isActive: boolean;
}

export interface MenuResponseDto {
  tenantBusiness?: TenantBusinessResponseDto;
  products?: ProductDto[];
  categories?: CategoryResponseDto[];
  coupons?: CouponDto[];
}

// --- Tipos de Domínio (Frontend) ---

export interface Category {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt?: string; // Optional no DTO
  updatedAt?: string; // Optional no DTO
}

export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  categoryId: number;
  imageUrl?: string;
  isActive: boolean;
  displayOrder: number;
  createdAt: string;
  updatedAt: string;
  categoryName?: string;
  addonGroups?: ProductAddonGroupResponseDto[];
}

export interface CartItem {
  id: number;
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  subtotal: number;
  notes?: string;
  selectedAddons?: SelectedAddon[];
}

export interface Cart {
  items: CartItem[];
  subtotal: number;
  deliveryFee: number;
  total: number;
}

export interface Customer {
  name: string;
  phone: string;
  email?: string;
  address?: string;
}

export interface OrderItem {
  id?: number;
  productId: number;
  productName?: string;
  quantity: number;
  unitPrice: number;
  subtotal?: number;
  notes?: string;
}

export interface AddressDto {
  zipCode: string;
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  state: string;
}

export interface CreateOrderItemAddonRequest {
  addonId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  deliveryAddress?: AddressDto;
  notes?: string;
  couponCode?: string;
  isDelivery: boolean;
  items: CreateOrderItemRequest[];
}

export interface CreateOrderItemRequest {
  productId: number;
  quantity: number;
  notes?: string;
  addons: CreateOrderItemAddonRequest[];
}

export enum PaymentMethod {
  CreditCard = 0,
  DebitCard = 1,
  Pix = 2,
  Cash = 3,
  BankTransfer = 4
}

export interface CancelOrderRequest {
  reason: string;
}

export interface UpdateOrderPaymentRequest {
  paymentMethod: PaymentMethod;
}

export interface UpdateOrderDeliveryTypeRequest {
  isDelivery: boolean;
  deliveryAddress?: string;
}

export interface Order {
  id: number;
  orderNumber?: string;
  customerName: string;
  customerPhone: string;
  customerEmail?: string;
  items: OrderItem[];
  subtotal: number;
  deliveryFee: number;
  total: number;
  status: OrderStatus;
  isDelivery: boolean;
  deliveryAddress?: string;
  notes?: string;
  estimatedPreparationMinutes?: number;
  createdAt: string;
  updatedAt: string;
}

export enum OrderStatus {
  Pending = 0,
  Confirmed = 1,
  Preparing = 2,
  Ready = 3, // ReadyForPickup
  OutForDelivery = 4,
  Delivered = 5,
  Cancelled = 6
}

// Addon Types
export enum AddonGroupType {
  Single = 1,
  Multiple = 2
}

export interface Addon {
  id: number;
  name: string;
  description?: string;
  price: number;
  isActive: boolean;
  displayOrder: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface AddonGroup {
  id: number;
  name: string;
  description?: string;
  type: AddonGroupType;
  minSelections?: number;
  maxSelections?: number;
  isRequired: boolean;
  displayOrder: number;
  isActive: boolean;
  addons: Addon[];
  createdAt?: string;
  updatedAt?: string;
}

export interface ProductWithAddons extends Omit<Product, 'addonGroups'> {
  addonGroups: AddonGroup[];
}

export interface SelectedAddon {
  addonId: number;
  quantity: number;
  addon: Addon;
}

export interface ProductSelection {
  product: Product;
  quantity: number;
  selectedAddons: SelectedAddon[];
  notes?: string;
}

// Coupon Types
export enum DiscountType {
  Percentage = 1,
  FixedAmount = 2
}

export interface Coupon {
  id: number;
  code: string;
  description?: string;
  discountType: DiscountType;
  discountValue: number;
  minOrderValue?: number;
  maxDiscountValue?: number;
  usageLimit?: number;
  usageCount?: number;
  startDate?: string;
  expirationDate?: string;
  isActive: boolean;
  firstOrderOnly?: boolean;
}

export type CouponDto = Coupon;

// Tenant & Storefront Types (Domain)
export interface TenantBusinessInfo {
  id: string;
  name: string;
  slug: string;
  description: string;
  logoUrl?: string;
  bannerUrl?: string;
  addressStreet: string;
  addressNumber: string;
  addressNeighborhood: string;
  addressCity: string;
  addressState: string;
  addressZipcode?: string;
  phone: string;
  whatsappNumber: string;
  email?: string;
  instagramUrl?: string;
  facebookUrl?: string;
  // No domínio, já queremos que seja objeto/array
  openingHours: Record<string, { open: string; close: string; isOpen: boolean }> | null;
  paymentMethods: string[];
  isOpen: boolean;
  latitude?: number;
  longitude?: number;
}

export interface StorefrontData {
  tenantBusiness: TenantBusinessInfo;
  categories: Category[];
  products: ProductWithAddons[];
  coupons?: Coupon[];
}

export interface ApiResponse<T> {
  data: T;
  succeeded: boolean;
  message?: string;
  errors?: string[];
}

export interface PaginatedResponse<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
