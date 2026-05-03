import { useState, useEffect, useMemo } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Grid,
  MenuItem,
  FormControlLabel,
  Checkbox,
  Box,
  Typography,
  Divider,
  Stack,
  Chip,
  CircularProgress,
  Alert,
  IconButton,
  Paper,
  Tabs,
  Tab,
  InputAdornment,
  Tooltip,
  Fade,
} from '@mui/material';
import {
  Close as CloseIcon,
  Business as BusinessIcon,
  Payments as PaymentsIcon,
  Apps as AppsIcon,
  Description as DescriptionIcon,
  Settings as SettingsIcon,
  Search as SearchIcon,
  Info as InfoIcon,
  LocalOffer as LocalOfferIcon,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import type { SubscriptionPlan } from '../../../shared/types';
import { useModules } from '../../modules/hooks/useModules';

const planSchema = z.object({
  name: z.string().min(1, 'Nome é obrigatório'),
  slug: z.string().min(1, 'Slug é obrigatório'),
  description: z.string().optional(),
  price: z.preprocess((val) => Number(val), z.number().min(0, 'Preço deve ser positivo')),
  billingCycle: z.string().min(1, 'Ciclo de cobrança é obrigatório'),
  status: z.string().min(1, 'Status é obrigatório'),
  isTrial: z.boolean().default(false),
  trialPeriodDays: z.preprocess((val) => Number(val || 0), z.number().min(0)),
  sortOrder: z.preprocess((val) => Number(val || 0), z.number()),
});

type PlanFormData = z.infer<typeof planSchema>;

interface PlanFormDialogProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: any) => Promise<void>;
  plan?: SubscriptionPlan | null;
  loading?: boolean;
}

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`plan-tabpanel-${index}`}
      aria-labelledby={`plan-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ pt: 3 }}>{children}</Box>}
    </div>
  );
}

export function PlanFormDialog({ open, onClose, onSubmit, plan, loading = false }: PlanFormDialogProps) {
  const { modules, loading: loadingModules } = useModules({ autoLoad: true, pageSize: 100 });
  const [selectedModuleIds, setSelectedModuleIds] = useState<string[]>([]);
  const [tabValue, setTabValue] = useState(0);
  const [moduleSearch, setModuleSearch] = useState('');

  const {
    register,
    handleSubmit,
    control,
    reset,
    formState: { errors },
  } = useForm<PlanFormData>({
    resolver: zodResolver(planSchema),
    defaultValues: {
      name: '',
      slug: '',
      description: '',
      price: 0,
      billingCycle: 'Monthly',
      status: 'Ativo',
      isTrial: false,
      trialPeriodDays: 0,
      sortOrder: 0,
    },
  });

  useEffect(() => {
    if (plan && open) {
      reset({
        name: plan.name,
        slug: plan.slug,
        description: plan.description || '',
        price: plan.price,
        billingCycle: plan.billingCycle || 'Monthly',
        status: plan.status || 'Ativo',
        isTrial: plan.isTrial || false,
        trialPeriodDays: plan.trialPeriodDays || 0,
        sortOrder: plan.sortOrder || 0,
      });

      try {
        if (plan.features) {
          const features = JSON.parse(plan.features);
          if (Array.isArray(features.moduleIds)) {
            setSelectedModuleIds(features.moduleIds);
          }
        } else {
          setSelectedModuleIds([]);
        }
      } catch (e) {
        setSelectedModuleIds([]);
      }
    } else if (open) {
      reset({
        name: '',
        slug: '',
        description: '',
        price: 0,
        billingCycle: 'Monthly',
        status: 'Ativo',
        isTrial: false,
        trialPeriodDays: 0,
        sortOrder: 0,
      });
      setSelectedModuleIds([]);
      setTabValue(0);
    }
  }, [plan, reset, open]);

  const filteredModules = useMemo(() => {
    return modules.filter(m => 
      m.name.toLowerCase().includes(moduleSearch.toLowerCase()) || 
      m.key?.toLowerCase().includes(moduleSearch.toLowerCase())
    );
  }, [modules, moduleSearch]);

  const handleFormSubmit = async (data: PlanFormData) => {
    const finalData = {
      ...data,
      features: JSON.stringify({ moduleIds: selectedModuleIds }),
    };
    await onSubmit(finalData);
  };

  const toggleModule = (moduleId: string) => {
    setSelectedModuleIds((prev) =>
      prev.includes(moduleId) ? prev.filter((id) => id !== moduleId) : [...prev, moduleId]
    );
  };

  const handleTabChange = (_: React.SyntheticEvent, newValue: number) => {
    setTabValue(newValue);
  };

  return (
    <Dialog 
      open={open} 
      onClose={onClose} 
      maxWidth="lg" 
      fullWidth
      TransitionComponent={Fade}
      transitionDuration={400}
      PaperProps={{
        sx: {
          borderRadius: 3,
          boxShadow: '0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1)',
          minHeight: 600
        }
      }}
    >
      <DialogTitle sx={{ 
        p: 0, 
        bgcolor: 'primary.main', 
        color: 'primary.contrastText',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        pr: 2
      }}>
        <Box sx={{ p: 3, display: 'flex', alignItems: 'center', gap: 2 }}>
          <Box sx={{ 
            bgcolor: 'rgba(255,255,255,0.2)', 
            p: 1, 
            borderRadius: 2,
            display: 'flex'
          }}>
            <LocalOfferIcon />
          </Box>
          <Box>
            <Typography variant="h6" sx={{ fontWeight: 700 }}>
              {plan ? 'Configurar Plano' : 'Criar Novo Plano'}
            </Typography>
            <Typography variant="caption" sx={{ opacity: 0.8, display: 'block' }}>
              {plan ? `Editando: ${plan.name}` : 'Defina as regras e recursos do novo pacote'}
            </Typography>
          </Box>
        </Box>
        <IconButton onClick={onClose} sx={{ color: 'inherit' }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>

      <Box component="form" onSubmit={handleSubmit(handleFormSubmit)} sx={{ display: 'flex', flexDirection: 'column' }}>
        <DialogContent sx={{ p: 0 }}>
          <Box sx={{ borderBottom: 1, borderColor: 'divider', px: 3, bgcolor: 'grey.50' }}>
            <Tabs value={tabValue} onChange={handleTabChange} aria-label="plan tabs">
              <Tab icon={<DescriptionIcon sx={{ fontSize: 20 }} />} iconPosition="start" label="Informações" />
              <Tab icon={<PaymentsIcon sx={{ fontSize: 20 }} />} iconPosition="start" label="Faturamento" />
              <Tab 
                icon={<AppsIcon sx={{ fontSize: 20 }} />} 
                iconPosition="start" 
                label={
                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    Recursos
                    <Chip size="small" label={selectedModuleIds.length} sx={{ height: 20, fontSize: '0.65rem' }} />
                  </Box>
                } 
              />
            </Tabs>
          </Box>

          <Box sx={{ px: 4, pb: 4 }}>
            {/* TAB 1: INFORMAÇÕES BÁSICAS */}
            <TabPanel value={tabValue} index={0}>
              <Grid container spacing={4}>
                <Grid item xs={12} md={6}>
                  <Paper variant="outlined" sx={{ p: 4, borderRadius: 2, bgcolor: 'grey.50', height: '100%' }}>
                    <Typography variant="h6" sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1, fontWeight: 600 }}>
                      <DescriptionIcon color="primary" /> Identidade do Plano
                    </Typography>
                    <Stack spacing={3}>
                      <TextField
                        {...register('name')}
                        label="Nome Comercial do Plano"
                        fullWidth
                        error={!!errors.name}
                        helperText={errors.name?.message}
                        placeholder="Ex: Plano Premium"
                        InputProps={{
                          startAdornment: (
                            <InputAdornment position="start">
                              <LocalOfferIcon fontSize="small" color="primary" />
                            </InputAdornment>
                          ),
                          sx: { fontWeight: 600 }
                        }}
                      />
                      <TextField
                        {...register('slug')}
                        label="Slug (Identificador Técnico)"
                        fullWidth
                        error={!!errors.slug}
                        helperText={errors.slug ? errors.slug.message : "Usado internamente e em URLs"}
                        placeholder="plano-premium"
                        disabled={!!plan}
                      />
                      <TextField
                        {...register('sortOrder')}
                        label="Prioridade na Vitrine"
                        type="number"
                        fullWidth
                        helperText="Quanto menor o número, mais à esquerda ele aparecerá para o cliente"
                      />
                    </Stack>
                  </Paper>
                </Grid>
                <Grid item xs={12} md={6}>
                  <Paper variant="outlined" sx={{ p: 4, borderRadius: 2, height: '100%' }}>
                    <Typography variant="h6" sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1, fontWeight: 600 }}>
                      <InfoIcon color="primary" /> Detalhamento
                    </Typography>
                    <Stack spacing={3}>
                      <TextField
                        {...register('description')}
                        label="Descrição do Plano"
                        fullWidth
                        multiline
                        rows={8}
                        placeholder="Descreva as principais vantagens e o público-alvo deste plano..."
                        helperText="Este texto pode ser exibido no site de vendas"
                      />
                    </Stack>
                  </Paper>
                </Grid>
              </Grid>
            </TabPanel>

            {/* TAB 2: FATURAMENTO E STATUS */}
            <TabPanel value={tabValue} index={1}>
              <Grid container spacing={4}>
                <Grid item xs={12} md={6}>
                  <Paper variant="outlined" sx={{ p: 4, borderRadius: 2, bgcolor: 'grey.50', height: '100%' }}>
                    <Typography variant="h6" sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1, fontWeight: 600 }}>
                      <PaymentsIcon color="primary" /> Estrutura de Preços
                    </Typography>
                    <Stack spacing={3}>
                      <TextField
                        {...register('price')}
                        label="Valor da Mensalidade"
                        type="number"
                        fullWidth
                        required
                        variant="outlined"
                        InputProps={{
                          startAdornment: <InputAdornment position="start">R$</InputAdornment>,
                          sx: { fontSize: '1.2rem', fontWeight: 600 }
                        }}
                      />
                      <Controller
                        name="billingCycle"
                        control={control}
                        render={({ field }) => (
                          <TextField {...field} select label="Ciclo de Cobrança" fullWidth variant="outlined">
                            <MenuItem value="Monthly">Mensal</MenuItem>
                            <MenuItem value="Yearly">Anual</MenuItem>
                            <MenuItem value="Weekly">Semanal</MenuItem>
                            <MenuItem value="Daily">Diário</MenuItem>
                          </TextField>
                        )}
                      />
                    </Stack>
                  </Paper>
                </Grid>

                <Grid item xs={12} md={6}>
                  <Stack spacing={3} sx={{ height: '100%' }}>
                    <Paper variant="outlined" sx={{ p: 3, borderRadius: 2 }}>
                      <Typography variant="subtitle2" sx={{ mb: 2, fontWeight: 600 }}>Status do Plano</Typography>
                      <Controller
                        name="status"
                        control={control}
                        render={({ field }) => (
                          <TextField {...field} select fullWidth>
                            <MenuItem value="Ativo">🟢 Ativo (Público no Site)</MenuItem>
                            <MenuItem value="Inativo">🟡 Inativo (Somente Admin)</MenuItem>
                            <MenuItem value="Arquivado">🔴 Arquivado</MenuItem>
                          </TextField>
                        )}
                      />
                    </Paper>

                    <Paper variant="outlined" sx={{ p: 3, borderRadius: 2, flex: 1 }}>
                      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
                        <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>Período de Teste (Trial)</Typography>
                        <Controller
                          name="isTrial"
                          control={control}
                          render={({ field }) => (
                            <Checkbox {...field} checked={field.value} />
                          )}
                        />
                      </Box>
                      <TextField
                        {...register('trialPeriodDays')}
                        label="Quantidade de Dias de Cortesia"
                        type="number"
                        fullWidth
                        placeholder="Ex: 30"
                        disabled={!control._formValues.isTrial}
                        helperText="O restaurante poderá usar o sistema gratuitamente por este período"
                      />
                    </Paper>
                  </Stack>
                </Grid>
              </Grid>
            </TabPanel>

            {/* TAB 3: MÓDULOS / RECURSOS */}
            <TabPanel value={tabValue} index={2}>
              <Box sx={{ mb: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <Box>
                  <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>Seleção de Recursos</Typography>
                  <Typography variant="caption" color="text.secondary">Habilite os módulos que estarão disponíveis para este plano</Typography>
                </Box>
                <TextField
                  placeholder="Buscar módulo..."
                  size="small"
                  value={moduleSearch}
                  onChange={(e) => setModuleSearch(e.target.value)}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <SearchIcon fontSize="small" />
                      </InputAdornment>
                    ),
                  }}
                  sx={{ width: 250 }}
                />
              </Box>

              {loadingModules ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
                  <CircularProgress size={40} thickness={4} />
                </Box>
              ) : (
                <Grid container spacing={2} sx={{ maxHeight: 500, overflowY: 'auto', p: 1 }}>
                  {filteredModules.map((module) => {
                    const isSelected = selectedModuleIds.includes(module.id);
                    return (
                      <Grid item xs={12} sm={6} md={4} key={module.id}>
                        <Paper
                          variant="outlined"
                          onClick={() => toggleModule(module.id)}
                          sx={{
                            p: 2,
                            cursor: 'pointer',
                            transition: 'all 0.2s',
                            position: 'relative',
                            borderWidth: 2,
                            borderColor: isSelected ? 'primary.main' : 'divider',
                            bgcolor: isSelected ? 'primary.50' : 'background.paper',
                            '&:hover': {
                              borderColor: 'primary.light',
                              transform: 'translateY(-2px)',
                              boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
                            }
                          }}
                        >
                          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
                            <Box sx={{ 
                              p: 0.5, 
                              bgcolor: isSelected ? 'primary.main' : 'grey.200', 
                              borderRadius: 1, 
                              color: isSelected ? 'white' : 'text.secondary',
                              display: 'flex'
                            }}>
                              <SettingsIcon sx={{ fontSize: 18 }} />
                            </Box>
                            <Checkbox 
                              checked={isSelected} 
                              size="small" 
                              sx={{ p: 0 }}
                            />
                          </Box>
                          <Typography variant="body2" sx={{ fontWeight: 600, color: isSelected ? 'primary.dark' : 'text.primary' }}>
                            {module.name}
                          </Typography>
                          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 0.5 }}>
                            Code: {module.key}
                          </Typography>
                        </Paper>
                      </Grid>
                    );
                  })}
                  {filteredModules.length === 0 && (
                    <Grid item xs={12}>
                      <Box sx={{ textAlign: 'center', py: 4 }}>
                        <Typography color="text.secondary">Nenhum módulo encontrado para "{moduleSearch}"</Typography>
                      </Box>
                    </Grid>
                  )}
                </Grid>
              )}
            </TabPanel>
          </Box>
        </DialogContent>

        <DialogActions sx={{ p: 3, bgcolor: 'grey.50', borderTop: 1, borderColor: 'divider' }}>
          <Button 
            onClick={onClose} 
            disabled={loading}
            variant="outlined"
            color="inherit"
          >
            Cancelar
          </Button>
          <Box sx={{ flex: 1 }} />
          <Button 
            type="submit" 
            variant="contained" 
            disabled={loading}
            size="large"
            startIcon={loading ? <CircularProgress size={20} /> : null}
            sx={{ px: 4 }}
          >
            {loading ? 'Salvando...' : plan ? 'Salvar Alterações' : 'Criar Plano'}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
