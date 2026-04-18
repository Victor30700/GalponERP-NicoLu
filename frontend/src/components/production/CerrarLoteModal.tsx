'use client'

import { useState } from 'react'
import { useLote } from '@/hooks/useLotes'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Lock, Calendar, DollarSign, FileText } from 'lucide-react'
import { toast } from 'sonner'

interface CerrarLoteModalProps {
  isOpen: boolean
  onClose: () => void
  loteId: string
  loteNombre: string
}

export function CerrarLoteModal({ isOpen, onClose, loteId, loteNombre }: CerrarLoteModalProps) {
  const { cerrarLote } = useLote(loteId)

  const [formData, setFormData] = useState({
    fechaCierre: new Date().toISOString().split('T')[0],
    precioVentaPromedio: '',
    observaciones: ''
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!formData.fechaCierre || !formData.precioVentaPromedio) {
      toast.error('Por favor complete los campos obligatorios')
      return
    }

    cerrarLote.mutate({
      loteId,
      fechaCierre: new Date(formData.fechaCierre).toISOString(),
      precioVentaPromedio: Number(formData.precioVentaPromedio),
      observaciones: formData.observaciones
    }, {
      onSuccess: () => {
        toast.success('Lote cerrado correctamente')
        onClose()
      },
      onError: (err: any) => toast.error(err.message || 'Error al cerrar lote')
    })
  }

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div 
            initial={{ opacity: 0 }} 
            animate={{ opacity: 1 }} 
            exit={{ opacity: 0 }} 
            onClick={onClose} 
            className="fixed inset-0 bg-black/80 backdrop-blur-md z-[150]" 
          />
          <motion.div 
            initial={{ opacity: 0, scale: 0.95, y: 20 }} 
            animate={{ opacity: 1, scale: 1, y: 0 }} 
            exit={{ opacity: 0, scale: 0.95, y: 20 }} 
            className="fixed inset-0 m-auto w-full max-w-lg h-fit glass z-[160] p-8 rounded-[2.5rem] border border-border shadow-2xl"
          >
            <div className="flex items-center justify-between mb-8">
              <div className="flex items-center gap-4">
                <div className="p-3 rounded-2xl bg-emerald-500/10 text-emerald-500">
                  <Lock size={24} />
                </div>
                <div>
                  <h2 className="text-2xl font-black text-foreground uppercase tracking-tighter">Cerrar Lote</h2>
                  <p className="text-xs text-muted-foreground font-bold uppercase tracking-widest">{loteNombre}</p>
                </div>
              </div>
              <button onClick={onClose} className="p-2 bg-muted/50 rounded-full text-muted-foreground hover:bg-muted/80 transition-colors">
                <X size={24} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="space-y-4">
                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Fecha de Cierre</label>
                  <div className="relative">
                    <input
                      type="date"
                      value={formData.fechaCierre}
                      onChange={(e) => setFormData({ ...formData, fechaCierre: e.target.value })}
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-emerald-500/50 transition-all"
                    />
                    <Calendar size={18} className="absolute right-5 top-1/2 -translate-y-1/2 text-muted-foreground" />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Precio Venta Promedio (por kg)</label>
                  <div className="relative">
                    <input
                      type="number"
                      step="0.01"
                      value={formData.precioVentaPromedio}
                      onChange={(e) => setFormData({ ...formData, precioVentaPromedio: e.target.value })}
                      placeholder="Ej: 45.50"
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-emerald-500/50 transition-all"
                    />
                    <DollarSign size={18} className="absolute right-5 top-1/2 -translate-y-1/2 text-muted-foreground" />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Observaciones Finales</label>
                  <div className="relative">
                    <textarea
                      value={formData.observaciones}
                      onChange={(e) => setFormData({ ...formData, observaciones: e.target.value })}
                      placeholder="Detalles adicionales sobre el cierre del ciclo..."
                      rows={3}
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-emerald-500/50 transition-all resize-none"
                    />
                    <FileText size={18} className="absolute right-5 top-6 text-muted-foreground" />
                  </div>
                </div>
              </div>

              <div className="pt-4">
                <button
                  type="submit"
                  disabled={cerrarLote.isPending}
                  className="w-full py-5 bg-emerald-500 hover:bg-emerald-600 text-black font-black rounded-3xl transition-all flex items-center justify-center gap-3 disabled:opacity-50 shadow-xl shadow-emerald-500/20 active:scale-95"
                >
                  <Lock size={24} />
                  FINALIZAR Y CERRAR LOTE
                </button>
                <p className="text-[10px] text-center text-muted-foreground mt-4 font-bold uppercase tracking-widest">
                  Esta acción es irreversible y calculará los KPIs finales de liquidación.
                </p>
              </div>
            </form>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  )
}
