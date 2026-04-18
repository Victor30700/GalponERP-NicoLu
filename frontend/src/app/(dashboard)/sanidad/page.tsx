'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  ShieldCheck, AlertCircle, TrendingDown, 
  Calendar as CalendarIcon, Filter, 
  ChevronRight, ArrowUpRight, 
  Clock, Bird, Trash2, Edit, Save, X, Plus, Droplets, ClipboardCheck
} from 'lucide-react'
import { toast } from 'sonner'
import { cn } from '@/lib/utils'
import { confirmDestructiveAction } from '@/lib/swal'
import { SanitaryEventModal } from '@/components/production/SanitaryEventModal'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/lib/rbac'

export default function SanidadPage() {
  const { profile } = useAuth()
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null
  const isEmpleado = userRole === UserRole.Empleado

  const queryClient = useQueryClient()
  const [isSanitaryModalOpen, setIsSanitaryModalOpen] = useState(false)
  const [dateRange, setDateRange] = useState({
    inicio: new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().split('T')[0],
    fin: new Date().toISOString().split('T')[0]
  })
  const [isEditing, setIsEditing] = useState<string | null>(null)
  const [editForm, setEditForm] = useState({ cantidad: 0, causa: '', fecha: '' })

  const { data: reporte, isLoading: isLoadingReporte } = useQuery({
    queryKey: ['mortalidad-reporte', dateRange],
    queryFn: () => api.get<any>(`/api/Mortalidad/reporte-transversal?inicio=${dateRange.inicio}&fin=${dateRange.fin}`)
  })

  const { data: todasBajas, isLoading: isLoadingBajas } = useQuery({
    queryKey: ['mortalidad-todas'],
    queryFn: () => api.get<any[]>('/api/Mortalidad')
  })

  const { data: lotesActivos = [] } = useQuery({
    queryKey: ['lotes-activos-sanidad'],
    queryFn: () => api.get<any[]>('/api/Lotes?soloActivos=true')
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Mortalidad/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad-todas'] })
      queryClient.invalidateQueries({ queryKey: ['mortalidad-reporte'] })
      toast.success('Registro eliminado')
    }
  })

  const updateMutation = useMutation({
    mutationFn: (data: { id: string, cantidad: number, causa: string, fecha: string }) => 
      api.put(`/api/Mortalidad/${data.id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad-todas'] })
      queryClient.invalidateQueries({ queryKey: ['mortalidad-reporte'] })
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

  const handleSaveEdit = (id: string) => {
    updateMutation.mutate({ 
      id, 
      cantidad: Number(editForm.cantidad), 
      causa: editForm.causa, 
      fecha: new Date(editForm.fecha).toISOString() 
    })
  }

  return (
    <div className="space-y-8">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-black text-foreground flex items-center gap-3">
            <ShieldCheck size={32} className="text-primary" />
            Control Sanitario
          </h1>
          <p className="text-muted-foreground mt-1">Gestión de mortalidad y bioseguridad del plantel.</p>
        </div>

        <div className="flex items-center gap-3">
          <div className="flex items-center gap-2 bg-muted/50 p-1.5 rounded-2xl border border-border">
            <div className="flex items-center gap-2 px-3 py-2">
              <CalendarIcon size={16} className="text-muted-foreground" />
              <input 
                type="date" 
                value={dateRange.inicio}
                onChange={(e) => setDateRange(prev => ({ ...prev, inicio: e.target.value }))}
                className="bg-transparent text-xs font-bold text-foreground focus:outline-none"
              />
            </div>
            <div className="w-px h-4 bg-muted/50" />
            <div className="flex items-center gap-2 px-3 py-2">
              <input 
                type="date" 
                value={dateRange.fin}
                onChange={(e) => setDateRange(prev => ({ ...prev, fin: e.target.value }))}
                className="bg-transparent text-xs font-bold text-foreground focus:outline-none"
              />
            </div>
          </div>

          <button 
            onClick={() => setIsSanitaryModalOpen(true)}
            className="flex items-center gap-2 px-6 py-4 bg-primary text-black font-black rounded-2xl text-xs uppercase tracking-widest hover:scale-105 transition-all shadow-lg shadow-primary/20"
          >
            <Plus size={18} /> Registrar Sanidad
          </button>
        </div>
      </div>

      {/* Reporte Transversal Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} className="p-6 glass rounded-3xl border border-border relative overflow-hidden group">
          <div className="absolute top-0 right-0 p-4 opacity-10 group-hover:opacity-20 transition-opacity">
            <AlertCircle size={80} />
          </div>
          <p className="text-[10px] font-black text-red-500 uppercase tracking-[0.2em] mb-1">Mortalidad Total</p>
          <h2 className="text-5xl font-black text-foreground mb-2">{reporte?.totalBajas || 0}</h2>
          <p className="text-xs text-muted-foreground font-medium">Aves perdidas en el periodo seleccionado.</p>
        </motion.div>

        <div className="md:col-span-2 p-6 glass rounded-3xl border border-border">
          <p className="text-[10px] font-black text-primary uppercase tracking-[0.2em] mb-4">Distribución por Causa</p>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            {reporte?.porCausa?.map((item: any, idx: number) => (
              <div key={idx} className="flex items-center justify-between p-3 bg-muted/50 rounded-2xl border border-border">
                <div className="flex items-center gap-3">
                  <div className="w-2 h-2 rounded-full bg-primary shadow-[0_0_8px_rgba(234,255,0,0.5)]" />
                  <span className="text-xs font-bold text-foreground truncate max-w-[120px]">{item.causa}</span>
                </div>
                <div className="flex items-center gap-3">
                  <span className="text-xs font-black text-foreground">{item.cantidad}</span>
                  <span className="text-[10px] font-bold text-muted-foreground bg-slate-800 px-2 py-0.5 rounded-full">{item.porcentaje.toFixed(1)}%</span>
                </div>
              </div>
            ))}
            {(!reporte?.porCausa || reporte?.porCausa.length === 0) && (
              <div className="col-span-2 py-8 text-center">
                <p className="text-muted-foreground text-xs italic">No hay datos para mostrar.</p>
              </div>
            )}
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Historial Detallado */}
        <div className="lg:col-span-2 space-y-4">
          <div className="flex items-center justify-between mb-2 px-2">
            <h3 className="text-sm font-black text-foreground uppercase tracking-widest">Registros Recientes</h3>
            <span className="text-[10px] font-bold text-muted-foreground uppercase">Total: {todasBajas?.length || 0}</span>
          </div>

          <div className="space-y-3">
            {isLoadingBajas ? (
               Array.from({ length: 3 }).map((_, i) => (
                <div key={i} className="h-24 glass rounded-2xl animate-pulse" />
              ))
            ) : todasBajas?.map((baja: any) => (
              <motion.div 
                layout
                key={baja.id} 
                className={cn(
                  "p-5 rounded-3xl border transition-all",
                  isEditing === baja.id ? "bg-primary/5 border-primary/30" : "glass border-border hover:border-border"
                )}
              >
                {isEditing === baja.id ? (
                  <div className="space-y-4">
                    <div className="grid grid-cols-2 gap-4">
                      <div className="space-y-1">
                        <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Cantidad</label>
                        <input 
                          type="number" 
                          value={editForm.cantidad}
                          onChange={(e) => setEditForm(prev => ({ ...prev, cantidad: Number(e.target.value) }))}
                          className="w-full bg-muted/50 border border-border rounded-xl px-4 py-2 text-foreground font-bold focus:outline-none focus:ring-1 focus:ring-primary/50"
                        />
                      </div>
                      <div className="space-y-1">
                        <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Fecha</label>
                        <input 
                          type="date" 
                          value={editForm.fecha}
                          onChange={(e) => setEditForm(prev => ({ ...prev, fecha: e.target.value }))}
                          className="w-full bg-muted/50 border border-border rounded-xl px-4 py-2 text-foreground font-bold focus:outline-none focus:ring-1 focus:ring-primary/50 text-xs"
                        />
                      </div>
                    </div>
                    <div className="space-y-1">
                      <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Causa / Observación</label>
                      <input 
                        type="text" 
                        value={editForm.causa}
                        onChange={(e) => setEditForm(prev => ({ ...prev, causa: e.target.value }))}
                        className="w-full bg-muted/50 border border-border rounded-xl px-4 py-2 text-foreground font-medium focus:outline-none focus:ring-1 focus:ring-primary/50"
                      />
                    </div>
                    <div className="flex gap-2 justify-end pt-2">
                      <button onClick={() => setIsEditing(null)} className="p-2 text-muted-foreground hover:text-foreground transition-colors"><X size={20} /></button>
                      <button onClick={() => handleSaveEdit(baja.id)} className="flex items-center gap-2 px-4 py-2 bg-primary text-black font-black rounded-xl text-[10px] uppercase"><Save size={16} /> Guardar</button>
                    </div>
                  </div>
                ) : (
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-5">
                      <div className="w-14 h-14 rounded-2xl bg-red-500/10 flex flex-col items-center justify-center border border-red-500/20">
                        <span className="text-xl font-black text-red-500 leading-none">{baja.cantidadBajas}</span>
                        <span className="text-[8px] font-black text-red-500/70 uppercase">aves</span>
                      </div>
                      <div>
                        <div className="flex items-center gap-2 mb-1">
                          <h4 className="text-foreground font-black text-sm uppercase tracking-tight">{baja.causa}</h4>
                          <span className="px-2 py-0.5 bg-muted/50 rounded text-[8px] font-black text-muted-foreground uppercase">ID: {baja.loteId.split('-')[0]}</span>
                        </div>
                        <div className="flex items-center gap-4">
                          <div className="flex items-center gap-1.5 text-muted-foreground">
                            <Clock size={12} />
                            <span className="text-[10px] font-bold">{new Date(baja.fecha).toLocaleDateString()}</span>
                          </div>
                          <div className="flex items-center gap-1.5 text-muted-foreground">
                            <Bird size={12} />
                            <span className="text-[10px] font-bold">Lote: {baja.loteNombre || baja.loteId.split('-')[0]}</span>
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    <div className="flex items-center gap-2">
                      <button onClick={() => handleEdit(baja)} className="p-2.5 bg-muted/50 hover:bg-muted/50 rounded-xl text-muted-foreground hover:text-primary transition-all">
                        <Edit size={18} />
                      </button>
                      {!isEmpleado && (
                        <button 
                          onClick={async () => {
                            const result = await confirmDestructiveAction('¿Eliminar registro?', 'Esta acción no se puede deshacer.')
                            if (result.isConfirmed) {
                              deleteMutation.mutate(baja.id)
                            }
                          }}
                          className="p-2.5 bg-red-500/5 hover:bg-red-500/10 rounded-xl text-red-500/50 hover:text-red-500 transition-all"
                        >
                          <Trash2 size={18} />
                        </button>
                      )}
                    </div>
                  </div>
                )}
              </motion.div>
            ))}

            {todasBajas?.length === 0 && (
              <div className="p-12 text-center glass rounded-3xl border border-border border-dashed">
                <AlertCircle size={40} className="mx-auto text-slate-700 mb-4" />
                <p className="text-muted-foreground font-bold italic">No hay registros de mortalidad aún.</p>
              </div>
            )}
          </div>
        </div>

        {/* Info lateral / Consejos */}
        <div className="space-y-6">
          <div className="p-6 bg-primary rounded-[2rem] text-black">
            <h4 className="font-black uppercase tracking-tighter text-lg leading-tight mb-2">Importancia del Registro Sanitario</h4>
            <p className="text-xs font-bold text-black/70 leading-relaxed mb-4">
              Mantener al día la mortalidad permite detectar brotes tempranos y calcular el FCR real de cada lote.
            </p>
            <div className="p-4 bg-black/5 rounded-2xl">
              <div className="flex items-center justify-between mb-2">
                <span className="text-[10px] font-black uppercase opacity-60">Meta Biológica</span>
                <span className="text-xs font-black">{'<'} 3%</span>
              </div>
              <div className="w-full h-1.5 bg-black/10 rounded-full overflow-hidden">
                <div className="w-1/3 h-full bg-black rounded-full" />
              </div>
            </div>
          </div>

          <div className="p-6 glass rounded-[2rem] border border-border">
            <h4 className="text-foreground font-black uppercase tracking-widest text-xs mb-4">Lotes Activos</h4>
            <div className="space-y-3">
               {lotesActivos.map((lote: any) => (
                 <div key={lote.id} className="p-4 bg-muted/50 rounded-2xl border border-border flex items-center justify-between group cursor-pointer hover:bg-muted/50 transition-all"
                    onClick={() => window.location.href = `/lotes/${lote.id}`}
                 >
                    <div>
                      <p className="text-xs font-black text-foreground uppercase">{lote.nombre || lote.nombreLote}</p>
                      <p className="text-[10px] text-muted-foreground font-bold uppercase">{lote.galponNombre}</p>
                    </div>
                    <div className="p-2 bg-slate-800 rounded-lg text-muted-foreground group-hover:text-primary transition-colors">
                      <ChevronRight size={16} />
                    </div>
                 </div>
               ))}
               {lotesActivos.length === 0 && (
                 <p className="text-[10px] text-muted-foreground italic">No hay lotes activos actualmente.</p>
               )}
            </div>
          </div>

          <div className="p-6 glass rounded-[2rem] border border-border">
            <h4 className="text-foreground font-black uppercase tracking-widest text-xs mb-4">Próximas Actividades</h4>
            <div className="space-y-4">
                <div className="flex gap-3 items-start">
                   <div className="p-2 bg-emerald-500/10 text-emerald-500 rounded-lg">
                      <ClipboardCheck size={16} />
                   </div>
                   <div>
                      <p className="text-[10px] font-bold text-foreground leading-tight">Accede al detalle de cada lote para ver su calendario específico.</p>
                      <p className="text-[8px] text-muted-foreground uppercase mt-1 font-black">Rastro de Sanidad Activo</p>
                   </div>
                </div>
            </div>
          </div>
        </div>
      </div>

      <SanitaryEventModal 
        isOpen={isSanitaryModalOpen}
        onClose={() => setIsSanitaryModalOpen(false)}
      />
    </div>
  )
}


