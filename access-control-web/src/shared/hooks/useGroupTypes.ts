import { useState, useCallback, useEffect } from 'react';
import type { 
  GroupType, 
  CreateGroupTypeRequest, 
  UpdateGroupTypeRequest,
  PaginatedResponse 
} from '../types';
import { GroupTypeService } from '../services';
import { useAsyncAction } from './useAsyncAction';

/**
 * Interface para o retorno do hook useGroupTypes
 */
export interface UseGroupTypesReturn {
  // Estado
  groupTypes: GroupType[];
  loading: boolean;
  error: string | null;
  
  // Estados de carregamento específicos
  creating: boolean;
  updating: boolean;
  deleting: boolean;
  
  // Ações
  fetchGroupTypes: () => Promise<void>;
  createGroupType: (data: CreateGroupTypeRequest) => Promise<GroupType | null>;
  updateGroupType: (id: string, data: UpdateGroupTypeRequest) => Promise<GroupType | null>;
  deleteGroupType: (id: string) => Promise<boolean>;
  clearError: () => void;
}

/**
 * Hook para gerenciar operações CRUD de Group Types
 * Centraliza estado e lógica de negócio com tipagem forte
 */
export const useGroupTypes = (): UseGroupTypesReturn => {
  // Estado principal
  const [groupTypes, setGroupTypes] = useState<GroupType[]>([]);
  const [error, setError] = useState<string | null>(null);

  // Actions com loading states tipados
  const {
    execute: executeFetch,
    loading: fetchLoading,
  } = useAsyncAction();

  const {
    execute: executeCreate,
    loading: creating,
  } = useAsyncAction();

  const {
    execute: executeUpdate,
    loading: updating,
  } = useAsyncAction();

  const {
    execute: executeDelete,
    loading: deleting,
  } = useAsyncAction();

  /**
   * Limpa mensagens de erro
   */
  const clearError = useCallback((): void => {
    setError(null);
  }, []);

  /**
   * Busca todos os tipos de grupos
   */
  const fetchGroupTypes = useCallback(async (): Promise<void> => {
    try {
      setError(null);
      const data = await executeFetch<PaginatedResponse<GroupType>>(() => 
        GroupTypeService.getGroupTypes()
      );
      
      if (data) {
        setGroupTypes(data.data);
      }
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao carregar tipos de grupo';
      setError(errorMessage);
      console.error('❌ Erro ao buscar group types:', err);
    }
  }, [executeFetch]);

  /**
   * Cria um novo tipo de grupo
   */
  const createGroupType = useCallback(async (data: CreateGroupTypeRequest): Promise<GroupType | null> => {
    try {
      setError(null);
      
      const newGroupType = await executeCreate<GroupType>(() => 
        GroupTypeService.createGroupType(data)
      );
      
      if (newGroupType) {
        // Adiciona o novo item na lista
        setGroupTypes(prev => {
          const updated = [newGroupType, ...prev];
          return updated;
        });
        return newGroupType;
      }
      
      return null;
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao criar tipo de grupo';
      setError(errorMessage);
      console.error('❌ Erro ao criar group type:', err);
      return null;
    }
  }, [executeCreate]);

  /**
   * Atualiza um tipo de grupo existente
   */
  const updateGroupType = useCallback(async (id: string, data: UpdateGroupTypeRequest): Promise<GroupType | null> => {
    try {
      setError(null);
      
      const updatedGroupType = await executeUpdate<GroupType>(() => 
        GroupTypeService.updateGroupType(id, data)
      );
      
      if (updatedGroupType) {
        // Atualiza o item na lista
        setGroupTypes(prev => {
          const updated = prev.map(item => 
            item.id === id ? updatedGroupType : item
          );
          return updated;
        });
        return updatedGroupType;
      }
      
      return null;
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao atualizar tipo de grupo';
      setError(errorMessage);
      console.error('❌ Erro ao atualizar group type:', err);
      return null;
    }
  }, [executeUpdate]);

  /**
   * Exclui um tipo de grupo
   */
  const deleteGroupType = useCallback(async (id: string): Promise<boolean> => {
    try {
      setError(null);
      await executeDelete<void>(() => 
        GroupTypeService.deleteGroupType(id)
      );
      
      // Remove o item da lista
      setGroupTypes(prev => prev.filter(item => item.id !== id));
      return true;
    } catch (err: unknown) {
      const errorMessage = err instanceof Error ? err.message : 'Erro ao excluir tipo de grupo';
      setError(errorMessage);
      console.error('❌ Erro ao deletar group type:', err);
      return false;
    }
  }, [executeDelete]);

  /**
   * Carrega os dados na inicialização do hook
   */
  useEffect(() => {
    fetchGroupTypes();
  }, [fetchGroupTypes]);

  return {
    // Estado
    groupTypes,
    loading: fetchLoading,
    error,
    
    // Estados de carregamento específicos
    creating,
    updating,
    deleting,
    
    // Ações
    fetchGroupTypes,
    createGroupType,
    updateGroupType,
    deleteGroupType,
    clearError,
  };
};