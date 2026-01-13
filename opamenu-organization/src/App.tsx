import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { ConfirmationProvider } from './context/ConfirmationContext';
import { LoginPage } from './pages/auth/LoginPage';
import { DashboardPage } from './pages/dashboard/DashboardPage';
import { ProtectedRoute } from './routes/ProtectedRoute';
import { PublicRoute } from './routes/PublicRoute';
import { MainLayout } from './shared/components/layout/MainLayout';
import { PlansPage } from './pages/plans/PlansPage';
import { TenantProductsPage } from './pages/products/TenantProductsPage';
import { Toaster } from './shared/components/feedback/Toaster';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ConfirmationProvider>
          <Routes>
            <Route element={<PublicRoute />}>
              <Route path="/login" element={<LoginPage />} />
            </Route>

            <Route element={<ProtectedRoute />}>
              <Route element={<MainLayout />}>
                <Route path="/dashboard" element={<DashboardPage />} />
                <Route path="/plans" element={<PlansPage />} />
                <Route path="/products" element={<TenantProductsPage />} />
                <Route path="/" element={<Navigate to="/dashboard" replace />} />
              </Route>
            </Route>
            
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
          <Toaster />
        </ConfirmationProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
