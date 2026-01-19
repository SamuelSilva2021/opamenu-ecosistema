import { useEffect, useState, useMemo } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  IconButton,
  Box,
  TextField,
  Typography,
  Chip,
  CircularProgress,
  Alert,
  MenuItem,
} from '@mui/material';
import { Close as CloseIcon } from '@mui/icons-material';
import type { Tenant, Subscription } from '../../../shared/types';
import { SubscriptionService } from '../../../shared/services';

interface TenantEditDialogProps {
  open: boolean;
  tenantId: string | null;
  onClose: () => void;
  loadTenant: (id: string) => Promise<Tenant>;
  onSubmit: (id: string, data: Partial<Tenant>) => Promise<Tenant>;
}

type TenantFormValues = Partial<Tenant>;

export function TenantEditDialog({
  open,
  tenantId,
  onClose,
  loadTenant,
  onSubmit,
}: TenantEditDialogProps) {
  const [formValues, setFormValues] = useState<TenantFormValues>({});
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [subscription, setSubscription] = useState<Subscription | null>(null);
  const [subscriptionError, setSubscriptionError] = useState<string | null>(null);

  useEffect(() => {
    if (!open || !tenantId) {
      setFormValues({});
      setSubscription(null);
      setError(null);
      setSubscriptionError(null);
      setLoading(false);
      setSaving(false);
      return;
    }

    const loadData = async () => {
      setLoading(true);
      setError(null);
      setSubscriptionError(null);

      try {
        const tenant = await loadTenant(tenantId);
        setFormValues(tenant);
      } catch (err: unknown) {
        const message =
          typeof err === 'object' && err !== null && 'message' in err
            ? String((err as { message?: unknown }).message || 'Erro ao carregar tenant')
            : 'Erro ao carregar tenant';
        setError(message);
      } finally {
        setLoading(false);
      }

      try {
        const sub = await SubscriptionService.getByTenantId(tenantId);
        setSubscription(sub);
      } catch (err: unknown) {
        const message =
          typeof err === 'object' && err !== null && 'message' in err
            ? String((err as { message?: unknown }).message || 'Erro ao carregar assinatura')
            : 'Erro ao carregar assinatura';
        setSubscriptionError(message);
      }
    };

    void loadData();
  }, [open, tenantId, loadTenant]);

  const handleClose = () => {
    if (saving) return;
    onClose();
  };

  const handleChange = (field: keyof TenantFormValues) => (event: React.ChangeEvent<HTMLInputElement>) => {
    const { value } = event.target;
    setFormValues(prev => ({ ...prev, [field]: value }));
  };

  const handleSubmit = async () => {
    if (!tenantId) return;

    if (!formValues.name || !formValues.slug) {
      setError('Nome e slug são obrigatórios');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const data: Partial<Tenant> = {
        name: formValues.name,
        slug: formValues.slug,
        domain: formValues.domain,
        status: formValues.status,
        email: formValues.email,
        phone: formValues.phone,
        website: formValues.website,
        cnpjCpf: formValues.cnpjCpf,
        razaoSocial: formValues.razaoSocial,
        inscricaoEstadual: formValues.inscricaoEstadual,
        inscricaoMunicipal: formValues.inscricaoMunicipal,
        addressStreet: formValues.addressStreet,
        addressNumber: formValues.addressNumber,
        addressComplement: formValues.addressComplement,
        addressNeighborhood: formValues.addressNeighborhood,
        addressCity: formValues.addressCity,
        addressState: formValues.addressState,
        addressZipcode: formValues.addressZipcode,
        addressCountry: formValues.addressCountry,
        billingStreet: formValues.billingStreet,
        billingNumber: formValues.billingNumber,
        billingComplement: formValues.billingComplement,
        billingNeighborhood: formValues.billingNeighborhood,
        billingCity: formValues.billingCity,
        billingState: formValues.billingState,
        billingZipcode: formValues.billingZipcode,
        billingCountry: formValues.billingCountry,
        legalRepresentativeName: formValues.legalRepresentativeName,
        legalRepresentativeCpf: formValues.legalRepresentativeCpf,
        legalRepresentativeEmail: formValues.legalRepresentativeEmail,
        legalRepresentativePhone: formValues.legalRepresentativePhone,
      };

      await onSubmit(tenantId, data);
      onClose();
    } catch (err: unknown) {
      const message =
        typeof err === 'object' && err !== null && 'message' in err
          ? String((err as { message?: unknown }).message || 'Erro ao salvar tenant')
          : 'Erro ao salvar tenant';
      setError(message);
    } finally {
      setSaving(false);
    }
  };

  const statusOptions = useMemo(
    () => ['Ativo', 'Pendente', 'Suspenso', 'Cancelado'],
    [],
  );

  const subscriptionStatusColor = useMemo(() => {
    if (!subscription) return 'default' as const;
    if (subscription.status === 'Ativo' || subscription.status === 'Trial') return 'success' as const;
    if (subscription.status === 'Pendente') return 'warning' as const;
    return 'default' as const;
  }, [subscription]);

  const isPaymentOk = useMemo(() => {
    if (!subscription) return false;
    if (!formValues.status) return false;
    const tenantStatus = formValues.status;
    const subStatus = subscription.status;
    const subscriptionActive = subStatus === 'Ativo' || subStatus === 'Trial';
    const tenantActive = tenantStatus === 'Ativo';
    return tenantActive && subscriptionActive;
  }, [subscription, formValues.status]);

  const formatDate = (value?: string | null) => {
    if (!value) return '-';
    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return value;
    return date.toLocaleDateString('pt-BR');
  };

  const hasLoadedTenant = Boolean(formValues.id);

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="md"
      fullWidth
    >
      <DialogTitle>
        <Box display="flex" justifyContent="space-between" alignItems="center">
          Editar Tenant
          <IconButton
            edge="end"
            color="inherit"
            onClick={handleClose}
            aria-label="fechar"
            disabled={saving}
          >
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>

      <DialogContent dividers>
        {loading && !hasLoadedTenant ? (
          <Box display="flex" justifyContent="center" py={4}>
            <CircularProgress />
          </Box>
        ) : (
          <Box display="flex" flexDirection="column" gap={3}>
            {error && (
              <Alert severity="error">{error}</Alert>
            )}

            <Box display="flex" flexDirection="column" gap={1}>
              <Typography variant="subtitle2">Plano e pagamento</Typography>
              {subscription ? (
                <Box display="flex" flexWrap="wrap" gap={1}>
                  <Chip
                    label={subscription.plan?.name || 'Plano não identificado'}
                    color="primary"
                    variant="outlined"
                  />
                  <Chip
                    label={`Status: ${subscription.status}`}
                    color={subscriptionStatusColor}
                    variant="outlined"
                  />
                  <Chip
                    label={isPaymentOk ? 'Pagamento em dia' : 'Pagamento pendente ou assinatura inativa'}
                    color={isPaymentOk ? 'success' : 'warning'}
                    variant="outlined"
                  />
                  <Chip
                    label={`Período: ${formatDate(subscription.currentPeriodStart)} a ${formatDate(subscription.currentPeriodEnd)}`}
                    variant="outlined"
                  />
                </Box>
              ) : (
                <Typography variant="body2" color="text.secondary">
                  Nenhuma assinatura ativa para este tenant.
                </Typography>
              )}
              {subscriptionError && (
                <Typography variant="body2" color="error">
                  {subscriptionError}
                </Typography>
              )}
            </Box>

            <Box display="flex" flexDirection="column" gap={2}>
              <TextField
                label="Nome"
                fullWidth
                value={formValues.name || ''}
                onChange={handleChange('name')}
                required
              />
              <TextField
                label="Slug"
                fullWidth
                value={formValues.slug || ''}
                onChange={handleChange('slug')}
                required
              />

              <TextField
                label="Domínio"
                fullWidth
                value={formValues.domain || ''}
                onChange={handleChange('domain')}
              />
              <TextField
                select
                label="Status"
                fullWidth
                value={formValues.status || ''}
                onChange={handleChange('status')}
              >
                {statusOptions.map(option => (
                  <MenuItem key={option} value={option}>
                    {option}
                  </MenuItem>
                ))}
              </TextField>

              <TextField
                label="Email"
                type="email"
                fullWidth
                value={formValues.email || ''}
                onChange={handleChange('email')}
              />
              <TextField
                label="Telefone"
                fullWidth
                value={formValues.phone || ''}
                onChange={handleChange('phone')}
              />

              <TextField
                label="Website"
                fullWidth
                value={formValues.website || ''}
                onChange={handleChange('website')}
              />

              <TextField
                label="CNPJ/CPF"
                fullWidth
                value={formValues.cnpjCpf || ''}
                onChange={handleChange('cnpjCpf')}
              />
              <TextField
                label="Razão Social"
                fullWidth
                value={formValues.razaoSocial || ''}
                onChange={handleChange('razaoSocial')}
              />

              <TextField
                label="Inscrição Estadual"
                fullWidth
                value={formValues.inscricaoEstadual || ''}
                onChange={handleChange('inscricaoEstadual')}
              />
              <TextField
                label="Inscrição Municipal"
                fullWidth
                value={formValues.inscricaoMunicipal || ''}
                onChange={handleChange('inscricaoMunicipal')}
              />

              <Typography variant="subtitle2">Endereço</Typography>

              <TextField
                label="Logradouro"
                fullWidth
                value={formValues.addressStreet || ''}
                onChange={handleChange('addressStreet')}
              />
              <TextField
                label="Número"
                fullWidth
                value={formValues.addressNumber || ''}
                onChange={handleChange('addressNumber')}
              />

              <TextField
                label="Complemento"
                fullWidth
                value={formValues.addressComplement || ''}
                onChange={handleChange('addressComplement')}
              />
              <TextField
                label="Bairro"
                fullWidth
                value={formValues.addressNeighborhood || ''}
                onChange={handleChange('addressNeighborhood')}
              />

              <TextField
                label="Cidade"
                fullWidth
                value={formValues.addressCity || ''}
                onChange={handleChange('addressCity')}
              />
              <TextField
                label="Estado"
                fullWidth
                value={formValues.addressState || ''}
                onChange={handleChange('addressState')}
              />
              <TextField
                label="CEP"
                fullWidth
                value={formValues.addressZipcode || ''}
                onChange={handleChange('addressZipcode')}
              />

              <TextField
                label="País"
                fullWidth
                value={formValues.addressCountry || ''}
                onChange={handleChange('addressCountry')}
              />

              <Typography variant="subtitle2">Endereço de cobrança</Typography>

              <TextField
                label="Logradouro (cobrança)"
                fullWidth
                value={formValues.billingStreet || ''}
                onChange={handleChange('billingStreet')}
              />
              <TextField
                label="Número (cobrança)"
                fullWidth
                value={formValues.billingNumber || ''}
                onChange={handleChange('billingNumber')}
              />

              <TextField
                label="Complemento (cobrança)"
                fullWidth
                value={formValues.billingComplement || ''}
                onChange={handleChange('billingComplement')}
              />
              <TextField
                label="Bairro (cobrança)"
                fullWidth
                value={formValues.billingNeighborhood || ''}
                onChange={handleChange('billingNeighborhood')}
              />

              <TextField
                label="Cidade (cobrança)"
                fullWidth
                value={formValues.billingCity || ''}
                onChange={handleChange('billingCity')}
              />
              <TextField
                label="Estado (cobrança)"
                fullWidth
                value={formValues.billingState || ''}
                onChange={handleChange('billingState')}
              />
              <TextField
                label="CEP (cobrança)"
                fullWidth
                value={formValues.billingZipcode || ''}
                onChange={handleChange('billingZipcode')}
              />

              <TextField
                label="País (cobrança)"
                fullWidth
                value={formValues.billingCountry || ''}
                onChange={handleChange('billingCountry')}
              />

              <Typography variant="subtitle2">Representante legal</Typography>

              <TextField
                label="Nome do representante"
                fullWidth
                value={formValues.legalRepresentativeName || ''}
                onChange={handleChange('legalRepresentativeName')}
              />
              <TextField
                label="CPF do representante"
                fullWidth
                value={formValues.legalRepresentativeCpf || ''}
                onChange={handleChange('legalRepresentativeCpf')}
              />

              <TextField
                label="Email do representante"
                type="email"
                fullWidth
                value={formValues.legalRepresentativeEmail || ''}
                onChange={handleChange('legalRepresentativeEmail')}
              />
              <TextField
                label="Telefone do representante"
                fullWidth
                value={formValues.legalRepresentativePhone || ''}
                onChange={handleChange('legalRepresentativePhone')}
              />
            </Box>
          </Box>
        )}
      </DialogContent>

      <DialogActions sx={{ p: 2 }}>
        <Button
          onClick={handleClose}
          color="inherit"
          disabled={saving}
        >
          Cancelar
        </Button>
        <Button
          onClick={handleSubmit}
          variant="contained"
          disabled={saving || loading || !hasLoadedTenant}
        >
          {saving ? 'Salvando...' : 'Salvar'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
