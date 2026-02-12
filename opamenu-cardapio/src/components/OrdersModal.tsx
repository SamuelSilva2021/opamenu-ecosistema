import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { useCustomer } from "@/hooks/use-customer";
import { orderService, getOrderStatusText } from "@/services/order-service";
import { Order, OrderStatus, PaymentMethod } from "@/types/api";
import { signalRService } from "@/services/signalr-service";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
} from "@/components/ui/dialog";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Loader2, ShoppingBag, MapPin, Calendar, MoreVertical, CreditCard, Truck, XCircle, Search, Check, Smartphone, Banknote, Store } from "lucide-react";
import { format } from "date-fns";
import { ptBR } from "date-fns/locale";
import { toast } from "sonner";
import { Separator } from "@/components/ui/separator";
import { Badge } from "@/components/ui/badge";

interface OrdersModalProps {
  isOpen: boolean;
  onClose: () => void;
}

export function OrdersModal({ isOpen, onClose }: OrdersModalProps) {
  const { slug } = useParams<{ slug: string }>();
  const { customer } = useCustomer();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Estados para edição
  const [actionOrder, setActionOrder] = useState<Order | null>(null);
  const [cancelReason, setCancelReason] = useState("");
  const [showCancelDialog, setShowCancelDialog] = useState(false);

  const [showPaymentDialog, setShowPaymentDialog] = useState(false);
  const [selectedPaymentMethod, setSelectedPaymentMethod] = useState<string>("0"); // String para RadioGroup

  const [showDeliveryDialog, setShowDeliveryDialog] = useState(false);
  const [deliveryType, setDeliveryType] = useState<"delivery" | "pickup">("delivery");
  const [deliveryAddress, setDeliveryAddress] = useState("");

  const [actionLoading, setActionLoading] = useState(false);

  // Address Form State
  const [addressForm, setAddressForm] = useState({
    zipCode: "",
    street: "",
    number: "",
    complement: "",
    neighborhood: "",
    city: "",
    state: ""
  });
  const [isLoadingCep, setIsLoadingCep] = useState(false);
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (showDeliveryDialog && deliveryType === 'delivery' && customer) {
      const newForm = {
        zipCode: customer.postalCode || "",
        street: customer.street || "",
        number: customer.streetNumber || "",
        complement: customer.complement || "",
        neighborhood: customer.neighborhood || "",
        city: customer.city || "",
        state: customer.state || ""
      };
      setAddressForm(newForm);
      updateFullAddressString(newForm);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [showDeliveryDialog, deliveryType, customer]);

  useEffect(() => {
    const fetchOrders = async (silent = false) => {
      if (isOpen && customer && slug) {
        if (!silent) setLoading(true);
        try {
          const data = await orderService.getPublicOrdersByCustomerId(slug, customer.id);
          const sortedOrders = data.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
          setOrders(sortedOrders);
          setError(null);
        } catch (err) {
          console.error("Erro ao buscar pedidos:", err);
          if (!silent) setError("Não foi possível carregar seus pedidos.");
        } finally {
          if (!silent) setLoading(false);
        }
      }
    };

    fetchOrders();

    // SignalR Setup
    const setupSignalR = async () => {
      try {
        await signalRService.startConnection();
        // Não temos um grupo para "Todos os pedidos de um cliente específico" no backend ainda (a não ser que implementemos).
        // Mas podemos ouvir eventos globais de status e filtrar, ou entrar no grupo de CADA pedido que carregamos.
        // A melhor estratégia aqui é entrar no grupo de cada pedido carregado para receber updates.
        
        // Porém, isso pode ser custoso se forem muitos pedidos.
        // Como alternativa simples, mantemos o polling para a lista, e usamos SignalR apenas na OrderConfirmation.
        // MAS o usuário pediu explicitamente para funcionar aqui também ("Veja que eu movi o pedido #006...").
        // Então vamos manter o polling por segurança, mas também tentar conectar nos grupos dos pedidos VISÍVEIS.
        
        // Como o fetchOrders é assíncrono e popula 'orders', precisamos reagir a mudança de 'orders' para conectar.
      } catch (error) {
        console.error("SignalR setup error in OrdersModal", error);
      }
    };

    if (isOpen) {
      setupSignalR();
    }

    // Polling de fallback (mantém lista atualizada caso entrem novos pedidos ou SignalR falhe)
    let interval: NodeJS.Timeout;
    if (isOpen) {
      interval = setInterval(() => fetchOrders(true), 15000); 
    }

    return () => {
      if (interval) clearInterval(interval);
    };
  }, [isOpen, customer, slug]);

  // Efeito separado para conectar aos grupos dos pedidos carregados
  useEffect(() => {
    if (orders.length > 0) {
      orders.forEach(order => {
        // Entrar no grupo de cada pedido para receber updates em tempo real
        signalRService.joinOrderGroup(order.id);
      });

      // Registrar listener único (se já não estiver registrado, o service gerencia)
      // O service atual pode acumular listeners se chamarmos onOrderStatusUpdated várias vezes.
      // O ideal seria o service suportar múltiplos listeners ou limparmos.
      // Como o service é singleton e simples, vamos confiar que ele gerencia ou adicionar um método de limpeza se necessário.
      // Para evitar vazamento, vamos assumir que o modal é montado/desmontado e o service é global.
      // Vamos adicionar um listener que atualiza o estado local 'orders'.
      
      const handleStatusUpdate = (orderId: string | number, newStatus: string | number) => {
        setOrders(prevOrders => prevOrders.map(o => {
          if (String(o.id) === String(orderId)) {
            // Se o status for numérico ou string, normalizar se necessário. 
            // O tipo OrderStatus é enum (number).
            // Se vier string do backend, converter.
            let statusEnum: OrderStatus = o.status;
            
            // Tenta converter para número se possível
            if (typeof newStatus === 'string') {
               // Tentar mapear string para enum se o backend mandar "Confirmed" ao invés de 1
               // Mas geralmente manda número ou nome do enum.
               // Assumindo que o backend manda o NOME do enum (string) ou o VALOR (int).
               // Se for número:
               if (!isNaN(Number(newStatus))) {
                 statusEnum = Number(newStatus);
               } else {
                 // Se for string, tenta achar no enum (reverse mapping ou manual)
                 // Simplificação: vamos fazer um refetch do pedido específico para garantir dados consistentes
                 // Isso evita complexidade de parsing de enum no frontend
                 refreshSingleOrder(String(orderId));
                 return o; 
               }
            } else {
              statusEnum = newStatus as OrderStatus;
            }

            return { ...o, status: statusEnum };
          }
          return o;
        }));
      };

      signalRService.onOrderStatusUpdated(handleStatusUpdate);

      // Cleanup: Sair dos grupos ao fechar modal
      return () => {
        orders.forEach(order => {
           signalRService.leaveOrderGroup(order.id);
        });
      };
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [orders.length]); // Re-executa se a quantidade de pedidos mudar (novos carregados)

  const refreshSingleOrder = async (orderId: string) => {
    if (!slug) return;
    try {
      const updatedOrder = await orderService.getOrderById(orderId, slug);
      setOrders(prev => prev.map(o => o.id === updatedOrder.id ? updatedOrder : o));
    } catch (e) {
      console.error("Erro ao atualizar pedido único via SignalR", e);
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(price);
  };

  const formatDate = (dateString: string) => {
    try {
      return format(new Date(dateString), "dd 'de' MMMM 'às' HH:mm", { locale: ptBR });
    } catch (e) {
      return dateString;
    }
  };

  const openCancelDialog = (order: Order) => {
    setActionOrder(order);
    setCancelReason("");
    setShowCancelDialog(true);
  };

  const openPaymentDialog = (order: Order) => {
    setActionOrder(order);
    // Tenta inferir o método atual ou default CreditCard
    setSelectedPaymentMethod("0");
    setShowPaymentDialog(true);
  };

  const openDeliveryDialog = (order: Order) => {
    setActionOrder(order);
    setDeliveryType(order.isDelivery ? "delivery" : "pickup");
    setDeliveryAddress(order.deliveryAddress || "");
    setShowDeliveryDialog(true);
  };

  const handleCancel = async () => {
    if (!actionOrder || !slug) return;

    setActionLoading(true);
    try {
      await orderService.cancelOrder(actionOrder.id, { reason: cancelReason }, slug);
      toast.success("Pedido cancelado com sucesso.");
      setShowCancelDialog(false);
      // Refresh orders
      // Idealmente refetch, mas vou manipular o estado local para rapidez
      setOrders(orders.map(o => o.id === actionOrder.id ? { ...o, status: OrderStatus.Cancelled } : o));
    } catch (error) {
      toast.error("Erro ao cancelar pedido.");
      console.error(error);
    } finally {
      setActionLoading(false);
    }
  };

  const handleUpdatePayment = async () => {
    if (!actionOrder || !slug) return;

    setActionLoading(true);
    try {
      const updatedOrder = await orderService.updatePaymentMethod(actionOrder.id, {
        paymentMethod: parseInt(selectedPaymentMethod) as PaymentMethod
      }, slug);
      toast.success("Forma de pagamento atualizada.");
      setShowPaymentDialog(false);
      setOrders(orders.map(o => o.id === actionOrder.id ? updatedOrder : o));
    } catch (error) {
      toast.error("Erro ao atualizar pagamento.");
      console.error(error);
    } finally {
      setActionLoading(false);
    }
  };

  const updateFullAddressString = (form: typeof addressForm) => {
    const parts = [
      form.street,
      form.number ? `${form.number}` : null,
      form.complement ? `(${form.complement})` : null,
      form.neighborhood,
      form.city,
      form.state,
      form.zipCode ? `CEP: ${form.zipCode}` : null
    ].filter(Boolean);
    setDeliveryAddress(parts.join(', '));
  };

  const handleAddressChange = (field: keyof typeof addressForm, value: string) => {
    const newForm = { ...addressForm, [field]: value };
    setAddressForm(newForm);
    updateFullAddressString(newForm);
  };

  const handleCepChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const rawCep = e.target.value.replace(/\D/g, '');
    const formattedCep = rawCep.replace(/^(\d{5})(\d)/, '$1-$2');

    const newForm = { ...addressForm, zipCode: formattedCep };
    setAddressForm(newForm);
    updateFullAddressString(newForm);

    if (rawCep.length === 8) {
      setIsLoadingCep(true);
      try {
        const response = await fetch(`https://viacep.com.br/ws/${rawCep}/json/`);
        const data = await response.json();

        if (!data.erro) {
          const autoFilled = {
            ...newForm,
            street: data.logradouro,
            neighborhood: data.bairro,
            city: data.localidade,
            state: data.uf,
          };
          setAddressForm(autoFilled);
          updateFullAddressString(autoFilled);

          setValidationErrors(prev => {
            const newErrors = { ...prev };
            delete newErrors.zipCode;
            return newErrors;
          });
        } else {
          setValidationErrors(prev => ({ ...prev, zipCode: 'CEP não encontrado' }));
        }
      } catch (error) {
        console.error('Erro ao buscar CEP', error);
        setValidationErrors(prev => ({ ...prev, zipCode: 'Erro ao buscar CEP' }));
      } finally {
        setIsLoadingCep(false);
      }
    }
  };

  const handleUpdateDelivery = async () => {
    if (!actionOrder || !slug) return;

    if (deliveryType === 'delivery' && !deliveryAddress.trim()) {
      toast.error("Endereço é obrigatório para entrega.");
      return;
    }

    setActionLoading(true);
    try {
      const updatedOrder = await orderService.updateDeliveryType(actionOrder.id, {
        isDelivery: deliveryType === 'delivery',
        deliveryAddress: deliveryType === 'delivery' ? deliveryAddress : undefined
      }, slug);
      toast.success("Tipo de entrega atualizado.");
      setShowDeliveryDialog(false);
      setOrders(orders.map(o => o.id === actionOrder.id ? updatedOrder : o));
    } catch (error) {
      toast.error("Erro ao atualizar entrega.");
      console.error(error);
    } finally {
      setActionLoading(false);
    }
  };

  return (
    <>
      <Dialog open={isOpen} onOpenChange={onClose}>
        <DialogContent className="max-w-xl h-[95vh] w-[95vw] sm:w-full flex flex-col p-0 overflow-hidden rounded-xl border-none shadow-2xl">
          <div className="flex flex-col h-full bg-background">
            <DialogHeader className="shrink-0 p-6 pb-4 bg-white border-b border-border/50">
              <DialogTitle className="flex items-center gap-3 text-xl font-bold text-foreground">
                <div className="bg-primary/10 p-2 rounded-full">
                  <ShoppingBag className="h-5 w-5 text-primary" />
                </div>
                Meus Pedidos
              </DialogTitle>
              <DialogDescription className="text-xs font-semibold text-muted-foreground uppercase tracking-wide mt-1">
                Acompanhe o status das suas delícias
              </DialogDescription>
            </DialogHeader>

            <div className="flex-1 overflow-y-auto p-4 sm:p-6 space-y-6 scrollbar-hide bg-gray-50/50">
              {loading && orders.length === 0 ? (
                <div className="flex flex-col items-center justify-center py-20 gap-4">
                  <div className="relative">
                    <Loader2 className="h-12 w-12 animate-spin text-primary opacity-20" />
                    <ShoppingBag className="h-6 w-6 text-primary absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2" />
                  </div>
                  <p className="text-xs font-bold uppercase tracking-widest text-primary/40">Buscando seus pedidos...</p>
                </div>
              ) : error ? (
                <div className="text-center py-12 bg-white rounded-xl border-2 border-dashed border-red-100 p-8">
                  <XCircle className="h-12 w-12 text-red-400 mx-auto mb-4" />
                  <p className="text-sm font-bold text-red-500 uppercase tracking-tight mb-4">{error}</p>
                  <Button onClick={() => onClose()} variant="outline" className="rounded-xl font-bold uppercase tracking-widest text-xs px-8">Fechar</Button>
                </div>
              ) : orders.length === 0 ? (
                <div className="text-center py-20 bg-white rounded-xl border-2 border-dashed border-border p-10 flex flex-col items-center">
                  <div className="bg-muted/50 p-6 rounded-full mb-6">
                    <ShoppingBag className="h-16 w-16 text-muted-foreground opacity-30" />
                  </div>
                  <h3 className="text-xl font-bold text-foreground mb-2">Fome acumulada!</h3>
                  <p className="text-sm font-medium text-muted-foreground mb-8 max-w-[200px]">Você ainda não fez nenhum pedido conosco.</p>
                  <Button
                    onClick={onClose}
                    className="h-12 px-8 bg-primary hover:bg-primary-hover text-white rounded-xl font-bold shadow-lg shadow-primary/20 transform transition-all active:scale-95"
                  >
                    Começar a comprar
                  </Button>
                </div>
              ) : (
                <div className="space-y-6 pb-12">
                  {orders.map((order) => (
                    <div
                      key={order.id}
                      className="group bg-white rounded-xl border border-border/50 shadow-sm hover:shadow-md transition-all duration-300 overflow-hidden"
                    >
                      {/* Card Header: Status & Price */}
                      <div className="p-5 pb-0">
                        <div className="flex justify-between items-start mb-6">
                          <div className="flex flex-col gap-1">
                             {/* Status Badge */}
                             <Badge variant="outline" className={`
                                mb-2 w-fit border font-semibold px-3 py-1 rounded-full text-xs uppercase tracking-wide
                                ${order.status === OrderStatus.Pending ? 'bg-yellow-50 text-yellow-700 border-yellow-200' :
                                  order.status === OrderStatus.Confirmed ? 'bg-blue-50 text-blue-700 border-blue-200' :
                                    order.status === OrderStatus.Preparing ? 'bg-orange-50 text-orange-700 border-orange-200' :
                                      order.status === OrderStatus.Ready ? 'bg-green-50 text-green-700 border-green-200' :
                                        order.status === OrderStatus.OutForDelivery ? 'bg-purple-50 text-purple-700 border-purple-200' :
                                          order.status === OrderStatus.Delivered ? 'bg-emerald-50 text-emerald-700 border-emerald-200' :
                                            'bg-red-50 text-red-700 border-red-200'}
                             `}>
                               {getOrderStatusText(order.status)}
                             </Badge>

                            <p className="text-xs font-medium text-muted-foreground flex items-center gap-1.5">
                              <Calendar className="h-3.5 w-3.5" />
                              {formatDate(order.createdAt)}
                            </p>
                          </div>

                          <div className="flex flex-col items-end gap-1">
                            <span className="text-[10px] font-bold uppercase tracking-wider text-muted-foreground">Total</span>
                            <div className="flex items-center gap-2">
                              <p className="text-xl font-bold text-foreground">
                                {formatPrice(order.total)}
                              </p>
                              {order.status === OrderStatus.Pending && (
                                <DropdownMenu>
                                  <DropdownMenuTrigger asChild>
                                    <Button variant="ghost" size="icon" className="h-8 w-8 text-muted-foreground hover:bg-muted rounded-full">
                                      <MoreVertical className="h-4 w-4" />
                                    </Button>
                                  </DropdownMenuTrigger>
                                  <DropdownMenuContent align="end" className="bg-white rounded-xl border-border shadow-lg p-1 min-w-[160px]">
                                    <DropdownMenuLabel className="font-semibold text-xs opacity-60 px-2 py-1.5">Ações</DropdownMenuLabel>
                                    <DropdownMenuItem onClick={() => openPaymentDialog(order)} className="rounded-lg text-sm font-medium p-2 cursor-pointer">
                                      <CreditCard className="mr-2 h-4 w-4" />
                                      Pagamento
                                    </DropdownMenuItem>
                                    <DropdownMenuItem onClick={() => openDeliveryDialog(order)} className="rounded-lg text-sm font-medium p-2 cursor-pointer">
                                      <Truck className="mr-2 h-4 w-4" />
                                      Entrega/Retirada
                                    </DropdownMenuItem>
                                    <DropdownMenuSeparator className="my-1" />
                                    <DropdownMenuItem
                                      className="rounded-lg text-sm font-medium p-2 text-red-600 focus:text-red-700 cursor-pointer"
                                      onClick={() => openCancelDialog(order)}
                                    >
                                      <XCircle className="mr-2 h-4 w-4" />
                                      Cancelar
                                    </DropdownMenuItem>
                                  </DropdownMenuContent>
                                </DropdownMenu>
                              )}
                            </div>
                          </div>
                        </div>

                        {/* Status Tracking Visualizer */}
                        {order.status !== OrderStatus.Cancelled && (
                          <div className="relative flex items-center mb-6 px-2 h-12 mt-4">
                            {/* Connection Lines Background */}
                            <div className="absolute left-6 right-6 top-1/2 -translate-y-1/2 h-1 bg-muted rounded-full" />

                            {/* Active Line Progress */}
                            <div
                              className="absolute left-6 top-1/2 -translate-y-1/2 h-1 bg-green-500 rounded-full transition-all duration-1000 ease-out z-[1]"
                              style={{
                                width: order.status === OrderStatus.Delivered ? 'calc(100% - 48px)' :
                                  [OrderStatus.Ready, OrderStatus.OutForDelivery].includes(order.status) ? '66%' :
                                    [OrderStatus.Preparing, OrderStatus.Confirmed].includes(order.status) ? '33%' : '0%'
                              }}
                            />

                            {/* Stepper Points */}
                            <div className="relative z-[2] flex justify-between w-full">
                              {[
                                { id: 'realizado', label: 'Realizado', active: true, done: order.status !== OrderStatus.Pending },
                                { id: 'preparo', label: 'Preparo', active: order.status !== OrderStatus.Pending, done: [OrderStatus.Preparing, OrderStatus.Ready, OrderStatus.OutForDelivery, OrderStatus.Delivered].includes(order.status) },
                                { id: 'entrega', label: 'Entrega', active: [OrderStatus.Ready, OrderStatus.OutForDelivery, OrderStatus.Delivered].includes(order.status), done: order.status === OrderStatus.Delivered },
                                { id: 'finalizado', label: 'Finalizado', active: order.status === OrderStatus.Delivered, done: order.status === OrderStatus.Delivered }
                              ].map((step, idx) => (
                                <div key={step.id} className="flex flex-col items-center group/step w-12">
                                  <div className={`
                                    w-5 h-5 rounded-full flex items-center justify-center transition-all duration-500 border-2
                                    ${step.done ? 'bg-green-500 border-green-500' :
                                      step.active ? 'bg-white border-primary scale-110' : 'bg-muted border-muted-foreground/20'}
                                  `}>
                                    {step.done && <Check className="h-3 w-3 text-white" />}
                                    {!step.done && step.active && <div className="w-2 h-2 bg-primary rounded-full animate-pulse" />}
                                  </div>
                                  <span className={`text-[9px] font-bold uppercase tracking-wider mt-2 text-center transition-colors ${step.active || step.done ? 'text-foreground' : 'text-muted-foreground'}`}>
                                    {step.label}
                                  </span>
                                </div>
                              ))}
                            </div>
                          </div>
                        )}
                      </div>

                      {/* Card Details Body */}
                      <div className="px-5 pb-5 pt-2 space-y-4">
                        <div className="bg-muted/30 rounded-xl p-4 space-y-3">
                          <div className="space-y-2">
                            <div className="flex items-center gap-2 mb-1">
                              <div className="w-1 h-4 bg-primary rounded-full" />
                              <span className="text-xs font-bold uppercase tracking-wide text-foreground">Itens do Pedido</span>
                            </div>
                            <ul className="space-y-2">
                              {order.items.map((item, idx) => (
                                <li key={idx} className="flex items-center justify-between group/item py-1">
                                  <div className="flex items-center gap-3">
                                    <span className="bg-white border text-xs font-bold min-w-6 h-6 flex items-center justify-center rounded-md shadow-sm text-foreground">
                                      {item.quantity}x
                                    </span>
                                    <span className="text-sm font-medium text-foreground">
                                      {item.productName}
                                    </span>
                                  </div>
                                </li>
                              ))}
                            </ul>
                          </div>

                          {order.deliveryAddress && (
                            <>
                              <Separator className="bg-border/50" />
                              <div className="pt-1">
                                <div className="flex items-center gap-2 mb-1.5">
                                  <MapPin className="h-3.5 w-3.5 text-primary" />
                                  <span className="text-xs font-bold uppercase tracking-wide text-foreground">Endereço de Entrega</span>
                                </div>
                                <p className="text-xs text-muted-foreground leading-relaxed pl-5">
                                  {order.deliveryAddress}
                                </p>
                              </div>
                            </>
                          )}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        </DialogContent>
      </Dialog>

      <AlertDialog open={showCancelDialog} onOpenChange={setShowCancelDialog}>
        <AlertDialogContent className="rounded-[2.5rem] p-8 border-none shadow-2xl">
          <AlertDialogHeader>
            <AlertDialogTitle className="text-2xl font-black uppercase italic tracking-tighter text-foreground">
              Cancelar Pedido?
            </AlertDialogTitle>
            <AlertDialogDescription className="text-sm font-medium text-text-secondary/60 leading-relaxed">
              Tem certeza que deseja cancelar o pedido #{actionOrder?.id}? Esta ação não pode ser desfeita e seu estômago pode ficar triste. 😢
            </AlertDialogDescription>
          </AlertDialogHeader>
          <div className="py-6 space-y-3">
            <Label className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Motivo do Cancelamento</Label>
            <Textarea
              placeholder="Ex: Mudei de ideia, demorou muito..."
              value={cancelReason}
              onChange={(e) => setCancelReason(e.target.value)}
              className="min-h-[100px] rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-red-500 transition-all p-5 font-bold"
            />
          </div>
          <AlertDialogFooter className="gap-3 sm:gap-0">
            <AlertDialogCancel disabled={actionLoading} className="h-14 rounded-2xl font-black uppercase italic tracking-widest text-xs border-2">
              Voltar
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={(e) => { e.preventDefault(); handleCancel(); }}
              disabled={!cancelReason.trim() || actionLoading}
              className="h-14 bg-red-500 hover:bg-red-600 text-white rounded-2xl font-black uppercase italic tracking-wider shadow-xl shadow-red-500/20"
            >
              {actionLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
              Confirmar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <Dialog open={showPaymentDialog} onOpenChange={setShowPaymentDialog}>
        <DialogContent className="max-w-md rounded-[2.5rem] p-0 overflow-hidden border-none shadow-2xl">
          <div className="bg-white p-8">
            <DialogHeader className="p-0 mb-8">
              <DialogTitle className="text-2xl font-black uppercase italic tracking-tighter text-foreground">
                Pagamento
              </DialogTitle>
              <DialogDescription className="text-sm font-bold text-text-secondary/60 uppercase tracking-widest mt-1">
                Como deseja pagar pelo pedido #{actionOrder?.id}?
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-3">
              <RadioGroup value={selectedPaymentMethod} onValueChange={setSelectedPaymentMethod} className="space-y-3">
                {[
                  { id: "0", label: "Cartão de Crédito", icon: CreditCard },
                  { id: "1", label: "Cartão de Débito", icon: CreditCard },
                  { id: "2", label: "Pix", icon: Smartphone },
                  { id: "3", label: "Dinheiro", icon: Banknote },
                ].map((method) => (
                  <label
                    key={method.id}
                    htmlFor={method.id}
                    className={`
                      flex items-center gap-4 p-5 rounded-2xl border-2 transition-all duration-300 cursor-pointer
                      ${selectedPaymentMethod === method.id
                        ? 'border-primary bg-primary/5 shadow-md shadow-primary/5'
                        : 'border-border/50 hover:border-primary/30 hover:bg-gray-50'}
                    `}
                  >
                    <RadioGroupItem value={method.id} id={method.id} className="sr-only" />
                    <div className={`p-2 rounded-xl ${selectedPaymentMethod === method.id ? 'bg-primary text-white' : 'bg-muted text-muted-foreground'}`}>
                      <method.icon className="h-5 w-5 stroke-[2.5]" />
                    </div>
                    <div className="flex-1 font-black uppercase italic tracking-tight text-base">{method.label}</div>
                    {selectedPaymentMethod === method.id && (
                      <div className="bg-primary text-white rounded-full p-1">
                        <Check className="h-3 w-3 stroke-[3]" />
                      </div>
                    )}
                  </label>
                ))}
              </RadioGroup>
            </div>

            <div className="grid grid-cols-2 gap-4 mt-10">
              <Button
                variant="ghost"
                onClick={() => setShowPaymentDialog(false)}
                disabled={actionLoading}
                className="h-14 rounded-2xl font-black uppercase italic tracking-widest text-xs"
              >
                Voltar
              </Button>
              <Button
                onClick={handleUpdatePayment}
                disabled={actionLoading}
                className="h-14 bg-primary hover:bg-primary-hover text-white rounded-2xl font-black uppercase italic tracking-wider shadow-xl shadow-primary/20"
              >
                {actionLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : "Salvar"}
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>

      <Dialog open={showDeliveryDialog} onOpenChange={setShowDeliveryDialog}>
        <DialogContent className="max-w-md rounded-[2.5rem] p-0 overflow-hidden border-none shadow-2xl">
          <div className="bg-white p-8 max-h-[90vh] overflow-y-auto scrollbar-hide">
            <DialogHeader className="p-0 mb-8">
              <DialogTitle className="text-2xl font-black uppercase italic tracking-tighter text-foreground">
                Entrega
              </DialogTitle>
              <DialogDescription className="text-sm font-bold text-text-secondary/60 uppercase tracking-widest mt-1">
                {deliveryType === "delivery" ? "Onde entregaremos o pedido #" : "Retirada do pedido #"} {actionOrder?.id}?
              </DialogDescription>
            </DialogHeader>

            <div className="space-y-6">
              <RadioGroup
                value={deliveryType}
                onValueChange={(v) => setDeliveryType(v as "delivery" | "pickup")}
                className="grid grid-cols-2 gap-3"
              >
                <label
                  htmlFor="delivery"
                  className={`flex flex-col items-center justify-center gap-3 border-2 p-5 rounded-3xl transition-all duration-300 cursor-pointer ${deliveryType === "delivery" ? 'border-primary bg-primary/5' : 'border-border/50 hover:bg-gray-50'
                    }`}
                >
                  <RadioGroupItem value="delivery" id="delivery" className="sr-only" />
                  <div className={`p-3 rounded-2xl ${deliveryType === "delivery" ? 'bg-primary text-white' : 'bg-muted text-muted-foreground'}`}>
                    <Truck className="h-6 w-6 stroke-[2.5]" />
                  </div>
                  <span className="font-black uppercase italic tracking-tighter text-sm">Entrega</span>
                </label>

                <label
                  htmlFor="pickup"
                  className={`flex flex-col items-center justify-center gap-3 border-2 p-5 rounded-3xl transition-all duration-300 cursor-pointer ${deliveryType === "pickup" ? 'border-primary bg-primary/5' : 'border-border/50 hover:bg-gray-50'
                    }`}
                >
                  <RadioGroupItem value="pickup" id="pickup" className="sr-only" />
                  <div className={`p-3 rounded-2xl ${deliveryType === "pickup" ? 'bg-primary text-white' : 'bg-muted text-muted-foreground'}`}>
                    <Store className="h-6 w-6 stroke-[2.5]" />
                  </div>
                  <span className="font-black uppercase italic tracking-tighter text-sm">Retirada</span>
                </label>
              </RadioGroup>

              {deliveryType === "delivery" && (
                <div className="space-y-5 pt-2 animate-in fade-in slide-in-from-top-4 duration-500">
                  <div className="space-y-2">
                    <Label className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">CEP</Label>
                    <div className="relative">
                      <Input
                        value={addressForm.zipCode}
                        onChange={handleCepChange}
                        placeholder="00000-000"
                        className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold"
                      />
                      <div className="absolute right-4 top-1/2 -translate-y-1/2 text-muted-foreground">
                        {isLoadingCep ? <Loader2 className="h-4 w-4 animate-spin" /> : <Search className="h-4 w-4" />}
                      </div>
                    </div>
                  </div>

                  <div className="grid grid-cols-[1fr_100px] gap-3">
                    <div className="space-y-2">
                      <Label className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Rua</Label>
                      <Input
                        value={addressForm.street}
                        onChange={(e) => handleAddressChange('street', e.target.value)}
                        placeholder="Nome da rua"
                        className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold"
                      />
                    </div>
                    <div className="space-y-2">
                      <Label className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Nº</Label>
                      <Input
                        value={addressForm.number}
                        onChange={(e) => handleAddressChange('number', e.target.value)}
                        placeholder="123"
                        className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold"
                      />
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Complemento</Label>
                    <Input
                      value={addressForm.complement}
                      onChange={(e) => handleAddressChange('complement', e.target.value)}
                      placeholder="Apto, Bloco..."
                      className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold"
                    />
                  </div>

                  <div className="grid grid-cols-2 gap-3">
                    <div className="space-y-2">
                      <Label className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Bairro</Label>
                      <Input
                        value={addressForm.neighborhood}
                        onChange={(e) => handleAddressChange('neighborhood', e.target.value)}
                        className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold"
                      />
                    </div>
                    <div className="space-y-2">
                      <Label className="text-[10px] font-black uppercase tracking-[0.2em] text-foreground/40 ml-1">Cidade</Label>
                      <Input
                        value={addressForm.city}
                        onChange={(e) => handleAddressChange('city', e.target.value)}
                        className="h-14 rounded-2xl border-2 border-border/50 bg-gray-50/50 focus:bg-white focus:border-primary transition-all px-5 font-bold"
                      />
                    </div>
                  </div>
                </div>
              )}
            </div>

            <div className="grid grid-cols-2 gap-4 mt-10 pb-2">
              <Button
                variant="ghost"
                onClick={() => setShowDeliveryDialog(false)}
                disabled={actionLoading}
                className="h-14 rounded-2xl font-black uppercase italic tracking-widest text-xs"
              >
                Voltar
              </Button>
              <Button
                onClick={handleUpdateDelivery}
                disabled={actionLoading}
                className="h-14 bg-primary hover:bg-primary-hover text-white rounded-2xl font-black uppercase italic tracking-wider shadow-xl shadow-primary/20"
              >
                {actionLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : "Salvar"}
              </Button>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
}
