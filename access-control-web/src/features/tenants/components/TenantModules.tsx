import { useState, useEffect, useCallback } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Box,
  Typography,
  Checkbox,
  FormControlLabel,
  CircularProgress,
  Alert,
  Stack,
  Chip,
  IconButton,
} from '@mui/material';
import {
  Apps as AppsIcon,
  Close as CloseIcon,
} from '@mui/icons-material';
import type { TenantSummary, Module, Permission } from '../../../shared/types';
import { ModuleService, PermissionService } from '../../../shared/services';

export interface TenantModulesProps {
  open: boolean;
  tenant: TenantSummary | null;
  onClose: () => void;
}

interface TenantModuleState {
  modules: Module[];
  permissions: Permission[];
}

export const TenantModules = ({ open, tenant, onClose }: TenantModulesProps) => {
  const [state, setState] = useState<TenantModuleState>({ modules: [], permissions: [] });
  const [selectedModuleIds, setSelectedModuleIds] = useState<string[]>([]);
  const [initialModuleIds, setInitialModuleIds] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadData = useCallback(async () => {
    if (!tenant || !open) {
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const [modulesResponse, tenantPermissions] = await Promise.all([
        ModuleService.getModules({ page: 1, limit: 100 }),
        PermissionService.getPermissionsByTenant(tenant.id),
      ]);

      const modules = modulesResponse.data.filter(module => module.isActive);

      const activeModuleIds = tenantPermissions
        .filter(permission => permission.isActive && permission.moduleId)
        .map(permission => permission.moduleId as string);

      setState({ modules, permissions: tenantPermissions });
      setSelectedModuleIds(activeModuleIds);
      setInitialModuleIds(activeModuleIds);
    } catch (err: unknown) {
      const message =
        typeof err === 'object' && err !== null && 'message' in err
          ? String((err as { message?: unknown }).message || 'Erro ao carregar módulos')
          : 'Erro ao carregar módulos';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [tenant, open]);

  useEffect(() => {
    if (open && tenant) {
      loadData();
    }
  }, [open, tenant, loadData]);

  const handleToggleModule = (moduleId: string) => {
    setSelectedModuleIds(prev =>
      prev.includes(moduleId)
        ? prev.filter(id => id !== moduleId)
        : [...prev, moduleId]
    );
  };

  const handleClose = () => {
    if (saving) return;
    onClose();
  };

  const handleSave = async () => {
    if (!tenant) return;

    setSaving(true);
    setError(null);

    try {
      const toAdd = selectedModuleIds.filter(id => !initialModuleIds.includes(id));
      const toRemove = initialModuleIds.filter(id => !selectedModuleIds.includes(id));

      for (const moduleId of toAdd) {
        await PermissionService.createPermission({
          tenantId: tenant.id,
          moduleId,
          isActive: true,
        });
      }

      if (toRemove.length > 0) {
        const permissionsToRemove = state.permissions.filter(
          permission =>
            permission.moduleId &&
            toRemove.includes(permission.moduleId) &&
            permission.tenantId === tenant.id &&
            !permission.roleId,
        );

        for (const permission of permissionsToRemove) {
          await PermissionService.deletePermission(permission.id);
        }
      }

      setInitialModuleIds(selectedModuleIds);
      onClose();
    } catch (err: unknown) {
      const message =
        typeof err === 'object' && err !== null && 'message' in err
          ? String((err as { message?: unknown }).message || 'Erro ao salvar módulos do tenant')
          : 'Erro ao salvar módulos do tenant';
      setError(message);
    } finally {
      setSaving(false);
    }
  };

  const selectedModules = state.modules.filter(module => selectedModuleIds.includes(module.id));

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="md"
      fullWidth
      PaperProps={{ sx: { minHeight: 480 } }}
    >
      <DialogTitle>
        <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <AppsIcon />
            <Box>
              <Typography variant="h6">Módulos do Tenant</Typography>
              {tenant && (
                <Typography variant="body2" color="text.secondary">
                  {tenant.name} ({tenant.slug})
                </Typography>
              )}
            </Box>
          </Box>
          <IconButton onClick={handleClose} disabled={saving}>
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>

      <DialogContent dividers>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
            <CircularProgress />
          </Box>
        ) : (
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={3} alignItems="flex-start">
            <Box sx={{ flex: 1 }}>
              <Typography variant="subtitle1" gutterBottom>
                Módulos disponíveis
              </Typography>
              <Stack spacing={1}>
                {state.modules.map(module => (
                  <FormControlLabel
                    key={module.id}
                    control={
                      <Checkbox
                        checked={selectedModuleIds.includes(module.id)}
                        onChange={() => handleToggleModule(module.id)}
                      />
                    }
                    label={
                      <Box>
                        <Typography variant="body2" sx={{ fontWeight: 500 }}>
                          {module.name}
                        </Typography>
                        {module.description && (
                          <Typography variant="caption" color="text.secondary">
                            {module.description}
                          </Typography>
                        )}
                      </Box>
                    }
                  />
                ))}
                {state.modules.length === 0 && (
                  <Typography variant="body2" color="text.secondary">
                    Nenhum módulo cadastrado.
                  </Typography>
                )}
              </Stack>
            </Box>

            <Box sx={{ flex: 1 }}>
              <Typography variant="subtitle1" gutterBottom>
                Módulos vinculados
              </Typography>
              {selectedModules.length === 0 ? (
                <Typography variant="body2" color="text.secondary">
                  Nenhum módulo vinculado a este tenant.
                </Typography>
              ) : (
                <Stack direction="row" flexWrap="wrap" gap={1}>
                  {selectedModules.map(module => (
                    <Chip key={module.id} label={module.name} color="primary" variant="outlined" />
                  ))}
                </Stack>
              )}
            </Box>
          </Stack>
        )}
      </DialogContent>

      <DialogActions>
        <Button onClick={handleClose} disabled={saving}>
          Cancelar
        </Button>
        <Button onClick={handleSave} color="primary" variant="contained" disabled={saving || loading}>
          {saving ? 'Salvando...' : 'Salvar'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

