import { Search, ShoppingCart } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import logoOpamenu from "@/assets/logo_opamenu.png";

interface HeaderProps {
  searchQuery: string;
  onSearchChange: (query: string) => void;
  cartItemCount: number;
  onCartToggle: () => void;
}

const Header = ({ searchQuery, onSearchChange, cartItemCount, onCartToggle }: HeaderProps) => {
  return (
    <header className="sticky top-0 z-50 bg-card/95 backdrop-blur-sm border-b border-border">
      <div className="container mx-auto px-4 py-4">
        <div className="flex items-center justify-between gap-4">
          {/* Logo */}
          <div className="flex-shrink-0 flex items-center gap-3">
            <img 
              src={logoOpamenu} 
              alt="Opamenu Logo" 
              className="h-16 w-auto"
            />
          </div>
          {/* Search Bar */}
          <div className="flex-1 max-w-md relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
            <Input
              type="text"
              placeholder="Buscar pratos, bebidas..."
              value={searchQuery}
              onChange={(e) => onSearchChange(e.target.value)}
              className="pl-10 bg-input border-input-border focus:ring-2 focus:ring-opamenu-green/20 transition-all duration-200"
            />
          </div>

          {/* Cart Button */}
          <Button
            variant="outline"
            size="icon"
            onClick={onCartToggle}
            className="relative hover:bg-opamenu-green/10 hover:border-opamenu-green transition-all duration-200"
          >
            <ShoppingCart className="h-5 w-5" />
            {cartItemCount > 0 && (
              <Badge 
                variant="destructive" 
                className="absolute -top-2 -right-2 h-5 w-5 p-0 flex items-center justify-center text-xs bg-primary animate-cart-bounce"
              >
                {cartItemCount > 9 ? '9+' : cartItemCount}
              </Badge>
            )}
          </Button>
        </div>
      </div>
    </header>
  );
};

export default Header;