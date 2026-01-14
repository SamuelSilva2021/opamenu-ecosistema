import { Bell, Search, Menu, LogOut, User as UserIcon } from 'lucide-react';
import { useAuth } from '../../../context/AuthContext';

export const Header = () => {
  const { user, logout } = useAuth();

  return (
    <header className="h-16 bg-white border-b border-slate-200 fixed top-0 right-0 left-72 z-10 flex items-center justify-between px-8 transition-all duration-300 ease-in-out">
      <div className="flex items-center gap-4">
        <button className="lg:hidden p-2 -ml-2 text-slate-500 hover:text-slate-700">
          <Menu size={20} />
        </button>
        <div className="relative w-96 hidden md:block">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={16} />
          <input 
            type="text" 
            placeholder="Buscar organizações, usuários..." 
            className="w-full pl-10 pr-4 py-2 rounded-lg border border-slate-200 bg-slate-50 text-slate-700 placeholder-slate-400 focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 focus:bg-white transition-all text-sm"
          />
        </div>
      </div>

      <div className="flex items-center gap-6">
        <button className="relative text-slate-500 hover:text-slate-700 transition-colors">
          <Bell size={20} />
          <span className="absolute top-0 right-0 w-2 h-2 bg-red-500 border-2 border-white rounded-full translate-x-1/4 -translate-y-1/4"></span>
        </button>
        <div className="h-8 w-px bg-slate-200 mx-2"></div>
        <div className="flex items-center gap-3">
          <div className="flex flex-col items-end">
             <span className="text-sm font-medium text-slate-700">{user?.name || 'Usuário'}</span>
             <span className="text-xs text-slate-500">{user?.email}</span>
          </div>
          <div className="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center text-slate-600">
            <UserIcon size={18} />
          </div>
          <button 
            onClick={logout}
            className="p-2 text-slate-400 hover:text-red-500 transition-colors ml-2"
            title="Sair"
          >
            <LogOut size={18} />
          </button>
        </div>
      </div>
    </header>
  );
};
