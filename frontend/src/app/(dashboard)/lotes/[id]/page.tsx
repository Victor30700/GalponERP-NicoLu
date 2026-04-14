'use client'

import { useParams, useRouter } from 'next/navigation'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  Bird, TrendingUp, AlertCircle, DollarSign, Plus, 
  ChevronLeft, ClipboardCheck, Scale, Droplets, PieChart as PieChartIcon,
  Settings, Lock, Unlock, XCircle, FileText, ArrowLeftRight, Trash2, Edit,
  Calendar, Ruler, History
} from 'lucide-react'
import Link from 'next/link'
import { toast } from 'sonner'
import { useState } from 'react'
import dynamic from 'next/dynamic'
import { LoteFormModal } from '@/components/production/LoteFormModal'
import { ManualActivityModal } from '@/components/production/ManualActivityModal'
import { cn } from '@/lib/utils'
import { confirmAction, confirmDestructiveAction, promptAction } from '@/lib/swal'
import { useCalendarioSanitario, EstadoCalendario, TipoActividad } from '@/hooks/useCalendarioSanitario'
import { useProyeccionSacrificio } from '@/hooks/useDashboard'
import { usePesajes } from '@/hooks/usePesajes'

// Optimización: Lazy Loading de componentes pesados
const QuickRecordModal = dynamic(() => import('@/components/production/QuickRecordModal').then(mod => mod.QuickRecordModal), { ssr: false })

const ResponsiveContainer = dynamic(() => import('recharts').then(mod => mod.ResponsiveContainer), { ssr: false })
const AreaChart = dynamic(() => import('recharts').then(mod => mod.AreaChart), { ssr: false })
const Area = dynamic(() => import('recharts').then(mod => mod.Area), { ssr: false })
const LineChart = dynamic(() => import('recharts').then(mod => mod.LineChart), { ssr: false })
const Line = dynamic(() => import('recharts').then(mod => mod.Line), { ssr: false })
const XAxis = dynamic(() => import('recharts').then(mod => mod.XAxis), { ssr: false })
const YAxis = dynamic(() => import('recharts').then(mod => mod.YAxis), { ssr: false })
const CartesianGrid = dynamic(() => import('recharts').then(mod => mod.CartesianGrid), { ssr: false })
const Tooltip = dynamic(() => import('recharts').then(mod => mod.Tooltip), { ssr: false })

export default function LoteDashboard() {
  const { id } = useParams()
  const router = useRouter()
  const queryClient = useQueryClient()
  const [activeTab, setActiveTab] = useState<'overview' | 'daily' | 'sanitary' | 'actions'>('overview')
  const [recordType, setRecordType] = useState<'mortality' | 'feed' | 'water' | 'weight' | null>(null)
  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [isTransferModalOpen, setIsTransferModalOpen] = useState(false)
  const [isManualModalOpen, setIsManualModalOpen] = useState(false)
  const [newGalponId, setNewGalponId] = useState('')

  const { data: lote, isLoading: isLoadingLote } = useQuery({
    queryKey: ['lote', id],
    queryFn: () => api.get<any>(`/api/Lotes/${id}`),
    enabled: !!id
  })

  const { pesajes, isLoading: isLoadingPesajes, eliminarPesaje } = usePesajes(id as string)

  const { data: galpones = [] } = useQuery({
    queryKey: ['galpones'],
    queryFn: () => api.get<any[]>('/api/Galpones'),
    enabled: isTransferModalOpen && !!id
  })

  const { data: tendenciasResponse } = useQuery({
    queryKey: ['lote-tendencias', id],
    queryFn: () => api.get<any>(`/api/Mortalidad/lote/${id}/tendencias`),
    enabled: !!id
  })

  // Normalizar tendencias
  const tendencias = Array.isArray(tendenciasResponse) 
    ? tendenciasResponse 
    : (tendenciasResponse?.tendencias || [])

  const { data: rendimiento } = useQuery({
    queryKey: ['lote-rendimiento', id],
    queryFn: () => api.get<any>(`/api/Lotes/${id}/rendimiento-vivo`),
    enabled: !!id
  })

  // Hook de Proyección de Sacrificio
  const { data: proyeccion } = useProyeccionSacrificio(id as string)

  // Hook de Calendario Sanitario
  const { 
    calendario, 
    aplicarActividad, 
    agregarActividadManual, 
    reprogramarActividad 
  } = useCalendarioSanitario(id as string)

  // Actions Mutations
  const cerrarLote = useMutation({
    mutationFn: () => api.post(`/api/Lotes/${id}/cerrar`, { LoteId: id }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lote', id] })
      toast.success('Lote cerrado correctamente')
    },
  })

  const reabrirLote = useMutation({
    mutationFn: () => api.put(`/api/Lotes/${id}/reabrir`, {}),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lote', id] })
      toast.success('Lote reabierto correctamente')
    },
  })

  const cancelarLote = useMutation({
    mutationFn: (justificacion: string) => api.post(`/api/Lotes/${id}/cancelar`, justificacion),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lote', id] })
      toast.success('Lote cancelado correctamente')
    },
  })

  const eliminarLote = useMutation({
    mutationFn: () => api.delete(`/api/Lotes/${id}`),
    onSuccess: () => {
      toast.success('Lote eliminado correctamente')
      router.push('/lotes')
    },
  })

  const trasladarLote = useMutation({
    mutationFn: (galponId: string) => api.post(`/api/Lotes/${id}/trasladar`, galponId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['lote', id] })
      toast.success('Lote trasladado correctamente')
      setIsTransferModalOpen(false)
    },
  })

  const descargarReporte = () => {
    const baseUrl = process.env.NEXT_PUBLIC_API_URL || ''
    window.open(`${baseUrl}/api/Lotes/${id}/reporte-cierre-pdf`, '_blank')
  }

  if (isLoadingLote) return (
    <div className="h-96 flex items-center justify-center">
      <div className="w-12 h-12 border-4 border-primary/20 border-t-primary rounded-full animate-spin" />
    </div>
  )

  const kpis = [
    { label: 'Pollos Vivos', value: lote?.avesVivas?.toLocaleString() || '0', sub: `${lote?.mortalidadPorcentaje?.toFixed(1)}% mortalidad`, icon: Bird, color: 'text-emerald-500', bg: 'bg-emerald-500/10' },
    { label: 'Conversión (FCR)', value: lote?.fcrActual?.toFixed(2) || '0.00', sub: 'Eficiencia alimenticia', icon: TrendingUp, color: 'text-blue-500', bg: 'bg-blue-500/10' },
    { label: 'Costo Acumulado', value: `$${lote?.costoTotalAcumulado?.toLocaleString() || '0'}`, sub: 'Inversión total', icon: DollarSign, color: 'text-primary', bg: 'bg-primary/10' },
    { label: 'Días de Vida', value: rendimiento?.diasDeVida || '0', sub: 'Edad cronológica', icon: Calendar, color: 'text-amber-500', bg: 'bg-amber-500/10' },
  ]

  return (
    <div className="space-y-6 pb-20">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Link href="/lotes" className="p-2 hover:bg-muted/50 rounded-xl transition-colors text-muted-foreground">
            <ChevronLeft size={24} />
          </Link>
          <div>
            <h1 className="text-2xl font-black text-foreground">{lote?.nombreLote}</h1>
            <div className="flex items-center gap-2">
              <p className="text-xs font-bold text-primary uppercase tracking-widest">{lote?.galponNombre}</p>
              <span className={cn(
                "px-2 py-0.5 rounded text-[10px] font-black uppercase",
                lote?.estado === 'Activo' ? "bg-emerald-500/10 text-emerald-500" : 
                lote?.estado === 'Cerrado' ? "bg-slate-500/10 text-muted-foreground" : "bg-red-500/10 text-red-500"
              )}>
                {lote?.estado}
              </span>
            </div>
          </div>
        </div>
        <div className="flex gap-2">
          <button 
            onClick={() => setIsEditModalOpen(true)}
            className="p-2 bg-muted/50 hover:bg-muted/50 rounded-xl text-muted-foreground transition-colors"
          >
            <Edit size={20} />
          </button>
          <button 
            onClick={() => setRecordType('mortality')}
            className="hidden md:flex items-center gap-2 px-4 py-2 bg-primary text-black font-black rounded-xl text-xs uppercase tracking-tighter"
          >
            <Plus size={16} /> Registro Rápido
          </button>
        </div>
      </div>

      {/* Tabs Selector */}
      <div className="flex p-1 bg-muted/50 rounded-2xl border border-border overflow-x-auto no-scrollbar">
        {[
          { id: 'overview', label: 'Resumen', icon: PieChartIcon },
          { id: 'daily', label: 'Operación', icon: ClipboardCheck },
          { id: 'sanitary', label: 'Sanidad', icon: Droplets },
          { id: 'actions', label: 'Acciones', icon: Settings },
        ].map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id as any)}
            className={`flex-1 flex items-center justify-center gap-2 py-3 px-4 rounded-xl text-xs font-bold uppercase tracking-widest transition-all whitespace-nowrap ${
              activeTab === tab.id ? 'bg-primary text-black shadow-lg shadow-primary/20' : 'text-muted-foreground'
            }`}
          >
            <tab.icon size={16} />
            {tab.label}
          </button>
        ))}
      </div>

      {activeTab === 'overview' && (
        <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} className="space-y-6">
          {/* KPI Grid */}
          <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
            {kpis.map((kpi, idx) => (
              <div key={idx} className="p-4 glass rounded-2xl border border-border flex flex-col justify-between">
                <div className={`p-2 w-fit rounded-lg ${kpi.bg} ${kpi.color} mb-4`}>
                  <kpi.icon size={20} />
                </div>
                <div>
                  <p className="text-2xl font-black text-foreground">{kpi.value}</p>
                  <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider">{kpi.label}</p>
                  <p className={`text-[10px] mt-1 font-medium ${kpi.label === 'Pollos Vivos' ? 'text-red-400/80' : 'text-slate-600'}`}>{kpi.sub}</p>
                </div>
              </div>
            ))}
          </div>

          {/* Rendimiento Vivo Grid */}
          {rendimiento && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
               <div className="p-4 glass rounded-2xl border border-border">
                <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider mb-2">Biomasa Total</p>
                <div className="flex items-baseline gap-2">
                  <p className="text-xl font-black text-foreground">{rendimiento.biomasaTotalKg?.toLocaleString()} kg</p>
                  <p className="text-[10px] text-emerald-500 font-bold uppercase">Estimado</p>
                </div>
              </div>
              <div className="p-4 glass rounded-2xl border border-border">
                <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider mb-2">Costo por Kilo Vivo</p>
                <div className="flex items-baseline gap-2">
                  <p className="text-xl font-black text-primary">${rendimiento.costoPorKiloVivo?.toFixed(2)}</p>
                  <p className="text-[10px] text-slate-600 font-bold uppercase">Actual</p>
                </div>
              </div>
              <div className="p-4 glass rounded-2xl border border-border">
                <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider mb-2">Alimento Consumido</p>
                <div className="flex items-baseline gap-2">
                  <p className="text-xl font-black text-blue-400">{rendimiento.alimentoConsumidoKg?.toLocaleString()} kg</p>
                </div>
              </div>
            </div>
          )}

          {/* Charts Section */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="p-6 glass rounded-3xl border border-border">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-sm font-black text-foreground uppercase tracking-widest">Curva de Mortalidad</h3>
                <span className="text-[10px] text-emerald-500 font-bold px-2 py-1 bg-emerald-500/10 rounded uppercase">Semanal</span>
              </div>
              <div className="h-64 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart data={tendencias || []}>
                    <defs>
                      <linearGradient id="colorMort" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="5%" stopColor="#ef4444" stopOpacity={0.3}/>
                        <stop offset="95%" stopColor="#ef4444" stopOpacity={0}/>
                      </linearGradient>
                    </defs>
                    <CartesianGrid strokeDasharray="3 3" stroke="#ffffff05" vertical={false} />
                    <XAxis dataKey="fecha" stroke="#475569" fontSize={10} tickLine={false} axisLine={false} />
                    <YAxis stroke="#475569" fontSize={10} tickLine={false} axisLine={false} />
                    <Tooltip 
                      contentStyle={{ background: '#0f172a', border: '1px solid #ffffff10', borderRadius: '12px' }}
                      itemStyle={{ color: '#ef4444' }}
                    />
                    <Area type="monotone" dataKey="cantidad" stroke="#ef4444" fillOpacity={1} fill="url(#colorMort)" strokeWidth={3} />
                  </AreaChart>
                </ResponsiveContainer>
              </div>
            </div>

            <div className="p-6 glass rounded-3xl border border-border">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-sm font-black text-foreground uppercase tracking-widest">Conversión Alimenticia</h3>
                <span className="text-[10px] text-blue-500 font-bold px-2 py-1 bg-blue-500/10 rounded uppercase">Histórico</span>
              </div>
              <div className="h-64 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={tendencias || []}>
                    <CartesianGrid strokeDasharray="3 3" stroke="#ffffff05" vertical={false} />
                    <XAxis dataKey="fecha" stroke="#475569" fontSize={10} tickLine={false} axisLine={false} />
                    <YAxis stroke="#475569" fontSize={10} tickLine={false} axisLine={false} />
                    <Tooltip 
                      contentStyle={{ background: '#0f172a', border: '1px solid #ffffff10', borderRadius: '12px' }}
                      itemStyle={{ color: '#3b82f6' }}
                    />
                    <Line type="monotone" dataKey="fcr" stroke="#3b82f6" strokeWidth={3} dot={{ fill: '#3b82f6', r: 4 }} activeDot={{ r: 6 }} />
                  </LineChart>
                </ResponsiveContainer>
              </div>
            </div>
          </div>
        </motion.div>
      )}

      {activeTab === 'daily' && (
        <motion.div initial={{ opacity: 0, x: 20 }} animate={{ opacity: 1, x: 0 }} className="space-y-4">
          
          {/* Proyección de Sacrificio */}
          {proyeccion && (
            <div className="p-6 bg-gradient-to-br from-indigo-500/10 to-purple-500/10 border border-indigo-500/20 rounded-[2rem] relative overflow-hidden group">
              <div className="absolute top-0 right-0 p-6 opacity-10 group-hover:scale-110 transition-transform">
                <TrendingUp size={100} />
              </div>
              <div className="relative z-10">
                <div className="flex items-center gap-3 mb-4">
                  <div className="p-2 bg-indigo-500/20 text-indigo-400 rounded-lg">
                    <Calendar size={20} />
                  </div>
                  <h3 className="text-sm font-black text-foreground uppercase tracking-widest">Proyección de Sacrificio</h3>
                </div>
                <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
                  <div>
                    <p className="text-[10px] font-black text-muted-foreground uppercase mb-1">Fecha Estimada</p>
                    <p className="text-lg font-black text-foreground">{new Date(proyeccion.fechaEstimada).toLocaleDateString()}</p>
                  </div>
                  <div>
                    <p className="text-[10px] font-black text-muted-foreground uppercase mb-1">Días Restantes</p>
                    <p className="text-lg font-black text-indigo-400">{proyeccion.diasRestantes} días</p>
                  </div>
                  <div>
                    <p className="text-[10px] font-black text-muted-foreground uppercase mb-1">Peso Proyectado</p>
                    <p className="text-lg font-black text-emerald-400">{proyeccion.pesoEstimado?.toFixed(2)} kg</p>
                  </div>
                  <div>
                    <p className="text-[10px] font-black text-muted-foreground uppercase mb-1">FCR Proyectado</p>
                    <p className="text-lg font-black text-blue-400">{proyeccion.fcrEstimado?.toFixed(2)}</p>
                  </div>
                </div>
              </div>
            </div>
          )}

          <h2 className="text-lg font-black text-foreground uppercase tracking-widest mb-4">Registro Operativo de Campo</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <button 
              onClick={() => setRecordType('mortality')}
              className="p-6 glass rounded-2xl border border-border flex items-center gap-6 hover:border-red-500/50 transition-all text-left group"
            >
              <div className="w-14 h-14 rounded-2xl bg-red-500/10 text-red-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <AlertCircle size={28} />
              </div>
              <div>
                <h4 className="text-foreground font-black text-lg">Bajas</h4>
                <p className="text-muted-foreground text-xs">Mortalidad del día</p>
              </div>
            </button>
            <button 
              onClick={() => setRecordType('feed')}
              className="p-6 glass rounded-2xl border border-border flex items-center gap-6 hover:border-blue-500/50 transition-all text-left group"
            >
              <div className="w-14 h-14 rounded-2xl bg-blue-500/10 text-blue-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <Scale size={28} />
              </div>
              <div>
                <h4 className="text-foreground font-black text-lg">Alimento</h4>
                <p className="text-muted-foreground text-xs">Kilos servidos</p>
              </div>
            </button>
            <button 
              onClick={() => setRecordType('water')}
              className="p-6 glass rounded-2xl border border-border flex items-center gap-6 hover:border-amber-500/50 transition-all text-left group"
            >
              <div className="w-14 h-14 rounded-2xl bg-amber-500/10 text-amber-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <Droplets size={28} />
              </div>
              <div>
                <h4 className="text-foreground font-black text-lg">Agua</h4>
                <p className="text-muted-foreground text-xs">Litros consumidos</p>
              </div>
            </button>
            <button 
              onClick={() => setRecordType('weight')}
              className="p-6 glass rounded-2xl border border-border flex items-center gap-6 hover:border-indigo-500/50 transition-all text-left group"
            >
              <div className="w-14 h-14 rounded-2xl bg-indigo-500/10 text-indigo-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <Ruler size={28} />
              </div>
              <div>
                <h4 className="text-foreground font-black text-lg">Pesaje</h4>
                <p className="text-muted-foreground text-xs">Muestreo de peso</p>
              </div>
            </button>
          </div>

          {/* Historial de Pesajes */}
          <div className="mt-8">
            <h3 className="text-sm font-black text-foreground uppercase tracking-widest mb-4 flex items-center gap-2">
              <History size={18} className="text-indigo-400" /> Historial de Pesajes
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {pesajes.map((p) => (
                <div key={p.id} className="p-4 glass rounded-2xl border border-border flex items-center justify-between group">
                  <div>
                    <p className="text-xl font-black text-foreground">{p.pesoPromedioGramos} g</p>
                    <p className="text-[10px] text-muted-foreground font-bold uppercase">
                      {new Date(p.fecha).toLocaleDateString()} • {p.cantidadMuestreada} aves
                    </p>
                  </div>
                  <button 
                    onClick={async () => {
                      const result = await confirmDestructiveAction('¿Eliminar pesaje?', 'Esta acción no se puede deshacer.')
                      if (result.isConfirmed) eliminarPesaje.mutate(p.id)
                    }}
                    className="p-2 text-slate-600 hover:text-red-400 transition-colors opacity-0 group-hover:opacity-100"
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              ))}
              {pesajes.length === 0 && (
                <p className="col-span-full py-8 text-center text-muted-foreground italic text-xs uppercase font-bold tracking-widest border border-dashed border-border rounded-2xl">
                  Sin registros de pesaje
                </p>
              )}
            </div>
          </div>
        </motion.div>
      )}

      {activeTab === 'sanitary' && (
        <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="space-y-4">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-black text-foreground uppercase tracking-widest">Calendario Sanitario</h2>
            <div className="flex items-center gap-2">
              <button 
                onClick={() => setIsManualModalOpen(true)}
                className="p-2 bg-primary/10 text-primary hover:bg-primary hover:text-black rounded-xl transition-all"
                title="Nueva Actividad Manual"
              >
                <Plus size={20} />
              </button>
              <Link href={`/lotes/${id}/sanidad`} className="text-xs font-bold text-muted-foreground hover:text-foreground uppercase tracking-widest ml-2">Ver Todo</Link>
            </div>
          </div>
          
          <div className="space-y-3">
            {calendario.length === 0 ? (
              <div className="p-12 text-center glass rounded-3xl border border-border">
                <p className="text-muted-foreground font-bold italic">No hay actividades programadas.</p>
              </div>
            ) : (
              calendario.map((item: any) => {
                const esAplicado = item.estado === EstadoCalendario.Aplicado;
                return (
                  <div key={item.id} className={`p-5 rounded-2xl border transition-all ${
                    esAplicado ? 'bg-emerald-500/5 border-emerald-500/10 opacity-60' : 'glass border-border'
                  }`}>
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-4">
                        <div className={`p-3 rounded-xl ${esAplicado ? 'bg-emerald-500/20 text-emerald-500' : 'bg-slate-800 text-muted-foreground'}`}>
                          {item.tipo === TipoActividad.Vacuna ? <Droplets size={20} /> : <ClipboardCheck size={20} />}
                        </div>
                        <div>
                          <h4 className={`font-bold ${esAplicado ? 'text-emerald-500' : 'text-white'}`}>{item.descripcionTratamiento}</h4>
                          <p className="text-xs text-muted-foreground">
                            Día {item.diaDeAplicacion} 
                            {item.fechaProgramada && ` • ${new Date(item.fechaProgramada).toLocaleDateString()}`}
                          </p>
                        </div>
                      </div>
                      {!esAplicado && (
                        <button 
                          onClick={() => aplicarActividad.mutate({ id: item.id, data: { cantidadConsumida: 0 } })}
                          disabled={aplicarActividad.isPending}
                          className="px-4 py-2 bg-muted/50 hover:bg-emerald-500 hover:text-black rounded-xl text-[10px] font-black uppercase tracking-widest transition-all"
                        >
                          Aplicar
                        </button>
                      )}
                    </div>
                  </div>
                );
              })
            )}
          </div>
        </motion.div>
      )}

      {activeTab === 'actions' && (
        <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} className="space-y-6">
          <h2 className="text-lg font-black text-foreground uppercase tracking-widest mb-4">Gestión Avanzada</h2>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {lote?.estado === 'Activo' && (
              <button 
                onClick={async () => {
                  const result = await confirmAction('¿Cerrar Lote?', 'Esta acción registrará el cierre definitivo.', 'warning')
                  if (result.isConfirmed) {
                    cerrarLote.mutate()
                  }
                }}
                disabled={cerrarLote.isPending}
                className="p-6 glass rounded-2xl border border-border flex items-center gap-4 hover:border-emerald-500/50 transition-all text-left group"
              >
                <div className="w-12 h-12 rounded-xl bg-emerald-500/10 text-emerald-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                  <Lock size={24} />
                </div>
                <div>
                  <h4 className="text-foreground font-black text-sm uppercase">Cerrar Lote</h4>
                  <p className="text-muted-foreground text-[10px] uppercase font-bold">Finalizar ciclo productivo</p>
                </div>
              </button>
            )}

            {lote?.estado === 'Cerrado' && (
              <button 
                onClick={async () => {
                  const result = await confirmAction('¿Reabrir Lote?', 'El lote volverá a estar activo.', 'question')
                  if (result.isConfirmed) {
                    reabrirLote.mutate()
                  }
                }}
                disabled={reabrirLote.isPending}
                className="p-6 glass rounded-2xl border border-border flex items-center gap-4 hover:border-blue-500/50 transition-all text-left group"
              >
                <div className="w-12 h-12 rounded-xl bg-blue-500/10 text-blue-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                  <Unlock size={24} />
                </div>
                <div>
                  <h4 className="text-foreground font-black text-sm uppercase">Reabrir Lote</h4>
                  <p className="text-muted-foreground text-[10px] uppercase font-bold">Volver a estado activo</p>
                </div>
              </button>
            )}

            {lote?.estado === 'Activo' && (
              <button 
                onClick={async () => {
                  const result = await promptAction('Cancelar Lote', 'Ingrese la justificación para cancelar este lote:')
                  if (result.isConfirmed && result.value) {
                    cancelarLote.mutate(result.value)
                  }
                }}
                disabled={cancelarLote.isPending}
                className="p-6 glass rounded-2xl border border-border flex items-center gap-4 hover:border-red-500/50 transition-all text-left group"
              >
                <div className="w-12 h-12 rounded-xl bg-red-500/10 text-red-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                  <XCircle size={24} />
                </div>
                <div>
                  <h4 className="text-foreground font-black text-sm uppercase">Cancelar Lote</h4>
                  <p className="text-muted-foreground text-[10px] uppercase font-bold">Anular lote actual</p>
                </div>
              </button>
            )}

            <button 
              onClick={descargarReporte}
              className="p-6 glass rounded-2xl border border-border flex items-center gap-4 hover:border-primary/50 transition-all text-left group"
            >
              <div className="w-12 h-12 rounded-xl bg-primary/10 text-primary flex items-center justify-center group-hover:scale-110 transition-transform">
                <FileText size={24} />
              </div>
              <div>
                <h4 className="text-foreground font-black text-sm uppercase">Reporte PDF</h4>
                <p className="text-muted-foreground text-[10px] uppercase font-bold">Descargar informe de cierre</p>
              </div>
            </button>

            <button 
              onClick={() => setIsTransferModalOpen(true)}
              className="p-6 glass rounded-2xl border border-border flex items-center gap-4 hover:border-amber-500/50 transition-all text-left group"
            >
              <div className="w-12 h-12 rounded-xl bg-amber-500/10 text-amber-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <ArrowLeftRight size={24} />
              </div>
              <div>
                <h4 className="text-foreground font-black text-sm uppercase">Trasladar</h4>
                <p className="text-muted-foreground text-[10px] uppercase font-bold">Mover a otro galpón</p>
              </div>
            </button>

            <button 
              onClick={async () => {
                const result = await confirmDestructiveAction('¿Eliminar Lote?', 'Esta acción es permanente y no se puede deshacer.')
                if (result.isConfirmed) {
                  eliminarLote.mutate()
                }
              }}
              disabled={eliminarLote.isPending}
              className="p-6 glass rounded-2xl border border-border flex items-center gap-4 hover:border-red-500/80 transition-all text-left group"
            >
              <div className="w-12 h-12 rounded-xl bg-red-500/20 text-red-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <Trash2 size={24} />
              </div>
              <div>
                <h4 className="text-foreground font-black text-sm uppercase">Eliminar</h4>
                <p className="text-muted-foreground text-[10px] uppercase font-bold text-red-400">Acción destructiva</p>
              </div>
            </button>
          </div>
        </motion.div>
      )}

      {/* Modals */}
      <LoteFormModal 
        isOpen={isEditModalOpen} 
        onClose={() => setIsEditModalOpen(false)} 
        lote={lote}
      />

      {recordType && (
        <QuickRecordModal 
          isOpen={!!recordType} 
          onClose={() => setRecordType(null)} 
          loteId={id as string} 
          type={recordType}
          lote={lote}
        />
      )}

      {/* Trasladar Modal */}
      <AnimatePresence>
        {isTransferModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsTransferModalOpen(false)} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[120]" />
            <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.95 }} className="fixed inset-0 m-auto w-full max-w-md h-fit glass z-[130] p-8 rounded-[2rem] border border-border" >
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-xl font-black text-foreground uppercase tracking-widest">Trasladar Lote</h2>
                <button onClick={() => setIsTransferModalOpen(false)} className="text-muted-foreground hover:text-foreground"><XCircle size={24} /></button>
              </div>
              <div className="space-y-4">
                <p className="text-sm text-muted-foreground font-medium">Seleccione el galpón de destino para el lote <b>{lote?.nombreLote}</b>.</p>
                <select 
                  value={newGalponId}
                  onChange={(e) => setNewGalponId(e.target.value)}
                  className="w-full px-5 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all appearance-none"
                >
                  <option value="" className="bg-muted/50">Seleccionar Galpón</option>
                  {galpones.filter((g: any) => g.id !== lote?.galponId).map((g: any) => (
                    <option key={g.id} value={g.id} className="bg-muted/50">{g.nombre}</option>
                  ))}
                </select>
                <button
                  onClick={() => trasladarLote.mutate(newGalponId)}
                  disabled={trasladarLote.isPending || !newGalponId}
                  className="w-full py-4 bg-primary text-black font-black rounded-2xl uppercase tracking-widest hover:bg-primary/90 transition-all disabled:opacity-50"
                >
                  Confirmar Traslado
                </button>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>

      <ManualActivityModal 
        isOpen={isManualModalOpen} 
        onClose={() => setIsManualModalOpen(false)} 
        loteId={id as string} 
      />
    </div>
  )
}



