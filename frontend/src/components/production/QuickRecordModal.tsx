'use client'

import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, AlertCircle, Scale, Droplets, Ruler } from 'lucide-react'
import { toast } from 'sonner'

interface QuickRecordProps {
  isOpen: boolean
  onClose: () => void
  loteId: string
  type: 'mortality' | 'feed' | 'water' | 'weight'
  lote?: any
}

export function QuickRecordModal({ isOpen, onClose, loteId, type, lote }: QuickRecordProps) {
  const [value, setValue] = useState('')
  const [secondaryValue, setSecondaryValue] = useState('') // Para cantidad muestreada o temperatura
  const [tertiaryValue, setTertiaryValue] = useState('') // Para humedad
  const [nota, setNota] = useState('')
  const queryClient = useQueryClient()

  const mutation = useMutation({
    mutationFn: (data: any) => {
      let endpoint = '/api/Mortalidad'
      if (type === 'feed') endpoint = '/api/inventario/consumo-diario'
      if (type === 'water') endpoint = '/api/Sanidad/bienestar'
      if (type === 'weight') endpoint = '/api/Pesajes'
      
      return api.post(endpoint, data)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] })
      queryClient.invalidateQueries({ queryKey: ['lote-tendencias', loteId] })
      queryClient.invalidateQueries({ queryKey: ['pesajes', 'lote', loteId] })
      queryClient.invalidateQueries({ queryKey: ['sanidad'] })
      toast.success('Registro guardado correctamente')
      onClose()
      setValue('')
      setSecondaryValue('')
      setTertiaryValue('')
      setNota('')
    },
    onError: (err: any) => toast.error(err.message),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!value) return
    
    let data: any = {}

    if (type === 'mortality') {
      data = { loteId, cantidad: Number(value), causa: nota || 'Registro rutinario', fecha: new Date().toISOString() }
    } else if (type === 'feed') {
      if (!lote?.productoId) {
          toast.error('El lote no tiene un producto de alimento asignado.')
          return
      }
      data = { 
          loteId, 
          productoId: lote.productoId,
          cantidad: Number(value), 
          justificacion: nota || 'Consumo diario' 
      }
    } else if (type === 'water') {
      data = { 
          loteId, 
          fecha: new Date().toISOString(),
          consumoAgua: Number(value),
          temperatura: Number(secondaryValue) || 0,
          humedad: Number(tertiaryValue) || 0,
          observaciones: nota 
      }
    } else if (type === 'weight') {
      data = {
        loteId,
        fecha: new Date().toISOString(),
        pesoPromedioGramos: Number(value),
        cantidadMuestreada: Number(secondaryValue) || 10
      }
    }

    mutation.mutate(data)
  }

  const config = {
    mortality: { title: 'Registrar Bajas', icon: AlertCircle, color: 'text-red-500', bg: 'bg-red-500/10', label: 'Cantidad de aves', unit: 'und' },
    feed: { title: 'Consumo Alimento', icon: Scale, color: 'text-blue-500', bg: 'bg-blue-500/10', label: 'Cantidad servida', unit: 'kg' },
    water: { title: 'Consumo Agua', icon: Droplets, color: 'text-amber-500', bg: 'bg-amber-500/10', label: 'Litros consumidos', unit: 'L' },
    weight: { title: 'Registro de Pesaje', icon: Ruler, color: 'text-indigo-500', bg: 'bg-indigo-500/10', label: 'Peso Promedio', unit: 'g' },
  }[type]

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[100]" />
          <motion.div initial={{ y: '100%' }} animate={{ y: 0 }} exit={{ y: '100%' }} transition={{ type: 'spring', damping: 25, stiffness: 200 }} className="fixed bottom-0 left-0 right-0 glass z-[110] p-8 rounded-t-[2.5rem] border-t border-border" >
            <div className="flex items-center justify-between mb-8">
              <div className="flex items-center gap-4">
                <div className={`p-3 rounded-2xl ${config.bg} ${config.color}`}>
                  <config.icon size={24} />
                </div>
                <h2 className="text-2xl font-black text-foreground">{config.title}</h2>
              </div>
              <button onClick={onClose} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={24} /></button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="space-y-2">
                <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">{config.label}</label>
                <div className="relative">
                  <input
                    autoFocus
                    type="number"
                    value={value}
                    onChange={(e) => setValue(e.target.value)}
                    className="w-full px-6 py-6 bg-muted/50 border border-border rounded-3xl text-4xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                    placeholder="0"
                  />
                  <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-xl uppercase">{config.unit}</span>
                </div>
              </div>

              {type === 'weight' && (
                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Cantidad Muestreada</label>
                  <div className="relative">
                    <input
                      type="number"
                      value={secondaryValue}
                      onChange={(e) => setSecondaryValue(e.target.value)}
                      className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-2xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                      placeholder="10"
                    />
                    <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-sm uppercase">aves</span>
                  </div>
                </div>
              )}

              {type === 'water' && (
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Temperatura</label>
                    <div className="relative">
                      <input
                        type="number"
                        step="0.1"
                        value={secondaryValue}
                        onChange={(e) => setSecondaryValue(e.target.value)}
                        className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-2xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                        placeholder="0.0"
                      />
                      <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-sm uppercase">°C</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Humedad</label>
                    <div className="relative">
                      <input
                        type="number"
                        step="0.1"
                        value={tertiaryValue}
                        onChange={(e) => setTertiaryValue(e.target.value)}
                        className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-2xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                        placeholder="0.0"
                      />
                      <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-sm uppercase">%</span>
                    </div>
                  </div>
                </div>
              )}

              <div className="space-y-2">
                <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Notas / Observaciones</label>
                <textarea
                  value={nota}
                  onChange={(e) => setNota(e.target.value)}
                  rows={2}
                  className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                  placeholder="Opcional..."
                />
              </div>


              <button
                type="submit"
                disabled={mutation.isPending || !value}
                className="w-full py-5 bg-primary hover:bg-primary/90 text-black font-black rounded-3xl transition-all flex items-center justify-center gap-3 disabled:opacity-50 shadow-xl shadow-primary/20 active:scale-95"
              >
                <Save size={24} />
                GUARDAR REGISTRO
              </button>
            </form>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  )
}

