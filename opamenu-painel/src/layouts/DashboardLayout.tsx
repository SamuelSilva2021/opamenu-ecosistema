import { Outlet, Navigate } from "react-router-dom";
import { UserNav } from "@/components/layout/UserNav";
import { useAuthStore } from "@/store/auth.store";
import { useState } from "react";
import { PanelLeftClose, PanelLeftOpen } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Sidebar, MobileSidebar } from "@/components/layout/Sidebar";

export default function DashboardLayout() {
  const { isAuthenticated } = useAuthStore();
  const [isCollapsed, setIsCollapsed] = useState(false);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="flex min-h-screen bg-gray-50/50 dark:bg-zinc-900">
      <Sidebar isCollapsed={isCollapsed} />
      
      <div className="flex-1 flex flex-col min-h-screen transition-all duration-300">
        <header className="sticky top-0 z-10 flex h-16 items-center gap-4 border-b bg-background/95 px-4 md:px-6 backdrop-blur shadow-sm justify-between">
            <div className="flex items-center gap-4">
                <Button 
                    variant="ghost" 
                    size="icon" 
                    onClick={() => setIsCollapsed(!isCollapsed)}
                    className="hidden md:flex"
                >
                    {isCollapsed ? <PanelLeftOpen className="h-5 w-5" /> : <PanelLeftClose className="h-5 w-5" />}
                </Button>
                <div className="md:hidden flex items-center gap-2">
                    <MobileSidebar />
                    <span className="font-semibold text-lg">OpaMenu</span>
                </div>
            </div>
            
            <div className="flex items-center gap-4">
                <UserNav />
            </div>
        </header>
        
        <main className="flex-1 p-4 md:p-6 overflow-y-auto">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
