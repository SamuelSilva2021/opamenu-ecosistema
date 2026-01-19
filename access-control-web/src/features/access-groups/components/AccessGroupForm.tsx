import { useState, useEffect } from 'react';
import {
  Box,
  TextField,
  FormControlLabel,
  Switch,
  Stack,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  FormHelperText,
} from '@mui/material';
import type { AccessGroup, CreateAccessGroupRequest, UpdateAccessGroupRequest, GroupType } from '../../../shared/types';

export interface AccessGroupFormData {
  name: string;
  description: string;
  tenantId?: string;
  code: string;
  groupTypeId: string;
  isActive: boolean;
  createdAt?: string;
}

export interface AccessGroupFormProps {
  initialData?: AccessGroup | null;
  groupTypes?: GroupType[];
  onSubmit: (data: CreateAccessGroupRequest | UpdateAccessGroupRequest) => void;
  isSubmitting?: boolean;
}

/**
 * Formulário reutilizável para criar/editar Access Groups
 * Validação client-side e UX otimizada
 */
export const AccessGroupForm = ({
  initialData,
  groupTypes = [],
  onSubmit,
  isSubmitting = false,
}: AccessGroupFormProps) => {
  const [formData, setFormData] = useState<AccessGroupFormData>({
    name: '',
    description: '',
    tenantId: '',
    code: '',
    groupTypeId: '',
    isActive: true,
    createdAt: undefined,
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Preenche formulário para edição
  useEffect(() => {
    if (initialData) {
      setFormData({
        name: initialData.name,
        description: initialData.description || '',
        code: initialData.code || '',
        groupTypeId: initialData.groupTypeId || '',
        isActive: initialData.isActive,
        createdAt: initialData.createdAt,
        tenantId: initialData.tenantId || '',
      });
    }
  }, [initialData]);

  // Validation helpers
  const validateField = (name: string, value: string) => {
    switch (name) {
      case 'name':
        if (!value.trim()) return 'Nome é obrigatório';
        if (value.length < 2) return 'Nome deve ter pelo menos 2 caracteres';
        if (value.length > 100) return 'Nome deve ter no máximo 100 caracteres';
        break;
      case 'code':
        if (value && value.length > 50) return 'Código deve ter no máximo 50 caracteres';
        if (value && !/^[A-Z0-9_-]*$/.test(value)) return 'Código deve conter apenas letras maiúsculas, números, underscore e hífen';
        break;
      case 'groupTypeId':
        if (!value) return 'Tipo de grupo é obrigatório';
        break;
      case 'description':
        if (!value.trim()) return 'Descrição é obrigatória';
        if (value.length > 500) return 'Descrição deve ter no máximo 500 caracteres';
        break;
    }
    return '';
  };

  const handleInputChange = (field: string, value: string | boolean) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));

    // Clear error when user starts typing
    if (errors[field] && value) {
      setErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }

    // Validate field if it's a string
    if (typeof value === 'string') {
      const error = validateField(field, value);
      if (error) {
        setErrors(prev => ({
          ...prev,
          [field]: error,
        }));
      }
    }
  };

  const handleSubmit = () => {
    const newErrors: Record<string, string> = {};
    
    newErrors.name = validateField('name', formData.name);
    newErrors.code = validateField('code', formData.code);
    newErrors.groupTypeId = validateField('groupTypeId', formData.groupTypeId);
    newErrors.description = validateField('description', formData.description);

    // Remove empty errors
    Object.keys(newErrors).forEach(key => {
      if (!newErrors[key]) delete newErrors[key];
    });

    setErrors(newErrors);

    // Submit if no errors
    if (Object.keys(newErrors).length === 0) {
      onSubmit({
        name: formData.name.trim(),
        description: formData.description.trim(),
        tenantId: formData.tenantId || undefined,
        code: formData.code.trim() || undefined,
        groupTypeId: formData.groupTypeId,
        isActive: formData.isActive,
        createdAt: formData.createdAt || undefined,
      });
    }
  };

  const activeGroupTypes = groupTypes.filter(gt => gt.isActive);

  return (
    <Box 
      component="form" 
      id="access-group-form"
      onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}
    >
      <Stack spacing={3}>
        {/* Nome */}
        <TextField
          fullWidth
          label="Nome"
          placeholder="Ex: Administradores do Sistema"
          value={formData.name}
          onChange={(e) => handleInputChange('name', e.target.value)}
          error={!!errors.name}
          helperText={errors.name || 'Nome descritivo do grupo de acesso'}
          disabled={isSubmitting}
          required
        />

        {/* Código */}
        <TextField
          fullWidth
          label="Código"
          placeholder="Ex: ADMIN_SYSTEM"
          value={formData.code}
          onChange={(e) => handleInputChange('code', e.target.value.toUpperCase())}
          error={!!errors.code}
          helperText={errors.code || 'Código único opcional (letras maiúsculas, números, _ e -)'}
          disabled={isSubmitting}
          inputProps={{
            style: { fontFamily: 'monospace', fontWeight: 600 }
          }}
        />

        {/* Tipo de Grupo */}
        <FormControl 
          fullWidth 
          error={!!errors.groupTypeId}
          disabled={isSubmitting}
          required
        >
          <InputLabel id="group-type-label">Tipo de Grupo</InputLabel>
          <Select
            labelId="group-type-label"
            value={formData.groupTypeId}
            label="Tipo de Grupo"
            onChange={(e) => handleInputChange('groupTypeId', e.target.value)}
          >
            {activeGroupTypes.length === 0 && (
              <MenuItem disabled>
                Nenhum tipo de grupo disponível
              </MenuItem>
            )}
            {activeGroupTypes.map((groupType) => (
              <MenuItem key={groupType.id} value={groupType.id}>
                <Box>
                  <Box sx={{ fontWeight: 600 }}>
                    {groupType.name}
                  </Box>
                  {groupType.description && (
                    <Box sx={{ fontSize: '0.875rem', color: 'text.secondary' }}>
                      {groupType.description}
                    </Box>
                  )}
                </Box>
              </MenuItem>
            ))}
          </Select>
          <FormHelperText>
            {errors.groupTypeId || 'Selecione o tipo de grupo para classificação'}
          </FormHelperText>
        </FormControl>

        <TextField
          fullWidth
          label="Tenant ID"
          placeholder="Ex: 123e4567-e89b-12d3-a456-426614174000"
          value={formData.tenantId}
          onChange={(e) => handleInputChange('tenantId', e.target.value)}
          error={!!errors.tenantId}
          helperText={errors.tenantId || 'ID do tenant (opcional)'}
          disabled={isSubmitting}
        />
        
        {/* Descrição */}
        <TextField
          fullWidth
          label="Descrição"
          placeholder="Descreva a finalidade deste grupo de acesso..."
          value={formData.description}
          onChange={(e) => handleInputChange('description', e.target.value)}
          error={!!errors.description}
          helperText={errors.description || 'Descrição obrigatória do grupo de acesso'}
          disabled={isSubmitting}
          multiline
          rows={3}
          required
        />

        {/* Status Ativo */}
        <FormControlLabel
          control={
            <Switch
              checked={formData.isActive}
              onChange={(e) => handleInputChange('isActive', e.target.checked)}
              disabled={isSubmitting}
              color="primary"
            />
          }
          label="Grupo de acesso ativo"
        />
      </Stack>
    </Box>
  );
};