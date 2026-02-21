import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { DollarSign, ShoppingBag, Users, Activity, Loader2, Calendar as CalendarIcon } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { dashboardService } from "@/features/dashboard/dashboard.service";
import { format, subDays, startOfMonth, endOfMonth, startOfToday, endOfToday, startOfYesterday, endOfYesterday } from "date-fns";
import { ptBR } from "date-fns/locale";
import { PermissionGate } from "@/components/auth/PermissionGate";
import { useState } from "react";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { cn } from "@/lib/utils";
import type { DateRange } from "react-day-picker";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip as RechartsTooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Legend
} from 'recharts';

const COLORS = ['#3b82f6', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899'];

export default function DashboardPage() {
  const [period, setPeriod] = useState<string>("this-month");
  const [dateRange, setDateRange] = useState<DateRange | undefined>({
    from: startOfMonth(new Date()),
    to: endOfMonth(new Date()),
  });

  const { data: summary, isLoading } = useQuery({
    queryKey: ["dashboard-summary", period, dateRange],
    queryFn: () => {
      let startDate: string | undefined;
      let endDate: string | undefined;

      if (period === "custom") {
        startDate = dateRange?.from?.toISOString();
        endDate = dateRange?.to?.toISOString();
      } else {
        const now = new Date();
        switch (period) {
          case "today":
            startDate = startOfToday().toISOString();
            endDate = endOfToday().toISOString();
            break;
          case "yesterday":
            startDate = startOfYesterday().toISOString();
            endDate = endOfYesterday().toISOString();
            break;
          case "last-7-days":
            startDate = subDays(now, 7).toISOString();
            endDate = now.toISOString();
            break;
          case "last-30-days":
            startDate = subDays(now, 30).toISOString();
            endDate = now.toISOString();
            break;
          case "this-month":
            startDate = startOfMonth(now).toISOString();
            endDate = endOfMonth(now).toISOString();
            break;
        }
      }

      return dashboardService.getSummary({ startDate, endDate });
    },
  });

  const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value);
  };

  const handlePeriodChange = (value: string) => {
    setPeriod(value);
    const now = new Date();
    if (value !== "custom") {
      let range: DateRange | undefined;
      switch (value) {
        case "today":
          range = { from: startOfToday(), to: endOfToday() };
          break;
        case "yesterday":
          range = { from: startOfYesterday(), to: endOfYesterday() };
          break;
        case "last-7-days":
          range = { from: subDays(now, 7), to: now };
          break;
        case "last-30-days":
          range = { from: subDays(now, 30), to: now };
          break;
        case "this-month":
          range = { from: startOfMonth(now), to: endOfMonth(now) };
          break;
      }
      setDateRange(range);
    }
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
        <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
          <div>
            <h2 className="text-3xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">Dashboard</h2>
            <p className="text-muted-foreground mt-1">
              Visão geral do seu restaurante e métricas importantes.
            </p>
          </div>

          <div className="flex items-center gap-2">
            <Select value={period} onValueChange={handlePeriodChange}>
              <SelectTrigger className="w-[180px] bg-white dark:bg-zinc-800 border-zinc-200 dark:border-zinc-700">
                <SelectValue placeholder="Selecionar período" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="today">Hoje</SelectItem>
                <SelectItem value="yesterday">Ontem</SelectItem>
                <SelectItem value="last-7-days">Últimos 7 dias</SelectItem>
                <SelectItem value="last-30-days">Últimos 30 dias</SelectItem>
                <SelectItem value="this-month">Este mês</SelectItem>
                <SelectItem value="custom">Personalizado</SelectItem>
              </SelectContent>
            </Select>

            {period === "custom" && (
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    id="date"
                    variant={"outline"}
                    className={cn(
                      "w-[300px] justify-start text-left font-normal bg-white dark:bg-zinc-800 border-zinc-200 dark:border-zinc-700",
                      !dateRange && "text-muted-foreground"
                    )}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" />
                    {dateRange?.from ? (
                      dateRange.to ? (
                        <>
                          {format(dateRange.from, "dd/MM/yyyy")} -{" "}
                          {format(dateRange.to, "dd/MM/yyyy")}
                        </>
                      ) : (
                        format(dateRange.from, "dd/MM/yyyy")
                      )
                    ) : (
                      <span>Selecione um período</span>
                    )}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="end">
                  <Calendar
                    initialFocus
                    mode="range"
                    defaultMonth={dateRange?.from}
                    selected={dateRange}
                    onSelect={setDateRange}
                    numberOfMonths={2}
                    locale={ptBR}
                  />
                </PopoverContent>
              </Popover>
            )}
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
          <Card className="border-none shadow-md bg-white dark:bg-zinc-800 hover:shadow-lg transition-shadow duration-200">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                Vendas no Período
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
                </span> em relação ao período anterior
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
              <CardTitle className="text-sm font-medium text-muted-foreground">Pedidos no Período</CardTitle>
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
                </span> em relação ao período anterior
              </p>
            </CardContent>
          </Card>

          <Card className="border-none shadow-md bg-white dark:bg-zinc-800 hover:shadow-lg transition-shadow duration-200">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">
                Ticket Médio
              </CardTitle>
              <div className="h-8 w-8 rounded-full bg-yellow-100 flex items-center justify-center">
                <DollarSign className="h-4 w-4 text-yellow-600" />
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-zinc-900 dark:text-zinc-50">
                {formatCurrency(summary?.averageTicket || 0)}
              </div>
              <p className="text-xs text-muted-foreground mt-1">
                Média por pedido no período
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
                Total de clientes na plataforma
              </p>
            </CardContent>
          </Card>
        </div>

        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
          <Card className="col-span-4 border-none shadow-md bg-white dark:bg-zinc-800">
            <CardHeader>
              <CardTitle>Vendas Diárias</CardTitle>
            </CardHeader>
            <CardContent className="h-[350px]">
              {summary?.dailySales && summary.dailySales.length > 0 ? (
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={summary.dailySales}>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="#e2e8f0" />
                    <XAxis
                      dataKey="date"
                      axisLine={false}
                      tickLine={false}
                      tick={{ fill: '#64748b', fontSize: 12 }}
                      dy={10}
                    />
                    <YAxis
                      axisLine={false}
                      tickLine={false}
                      tick={{ fill: '#64748b', fontSize: 12 }}
                      tickFormatter={(value: any) => `R$ ${value}`}
                    />
                    <RechartsTooltip
                      cursor={{ fill: '#f1f5f9' }}
                      contentStyle={{
                        borderRadius: '8px',
                        border: 'none',
                        boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)'
                      }}
                      formatter={(value: any) => [formatCurrency(Number(value || 0)), 'Total']}
                    />
                    <Bar
                      dataKey="total"
                      fill="#3b82f6"
                      radius={[4, 4, 0, 0]}
                      barSize={30}
                    />
                  </BarChart>
                </ResponsiveContainer>
              ) : (
                <div className="h-full flex items-center justify-center text-muted-foreground bg-zinc-50 dark:bg-zinc-900 rounded-md">
                  Nenhum dado de vendas no período
                </div>
              )}
            </CardContent>
          </Card>

          <Card className="col-span-3 border-none shadow-md bg-white dark:bg-zinc-800">
            <CardHeader>
              <CardTitle>Vendas por Categoria</CardTitle>
            </CardHeader>
            <CardContent className="h-[350px]">
              {summary?.categorySales && summary.categorySales.length > 0 ? (
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={summary.categorySales}
                      cx="50%"
                      cy="50%"
                      innerRadius={60}
                      outerRadius={80}
                      paddingAngle={5}
                      dataKey="total"
                      nameKey="categoryName"
                    >
                      {summary.categorySales.map((_, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <RechartsTooltip
                      formatter={(value: any) => formatCurrency(Number(value || 0))}
                    />
                    <Legend verticalAlign="bottom" height={36} />
                  </PieChart>
                </ResponsiveContainer>
              ) : (
                <div className="h-full flex items-center justify-center text-muted-foreground bg-zinc-50 dark:bg-zinc-900 rounded-md">
                  Nenhuma categoria identificada
                </div>
              )}
            </CardContent>
          </Card>
        </div>

        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
          {/* Recent Orders moved down or adjusted to fit the new layout if needed */}
          <Card className="col-span-7 border-none shadow-md bg-white dark:bg-zinc-800">
            <CardHeader>
              <CardTitle>Vendas Recentes</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                {summary?.recentOrders && summary.recentOrders.length > 0 ? (
                  summary.recentOrders.map((order) => (
                    <div key={order.id} className="flex items-center p-4 border rounded-lg border-zinc-100 dark:border-zinc-700">
                      <div className="space-y-1">
                        <p className="text-sm font-medium leading-none">Pedido #{order.id.split('-')[0]}</p>
                        <p className="text-xs text-muted-foreground">
                          {order.customerName}
                        </p>
                        <p className="text-[10px] text-muted-foreground uppercase">
                          {format(new Date(order.createdAt), "dd/MM HH:mm", { locale: ptBR })}
                        </p>
                      </div>
                      <div className="ml-auto font-medium text-green-600">+{formatCurrency(order.amount)}</div>
                    </div>
                  ))
                ) : (
                  <p className="text-sm text-muted-foreground text-center py-4 col-span-full">Nenhuma venda no período.</p>
                )}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </PermissionGate>
  );
}
