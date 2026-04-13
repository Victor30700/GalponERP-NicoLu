'use client'

import { Search, Bell, User } from 'lucide-react'
import { usePathname } from 'next/navigation'
import { navigationItems } from '@/config/navigation'
import { useAuth } from '@/context/AuthContext'

export function TopNavbar() {
  const pathname = usePathname()
  const { profile } = useAuth()
  
  // Encontrar el nombre de la página actual basado en la ruta
  const currentPage = navigationItems.find(item => item.href === pathname)?.name || 'Dashboard'

  return (
    <header className="h-16 flex items-center justify-between px-6 glass sticky top-0 z-30 border-b border-white/5">
      <div className="flex items-center gap-4">
        <h2 className="text-slate-400 font-medium md:flex hidden items-center gap-2">
          <span>Sistema</span>
          <span className="w-1 h-1 bg-slate-600 rounded-full" />
          <span className="text-white">{currentPage}</span>
        </h2>
        {/* En móvil mostrar el logo en el navbar superior */}
        <h1 className="text-xl font-bold text-white md:hidden tracking-tight">
          Galpon<span className="text-primary">ERP</span>
        </h1>
      </div>

      <div className="flex items-center gap-4">
        <div className="relative md:block hidden">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
          <input
            type="text"
            placeholder="Buscar..."
            className="pl-10 pr-4 py-2 bg-white/5 border border-white/5 rounded-full text-sm text-white focus:outline-none focus:ring-1 focus:ring-primary/50 transition-all w-64"
          />
        </div>
        
        <button className="p-2 text-slate-400 hover:text-white transition-colors relative">
          <Bell size={20} />
          <span className="absolute top-2 right-2 w-2 h-2 bg-primary rounded-full" />
        </button>

        <div className="flex items-center gap-3 pl-4 border-l border-white/10">
          <div className="text-right md:block hidden">
            <p className="text-xs font-semibold text-white">{profile?.nombre || 'Usuario'}</p>
            <p className="text-[10px] text-slate-500 uppercase tracking-wider">{profile?.rol || 'Empleado'}</p>
          </div>
          <div className="w-9 h-9 bg-primary/20 rounded-full flex items-center justify-center text-primary border border-primary/20 overflow-hidden">
            {profile?.nombre ? (
              <span className="text-xs font-bold">{profile.nombre.charAt(0)}</span>
            ) : (
              <User size={18} />
            )}
          </div>
        </div>
      </div>
    </header>
  )
}
