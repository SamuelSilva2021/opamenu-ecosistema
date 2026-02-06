import { CheckCircle, Clock, MapPin, Phone, Mail, Package, ArrowLeft, RefreshCw } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { Badge } from "@/components/ui/badge";
import { Order } from "@/types/api";
import { useState, useEffect } from "react";
import { useOrder } from "@/hooks/use-order";

interface OrderConfirmationProps {
  order: Order | null;
  onBackToMenu: () => void;
  onNewOrder: () => void;
}

const OrderConfirmation = ({ order, onBackToMenu, onNewOrder }: OrderConfirmationProps) => {
  const [currentOrder, setCurrentOrder] = useState<Order | null>(order);
  const [isRefreshing, setIsRefreshing] = useState(false);
  const { getOrderById } = useOrder();

  useEffect(() => {
    if (!currentOrder?.id) return;

    const interval = setInterval(async () => {
      try {
        setIsRefreshing(true);
        const updatedOrder = await getOrderById(currentOrder.id);
        if (updatedOrder) {
          setCurrentOrder(updatedOrder);
        }
      } catch (error) {
        console.error('Erro ao atualizar status do pedido:', error);
      } finally {
        setIsRefreshing(false);
      }
    }, 30000); // 30 segundos

    return () => clearInterval(interval);
  }, [currentOrder?.id, getOrderById]);

  // Função para atualizar manualmente
  const handleRefreshStatus = async () => {
    if (!currentOrder?.id) return;

    try {
      setIsRefreshing(true);
      const updatedOrder = await getOrderById(currentOrder.id);
      if (updatedOrder) {
        setCurrentOrder(updatedOrder);
      }
    } catch (error) {
      console.error('Erro ao atualizar status do pedido:', error);
    } finally {
      setIsRefreshing(false);
    }
  };
  const formatPrice = (price: number | undefined | null) => {
    if (price === undefined || price === null || isNaN(price)) {
      return 'R$ 0,00';
    }
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(price);
  };

  const formatDate = (date: string | undefined | null) => {
    if (!date) {
      return 'Data não disponível';
    }

    const parsedDate = new Date(date);
    if (isNaN(parsedDate.getTime())) {
      return 'Data inválida';
    }

    return new Intl.DateTimeFormat('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(parsedDate);
  };

  const getStatusColor = (status: string | number | undefined | null) => {
    if (status === undefined || status === null) {
      return 'bg-gray-100 text-gray-800';
    }
    const statusStr = String(status).toLowerCase();
    switch (statusStr) {
      case 'pending':
      case '0':
        return 'bg-yellow-100 text-yellow-800';
      case 'confirmed':
      case '1':
        return 'bg-blue-100 text-blue-800';
      case 'preparing':
      case '2':
        return 'bg-orange-100 text-orange-800';
      case 'ready':
      case '3':
        return 'bg-green-100 text-green-800';
      case 'outfordelivery':
      case '4':
        return 'bg-purple-100 text-purple-800';
      case 'delivered':
      case '5':
        return 'bg-emerald-100 text-emerald-800';
      case 'cancelled':
      case '6':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusText = (status: string | number | undefined | null) => {
    if (status === undefined || status === null) {
      return 'Status Indefinido';
    }
    const statusStr = String(status).toLowerCase();
    switch (statusStr) {
      case 'pending':
      case '0':
        return 'Aguardando Confirmação';
      case 'confirmed':
      case '1':
        return 'Confirmado';
      case 'preparing':
      case '2':
        return 'Preparando';
      case 'ready':
      case '3':
        return 'Pronto';
      case 'outfordelivery':
      case '4':
        return 'Saiu para entrega';
      case 'delivered':
      case '5':
        return 'Entregue';
      case 'cancelled':
      case '6':
        return 'Cancelado';
      default:
        return statusStr;
    }
  };

  if (!currentOrder) {
    return (
      <div className="max-w-4xl mx-auto p-4 md:p-6">
        <Card className="border-none shadow-sm">
          <CardContent className="p-8 text-center">
            <div className="text-6xl mb-4">⚠️</div>
            <h2 className="text-xl font-semibold mb-2">Pedido não encontrado</h2>
            <p className="text-muted-foreground mb-4">
              Não foi possível carregar as informações do pedido.
            </p>
            <Button onClick={onBackToMenu}>
              Voltar ao Menu
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="max-w-5xl mx-auto py-4 md:p-6 space-y-6">
      {/* Header de Sucesso */}
      <Card className="border-green-200 bg-green-50 shadow-sm mx-0 md:mx-0">
        <CardContent className="p-6 text-center">
          <CheckCircle className="h-16 w-16 text-green-600 mx-auto mb-4" />
          <h1 className="text-2xl font-bold text-green-800 mb-2">
            Pedido Enviado!
          </h1>
          <p className="text-green-700">
            Seu pedido foi enviado com sucesso!
          </p>
        </CardContent>
      </Card>

      {/* Informações do Pedido */}
      <Card className="border-none shadow-sm mx-0 md:mx-0">
        <CardHeader className="pb-4 px-4 md:px-6">
          <CardTitle className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
            <span className="flex items-center gap-2 text-xl">
              <Package className="h-6 w-6 text-opamenu-green" />
              Pedido #{String(currentOrder.orderNumber || 0).padStart(3, '0')}
            </span>
            <div className="flex items-center gap-2 w-full sm:w-auto">
              <Badge className={`${getStatusColor(currentOrder.status)} text-sm py-1 px-3 flex-1 sm:flex-none justify-center`}>
                {getStatusText(currentOrder.status)}
              </Badge>
              <Button
                variant="ghost"
                size="icon"
                onClick={handleRefreshStatus}
                disabled={isRefreshing}
                className="shrink-0"
              >
                <RefreshCw className={`h-4 w-4 ${isRefreshing ? 'animate-spin' : ''}`} />
              </Button>
            </div>
          </CardTitle>
        </CardHeader>

        <CardContent className="space-y-6 px-4 md:px-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Dados do Cliente */}
            <div className="space-y-3">
              <h3 className="font-semibold text-base flex items-center gap-2">
                <div className="w-1 h-4 bg-opamenu-green rounded-full"></div>
                Dados do Cliente
              </h3>
              <div className="bg-muted/30 p-4 rounded-lg space-y-3 text-sm">
                <div className="flex items-center gap-3">
                  <Phone className="h-4 w-4 text-muted-foreground shrink-0" />
                  <span className="font-medium">{currentOrder.customerPhone}</span>
                </div>
                {currentOrder.customerEmail && (
                  <div className="flex items-center gap-3">
                    <Mail className="h-4 w-4 text-muted-foreground shrink-0" />
                    <span className="break-all">{currentOrder.customerEmail}</span>
                  </div>
                )}
              </div>
            </div>

            {/* Tipo de Entrega */}
            <div className="space-y-3">
              <h3 className="font-semibold text-base flex items-center gap-2">
                <div className="w-1 h-4 bg-opamenu-green rounded-full"></div>
                Entrega
              </h3>
              <div className="bg-muted/30 p-4 rounded-lg flex items-start gap-3">
                <MapPin className="h-4 w-4 text-muted-foreground mt-0.5 shrink-0" />
                <div className="text-sm">
                  {currentOrder.isDelivery ? (
                    <div className="space-y-1">
                      <p className="font-medium">Entrega em domicílio</p>
                      <p className="text-muted-foreground leading-relaxed">
                        {currentOrder.deliveryAddress}
                      </p>
                    </div>
                  ) : (
                    <p className="font-medium">Retirada no local</p>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* Tempo Estimado */}
          {currentOrder.estimatedPreparationMinutes && (
            <div className="flex items-center gap-3 p-4 bg-blue-50/50 border border-blue-100 rounded-xl">
              <Clock className="h-5 w-5 text-blue-600 shrink-0" />
              <span className="text-sm text-blue-900">
                <span className="font-semibold">Tempo estimado:</span>{' '}
                {currentOrder.estimatedPreparationMinutes} minutos
              </span>
            </div>
          )}

          <Separator />

          {/* Items do Pedido */}
          <div className="space-y-4">
            <h3 className="font-semibold text-base">Itens do Pedido</h3>
            <div className="space-y-3">
              {currentOrder.items?.map((item, index) => (
                <div key={index} className="flex flex-col sm:flex-row justify-between sm:items-center py-3 px-4 bg-muted/20 rounded-lg gap-2">
                  <div className="flex-1 space-y-1">
                    <div className="flex items-start justify-between sm:block">
                      <p className="font-medium text-base">
                        {item.productName || `Produto ${item.productId}`}
                      </p>
                      <p className="font-semibold sm:hidden">
                        {formatPrice(item.subtotal || (item.unitPrice * item.quantity))}
                      </p>
                    </div>
                    <p className="text-sm text-muted-foreground">
                      {item.quantity}x {formatPrice(item.unitPrice)}
                    </p>
                    {item.notes && (
                      <p className="text-xs text-muted-foreground italic bg-background/50 p-2 rounded mt-1">
                        Obs: {item.notes}
                      </p>
                    )}
                  </div>
                  <div className="text-right hidden sm:block">
                    <p className="font-semibold">
                      {formatPrice(item.subtotal || (item.unitPrice * item.quantity))}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <Separator />

          {/* Resumo do Pedido */}
          <div className="bg-muted/30 p-4 rounded-xl space-y-3">
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground">Subtotal</span>
              <span className="font-medium">{formatPrice(currentOrder.subtotal)}</span>
            </div>
            {currentOrder.deliveryFee > 0 && (
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground">Taxa de entrega</span>
                <span className="font-medium">{formatPrice(currentOrder.deliveryFee)}</span>
              </div>
            )}
            <Separator className="bg-border/50" />
            <div className="flex justify-between items-center">
              <span className="font-semibold text-lg">Total</span>
              <span className="text-xl font-bold text-opamenu-green">{formatPrice(currentOrder.total)}</span>
            </div>
          </div>

          {/* Observações */}
          {currentOrder.notes && (
            <div className="space-y-2">
              <h3 className="font-semibold text-sm">Observações do Pedido</h3>
              <p className="text-sm bg-yellow-50/50 border border-yellow-100 p-4 rounded-lg text-yellow-900">
                {currentOrder.notes}
              </p>
            </div>
          )}

          {/* Data do Pedido */}
          <div className="text-xs text-muted-foreground text-center pt-2">
            Pedido realizado em {formatDate(currentOrder.createdAt)}
          </div>
        </CardContent>
      </Card>

      {/* Botões de Ação */}
      <div className="flex flex-col sm:flex-row gap-4 pt-2 px-4 md:px-0">
        <Button
          variant="outline"
          onClick={onBackToMenu}
          className="flex items-center justify-center gap-2 flex-1 h-12 text-base order-2 sm:order-1"
        >
          <ArrowLeft className="h-4 w-4" />
          Voltar ao Menu
        </Button>

        <Button
          onClick={onNewOrder}
          className="flex items-center justify-center gap-2 flex-1 bg-opamenu-green hover:bg-opamenu-green/90 h-12 text-base font-medium shadow-md shadow-green-900/10 order-1 sm:order-2"
        >
          <Package className="h-5 w-5" />
          Fazer Novo Pedido
        </Button>
      </div>
    </div >
  );
};

export default OrderConfirmation;
