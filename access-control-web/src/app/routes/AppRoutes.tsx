import { Routes, Route, Navigate } from 'react-router-dom';
import { LoginPage } from '../../features/auth';
import { DashboardPage } from '../../features/dashboard';
import { PlansPage } from '../../features/plans/PlansPage';
import { ModulesPage } from '../../features/modules';
import { RolesPage } from '../../features/roles';
import { UsersPage } from '../../features/users';
import { TenantsPage } from '../../features/tenants';
import { ProtectedRoute } from './ProtectedRoute';
import { MainLayout } from '../../shared/components';
import { ROUTES } from '../../shared/constants';

/**
 * Definição das rotas da aplicação
 * Todas as rotas administrativas são restritas ao papel SUPER_ADMIN
 */
export const AppRoutes = () => {
  return (
    <Routes>
      {/* Rota raiz - redireciona para dashboard */}
      <Route path="/" element={<Navigate to={ROUTES.DASHBOARD} replace />} />

      {/* Rota de login - pública */}
      <Route path={ROUTES.LOGIN} element={<LoginPage />} />

      {/* Rotas protegidas com layout e restrição de SUPER_ADMIN */}
      <Route
        path={ROUTES.DASHBOARD}
        element={
          <ProtectedRoute requiredRoles={['SUPER_ADMIN']}>
            <MainLayout>
              <DashboardPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />

      {/* Rotas de Planos */}
      <Route
        path={ROUTES.PLANS}
        element={
          <ProtectedRoute requiredRoles={['SUPER_ADMIN']}>
            <MainLayout>
              <PlansPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />

      {/* Rotas de Módulos */}
      <Route
        path={ROUTES.MODULES}
        element={
          <ProtectedRoute requiredRoles={['SUPER_ADMIN']}>
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
          <ProtectedRoute requiredRoles={['SUPER_ADMIN']}>
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
          <ProtectedRoute requiredRoles={['SUPER_ADMIN']}>
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
          <ProtectedRoute requiredRoles={['SUPER_ADMIN']}>
            <MainLayout>
              <TenantsPage />
            </MainLayout>
          </ProtectedRoute>
        }
      />

      {/* Rota de fallback - redireciona para dashboard */}
      <Route path="*" element={<Navigate to={ROUTES.DASHBOARD} replace />} />
    </Routes>
  );
};