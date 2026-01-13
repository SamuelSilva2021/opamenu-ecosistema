import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { CustomerResponseDto } from '@/types/api';
import { customerService, CreateCustomerRequestDto, UpdateCustomerRequestDto } from '@/services/customer-service';
import { useToast } from '@/components/ui/use-toast';

interface CustomerContextType {
  customer: CustomerResponseDto | null;
  isLoading: boolean;
  login: (phoneNumber: string) => Promise<boolean>;
  register: (data: CreateCustomerRequestDto) => Promise<void>;
  updateProfile: (data: UpdateCustomerRequestDto) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
}

const CustomerContext = createContext<CustomerContextType | undefined>(undefined);

export const CustomerProvider = ({ children, slug }: { children: ReactNode; slug?: string }) => {
  const [customer, setCustomer] = useState<CustomerResponseDto | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const { toast } = useToast();

  const storageKey = slug ? `opamenu-customer-${slug}` : 'opamenu-customer';

  useEffect(() => {
    if (!slug) return;
    const savedCustomer = localStorage.getItem(storageKey);
    if (savedCustomer) {
      try {
        setCustomer(JSON.parse(savedCustomer));
      } catch (e) {
        localStorage.removeItem(storageKey);
      }
    }
  }, [slug, storageKey]);

  const login = async (phoneNumber: string): Promise<boolean> => {
    if (!slug) return false;
    setIsLoading(true);
    try {
      const response = await customerService.getCustomer(slug, phoneNumber);
      
      // Handle ApiResponse structure safely
      const responseData = response as any;
      let customerData: CustomerResponseDto | null = null;

      if (responseData.succeeded && responseData.data) {
          customerData = responseData.data;
      } else if (responseData.id && responseData.name) {
          customerData = responseData as CustomerResponseDto;
      }

      if (customerData) {
           setCustomer(customerData);
           localStorage.setItem(storageKey, JSON.stringify(customerData));
           toast({
              title: "Bem-vindo!",
              description: `Olá, ${customerData.name}`,
           });
           return true;
      }
      
      // Not found or unsuccessful but no error thrown
      return false;
    } catch (error: any) {
        console.error("Login error", error);
        console.log(error.status);
        if (error.status === 404 || (error.response && error.response.status === 404)) {
             return false; 
        }

        toast({
            variant: "destructive",
            title: "Erro ao entrar",
            description: error.message || "Ocorreu um erro ao tentar entrar. Verifique o número e tente novamente.",
        });
        throw error;
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (data: CreateCustomerRequestDto) => {
    if (!slug) return;
    setIsLoading(true);
    try {
        const response = await customerService.createCustomer(slug, data);
        const responseData = response as any;
        const success = response.succeeded || responseData.success;
        const customerData = response.data || (success ? response : null);

        if (success && customerData) {
            setCustomer(customerData as CustomerResponseDto);
            localStorage.setItem(storageKey, JSON.stringify(customerData));
            toast({
                title: "Cadastro realizado!",
                description: `Bem-vindo, ${data.name}`,
            });
        }
    } catch (error: any) {
        console.error("Registration error", error);
        toast({
            variant: "destructive",
            title: "Erro ao cadastrar",
            description: error.message || "Ocorreu um erro ao realizar o cadastro.",
        });
        throw error;
    } finally {
        setIsLoading(false);
    }
  };

  const logout = () => {
    setCustomer(null);
    localStorage.removeItem(storageKey);
    toast({
        title: "Até logo!",
        description: "Você saiu da sua conta.",
    });
  };

  const updateProfile = async (data: UpdateCustomerRequestDto) => {
    if (!slug) return;
    setIsLoading(true);
    try {
        const response = await customerService.updateCustomer(slug, data);
        
        // Handle ApiResponse structure safely (same as login logic)
        const responseData = response as any;
        let customerData: CustomerResponseDto | null = null;

        if (responseData.succeeded && responseData.data) {
            customerData = responseData.data;
        } else if (responseData.id && responseData.name) {
            customerData = responseData as CustomerResponseDto;
        }

        if (customerData) {
            setCustomer(customerData);
            localStorage.setItem(storageKey, JSON.stringify(customerData));
            toast({
                title: "Perfil atualizado!",
                description: "Seus dados foram salvos com sucesso.",
            });
        }
    } catch (error: any) {
        console.error("Update profile error", error);
        toast({
            variant: "destructive",
            title: "Erro ao atualizar perfil",
            description: error.message || "Ocorreu um erro ao atualizar seus dados.",
        });
        throw error;
    } finally {
        setIsLoading(false);
    }
  };

  return (
    <CustomerContext.Provider value={{ customer, isLoading, login, register, updateProfile, logout, isAuthenticated: !!customer }}>
      {children}
    </CustomerContext.Provider>
  );
};

export const useCustomer = () => {
  const context = useContext(CustomerContext);
  if (context === undefined) {
    throw new Error('useCustomer must be used within a CustomerProvider');
  }
  return context;
};
