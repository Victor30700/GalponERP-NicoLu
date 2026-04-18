'use client'

import { useState } from 'react'
import { User, LogOut, Settings, ChevronDown } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'
import { motion, AnimatePresence } from 'framer-motion'
import { cn } from '@/lib/utils'
import Link from 'next/link'

export function UserDropdown() {
  const [isOpen, setIsOpen] = useState(false)
  const { profile, logout } = useAuth()

  return (
    <div className="relative">
      <button 
        onClick={() => setIsOpen(!isOpen)}
        className={cn(
          "flex items-center gap-3 pl-4 border-l border-border hover:bg-muted/30 transition-all rounded-r-lg py-1 pr-2",
          isOpen && "bg-muted/30"
        )}
      >
        <div className="text-right md:block hidden">
          <p className="text-xs font-semibold text-foreground leading-tight">{profile?.nombre || 'Usuario'}</p>
          <p className="text-[10px] text-muted-foreground uppercase tracking-wider">{profile?.rol || 'Empleado'}</p>
        </div>
        
        <div className="relative">
          <div className="w-9 h-9 bg-primary/20 rounded-full flex items-center justify-center text-primary border border-primary/20 overflow-hidden shadow-sm">
            {profile?.nombre ? (
              <span className="text-xs font-bold">{profile.nombre.charAt(0)}</span>
            ) : (
              <User size={18} />
            )}
          </div>
          <div className="absolute -bottom-0.5 -right-0.5 w-3 h-3 bg-emerald-500 border-2 border-background rounded-full" />
        </div>
        
        <ChevronDown 
          size={14} 
          className={cn("text-muted-foreground transition-transform duration-200", isOpen && "rotate-180")} 
        />
      </button>

      <AnimatePresence>
        {isOpen && (
          <>
            <div 
              className="fixed inset-0 z-40" 
              onClick={() => setIsOpen(false)} 
            />
            
            <motion.div
              initial={{ opacity: 0, y: 10, scale: 0.95 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              exit={{ opacity: 0, y: 10, scale: 0.95 }}
              transition={{ duration: 0.2 }}
              className="absolute right-0 mt-2 w-[calc(100vw-2rem)] md:w-56 bg-card border border-border rounded-2xl shadow-2xl z-50 overflow-hidden"
            >
              <div className="p-4 bg-muted/30 border-b border-border">
                <p className="text-sm font-bold text-foreground truncate">{profile?.nombre}</p>
                <p className="text-[10px] text-muted-foreground truncate">{profile?.email || 'usuario@sistema.com'}</p>
              </div>

              <div className="p-2">
                <Link 
                  href="/configuracion"
                  onClick={() => setIsOpen(false)}
                  className="w-full flex items-center gap-3 px-3 py-2 text-sm text-muted-foreground hover:text-foreground hover:bg-muted/50 rounded-xl transition-all"
                >
                  <Settings size={18} />
                  <span>Configuración</span>
                </Link>
                
                <hr className="my-2 border-border mx-2" />
                
                <button 
                  onClick={() => {
                    setIsOpen(false)
                    logout()
                  }}
                  className="w-full flex items-center gap-3 px-3 py-2 text-sm text-red-500 hover:bg-red-500/10 rounded-xl transition-all font-medium"
                >
                  <LogOut size={18} />
                  <span>Cerrar Sesión</span>
                </button>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
