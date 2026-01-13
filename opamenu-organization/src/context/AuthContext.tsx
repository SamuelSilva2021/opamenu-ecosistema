import React, { createContext, useContext, useState, useEffect } from 'react';
import type { AuthState, LoginRequest } from '../types/auth.types';
import { authService } from '../services/auth.service';

interface AuthContextType extends AuthState {
  login: (data: LoginRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [state, setState] = useState<AuthState>({
    user: null,
    isAuthenticated: false,
    isLoading: true,
    requiresPayment: false
  });

  useEffect(() => {
    const initAuth = () => {
      const token = localStorage.getItem('accessToken');
      if (token) {
        const user = authService.getUserFromToken(token);
        if (user) {
          setState({
            user,
            isAuthenticated: true,
            isLoading: false,
            requiresPayment: false // Idealmente salvar isso no storage ou validar com API
          });
          return;
        }
      }
      setState(s => ({ ...s, isLoading: false }));
    };

    initAuth();
  }, []);

  const login = async (data: LoginRequest) => {
    setState(s => ({ ...s, isLoading: true }));
    try {
      const response = await authService.login(data);
      
      localStorage.setItem('accessToken', response.accessToken);
      localStorage.setItem('refreshToken', response.refreshToken);
      
      const user = authService.getUserFromToken(response.accessToken);
      
      setState({
        user,
        isAuthenticated: true,
        isLoading: false,
        requiresPayment: response.requiresPayment
      });
    } catch (error) {
      setState(s => ({ ...s, isLoading: false, isAuthenticated: false }));
      throw error;
    }
  };

  const logout = () => {
    authService.logout();
    setState({
      user: null,
      isAuthenticated: false,
      isLoading: false,
      requiresPayment: false
    });
  };

  return (
    <AuthContext.Provider value={{ ...state, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
