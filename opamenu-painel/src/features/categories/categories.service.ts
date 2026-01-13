import { api } from "@/lib/axios";
import type { ApiResponse } from "@/types/api";
import type {
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from "./types";

export const categoriesService = {
  getCategories: async (): Promise<Category[]> => {
    const response = await api.get<Category[]>("/categories");
    return response.data;
  },

  getActiveCategories: async (): Promise<Category[]> => {
    const response = await api.get<Category[]>("/categories/active");
    return response.data;
  },

  getCategoryById: async (id: number): Promise<Category> => {
    const response = await api.get<Category>(`/categories/${id}`);
    return response.data;
  },

  createCategory: async (data: CreateCategoryRequest): Promise<ApiResponse<Category>> => {
    const response = await api.post<ApiResponse<Category>>("/categories", data);
    return response.data;
  },

  updateCategory: async (id: number, data: UpdateCategoryRequest): Promise<ApiResponse<Category>> => {
    const response = await api.patch<ApiResponse<Category>>(`/categories/${id}`, data);
    return response.data;
  },

  deleteCategory: async (id: number): Promise<ApiResponse<boolean>> => {
    const response = await api.delete<ApiResponse<boolean>>(`/categories/${id}`);
    return response.data;
  },

  toggleCategoryStatus: async (id: number): Promise<ApiResponse<Category>> => {
    const response = await api.put<ApiResponse<Category>>(`/categories/${id}/toggle-active`);
    return response.data;
  },

  canDeleteCategory: async (id: number): Promise<{ canDelete: boolean }> => {
    const response = await api.get<{ canDelete: boolean }>(`/categories/${id}/can-delete`);
    return response.data;
  }
};
