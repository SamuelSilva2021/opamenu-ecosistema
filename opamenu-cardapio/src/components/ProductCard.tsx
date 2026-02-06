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
      className="group hover:shadow-lg transition-all duration-300 hover:scale-[1.02] bg-card border-border overflow-hidden animate-fade-in-up cursor-pointer"
      onClick={() => onProductClick(product.id)}
    >

      <div className="aspect-[4/3] overflow-hidden bg-white p-2 relative flex items-center justify-center">
        {product.imageUrl ? (
          <img
            src={getFullImageUrl(product.imageUrl) || ''}
            alt={product.name}
            loading="lazy"
            decodings="async"
            className="w-full h-full object-contain group-hover:scale-105 transition-transform duration-300"
          />
        ) : (
          <div className="w-full h-full flex items-center justify-center bg-gradient-to-br from-primary/20 to-opamenu-green/20">
            <div className="text-center text-muted-foreground">
              <div className="text-4xl mb-2">🍽️</div>
              <p className="text-sm">Imagem não disponível</p>
            </div>
          </div>
        )}
      </div>

      <CardContent className="p-4">
        <div className="mb-3">
          <h3 className="font-semibold text-lg text-foreground group-hover:text-primary transition-colors">
            {product.name}
          </h3>
          <p className="text-sm text-muted-foreground line-clamp-2 mt-1">
            {product.description}
          </p>
        </div>

        <div className="flex items-center justify-between">
          <div className="flex-1">
            <span className="text-2xl font-bold text-primary">
              {formatPrice(product.price)}
            </span>
          </div>

          <div className="flex items-center gap-2" onClick={(e) => e.stopPropagation()}>
            {cartQuantity > 0 && (
              <div className="flex items-center gap-2">
                <Button
                  size="sm"
                  variant="outline"
                  onClick={(e) => {
                    e.stopPropagation();
                    onRemoveFromCart(product.id);
                  }}
                  className="h-8 w-8 p-0 hover:bg-destructive/10 hover:border-destructive"
                >
                  <Minus className="h-4 w-4" />
                </Button>
                <span className="w-8 text-center font-medium">
                  {cartQuantity}
                </span>
                <Button
                  size="sm"
                  onClick={(e) => {
                    e.stopPropagation();
                    onAddToCart(product.id);
                  }}
                  className="h-8 w-8 p-0 bg-primary hover:bg-primary/90 text-white"
                  disabled={!product.isActive}
                >
                  <Plus className="h-4 w-4" />
                </Button>
              </div>
            )}

            {cartQuantity === 0 && product.isActive && (
              <Button
                size="sm"
                variant="ghost"
                className="h-8 w-8 p-0 text-primary hover:bg-primary/10"
                onClick={(e) => {
                  e.stopPropagation();
                  onAddToCart(product.id);
                }}
              >
                <Plus className="h-4 w-4" />
              </Button>
            )}
          </div>
        </div>

        {!product.isActive && (
          <div className="mt-2">
            <Badge variant="secondary" className="text-xs">
              Indisponível
            </Badge>
          </div>
        )}
      </CardContent>
    </Card>
  );
};

export default ProductCard;