'use client'

import { useParams } from 'next/navigation'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion } from 'framer-motion'
import { 
  Bird, TrendingUp, AlertCircle, DollarSign, Plus, 
  ChevronLeft, ClipboardCheck, Scale, Droplets, PieChart as PieChartIcon
} from 'lucide-react'
import Link from 'next/link'
import { toast } from 'sonner'
import { useState } from 'react'
import dynamic from 'next/dynamic'

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
  const queryClient = useQueryClient()
  const [activeTab, setActiveTab] = useState<'overview' | 'daily' | 'sanitary'>('overview')
  const [recordType, setRecordType] = useState<'mortality' | 'feed' | 'water' | null>(null)

  // ... (Queries y Mutations iguales)

  const { data: lote, isLoading: isLoadingLote } = useQuery({
    queryKey: ['lote', id],
    queryFn: () => api.get<any>(`/api/Lotes/${id}`),
  })

  const { data: tendencias } = useQuery({
    queryKey: ['lote-tendencias', id],
    queryFn: () => api.get<any>(`/api/Mortalidad/lote/${id}/tendencias`),
  })

  const { data: calendario = [] } = useQuery({
    queryKey: ['calendario', id],
    queryFn: () => api.get<any[]>(`/api/calendario/${id}`),
  })

  // Quick Action Mutations
  const aplicarActividad = useMutation({
    mutationFn: (actId: string) => api.patch(`/api/calendario/${actId}/aplicar`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['calendario', id] })
      toast.success('Actividad marcada como completada')
    },
  })

  if (isLoadingLote) return (
    <div className="h-96 flex items-center justify-center">
      <div className="w-12 h-12 border-4 border-primary/20 border-t-primary rounded-full animate-spin" />
    </div>
  )

  const kpis = [
    { label: 'Pollos Vivos', value: lote?.avesVivas?.toLocaleString() || '0', sub: `${lote?.mortalidadPorcentaje?.toFixed(1)}% mortalidad`, icon: Bird, color: 'text-emerald-500', bg: 'bg-emerald-500/10' },
    { label: 'Conversión (FCR)', value: lote?.fcrActual?.toFixed(2) || '0.00', sub: 'Eficiencia alimenticia', icon: TrendingUp, color: 'text-blue-500', bg: 'bg-blue-500/10' },
    { label: 'Costo Acumulado', value: `$${lote?.costoTotalAcumulado?.toLocaleString() || '0'}`, sub: 'Inversión total', icon: DollarSign, color: 'text-primary', bg: 'bg-primary/10' },
    { label: 'Peso Promedio', value: '1.85 kg', sub: 'Estimado actual', icon: Scale, color: 'text-amber-500', bg: 'bg-amber-500/10' },
  ]

  return (
    <div className="space-y-6 pb-20">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <Link href="/lotes" className="p-2 hover:bg-white/5 rounded-xl transition-colors text-slate-400">
            <ChevronLeft size={24} />
          </Link>
          <div>
            <h1 className="text-2xl font-black text-white">{lote?.nombreLote}</h1>
            <p className="text-xs font-bold text-primary uppercase tracking-widest">{lote?.galponNombre}</p>
          </div>
        </div>
        <div className="flex gap-2">
          <button className="hidden md:flex items-center gap-2 px-4 py-2 bg-primary text-black font-black rounded-xl text-xs uppercase tracking-tighter">
            <Plus size={16} /> Registro Rápido
          </button>
        </div>
      </div>

      {/* Tabs Selector (Mobile-First) */}
      <div className="flex p-1 bg-slate-900/50 rounded-2xl border border-white/5 overflow-x-auto no-scrollbar">
        {[
          { id: 'overview', label: 'Resumen 360°', icon: PieChartIcon },
          { id: 'daily', label: 'Operación Diaria', icon: ClipboardCheck },
          { id: 'sanitary', label: 'Plan Sanitario', icon: Droplets },
        ].map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id as any)}
            className={`flex-1 flex items-center justify-center gap-2 py-3 px-4 rounded-xl text-xs font-bold uppercase tracking-widest transition-all whitespace-nowrap ${
              activeTab === tab.id ? 'bg-primary text-black shadow-lg shadow-primary/20' : 'text-slate-400'
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
              <div key={idx} className="p-4 glass-dark rounded-2xl border border-white/5 flex flex-col justify-between">
                <div className={`p-2 w-fit rounded-lg ${kpi.bg} ${kpi.color} mb-4`}>
                  <kpi.icon size={20} />
                </div>
                <div>
                  <p className="text-2xl font-black text-white">{kpi.value}</p>
                  <p className="text-[10px] text-slate-500 uppercase font-bold tracking-wider">{kpi.label}</p>
                  <p className={`text-[10px] mt-1 font-medium ${kpi.label === 'Pollos Vivos' ? 'text-red-400/80' : 'text-slate-600'}`}>{kpi.sub}</p>
                </div>
              </div>
            ))}
          </div>

          {/* Charts Section */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div className="p-6 glass-dark rounded-3xl border border-white/5">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-sm font-black text-white uppercase tracking-widest">Curva de Mortalidad</h3>
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

            <div className="p-6 glass-dark rounded-3xl border border-white/5">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-sm font-black text-white uppercase tracking-widest">Conversión Alimenticia</h3>
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
          <h2 className="text-lg font-black text-white uppercase tracking-widest mb-4">Registro Operativo de Campo</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <button 
              onClick={() => setRecordType('mortality')}
              className="p-6 glass-dark rounded-2xl border border-white/5 flex items-center gap-6 hover:border-red-500/50 transition-all text-left group"
            >
              <div className="w-14 h-14 rounded-2xl bg-red-500/10 text-red-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <AlertCircle size={28} />
              </div>
              <div>
                <h4 className="text-white font-black text-lg">Registrar Bajas</h4>
                <p className="text-slate-500 text-sm">Notificar mortalidad del día.</p>
              </div>
            </button>
            <button 
              onClick={() => setRecordType('feed')}
              className="p-6 glass-dark rounded-2xl border border-white/5 flex items-center gap-6 hover:border-blue-500/50 transition-all text-left group"
            >
              <div className="w-14 h-14 rounded-2xl bg-blue-500/10 text-blue-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <Scale size={28} />
              </div>
              <div>
                <h4 className="text-white font-black text-lg">Consumo Alimento</h4>
                <p className="text-slate-500 text-sm">Kilos servidos hoy.</p>
              </div>
            </button>
            <button 
              onClick={() => setRecordType('water')}
              className="p-6 glass-dark rounded-2xl border border-white/5 flex items-center gap-6 hover:border-amber-500/50 transition-all text-left group"
            >
              <div className="w-14 h-14 rounded-2xl bg-amber-500/10 text-amber-500 flex items-center justify-center group-hover:scale-110 transition-transform">
                <Droplets size={28} />
              </div>
              <div>
                <h4 className="text-white font-black text-lg">Consumo de Agua</h4>
                <p className="text-slate-500 text-sm">Litros consumidos.</p>
              </div>
            </button>
          </div>
        </motion.div>
      )}

      {activeTab === 'sanitary' && (
        <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="space-y-4">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-black text-white uppercase tracking-widest">Calendario Sanitario</h2>
            <Link href={`/lotes/${id}/sanidad`} className="text-xs font-bold text-primary uppercase tracking-widest">Ver Todo</Link>
          </div>
          
          <div className="space-y-3">
            {calendario.length === 0 ? (
              <div className="p-12 text-center glass-dark rounded-3xl border border-white/5">
                <p className="text-slate-500 font-bold italic">No hay actividades programadas.</p>
              </div>
            ) : (
              calendario.map((item: any) => (
                <div key={item.id} className={`p-5 rounded-2xl border transition-all ${
                  item.aplicada ? 'bg-emerald-500/5 border-emerald-500/10 opacity-60' : 'glass-dark border-white/5'
                }`}>
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-4">
                      <div className={`p-3 rounded-xl ${item.aplicada ? 'bg-emerald-500/20 text-emerald-500' : 'bg-slate-800 text-slate-400'}`}>
                        {item.tipo === 'Vacuna' ? <Droplets size={20} /> : <ClipboardCheck size={20} />}
                      </div>
                      <div>
                        <h4 className={`font-bold ${item.aplicada ? 'text-emerald-500' : 'text-white'}`}>{item.actividad}</h4>
                        <p className="text-xs text-slate-500">Día {item.diaProgramado} • {new Date(item.fechaProgramada).toLocaleDateString()}</p>
                      </div>
                    </div>
                    {!item.aplicada && (
                      <button 
                        onClick={() => aplicarActividad.mutate(item.id)}
                        disabled={aplicarActividad.isPending}
                        className="px-4 py-2 bg-white/5 hover:bg-emerald-500 hover:text-black rounded-xl text-[10px] font-black uppercase tracking-widest transition-all"
                      >
                        Aplicar
                      </button>
                    )}
                  </div>
                </div>
              ))
            )}
          </div>
        </motion.div>
      )}

      {/* Modal de Registro Rápido */}
      {recordType && (
        <QuickRecordModal 
          isOpen={!!recordType} 
          onClose={() => setRecordType(null)} 
          loteId={id as string} 
          type={recordType} 
        />
      )}
    </div>
  )
}
