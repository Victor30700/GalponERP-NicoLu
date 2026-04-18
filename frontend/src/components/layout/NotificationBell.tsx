'use client'

import { useState, useEffect } from 'react'
import { Bell, Package, AlertTriangle, X } from 'lucide-react'
import { useProductos } from '@/hooks/useProductos'
import { motion, AnimatePresence } from 'framer-motion'
import { cn } from '@/lib/utils'
import Link from 'next/link'

export function NotificationBell() {
  const [isOpen, setIsOpen] = useState(false)
  const { productos } = useProductos()
  
  // Filtrar productos con stock bajo el umbral mínimo
  const lowStockProducts = productos.filter(p => p.isActive && p.stockActual <= p.umbralMinimo)
  const hasNotifications = lowStockProducts.length > 0

  return (
    <div className="relative">
      <button 
        onClick={() => setIsOpen(!isOpen)}
        className={cn(
          "p-2 text-muted-foreground hover:text-foreground transition-colors relative rounded-lg hover:bg-muted/50",
          isOpen && "bg-muted/50 text-foreground"
        )}
      >
        <Bell size={20} />
        {hasNotifications && (
          <span className="absolute top-2 right-2 w-2 h-2 bg-red-500 rounded-full border-2 border-background animate-pulse" />
        )}
      </button>

      <AnimatePresence>
        {isOpen && (
          <>
            {/* Overlay para cerrar al hacer click fuera */}
            <div 
              className="fixed inset-0 z-40" 
              onClick={() => setIsOpen(false)} 
            />
            
            <motion.div
              initial={{ opacity: 0, y: 10, scale: 0.95 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              exit={{ opacity: 0, y: 10, scale: 0.95 }}
              transition={{ duration: 0.2 }}
              className="absolute right-[-60px] md:right-0 mt-2 w-[calc(100vw-2rem)] md:w-80 bg-card border border-border rounded-2xl shadow-2xl z-50 overflow-hidden"
            >
              <div className="p-4 border-b border-border flex items-center justify-between bg-muted/30">
                <h3 className="font-bold text-sm">Notificaciones</h3>
                <span className="text-[10px] bg-primary/20 text-primary px-2 py-0.5 rounded-full font-bold">
                  {lowStockProducts.length} Alertas
                </span>
              </div>

              <div className="max-h-96 overflow-y-auto">
                {lowStockProducts.length === 0 ? (
                  <div className="p-8 text-center">
                    <div className="w-12 h-12 bg-muted rounded-full flex items-center justify-center mx-auto mb-3 text-muted-foreground">
                      <Bell size={20} />
                    </div>
                    <p className="text-xs text-muted-foreground">No tienes notificaciones pendientes</p>
                  </div>
                ) : (
                  <div className="divide-y divide-border">
                    {lowStockProducts.map((producto) => (
                      <Link 
                        key={producto.id}
                        href={`/inventario`}
                        onClick={() => setIsOpen(false)}
                        className="p-4 flex gap-3 hover:bg-muted/50 transition-colors block"
                      >
                        <div className="w-10 h-10 rounded-xl bg-red-500/10 flex items-center justify-center text-red-500 flex-shrink-0">
                          <AlertTriangle size={18} />
                        </div>
                        <div className="flex-1 min-w-0">
                          <p className="text-xs font-bold text-foreground truncate">Stock Crítico: {producto.nombre}</p>
                          <p className="text-[10px] text-muted-foreground mt-1">
                            Quedan <span className="text-red-500 font-bold">{producto.stockActual} {producto.unidadMedidaNombre}</span>. 
                            El umbral mínimo es {producto.umbralMinimo}.
                          </p>
                          <p className="text-[9px] text-primary font-bold uppercase mt-2">Gestionar Inventario →</p>
                        </div>
                      </Link>
                    ))}
                  </div>
                )}
              </div>
              
              {hasNotifications && (
                <div className="p-3 bg-muted/30 border-t border-border text-center">
                  <Link 
                    href="/inventario"
                    onClick={() => setIsOpen(false)}
                    className="text-[10px] font-black uppercase tracking-widest text-muted-foreground hover:text-foreground transition-colors"
                  >
                    Ver todo el inventario
                  </Link>
                </div>
              )}
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
