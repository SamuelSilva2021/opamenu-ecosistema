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
          <DropdownMenuContent align="end" className="w-64 bg-white/95 backdrop-blur-xl border-2 border-border/50 shadow-2xl z-50 mt-2 rounded-[2rem] p-2">
            <DropdownMenuLabel className="text-xs font-black uppercase tracking-widest text-muted-foreground px-4 py-3">
              Minha Conta
            </DropdownMenuLabel>
            <DropdownMenuSeparator className="bg-border/50 mx-2" />
            <DropdownMenuItem 
              className="cursor-pointer rounded-xl font-bold p-3 focus:bg-primary/5 focus:text-primary transition-colors gap-3" 
              onClick={onEditProfileClick}
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
      {/* Top Menu (Desktop Bar) */}
      <div className="hidden lg:block bg-primary text-white py-4 px-4 shadow-lg">
        <div className="container mx-auto flex items-center justify-between">
          <div className="flex items-center gap-8">
            <a href="#" className="flex items-center gap-2 font-black text-xl tracking-tighter uppercase italic">
              {tenant.name}
            </a>
            <div className="flex items-center gap-6">
              <a href="#" className="flex items-center gap-2 font-bold hover:text-white/80 transition-colors">
                <Home className="w-4 h-4" /> Início
              </a>
              <a href="#" className="flex items-center gap-2 font-bold hover:text-white/80 transition-colors">
                <Percent className="w-4 h-4" /> Ofertas
              </a>
              {customer && (
                <a
                  href="#"
                  onClick={(e) => {
                    e.preventDefault();
                    onOrdersClick?.();
                  }}
                  className="flex items-center gap-2 font-bold hover:text-white/80 transition-colors"
                >
                  <ShoppingBag className="w-4 h-4" /> Meus Pedidos
                </a>
              )}
            </div>
          </div>

          <UserMenu />
        </div>
      </div>

      {/* Mobile Header Hero Section */}
      <div className="lg:hidden relative">
        <div className="bg-primary h-32 w-full rounded-b-[2.5rem] shadow-inner relative overflow-hidden">
          {/* Subtle pattern or gradient overlay */}
          <div className="absolute inset-0 opacity-20 bg-[radial-gradient(circle_at_center,_var(--opamenu-orange-light)_0%,_transparent_70%)]"></div>
        </div>

        <div className="px-4 -mt-16 relative z-20">
          <div className="bg-white rounded-3xl p-5 shadow-xl border border-border/50">
            <div className="flex items-start gap-4">
              {/* Logo */}
              <div className="shrink-0">
                {tenant.logoUrl ? (
                  <img
                    src={tenant.logoUrl}
                    alt={tenant.name}
                    className="w-24 h-24 rounded-2xl object-cover shadow-md border-4 border-white bg-white"
                  />
                ) : (
                  <div className="w-24 h-24 rounded-2xl bg-muted flex items-center justify-center text-primary font-black text-3xl shadow-md border-4 border-white">
                    {tenant.name.substring(0, 1)}
                  </div>
                )}
              </div>

              {/* Quick Info */}
              <div className="flex-1 min-w-0 pt-1">
                <h1 className="text-2xl font-black text-foreground leading-tight tracking-tight truncate mb-1">
                  {tenant.name}
                </h1>

                <div className="flex flex-col gap-1.5">
                  <div className={`flex items-center gap-1.5 text-xs font-bold px-2 py-0.5 rounded-full w-fit ${status.isOpen ? 'bg-green-100 text-green-700' : 'bg-red-100 text-red-700'}`}>
                    <Clock className="w-3.5 h-3.5" />
                    <span>{status.text}</span>
                  </div>

                  <div className="flex items-center gap-1 text-sm text-text-secondary font-medium truncate">
                    <MapPin className="w-3.5 h-3.5 shrink-0" />
                    <span className="truncate">
                      {tenant.addressNeighborhood || "Bairro não informado"}
                    </span>
                  </div>
                </div>
              </div>
            </div>

            <div className="mt-4 pt-4 border-t border-border/50 flex items-center justify-between">
              <Button
                variant="ghost"
                className="p-0 h-auto text-primary font-bold hover:bg-transparent flex items-center gap-1.5"
                onClick={() => setIsModalOpen(true)}
              >
                <Settings className="w-4 h-4" />
                Ver detalhes da loja
              </Button>

              {!customer && (
                <Button
                  size="sm"
                  className="rounded-full font-bold bg-accent text-accent-foreground hover:bg-accent/90"
                  onClick={onLoginClick}
                >
                  Entrar
                </Button>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Desktop Info (Old layout kept for desktop but refined) */}
      <div className="hidden lg:block bg-white border-b py-8 relative z-10">
        <div className="container mx-auto px-4 flex items-center gap-8">
          <div className="relative shrink-0">
            {tenant.logoUrl ? (
              <img
                src={tenant.logoUrl}
                alt={tenant.name}
                className="w-40 h-40 rounded-3xl object-cover shadow-2xl border-4 border-white bg-white"
              />
            ) : (
              <div className="w-40 h-40 rounded-3xl bg-muted flex items-center justify-center text-primary font-black text-5xl shadow-2xl border-4 border-white">
                {tenant.name.substring(0, 1)}
              </div>
            )}
          </div>

          <div className="flex-1 space-y-4">
            <h1 className="text-5xl font-black text-foreground tracking-tighter uppercase italic">
              {tenant.name}
            </h1>

            <div className="flex items-center gap-6 text-base font-bold text-text-secondary">
              <div className={`flex items-center gap-2 ${status.isOpen ? 'text-green-600' : 'text-red-600'}`}>
                <Clock className="w-5 h-5" />
                <span>{status.text}</span>
              </div>

              <span className="text-border">|</span>

              <div className="flex items-center gap-2">
                <MapPin className="w-5 h-5" />
                <span>
                  {[
                    tenant.addressNeighborhood,
                    tenant.addressCity
                  ].filter(Boolean).join(' - ') || "Localização não informada"}
                </span>
              </div>

              <span className="text-border">|</span>

              <Button
                variant="link"
                className="p-0 h-auto text-primary font-black uppercase tracking-widest text-xs hover:text-primary-hover"
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
