import { useState } from "react";
import { usePOSStore } from "../store/pos.store";
import { posService } from "../pos.service";
import { CustomerSelector } from "./CustomerSelector";
import { OrderTypeSelector } from "./OrderTypeSelector";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Trash2, Plus, Minus, CreditCard } from "lucide-react";
import { OrderType } from "../types";
import { useToast } from "@/hooks/use-toast";
import { Label } from "@/components/ui/label";

export function CartSidebar() {
    const { 
        items, customer, orderType, deliveryFee, couponCode, tableId,
        removeItem, updateItemQuantity, setCustomer, setOrderType, setDeliveryFee, setTableId, clearCart,
        getSubtotal, getTotal 
    } = usePOSStore();
    
    const { toast } = useToast();
    const [isLoading, setIsLoading] = useState(false);
    const [tempCustomerName, setTempCustomerName] = useState("");
    const [tempCustomerPhone, setTempCustomerPhone] = useState("");
    const [tempCustomerAddress, setTempCustomerAddress] = useState<any>(null);

    const handleCheckout = async () => {
        if (items.length === 0) {
            toast({ title: "Carrinho vazio", description: "Adicione itens ao carrinho para finalizar.", variant: "destructive" });
            return;
        }

        if (orderType === OrderType.Table && !tableId) {
             toast({ title: "Mesa obrigatória", description: "Informe o número da mesa.", variant: "destructive" });
             return;
        }
        
        setIsLoading(true);
        try {
            await posService.createOrder(
                items, 
                customer, 
                orderType, 
                deliveryFee, 
                couponCode, 
                tableId,
                orderType === OrderType.Delivery ? tempCustomerAddress : undefined,
                (!customer && tempCustomerName) ? { name: tempCustomerName, phone: tempCustomerPhone || "99999999999" } : undefined
            );
            
            toast({ title: "Pedido criado com sucesso!" });
            clearCart();
            setTempCustomerName("");
            setTempCustomerPhone("");
            setTempCustomerAddress(null);
        } catch (error) {
            console.error(error);
            toast({ title: "Erro ao criar pedido", description: "Tente novamente.", variant: "destructive" });
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="flex flex-col h-full bg-white dark:bg-zinc-950 border-l shadow-xl z-10">
            <div className="p-5 space-y-4 border-b bg-muted/5">
                <div className="flex items-center justify-between">
                    <h2 className="font-bold text-xl text-primary">Novo Pedido</h2>
                    <div className="text-xs font-medium px-2 py-1 bg-primary/10 text-primary rounded-full">
                        #{Math.floor(Math.random() * 1000).toString().padStart(4, '0')}
                    </div>
                </div>
                
                <OrderTypeSelector value={orderType} onChange={setOrderType} />
                
                {orderType === OrderType.Table && (
                    <div className="space-y-2">
                        <Label>Mesa</Label>
                        <Input 
                            placeholder="Número da mesa" 
                            value={tableId || ""} 
                            onChange={(e) => setTableId(e.target.value)} 
                            className="bg-background"
                        />
                    </div>
                )}
                
                <CustomerSelector 
                    selectedCustomer={customer} 
                    onSelect={setCustomer}
                    tempCustomerName={tempCustomerName}
                    onTempCustomerNameChange={setTempCustomerName}
                    tempCustomerPhone={tempCustomerPhone}
                    onTempCustomerPhoneChange={setTempCustomerPhone}
                    tempCustomerAddress={tempCustomerAddress}
                    onTempCustomerAddressChange={setTempCustomerAddress}
                    orderType={orderType}
                />
            </div>

            <div className="flex-1 p-4 bg-muted/5 overflow-y-auto custom-scrollbar">
                {items.length === 0 ? (
                    <div className="flex flex-col items-center justify-center h-40 text-muted-foreground text-sm border-2 border-dashed border-muted-foreground/20 rounded-lg m-2">
                        <CreditCard className="h-8 w-8 mb-2 opacity-50" />
                        Carrinho vazio
                    </div>
                ) : (
                    <div className="space-y-3">
                        {items.map((item) => (
                            <div key={item.tempId} className="flex gap-3 p-3 bg-background rounded-lg border shadow-sm group hover:border-primary/50 transition-colors">
                                <div className="flex-1">
                                    <div className="flex justify-between items-start mb-2">
                                        <span className="font-medium text-sm line-clamp-2 leading-tight">{item.product.name}</span>
                                        <span className="font-bold text-sm ml-2 whitespace-nowrap">
                                            {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(item.totalPrice)}
                                        </span>
                                    </div>
                                    <div className="flex items-center justify-between">
                                        <div className="flex items-center border rounded-md bg-muted/20">
                                            <Button 
                                                variant="ghost" 
                                                size="icon" 
                                                className="h-7 w-7 rounded-none hover:bg-background"
                                                onClick={() => updateItemQuantity(item.tempId, item.quantity - 1)}
                                            >
                                                <Minus className="h-3 w-3" />
                                            </Button>
                                            <span className="w-8 text-center text-sm font-medium">{item.quantity}</span>
                                            <Button 
                                                variant="ghost" 
                                                size="icon" 
                                                className="h-7 w-7 rounded-none hover:bg-background"
                                                onClick={() => updateItemQuantity(item.tempId, item.quantity + 1)}
                                            >
                                                <Plus className="h-3 w-3" />
                                            </Button>
                                        </div>
                                        <Button 
                                            variant="ghost" 
                                            size="icon" 
                                            className="h-7 w-7 text-muted-foreground hover:text-destructive hover:bg-destructive/10 opacity-0 group-hover:opacity-100 transition-opacity"
                                            onClick={() => removeItem(item.tempId)}
                                        >
                                            <Trash2 className="h-4 w-4" />
                                        </Button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            <div className="p-5 bg-background border-t shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.05)] z-20">
                <div className="space-y-2 text-sm mb-4">
                    <div className="flex justify-between text-muted-foreground">
                        <span>Subtotal</span>
                        <span>{new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(getSubtotal())}</span>
                    </div>
                    {orderType === OrderType.Delivery && (
                         <div className="flex justify-between items-center text-muted-foreground">
                            <span>Taxa de Entrega</span>
                            <div className="flex items-center w-24">
                                <Input 
                                    type="number" 
                                    className="h-7 text-right px-2 bg-muted/20" 
                                    value={deliveryFee}
                                    onChange={(e) => setDeliveryFee(Number(e.target.value))}
                                />
                            </div>
                        </div>
                    )}
                </div>

                <div className="flex justify-between items-end mb-4 pt-4 border-t border-dashed">
                    <span className="text-lg font-medium text-muted-foreground">Total</span>
                    <span className="text-3xl font-bold text-primary">
                        {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(getTotal())}
                    </span>
                </div>

                <Button 
                    className="w-full h-12 text-base font-semibold shadow-lg shadow-primary/20 hover:shadow-primary/30 transition-all" 
                    size="lg" 
                    onClick={handleCheckout} 
                    disabled={isLoading || items.length === 0}
                >
                    {isLoading ? (
                        "Processando..."
                    ) : (
                        <>
                            <CreditCard className="mr-2 h-5 w-5" />
                            Finalizar Pedido
                        </>
                    )}
                </Button>
            </div>
        </div>
    );
}
