import { api } from "@/lib/axios";
import type { ApiResponse } from "@/types/api";
import type {
  Product,
  CreateProductRequest,
  UpdateProductRequest,
  ProductSearchRequest,
  ProductAddonGroupResponse,
  AddProductAddonGroupRequest,
  UpdateProductAddonGroupRequest,
} from "./types";

export const productsService = {
  getProducts: async (filters?: ProductSearchRequest): Promise<Product[]> => {
    const params = new URLSearchParams();
    if (filters?.searchTerm) params.append("searchTerm", filters.searchTerm);
    if (filters?.categoryId) params.append("categoryId", filters.categoryId.toString());
    if (filters?.minPrice) params.append("minPrice", filters.minPrice.toString());
    if (filters?.maxPrice) params.append("maxPrice", filters.maxPrice.toString());

    const response = await api.get<Product[]>("/products", { params });
    return response.data;
  },

  getProductById: async (id: number): Promise<Product> => {
    const response = await api.get<Product>(`/products/${id}`);
    return response.data;
  },

  createProduct: async (data: CreateProductRequest): Promise<ApiResponse<Product>> => {
    const response = await api.post<ApiResponse<Product>>("/products", data);
    return response.data;
  },

  updateProduct: async (id: number, data: UpdateProductRequest): Promise<ApiResponse<Product>> => {
    const response = await api.put<ApiResponse<Product>>(`/products/${id}`, data);
    return response.data;
  },

  deleteProduct: async (id: number): Promise<ApiResponse<boolean>> => {
    const response = await api.delete<ApiResponse<boolean>>(`/products/${id}`);
    return response.data;
  },

  toggleProductStatus: async (id: number): Promise<ApiResponse<Product>> => {
    const response = await api.patch<ApiResponse<Product>>(`/products/${id}/toggle-status`);
    return response.data;
  },

  // Product Addon Groups
  getProductAddonGroups: async (productId: number): Promise<ProductAddonGroupResponse[]> => {
    const response = await api.get<ProductAddonGroupResponse[]>(`/products/${productId}/addon-groups`);
    return response.data;
  },

  addProductAddonGroup: async (productId: number, data: AddProductAddonGroupRequest): Promise<ApiResponse<ProductAddonGroupResponse>> => {
    const response = await api.post<ApiResponse<ProductAddonGroupResponse>>(`/products/${productId}/addon-groups`, data);
    return response.data;
  },

  updateProductAddonGroup: async (productId: number, addonGroupId: number, data: UpdateProductAddonGroupRequest): Promise<ApiResponse<ProductAddonGroupResponse>> => {
    const response = await api.put<ApiResponse<ProductAddonGroupResponse>>(`/products/${productId}/addon-groups/${addonGroupId}`, data);
    return response.data;
  },

  deleteProductAddonGroup: async (productId: number, addonGroupId: number): Promise<ApiResponse<boolean>> => {
    const response = await api.delete<ApiResponse<boolean>>(`/products/${productId}/addon-groups/${addonGroupId}`);
    return response.data;
  },
};
