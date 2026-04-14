'use client'

import { Search, Bell, User, LogOut, Sun, Moon } from 'lucide-react'
import { usePathname } from 'next/navigation'
import { navigationItems } from '@/config/navigation'
import { useAuth } from '@/context/AuthContext'
import { useTheme } from '@/context/ThemeContext'
import { motion } from 'framer-motion'

export function TopNavbar() {
  const pathname = usePathname()
  const { profile, logout } = useAuth()
  const { theme, toggleTheme } = useTheme()
  
  // Encontrar el nombre de la página actual basado en la ruta
  const currentPage = navigationItems.find(item => item.href === pathname)?.name || 'Dashboard'

  return (
    <header className="h-16 flex items-center justify-between px-6 glass sticky top-0 z-30 border-b border-border">
      <div className="flex items-center gap-4">
        <h2 className="text-muted-foreground font-medium md:flex hidden items-center gap-2">
          <span>Sistema</span>
          <span className="w-1 h-1 bg-muted-foreground/30 rounded-full" />
          <span className="text-foreground">{currentPage}</span>
        </h2>
        {/* En móvil mostrar el logo en el navbar superior */}
        <h1 className="text-xl font-bold text-foreground md:hidden tracking-tight">
          Galpon<span className="text-primary">ERP</span>
        </h1>
      </div>

      <div className="flex items-center gap-4">
        <div className="relative md:block hidden">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
          <input
            type="text"
            placeholder="Buscar..."
            className="pl-10 pr-4 py-2 bg-muted/50 border border-border rounded-full text-sm text-foreground focus:outline-none focus:ring-1 focus:ring-primary/50 transition-all w-64"
          />
        </div>
        
        {/* Toggle de Tema */}
        <button 
          onClick={toggleTheme}
          className="p-2 text-muted-foreground hover:text-foreground transition-colors relative rounded-lg hover:bg-muted/50"
          title={theme === 'dark' ? 'Cambiar a modo claro' : 'Cambiar a modo oscuro'}
        >
          <motion.div
            initial={false}
            animate={{ rotate: theme === 'dark' ? 0 : 180, scale: 1 }}
            transition={{ type: 'spring', stiffness: 200, damping: 10 }}
          >
            {theme === 'dark' ? <Moon size={20} /> : <Sun size={20} />}
          </motion.div>
        </button>

        <button className="p-2 text-muted-foreground hover:text-foreground transition-colors relative rounded-lg hover:bg-muted/50">
          <Bell size={20} />
          <span className="absolute top-2 right-2 w-2 h-2 bg-primary rounded-full" />
        </button>

        <div className="flex items-center gap-3 pl-4 border-l border-border">
          <div className="text-right md:block hidden">
            <p className="text-xs font-semibold text-foreground">{profile?.nombre || 'Usuario'}</p>
            <p className="text-[10px] text-muted-foreground uppercase tracking-wider">{profile?.rol || 'Empleado'}</p>
          </div>
          <div className="w-9 h-9 bg-primary/20 rounded-full flex items-center justify-center text-primary border border-primary/20 overflow-hidden">
            {profile?.nombre ? (
              <span className="text-xs font-bold">{profile.nombre.charAt(0)}</span>
            ) : (
              <User size={18} />
            )}
          </div>
          
          <button 
            onClick={logout}
            className="p-2 text-muted-foreground hover:text-destructive transition-colors rounded-lg hover:bg-destructive/10"
            title="Cerrar Sesión"
          >
            <LogOut size={20} />
          </button>
        </div>
      </div>
    </header>
  )
}
