import axios, { type AxiosInstance } from 'axios';
import { useAuthStore } from '@/store/auth.store';

// Service: Business Core
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:7243/api',
});

// Service: Authentication & Access Control
export const apiAuth = axios.create({
  baseURL: import.meta.env.VITE_API_URL_AUTH || 'https://localhost:7019/api',
});

// Helper to attach interceptors to an instance
const attachInterceptors = (axiosInstance: AxiosInstance) => {
  axiosInstance.interceptors.request.use((config) => {
    const token = useAuthStore.getState().accessToken;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });

  axiosInstance.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        useAuthStore.getState().logout();
      }
      return Promise.reject(error);
    }
  );
};

// Attach to both instances
attachInterceptors(api);
attachInterceptors(apiAuth);
