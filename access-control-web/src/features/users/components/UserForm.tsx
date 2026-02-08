import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Box,
  Typography,
  Divider,
  Alert,
  InputAdornment,
  IconButton,
  Stack
} from '@mui/material';
import {
  Visibility,
  VisibilityOff,
  Person as PersonIcon,
  Email as EmailIcon,
  Phone as PhoneIcon,
  Lock as LockIcon,
  Business as BusinessIcon,
  Security as SecurityIcon
} from '@mui/icons-material';
import { type UserAccount, type CreateUserAccountRequest, type UpdateUserAccountRequest, UserAccountStatus, type Role } from '../../../shared/types';
import { RoleService } from '../../../shared/services';

interface UserFormProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: CreateUserAccountRequest | UpdateUserAccountRequest) => Promise<void>;
  user?: UserAccount | null;
  loading?: boolean;
  onValidateEmail?: (email: string, excludeUserId?: string) => Promise<boolean>;
}

interface FormData {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  tenantId: string;
  roleId: string;
  status: UserAccountStatus;
  createdAt?: string;
}

interface FormErrors {
  username?: string;
  email?: string;
  password?: string;
  confirmPassword?: string;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  tenantId?: string;
  roleId?: string;
  general?: string;
  createdAt?: string;
}

const USER_STATUS_OPTIONS = [
  { value: UserAccountStatus.Active, label: 'Ativo' },
  { value: UserAccountStatus.Inactive, label: 'Inativo' },
  { value: UserAccountStatus.Pending, label: 'Pendente' },
  { value: UserAccountStatus.Suspended, label: 'Suspenso' },
];

export function UserForm({
  open,
  onClose,
  onSubmit,
  user = null,
  loading = false,
  onValidateEmail
}: UserFormProps) {

  const [formData, setFormData] = useState<FormData>({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    phoneNumber: '',
    tenantId: '',
    roleId: '',
    status: UserAccountStatus.Active,
    createdAt: undefined
  });

  const [roles, setRoles] = useState<Role[]>([]);
  const [loadingRoles, setLoadingRoles] = useState(false);

  const [errors, setErrors] = useState<FormErrors>({});
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [validating, setValidating] = useState(false);

  const isEditMode = Boolean(user);

  useEffect(() => {
    if (open) {
      if (user) {
        setFormData({
          username: user.username || '',
          email: user.email || '',
          password: '',
          confirmPassword: '',
          firstName: user.firstName || '',
          lastName: user.lastName || '',
          phoneNumber: user.phoneNumber || '',
          tenantId: user.tenantId || '',
          roleId: user.roleId || '',
          status: user.status || UserAccountStatus.Active,
          createdAt: user.createdAt || undefined
        });
      } else {
        setFormData({
          username: '',
          email: '',
          password: '',
          confirmPassword: '',
          firstName: '',
          lastName: '',
          phoneNumber: '',
          tenantId: '',
          roleId: '',
          status: UserAccountStatus.Active,
          createdAt: undefined
        });
      }
      setErrors({});
      loadRoles();
    }
  }, [open, user]);

  const loadRoles = async () => {
    setLoadingRoles(true);
    try {
      const data = await RoleService.getAllRoles();
      setRoles(data);
    } catch (error) {
      console.error('Erro ao carregar roles:', error);
    } finally {
      setLoadingRoles(false);
    }
  };

  const handleFieldChange = (field: keyof FormData, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));

    if (errors[field as keyof FormErrors]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const validateField = async (field: keyof FormData, value: string): Promise<string | undefined> => {
    switch (field) {
      case 'username':
        if (!value.trim()) return 'Username é obrigatório';
        if (value.length > 100) return 'Username não pode exceder 100 caracteres';
        break;

      case 'email':
        if (!value.trim()) return 'Email é obrigatório';
        if (value.length > 255) return 'Email não pode exceder 255 caracteres';
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) return 'Formato de email inválido';

        if (onValidateEmail) {
          const isUnique = await onValidateEmail(value, user?.id);
          if (!isUnique) return 'Este email já está em uso';
        }
        break;

      case 'password':
        if (!isEditMode && !value) return 'Senha é obrigatória';
        if (value && value.length < 6) return 'Senha deve ter pelo menos 6 caracteres';
        if (value && value.length > 100) return 'Senha não pode exceder 100 caracteres';
        break;

      case 'confirmPassword':
        if (!isEditMode && !value) return 'Confirmação de senha é obrigatória';
        if (value && value !== formData.password) return 'Senhas não coincidem';
        break;

      case 'firstName':
        if (!value.trim()) return 'Nome é obrigatório';
        if (value.length > 100) return 'Nome não pode exceder 100 caracteres';
        break;

      case 'lastName':
        if (!value.trim()) return 'Sobrenome é obrigatório';
        if (value.length > 100) return 'Sobrenome não pode exceder 100 caracteres';
        break;

      case 'phoneNumber':
        if (value && value.length > 20) return 'Telefone não pode exceder 20 caracteres';
        if (value && !/^[\d\s()+-]+$/.test(value)) return 'Formato de telefone inválido';
        break;

      case 'tenantId':
        // TenantId é opcional, mas se preenchido deve ser um GUID válido
        if (value && !/^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value)) {
          return 'Formato de Tenant ID inválido (deve ser um GUID)';
        }
        break;
    }

    return undefined;
  };

  const validateForm = async (): Promise<boolean> => {
    setValidating(true);
    const newErrors: FormErrors = {};

    for (const [field, value] of Object.entries(formData)) {
      if (field === 'status') continue;

      const error = await validateField(field as keyof FormData, value);
      if (error) {
        newErrors[field as keyof FormErrors] = error;
      }
    }

    setErrors(newErrors);
    setValidating(false);

    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async () => {
    const isValid = await validateForm();
    if (!isValid) return;

    try {
      if (isEditMode) {
        const updateData: UpdateUserAccountRequest = {
          username: formData.username,
          email: formData.email,
          firstName: formData.firstName,
          lastName: formData.lastName,
          phoneNumber: formData.phoneNumber || undefined,
          status: formData.status,
          isEmailVerified: user?.isEmailVerified,
          tenantId: formData.tenantId || undefined,
          roleId: formData.roleId || undefined
        };

        await onSubmit(updateData);
      } else {
        const createData: CreateUserAccountRequest = {
          email: formData.email,
          password: formData.password,
          confirmPassword: formData.confirmPassword,
          firstName: formData.firstName,
          lastName: formData.lastName,
          phoneNumber: formData.phoneNumber || undefined,
          tenantId: formData.tenantId?.trim() || undefined,
          roleId: formData.roleId || undefined
        };

        await onSubmit(createData);
      }

      onClose();
    } catch (error: any) {
      setErrors(prev => ({
        ...prev,
        general: error.message || 'Erro ao salvar usuário'
      }));
    }
  };

  const handleBlur = async (field: keyof FormData) => {
    const error = await validateField(field, formData[field] || '');

    if (error) {
      setErrors(prev => ({ ...prev, [field]: error }));
    }
  };

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="md"
      fullWidth
      PaperProps={{
        sx: { minHeight: '600px' }
      }}
    >
      <DialogTitle>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <PersonIcon />
          <Typography variant="h6">
            {isEditMode ? 'Editar Usuário' : 'Novo Usuário'}
          </Typography>
        </Box>
      </DialogTitle>

      <DialogContent dividers>
        {errors.general && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {errors.general}
          </Alert>
        )}

        <Stack spacing={3}>
          <Box>
            <Typography variant="subtitle1" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <PersonIcon fontSize="small" />
              Informações Básicas
            </Typography>
            <Divider sx={{ mb: 2 }} />
          </Box>

          <TextField
            fullWidth
            label="Username *"
            value={formData.username}
            onChange={(e) => handleFieldChange('username', e.target.value)}
            onBlur={() => handleBlur('username')}
            error={Boolean(errors.username)}
            helperText={errors.username}
            disabled={loading || validating}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <PersonIcon />
                </InputAdornment>
              )
            }}
          />

          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField
              fullWidth
              label="Nome *"
              value={formData.firstName}
              onChange={(e) => handleFieldChange('firstName', e.target.value)}
              onBlur={() => handleBlur('firstName')}
              error={Boolean(errors.firstName)}
              helperText={errors.firstName}
              disabled={loading || validating}
            />
            <TextField
              fullWidth
              label="Sobrenome *"
              value={formData.lastName}
              onChange={(e) => handleFieldChange('lastName', e.target.value)}
              onBlur={() => handleBlur('lastName')}
              error={Boolean(errors.lastName)}
              helperText={errors.lastName}
              disabled={loading || validating}
            />
          </Stack>

          <TextField
            fullWidth
            label="Email *"
            type="email"
            value={formData.email}
            onChange={(e) => handleFieldChange('email', e.target.value)}
            onBlur={() => handleBlur('email')}
            error={Boolean(errors.email)}
            helperText={errors.email || 'Email será usado como base para gerar o username automaticamente'}
            disabled={loading || validating}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <EmailIcon />
                </InputAdornment>
              )
            }}
          />

          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <TextField
              fullWidth
              label="Telefone"
              value={formData.phoneNumber}
              onChange={(e) => handleFieldChange('phoneNumber', e.target.value)}
              onBlur={() => handleBlur('phoneNumber')}
              error={Boolean(errors.phoneNumber)}
              helperText={errors.phoneNumber}
              disabled={loading || validating}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <PhoneIcon />
                  </InputAdornment>
                )
              }}
            />
            <TextField
              fullWidth
              label="Tenant ID (Cliente)"
              value={formData.tenantId}
              onChange={(e) => handleFieldChange('tenantId', e.target.value)}
              onBlur={() => handleBlur('tenantId')}
              error={Boolean(errors.tenantId)}
              helperText={errors.tenantId || 'ID do cliente/organização (opcional)'}
              disabled={loading || validating}
              placeholder="00000000-0000-0000-0000-000000000000"
              inputProps={{ maxLength: 36 }}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <BusinessIcon />
                  </InputAdornment>
                )
              }}
            />
          </Stack>

          <FormControl fullWidth>
            <InputLabel>Papel (Role)</InputLabel>
            <Select
              value={formData.roleId}
              label="Papel (Role)"
              onChange={(e) => handleFieldChange('roleId', e.target.value)}
              disabled={loading || validating || loadingRoles}
              sx={{
                '& .MuiSelect-select': {
                  display: 'flex',
                  alignItems: 'center',
                  gap: 1
                }
              }}
              startAdornment={
                <InputAdornment position="start" sx={{ mr: 1 }}>
                  <SecurityIcon />
                </InputAdornment>
              }
            >
              <MenuItem value="">
                <em>Nenhum</em>
              </MenuItem>
              {roles.map((role) => (
                <MenuItem key={role.id} value={role.id}>
                  {role.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          {isEditMode && (
            <FormControl fullWidth>
              <InputLabel>Status</InputLabel>
              <Select
                value={formData.status}
                label="Status"
                onChange={(e) => handleFieldChange('status', e.target.value as UserAccountStatus)}
                disabled={loading || validating}
              >
                {USER_STATUS_OPTIONS.map((option) => (
                  <MenuItem key={option.value} value={option.value}>
                    {option.label}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          )}

          {!isEditMode && (
            <>
              <Box sx={{ mt: 2 }}>
                <Typography variant="subtitle1" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <LockIcon fontSize="small" />
                  Credenciais de Acesso
                </Typography>
                <Divider sx={{ mb: 2 }} />
              </Box>

              <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
                <TextField
                  fullWidth
                  label="Senha *"
                  type={showPassword ? 'text' : 'password'}
                  value={formData.password}
                  onChange={(e) => handleFieldChange('password', e.target.value)}
                  onBlur={() => handleBlur('password')}
                  error={Boolean(errors.password)}
                  helperText={errors.password || 'Mínimo de 6 caracteres'}
                  disabled={loading || validating}
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          onClick={() => setShowPassword(!showPassword)}
                          edge="end"
                        >
                          {showPassword ? <VisibilityOff /> : <Visibility />}
                        </IconButton>
                      </InputAdornment>
                    )
                  }}
                />
                <TextField
                  fullWidth
                  label="Confirmar Senha *"
                  type={showConfirmPassword ? 'text' : 'password'}
                  value={formData.confirmPassword}
                  onChange={(e) => handleFieldChange('confirmPassword', e.target.value)}
                  onBlur={() => handleBlur('confirmPassword')}
                  error={Boolean(errors.confirmPassword)}
                  helperText={errors.confirmPassword}
                  disabled={loading || validating}
                  InputProps={{
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                          edge="end"
                        >
                          {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                        </IconButton>
                      </InputAdornment>
                    )
                  }}
                />
              </Stack>
            </>
          )}
        </Stack>
      </DialogContent>

      <DialogActions sx={{ p: 2, gap: 1 }}>
        <Button
          onClick={onClose}
          disabled={loading || validating}
        >
          Cancelar
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={loading || validating}
        >
          {loading || validating ? 'Salvando...' : (isEditMode ? 'Atualizar' : 'Criar')}
        </Button>
      </DialogActions>
    </Dialog>
  );
}