import { Plus, Minus, Loader2 } from "lucide-react";
import { Skeleton } from "@/components/ui/skeleton";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Product as ApiProduct, ProductWithAddons } from "@/types/api";
import { getFullImageUrl } from "@/utils/image-url";

interface ProductCardProps {
  product?: ApiProduct | ProductWithAddons;
  isLoading?: boolean;
  cartQuantity?: number;
  onAddToCart?: (productId: string) => void;
  onRemoveFromCart?: (productId: string) => void;
  onProductClick?: (productId: string) => void;
}

const ProductCard = ({
  product,
  isLoading = false,
  cartQuantity = 0,
  onAddToCart = () => { },
  onRemoveFromCart = () => { },
  onProductClick = () => { }
}: ProductCardProps) => {
  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(price);
  };

  if (isLoading || !product) {
    return (
      <Card className="bg-card border-border overflow-hidden">
        <div className="aspect-[4/3] p-2">
          <Skeleton className="w-full h-full rounded-lg" />
        </div>
        <CardContent className="p-4 space-y-3">
          <Skeleton className="h-6 w-3/4" />
          <Skeleton className="h-4 w-full" />
          <div className="flex justify-between items-center">
            <Skeleton className="h-8 w-1/3" />
            <Skeleton className="h-8 w-8 rounded-full" />
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card
      className="group hover:shadow-2xl transition-all duration-300 hover:scale-[1.01] bg-white border border-border/50 overflow-hidden animate-fade-in-up cursor-pointer rounded-3xl"
      onClick={() => onProductClick(product.id)}
    >
      <div className="flex flex-col h-full">
        {/* Image Section */}
        <div className="aspect-[4/3] overflow-hidden bg-white relative flex items-center justify-center p-4">
          <div className="absolute inset-0 bg-gradient-to-t from-black/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
          {product.imageUrl ? (
            <img
              src={getFullImageUrl(product.imageUrl) || ''}
              alt={product.name}
              loading="lazy"
              decoding="async"
              className="w-full h-full object-contain group-hover:scale-110 transition-transform duration-700 ease-out"
            />
          ) : (
            <div className="w-full h-full flex items-center justify-center bg-muted/30 rounded-2xl">
              <div className="text-center text-muted-foreground/40">
                <div className="text-5xl mb-2 opacity-50">🍔</div>
                <p className="text-xs font-black uppercase tracking-tighter">Imagem em breve</p>
              </div>
            </div>
          )}

          {/* Badge for Indisponível */}
          {!product.isActive && (
            <div className="absolute inset-0 bg-white/60 backdrop-blur-[2px] flex items-center justify-center z-10">
              <Badge variant="destructive" className="font-black uppercase tracking-widest px-4 py-1 text-xs">
                Esgotado
              </Badge>
            </div>
          )}
        </div>

        {/* Content Section */}
        <CardContent className="p-5 flex-1 flex flex-col justify-between">
          <div className="space-y-1.5 mb-4">
            <h3 className="font-black text-xl text-foreground group-hover:text-primary transition-colors leading-tight uppercase italic tracking-tighter">
              {product.name}
            </h3>
            <p className="text-xs text-text-secondary font-medium line-clamp-2 leading-relaxed opacity-80">
              {product.description || "O sabor que você já conhece com a qualidade que você confia."}
            </p>
          </div>

          <div className="flex items-center justify-between mt-auto">
            <div className="flex flex-col">
              <span className="text-xs font-black text-primary/40 uppercase tracking-widest">A partir de</span>
              <span className="text-2xl font-black text-primary tracking-tighter">
                {formatPrice(product.price)}
              </span>
            </div>

            <div className="flex items-center gap-2" onClick={(e) => e.stopPropagation()}>
              {cartQuantity > 0 ? (
                <div className="flex items-center bg-muted p-1 rounded-full border border-border/50">
                  <Button
                    size="icon"
                    variant="ghost"
                    onClick={(e) => {
                      e.stopPropagation();
                      onRemoveFromCart(product.id);
                    }}
                    className="h-8 w-8 rounded-full hover:bg-white text-foreground"
                  >
                    <Minus className="h-4 w-4" />
                  </Button>
                  <span className="w-8 text-center font-black text-sm">
                    {cartQuantity}
                  </span>
                  <Button
                    size="icon"
                    onClick={(e) => {
                      e.stopPropagation();
                      onAddToCart(product.id);
                    }}
                    className="h-8 w-8 rounded-full bg-primary hover:bg-primary-hover text-white shadow-md shadow-primary/20"
                    disabled={!product.isActive}
                  >
                    <Plus className="h-4 w-4" />
                  </Button>
                </div>
              ) : (
                <Button
                  size="icon"
                  className="h-12 w-12 rounded-2xl bg-accent hover:bg-accent/90 text-accent-foreground shadow-lg shadow-accent/20 group-hover:scale-110 transition-transform"
                  onClick={(e) => {
                    e.stopPropagation();
                    onAddToCart(product.id);
                  }}
                  disabled={!product.isActive}
                >
                  <Plus className="h-6 w-6 stroke-[3]" />
                </Button>
              )}
            </div>
          </div>
        </CardContent>
      </div>
    </Card>
  );
};

export default ProductCard;