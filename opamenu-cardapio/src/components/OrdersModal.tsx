import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { useCustomer } from "@/hooks/use-customer";
import { orderService, getOrderStatusText } from "@/services/order-service";
import { Order, OrderStatus, PaymentMethod } from "@/types/api";
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

    let interval: NodeJS.Timeout;
    if (isOpen) {
      interval = setInterval(() => fetchOrders(true), 15000); // Poll a cada 15 segundos
    }

    return () => {
      if (interval) clearInterval(interval);
    };
  }, [isOpen, customer, slug]);

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
        <DialogContent className="sm:max-w-[600px] max-h-[90vh] overflow-y-auto bg-white p-0 gap-0">
          <DialogHeader className="p-6 pb-2 border-b sticky top-0 bg-white z-10">
            <DialogTitle className="flex items-center gap-2 text-xl font-bold">
              <ShoppingBag className="h-5 w-5 text-primary" />
              Meus Pedidos
            </DialogTitle>
          </DialogHeader>

          <div className="p-6 space-y-4">
            {loading ? (
              <div className="flex flex-col items-center justify-center py-12 gap-4">
                <Loader2 className="h-8 w-8 animate-spin text-primary" />
                <p className="text-gray-500">Carregando seus pedidos...</p>
              </div>
            ) : error ? (
              <div className="text-center py-8">
                <p className="text-red-500 mb-4">{error}</p>
                <Button onClick={() => onClose()} variant="outline">Fechar</Button>
              </div>
            ) : orders.length === 0 ? (
              <div className="text-center py-12 text-gray-500">
                <ShoppingBag className="h-12 w-12 mx-auto mb-4 text-gray-300" />
                <p>Você ainda não fez nenhum pedido.</p>
                <Button
                  variant="link"
                  className="mt-2 text-primary"
                  onClick={onClose}
                >
                  Começar a comprar
                </Button>
              </div>
            ) : (
              <div className="space-y-4">
                {orders.map((order) => (
                  <div
                    key={order.id}
                    className="border rounded-lg p-4 hover:bg-gray-50 transition-colors"
                  >
                    <div className="flex justify-between items-start mb-4">
                      <div className="flex-1">
                        <div className="flex items-center justify-between mb-4">
                          <span className={`text-xs font-bold px-2.5 py-1 rounded-full ${order.status === OrderStatus.Pending ? 'bg-yellow-100 text-yellow-800' :
                              order.status === OrderStatus.Confirmed ? 'bg-blue-100 text-blue-800' :
                                order.status === OrderStatus.OutForDelivery ? 'bg-orange-100 text-orange-800' :
                                  order.status === OrderStatus.Delivered ? 'bg-green-100 text-green-800' :
                                    order.status === OrderStatus.Cancelled ? 'bg-red-100 text-red-800' :
                                      'bg-gray-100 text-gray-800'
                            }`}>
                            {getOrderStatusText(order.status)}
                          </span>
                          <div className="flex items-center gap-2">
                            <p className="font-bold text-lg text-primary">
                              {formatPrice(order.total)}
                            </p>
                            {order.status === OrderStatus.Pending && (
                              <DropdownMenu>
                                <DropdownMenuTrigger asChild>
                                  <Button variant="ghost" size="icon" className="h-8 w-8">
                                    <MoreVertical className="h-4 w-4" />
                                  </Button>
                                </DropdownMenuTrigger>
                                <DropdownMenuContent align="end" className="bg-white">
                                  <DropdownMenuLabel>Ações do Pedido</DropdownMenuLabel>
                                  <DropdownMenuSeparator />
                                  <DropdownMenuItem onClick={() => openPaymentDialog(order)}>
                                    <CreditCard className="mr-2 h-4 w-4" />
                                    Alterar Pagamento
                                  </DropdownMenuItem>
                                  <DropdownMenuItem onClick={() => openDeliveryDialog(order)}>
                                    <Truck className="mr-2 h-4 w-4" />
                                    Entrega/Retirada
                                  </DropdownMenuItem>
                                  <DropdownMenuSeparator />
                                  <DropdownMenuItem
                                    className="text-red-600 focus:text-red-600"
                                    onClick={() => openCancelDialog(order)}
                                  >
                                    <XCircle className="mr-2 h-4 w-4" />
                                    Cancelar Pedido
                                  </DropdownMenuItem>
                                </DropdownMenuContent>
                              </DropdownMenu>
                            )}
                          </div>
                        </div>

                        {/* Status Stepper */}
                        {order.status !== OrderStatus.Cancelled && (
                          <div className="flex items-center mb-4 px-2">
                            <div className="flex flex-col items-center flex-1">
                              <div className={`h-2 w-2 rounded-full mb-1 ${order.status !== OrderStatus.Pending ? 'bg-green-500' : 'bg-gray-300 animate-pulse'}`} />
                              <span className="text-[10px] text-gray-500">Realizado</span>
                            </div>
                            <div className={`h-0.5 flex-1 mx-1 ${order.status !== OrderStatus.Pending ? 'bg-green-500' : 'bg-gray-200'}`} />
                            <div className="flex flex-col items-center flex-1">
                              <div className={`h-2 w-2 rounded-full mb-1 ${[OrderStatus.Preparing, OrderStatus.Ready, OrderStatus.OutForDelivery, OrderStatus.Delivered].includes(order.status) ? 'bg-green-500' :
                                  order.status === OrderStatus.Confirmed ? 'bg-blue-500 animate-pulse' : 'bg-gray-300'
                                }`} />
                              <span className="text-[10px] text-gray-500">Preparo</span>
                            </div>
                            <div className={`h-0.5 flex-1 mx-1 ${[OrderStatus.Preparing, OrderStatus.Ready, OrderStatus.OutForDelivery, OrderStatus.Delivered].includes(order.status) ? 'bg-green-500' : 'bg-gray-200'}`} />
                            <div className="flex flex-col items-center flex-1">
                              <div className={`h-2 w-2 rounded-full mb-1 ${order.status === OrderStatus.Delivered ? 'bg-green-500' :
                                  [OrderStatus.Ready, OrderStatus.OutForDelivery].includes(order.status) ? 'bg-orange-500 animate-pulse' : 'bg-gray-300'
                                }`} />
                              <span className="text-[10px] text-gray-500">Entrega</span>
                            </div>
                            <div className={`h-0.5 flex-1 mx-1 ${order.status === OrderStatus.Delivered ? 'bg-green-500' : 'bg-gray-200'}`} />
                            <div className="flex flex-col items-center flex-1">
                              <div className={`h-2 w-2 rounded-full mb-1 ${order.status === OrderStatus.Delivered ? 'bg-green-500' : 'bg-gray-300'}`} />
                              <span className="text-[10px] text-gray-500">Finalizado</span>
                            </div>
                          </div>
                        )}

                        <p className="text-xs text-gray-500 flex items-center gap-1">
                          <Calendar className="h-3 w-3" />
                          {formatDate(order.createdAt)}
                        </p>
                      </div>
                    </div>

                    <div className="border-t pt-3 mt-3">
                      <p className="text-sm font-medium text-gray-700 mb-2">Itens:</p>
                      <ul className="text-sm text-gray-600 space-y-1">
                        {order.items.map((item, idx) => (
                          <li key={idx} className="flex justify-between">
                            <span>{item.quantity}x {item.productName}</span>
                          </li>
                        ))}
                      </ul>
                    </div>

                    {order.deliveryAddress && (
                      <div className="border-t pt-3 mt-3 flex items-start gap-2 text-sm text-gray-500">
                        <MapPin className="h-4 w-4 shrink-0 mt-0.5" />
                        <span>{order.deliveryAddress}</span>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        </DialogContent>
      </Dialog>

      <AlertDialog open={showCancelDialog} onOpenChange={setShowCancelDialog}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Cancelar Pedido #{actionOrder?.id}</AlertDialogTitle>
            <AlertDialogDescription>
              Tem certeza que deseja cancelar este pedido? Esta ação não pode ser desfeita.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <div className="py-4">
            <Label htmlFor="reason" className="mb-2 block">Motivo do Cancelamento</Label>
            <Textarea
              id="reason"
              placeholder="Ex: Mudei de ideia, demorou muito..."
              value={cancelReason}
              onChange={(e) => setCancelReason(e.target.value)}
            />
          </div>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={actionLoading}>Voltar</AlertDialogCancel>
            <AlertDialogAction
              onClick={(e) => { e.preventDefault(); handleCancel(); }}
              disabled={!cancelReason.trim() || actionLoading}
              className="bg-red-600 hover:bg-red-700"
            >
              {actionLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
              Confirmar Cancelamento
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      <Dialog open={showPaymentDialog} onOpenChange={setShowPaymentDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Alterar Forma de Pagamento</DialogTitle>
            <DialogDescription>
              Selecione a nova forma de pagamento para o pedido #{actionOrder?.id}.
            </DialogDescription>
          </DialogHeader>
          <div className="py-4">
            <RadioGroup value={selectedPaymentMethod} onValueChange={setSelectedPaymentMethod} className="space-y-3">
              <label
                htmlFor="credit"
                className={`flex items-center gap-3 border p-3 rounded-lg transition-colors cursor-pointer ${selectedPaymentMethod === "0" ? 'border-opamenu-orange bg-opamenu-orange/5' : 'border-border hover:bg-gray-50'
                  }`}
              >
                <RadioGroupItem value="0" id="credit" className="sr-only" />
                <CreditCard className={`h-5 w-5 ${selectedPaymentMethod === "0" ? 'text-opamenu-orange' : 'text-muted-foreground'}`} />
                <div className="flex-1 font-medium text-base">Cartão de Crédito</div>
                {selectedPaymentMethod === "0" && <Check className="h-5 w-5 text-opamenu-orange" />}
              </label>

              <label
                htmlFor="debit"
                className={`flex items-center gap-3 border p-3 rounded-lg transition-colors cursor-pointer ${selectedPaymentMethod === "1" ? 'border-opamenu-orange bg-opamenu-orange/5' : 'border-border hover:bg-gray-50'
                  }`}
              >
                <RadioGroupItem value="1" id="debit" className="sr-only" />
                <CreditCard className={`h-5 w-5 ${selectedPaymentMethod === "1" ? 'text-opamenu-orange' : 'text-muted-foreground'}`} />
                <div className="flex-1 font-medium text-base">Cartão de Débito</div>
                {selectedPaymentMethod === "1" && <Check className="h-5 w-5 text-opamenu-orange" />}
              </label>

              <label
                htmlFor="pix"
                className={`flex items-center gap-3 border p-3 rounded-lg transition-colors cursor-pointer ${selectedPaymentMethod === "2" ? 'border-opamenu-orange bg-opamenu-orange/5' : 'border-border hover:bg-gray-50'
                  }`}
              >
                <RadioGroupItem value="2" id="pix" className="sr-only" />
                <Smartphone className={`h-5 w-5 ${selectedPaymentMethod === "2" ? 'text-opamenu-orange' : 'text-muted-foreground'}`} />
                <div className="flex-1 font-medium text-base">Pix</div>
                {selectedPaymentMethod === "2" && <Check className="h-5 w-5 text-opamenu-orange" />}
              </label>

              <label
                htmlFor="cash"
                className={`flex items-center gap-3 border p-3 rounded-lg transition-colors cursor-pointer ${selectedPaymentMethod === "3" ? 'border-opamenu-orange bg-opamenu-orange/5' : 'border-border hover:bg-gray-50'
                  }`}
              >
                <RadioGroupItem value="3" id="cash" className="sr-only" />
                <Banknote className={`h-5 w-5 ${selectedPaymentMethod === "3" ? 'text-opamenu-orange' : 'text-muted-foreground'}`} />
                <div className="flex-1 font-medium text-base">Dinheiro</div>
                {selectedPaymentMethod === "3" && <Check className="h-5 w-5 text-opamenu-orange" />}
              </label>
            </RadioGroup>
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowPaymentDialog(false)} disabled={actionLoading}>
              Cancelar
            </Button>
            <Button onClick={handleUpdatePayment} disabled={actionLoading}>
              {actionLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
              Salvar Alteração
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={showDeliveryDialog} onOpenChange={setShowDeliveryDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Alterar Tipo de Entrega</DialogTitle>
            <DialogDescription>
              Escolha entre Entrega ou Retirada.
            </DialogDescription>
          </DialogHeader>
          <div className="py-4 space-y-4">
            <RadioGroup
              value={deliveryType}
              onValueChange={(v) => setDeliveryType(v as "delivery" | "pickup")}
              className="flex gap-4"
            >
              <label
                htmlFor="delivery"
                className={`flex-1 flex items-center gap-3 border p-3 rounded-lg transition-colors cursor-pointer ${deliveryType === "delivery" ? 'border-opamenu-orange bg-opamenu-orange/5' : 'border-border hover:bg-gray-50'
                  }`}
              >
                <RadioGroupItem value="delivery" id="delivery" className="sr-only" />
                <div className="flex items-center gap-2 flex-1">
                  <Truck className={`h-5 w-5 ${deliveryType === "delivery" ? 'text-opamenu-orange' : 'text-muted-foreground'}`} />
                  <span className="font-medium text-base">Entrega</span>
                </div>
                {deliveryType === "delivery" && <Check className="h-5 w-5 text-opamenu-orange" />}
              </label>

              <label
                htmlFor="pickup"
                className={`flex-1 flex items-center gap-3 border p-3 rounded-lg transition-colors cursor-pointer ${deliveryType === "pickup" ? 'border-opamenu-green bg-opamenu-green/5' : 'border-border hover:bg-gray-50'
                  }`}
              >
                <RadioGroupItem value="pickup" id="pickup" className="sr-only" />
                <div className="flex items-center gap-2 flex-1">
                  <Store className={`h-5 w-5 ${deliveryType === "pickup" ? 'text-opamenu-green' : 'text-muted-foreground'}`} />
                  <span className="font-medium text-base">Retirada</span>
                </div>
                {deliveryType === "pickup" && <Check className="h-5 w-5 text-opamenu-green" />}
              </label>
            </RadioGroup>

            {deliveryType === "delivery" && (
              <div className="space-y-4 pt-2">
                <div className="flex items-center gap-2">
                  <MapPin className="h-4 w-4 text-primary" />
                  <h4 className="font-semibold text-sm">Endereço de Entrega</h4>
                </div>

                {/* CEP */}
                <div className="space-y-2">
                  <Label htmlFor="zipCode">CEP</Label>
                  <div className="relative">
                    <Input
                      id="zipCode"
                      value={addressForm.zipCode}
                      onChange={handleCepChange}
                      placeholder="00000-000"
                      maxLength={9}
                      className={validationErrors.zipCode ? 'border-destructive pr-10' : 'pr-10'}
                    />
                    <div className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground">
                      {isLoadingCep ? (
                        <Loader2 className="h-4 w-4 animate-spin" />
                      ) : (
                        <Search className="h-4 w-4" />
                      )}
                    </div>
                  </div>
                  {validationErrors.zipCode && (
                    <p className="text-xs text-destructive">{validationErrors.zipCode}</p>
                  )}
                </div>

                {/* Rua e Número */}
                <div className="grid grid-cols-[1fr_80px] gap-3">
                  <div className="space-y-2">
                    <Label htmlFor="street">Rua</Label>
                    <Input
                      id="street"
                      value={addressForm.street}
                      onChange={(e) => handleAddressChange('street', e.target.value)}
                      placeholder="Nome da rua"
                      disabled={isLoadingCep}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="number">Número</Label>
                    <Input
                      id="number"
                      value={addressForm.number}
                      onChange={(e) => handleAddressChange('number', e.target.value)}
                      placeholder="123"
                      disabled={isLoadingCep}
                    />
                  </div>
                </div>

                {/* Complemento */}
                <div className="space-y-2">
                  <Label htmlFor="complement">Complemento (opcional)</Label>
                  <Input
                    id="complement"
                    value={addressForm.complement}
                    onChange={(e) => handleAddressChange('complement', e.target.value)}
                    placeholder="Apto 101"
                    disabled={isLoadingCep}
                  />
                </div>

                {/* Bairro, Cidade, UF */}
                <div className="grid grid-cols-[1fr_1fr_60px] gap-3">
                  <div className="space-y-2">
                    <Label htmlFor="neighborhood">Bairro</Label>
                    <Input
                      id="neighborhood"
                      value={addressForm.neighborhood}
                      onChange={(e) => handleAddressChange('neighborhood', e.target.value)}
                      placeholder="Bairro"
                      disabled={isLoadingCep}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="city">Cidade</Label>
                    <Input
                      id="city"
                      value={addressForm.city}
                      onChange={(e) => handleAddressChange('city', e.target.value)}
                      placeholder="Cidade"
                      disabled={isLoadingCep}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="state">UF</Label>
                    <Input
                      id="state"
                      value={addressForm.state}
                      onChange={(e) => handleAddressChange('state', e.target.value)}
                      placeholder="UF"
                      maxLength={2}
                      disabled={isLoadingCep}
                    />
                  </div>
                </div>
              </div>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowDeliveryDialog(false)} disabled={actionLoading}>
              Cancelar
            </Button>
            <Button onClick={handleUpdateDelivery} disabled={actionLoading}>
              {actionLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
              Salvar Alteração
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}
