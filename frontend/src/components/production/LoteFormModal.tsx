'use client'

import { useState, useEffect } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, Bird, Calendar, Hash, DollarSign, ClipboardList } from 'lucide-react'
import { toast } from 'sonner'

interface LoteFormModalProps {
  isOpen: boolean
  onClose: () => void
  lote?: any // If provided, we are editing
}

export function LoteFormModal({ isOpen, onClose, lote }: LoteFormModalProps) {
  const queryClient = useQueryClient()
  const isEditing = !!lote

  const [formData, setFormData] = useState({
    galponId: '',
    fechaIngreso: new Date().toISOString().split('T')[0],
    cantidadInicial: '',
    costoUnitarioPollito: '',
    plantillaSanitariaId: ''
  })

  useEffect(() => {
    if (lote) {
      setFormData({
        galponId: lote.galponId || '',
        fechaIngreso: lote.fechaIngreso ? lote.fechaIngreso.split('T')[0] : new Date().toISOString().split('T')[0],
        cantidadInicial: lote.cantidadInicial?.toString() || '',
        costoUnitarioPollito: lote.costoUnitarioPollito?.toString() || '',
        plantillaSanitariaId: lote.plantillaSanitariaId || ''
      })
    } else {
      setFormData({
        galponId: '',
        fechaIngreso: new Date().toISOString().split('T')[0],
        cantidadInicial: '',
        costoUnitarioPollito: '',
        plantillaSanitariaId: ''
      })
    }
  }, [lote, isOpen])

  const { data: galpones = [] } = useQuery({
    queryKey: ['galpones'],
    queryFn: () => api.get<any[]>('/api/Galpones'),
    enabled: isOpen
  })

  const { data: plantillas = [] } = useQuery({
    queryKey: ['plantillas'],
    queryFn: () => api.get<any[]>('/api/Plantillas'),
    enabled: isOpen
  })

  const mutation = useMutation({
    mutationFn: (data: any) => {
      if (isEditing) {
        // Omitir plantillaSanitariaId ya que ActualizarLoteCommand no lo soporta
        const { plantillaSanitariaId, ...updateData } = data
        return api.put(`/api/Lotes/${lote.id}`, { ...updateData, id: lote.id })
      }
      return api.post('/api/Lotes', data)
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lotes'] })
      if (isEditing) {
        queryClient.invalidateQueries({ queryKey: ['lote', lote.id] })
      }
      toast.success(isEditing ? 'Lote actualizado correctamente' : 'Lote creado correctamente')
      onClose()
    },
    onError: (err: any) => toast.error(err.message || 'Error al procesar la solicitud'),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!formData.galponId || !formData.fechaIngreso || !formData.cantidadInicial || !formData.costoUnitarioPollito) {
      toast.error('Por favor complete todos los campos obligatorios')
      return
    }

    const payload = {
      ...formData,
      cantidadInicial: Number(formData.cantidadInicial),
      costoUnitarioPollito: Number(formData.costoUnitarioPollito),
      fechaIngreso: new Date(formData.fechaIngreso).toISOString()
    }

    mutation.mutate(payload)
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
            className="fixed inset-0 bg-black/80 backdrop-blur-md z-[100]" 
          />
          <motion.div 
            initial={{ opacity: 0, scale: 0.95, y: 20 }} 
            animate={{ opacity: 1, scale: 1, y: 0 }} 
            exit={{ opacity: 0, scale: 0.95, y: 20 }} 
            className="fixed inset-0 m-auto w-full max-w-xl h-fit glass z-[110] p-8 rounded-[2.5rem] border border-border shadow-2xl"
          >
            <div className="flex items-center justify-between mb-8">
              <div className="flex items-center gap-4">
                <div className="p-3 rounded-2xl bg-primary/10 text-primary">
                  <Bird size={24} />
                </div>
                <h2 className="text-2xl font-black text-foreground">
                  {isEditing ? 'Editar Lote' : 'Nuevo Lote'}
                </h2>
              </div>
              <button onClick={onClose} className="p-2 bg-muted/50 rounded-full text-muted-foreground hover:bg-muted/80 transition-colors">
                <X size={24} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Galpón</label>
                  <div className="relative">
                    <select
                      value={formData.galponId}
                      onChange={(e) => setFormData({ ...formData, galponId: e.target.value })}
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all appearance-none"
                    >
                      <option value="" className="bg-background">Seleccionar Galpón</option>
                      {galpones.map((g: any) => (
                        <option key={g.id} value={g.id} className="bg-background">{g.nombre}</option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Fecha de Ingreso</label>
                  <div className="relative">
                    <input
                      type="date"
                      value={formData.fechaIngreso}
                      onChange={(e) => setFormData({ ...formData, fechaIngreso: e.target.value })}
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Cantidad Inicial</label>
                  <div className="relative">
                    <input
                      type="number"
                      value={formData.cantidadInicial}
                      onChange={(e) => setFormData({ ...formData, cantidadInicial: e.target.value })}
                      placeholder="Ej: 2500"
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                    <Hash size={18} className="absolute right-5 top-1/2 -translate-y-1/2 text-muted-foreground" />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Costo Unitario Pollito</label>
                  <div className="relative">
                    <input
                      type="number"
                      step="0.01"
                      value={formData.costoUnitarioPollito}
                      onChange={(e) => setFormData({ ...formData, costoUnitarioPollito: e.target.value })}
                      placeholder="Ej: 10.50"
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                    <DollarSign size={18} className="absolute right-5 top-1/2 -translate-y-1/2 text-muted-foreground" />
                  </div>
                </div>

                <div className="md:col-span-2 space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Plantilla Sanitaria</label>
                  <div className="relative">
                    <select
                      value={formData.plantillaSanitariaId}
                      onChange={(e) => setFormData({ ...formData, plantillaSanitariaId: e.target.value })}
                      className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all appearance-none"
                    >
                      <option value="" className="bg-background">Sin Plantilla (Manual)</option>
                      {plantillas.map((p: any) => (
                        <option key={p.id} value={p.id} className="bg-background">{p.nombre}</option>
                      ))}
                    </select>
                    <ClipboardList size={18} className="absolute right-5 top-1/2 -translate-y-1/2 text-muted-foreground" />
                  </div>
                </div>
              </div>


              <div className="pt-4">
                <button
                  type="submit"
                  disabled={mutation.isPending}
                  className="w-full py-5 bg-primary hover:bg-primary/90 text-black font-black rounded-3xl transition-all flex items-center justify-center gap-3 disabled:opacity-50 shadow-xl shadow-primary/20 active:scale-95"
                >
                  <Save size={24} />
                  {isEditing ? 'ACTUALIZAR LOTE' : 'GUARDAR LOTE'}
                </button>
              </div>
            </form>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  )
}

