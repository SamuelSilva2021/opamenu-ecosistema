import axios from 'axios';
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';
import { API_BASE_URL, HTTP_STATUS } from '../constants';
import type { ApiError } from '../types';
import { getToken, clearAuth } from './auth-storage';

// Variável para controlar se já estamos fazendo logout
let isLoggingOut = false;

class HttpClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_BASE_URL,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors(): void {
    // Request interceptor - adiciona token de autenticação
    this.client.interceptors.request.use(
      (config) => {
        const token = getToken();
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor - trata respostas e erros SEM causar reloads
    this.client.interceptors.response.use(
      (response: AxiosResponse) => response,
      (error) => {
        // Tratamento especial para 401 (token expirado) SEM reload
        if (error.response?.status === HTTP_STATUS.UNAUTHORIZED) {
          // Só faz logout uma vez para evitar loops
          if (!isLoggingOut) {
            isLoggingOut = true;
            
            // Limpa dados de autenticação
            clearAuth();
            
            // Usa evento personalizado para notificar o store (SEM reload)
            window.dispatchEvent(new CustomEvent('auth:token-expired'));
            
            // Reset após timeout
            setTimeout(() => {
              isLoggingOut = false;
            }, 1000);
          }
        }
        
        // Tratamento para o padrão de erro da API Authentication (Array de erros no 400)
        if (error.response?.status === HTTP_STATUS.BAD_REQUEST && Array.isArray(error.response.data)) {
            const apiErrors = error.response.data;
            const errorMessage = apiErrors.map((e: any) => e.message || 'Erro desconhecido').join(', ');
            
            const apiError: ApiError = {
                message: errorMessage,
                status: error.response.status,
                errors: apiErrors
            };
            return Promise.reject(apiError);
        }

        const apiError: ApiError = {
          message: error.response?.data?.message || 'Erro interno do servidor',
          status: error.response?.status || HTTP_STATUS.INTERNAL_SERVER_ERROR,
          errors: error.response?.data?.errors,
        };
        
        return Promise.reject(apiError);
      }
    );
  }

  async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.get<T>(url, config);
    return response.data;
  }

  async post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.post<T>(url, data, config);
    return response.data;
  }

  async put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.put<T>(url, data, config);
    return response.data;
  }

  async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.delete<T>(url, config);
    return response.data;
  }

  async patch<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const response = await this.client.patch<T>(url, data, config);
    return response.data;
  }
}

export const httpClient = new HttpClient();