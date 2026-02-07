import { useState, useEffect, useCallback } from 'react';
import { RoleService } from '../../../shared/services';
import type { Role, CreateRoleRequest, UpdateRoleRequest } from '../../../shared/types';

interface UseRolesOptions {
  autoLoad?: boolean;
  pageSize?: number;
}

interface UseRolesResult {
  // Estado
  roles: Role[];
  loading: boolean;
  error: string | null;
  totalItems: number;
  currentPage: number;
  totalPages: number;

  // Ações CRUD
  loadRoles: (page?: number) => Promise<void>;
  createRole: (role: CreateRoleRequest) => Promise<Role>;
  updateRole: (id: string, role: UpdateRoleRequest) => Promise<Role>;
  deleteRole: (id: string) => Promise<void>;
  toggleStatus: (role: Role) => Promise<Role>;
  clearError: () => void;
  refetch: () => Promise<void>;
}

/**
 * Hook personalizado para gerenciar roles
 * Centraliza toda a lógica de estado e operações CRUD dos roles
 * 
 * Features:
 * - Carregamento automático opcional
 * - Paginação integrada
 * - Estados de loading e error
 * - Operações CRUD completas
 * - Toggle de status ativo/inativo
 * - Cache local dos dados
 * 
 * @param options - Configurações do hook
 */
export const useRoles = (options: UseRolesOptions = {}): UseRolesResult => {
  // Configurações padrão
  const { autoLoad = true, pageSize = 10 } = options;

  // Estados
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [totalItems, setTotalItems] = useState<number>(0);
  const [currentPage, setCurrentPage] = useState<number>(1);
  const [totalPages, setTotalPages] = useState<number>(0);

  /**
   * Limpa mensagens de erro
   */
  const clearError = useCallback(() => {
    setError(null);
  }, []);

  /**
   * Carrega roles com paginação
   */
  const loadRoles = useCallback(async (page: number = 1) => {
    setLoading(true);
    setError(null);

    try {
      const response = await RoleService.getRoles({
        page,
        limit: pageSize
      });

      setRoles(response.items || []);
      setCurrentPage(response.page || page);
      setTotalItems(response.total || 0);
      setTotalPages(response.totalPages || 0);

    } catch (err: any) {
      console.error('❌ useRoles: Erro ao carregar roles:', err);
      setError(err.message || 'Erro ao carregar roles');
      setRoles([]);
    } finally {
      setLoading(false);
    }
  }, [pageSize]);

  /**
   * Cria um novo role
   */
  const createRole = useCallback(async (roleData: CreateRoleRequest): Promise<Role> => {
    setLoading(true);
    setError(null);

    try {
      const newRole = await RoleService.createRole(roleData);

      // Recarrega a lista para refletir mudanças
      await loadRoles(currentPage);

      return newRole;

    } catch (err: any) {
      console.error('❌ useRoles: Erro ao criar role:', err);
      setError(err.message || 'Erro ao criar role');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [loadRoles, currentPage]);

  /**
   * Atualiza um role existente
   */
  const updateRole = useCallback(async (id: string, roleData: UpdateRoleRequest): Promise<Role> => {
    setLoading(true);
    setError(null);

    try {
      const updatedRole = await RoleService.updateRole(id, roleData);

      // Atualiza o role na lista local
      setRoles(prev => prev.map(role =>
        role.id === id ? updatedRole : role
      ));

      return updatedRole;

    } catch (err: any) {
      console.error('❌ useRoles: Erro ao atualizar role:', err);
      setError(err.message || 'Erro ao atualizar role');
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Remove um role
   */
  const deleteRole = useCallback(async (id: string): Promise<void> => {
    setLoading(true);
    setError(null);

    try {
      await RoleService.deleteRole(id);

      // Remove o role da lista local
      setRoles(prev => prev.filter(role => role.id !== id));

      // Se a página atual ficou vazia e não é a primeira, volta uma página
      const remainingItems = roles.length - 1;
      if (remainingItems === 0 && currentPage > 1) {
        await loadRoles(currentPage - 1);
      } else {
        // Atualiza o total de itens
        setTotalItems(prev => Math.max(0, prev - 1));
      }

    } catch (err: any) {
      console.error('❌ useRoles: Erro ao remover role:', err);
      setError(err.message || 'Erro ao remover role');
      throw err;
    } finally {
      setLoading(false);
    }
  }, [roles.length, currentPage, loadRoles]);

  /**
   * Alterna o status ativo/inativo de um role
   */
  const toggleStatus = useCallback(async (role: Role): Promise<Role> => {
    setLoading(true);
    setError(null);

    try {
      const updatedRole = await RoleService.toggleRoleStatus(role);

      // Atualiza o role na lista local
      setRoles(prev => prev.map(r =>
        r.id === role.id ? updatedRole : r
      ));

      return updatedRole;

    } catch (err: any) {
      console.error('❌ useRoles: Erro ao alterar status do role:', err);
      setError(err.message || 'Erro ao alterar status do role');
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * Recarrega os dados da página atual
   */
  const refetch = useCallback(async () => {
    await loadRoles(currentPage);
  }, [loadRoles, currentPage]);

  // Carregamento automático na inicialização
  useEffect(() => {
    if (autoLoad) {
      loadRoles(1);
    }
  }, [autoLoad, loadRoles]);

  return {
    // Estado
    roles,
    loading,
    error,
    totalItems,
    currentPage,
    totalPages,

    // Ações CRUD
    loadRoles,
    createRole,
    updateRole,
    deleteRole,
    toggleStatus,
    clearError,
    refetch,
  } as any; // Cast temporário para evitar quebra de interface externa enquanto refatoro outros arquivos
};
