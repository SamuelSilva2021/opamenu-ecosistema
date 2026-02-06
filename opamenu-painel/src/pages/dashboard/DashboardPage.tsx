import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { DollarSign, ShoppingBag, Users, Activity, Loader2 } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { dashboardService } from "@/features/dashboard/dashboard.service";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { PermissionGate } from "@/components/auth/PermissionGate";

export default function DashboardPage() {
  const { data: summary, isLoading } = useQuery({
    queryKey: ["dashboard-summary"],
    queryFn: dashboardService.getSummary,
  });

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  if (isLoading) {
    return (
      <div className="flex h-[50vh] items-center justify-center">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <PermissionGate module="DASHBOARD" operation="READ" fallback={
      <div className="flex h-[50vh] items-center justify-center">
        <p className="text-muted-foreground">Você não tem permissão para visualizar o dashboard.</p>
      </div>
    }>
      <div className="space-y-8 animate-in fade-in duration-500">
        <div className="flex items-center justify-between space-y-2">
          <div>
            <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Dashboard</h2>
            <p className="text-muted-foreground mt-1">
              Visão geral do seu restaurante e métricas importantes.
            </p>
          </div>
          {/* Adicionar aqui botões de ação rápida se necessário */}
        </div>

        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
          <Card className="border-none shadow-md bg-white dark:bg-zinc-800 hover:shadow-lg transition-shadow duration-200">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                Vendas Totais
              </CardTitle>
              <div className="h-8 w-8 rounded-full bg-green-100 flex items-center justify-center">
                <DollarSign className="h-4 w-4 text-green-600" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-zinc-900 dark:text-zinc-50">
                {formatCurrency(summary?.totalRevenue || 0)}
              </div>
              <p className="text-xs text-muted-foreground mt-1">
                <span className={`font-medium ${summary?.totalRevenueGrowth && summary.totalRevenueGrowth >= 0 ? 'text-green-600' : 'text-red-600'}`}>
                  {summary?.totalRevenueGrowth && summary.totalRevenueGrowth > 0 ? '+' : ''}
                  {summary?.totalRevenueGrowth?.toFixed(1)}%
                </span> em relação ao mês anterior
              </p>
            </CardContent>
          </Card>

          <Card className="border-none shadow-md bg-white dark:bg-zinc-800 hover:shadow-lg transition-shadow duration-200">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                Pedidos Hoje
              </CardTitle>
              <div className="h-8 w-8 rounded-full bg-blue-100 flex items-center justify-center">
                <ShoppingBag className="h-4 w-4 text-blue-600" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-zinc-900 dark:text-zinc-50">
                {summary?.ordersToday || 0}
              </div>
              <p className="text-xs text-muted-foreground mt-1">
                <span className={`font-medium ${summary?.ordersTodayGrowth && summary.ordersTodayGrowth >= 0 ? 'text-blue-600' : 'text-red-600'}`}>
                  {summary?.ordersTodayGrowth && summary.ordersTodayGrowth > 0 ? '+' : ''}
                  {summary?.ordersTodayGrowth?.toFixed(1)}%
                </span> em relação a ontem
              </p>
            </CardContent>
          </Card>

          <Card className="border-none shadow-md bg-white dark:bg-zinc-800 hover:shadow-lg transition-shadow duration-200">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Vendas (Qtd)</CardTitle>
              <div className="h-8 w-8 rounded-full bg-orange-100 flex items-center justify-center">
                <Activity className="h-4 w-4 text-orange-600" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-zinc-900 dark:text-zinc-50">
                {summary?.totalOrders || 0}
              </div>
              <p className="text-xs text-muted-foreground mt-1">
                <span className={`font-medium ${summary?.totalOrdersGrowth && summary.totalOrdersGrowth >= 0 ? 'text-orange-600' : 'text-red-600'}`}>
                  {summary?.totalOrdersGrowth && summary.totalOrdersGrowth > 0 ? '+' : ''}
                  {summary?.totalOrdersGrowth?.toFixed(1)}%
                </span> em relação ao mês anterior
              </p>
            </CardContent>
          </Card>

          <Card className="border-none shadow-md bg-white dark:bg-zinc-800 hover:shadow-lg transition-shadow duration-200">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                Clientes Ativos
              </CardTitle>
              <div className="h-8 w-8 rounded-full bg-purple-100 flex items-center justify-center">
                <Users className="h-4 w-4 text-purple-600" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-zinc-900 dark:text-zinc-50">
                {summary?.activeCustomers || 0}
              </div>
              <p className="text-xs text-muted-foreground mt-1">
                <span className={`font-medium ${summary?.activeCustomersGrowth && summary.activeCustomersGrowth >= 0 ? 'text-purple-600' : 'text-red-600'}`}>
                  {summary?.activeCustomersGrowth && summary.activeCustomersGrowth > 0 ? '+' : ''}
                  {summary?.activeCustomersGrowth?.toFixed(1)}%
                </span> desde o último mês
              </p>
            </CardContent>
          </Card>
        </div>

        {/* Placeholder para gráficos futuros */}
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
          <Card className="col-span-4 border-none shadow-md bg-white dark:bg-zinc-800">
            <CardHeader>
              <CardTitle>Visão Geral</CardTitle>
            </CardHeader>
            <CardContent className="pl-2">
              <div className="h-[300px] flex items-center justify-center text-muted-foreground bg-zinc-50 dark:bg-zinc-900 rounded-md">
                Gráfico de Vendas (Em breve)
              </div>
            </CardContent>
          </Card>
          <Card className="col-span-3 border-none shadow-md bg-white dark:bg-zinc-800">
            <CardHeader>
              <CardTitle>Vendas Recentes</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-8">
                {summary?.recentOrders && summary.recentOrders.length > 0 ? (
                  summary.recentOrders.map((order) => (
                    <div key={order.id} className="flex items-center">
                      <div className="space-y-1">
                        <p className="text-sm font-medium leading-none">Pedido #{order.id}</p>
                        <p className="text-xs text-muted-foreground">
                          {order.customerName} - {format(new Date(order.createdAt), "dd/MM 'às' HH:mm", { locale: ptBR })}
                        </p>
                      </div>
                      <div className="ml-auto font-medium">+{formatCurrency(order.amount)}</div>
                    </div>
                  ))
                ) : (
                  <p className="text-sm text-muted-foreground text-center py-4">Nenhuma venda recente.</p>
                )}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </PermissionGate>
  );
}
