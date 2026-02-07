import { Routes, Route, Navigate } from 'react-router-dom';
import { LoginPage } from '../../features/auth';
import { DashboardPage } from '../../features/dashboard';
import { ModulesPage } from '../../features/modules';
import { RolesPage } from '../../features/roles';
import { UsersPage } from '../../features/users';
import { TenantsPage } from '../../features/tenants';
import { ProtectedRoute } from './ProtectedRoute';
import { MainLayout } from '../../shared/components';
import { ROUTES } from '../../shared/constants';

export const AppRoutes = () => {
  return (
    <Routes>
      {/* Rota raiz - redireciona para dashboard */}
      <Route path="/" element={<Navigate to={ROUTES.DASHBOARD} replace />} />

      {/* Rota de login - pública */}
      <Route path={ROUTES.LOGIN} element={<LoginPage />} />

      {/* Rotas protegidas com layout */}
      <Route
        path={ROUTES.DASHBOARD}
        element={
          <ProtectedRoute>
            <MainLayout>
              <DashboardPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />

      {/* Rotas de Módulos */}
      <Route
        path={ROUTES.MODULES}
        element={
          <ProtectedRoute>
            <MainLayout>
              <ModulesPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />

      {/* Rotas de Roles */}
      <Route
        path={ROUTES.ROLES}
        element={
          <ProtectedRoute>
            <MainLayout>
              <RolesPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />

      {/* Rotas de Usuários */}
      <Route
        path={ROUTES.USERS}
        element={
          <ProtectedRoute>
            <MainLayout>
              <UsersPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />

      {/* Rotas de Tenants */}
      <Route
        path={ROUTES.TENANTS}
        element={
          <ProtectedRoute>
            <MainLayout>
              <TenantsPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />

      {/* TODO: Adicionar mais rotas conforme necessário */}

      {/* Rota de fallback - redireciona para dashboard */}
      <Route path="*" element={<Navigate to={ROUTES.DASHBOARD} replace />} />
    </Routes>
  );
};