import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  Checkbox,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  Security as SecurityIcon,
  Save as SaveIcon,
  Close as CloseIcon
} from '@mui/icons-material';
import { RoleService, ModuleService } from '../../../shared/services';
import type { Role, Module, SimplifiedPermission } from '../../../shared/types';

interface RolePermissionsProps {
  open: boolean;
  onClose: () => void;
  role: Role | null;
  onSuccess?: () => void;
}

const ACTIONS = ['READ', 'CREATE', 'UPDATE', 'DELETE'];

export const RolePermissions: React.FC<RolePermissionsProps> = ({
  open,
  onClose,
  role,
  onSuccess
}) => {
  const [modules, setModules] = useState<Module[]>([]);
  const [selectedPermissions, setSelectedPermissions] = useState<SimplifiedPermission[]>([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open && role) {
      loadData();
    }
  }, [open, role]);

  const loadData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [modulesRes, roleRes] = await Promise.all([
        ModuleService.getModules({ page: 1, limit: 100 }),
        RoleService.getRoleById(role!.id)
      ]);
      setModules(modulesRes.data);
      setSelectedPermissions(roleRes.permissions || []);
    } catch (err: any) {
      console.error('Erro ao carregar dados:', err);
      setError('Falha ao carregar módulos ou permissões do role.');
    } finally {
      setLoading(false);
    }
  };

  const isActionSelected = (moduleKey: string, action: string) => {
    const perm = selectedPermissions.find(p => p.module === moduleKey);
    return perm?.actions.includes(action) || false;
  };

  const handleToggleAction = (moduleKey: string, action: string) => {
    setSelectedPermissions(prev => {
      const existing = prev.find(p => p.module === moduleKey);
      if (existing) {
        const newActions = existing.actions.includes(action)
          ? existing.actions.filter(a => a !== action)
          : [...existing.actions, action];

        if (newActions.length === 0) {
          return prev.filter(p => p.module !== moduleKey);
        }

        return prev.map(p => p.module === moduleKey ? { ...p, actions: newActions } : p);
      } else {
        return [...prev, { module: moduleKey, actions: [action] }];
      }
    });
  };

  const handleToggleModuleAll = (moduleKey: string, checked: boolean) => {
    setSelectedPermissions(prev => {
      const otherPerms = prev.filter(p => p.module !== moduleKey);
      if (checked) {
        return [...otherPerms, { module: moduleKey, actions: [...ACTIONS] }];
      }
      return otherPerms;
    });
  };

  const handleSave = async () => {
    if (!role) return;
    setSaving(true);
    setError(null);
    try {
      await RoleService.updateRole(role.id, {
        permissions: selectedPermissions
      });
      if (onSuccess) onSuccess();
      onClose();
    } catch (err: any) {
      console.error('Erro ao salvar permissões:', err);
      setError('Falha ao salvar as permissões do role.');
    } finally {
      setSaving(false);
    }
  };

  if (!role) return null;

  return (
    <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
      <DialogTitle>
        <Box display="flex" alignItems="center" gap={1}>
          <SecurityIcon color="primary" />
          <Typography variant="h6">Permissões: {role.name}</Typography>
        </Box>
      </DialogTitle>
      <DialogContent dividers>
        {loading ? (
          <Box py={10} textAlign="center"><CircularProgress /></Box>
        ) : error ? (
          <Alert severity="error">{error}</Alert>
        ) : (
          <TableContainer component={Paper} variant="outlined">
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Módulo</TableCell>
                  {ACTIONS.map(action => (
                    <TableCell key={action} align="center">{action}</TableCell>
                  ))}
                  <TableCell align="center">Todos</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {modules.map(module => {
                  const moduleKey = module.key || '';
                  const allSelected = ACTIONS.every(a => isActionSelected(moduleKey, a));
                  const someSelected = ACTIONS.some(a => isActionSelected(moduleKey, a)) && !allSelected;

                  return (
                    <TableRow key={module.id}>
                      <TableCell>
                        <Typography variant="subtitle2">{module.name}</Typography>
                        <Typography variant="caption" color="textSecondary">{moduleKey}</Typography>
                      </TableCell>
                      {ACTIONS.map(action => (
                        <TableCell key={action} align="center">
                          <Checkbox
                            size="small"
                            checked={isActionSelected(moduleKey, action)}
                            onChange={() => handleToggleAction(moduleKey, action)}
                          />
                        </TableCell>
                      ))}
                      <TableCell align="center">
                        <Checkbox
                          size="small"
                          indeterminate={someSelected}
                          checked={allSelected}
                          onChange={(e) => handleToggleModuleAll(moduleKey, e.target.checked)}
                        />
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} startIcon={<CloseIcon />}>Cancelar</Button>
        <Button
          onClick={handleSave}
          variant="contained"
          disabled={loading || saving}
          startIcon={saving ? <CircularProgress size={16} /> : <SaveIcon />}
        >
          {saving ? 'Salvando...' : 'Salvar'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
