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
    async (error: any) => {
      const originalRequest = error.config;

      // If error is 401 and we haven't tried to refresh yet
      if (error.response?.status === 401 && !originalRequest._retry) {
        originalRequest._retry = true;

        try {
          const refreshToken = useAuthStore.getState().refreshToken;
          if (!refreshToken) {
            throw new Error("No refresh token");
          }

          // Use a new instance or raw axios to avoid interceptor loop if this fails
          // We use the same BaseURL as 'api' (Business Core) because AuthController is in the main API
          const response = await axios.post(
            `${api.defaults.baseURL}/auth/refresh-token`,
            { refreshToken }
          );

          if (response.data && response.data.accessToken) {
            const { accessToken, refreshToken: newRefreshToken } = response.data;

            // Update store
            useAuthStore.getState().setAccessToken(accessToken, newRefreshToken || refreshToken);

            // Update authorization header
            originalRequest.headers.Authorization = `Bearer ${accessToken}`;

            // Retry original request
            return axios(originalRequest);
          }
        } catch (refreshError) {
          // If refresh fails, logout
          useAuthStore.getState().logout();
          return Promise.reject(refreshError);
        }
      }

      return Promise.reject(error);
    }
  );
};

// Attach to both instances
attachInterceptors(api);
attachInterceptors(apiAuth);
