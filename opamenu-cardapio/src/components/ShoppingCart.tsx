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
      {/* Backdrop - apenas para dispositivos móveis */}
      {isOpen && (
        <div
          className="fixed inset-0 bg-black/50 z-40 backdrop-blur-sm md:hidden"
          onClick={onClose}
        />
      )}

      {/* Cart Panel */}
      <div className={`fixed right-0 top-0 h-full w-[80vw] sm:w-[400px] max-w-md bg-card shadow-2xl z-50 transform transition-transform duration-300 border-l ${isOpen ? 'translate-x-0' : 'translate-x-full'
        }`}>
        <div className="h-full rounded-none flex flex-col">
          <div className="flex flex-row items-center justify-between p-6 border-b bg-card">
            <div className="flex items-center gap-2">
              <ShoppingBag className="h-5 w-5 text-primary" />
              <span className="font-semibold">Carrinho</span>
              {totalItems > 0 && (
                <Badge className="bg-primary text-white">
                  {totalItems}
                </Badge>
              )}
            </div>
            <Button variant="ghost" size="icon" onClick={onClose}>
              <X className="h-5 w-5" />
            </Button>
          </div>

          <div className="flex-1 min-h-0 flex flex-col">
            {cartItems.length === 0 ? (
              <div className="flex-1 flex items-center justify-center text-center p-6">
                <div>
                  <div className="text-6xl mb-4">🛒</div>
                  <h3 className="text-lg font-medium text-foreground mb-2">
                    Carrinho vazio
                  </h3>
                  <p className="text-muted-foreground">
                    Adicione alguns produtos deliciosos ao seu carrinho!
                  </p>
                </div>
              </div>
            ) : (
              <>
                {/* Cart Items */}
                <div className="flex-1 overflow-y-auto p-4 space-y-4">
                  {cartItems.map((item, index) => (
                    <div key={item.cartItemId || `${item.product.id}-${index}`} className="flex flex-col gap-3 p-3 bg-muted/30 rounded-lg">
                      <div className="flex gap-3">
                        <div className="w-16 h-16 bg-muted rounded-lg overflow-hidden flex-shrink-0">
                          {item.product.imageUrl ? (
                            <img
                              src={item.product.imageUrl}
                              alt={item.product.name}
                              className="w-full h-full object-cover"
                            />
                          ) : (
                            <div className="w-full h-full flex items-center justify-center text-2xl">
                              🍽️
                            </div>
                          )}
                        </div>

                        <div className="flex-1 min-w-0 flex flex-col justify-center">
                          <h4 className="font-medium text-foreground line-clamp-2 text-sm">
                            {item.product.name}
                          </h4>
                          <p className="text-opamenu-orange font-semibold whitespace-nowrap mt-1">
                            {formatPrice(item.unitPrice)}
                          </p>
                        </div>

                        <div className="flex items-start gap-1">
                          {onEditItem && (
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={(e) => {
                                e.stopPropagation();
                                onEditItem(item);
                              }}
                              className="text-muted-foreground hover:text-primary hover:bg-primary/10 h-8 w-8 -mt-1"
                            >
                              <Pencil className="h-4 w-4" />
                            </Button>
                          )}
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={(e) => {
                              e.stopPropagation();
                              onRemoveItem(item.cartItemId || item.product.id);
                            }}
                            className="text-muted-foreground hover:text-destructive hover:bg-destructive/10 h-8 w-8 -mt-1 -mr-1"
                          >
                            <X className="h-4 w-4" />
                          </Button>
                        </div>
                      </div>

                      {/* Exibir adicionais selecionados */}
                      {item.selectedAddons && item.selectedAddons.length > 0 && (
                        <div className="pl-[76px] space-y-1">
                          {item.selectedAddons.map((selectedAddon, addonIndex) => (
                            <div key={addonIndex} className="text-xs text-muted-foreground flex justify-between">
                              <span>
                                + {selectedAddon.addon.name}
                                {selectedAddon.quantity > 1 && ` (${selectedAddon.quantity}x)`}
                              </span>
                              {selectedAddon.addon.price > 0 && (
                                <span className="font-medium">
                                  {formatPrice(selectedAddon.addon.price * selectedAddon.quantity)}
                                </span>
                              )}
                            </div>
                          ))}
                        </div>
                      )}

                      <div className="flex items-center justify-end border-t pt-3 mt-1">
                        <div className="flex items-center gap-3">
                          <span className="text-xs text-muted-foreground mr-2">Quantidade:</span>
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={(e) => {
                              e.stopPropagation();
                              onUpdateQuantity(item.cartItemId || item.product.id, item.quantity - 1);
                            }}
                            className="h-8 w-8 p-0"
                          >
                            <Minus className="h-3 w-3" />
                          </Button>
                          <span className="w-8 text-center font-medium">
                            {item.quantity}
                          </span>
                          <Button
                            size="sm"
                            onClick={(e) => {
                              e.stopPropagation();
                              onUpdateQuantity(item.cartItemId || item.product.id, item.quantity + 1);
                            }}
                            className="h-8 w-8 p-0 bg-opamenu-orange hover:bg-opamenu-orange/90"
                          >
                            <Plus className="h-3 w-3" />
                          </Button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>

                {/* Cart Summary */}
                <div className="border-t p-4 space-y-4 bg-card">
                  {/* Coupon Section */}
                  <div className="space-y-2">
                    {coupon ? (
                      <div className="bg-green-50 border border-green-200 rounded-lg p-3 flex items-center justify-between">
                        <div className="flex items-center gap-2">
                          <Tag className="h-4 w-4 text-green-600" />
                          <div>
                            <p className="font-medium text-green-800 text-sm">Cupom: {coupon.code}</p>
                            <p className="text-xs text-green-600">
                              {coupon.eDiscountType === EDiscountType.Porcentagem ? `${coupon.discountValue}% OFF` : `R$ ${coupon.discountValue} OFF`} applied
                            </p>
                          </div>
                        </div>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={onRemoveCoupon}
                          className="text-red-500 hover:text-red-700 hover:bg-red-50 h-8 w-8 p-0"
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </div>
                    ) : (
                      <div className="flex gap-2">
                        <Input
                          placeholder="Código do cupom"
                          value={couponCode}
                          onChange={(e) => setCouponCode(e.target.value.toUpperCase())}
                          className="h-9"
                          disabled={isValidating}
                        />
                        <Button
                          size="sm"
                          onClick={handleApplyCoupon}
                          disabled={!couponCode || isValidating}
                          className="h-9"
                        >
                          {isValidating ? <Loader2 className="h-4 w-4 animate-spin" /> : "Aplicar"}
                        </Button>
                      </div>
                    )}
                    {couponError && (
                      <p className="text-xs text-red-500 ml-1">{couponError}</p>
                    )}
                  </div>

                  <Separator />

                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span>Subtotal ({totalItems} items)</span>
                      <span>{formatPrice(subtotal)}</span>
                    </div>
                    {discount > 0 && (
                      <div className="flex justify-between text-sm text-green-600 font-medium">
                        <span>Desconto</span>
                        <span>- {formatPrice(discount)}</span>
                      </div>
                    )}
                    <Separator />
                    <div className="flex justify-between font-semibold text-lg">
                      <span>Total</span>
                      <span className="text-opamenu-orange">{formatPrice(totalPrice)}</span>
                    </div>
                  </div>

                  <Button
                    className="w-full bg-opamenu-orange hover:bg-opamenu-orange/90 text-white font-semibold py-4 text-lg transition-all duration-200 hover:shadow-lg"
                    size="lg"
                    onClick={onCheckout}
                  >
                    Finalizar Pedido
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