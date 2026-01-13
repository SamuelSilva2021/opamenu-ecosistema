import { useState } from 'react';
import {
  PageHeader,
  ResponsiveContainer,
  StyledCard,
} from '../../shared/components';
import { Add as AddIcon, Groups as GroupsIcon } from '@mui/icons-material';
import { Typography, Box, CircularProgress, Alert } from '@mui/material';
import { useAccessGroups } from '../../shared/hooks';
import { useGroupTypes } from '../group-types/hooks';
import { useAccessGroupOperations } from '../../shared/hooks/operations';
import { AccessGroupsList, AccessGroupDialog } from './components';
import type { AccessGroup, CreateAccessGroupRequest, UpdateAccessGroupRequest } from '../../shared/types';


export const AccessGroupsPage = () => {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingAccessGroup, setEditingAccessGroup] = useState<AccessGroup | null>(null);

  const { canRead, canCreate, canUpdate, canDelete } = useAccessGroupOperations();

  const {
    data: accessGroups = [],
    isLoading: loading,
    error,
    totalCount,
    currentPage,
    pageSize,
    createAccessGroup,
    updateAccessGroup,
    deleteAccessGroup,
    loadAccessGroups,
    refreshData,
    clearError,
    setPageSize,
  } = useAccessGroups();

  const {
    groupTypes = [],
  } = useGroupTypes();

  if (!canRead) {
    return (
      <ResponsiveContainer>
        <Box sx={{ textAlign: 'center', py: 8 }}>
          <GroupsIcon sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
          <Typography variant="h6" color="text.secondary" gutterBottom>
            Acesso Negado
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Você não tem permissão para visualizar grupos de acesso.
          </Typography>
        </Box>
      </ResponsiveContainer>
    );
  }

  console.log('🔄 AccessGroupsPage renderizou com:', {
    accessGroupsCount: accessGroups.length,
    loading,
    error: !!error
  });

  const handleCreateAccessGroup = () => {
    if (!canCreate) {
      alert('Você não tem permissão para criar grupos de acesso.');
      return;
    }
    setEditingAccessGroup(null);
    setDialogOpen(true);
  };

  const handleEditAccessGroup = (accessGroup: AccessGroup) => {
    if (!canUpdate) {
      alert('Você não tem permissão para editar grupos de acesso.');
      return;
    }
    setEditingAccessGroup(accessGroup);
    setDialogOpen(true);
  };

  const handleDeleteAccessGroup = async (accessGroup: AccessGroup) => {
    if (!canDelete) {
      alert('Você não tem permissão para excluir grupos de acesso.');
      return;
    }
    if (window.confirm(`Tem certeza que deseja excluir o grupo "${accessGroup.name}"?`)) {
      await deleteAccessGroup(accessGroup.id);
      await refreshData();
    }
  };

  const handleToggleStatus = async (accessGroup: AccessGroup) => {
    console.log(`🔄 Toggle status: ${accessGroup.name}`);
  };

  const handlePageChange = async (page: number) => {
    await loadAccessGroups(page);
  };

  const handlePageSizeChange = async (newPageSize: number) => {
    setPageSize(newPageSize);
    await loadAccessGroups(1);
  };

  const handleDialogSubmit = async (data: CreateAccessGroupRequest | UpdateAccessGroupRequest) => {
    try {
      if (editingAccessGroup) {
        const result = await updateAccessGroup(editingAccessGroup.id, data as UpdateAccessGroupRequest);
        console.log('✏️ Update result:', result);
      } else {
        const result = await createAccessGroup(data as CreateAccessGroupRequest);
        console.log('➕ Create result:', result);
      }
      
      setDialogOpen(false);
      setEditingAccessGroup(null);
      clearError();
      
      await refreshData();
    } catch (error) {
      console.error('❌ Erro na operação:', error);
    }
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
    setEditingAccessGroup(null);
    clearError();
  };

  return (
    <ResponsiveContainer>
      <PageHeader
        title="Grupos de Acesso"
        subtitle="Gerencie os grupos de acesso e suas permissões"
        icon={<GroupsIcon />}
        actionButton={canCreate ? {
          label: 'Criar Grupo',
          onClick: handleCreateAccessGroup,
          icon: <AddIcon />
        } : undefined}
      />

      <StyledCard>
        {error && (
          <Alert 
            severity="error" 
            onClose={clearError}
            sx={{ mb: 2 }}
          >
            {typeof error === 'string' ? error : 'Erro ao carregar grupos de acesso'}
          </Alert>
        )}

        {loading && (!accessGroups || accessGroups.length === 0) ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
            <CircularProgress />
          </Box>
        ) : (!accessGroups || accessGroups.length === 0) ? (
          <Box sx={{ textAlign: 'center', py: 8 }}>
            <GroupsIcon sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />
            <Typography variant="h6" color="text.secondary" gutterBottom>
              Nenhum grupo de acesso encontrado
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Clique em "Criar Grupo" para adicionar seu primeiro grupo de acesso
            </Typography>
          </Box>
        ) : (
          <AccessGroupsList
            accessGroups={accessGroups}
            loading={loading}
            onEdit={handleEditAccessGroup}
            onDelete={handleDeleteAccessGroup}
            onToggleStatus={handleToggleStatus}
            totalCount={totalCount}
            currentPage={currentPage}
            pageSize={pageSize}
            onPageChange={handlePageChange}
            onPageSizeChange={handlePageSizeChange}
          />
        )}
      </StyledCard>

      <AccessGroupDialog
        open={dialogOpen}
        accessGroup={editingAccessGroup}
        groupTypes={groupTypes}
        onClose={handleDialogClose}
        onSubmit={handleDialogSubmit}
        isSubmitting={loading}
      />
    </ResponsiveContainer>
  );
};