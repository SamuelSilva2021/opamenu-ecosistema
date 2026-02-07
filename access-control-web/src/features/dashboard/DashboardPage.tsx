import { Box, Typography, Card, CardContent, Button, Grid } from '@mui/material';
import {
  LogoutOutlined,
} from '@mui/icons-material';
import { Link } from 'react-router-dom';
import { useAuth } from '../../shared/hooks';
import { ROUTES } from '../../shared/constants';
import { ResponsiveContainer, StyledCard } from '../../shared/components';

export const DashboardPage = () => {
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
  };

  return (
    <ResponsiveContainer>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Acesso Rápido
        </Typography>
        <Button
          variant="outlined"
          color="secondary"
          startIcon={<LogoutOutlined />}
          onClick={handleLogout}
        >
          Sair
        </Button>
      </Box>

      {/* Informações do usuário */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Bem-vindo ao Access Control System!
          </Typography>

          {user && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="body1">
                <strong>Usuário:</strong> {user.fullName}
              </Typography>
              <Typography variant="body1">
                <strong>Email:</strong> {user.email}
              </Typography>
              {user.tenant && (
                <Typography variant="body1">
                  <strong>Tenant:</strong> {user.tenant.name}
                </Typography>
              )}
            </Box>
          )}
        </CardContent>
      </Card>

      {/* Ações rápidas */}
      <Typography variant="h6" gutterBottom>
        Módulos Disponíveis
      </Typography>

      <Grid container spacing={3}>
        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StyledCard>
            <CardContent sx={{ textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>Usuários</Typography>
              <Typography variant="body2" color="textSecondary" mb={2}>Gerenciar usuários e acessos</Typography>
              <Button variant="outlined" component={Link} to={ROUTES.USERS}>Acessar</Button>
            </CardContent>
          </StyledCard>
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StyledCard>
            <CardContent sx={{ textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>Perfis</Typography>
              <Typography variant="body2" color="textSecondary" mb={2}>Gerenciar papéis e permissões</Typography>
              <Button variant="outlined" component={Link} to={ROUTES.ROLES}>Acessar</Button>
            </CardContent>
          </StyledCard>
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StyledCard>
            <CardContent sx={{ textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>Módulos</Typography>
              <Typography variant="body2" color="textSecondary" mb={2}>Configurar módulos do sistema</Typography>
              <Button variant="outlined" component={Link} to={ROUTES.MODULES}>Acessar</Button>
            </CardContent>
          </StyledCard>
        </Grid>

        <Grid size={{ xs: 12, sm: 6, md: 3 }}>
          <StyledCard>
            <CardContent sx={{ textAlign: 'center' }}>
              <Typography variant="h6" gutterBottom>Tenants</Typography>
              <Typography variant="body2" color="textSecondary" mb={2}>Gerenciar instâncias de clientes</Typography>
              <Button variant="outlined" component={Link} to={ROUTES.TENANTS}>Acessar</Button>
            </CardContent>
          </StyledCard>
        </Grid>
      </Grid>
    </ResponsiveContainer>
  );
};
