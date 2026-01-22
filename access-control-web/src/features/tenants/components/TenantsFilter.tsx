import React from 'react';
import { useForm } from 'react-hook-form';
import { Box, TextField, Button } from '@mui/material';
import Grid from '@mui/material/Grid';
import { Search as SearchIcon, Clear as ClearIcon } from '@mui/icons-material';
import type { TenantFilters } from '../../../shared/types';

interface TenantsFilterProps {
  onSearch: (filters: TenantFilters) => void;
  initialFilters?: TenantFilters;
}

export const TenantsFilter: React.FC<TenantsFilterProps> = ({ onSearch, initialFilters }) => {
  const { register, handleSubmit, reset } = useForm<TenantFilters>({
    defaultValues: initialFilters || {
      name: '',
      email: '',
      phone: '',
      domain: '',
      slug: '',
      status: '',
    },
  });

  const onSubmit = (data: TenantFilters) => {
    // Limpa campos vazios antes de enviar
    const cleanData = Object.fromEntries(
      Object.entries(data).filter(([_, v]) => v !== '' && v !== undefined)
    ) as TenantFilters;
    onSearch(cleanData);
  };

  const handleClear = () => {
    reset({
      name: '',
      email: '',
      phone: '',
      domain: '',
      slug: '',
      status: '',
    });
    onSearch({});
  };

  return (
    <Box component="form" onSubmit={handleSubmit(onSubmit)} sx={{ mb: 3, p: 2, bgcolor: 'background.paper', borderRadius: 1 }}>
      <Grid container spacing={2} alignItems="center">
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <TextField
            label="Nome"
            fullWidth
            size="small"
            {...register('name')}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <TextField
            label="Email"
            fullWidth
            size="small"
            {...register('email')}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <TextField
            label="Telefone"
            fullWidth
            size="small"
            {...register('phone')}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <TextField
            label="Domínio"
            fullWidth
            size="small"
            {...register('domain')}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <TextField
            label="Slug"
            fullWidth
            size="small"
            {...register('slug')}
          />
        </Grid>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <TextField
            select
            label="Status"
            fullWidth
            size="small"
            defaultValue=""
            InputLabelProps={{ shrink: true }}
            slotProps={{
                select: { native: true }
            }}
            {...register('status')}
          >
            <option value="">Todos</option>
            <option value="Ativo">Ativo</option>
            <option value="Pendente">Pendente</option>
            <option value="Suspenso">Suspenso</option>
            <option value="Inativo">Inativo</option>
            <option value="Cancelado">Cancelado</option>
          </TextField>
        </Grid>
        <Grid size={{ xs: 12, sm: 12, md: 6 }} sx={{ display: 'flex', gap: 1 }}>
          <Button
            variant="contained"
            startIcon={<SearchIcon />}
            type="submit"
            fullWidth
          >
            Filtrar
          </Button>
          <Button
            variant="outlined"
            startIcon={<ClearIcon />}
            onClick={handleClear}
            fullWidth
          >
            Limpar
          </Button>
        </Grid>
      </Grid>
    </Box>
  );
};
