import { useState } from "react";
import { NavLink } from "react-router-dom";
import { cn } from "@/lib/utils";
import { useAuthStore } from "@/store/auth.store";
import { hasPermission } from "@/lib/permissions";
import {
  LayoutDashboard,
  UtensilsCrossed,
  Menu,
  Settings,
  Users,
  ShoppingBag,
  FileText,
  Layers,
  Tags,
  FolderTree,
  Ticket,
  CreditCard
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { Badge } from "@/components/ui/badge";

interface SidebarProps extends React.HTMLAttributes<HTMLDivElement> {
  isCollapsed?: boolean;
}

const routes = [
  {
    title: "Dashboard",
    icon: LayoutDashboard,
    href: "/dashboard",
    variant: "default",
    module: "DASHBOARD",
  },
  {
    title: "Grupos de Adicionais",
    icon: Tags,
    href: "/dashboard/addon-groups",
    variant: "ghost",
    module: "ADITIONAL_GROUP",
  },
    {
    title: "Adicionais",
    icon: Layers,
    href: "/dashboard/addons",
    variant: "ghost",
    module: "ADITIONAL",
  },
  {
    title: "Categorias",
    icon: FolderTree,
    href: "/dashboard/categories",
    variant: "ghost",
    module: "CATEGORY",
  },
  {
    title: "Produtos",
    icon: UtensilsCrossed,
    href: "/dashboard/products",
    variant: "ghost",
    module: "PRODUCT",
  },
  {
    title: "Cupons",
    icon: Ticket,
    href: "/dashboard/coupons",
    variant: "ghost",
    module: "COUPON",
  },
  {
    title: "Pedidos",
    icon: ShoppingBag,
    href: "/dashboard/orders",
    variant: "ghost",
    module: "ORDER",
  },
  {
    title: "Clientes",
    icon: Users,
    href: "/dashboard/customers",
    variant: "ghost",
    module: "CUSTOMER",
  },
  {
    title: "Relatórios",
    icon: FileText,
    href: "/dashboard/reports",
    variant: "ghost",
    comingSoon: true,
    module: "REPORT",
  },
  {
    title: "Assinatura",
    icon: CreditCard,
    href: "/dashboard/subscription",
    variant: "ghost",
    module: "SUBSCRIPTION",
  },
  {
    title: "Configurações",
    icon: Settings,
    href: "/dashboard/settings",
    variant: "ghost",
    module: "SETTINGS",
  },
];

function SidebarContent({ isCollapsed }: { isCollapsed?: boolean }) {
  const { user } = useAuthStore();
  
  const filteredRoutes = routes.filter(route => 
    hasPermission(user, route.module, "READ")
  );

  return (
    <div className="flex h-full flex-col bg-zinc-900 text-white">
      <div className={cn("flex h-16 items-center px-4 border-b border-zinc-800", isCollapsed ? "justify-center" : "px-6")}>
        <div className="flex items-center gap-2">
            <div className="bg-primary rounded-lg p-1">
                <UtensilsCrossed className="h-5 w-5 text-primary-foreground" />
            </div>
            {!isCollapsed && (
            <span className="text-lg font-bold tracking-tight">
                OpaMenu
            </span>
            )}
        </div>
      </div>
      <ScrollArea className="flex-1 py-4">
        <nav className="grid gap-1 px-2">
          {filteredRoutes.map((route) => (
            <TooltipProvider key={route.href} delayDuration={0}>
              <Tooltip>
                <TooltipTrigger asChild>
                  {/* @ts-ignore - comingSoon property */}
                  {route.comingSoon ? (
                    <div
                      className={cn(
                        "flex flex-row items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-all duration-200 whitespace-nowrap opacity-50 cursor-not-allowed text-zinc-400",
                        isCollapsed && "justify-center px-2"
                      )}
                    >
                      <div className="items-center justify-center shrink-0 grid grid-cols-12 w-full">
                        <route.icon className={cn("h-5 w-5 col-span-2")} />
                        {!isCollapsed && (
                            <div className="col-span-10 flex items-center justify-between gap-2">
                                <span>{route.title}</span>
                                <Badge variant="secondary" className="text-[10px] px-1 py-0 h-5">Em breve</Badge>
                            </div>
                        )}
                      </div>
                    </div>
                  ) : (
                    <NavLink
                        to={route.href}
                        end={route.href === "/dashboard"}
                        className={({ isActive }) =>
                        cn(
                            "flex flex-row items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition-all duration-200 whitespace-nowrap",
                            isActive 
                                ? "bg-primary text-primary-foreground shadow-md" 
                                : "text-zinc-400 hover:text-white hover:bg-zinc-800/50",
                            isCollapsed && "justify-center px-2"
                        )
                        }
                    >
                        <div className="items-center justify-center shrink-0 grid grid-cols-12 w-full">
                            <route.icon className={cn("h-5 w-5 col-span-2")} />
                            {!isCollapsed && <span className="inline-block col-span-10">{route.title}</span>}  
                        </div>
                    </NavLink>
                  )}
                </TooltipTrigger>
                {isCollapsed && (
                  <TooltipContent side="right" className="flex items-center gap-4 bg-zinc-900 text-white border-zinc-800">
                    {route.title}
                    {/* @ts-ignore */}
                    {route.comingSoon && <span className="text-xs text-zinc-500">(Em breve)</span>}
                  </TooltipContent>
                )}
              </Tooltip>
            </TooltipProvider>
          ))}
        </nav>
      </ScrollArea>
      <div className="p-4 border-t border-zinc-800">
         {!isCollapsed && (
            <div className="rounded-lg bg-zinc-800/50 p-4">
                <div className="flex items-center gap-2 mb-2">
                    <div className="h-2 w-2 rounded-full bg-green-500 animate-pulse" />
                    <span className="text-xs font-medium text-zinc-400">Sistema Online</span>
                </div>
                <p className="text-xs text-zinc-500">v1.0.0</p>
            </div>
         )}
      </div>
    </div>
  );
}

export function MobileSidebar() {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <Sheet open={isOpen} onOpenChange={setIsOpen}>
      <SheetTrigger asChild>
        <Button variant="ghost" className="md:hidden -ml-2 h-10 w-10 p-0 text-zinc-500">
          <Menu className="h-6 w-6" />
          <span className="sr-only">Toggle Menu</span>
        </Button>
      </SheetTrigger>
      <SheetContent side="left" className="w-72 p-0 bg-zinc-900 border-r-zinc-800 text-white">
        <SidebarContent />
      </SheetContent>
    </Sheet>
  );
}

export function Sidebar({ className, isCollapsed }: SidebarProps) {
  return (
    <div
      className={cn(
        "hidden border-r border-zinc-800 bg-zinc-900 md:block h-screen sticky top-0 transition-all duration-300 shadow-xl z-20",
        isCollapsed ? "w-[80px]" : "w-72",
        className
      )}
    >
      <SidebarContent isCollapsed={isCollapsed} />
    </div>
  );
}
