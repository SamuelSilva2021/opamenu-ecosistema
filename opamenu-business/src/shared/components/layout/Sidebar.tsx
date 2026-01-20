import { LayoutDashboard, Settings, Users, Building2, ChevronRight, LogOut, CreditCard, Package, Gift } from 'lucide-react';
import { Link, useLocation } from 'react-router-dom';
import { clsx } from 'clsx';

const menuItems = [
  { icon: LayoutDashboard, label: 'Dashboard', path: '/' },
  { icon: Building2, label: 'Organização', path: '/organization' },
  { icon: CreditCard, label: 'Planos', path: '/plans' },
  { icon: Package, label: 'Produtos', path: '/products' },
  { icon: Users, label: 'Usuários', path: '/users' },
  { icon: Settings, label: 'Configurações', path: '/settings' },
];

export const Sidebar = () => {
  const location = useLocation();

  return (
    <aside className="w-72 bg-slate-900 text-slate-300 h-screen fixed left-0 top-0 flex flex-col shadow-xl z-20 transition-all duration-300 ease-in-out">
      {/* Brand Section */}
      <div className="h-16 flex items-center px-6 border-b border-slate-800/50 bg-slate-900">
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-lg bg-blue-600 flex items-center justify-center text-white shadow-lg shadow-blue-900/20">
            <Building2 size={18} strokeWidth={2.5} />
          </div>
          <span className="text-lg font-bold text-white tracking-tight">OpaMenu <span className="text-blue-500 font-light">Org</span></span>
        </div>
      </div>
      
      {/* Navigation */}
      <nav className="flex-1 py-6 px-4 space-y-1 overflow-y-auto">
        <div className="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-4 px-2">Menu Principal</div>
        {menuItems.map((item) => {
          const isActive = location.pathname === item.path || (item.path !== '/' && location.pathname.startsWith(item.path));
          const Icon = item.icon;
          
          return (
            <Link
              key={item.path}
              to={item.path}
              className={clsx(
                "group flex items-center justify-between px-3 py-2.5 rounded-lg text-sm font-medium transition-all duration-200",
                isActive 
                  ? "bg-blue-600 text-white shadow-md shadow-blue-900/20" 
                  : "text-slate-400 hover:text-white hover:bg-slate-800"
              )}
            >
              <div className="flex items-center gap-3">
                <Icon size={18} className={clsx("transition-colors", isActive ? "text-white" : "text-slate-500 group-hover:text-white")} />
                {item.label}
              </div>
              {isActive && <ChevronRight size={14} className="text-blue-200" />}
            </Link>
          );
        })}
      </nav>

      {/* User Profile Section */}
      <div className="p-4 border-t border-slate-800 bg-slate-900/50">
        <button className="flex items-center gap-3 w-full p-2 rounded-lg hover:bg-slate-800 transition-colors group text-left">
          <div className="w-9 h-9 rounded-full bg-gradient-to-tr from-blue-500 to-indigo-600 flex items-center justify-center text-white font-bold text-sm shadow-md ring-2 ring-slate-800 group-hover:ring-slate-700 transition-all">
            AD
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-semibold text-white truncate">Admin User</p>
            <p className="text-xs text-slate-500 truncate group-hover:text-slate-400 transition-colors">admin@opamenu.com</p>
          </div>
          <LogOut size={16} className="text-slate-500 group-hover:text-slate-300 transition-colors" />
        </button>
      </div>
    </aside>
  );
};
