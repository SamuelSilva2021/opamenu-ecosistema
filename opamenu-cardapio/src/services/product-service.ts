import { httpClient, withApiErrorHandling } from './http-client';
import { API_ENDPOINTS } from '@/config/api';
import { Product, Category, ApiResponse, PaginatedResponse, ProductWithAddons, ProductDto, MenuResponseDto, ProductAddonGroupResponseDto, AddonGroup, Addon } from '@/types/api';

// Serviço para gerenciar produtos
export class ProductService {
  // Buscar todos os produtos disponíveis para o menu
  async getMenuProducts(): Promise<Product[]> {
    return withApiErrorHandling(async () => {
      return httpClient.get<Product[]>(API_ENDPOINTS.PRODUCTS_MENU);
    });
  }

  // Buscar produto por ID
  async getProductById(id: number): Promise<Product> {
    return withApiErrorHandling(async () => {
      return httpClient.get<Product>(`${API_ENDPOINTS.PRODUCTS}/${id}`);
    });
  }

  // Buscar produto com adicionais por ID
  async getProductWithAddons(id: number): Promise<ProductWithAddons> {
    return withApiErrorHandling(async () => {
      const response = await httpClient.get<ApiResponse<ProductWithAddons>>(
        API_ENDPOINTS.PRODUCT_WITH_ADDONS(id)
      );
      return response.data;
    });
  }

  // Buscar produtos por categoria
  async getProductsByCategory(categoryId: number): Promise<Product[]> {
    return withApiErrorHandling(async () => {
      const response = await httpClient.get<ApiResponse<Product[]>>(
        API_ENDPOINTS.PRODUCTS_BY_CATEGORY(categoryId)
      );
      return response.data;
    });
  }

  // Buscar produtos com filtros
  async getProducts(filters?: {
    categoryId?: number;
    search?: string;
    isActive?: boolean;
    pageNumber?: number;
    pageSize?: number;
  }): Promise<PaginatedResponse<Product>> {
    const params: Record<string, string> = {};
    
    if (filters?.categoryId) params.categoryId = filters.categoryId.toString();
    if (filters?.search) params.search = filters.search;
    if (filters?.isActive !== undefined) params.isActive = filters.isActive.toString();
    if (filters?.pageNumber) params.pageNumber = filters.pageNumber.toString();
    if (filters?.pageSize) params.pageSize = filters.pageSize.toString();

    return withApiErrorHandling(async () => {
      const response = await httpClient.get<ApiResponse<PaginatedResponse<Product>>>(
        API_ENDPOINTS.PRODUCTS,
        params
      );
      return response.data;
    });
  }

  // Buscar menu público por slug
  async getMenuProductsBySlug(slug: string): Promise<MenuResponseDto> {
    return withApiErrorHandling(async () => {
      const response = await httpClient.get<MenuResponseDto>(API_ENDPOINTS.PUBLIC.MENU(slug));
      return response;
    });
  }

  // Buscar produto por ID e Slug
  async getProductByIdAndSlug(id: number, slug: string): Promise<ProductDto> {
    return withApiErrorHandling(async () => {
      const response = await httpClient.get<ProductDto>(API_ENDPOINTS.PUBLIC.PRODUCT(slug, id));
      return response;
    });
  }
}

// Serviço para gerenciar categorias
export class CategoryService {
  // Buscar todas as categorias ativas
  async getActiveCategories(): Promise<Category[]> {
    return withApiErrorHandling(async () => {
      const response = await httpClient.get<ApiResponse<Category[]>>(API_ENDPOINTS.CATEGORIES_ACTIVE);
      return response.data;
    });
  }

  // Buscar categorias ativas por slug
  async getActiveCategoriesBySlug(slug: string): Promise<Category[]> {
    return withApiErrorHandling(async () => {
      const response = await httpClient.get<ApiResponse<Category[]>>(API_ENDPOINTS.PUBLIC.CATEGORIES(slug));
      return response.data || [];
    });
  }

  // Buscar categoria por ID
  async getCategoryById(id: number): Promise<Category> {
    return withApiErrorHandling(async () => {
      const response = await httpClient.get<ApiResponse<Category>>(`${API_ENDPOINTS.CATEGORIES}/${id}`);
      return response.data;
    });
  }
}

// Cache simples para melhor performance
interface CacheEntry<T> { data: T; expiry: number }

class CacheService {
  private cache = new Map<string, CacheEntry<unknown>>();

  set<T>(key: string, data: T, ttlMinutes: number): void {
    const expiry = Date.now() + ttlMinutes * 60 * 1000;
    this.cache.set(key, { data, expiry });
  }

  get<T>(key: string): T | null {
    const item = this.cache.get(key);
    if (!item) return null;

    if (Date.now() > item.expiry) {
      this.cache.delete(key);
      return null;
    }

    return item.data as T;
  }

  clear(): void {
    this.cache.clear();
  }
}

// Instâncias dos serviços
export const productService = new ProductService();
export const categoryService = new CategoryService();
export const cacheService = new CacheService();

// Serviços com cache
export const getCachedProducts = async (slug?: string): Promise<Product[]> => {
  if (!slug) return [];

  const cacheKey = `menu-products-${slug}`;
  
  // Tentar buscar do cache primeiro
  const cached = cacheService.get<ProductDto[]>(cacheKey);
  if (cached) {
    return cached;
  }
  
  // Buscar da API e cachear
  const menuResponse = await productService.getMenuProductsBySlug(slug);
  const items = menuResponse.products || [];
  const mapped = items.map(mapProductDtoToProduct);
  cacheService.set(cacheKey, mapped, 10);
  return mapped; 
};

export const getCachedCategories = async (slug?: string): Promise<Category[]> => {
  if (!slug) return [];

  const cacheKey = `active-categories-${slug}`;
  
  // Tentar buscar do cache primeiro
  const cached = cacheService.get<Category[]>(cacheKey);
  if (cached) {
    return cached;
  }
  
  // Buscar da API e cachear
  const categories = await categoryService.getActiveCategoriesBySlug(slug);
  cacheService.set(cacheKey, categories, 30); // Cache por 30 minutos
  
  return categories;
};

// Função para buscar produto com adicionais com cache
export const getCachedProductWithAddons = async (productId: number, slug?: string): Promise<ProductWithAddons> => {
  if (!slug) throw new Error("Slug required");

  const cacheKey = `product-with-addons-${slug}-${productId}`;
  
  // Tentar buscar do cache primeiro
  const cached = cacheService.get<ProductWithAddons>(cacheKey);
  if (cached) {
    return cached;
  }
  
  // Buscar da API e cachear
  const product = await productService.getProductByIdAndSlug(productId, slug);

  const wrappers: ProductAddonGroupResponseDto[] = product.addonGroups || [];

  const mappedAddonGroups: AddonGroup[] = wrappers.map((wrapper) => {
    const group = wrapper.addonGroup;
    const minSelections = wrapper.minSelectionsOverride ?? group.minSelections;
    const maxSelections = wrapper.maxSelectionsOverride ?? group.maxSelections;
    const addons: Addon[] = group.addons.map((a) => ({
      id: a.id,
      name: a.name,
      description: a.description,
      price: a.price,
      isActive: a.isActive,
      displayOrder: a.displayOrder,
    }));

    return {
      id: group.id,
      name: group.name,
      description: group.description,
      type: group.type,
      minSelections,
      maxSelections,
      isRequired: wrapper.isRequired,
      displayOrder: wrapper.displayOrder,
      isActive: group.isActive,
      addons,
    };
  });

  mappedAddonGroups.sort((a, b) => a.displayOrder - b.displayOrder);

  const productWithAddons: ProductWithAddons = {
    ...mapProductDtoToProduct(product),
    addonGroups: mappedAddonGroups,
  };

  cacheService.set(cacheKey, productWithAddons, 5); // Cache por 5 minutos
  
  return productWithAddons;
};

const mapProductDtoToProduct = (dto: ProductDto): Product => ({
  id: dto.id,
  name: dto.name,
  description: dto.description || '',
  price: dto.price,
  categoryId: dto.categoryId,
  imageUrl: dto.imageUrl,
  isActive: dto.isActive,
  displayOrder: dto.displayOrder,
  createdAt: dto.createdAt,
  updatedAt: dto.updatedAt,
  categoryName: dto.categoryName,
});


// Limpar cache quando necessário (útil para desenvolvimento)
export const clearCache = () => {
  cacheService.clear();
};

// Validar se os produtos estão chegando corretamente
export const validateApiConnection = async (): Promise<boolean> => {
  try {
    const products = await productService.getMenuProducts();
    const categories = await categoryService.getActiveCategories();
    
    return true;
  } catch (error) {
    return false;
  }
};
