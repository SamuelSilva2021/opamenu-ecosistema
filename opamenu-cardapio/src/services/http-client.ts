import { API_CONFIG } from '@/config/api';
import { ApiResponse } from '@/types/api';

// Classe de erro personalizada para APIs
export class ApiError extends Error {
  constructor(
    public status: number,
    public message: string,
    public data?: any
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

// Cliente HTTP para comunicação com a API
export class HttpClient {
  private baseURL: string;
  private defaultHeaders: HeadersInit;

  constructor() {
    this.baseURL = API_CONFIG.BASE_URL;
    this.defaultHeaders = API_CONFIG.DEFAULT_HEADERS;
  }

  // Método genérico para fazer requisições
  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseURL}${endpoint}`;
    
    const config: RequestInit = {
      ...options,
      headers: {
        ...this.defaultHeaders,
        ...options.headers,
      },
    };

    try {
      const response = await fetch(url, config);      
      const data = await response.json();

      if (!response.ok) {
        throw new ApiError(
          response.status,
          data.message || `HTTP Error: ${response.status}`,
          data
        );
      }

      return data;
    } catch (error) {      
      if (error instanceof ApiError) {
        throw error;
      }
      
      // Erro de rede ou parsing
      throw new ApiError(
        0,
        error instanceof Error ? error.message : 'Network error',
        error
      );
    }
  }

  // Métodos HTTP específicos
  async get<T>(endpoint: string, params?: Record<string, string>): Promise<T> {
    const url = params 
      ? `${endpoint}?${new URLSearchParams(params)}`
      : endpoint;
      
    return this.request<T>(url, {
      method: 'GET',
    });
  }

  async post<T>(endpoint: string, data?: any): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async put<T>(endpoint: string, data?: any): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, {
      method: 'DELETE',
    });
  }

  // Método para verificar conectividade
  async healthCheck(): Promise<boolean> {
    try {
      await this.get('/health');
      return true;
    } catch {
      return false;
    }
  }
}

// Instância singleton do cliente HTTP
export const httpClient = new HttpClient();

// Utilitários para tratamento de erros
export const handleApiError = (error: unknown): string => {
  if (error instanceof ApiError) {
    switch (error.status) {
      case 0:
        return 'Erro de conexão. Verifique sua internet.';
      case 400:
        return 'Dados inválidos enviados.';
      case 401:
        return 'Não autorizado.';
      case 403:
        return 'Acesso negado.';
      case 404:
        return 'Recurso não encontrado.';
      case 500:
        return 'Erro interno do servidor.';
      default:
        return error.message || 'Erro desconhecido.';
    }
  }
  
  return 'Erro inesperado.';
};

// Wrapper para respostas da API com tratamento de erro
export const withApiErrorHandling = async <T>(
  apiCall: () => Promise<T>
): Promise<T> => {
  try {
    const response = await apiCall();
    return response;
    
  } catch (error) {
    throw error;
  }
};
