// Exportar todos os hooks customizados
export { useMenuProducts, useCategories, useProductsByCategory, useProductSearch, useApiHealth, useDebounce } from './use-api';
export { useTenantInfo } from './use-tenant';
export { useStorefront } from './use-storefront';
export { useCart } from './use-cart';
export { useOrder } from './use-order';
export { useCheckout } from './use-checkout';
export { useProductModal } from './useProductModal';
export { useCustomer } from './use-customer';

// Re-exportar tipos úteis
export type { CartHookReturn } from './use-cart';
export type { OrderHookReturn } from './use-order';
export type { CheckoutHookReturn } from './use-checkout';
