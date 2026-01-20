import { useState, useMemo, useEffect } from "react";
import { TenantBusinessInfo, CustomerResponseDto } from "@/types/api";
import { MapPin, Home, Percent, ShoppingBag, User, Clock, LogOut, Settings } from "lucide-react";
import { Button } from "@/components/ui/button";
import TenantInfoModal from "./TenantInfoModal";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

interface TenantHeaderProps {
  tenant: TenantBusinessInfo;
  customer?: CustomerResponseDto | null;
  onLoginClick?: () => void;
  onLogoutClick?: () => void;
  onEditProfileClick?: () => void;
  onOrdersClick?: () => void;
}

const TenantHeader = ({ tenant, customer, onLoginClick, onLogoutClick, onEditProfileClick, onOrdersClick }: TenantHeaderProps) => {
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [status, setStatus] = useState<{ isOpen: boolean; text: string; nextTime?: string }>({ 
    isOpen: false, 
    text: "Verificando..." 
  });

  useEffect(() => {
    const checkStatus = () => {
      if (!tenant.openingHours) {
        setStatus({ isOpen: false, text: "Horário não informado" });
        return;
      }

      try {
        const hoursMap = tenant.openingHours;
        const now = new Date();
        const days = ['sunday', 'monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday'];
        const currentDay = days[now.getDay()] as keyof typeof hoursMap;
        const todayHours = hoursMap[currentDay];

        if (!todayHours || !todayHours.isOpen) {
          setStatus({ isOpen: false, text: "Fechado hoje" });
          return;
        }

        const start = todayHours.open;
        const end = todayHours.close;
        
        if (!start || !end) {
           setStatus({ isOpen: false, text: "Horário indisponível" });
           return;
        }

        const [startH, startM] = start.split(':').map(Number);
        const [endH, endM] = end.split(':').map(Number);

        const startTime = new Date(now);
        startTime.setHours(startH, startM, 0);

        const endTime = new Date(now);
        endTime.setHours(endH, endM, 0);

        // Handle cases where end time is next day (e.g. 02:00)
        if (endTime < startTime) {
          endTime.setDate(endTime.getDate() + 1);
        }

        const currentTime = new Date();
        
        if (currentTime >= startTime && currentTime <= endTime) {
          setStatus({ isOpen: true, text: `Aberto • Fecha às ${end}` });
        } else {
            if (currentTime < startTime) {
                 setStatus({ isOpen: false, text: `Fechado • Abre às ${start}` });
            } else {
                 setStatus({ isOpen: false, text: "Fechado agora" });
            }
        }
      } catch (e) {
        console.error("Error parsing opening hours", e);
        setStatus({ isOpen: false, text: "Horário indisponível" });
      }
    };

    checkStatus();
    const interval = setInterval(checkStatus, 60000); // Update every minute
  }, [tenant.openingHours]);

  const UserMenu = () => {
    if (customer) {
      return (
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="flex items-center gap-2 font-medium hover:text-white/80 transition-colors p-0 h-auto hover:bg-transparent text-white">
              <User className="w-4 h-4" /> 
              <span className="max-w-[100px] truncate">{customer.name.split(' ')[0]}</span>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-56 bg-white border border-gray-200 shadow-lg z-50">
            <DropdownMenuLabel>Minha Conta</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem className="cursor-pointer" onClick={onEditProfileClick}>
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
      <button 
        onClick={onLoginClick}
        className="flex items-center gap-2 font-medium hover:text-white/80 transition-colors"
      >
        <User className="w-4 h-4" /> Entrar/Cadastrar
      </button>
    );
  };

  return (
    <div className="w-full font-sans">
      {/* Top Menu (Orange Bar) - Visible only on Desktop (lg and up) */}
      <div className="hidden lg:block bg-primary text-white py-3 px-4 shadow-md">
        <div className="container mx-auto flex items-center justify-center gap-8">
          <a href="#" className="flex items-center gap-2 font-medium bg-white text-primary px-6 py-1.5 rounded-full shadow-sm hover:bg-gray-100 transition-colors">
            <Home className="w-4 h-4" /> Início
          </a>
          <a href="#" className="flex items-center gap-2 font-medium hover:text-white/80 transition-colors">
            <Percent className="w-4 h-4" /> Promoções
          </a>
          {customer && (
            <a 
              href="#" 
              onClick={(e) => {
                e.preventDefault();
                onOrdersClick?.();
              }}
              className="flex items-center gap-2 font-medium hover:text-white/80 transition-colors"
            >
              <ShoppingBag className="w-4 h-4" /> Pedidos
            </a>
          )}
          
          <UserMenu />
        </div>
      </div>

      {/* Mobile Decorative Header (Orange Bar) - Visible only on Mobile (below lg) */}
      <div className="lg:hidden bg-primary h-24 w-full"></div>

      {/* Tenant Info Section */}
      <div className="bg-white border-b pb-8 pt-4 -mt-4 rounded-t-[2rem] relative z-10 lg:mt-0 lg:rounded-none">
        <div className="container mx-auto px-4 flex flex-col md:flex-row items-center md:items-start gap-6">
          {/* Logo - Overlapping with negative margin */}
          <div className="relative shrink-0 -mt-16 lg:-mt-12 z-20">
            {tenant.logoUrl ? (
              <img 
                src={tenant.logoUrl} 
                alt={tenant.name} 
                className="w-32 h-32 md:w-40 md:h-40 rounded-full object-cover shadow-lg border-4 border-white bg-white"
              />
            ) : (
              <div className="w-32 h-32 md:w-40 md:h-40 rounded-full bg-gray-200 flex items-center justify-center text-gray-400 font-bold text-3xl shadow-lg border-4 border-white">
                {tenant.name.substring(0, 1)}
              </div>
            )}
          </div>

          {/* Info */}
          <div className="flex-1 text-center md:text-left space-y-3 pt-2">
            <h1 className="text-3xl md:text-4xl font-bold text-gray-900 tracking-tight">
              {tenant.name}
            </h1>
            
            <div className="flex flex-col md:flex-row items-center gap-2 md:gap-4 text-sm md:text-base text-gray-600 flex-wrap justify-center md:justify-start">
              {/* Status */}
              <div className={`flex items-center gap-1 font-semibold ${status.isOpen ? 'text-green-600' : 'text-red-600'}`}>
                 <Clock className="w-4 h-4" />
                 <span>{status.text}</span>
              </div>
              
              <span className="hidden md:inline text-gray-300">|</span>

              <div className="flex items-center gap-1">
                <MapPin className="w-4 h-4" />
                <span>
                    {[
                        tenant.addressNeighborhood, 
                        tenant.addressCity
                    ].filter(Boolean).join(' - ') || "Localização não informada"}
                </span>
              </div>

              <span className="hidden md:inline text-gray-300">|</span>

              <Button 
                variant="link" 
                className="p-0 h-auto text-gray-600 font-semibold hover:text-primary"
                onClick={() => setIsModalOpen(true)}
              >
                Mais informações
              </Button>
            </div>
          </div>
        </div>
      </div>

      <TenantInfoModal 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
        tenant={tenant} 
      />
    </div>
  );
};

export default TenantHeader;
