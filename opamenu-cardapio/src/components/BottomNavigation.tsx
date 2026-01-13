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
    <div className="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-200 py-2 px-2 flex justify-between items-end shadow-[0_-4px_6px_-1px_rgba(0,0,0,0.1)] z-40 h-[70px] pb-2">
      <Button 
        variant="ghost" 
        className="flex flex-col items-center gap-1 h-auto py-2 text-gray-500 hover:text-primary min-w-[64px]"
        onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}
      >
        <Home className="h-6 w-6" />
        <span className="text-[10px]">Início</span>
      </Button>

      {customer && (
        <Button 
          variant="ghost" 
          className="flex flex-col items-center gap-1 h-auto py-2 text-gray-500 hover:text-primary min-w-[64px]"
          onClick={onCouponsClick}
        >
          <Percent className="h-6 w-6" />
          <span className="text-[10px]">Cupons</span>
        </Button>
      )}
      
      <div className="relative -top-5">
        <Button 
          className="rounded-full w-14 h-14 bg-[#FF4500] hover:bg-[#FF4500]/90 shadow-lg flex flex-col items-center justify-center gap-0 p-0"
          onClick={onCartClick}
        >
          <ShoppingCartIcon className="h-6 w-6 text-white" />
          {cartTotal > 0 && (
             <span className="text-[10px] text-white font-medium mt-0.5 leading-none">
               {formatPrice(cartTotal)}
             </span>
          )}
        </Button>
      </div>

      {customer && (
        <Button 
          variant="ghost" 
          className="flex flex-col items-center gap-1 h-auto py-2 text-gray-500 hover:text-primary min-w-[64px]"
          onClick={onOrdersClick}
        >
          <ShoppingBag className="h-6 w-6" />
          <span className="text-[10px]">Pedidos</span>
        </Button>
      )}

      <ProfileButton />
    </div>
  );
};

export default BottomNavigation;
