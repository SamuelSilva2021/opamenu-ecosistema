import React, { useState, useEffect } from 'react';
import {
  Box,
  TextField,
  FormControlLabel,
  Switch,
  Button,
  Stack,
  CircularProgress,
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  OutlinedInput
} from '@mui/material';
import type { SelectChangeEvent } from '@mui/material/Select';
import type { Permission, CreatePermissionRequest, UpdatePermissionRequest, Module, Operation } from '../../../shared/types';
import { ModuleService, OperationService } from '../../../shared/services';

interface PermissionFormProps {
  permission?: Permission | null;
  onSubmit: (data: CreatePermissionRequest | UpdatePermissionRequest) => Promise<void>;
  onCancel: () => void;
  loading?: boolean;
}

/**
 * Formulário de Permissão
 * Componente para criar/editar permissões
 */
export const PermissionForm: React.FC<PermissionFormProps> = ({
  permission,
  onSubmit,
  onCancel,
  loading = false,
}) => {
  const [formData, setFormData] = useState({
    tenantId: '',
    roleId: '',
    moduleId: '',
    operationIds: [] as string[],
    isActive: true,
  });
  
  const [modules, setModules] = useState<Module[]>([]);
  const [operations, setOperations] = useState<Operation[]>([]);
  const [loadingData, setLoadingData] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isEdit = !!permission;

  // Carrega dados necessários
  useEffect(() => {
    const loadData = async () => {
      setLoadingData(true);
      try {
        const [modulesResponse, operationsResponse] = await Promise.all([
          ModuleService.getModules({ limit: 100, isActive: true }),
          OperationService.getOperations({ limit: 100, isActive: true })
        ]);
        
        setModules(modulesResponse.data);
        setOperations(operationsResponse.data);
      } catch (err) {
        setError('Erro ao carregar dados necessários');
        console.error('Erro ao carregar dados:', err);
      } finally {
        setLoadingData(false);
      }
    };

    loadData();
  }, []);

  // Preenche o formulário quando editing
  useEffect(() => {
    if (permission) {
      // Extrai os IDs das operações se existirem
      const operationIds: string[] = permission.operations 
        ? permission.operations.map((op: Operation) => op.id)
        : [];
      
      setFormData({
        tenantId: permission.tenantId || '',
        roleId: permission.roleId || '',
        moduleId: permission.moduleId || '',
        operationIds: operationIds,
        isActive: permission.isActive,
      });
    }
  }, [permission]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    // Validação básica
    if (!formData.moduleId) {
      setError('Módulo é obrigatório');
      return;
    }

    try {
      const submitData = isEdit 
        ? {
            tenantId: formData.tenantId || undefined,
            roleId: formData.roleId || undefined,
            moduleId: formData.moduleId,
            operationIds: formData.operationIds.length > 0 ? formData.operationIds : undefined,
            isActive: formData.isActive,
          } as UpdatePermissionRequest
        : {
            tenantId: formData.tenantId || undefined,
            roleId: formData.roleId || undefined,
            moduleId: formData.moduleId,
            operationIds: formData.operationIds.length > 0 ? formData.operationIds : undefined,
            isActive: formData.isActive,
          } as CreatePermissionRequest;

      await onSubmit(submitData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao salvar permissão');
    }
  };

  const handleOperationChange = (event: SelectChangeEvent<string[]>) => {
    const value = event.target.value;
    setFormData(prev => ({
      ...prev,
      operationIds: typeof value === 'string' ? value.split(',') : value,
    }));
  };

  if (loadingData) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight={200}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box component="form" onSubmit={handleSubmit} noValidate>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Stack spacing={3}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <TextField
            name="tenantId"
            label="ID do Tenant (Opcional)"
            fullWidth
            value={formData.tenantId}
            onChange={(e) => setFormData(prev => ({ ...prev, tenantId: e.target.value }))}
            disabled={loading}
            placeholder="Deixe vazio para permissão global"
          />

          <TextField
            name="roleId"
            label="ID do Papel (Opcional)"
            fullWidth
            value={formData.roleId}
            onChange={(e) => setFormData(prev => ({ ...prev, roleId: e.target.value }))}
            disabled={loading}
            placeholder="ID do papel que terá esta permissão"
          />
        </Stack>

        <FormControl fullWidth required>
          <InputLabel>Módulo</InputLabel>
          <Select
            value={formData.moduleId}
            onChange={(e) => setFormData(prev => ({ ...prev, moduleId: e.target.value }))}
            disabled={loading}
            label="Módulo"
            error={!formData.moduleId}
          >
            <MenuItem value="">
              <em>Selecione um módulo</em>
            </MenuItem>
            {modules.map((module) => (
              <MenuItem key={module.id} value={module.id}>
                {module.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <FormControl fullWidth>
          <InputLabel>Operações</InputLabel>
          <Select
            multiple
            value={formData.operationIds}
            onChange={handleOperationChange}
            input={<OutlinedInput label="Operações" />}
            disabled={loading}
            renderValue={(selected) => (
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                {selected.map((value) => {
                  const operation = operations.find(op => op.id === value);
                  return (
                    <Chip
                      key={value}
                      label={operation?.name || value}
                      size="small"
                    />
                  );
                })}
              </Box>
            )}
          >
            {operations.map((operation) => (
              <MenuItem key={operation.id} value={operation.id}>
                {operation.name} ({operation.value})
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <FormControlLabel
          control={
            <Switch
              checked={formData.isActive}
              onChange={(e) => setFormData(prev => ({ ...prev, isActive: e.target.checked }))}
              disabled={loading}
            />
          }
          label="Permissão Ativa"
        />

        <Box display="flex" gap={2} justifyContent="flex-end">
          <Button
            variant="outlined"
            onClick={onCancel}
            disabled={loading}
          >
            Cancelar
          </Button>
          <Button
            type="submit"
            variant="contained"
            disabled={loading || !formData.moduleId}
            startIcon={loading ? <CircularProgress size={20} /> : undefined}
          >
            {loading ? 'Salvando...' : isEdit ? 'Atualizar' : 'Criar'}
          </Button>
        </Box>
      </Stack>
    </Box>
  );
};