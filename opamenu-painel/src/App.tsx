import { Routes, Route, Navigate } from 'react-router-dom'
import LoginPage from '@/pages/auth/LoginPage'
import RegisterPage from '@/pages/auth/RegisterPage'
import PlansPage from '@/pages/payment/PlansPage'
import DashboardLayout from '@/layouts/DashboardLayout'
import DashboardPage from '@/pages/dashboard/DashboardPage'
import AditionalGroupsPage from '@/features/aditionals/pages/AditionalGroupsPage'
import AditionalsPage from '@/features/aditionals/pages/AditionalsPage'
import CategoriesPage from '@/features/categories/pages/CategoriesPage'
import ProductsPage from '@/features/products/pages/ProductsPage'
import SettingsPage from '@/features/settings/pages/SettingsPage'
import CouponsPage from '@/features/coupons/pages/CouponsPage'
import CustomersPage from '@/features/customers/pages/CustomersPage'
import PlanPage from '@/features/subscription/pages/PlanPage'
import { Toaster } from "@/components/ui/toaster"
import OrdersPage from './features/orders/pages/OrdersPage'
import { POSPage } from '@/features/pos/pages/POSPage'
import EmployeesPage from '@/features/employees/pages/EmployeesPage'
import RolesPage from '@/features/employees/pages/RolesPage'

function App() {
  return (
    <>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/plans" element={<PlansPage />} />
        <Route path="/payment-required" element={<PlansPage />} />

        {/* Rotas Protegidas */}
        <Route path="/dashboard" element={<DashboardLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="pos" element={<POSPage />} />
          <Route path="products" element={<ProductsPage />} />
          <Route path="categories" element={<CategoriesPage />} />
          <Route path="aditionals" element={<AditionalsPage />} />
          <Route path="aditional-groups" element={<AditionalGroupsPage />} />
          <Route path="coupons" element={<CouponsPage />} />
          <Route path="orders" element={<OrdersPage />} />
          <Route path="customers" element={<CustomersPage />} />
          <Route path="employees" element={<EmployeesPage />} />
          <Route path="roles" element={<RolesPage />} />
          <Route path="settings" element={<SettingsPage />} />
          <Route path="subscription" element={<PlanPage />} />
          {/* Adicionar outras rotas filhas aqui, ex: /dashboard/menu, /dashboard/orders */}
        </Route>

        <Route path="/" element={<Navigate to="/dashboard" replace />} />
      </Routes>
      <Toaster />
    </>
  )
}

export default App
