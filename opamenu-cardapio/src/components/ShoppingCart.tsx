import { useState } from "react";
import { X, Plus, Minus, ShoppingBag, Tag, Loader2, Pencil } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { Input } from "@/components/ui/input";
import { CartItem } from "@/types/cart";
import { Coupon, EDiscountType } from "@/types/api";

interface ShoppingCartProps {
  isOpen: boolean;
  onClose: () => void;
  cartItems: CartItem[];
  onUpdateQuantity: (itemId: number | string, quantity: number) => void;
  onRemoveItem: (itemId: number | string) => void;
  onCheckout: () => void;
  subtotal: number;
  discount: number;
  totalPrice: number;
  coupon: Coupon | null;
  onApplyCoupon: (coupon: Coupon) => void;
  onRemoveCoupon: () => void;
  onValidateCoupon: (code: string) => Promise<boolean>;
  onEditItem?: (item: CartItem) => void;
}

const ShoppingCart = ({
  isOpen,
  onClose,
  cartItems,
  onUpdateQuantity,
  onRemoveItem,
  onCheckout,
  subtotal,
  discount,
  totalPrice,
  coupon,
  onApplyCoupon,
  onRemoveCoupon,
  onValidateCoupon,
  onEditItem
}: ShoppingCartProps) => {
  const [couponCode, setCouponCode] = useState("");
  const [isValidating, setIsValidating] = useState(false);
  const [couponError, setCouponError] = useState("");

  const handleApplyCoupon = async () => {
    if (!couponCode.trim()) return;

    setIsValidating(true);
    setCouponError("");

    try {
      const isValid = await onValidateCoupon(couponCode);
      if (isValid) {
        setCouponCode("");
      } else {
        setCouponError("Cupom inválido ou expirado");
      }
    } catch (error) {
      setCouponError("Erro ao validar cupom");
    } finally {
      setIsValidating(false);
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(price);
  };

  const totalItems = cartItems.reduce((sum, item) => sum + item.quantity, 0);
  // totalPrice is passed as prop now


  if (!isOpen) return null;

  return (
    <>
      {/* Backdrop */}
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/60 z-40 backdrop-blur-md transition-opacity duration-500"
          onClick={onClose}
        />
      )}

      {/* Cart Panel */}
      <div className={`fixed right-0 top-0 h-full w-[90vw] sm:w-[500px] bg-background/95 backdrop-blur-3xl shadow-2xl z-50 transform transition-all duration-700 ease-in-out border-l border-border/50 ${isOpen ? 'translate-x-0' : 'translate-x-full'
        }`}>
        <div className="h-full flex flex-col">
          {/* Header */}
          <div className="flex flex-row items-center justify-between p-8 border-b border-border/10">
            <div className="flex items-center gap-3">
              <div className="bg-primary/10 p-3 rounded-2xl">
                <ShoppingBag className="h-6 w-6 text-primary stroke-[2.5]" />
              </div>
              <div className="flex flex-col">
                <h2 className="font-black uppercase italic tracking-tighter text-2xl leading-none">Seu Carrinho</h2>
                <span className="text-[10px] font-bold text-text-secondary uppercase tracking-widest opacity-60">
                  {totalItems} {totalItems === 1 ? 'item selecionado' : 'items selecionados'}
                </span>
              </div>
            </div>
            <Button variant="ghost" size="icon" onClick={onClose} className="rounded-full h-12 w-12 hover:bg-muted">
              <X className="h-6 w-6 stroke-[3]" />
            </Button>
          </div>

          {/* Body */}
          <div className="flex-1 min-h-0 flex flex-col pt-4">
            {cartItems.length === 0 ? (
              <div className="flex-1 flex flex-col items-center justify-center text-center p-12">
                <div className="relative mb-8">
                  <div className="text-8xl animate-bounce-subtle">🛒</div>
                  <div className="absolute -bottom-2 -right-2 bg-primary text-white p-2 rounded-full shadow-lg">
                    <Plus className="h-4 w-4 stroke-[4]" />
                  </div>
                </div>
                <h3 className="text-2xl font-black uppercase tracking-tighter text-foreground mb-3">
                  Tá com fome de quê?
                </h3>
                <p className="text-sm font-medium text-text-secondary max-w-[200px]">
                  Seu carrinho está esperando por algo delicioso.
                </p>
                <Button
                  onClick={onClose}
                  className="mt-8 rounded-2xl font-black uppercase italic tracking-widest px-8"
                >
                  Explorar Cardápio
                </Button>
              </div>
            ) : (
              <>
                {/* Cart Items List */}
                <div className="flex-1 overflow-y-auto p-6 space-y-6 scrollbar-hide">
                  {cartItems.map((item, index) => (
                    <div
                      key={item.cartItemId || `${item.product.id}-${index}`}
                      className="group flex flex-col gap-4 p-5 bg-white rounded-[2rem] border border-border/50 shadow-sm hover:shadow-xl transition-all duration-300"
                    >
                      <div className="flex gap-5">
                        <div className="w-20 h-20 bg-muted/30 rounded-2xl overflow-hidden flex-shrink-0 border border-border/10">
                          {item.product.imageUrl ? (
                            <img
                              src={item.product.imageUrl}
                              alt={item.product.name}
                              className="w-full h-full object-contain p-2 group-hover:scale-110 transition-transform duration-500"
                            />
                          ) : (
                            <div className="w-full h-full flex items-center justify-center text-3xl opacity-40">
                              🍔
                            </div>
                          )}
                        </div>

                        <div className="flex-1 min-w-0 flex flex-col justify-center gap-1">
                          <h4 className="font-black text-lg text-foreground line-clamp-1 uppercase italic tracking-tighter">
                            {item.product.name}
                          </h4>
                          <span className="text-2xl font-black text-primary tracking-tighter">
                            {formatPrice(item.unitPrice)}
                          </span>
                        </div>

                        <div className="flex flex-col items-end gap-2">
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() => onRemoveItem(item.cartItemId || item.product.id)}
                            className="text-muted-foreground hover:text-destructive hover:bg-destructive/5 h-10 w-10 rounded-full"
                          >
                            <X className="h-5 w-5 stroke-[2.5]" />
                          </Button>
                          {onEditItem && (
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => onEditItem(item)}
                              className="text-muted-foreground hover:text-primary hover:bg-primary/5 h-10 w-10 rounded-full"
                            >
                              <Pencil className="h-4 w-4 stroke-[2.5]" />
                            </Button>
                          )}
                        </div>
                      </div>

                      {/* Seçao de Adicionais */}
                      {item.selectedAddons && item.selectedAddons.length > 0 && (
                        <div className="pl-2 space-y-1.5 border-l-2 border-primary/10">
                          {item.selectedAddons.map((selectedAddon, addonIndex) => (
                            <div key={addonIndex} className="text-xs flex justify-between items-center group/addon">
                              <span className="font-bold text-text-secondary/60">
                                <span className="text-primary/40 mr-1">•</span>
                                {selectedAddon.addon.name}
                                {selectedAddon.quantity > 1 && (
                                  <span className="ml-1 bg-muted px-1.5 py-0.5 rounded text-[9px] text-foreground">
                                    {selectedAddon.quantity}x
                                  </span>
                                )}
                              </span>
                              {selectedAddon.addon.price > 0 && (
                                <span className="font-black text-primary/60 tabular-nums">
                                  + {formatPrice(selectedAddon.addon.price * selectedAddon.quantity)}
                                </span>
                              )}
                            </div>
                          ))}
                        </div>
                      )}

                      <div className="flex items-center justify-between border-t border-border/10 pt-4 mt-2">
                        <span className="text-[10px] font-black uppercase tracking-[0.2em] text-primary/30">Quantidade</span>
                        <div className="flex items-center bg-muted/50 p-1 rounded-xl border border-border/50">
                          <Button
                            size="icon"
                            variant="ghost"
                            onClick={() => onUpdateQuantity(item.cartItemId || item.product.id, item.quantity - 1)}
                            className="h-8 w-8 p-0 rounded-lg hover:bg-white"
                          >
                            <Minus className="h-3 w-3" />
                          </Button>
                          <span className="w-8 text-center font-black text-sm">
                            {item.quantity}
                          </span>
                          <Button
                            size="icon"
                            variant="ghost"
                            onClick={() => onUpdateQuantity(item.cartItemId || item.product.id, item.quantity + 1)}
                            className="h-8 w-8 p-0 bg-white rounded-lg hover:shadow-sm"
                          >
                            <Plus className="h-3 w-3" />
                          </Button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>

                {/* Footer Summary */}
                <div className="shrink-0 p-8 space-y-6 bg-white border-t border-border/10 shadow-[0_-10px_40px_-15px_rgba(0,0,0,0.1)] rounded-t-[3rem]">
                  {/* Coupon Section */}
                  <div className="relative">
                    {coupon ? (
                      <div className="bg-green-500/10 border-2 border-green-500/20 rounded-2xl p-4 flex items-center justify-between group/coupon">
                        <div className="flex items-center gap-3">
                          <div className="bg-green-500 text-white p-2 rounded-xl">
                            <Tag className="h-4 w-4 stroke-[3]" />
                          </div>
                          <div>
                            <p className="font-black text-green-700 text-sm uppercase italic tracking-tighter leading-none">Cupom: {coupon.code}</p>
                            <p className="text-[10px] font-bold text-green-600 uppercase tracking-widest mt-1">
                              Economia de {coupon.eDiscountType === EDiscountType.Porcentagem ? `${coupon.discountValue}%` : formatPrice(coupon.discountValue)}
                            </p>
                          </div>
                        </div>
                        <Button
                          variant="ghost"
                          size="icon"
                          onClick={onRemoveCoupon}
                          className="text-green-600 hover:text-red-500 hover:bg-white h-10 w-10 rounded-full"
                        >
                          <X className="h-5 w-5 stroke-[3]" />
                        </Button>
                      </div>
                    ) : (
                      <div className="flex gap-2">
                        <Input
                          placeholder="CÓDIGO DO CUPOM"
                          value={couponCode}
                          onChange={(e) => setCouponCode(e.target.value.toUpperCase())}
                          className="h-14 rounded-2xl border-2 border-border focus:border-primary font-bold uppercase tracking-widest text-xs px-6"
                          disabled={isValidating}
                        />
                        <Button
                          onClick={handleApplyCoupon}
                          disabled={!couponCode || isValidating}
                          className="h-14 w-24 rounded-2xl font-black uppercase italic tracking-tighter"
                        >
                          {isValidating ? <Loader2 className="h-5 w-5 animate-spin" /> : "OK"}
                        </Button>
                      </div>
                    )}
                    {couponError && (
                      <p className="text-[10px] font-black uppercase tracking-widest text-red-500 mt-2 px-4">{couponError}</p>
                    )}
                  </div>

                  <div className="space-y-3">
                    <div className="flex justify-between items-center text-sm">
                      <span className="font-bold text-text-secondary/60 uppercase tracking-widest text-[10px]">Subtotal</span>
                      <span className="font-black text-foreground">{formatPrice(subtotal)}</span>
                    </div>
                    {discount > 0 && (
                      <div className="flex justify-between items-center text-sm text-green-600">
                        <span className="font-bold uppercase tracking-widest text-[10px]">Desconto</span>
                        <span className="font-black">- {formatPrice(discount)}</span>
                      </div>
                    )}
                    <div className="flex justify-between items-end pt-2">
                      <div className="flex flex-col">
                        <span className="text-[10px] font-black uppercase tracking-[0.2em] text-primary">VALOR TOTAL</span>
                        <span className="text-4xl font-black text-primary tracking-tighter leading-none">{formatPrice(totalPrice)}</span>
                      </div>
                    </div>
                  </div>

                  <Button
                    className="w-full h-20 bg-primary hover:bg-primary-hover text-white rounded-[2rem] font-black uppercase italic tracking-wider text-xl shadow-2xl shadow-primary/30 transition-all duration-300 transform border-b-8 border-primary-dark active:border-b-0 active:translate-y-1"
                    size="lg"
                    onClick={onCheckout}
                  >
                    Confirmar Pedido
                  </Button>
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </>
  );
};

export default ShoppingCart;