import React, { createContext, useContext, useEffect, useState } from 'react';
import { GlobalLoading } from '../../shared/components';
import { useAuthStore } from '../../shared/stores/auth.store';
import { usePermissionStore } from '../../shared/stores/permission.store';
import { getStoredUser } from '../../shared/utils/auth-storage';

interface AppInitializationState {
  isLoading: boolean;
  isInitialized: boolean;
  error: string | null;
}

interface AppInitializationContextType extends AppInitializationState {
  retry: () => void;
}

const AppInitializationContext = createContext<AppInitializationContextType | null>(null);

export const useAppInitialization = () => {
  const context = useContext(AppInitializationContext);
  if (!context) {
    throw new Error('useAppInitialization must be used within AppInitializationProvider');
  }
  return context;
};

interface AppInitializationProviderProps {
  children: React.ReactNode;
}

export const AppInitializationProvider: React.FC<AppInitializationProviderProps> = ({ children }) => {
  const [state, setState] = useState<AppInitializationState>({
    isLoading: true,
    isInitialized: false,
    error: null,
  });

  const { initialize: initializeAuth, isAuthenticated, isLoading: authLoading } = useAuthStore();
  const { setPermissions, clearPermissions } = usePermissionStore();

  const initializeApp = async () => {
    try {
      setState(prev => ({ ...prev, isLoading: true, error: null }));

      await initializeAuth();

      const currentState = useAuthStore.getState();
      console.log('👤 Estado da autenticação:', {
        isAuthenticated: currentState.isAuthenticated,
        hasUser: !!currentState.user,
      });

      if (currentState.isAuthenticated && currentState.user) {

        const storedUserData = getStoredUser();
        if (storedUserData && storedUserData.permissions) {
          setPermissions(storedUserData.permissions);     
        } else {
          clearPermissions();
        }
      } else {
        clearPermissions();
      }

      setState({
        isLoading: false,
        isInitialized: true,
        error: null,
      });

      console.log('🎉 Aplicação inicializada com sucesso!');
      
    } catch (error) {
      console.error('❌ Erro durante inicialização:', error);
      
      setState({
        isLoading: false,
        isInitialized: false,
        error: error instanceof Error ? error.message : 'Erro desconhecido durante inicialização',
      });
    }
  };

  const retry = () => {
    initializeApp();
  };

  useEffect(() => {
    initializeApp();
  }, []);

  useEffect(() => {
    if (authLoading) {
      setState(prev => ({ ...prev, isLoading: true }));
    } else if (state.isInitialized) {
      setState(prev => ({ ...prev, isLoading: false }));
    }
  }, [authLoading, state.isInitialized]);

  useEffect(() => {
    if (state.isInitialized && !authLoading) {
      const currentState = useAuthStore.getState();
      
      if (currentState.isAuthenticated && currentState.user) {
        const storedUserData = getStoredUser();
        if (storedUserData && storedUserData.permissions) {
          console.log('🔄 Reconfigurando permissões após login...');
          setPermissions(storedUserData.permissions);
        }
      } else {
        console.log('🔄 Limpando permissões após logout...');
        clearPermissions();
      }
    }
  }, [isAuthenticated, authLoading, state.isInitialized, setPermissions, clearPermissions]);

  const contextValue: AppInitializationContextType = {
    ...state,
    retry,
  };

  if (state.isLoading || authLoading) {
    const message = authLoading ? "Processando Login" : "Inicializando Sistema";
    const submessage = authLoading ? 
      "Autenticando usuário e carregando permissões..." : 
      "Carregando permissões e configurações do usuário...";
    
    return (
      <GlobalLoading
        message={message}
        submessage={submessage}
      />
    );
  }

  if (state.error) {
    return (
      <GlobalLoading
        message="Erro na Inicialização"
        submessage={`${state.error}. Tentando novamente...`}
      />
    );
  }

  return (
    <AppInitializationContext.Provider value={contextValue}>
      {children}
    </AppInitializationContext.Provider>
  );
};