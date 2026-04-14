'use client'

import { useParams, useRouter } from 'next/navigation'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion } from 'framer-motion'
import { 
  ChevronLeft, AlertCircle, Bird, 
  Calendar as CalendarIcon, Clock, 
  TrendingDown, Plus, Trash2, Edit, X, Save
} from 'lucide-react'
import Link from 'next/link'
import { toast } from 'sonner'
import { useState } from 'react'
import { cn } from '@/lib/utils'
import { confirmDestructiveAction } from '@/lib/swal'

export default function LoteSanidadPage() {
  const { id } = useParams()
  const router = useRouter()
  const queryClient = useQueryClient()
  const [isEditing, setIsEditing] = useState<string | null>(null)
  const [editForm, setEditForm] = useState({ cantidad: 0, causa: '', fecha: '' })

  const { data: lote } = useQuery({
    queryKey: ['lote', id],
    queryFn: () => api.get<any>(`/api/Lotes/${id}`),
    enabled: !!id
  })

  const { data: bajas, isLoading } = useQuery({
    queryKey: ['mortalidad-lote', id],
    queryFn: () => api.get<any[]>(`/api/Mortalidad/lote/${id}`),
    enabled: !!id
  })

  const deleteMutation = useMutation({
    mutationFn: (bajaId: string) => api.delete(`/api/Mortalidad/${bajaId}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad-lote', id] })
      queryClient.invalidateQueries({ queryKey: ['lote', id] })
      toast.success('Registro eliminado')
    }
  })

  const updateMutation = useMutation({
    mutationFn: (data: { id: string, cantidad: number, causa: string, fecha: string }) => 
      api.put(`/api/Mortalidad/${data.id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad-lote', id] })
      queryClient.invalidateQueries({ queryKey: ['lote', id] })
      toast.success('Registro actualizado')
      setIsEditing(null)
    }
  })

  const handleEdit = (baja: any) => {
    setIsEditing(baja.id)
    setEditForm({
      cantidad: baja.cantidadBajas,
      causa: baja.causa,
      fecha: baja.fecha.split('T')[0]
    })
  }

  const handleSaveEdit = (bajaId: string) => {
    updateMutation.mutate({ 
      id: bajaId, 
      cantidad: Number(editForm.cantidad), 
      causa: editForm.causa, 
      fecha: new Date(editForm.fecha).toISOString() 
    })
  }

  return (
    <div className="space-y-8 pb-20">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Link href={`/lotes/${id}`} className="p-2 hover:bg-white/5 rounded-xl transition-colors text-slate-400">
            <ChevronLeft size={24} />
          </Link>
          <div>
            <h1 className="text-2xl font-black text-white">Historial de Mortalidad</h1>
            <p className="text-xs font-bold text-primary uppercase tracking-widest">{lote?.nombreLote}</p>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* KPI del Lote */}
        <div className="space-y-4">
           <div className="p-6 glass-dark rounded-3xl border border-white/5">
            <div className="flex items-center gap-4 mb-6">
              <div className="p-3 rounded-2xl bg-red-500/10 text-red-500">
                <AlertCircle size={24} />
              </div>
              <div>
                <p className="text-[10px] font-black text-slate-500 uppercase tracking-widest">Mortalidad Actual</p>
                <p className="text-2xl font-black text-white">{lote?.mortalidadPorcentaje?.toFixed(2)}%</p>
              </div>
            </div>
            
            <div className="space-y-3">
              <div className="flex items-center justify-between p-3 bg-white/5 rounded-xl">
                <span className="text-[10px] font-black text-slate-500 uppercase">Aves Iniciales</span>
                <span className="text-sm font-black text-white">{lote?.cantidadAves?.toLocaleString()}</span>
              </div>
              <div className="flex items-center justify-between p-3 bg-white/5 rounded-xl">
                <span className="text-[10px] font-black text-slate-500 uppercase">Total Bajas</span>
                <span className="text-sm font-black text-red-500">{lote?.totalMortalidad?.toLocaleString()}</span>
              </div>
              <div className="flex items-center justify-between p-3 bg-white/5 rounded-xl">
                <span className="text-[10px] font-black text-slate-500 uppercase">Saldo Vivo</span>
                <span className="text-sm font-black text-emerald-500">{lote?.avesVivas?.toLocaleString()}</span>
              </div>
            </div>
          </div>

          <div className="p-6 bg-slate-900/50 rounded-3xl border border-white/5">
             <h4 className="text-white font-black uppercase tracking-widest text-[10px] mb-4">Notas de Bioseguridad</h4>
             <p className="text-xs text-slate-400 leading-relaxed italic">
               "El registro oportuno de bajas permite ajustar la ración de alimento y prevenir desperdicios económicos."
             </p>
          </div>
        </div>

        {/* Listado de Bajas */}
        <div className="lg:col-span-2 space-y-4">
          <div className="flex items-center justify-between mb-2 px-2">
            <h3 className="text-sm font-black text-white uppercase tracking-widest">Registros de este Lote</h3>
          </div>

          <div className="space-y-3">
            {isLoading ? (
               Array.from({ length: 4 }).map((_, i) => (
                <div key={i} className="h-20 glass-dark rounded-2xl animate-pulse" />
              ))
            ) : bajas?.map((baja: any) => (
              <motion.div 
                layout
                key={baja.id} 
                className={cn(
                  "p-5 rounded-3xl border transition-all",
                  isEditing === baja.id ? "bg-primary/5 border-primary/30" : "glass-dark border-white/5 hover:border-white/10"
                )}
              >
                {isEditing === baja.id ? (
                  <div className="space-y-4">
                    <div className="grid grid-cols-2 gap-4">
                      <div className="space-y-1">
                        <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest">Cantidad</label>
                        <input 
                          type="number" 
                          value={editForm.cantidad}
                          onChange={(e) => setEditForm(prev => ({ ...prev, cantidad: Number(e.target.value) }))}
                          className="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-2 text-white font-bold focus:outline-none focus:ring-1 focus:ring-primary/50"
                        />
                      </div>
                      <div className="space-y-1">
                        <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest">Fecha</label>
                        <input 
                          type="date" 
                          value={editForm.fecha}
                          onChange={(e) => setEditForm(prev => ({ ...prev, fecha: e.target.value }))}
                          className="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-2 text-white font-bold focus:outline-none focus:ring-1 focus:ring-primary/50 text-xs"
                        />
                      </div>
                    </div>
                    <div className="space-y-1">
                      <label className="text-[10px] font-black text-slate-500 uppercase tracking-widest">Causa / Observación</label>
                      <input 
                        type="text" 
                        value={editForm.causa}
                        onChange={(e) => setEditForm(prev => ({ ...prev, causa: e.target.value }))}
                        className="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-2 text-white font-medium focus:outline-none focus:ring-1 focus:ring-primary/50"
                      />
                    </div>
                    <div className="flex gap-2 justify-end pt-2">
                      <button onClick={() => setIsEditing(null)} className="p-2 text-slate-500 hover:text-white transition-colors"><X size={20} /></button>
                      <button onClick={() => handleSaveEdit(baja.id)} className="flex items-center gap-2 px-4 py-2 bg-primary text-black font-black rounded-xl text-[10px] uppercase"><Save size={16} /> Guardar</button>
                    </div>
                  </div>
                ) : (
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-5">
                      <div className="w-12 h-12 rounded-xl bg-red-500/10 flex items-center justify-center border border-red-500/20">
                        <span className="text-lg font-black text-red-500 leading-none">{baja.cantidadBajas}</span>
                      </div>
                      <div>
                        <h4 className="text-white font-black text-sm uppercase tracking-tight">{baja.causa}</h4>
                        <div className="flex items-center gap-4 mt-1">
                          <div className="flex items-center gap-1.5 text-slate-500">
                            <Clock size={12} />
                            <span className="text-[10px] font-bold">{new Date(baja.fecha).toLocaleDateString()}</span>
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    <div className="flex items-center gap-2">
                      <button onClick={() => handleEdit(baja)} className="p-2 bg-white/5 hover:bg-white/10 rounded-lg text-slate-500 transition-all">
                        <Edit size={16} />
                      </button>
                      <button 
                        onClick={async () => {
                          const result = await confirmDestructiveAction('¿Eliminar registro?', 'Esta acción no se puede deshacer.')
                          if (result.isConfirmed) {
                            deleteMutation.mutate(baja.id)
                          }
                        }}
                        className="p-2 bg-red-500/5 hover:bg-red-500/10 rounded-lg text-red-500/50 transition-all"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </div>
                )}
              </motion.div>
            ))}

            {bajas?.length === 0 && (
              <div className="p-12 text-center glass-dark rounded-3xl border border-white/5 border-dashed">
                <p className="text-slate-500 font-bold italic">No hay registros de bajas para este lote.</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
