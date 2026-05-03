import type { TenantProduct, CreateTenantProductRequest, UpdateTenantProductRequest } from '../types';
import { httpClient } from '../utils';
import { API_ENDPOINTS } from '../constants';

export class ProductService {
  static async getProducts(): Promise<TenantProduct[]> {
    const response = await httpClient.get<TenantProduct[]>(API_ENDPOINTS.TENANT_PRODUCTS);
    return response;
  }

  static async getProductById(id: string): Promise<TenantProduct> {
    const response = await httpClient.get<TenantProduct>(API_ENDPOINTS.TENANT_PRODUCT_BY_ID(id));
    return response;
  }

  static async createProduct(data: CreateTenantProductRequest): Promise<TenantProduct> {
    const response = await httpClient.post<TenantProduct>(API_ENDPOINTS.TENANT_PRODUCTS, data);
    return response;
  }

  static async updateProduct(id: string, data: UpdateTenantProductRequest): Promise<TenantProduct> {
    const response = await httpClient.put<TenantProduct>(API_ENDPOINTS.TENANT_PRODUCT_BY_ID(id), data);
    return response;
  }

  static async deleteProduct(id: string): Promise<boolean> {
    const response = await httpClient.delete<boolean>(API_ENDPOINTS.TENANT_PRODUCT_BY_ID(id));
    return response;
  }
}
