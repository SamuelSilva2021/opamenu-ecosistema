import { CheckCircle, Clock, MapPin, Phone, Mail, Package, ArrowLeft, RefreshCw, Check } from "lucide-react";
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
        <Card className="border-none shadow-none md:border md:shadow-sm rounded-[2.5rem]">
          <CardContent className="p-10 text-center space-y-4">
            <div className="bg-destructive/10 w-20 h-20 rounded-full flex items-center justify-center mx-auto">
              <span className="text-4xl">⚠️</span>
            </div>
            <h2 className="text-2xl font-black uppercase italic tracking-tighter text-foreground">Pedido não encontrado</h2>
            <p className="text-muted-foreground font-medium">
              Não foi possível carregar as informações do pedido.
            </p>
            <Button onClick={onBackToMenu} className="h-14 rounded-2xl font-black uppercase italic tracking-widest bg-primary hover:bg-primary/90 text-white mt-4">
              <ArrowLeft className="h-5 w-5 mr-2 stroke-[3]" />
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
      <Card className="border-0 shadow-none md:border md:shadow-sm rounded-[2.5rem] bg-green-50/50 border-green-100/50 overflow-hidden">
        <CardContent className="p-8 text-center space-y-4">
          <div className="bg-green-100 w-24 h-24 rounded-full flex items-center justify-center mx-auto mb-2 animate-in zoom-in duration-500">
            <CheckCircle className="h-12 w-12 text-green-600 stroke-[2.5]" />
          </div>
          <div className="space-y-2">
            <h1 className="text-3xl font-black text-green-800 uppercase italic tracking-tighter">
              Pedido Enviado!
            </h1>
            <p className="text-green-700 font-medium text-lg">
              Seu pedido foi recebido e já estamos cuidando dele.
            </p>
          </div>
        </CardContent>
      </Card>

      {/* Informações do Pedido */}
      <Card className="border-0 shadow-none md:border md:shadow-sm rounded-[2.5rem] overflow-hidden">
        <CardHeader className="pb-4 px-6 md:px-8 pt-8">
          <CardTitle className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-6">
            <div className="flex items-center gap-4">
              <div className="bg-primary/10 p-3 rounded-2xl">
                <Package className="h-8 w-8 text-primary" />
              </div>
              <div>
                <span className="block text-xs font-bold uppercase tracking-widest text-muted-foreground">Número do Pedido</span>
                <span className="font-black text-3xl text-foreground tracking-tight">#{String(currentOrder.orderNumber || 0).padStart(3, '0')}</span>
              </div>
            </div>
            
            <div className="flex items-center gap-3 w-full sm:w-auto">
              <Badge className={`${getStatusColor(currentOrder.status)} border text-sm font-bold uppercase tracking-wider py-2 px-4 rounded-xl flex-1 sm:flex-none justify-center`}>
                {getStatusText(currentOrder.status)}
              </Badge>
              <Button
                variant="outline"
                size="icon"
                onClick={handleRefreshStatus}
                disabled={isRefreshing}
                className="shrink-0 h-10 w-10 rounded-xl border-2"
              >
                <RefreshCw className={`h-4 w-4 stroke-[3] ${isRefreshing ? 'animate-spin' : ''}`} />
              </Button>
            </div>
          </CardTitle>
        </CardHeader>

        <CardContent className="space-y-8 px-6 md:px-8 pb-8">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            {/* Dados do Cliente */}
            <div className="space-y-4">
              <h3 className="font-black text-lg uppercase italic tracking-tight flex items-center gap-2">
                <div className="w-1.5 h-6 bg-primary rounded-full"></div>
                Dados do Cliente
              </h3>
              <div className="bg-muted/30 p-6 rounded-[2rem] space-y-4 border border-border/50">
                <div className="flex items-center gap-4">
                  <div className="bg-background p-2 rounded-xl shadow-sm">
                    <Phone className="h-5 w-5 text-muted-foreground stroke-[2.5]" />
                  </div>
                  <div>
                    <span className="block text-[10px] font-bold uppercase tracking-widest text-muted-foreground">Telefone</span>
                    <span className="font-bold text-foreground">{currentOrder.customerPhone}</span>
                  </div>
                </div>
                {currentOrder.customerEmail && (
                  <div className="flex items-center gap-4">
                    <div className="bg-background p-2 rounded-xl shadow-sm">
                      <Mail className="h-5 w-5 text-muted-foreground stroke-[2.5]" />
                    </div>
                    <div>
                      <span className="block text-[10px] font-bold uppercase tracking-widest text-muted-foreground">Email</span>
                      <span className="font-bold text-foreground break-all">{currentOrder.customerEmail}</span>
                    </div>
                  </div>
                )}
              </div>
            </div>

            {/* Tipo de Entrega */}
            <div className="space-y-4">
              <h3 className="font-black text-lg uppercase italic tracking-tight flex items-center gap-2">
                <div className="w-1.5 h-6 bg-primary rounded-full"></div>
                Entrega
              </h3>
              <div className="bg-muted/30 p-6 rounded-[2rem] flex items-start gap-4 border border-border/50 h-full">
                <div className="bg-background p-2 rounded-xl shadow-sm mt-1">
                  <MapPin className="h-5 w-5 text-muted-foreground stroke-[2.5]" />
                </div>
                <div className="space-y-1">
                  {currentOrder.isDelivery ? (
                    <>
                      <p className="font-black text-base uppercase tracking-tight">Entrega em domicílio</p>
                      <p className="text-muted-foreground font-medium text-sm leading-relaxed">
                        {currentOrder.deliveryAddress}
                      </p>
                    </>
                  ) : (
                    <p className="font-black text-base uppercase tracking-tight">Retirada no local</p>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* Tempo Estimado */}
          {currentOrder.estimatedPreparationMinutes && (
            <div className="flex items-center gap-4 p-6 bg-blue-50/50 border border-blue-100 rounded-[2rem]">
              <div className="bg-blue-100 p-3 rounded-2xl">
                <Clock className="h-6 w-6 text-blue-600 stroke-[2.5]" />
              </div>
              <div>
                <span className="block text-[10px] font-bold uppercase tracking-widest text-blue-600/80">Tempo Estimado</span>
                <span className="font-black text-xl text-blue-900 tracking-tight">
                  {currentOrder.estimatedPreparationMinutes} minutos
                </span>
              </div>
            </div>
          )}

          <Separator className="bg-border/50" />

          {/* Items do Pedido */}
          <div className="space-y-6">
            <h3 className="font-black text-lg uppercase italic tracking-tight">Itens do Pedido</h3>
            <div className="space-y-4">
              {currentOrder.items?.map((item, index) => (
                <div key={index} className="flex flex-col sm:flex-row justify-between sm:items-center py-4 px-6 bg-muted/20 rounded-[1.5rem] gap-4 border border-border/30 hover:bg-muted/30 transition-colors">
                  <div className="flex-1 space-y-2">
                    <div className="flex items-start justify-between sm:block">
                      <div className="flex items-center gap-3">
                         <Badge variant="outline" className="h-6 min-w-6 flex items-center justify-center p-0 rounded-md text-xs font-black bg-background border-2 border-muted-foreground/20">
                           {item.quantity}x
                         </Badge>
                         <p className="font-bold text-base uppercase tracking-tight text-foreground">
                          {item.productName || `Produto ${item.productId}`}
                        </p>
                      </div>
                      <p className="font-black text-base sm:hidden">
                        {formatPrice(item.subtotal || (item.unitPrice * item.quantity))}
                      </p>
                    </div>
                    {item.notes && (
                      <p className="text-sm text-muted-foreground font-medium italic bg-background/50 p-3 rounded-xl border border-border/50">
                        Obs: {item.notes}
                      </p>
                    )}
                  </div>
                  <div className="text-right hidden sm:block">
                    <p className="text-xs font-bold text-muted-foreground uppercase tracking-wider mb-1">Total Item</p>
                    <p className="font-black text-lg">
                      {formatPrice(item.subtotal || (item.unitPrice * item.quantity))}
                    </p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <Separator className="bg-border/50" />

          {/* Resumo do Pedido */}
          <div className="bg-muted/30 p-8 rounded-[2rem] space-y-4 border border-border/50">
            <div className="flex justify-between text-sm font-medium">
              <span className="text-muted-foreground uppercase tracking-wider text-xs font-bold">Subtotal</span>
              <span className="font-mono text-base">{formatPrice(currentOrder.subtotal)}</span>
            </div>
            {currentOrder.deliveryFee > 0 && (
              <div className="flex justify-between text-sm font-medium">
                <span className="text-muted-foreground uppercase tracking-wider text-xs font-bold">Taxa de entrega</span>
                <span className="font-mono text-base">{formatPrice(currentOrder.deliveryFee)}</span>
              </div>
            )}
            <Separator className="bg-border/50 my-2" />
            <div className="flex justify-between items-end pt-2">
              <span className="font-black text-xl uppercase italic tracking-tighter">Total</span>
              <span className="font-black text-3xl text-primary tracking-tight">{formatPrice(currentOrder.total)}</span>
            </div>
          </div>

          {/* Observações */}
          {currentOrder.notes && (
            <div className="space-y-3">
              <h3 className="font-black text-sm uppercase tracking-wider text-muted-foreground">Observações do Pedido</h3>
              <p className="text-sm font-medium bg-yellow-50/50 border border-yellow-100 p-6 rounded-[1.5rem] text-yellow-900 leading-relaxed">
                "{currentOrder.notes}"
              </p>
            </div>
          )}

          {/* Data do Pedido */}
          <div className="text-xs font-bold uppercase tracking-widest text-muted-foreground/60 text-center pt-4">
            Pedido realizado em {formatDate(currentOrder.createdAt)}
          </div>
        </CardContent>
      </Card>

      {/* Botões de Ação */}
      <div className="flex flex-col sm:flex-row gap-4 pt-4 px-0 md:px-0">
        <Button
          variant="outline"
          onClick={onBackToMenu}
          className="flex items-center justify-center gap-3 flex-1 h-16 rounded-2xl font-black uppercase italic tracking-widest order-2 sm:order-1 hover:bg-muted/50"
        >
          <ArrowLeft className="h-5 w-5 stroke-[3]" />
          Voltar ao Menu
        </Button>

        <Button
          onClick={onNewOrder}
          className="flex items-center justify-center gap-3 flex-1 bg-primary hover:bg-primary/90 h-16 rounded-2xl text-white font-black uppercase italic tracking-widest shadow-xl shadow-primary/20 order-1 sm:order-2"
        >
          <Package className="h-5 w-5 stroke-[3]" />
          Fazer Novo Pedido
        </Button>
      </div>
    </div >
  );
};

export default OrderConfirmation;
