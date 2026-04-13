'use client'

import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, AlertCircle, Scale, Droplets } from 'lucide-react'
import { toast } from 'sonner'

interface QuickRecordProps {
  isOpen: boolean
  onClose: () => void
  loteId: string
  type: 'mortality' | 'feed' | 'water'
}

export function QuickRecordModal({ isOpen, onClose, loteId, type }: QuickRecordProps) {
  const [value, setValue] = useState('')
  const [nota, setNota] = useState('')
  const queryClient = useQueryClient()

  const mutation = useMutation({
    mutationFn: (data: any) => {
      const endpoint = type === 'mortality' ? '/api/Mortalidad' : '/api/inventario/consumo-diario'
      return api.post(endpoint, data)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] })
      queryClient.invalidateQueries({ queryKey: ['lote-tendencias', loteId] })
      toast.success('Registro guardado correctamente')
      onClose()
      setValue('')
      setNota('')
    },
    onError: (err: any) => toast.error(err.message),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!value) return
    
    const data = type === 'mortality' 
      ? { loteId, cantidad: Number(value), causa: nota || 'Registro rutinario' }
      : { loteId, cantidad: Number(value), nota }

    mutation.mutate(data)
  }

  const config = {
    mortality: { title: 'Registrar Bajas', icon: AlertCircle, color: 'text-red-500', bg: 'bg-red-500/10', label: 'Cantidad de aves', unit: 'und' },
    feed: { title: 'Consumo Alimento', icon: Scale, color: 'text-blue-500', bg: 'bg-blue-500/10', label: 'Cantidad servida', unit: 'kg' },
    water: { title: 'Consumo Agua', icon: Droplets, color: 'text-amber-500', bg: 'bg-amber-500/10', label: 'Litros consumidos', unit: 'L' },
  }[type]

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[100]" />
          <motion.div initial={{ y: '100%' }} animate={{ y: 0 }} exit={{ y: '100%' }} transition={{ type: 'spring', damping: 25, stiffness: 200 }} className="fixed bottom-0 left-0 right-0 glass-dark z-[110] p-8 rounded-t-[2.5rem] border-t border-white/10" >
            <div className="flex items-center justify-between mb-8">
              <div className="flex items-center gap-4">
                <div className={`p-3 rounded-2xl ${config.bg} ${config.color}`}>
                  <config.icon size={24} />
                </div>
                <h2 className="text-2xl font-black text-white">{config.title}</h2>
              </div>
              <button onClick={onClose} className="p-2 bg-white/5 rounded-full text-slate-400"><X size={24} /></button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="space-y-2">
                <label className="text-xs font-black text-slate-500 uppercase tracking-widest ml-1">{config.label}</label>
                <div className="relative">
                  <input
                    autoFocus
                    type="number"
                    value={value}
                    onChange={(e) => setValue(e.target.value)}
                    className="w-full px-6 py-6 bg-white/5 border border-white/10 rounded-3xl text-4xl font-black text-white focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                    placeholder="0"
                  />
                  <span className="absolute right-6 top-1/2 -translate-y-1/2 text-slate-500 font-black text-xl uppercase">{config.unit}</span>
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-xs font-black text-slate-500 uppercase tracking-widest ml-1">Notas / Observaciones</label>
                <textarea
                  value={nota}
                  onChange={(e) => setNota(e.target.value)}
                  rows={2}
                  className="w-full px-6 py-4 bg-white/5 border border-white/10 rounded-2xl text-white font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
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
