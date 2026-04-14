'use client'

import { useState } from 'react'
import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { motion, AnimatePresence } from 'framer-motion'
import { cn } from '@/lib/utils'
import { navigationItems } from '@/config/navigation'
import { MoreHorizontal, X, LogOut } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'

export function BottomNav() {
  const [showMenu, setShowMenu] = useState(false)
  const pathname = usePathname()
  const { logout } = useAuth()

  // Acciones principales para la barra inferior
  const mainActions = navigationItems.slice(0, 4)

  return (
    <>
      <nav className="md:hidden fixed bottom-0 left-0 right-0 z-50 glass border-t border-border pb-safe">
        <div className="flex items-center justify-around h-16">
          {mainActions.map((item) => {
            const isActive = pathname === item.href
            return (
              <Link
                key={item.name}
                href={item.href}
                className={cn(
                  "flex flex-col items-center justify-center gap-1 w-full h-full transition-colors",
                  isActive ? "text-primary" : "text-muted-foreground"
                )}
              >
                <item.icon size={20} />
                <span className="text-[10px] font-medium uppercase tracking-tighter">{item.name}</span>
                {isActive && (
                  <motion.div 
                    layoutId="activeTab"
                    className="absolute bottom-0 w-8 h-1 bg-primary rounded-t-full" 
                  />
                )}
              </Link>
            )
          })}
          
          <button
            onClick={() => setShowMenu(true)}
            className="flex flex-col items-center justify-center gap-1 w-full h-full text-muted-foreground"
          >
            <MoreHorizontal size={20} />
            <span className="text-[10px] font-medium uppercase tracking-tighter">Más</span>
          </button>
        </div>
      </nav>

      {/* Drawer Menu */}
      <AnimatePresence>
        {showMenu && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              onClick={() => setShowMenu(false)}
              className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[60] md:hidden"
            />
            <motion.div
              initial={{ y: '100%' }}
              animate={{ y: 0 }}
              exit={{ y: '100%' }}
              transition={{ type: 'spring', damping: 25, stiffness: 200 }}
              className="fixed bottom-0 left-0 right-0 z-[70] glass rounded-t-3xl p-6 md:hidden max-h-[80vh] overflow-y-auto"
            >
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-xl font-bold text-foreground">Menú</h2>
                <button onClick={() => setShowMenu(false)} className="p-2 bg-muted/50 rounded-full text-muted-foreground">
                  <X size={20} />
                </button>
              </div>

              <div className="grid grid-cols-2 gap-3 mb-8">
                {navigationItems.map((item) => (
                  <Link
                    key={item.name}
                    href={item.href}
                    onClick={() => setShowMenu(false)}
                    className={cn(
                      "flex items-center gap-3 p-4 rounded-2xl transition-all",
                      pathname === item.href ? "bg-primary text-primary-foreground" : "bg-muted/50 text-muted-foreground"
                    )}
                  >
                    <item.icon size={20} />
                    <span className="font-medium">{item.name}</span>
                  </Link>
                ))}
              </div>

              <button
                onClick={logout}
                className="w-full flex items-center justify-center gap-3 p-4 rounded-2xl bg-destructive/10 text-destructive font-medium"
              >
                <LogOut size={20} />
                Cerrar Sesión
              </button>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </>
  )
}
