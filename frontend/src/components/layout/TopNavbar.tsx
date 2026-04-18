'use client'

import { Bell, User, LogOut, Sun, Moon } from 'lucide-react'
import { usePathname } from 'next/navigation'
import { navigationItems } from '@/config/navigation'
import { useAuth } from '@/context/AuthContext'
import { useTheme } from '@/context/ThemeContext'
import { motion } from 'framer-motion'
import { NotificationBell } from './NotificationBell'
import { UserDropdown } from './UserDropdown'

export function TopNavbar() {
  const pathname = usePathname()
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

      <div className="flex items-center gap-2">
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

        <NotificationBell />

        <UserDropdown />
      </div>
    </header>
  )
}
