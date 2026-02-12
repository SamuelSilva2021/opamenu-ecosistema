import { CheckCircle, Clock, MapPin, Phone, Mail, Package, ArrowLeft, Check } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { Badge } from "@/components/ui/badge";
import { Order } from "@/types/api";
import { useState, useEffect } from "react";
import { useOrder } from "@/hooks/use-order";
import { signalRService } from "@/services/signalr-service";

interface OrderConfirmationProps {
  order: Order | null;
  onBackToMenu: () => void;
  onNewOrder: () => void;
}

const OrderConfirmation = ({ order, onBackToMenu, onNewOrder }: OrderConfirmationProps) => {
  const [currentOrder, setCurrentOrder] = useState<Order | null>(order);
  const { getOrderById } = useOrder();

  // Setup SignalR connection and listeners
  useEffect(() => {
    if (!currentOrder?.id) return;

    const setupSignalR = async () => {
      try {
        await signalRService.startConnection();
        await signalRService.joinOrderGroup(currentOrder.id);

        signalRService.onOrderStatusUpdated(async (orderId, status) => {
          console.log(`Recebida atualização de status para pedido ${orderId}: ${status}`);
          // Se o ID corresponder (tratando string/number)
          if (String(orderId) === String(currentOrder.id)) {
             // Atualizar o pedido completo para garantir dados frescos
             const updatedOrder = await getOrderById(String(orderId));
             if (updatedOrder) {
               setCurrentOrder(updatedOrder);
             }
          }
        });
      } catch (error) {
        console.error("Erro ao configurar SignalR:", error);
      }
    };

    setupSignalR();

    return () => {
      if (currentOrder?.id) {
        signalRService.leaveOrderGroup(currentOrder.id);
      }
    };
    // Adicionando currentOrder.id e getOrderById como dependências
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentOrder?.id]);

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
      return 'bg-gray-100 text-gray-800 border-gray-200';
    }
    const statusStr = String(status).toLowerCase();
    switch (statusStr) {
      case 'pending':
      case '0':
        return 'bg-yellow-50 text-yellow-700 border-yellow-200';
      case 'confirmed':
      case '1':
        return 'bg-blue-50 text-blue-700 border-blue-200';
      case 'preparing':
      case '2':
        return 'bg-orange-50 text-orange-700 border-orange-200';
      case 'ready':
      case '3':
        return 'bg-green-50 text-green-700 border-green-200';
      case 'outfordelivery':
      case '4':
        return 'bg-purple-50 text-purple-700 border-purple-200';
      case 'delivered':
      case '5':
        return 'bg-emerald-50 text-emerald-700 border-emerald-200';
      case 'cancelled':
      case '6':
        return 'bg-red-50 text-red-700 border-red-200';
      default:
        return 'bg-gray-50 text-gray-700 border-gray-200';
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
      <div className="max-w-4xl mx-auto p-0 md:p-6">
        <Card className="border-none shadow-none md:border md:shadow-sm rounded-xl">
          <CardContent className="p-10 text-center space-y-4">
            <div className="bg-destructive/10 w-20 h-20 rounded-full flex items-center justify-center mx-auto">
              <span className="text-4xl">⚠️</span>
            </div>
            <h2 className="text-2xl font-bold text-foreground">Pedido não encontrado</h2>
            <p className="text-muted-foreground font-medium">
              Não foi possível carregar as informações do pedido.
            </p>
            <Button onClick={onBackToMenu} className="h-12 rounded-xl font-bold bg-primary hover:bg-primary/90 text-white mt-4">
              <ArrowLeft className="h-5 w-5 mr-2" />
              Voltar ao Menu
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto py-4 md:p-6 space-y-6">
      {/* Header de Sucesso */}
      <Card className="border-0 shadow-none md:border md:shadow-sm rounded-xl bg-green-50/50 border-green-100/50 overflow-hidden">
        <CardContent className="p-6 text-center space-y-2">
          <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-2 animate-in zoom-in duration-500">
            <CheckCircle className="h-8 w-8 text-green-600" />
          </div>
          <h1 className="text-2xl font-bold text-green-800">
            Pedido Enviado!
          </h1>
          <p className="text-green-700 font-medium">
            Seu pedido foi recebido e já estamos cuidando dele.
          </p>
        </CardContent>
      </Card>

      {/* Informações do Pedido */}
      <Card className="border-0 shadow-none md:border md:shadow-sm rounded-xl overflow-hidden">
        <CardHeader className="pb-4 px-6 md:px-8 pt-8">
          <CardTitle className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-6">
            <div className="flex items-center gap-4">
              <div className="bg-primary/10 p-3 rounded-full">
                <Package className="h-6 w-6 text-primary" />
              </div>
              <div>
                <span className="block text-xs font-semibold text-muted-foreground uppercase tracking-wide">Número do Pedido</span>
                <span className="font-bold text-2xl text-foreground">#{String(currentOrder.orderNumber || 0).padStart(3, '0')}</span>
              </div>
            </div>
            
            <div className="flex items-center gap-3 w-full sm:w-auto">
              <Badge className={`${getStatusColor(currentOrder.status)} border text-sm font-semibold py-1 px-4 rounded-full flex-1 sm:flex-none justify-center`}>
                {getStatusText(currentOrder.status)}
              </Badge>
            </div>
          </CardTitle>
        </CardHeader>

        <CardContent className="space-y-8 px-6 md:px-8 pb-8">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            {/* Dados do Cliente */}
            <div className="space-y-3">
              <h3 className="font-semibold text-lg flex items-center gap-2">
                <Phone className="w-5 h-5 text-primary" /> Contato
              </h3>
              <div className="space-y-2 text-sm">
                <div className="flex items-center gap-2 text-muted-foreground">
                   <Phone className="w-4 h-4" /> <span className="font-medium text-foreground">{currentOrder.customerPhone}</span>
                </div>
                {currentOrder.customerEmail && (
                  <div className="flex items-center gap-2 text-muted-foreground">
                    <Mail className="w-4 h-4" /> <span className="font-medium text-foreground break-all">{currentOrder.customerEmail}</span>
                  </div>
                )}
              </div>
            </div>

            {/* Tipo de Entrega */}
            <div className="space-y-3">
              <h3 className="font-semibold text-lg flex items-center gap-2">
                <MapPin className="w-5 h-5 text-primary" /> Entrega
              </h3>
              <div className="space-y-2 text-sm">
                 {currentOrder.isDelivery ? (
                    <>
                      <p className="font-medium text-foreground">Entrega em domicílio</p>
                      <p className="text-muted-foreground leading-relaxed">
                        {currentOrder.deliveryAddress}
                      </p>
                    </>
                  ) : (
                    <p className="font-medium text-foreground">Retirada no local</p>
                  )}
              </div>
            </div>
          </div>

          {/* Tempo Estimado */}
          {currentOrder.estimatedPreparationMinutes && (
            <div className="flex items-center gap-3 p-4 bg-blue-50/50 border border-blue-100 rounded-xl">
              <div className="bg-blue-100 p-2 rounded-full">
                <Clock className="h-5 w-5 text-blue-600" />
              </div>
              <div>
                <span className="block text-xs font-semibold text-blue-600/80 uppercase tracking-wide">Tempo Estimado</span>
                <span className="font-bold text-lg text-blue-900">
                  {currentOrder.estimatedPreparationMinutes} minutos
                </span>
              </div>
            </div>
          )}

          <Separator />

          {/* Items do Pedido */}
          <div className="space-y-4">
            <h3 className="font-semibold text-lg">Itens do Pedido</h3>
            <div className="space-y-3">
              {currentOrder.items?.map((item, index) => (
                <div key={index} className="flex flex-col sm:flex-row justify-between sm:items-center py-3 px-4 bg-muted/30 rounded-lg gap-3 hover:bg-muted/50 transition-colors">
                  <div className="flex-1 space-y-1">
                    <div className="flex items-start justify-between sm:block">
                      <div className="flex items-center gap-3">
                         <Badge variant="secondary" className="h-6 min-w-6 flex items-center justify-center p-0 rounded-md text-xs font-bold">
                           {item.quantity}x
                         </Badge>
                         <p className="font-medium text-base text-foreground">
                          {item.productName || `Produto ${item.productId}`}
                        </p>
                      </div>
                      <p className="font-bold text-base sm:hidden">
                        {formatPrice(item.subtotal || (item.unitPrice * item.quantity))}
                      </p>
                    </div>
                    {item.notes && (
                      <p className="text-sm text-muted-foreground italic pl-9">
                        Obs: {item.notes}
                      </p>
                    )}
                  </div>
                  <div className="text-right hidden sm:block">
                    <p className="font-bold text-base">
                      {formatPrice(item.subtotal || (item.unitPrice * item.quantity))}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <Separator />

          {/* Resumo do Pedido */}
          <div className="bg-muted/30 p-6 rounded-xl space-y-3">
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground font-medium">Subtotal</span>
              <span className="font-medium">{formatPrice(currentOrder.subtotal)}</span>
            </div>
            {currentOrder.deliveryFee > 0 && (
              <div className="flex justify-between text-sm">
                <span className="text-muted-foreground font-medium">Taxa de entrega</span>
                <span className="font-medium">{formatPrice(currentOrder.deliveryFee)}</span>
              </div>
            )}
            <Separator className="bg-border/50 my-1" />
            <div className="flex justify-between items-end pt-1">
              <span className="font-bold text-lg">Total</span>
              <span className="font-bold text-2xl text-primary">{formatPrice(currentOrder.total)}</span>
            </div>
          </div>

          {/* Observações */}
          {currentOrder.notes && (
            <div className="space-y-2">
              <h3 className="font-semibold text-sm text-muted-foreground uppercase tracking-wide">Observações do Pedido</h3>
              <p className="text-sm font-medium bg-yellow-50/50 border border-yellow-100 p-4 rounded-xl text-yellow-900 leading-relaxed">
                "{currentOrder.notes}"
              </p>
            </div>
          )}

          {/* Data do Pedido */}
          <div className="text-xs font-medium text-muted-foreground text-center pt-2">
            Pedido realizado em {formatDate(currentOrder.createdAt)}
          </div>
        </CardContent>
      </Card>

      {/* Botões de Ação */}
      <div className="flex flex-col sm:flex-row gap-4 pt-2">
        <Button
          variant="outline"
          onClick={onBackToMenu}
          className="flex items-center justify-center gap-2 flex-1 h-12 rounded-xl font-bold order-2 sm:order-1"
        >
          <ArrowLeft className="h-4 w-4" />
          Voltar ao Menu
        </Button>

        <Button
          onClick={onNewOrder}
          className="flex items-center justify-center gap-2 flex-1 bg-primary hover:bg-primary/90 h-12 rounded-xl text-white font-bold shadow-lg shadow-primary/20 order-1 sm:order-2"
        >
          <Package className="h-4 w-4" />
          Fazer Novo Pedido
        </Button>
      </div>
    </div >
  );
};

export default OrderConfirmation;
