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
import { usePlans } from './hooks/usePlans';
import { PlansList } from './components/PlansList';
import type { SubscriptionPlan } from '../../shared/types';

export function PlansPage() {
  const {
    plans,
    loading,
    error,
    totalItems,
    currentPage,
    loadPlans,
    deletePlan,
    clearError
  } = usePlans();

  const [pageSize, setPageSize] = useState(10);
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [planToDelete, setPlanToDelete] = useState<SubscriptionPlan | null>(null);

  const handlePageChange = (page: number) => {
    loadPlans(page);
  };

  const handlePageSizeChange = (size: number) => {
    setPageSize(size);
    loadPlans(1);
  };

  const handleCreatePlan = () => {
    // TODO: Implement Create Dialog
    console.log('Create plan');
  };

  const handleEditPlan = (plan: SubscriptionPlan) => {
    // TODO: Implement Edit Dialog
    console.log('Edit plan', plan);
  };

  const handleDeletePlan = (plan: SubscriptionPlan) => {
    setPlanToDelete(plan);
    setDeleteConfirmOpen(true);
  };

  const handleConfirmDelete = async () => {
    if (!planToDelete) return;
    try {
      await deletePlan(planToDelete.id);
    } finally {
      setDeleteConfirmOpen(false);
      setPlanToDelete(null);
    }
  };

  return (
    <ResponsiveContainer>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Planos de Assinatura
        </Typography>
        <Button variant="contained" color="primary" onClick={handleCreatePlan}>
          Novo Plano
        </Button>
      </Box>

      {error && (
        <Box sx={{ mb: 2 }}>
          <Alert severity="error" onClose={clearError}>
            {error}
          </Alert>
        </Box>
      )}

      <Paper sx={{ p: 2 }}>
        <PlansList
          plans={plans}
          loading={loading}
          totalItems={totalItems}
          currentPage={currentPage}
          pageSize={pageSize}
          onPageChange={handlePageChange}
          onPageSizeChange={handlePageSizeChange}
          onEdit={handleEditPlan}
          onDelete={handleDeletePlan}
        />
      </Paper>

      <Dialog open={deleteConfirmOpen} onClose={() => setDeleteConfirmOpen(false)}>
        <DialogTitle>Remover plano</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Tem certeza que deseja remover o plano {planToDelete?.name}? Clientes usando este plano podem ser afetados.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteConfirmOpen(false)}>Cancelar</Button>
          <Button onClick={handleConfirmDelete} color="error" variant="contained">
            Remover
          </Button>
        </DialogActions>
      </Dialog>
    </ResponsiveContainer>
  );
}
