import { Box, Typography, Grid, Paper, Divider } from '@mui/material';
import {
  People as PeopleIcon,
  Business as BusinessIcon,
  Payment as PaymentIcon,
  Warning as WarningIcon,
  TrendingUp as TrendingUpIcon,
} from '@mui/icons-material';
import { ResponsiveContainer } from '../../shared/components';
import { SummaryCard } from '../../shared/components/data-display/SummaryCard';
import { useAuth } from '../../shared/hooks';

export const DashboardPage = () => {
  const { user } = useAuth();

  // Mock data para demonstração - Futuramente virá de um hook useMetrics
  const metrics = {
    totalTenants: 124,
    activeTenants: 110,
    overdueTenants: 8,
    mrr: 15450.00,
    newTenantsMonth: 12
  };

  return (
    <ResponsiveContainer>
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom sx={{ fontWeight: 700 }}>
          Painel de Controle OpaMenu
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Bem-vindo, {user?.fullName}. Veja o resumo do ecossistema hoje.
        </Typography>
      </Box>

      {/* Métricas Principais */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <SummaryCard
            title="Total de Clientes"
            value={metrics.totalTenants}
            icon={<BusinessIcon />}
            subtitle="Clientes cadastrados no sistema"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <SummaryCard
            title="Clientes Ativos"
            value={metrics.activeTenants}
            icon={<PeopleIcon />}
            color="#2e7d32"
            subtitle={`${((metrics.activeTenants / metrics.totalTenants) * 100).toFixed(1)}% da base total`}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <SummaryCard
            title="Planos Vencidos"
            value={metrics.overdueTenants}
            icon={<WarningIcon />}
            color="#d32f2f"
            subtitle="Ação necessária do suporte"
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <SummaryCard
            title="MRR Estimado"
            value={new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(metrics.mrr)}
            icon={<TrendingUpIcon />}
            color="#0288d1"
            subtitle="Receita recorrente mensal"
          />
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        {/* Lado Esquerdo - Últimas Atividades / Alertas */}
        <Grid item xs={12} md={8}>
          <Paper sx={{ p: 3, height: '100%' }}>
            <Typography variant="h6" gutterBottom>
              Clientes que precisam de Atenção
            </Typography>
            <Divider sx={{ my: 2 }} />
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
              {[1, 2, 3].map((i) => (
                <Box key={i} sx={{ p: 2, bgcolor: 'grey.50', borderRadius: 1, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <Box>
                    <Typography variant="subtitle2">Restaurante Sabor & Arte</Typography>
                    <Typography variant="caption" color="text.secondary">Plano Premium - Vencido há 3 dias</Typography>
                  </Box>
                  <Box sx={{ display: 'flex', gap: 1 }}>
                    <Typography variant="body2" sx={{ color: 'error.main', fontWeight: 600 }}>R$ 199,00</Typography>
                  </Box>
                </Box>
              ))}
            </Box>
          </Paper>
        </Grid>

        {/* Lado Direito - Atalhos Rápidos */}
        <Grid item xs={12} md={4}>
          <Paper sx={{ p: 3, height: '100%' }}>
            <Typography variant="h6" gutterBottom>
              Ações Rápidas
            </Typography>
            <Divider sx={{ my: 2 }} />
            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                Use os atalhos abaixo para gerenciar a estrutura:
              </Typography>
              <Typography variant="body2" sx={{ p: 1, '&:hover': { bgcolor: 'primary.50' }, borderRadius: 1, cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 1 }}>
                <PaymentIcon fontSize="small" color="primary" /> Configurar novos planos
              </Typography>
              <Typography variant="body2" sx={{ p: 1, '&:hover': { bgcolor: 'primary.50' }, borderRadius: 1, cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 1 }}>
                <BusinessIcon fontSize="small" color="primary" /> Validar novos cadastros
              </Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>
    </ResponsiveContainer>
  );
};
