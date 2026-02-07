import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { CartItem } from "@/types/cart";
import { Trash2 } from "lucide-react";

interface InlineCartProps {
  cartItems: CartItem[];
  total: number;
  onUpdateQuantity: (itemId: number | string, quantity: number) => void;
  onRemoveItem: (itemId: number | string) => void;
  onEditItem?: (item: CartItem) => void;
  onClearCart: () => void;
  onCheckout: () => void;
}

const InlineCart = ({
  cartItems,
  total,
  onUpdateQuantity,
  onRemoveItem,
  onEditItem,
  onClearCart,
  onCheckout
}: InlineCartProps) => {
  if (cartItems.length === 0) {
    return (
      <Card className="border-none shadow-sm">
        <CardHeader>
          <CardTitle className="text-lg">Sua sacola</CardTitle>
        </CardHeader>
        <CardContent className="text-center py-8 text-gray-500 text-sm">
          Sua sacola está vazia
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="border-none shadow-sm">
      <CardHeader className="flex flex-row items-center justify-between pb-2">
        <CardTitle className="text-lg">Sua sacola</CardTitle>
        <Button
          variant="ghost"
          size="sm"
          className="h-auto p-0 text-xs text-primary hover:text-primary/80 font-semibold uppercase"
          onClick={onClearCart}
        >
          Limpar
        </Button>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="space-y-4 max-h-[300px] overflow-y-auto pr-2 custom-scrollbar">
          {cartItems.map((item, index) => (
            <div key={item.cartItemId || `${item.product.id}-${index}`} className="flex gap-2 text-sm">
              <div className="font-semibold text-gray-500 w-4 pt-0.5">{item.quantity}x</div>
              <div className="flex-1 space-y-1">
                <p className="font-medium text-gray-800">{item.product.name}</p>
                {item.notes && <p className="text-xs text-gray-500">Obs: {item.notes}</p>}
                <div className="flex items-center gap-3 mt-1">
                  <button
                    onClick={() => onEditItem?.(item)}
                    className="text-primary text-xs font-semibold hover:underline"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => onRemoveItem(item.cartItemId || item.product.id)}
                    className="text-gray-400 text-xs hover:text-red-500 transition-colors"
                  >
                    Remover
                  </button>
                </div>
              </div>
              <div className="font-medium text-gray-800 pt-0.5">
                {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(item.totalPrice)}
              </div>
            </div>
          ))}
        </div>

        <div className="border-t pt-4 space-y-2">
          <div className="flex justify-between text-sm text-gray-600">
            <span>Subtotal</span>
            <span>{new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(total)}</span>
          </div>
          <div className="flex justify-between text-sm text-gray-600">
            <span>Taxa de entrega</span>
            <span>{new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(5.00)}</span>
          </div>
          <div className="flex justify-between font-bold text-lg text-gray-900 pt-2">
            <span>Total</span>
            <span>{new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(total + 5)}</span>
          </div>
        </div>

        <Button
          className="w-full bg-primary hover:bg-primary/90 text-white font-semibold py-6"
          onClick={onCheckout}
        >
          Escolher forma de pagamento
        </Button>
      </CardContent>
    </Card>
  );
};

export default InlineCart;
