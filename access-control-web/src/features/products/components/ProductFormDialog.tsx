import { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Box,
  MenuItem,
  Alert,
  Grid,
  Typography,
  IconButton,
  Paper,
  Stack,
  InputAdornment,
  Fade,
  CircularProgress,
} from '@mui/material';
import {
  Close as CloseIcon,
  Inventory as InventoryIcon,
  Description as DescriptionIcon,
  Settings as SettingsIcon,
  Label as LabelIcon,
  PriceCheck as PriceIcon,
  Build as BuildIcon,
} from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { ProductService } from '../../../shared/services';
import type { TenantProduct } from '../../../shared/types';

const productSchema = z.object({
  name: z.string().min(1, 'Nome é obrigatório'),
  slug: z.string().min(1, 'Slug é obrigatório'),
  description: z.string().optional(),
  category: z.string().min(1, 'Categoria é obrigatória'),
  version: z.string().min(1, 'Versão é obrigatória'),
  status: z.string().min(1, 'Status é obrigatório'),
  pricingModel: z.string().min(1, 'Modelo de precificação é obrigatório'),
  basePrice: z.preprocess((val) => Number(val), z.number().min(0, 'Preço deve ser positivo')),
  setupFee: z.preprocess((val) => Number(val || 0), z.number().min(0)),
});

type ProductFormData = z.infer<typeof productSchema>;

interface ProductFormDialogProps {
  open: boolean;
  product?: TenantProduct;
  onClose: () => void;
  onSuccess: () => void;
}

export function ProductFormDialog({ open, product, onClose, onSuccess }: ProductFormDialogProps) {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    control,
    reset,
    formState: { errors },
  } = useForm<ProductFormData>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      name: '',
      slug: '',
      description: '',
      category: 'WebApp',
      version: '1.0.0',
      status: 'Ativo',
      pricingModel: 'Assinatura',
      basePrice: 0,
      setupFee: 0,
    },
  });

  useEffect(() => {
    if (product && open) {
      reset({
        name: product.name,
        slug: product.slug,
        description: product.description || '',
        category: product.category,
        version: product.version,
        status: product.status,
        pricingModel: product.pricingModel,
        basePrice: product.basePrice,
        setupFee: product.setupFee,
      });
    } else if (open) {
      reset({
        name: '',
        slug: '',
        description: '',
        category: 'WebApp',
        version: '1.0.0',
        status: 'Ativo',
        pricingModel: 'Assinatura',
        basePrice: 0,
        setupFee: 0,
      });
      setError(null);
    }
  }, [product, reset, open]);

  const handleFormSubmit = async (data: ProductFormData) => {
    setLoading(true);
    setError(null);

    try {
      if (product) {
        await ProductService.updateProduct(product.id, data);
      } else {
        await ProductService.createProduct(data);
      }
      onSuccess();
    } catch (err: any) {
      setError(err.message || 'Erro ao salvar produto');
    } finally {
      setLoading(false);
    }
  };

  const categories = ['WebApp', 'MobileApp', 'ApiService', 'DesktopApp', 'Plugin', 'Other'];
  const statuses = ['Ativo', 'Inativo', 'Descontinuado'];

  return (
    <Dialog 
      open={open} 
      onClose={onClose} 
      maxWidth="md" 
      fullWidth
      TransitionComponent={Fade}
      transitionDuration={400}
      PaperProps={{
        sx: {
          borderRadius: 3,
          boxShadow: '0 20px 25px -5px rgb(0 0 0 / 0.1), 0 8px 10px -6px rgb(0 0 0 / 0.1)',
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
            <InventoryIcon />
          </Box>
          <Box>
            <Typography variant="h6" sx={{ fontWeight: 700 }}>
              {product ? 'Editar Produto' : 'Novo Produto'}
            </Typography>
            <Typography variant="caption" sx={{ opacity: 0.8, display: 'block' }}>
              {product ? `Editando: ${product.name}` : 'Cadastre um novo software no ecossistema'}
            </Typography>
          </Box>
        </Box>
        <IconButton onClick={onClose} sx={{ color: 'inherit' }}>
          <CloseIcon />
        </IconButton>
      </DialogTitle>

      <Box component="form" onSubmit={handleSubmit(handleFormSubmit)}>
        <DialogContent sx={{ p: 4 }}>
          {error && <Alert severity="error" sx={{ mb: 3 }}>{error}</Alert>}

          <Grid container spacing={4}>
            {/* Coluna 1: Identificação */}
            <Grid item xs={12} md={6}>
              <Paper variant="outlined" sx={{ p: 3, borderRadius: 2, bgcolor: 'grey.50', height: '100%' }}>
                <Typography variant="subtitle1" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1, fontWeight: 600 }}>
                  <LabelIcon color="primary" fontSize="small" /> Identificação
                </Typography>
                <Stack spacing={2.5}>
                  <TextField
                    {...register('name')}
                    label="Nome do Produto"
                    fullWidth
                    required
                    error={!!errors.name}
                    helperText={errors.name?.message}
                    placeholder="Ex: OpaMenu POS"
                  />
                  <TextField
                    {...register('slug')}
                    label="Slug (Identificador)"
                    fullWidth
                    required
                    disabled={!!product}
                    error={!!errors.slug}
                    helperText={errors.slug ? errors.slug.message : "Identificador único usado internamente"}
                    placeholder="opamenu-pos"
                  />
                  <Controller
                    name="category"
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} select label="Categoria" fullWidth>
                        {categories.map((cat) => (
                          <MenuItem key={cat} value={cat}>{cat}</MenuItem>
                        ))}
                      </TextField>
                    )}
                  />
                </Stack>
              </Paper>
            </Grid>

            {/* Coluna 2: Detalhes e Versão */}
            <Grid item xs={12} md={6}>
              <Paper variant="outlined" sx={{ p: 3, borderRadius: 2, height: '100%' }}>
                <Typography variant="subtitle1" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1, fontWeight: 600 }}>
                  <BuildIcon color="primary" fontSize="small" /> Configurações
                </Typography>
                <Stack spacing={2.5}>
                  <TextField
                    {...register('version')}
                    label="Versão Atual"
                    fullWidth
                    placeholder="1.0.0"
                    InputProps={{
                      startAdornment: <InputAdornment position="start">v</InputAdornment>
                    }}
                  />
                  <Controller
                    name="status"
                    control={control}
                    render={({ field }) => (
                      <TextField {...field} select label="Status" fullWidth>
                        {statuses.map((st) => (
                          <MenuItem key={st} value={st}>{st}</MenuItem>
                        ))}
                      </TextField>
                    )}
                  />
                  <TextField
                    {...register('description')}
                    label="Descrição Curta"
                    fullWidth
                    multiline
                    rows={2}
                    placeholder="Breve descrição do propósito deste produto..."
                  />
                </Stack>
              </Paper>
            </Grid>

            {/* Linha 2: Precificação */}
            <Grid item xs={12}>
              <Paper variant="outlined" sx={{ p: 3, borderRadius: 2, bgcolor: 'primary.50', border: '1px dashed', borderColor: 'primary.main' }}>
                <Typography variant="subtitle1" sx={{ mb: 2, display: 'flex', alignItems: 'center', gap: 1, fontWeight: 600 }}>
                  <PriceIcon color="primary" fontSize="small" /> Modelo Comercial
                </Typography>
                <Grid container spacing={3}>
                  <Grid item xs={12} md={4}>
                    <TextField
                      {...register('basePrice')}
                      label="Preço Base (Mensal)"
                      type="number"
                      fullWidth
                      InputProps={{
                        startAdornment: <InputAdornment position="start">R$</InputAdornment>
                      }}
                    />
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <TextField
                      {...register('setupFee')}
                      label="Taxa de Implantação (Setup)"
                      type="number"
                      fullWidth
                      InputProps={{
                        startAdornment: <InputAdornment position="start">R$</InputAdornment>
                      }}
                    />
                  </Grid>
                  <Grid item xs={12} md={4}>
                    <Controller
                      name="pricingModel"
                      control={control}
                      render={({ field }) => (
                        <TextField {...field} select label="Modelo de Cobrança" fullWidth>
                          <MenuItem value="Assinatura">Assinatura</MenuItem>
                          <MenuItem value="Freemium">Freemium</MenuItem>
                          <MenuItem value="OneTime">Pagamento Único</MenuItem>
                        </TextField>
                      )}
                    />
                  </Grid>
                </Grid>
              </Paper>
            </Grid>
          </Grid>
        </DialogContent>

        <DialogActions sx={{ p: 3, bgcolor: 'grey.50', borderTop: 1, borderColor: 'divider' }}>
          <Button onClick={onClose} color="inherit" disabled={loading}>
            Cancelar
          </Button>
          <Box sx={{ flex: 1 }} />
          <Button 
            type="submit" 
            variant="contained" 
            disabled={loading}
            size="large"
            startIcon={loading ? <CircularProgress size={20} color="inherit" /> : null}
            sx={{ px: 4, borderRadius: 2 }}
          >
            {loading ? 'Salvando...' : product ? 'Salvar Alterações' : 'Cadastrar Produto'}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
