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
              className="flex flex-col items-center gap-1 h-auto py-2 text-gray-500 hover:text-primary min-w-[64px]"
            >
              <User className="h-6 w-6" />
              <span className="text-[10px] max-w-[60px] truncate">{customer.name.split(' ')[0]}</span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" side="top" className="w-56 bg-white border border-gray-200 shadow-lg z-50 mb-2">
            <DropdownMenuLabel>Minha Conta</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem className="cursor-pointer" onClick={onProfileClick}>
              <Settings className="mr-2 h-4 w-4" />
              <span>Editar Perfil</span>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem className="cursor-pointer text-red-600 focus:text-red-600" onClick={onLogoutClick}>
              <LogOut className="mr-2 h-4 w-4" />
              <span>Sair</span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      );
    }

    return (
      <Button
        variant="ghost"
        className="flex flex-col items-center gap-1 h-auto py-2 text-gray-500 hover:text-primary min-w-[64px]"
        onClick={onProfileClick}
      >
        <User className="h-6 w-6" />
        <span className="text-[10px]">Perfil</span>
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
