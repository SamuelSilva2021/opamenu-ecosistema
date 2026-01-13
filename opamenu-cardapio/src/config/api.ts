export const API_CONFIG = {
  // URL base da API - usando proxy do Vite em desenvolvimento
  BASE_URL: import.meta.env.VITE_API_URL || '/api',
  
  // Timeout para requisições (30 segundos)
  TIMEOUT: 30000,
  
  // Headers padrão
  DEFAULT_HEADERS: {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
  },
} as const;

// Exportar BASE_URL separadamente para facilitar o uso
export const API_BASE_URL = API_CONFIG.BASE_URL;

// Endpoints da API
export const API_ENDPOINTS = {
  // Categorias
  CATEGORIES: '/categories',
  CATEGORIES_ACTIVE: '/categories/active',
  
  // Produtos
  PRODUCTS: '/products',
  PRODUCTS_MENU: '/products/menu',
  PRODUCTS_BY_CATEGORY: (categoryId: number) => `/products/by-category/${categoryId}`,
  PRODUCT_WITH_ADDONS: (productId: number) => `/products/${productId}/with-addons`,
  
  // Pedidos
  ORDERS: '/orders',
  ORDER_BY_ID: (id: number) => `/orders/${id}`,
  
  // Health check
  HEALTH: '/health',

  // Endpoints Públicos (Slug-based)
  PUBLIC: {
     MENU: (slug: string) => `/public/${slug}/menu`,
     CATEGORIES: (slug: string) => `/public/${slug}/categories`,
     PRODUCTS_BY_CATEGORY: (slug: string, categoryId: number) => `/public/${slug}/products/by-category/${categoryId}`,
     PRODUCT: (slug: string, id: number) => `/public/${slug}/products/${id}`,
    ORDERS: (slug: string) => `/public/${slug}/orders`,
    ORDER: (slug: string, id: number) => `/public/${slug}/orders/${id}`,
    ORDERS_BY_CUSTOMER: (slug: string, customerId: string) => `/public/${slug}/orders/customer/${customerId}`,
    CUSTOMER: (slug: string) => `/public/${slug}/customer`
  }
} as const;

// Configuração de ambiente
export const ENV = {
  isDevelopment: import.meta.env.MODE === 'development',
  isProduction: import.meta.env.MODE === 'production',
  apiUrl: import.meta.env.VITE_API_URL,
} as const;

// Configurações específicas do cliente
export const CLIENT_CONFIG = {
  // Configurações do carrinho
  CART: {
    // Tempo para salvar no localStorage (minutos)
    SAVE_INTERVAL: 5,
    // Máximo de itens no carrinho
    MAX_ITEMS: 50,
    // Taxa de entrega padrão
    DEFAULT_DELIVERY_FEE: 5.00,
  },
  
  // Configurações de paginação
  PAGINATION: {
    DEFAULT_PAGE_SIZE: 20,
    MAX_PAGE_SIZE: 100,
  },
  
  // Configurações de cache
  CACHE: {
    // Tempo de cache para produtos (minutos)
    PRODUCTS_TTL: 10,
    // Tempo de cache para categorias (minutos)  
    CATEGORIES_TTL: 30,
  },
} as const;
