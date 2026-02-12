import { ShoppingCart as ShoppingCartIcon, Home, User, Settings, LogOut, Percent, ShoppingBag } from "lucide-react";
import { Button } from "@/components/ui/button";
import { CustomerResponseDto } from "@/types/api";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

interface BottomNavigationProps {
  cartItemCount: number;
  cartTotal: number;
  onCartClick: () => void;
  onProfileClick: () => void;
  onCouponsClick?: () => void;
  onOrdersClick?: () => void;
  customer?: CustomerResponseDto | null;
  onLogoutClick?: () => void;
}

const BottomNavigation = ({
  cartItemCount,
  cartTotal,
  onCartClick,
  onProfileClick,
  onCouponsClick,
  onOrdersClick,
  customer,
  onLogoutClick
}: BottomNavigationProps) => {
  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(price);
  };

  const ProfileButton = () => {
    if (customer) {
      return (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button
              variant="ghost"
              className="flex flex-col items-center gap-1 h-auto p-0 text-text-secondary hover:text-primary transition-all active:scale-95 min-w-[64px]"
            >
              <User className="h-6 w-6 stroke-[2.5]" />
              <span className="text-[10px] font-black uppercase tracking-widest max-w-[60px] truncate">
                {customer.name.split(' ')[0]}
              </span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" side="top" className="w-64 bg-white/95 backdrop-blur-xl border-2 border-border/50 shadow-2xl z-50 mb-4 rounded-[2rem] p-2">
            <DropdownMenuLabel className="text-xs font-black uppercase tracking-widest text-muted-foreground px-4 py-3">
              Minha Conta
            </DropdownMenuLabel>
            <DropdownMenuSeparator className="bg-border/50 mx-2" />
            <DropdownMenuItem 
              className="cursor-pointer rounded-xl font-bold p-3 focus:bg-primary/5 focus:text-primary transition-colors gap-3" 
              onClick={onProfileClick}
            >
              <div className="bg-primary/10 p-2 rounded-lg">
                <Settings className="h-4 w-4 stroke-[2.5] text-primary" />
              </div>
              <span className="uppercase tracking-tight text-xs">Editar Perfil</span>
            </DropdownMenuItem>
            <DropdownMenuSeparator className="bg-border/50 mx-2" />
            <DropdownMenuItem 
              className="cursor-pointer text-red-500 focus:text-red-600 focus:bg-red-50 rounded-xl font-bold p-3 transition-colors gap-3" 
              onClick={onLogoutClick}
            >
              <div className="bg-red-100 p-2 rounded-lg">
                <LogOut className="h-4 w-4 stroke-[2.5]" />
              </div>
              <span className="uppercase tracking-tight text-xs">Sair</span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      );
    }

    return (
      <Button
        variant="ghost"
        className="flex flex-col items-center gap-1 h-auto p-0 text-text-secondary hover:text-primary transition-all active:scale-95 min-w-[64px]"
        onClick={onProfileClick}
      >
        <User className="h-6 w-6 stroke-[2.5]" />
        <span className="text-[10px] font-black uppercase tracking-widest">Perfil</span>
      </Button>
    );
  };

  return (
    <div className="fixed bottom-0 left-0 right-0 bg-white/80 backdrop-blur-xl border-t border-border/50 py-3 px-6 flex justify-between items-center shadow-[0_-10px_40px_-15px_rgba(0,0,0,0.1)] z-40 h-[85px] pb-8 rounded-t-[2.5rem]">
      <Button
        variant="ghost"
        className="flex flex-col items-center gap-1 h-auto p-0 text-text-secondary hover:text-primary transition-all active:scale-95"
        onClick={() => {
          window.scrollTo({ top: 0, behavior: 'smooth' });
        }}
      >
        <Home className="h-6 w-6 stroke-[2.5]" />
        <span className="text-[10px] font-black uppercase tracking-widest">Início</span>
      </Button>

      {customer && (
        <Button
          variant="ghost"
          className="flex flex-col items-center gap-1 h-auto p-0 text-text-secondary hover:text-primary transition-all active:scale-95"
          onClick={onCouponsClick}
        >
          <Percent className="h-6 w-6 stroke-[2.5]" />
          <span className="text-[10px] font-black uppercase tracking-widest">Cupons</span>
        </Button>
      )}

      {/* Central Floating Cart */}
      <div className="relative -top-10">
        <Button
          className={`
            rounded-full w-20 h-20 shadow-2xl flex flex-col items-center justify-center gap-0 p-0 transition-all duration-500 active:scale-90 border-4 border-background
            ${cartItemCount > 0
              ? "bg-primary text-white shadow-primary/40 animate-bounce-subtle"
              : "bg-muted text-muted-foreground shadow-black/5"}
          `}
          onClick={onCartClick}
        >
          <div className="relative">
            <ShoppingCartIcon className="h-8 w-8 stroke-[2.5]" />
            {cartItemCount > 0 && (
              <span className="absolute -top-2 -right-2 bg-accent text-accent-foreground text-[10px] font-black w-6 h-6 rounded-full flex items-center justify-center border-2 border-primary shadow-sm">
                {cartItemCount}
              </span>
            )}
          </div>
          {cartTotal > 0 && (
            <span className="text-[9px] font-black uppercase tracking-tighter mt-1 leading-none">
              {formatPrice(cartTotal)}
            </span>
          )}
        </Button>
      </div>

      {customer && (
        <Button
          variant="ghost"
          className="flex flex-col items-center gap-1 h-auto p-0 text-text-secondary hover:text-primary transition-all active:scale-95"
          onClick={onOrdersClick}
        >
          <ShoppingBag className="h-6 w-6 stroke-[2.5]" />
          <span className="text-[10px] font-black uppercase tracking-widest">Pedidos</span>
        </Button>
      )}

      <ProfileButton />
    </div>
  );
};

export default BottomNavigation;
