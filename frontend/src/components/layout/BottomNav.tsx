'use client'

import { useState } from 'react'
import Link from 'next/link'
import { usePathname } from 'next/navigation'
import { motion, AnimatePresence } from 'framer-motion'
import { cn } from '@/lib/utils'
import { navigationItems, navigationSections } from '@/config/navigation'
import { MoreHorizontal, X, LogOut } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'

export function BottomNav() {
  const [showMenu, setShowMenu] = useState(false)
  const pathname = usePathname()
  const { logout, profile } = useAuth()

  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null

  // Filtrar secciones y items según rol
  const filteredSections = navigationSections.map(section => ({
    ...section,
    items: section.items.filter(item => 
      !item.roles || (userRole !== null && item.roles.includes(userRole))
    )
  })).filter(section => section.items.length > 0)

  // Obtener items planos filtrados para mainActions
  const filteredItems = filteredSections.flatMap(s => s.items)

  // Acciones principales para la barra inferior (Dashboard, Chat, Lotes, Inventario)
  const mainActions = [
    filteredItems.find(i => i.href === '/'),
    filteredItems.find(i => i.href === '/chat'),
    filteredItems.find(i => i.href === '/lotes'),
    filteredItems.find(i => i.href === '/inventario'),
  ].filter((item): item is any => !!item)

  return (
    <>
      <nav className="md:hidden fixed bottom-0 left-0 right-0 z-50 glass border-t border-border pb-safe shadow-[0_-10px_20px_rgba(0,0,0,0.1)]">
        <div className="flex items-center justify-around h-16">
          {mainActions.map((item) => {
            const isActive = pathname === item.href
            return (
              <Link
                key={item.name}
                href={item.href}
                className={cn(
                  "flex flex-col items-center justify-center gap-1 w-full h-full transition-all relative",
                  isActive ? "text-primary scale-110" : "text-muted-foreground"
                )}
              >
                <item.icon size={20} className={isActive ? "fill-primary/20" : ""} />
                <span className={cn("text-[9px] font-black uppercase tracking-tighter", isActive ? "opacity-100" : "opacity-60")}>
                  {item.name}
                </span>
                {isActive && (
                  <motion.div 
                    layoutId="activeTabMobile"
                    className="absolute -top-[1px] w-8 h-1 bg-primary rounded-b-full shadow-[0_2px_10px_rgba(59,130,246,0.5)]" 
                  />
                )}
              </Link>
            )
          })}
          
          <button
            onClick={() => setShowMenu(true)}
            className="flex flex-col items-center justify-center gap-1 w-full h-full text-muted-foreground"
          >
            <div className="w-10 h-10 rounded-full bg-muted/50 flex items-center justify-center">
                <MoreHorizontal size={20} />
            </div>
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
              className="fixed inset-0 bg-black/80 backdrop-blur-md z-[60] md:hidden"
            />
            <motion.div
              initial={{ y: '100%' }}
              animate={{ y: 0 }}
              exit={{ y: '100%' }}
              transition={{ type: 'spring', damping: 30, stiffness: 300 }}
              className="fixed bottom-0 left-0 right-0 z-[70] bg-card border-t border-border rounded-t-[2.5rem] p-6 md:hidden max-h-[90vh] overflow-y-auto no-scrollbar shadow-2xl"
            >
              <div className="w-12 h-1.5 bg-muted rounded-full mx-auto mb-6 opacity-30" />
              
              <div className="flex items-center justify-between mb-8">
                <div>
                    <h2 className="text-2xl font-black text-foreground tracking-tighter uppercase">Menú Principal</h2>
                    <p className="text-[10px] text-muted-foreground font-bold uppercase tracking-widest mt-1">Explora GalponERP</p>
                </div>
                <button onClick={() => setShowMenu(false)} className="w-10 h-10 bg-muted/50 rounded-full flex items-center justify-center text-muted-foreground">
                  <X size={20} />
                </button>
              </div>

              <div className="space-y-8 pb-20">
                {filteredSections.map((section) => (
                  <div key={section.title} className="space-y-3">
                    <h3 className="text-[10px] font-black text-muted-foreground uppercase tracking-[0.2em] opacity-50 px-2">
                        {section.title}
                    </h3>
                    <div className="grid grid-cols-2 gap-2">
                      {section.items.map((item) => (
                        <Link
                          key={item.name}
                          href={item.href}
                          onClick={() => setShowMenu(false)}
                          className={cn(
                            "flex items-center gap-3 p-4 rounded-2xl transition-all border",
                            pathname === item.href 
                                ? "bg-primary border-primary text-primary-foreground shadow-lg shadow-primary/20" 
                                : "bg-muted/30 border-border text-muted-foreground"
                          )}
                        >
                          <item.icon size={18} />
                          <span className="text-xs font-bold">{item.name}</span>
                        </Link>
                      ))}
                    </div>
                  </div>
                ))}

                <div className="pt-4">
                    <button
                        onClick={logout}
                        className="w-full flex items-center justify-center gap-3 p-5 rounded-2xl bg-red-500/10 text-red-500 font-black text-xs uppercase tracking-[0.2em] border border-red-500/20"
                    >
                        <LogOut size={20} />
                        <span>Cerrar Sesión</span>
                    </button>
                </div>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </>
  )
}
