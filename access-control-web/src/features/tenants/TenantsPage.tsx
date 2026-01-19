import { useState } from 'react';
import {
  Typography,
  Box,
  Paper,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
} from '@mui/material';
import { ResponsiveContainer } from '../../shared/components';
import { useTenants } from './hooks/useTenants';
import { TenantsList } from './components/TenantsList';
import { TenantsFilter } from './components/TenantsFilter';
import type { TenantSummary } from '../../shared/types';
import { TenantModules } from './components/TenantModules';
import { TenantEditDialog } from './components/TenantEditDialog';

export function TenantsPage() {
  const {
    tenants,
    loading,
    error,
    totalItems,
    currentPage,
    loadTenants,
    clearError,
    getTenantById,
    updateTenant,
    deleteTenant,
    filters,
    handleSearch,
  } = useTenants();

  const [pageSize, setPageSize] = useState(10);
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [tenantToDelete, setTenantToDelete] = useState<TenantSummary | null>(null);
  const [modulesOpen, setModulesOpen] = useState(false);
  const [tenantForModules, setTenantForModules] = useState<TenantSummary | null>(null);
  const [editOpen, setEditOpen] = useState(false);
  const [editingTenantId, setEditingTenantId] = useState<string | null>(null);

  const handlePageChange = (page: number) => {
    loadTenants(page);
  };

  const handlePageSizeChange = (size: number) => {
    setPageSize(size);
    loadTenants(1);
  };

  const handleEditTenant = (tenant: TenantSummary) => {
    setEditingTenantId(tenant.id);
    setEditOpen(true);
  };

  const handleManageModules = (tenant: TenantSummary) => {
    setTenantForModules(tenant);
    setModulesOpen(true);
  };

  const handleDeleteTenant = (tenant: TenantSummary) => {
    setTenantToDelete(tenant);
    setDeleteConfirmOpen(true);
  };

  const handleCloseModules = () => {
    setModulesOpen(false);
    setTenantForModules(null);
  };

  const handleConfirmDelete = async () => {
    if (!tenantToDelete) return;

    try {
      await deleteTenant(tenantToDelete.id);
    } finally {
      setDeleteConfirmOpen(false);
      setTenantToDelete(null);
    }
  };

  const handleCancelDelete = () => {
    setDeleteConfirmOpen(false);
    setTenantToDelete(null);
  };

  const handleCloseEdit = () => {
    setEditOpen(false);
    setEditingTenantId(null);
  };

  return (
    <ResponsiveContainer>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Tenants
        </Typography>
      </Box>

      <TenantsFilter
        onSearch={handleSearch}
        initialFilters={filters}
      />

      {error && (
        <Box sx={{ mb: 2 }}>
          <Alert severity="error" onClose={clearError}>
            {error}
          </Alert>
        </Box>
      )}

      <Paper sx={{ p: 2 }}>
        <TenantsList
          tenants={tenants}
          loading={loading}
          totalItems={totalItems}
          currentPage={currentPage}
          pageSize={pageSize}
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          onEdit={handleEditTenant}
                  onDelete={handleDeleteTenant}
                  onManageModules={handleManageModules}
        />
      </Paper>

      <Dialog open={deleteConfirmOpen} onClose={handleCancelDelete}>
        <DialogTitle>Remover tenant</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Tem certeza que deseja remover o tenant {tenantToDelete?.name}?
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCancelDelete}>Cancelar</Button>
          <Button onClick={handleConfirmDelete} color="error" variant="contained">
            Remover
          </Button>
        </DialogActions>
      </Dialog>

      <TenantEditDialog
        open={editOpen}
        tenantId={editingTenantId}
        onClose={handleCloseEdit}
        loadTenant={getTenantById}
        onSubmit={updateTenant}
      />

      <TenantModules
        open={modulesOpen}
        onClose={handleCloseModules}
        tenant={tenantForModules}
      />
    </ResponsiveContainer>
  );
}
