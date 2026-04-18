'use client'

import { useParams, useRouter } from 'next/navigation'
import { useLote } from '@/hooks/useLotes'
import { useGalpones } from '@/hooks/useGalpones'
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
import { LoteFormModal } from '@/components/production/LoteFormModal'
import { CerrarLoteModal } from '@/components/production/CerrarLoteModal'
import { ManualActivityModal } from '@/components/production/ManualActivityModal'
import { OperationFilters } from '@/components/production/OperationFilters'
import { OperationHistoryList } from '@/components/production/OperationHistoryList'
import { QuickRecordModal } from '@/components/production/QuickRecordModal'
import { cn } from '@/lib/utils'
import { confirmAction, confirmDestructiveAction, promptAction } from '@/lib/swal'
import { useCalendarioSanitario, EstadoCalendario, TipoActividad } from '@/hooks/useCalendarioSanitario'
import { useProyeccionSacrificio } from '@/hooks/useDashboard' 
import { usePesajes } from '@/hooks/usePesajes'
import { useMortalidad } from '@/hooks/useMortalidad'
import { useSanidad } from '@/hooks/useSanidad'
import { useInventario, useMovimientosLote } from '@/hooks/useInventario'
import { api } from '@/lib/api' // Keeping it for trends if needed, though we could create a hook for it
import { useQuery } from '@tanstack/react-query'
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, LineChart, Line } from 'recharts'

// --- Main Page Component ---

export default function LoteDashboard() {
  const { id } = useParams()
  const router = useRouter()
  const [activeTab, setActiveTab] = useState<'overview' | 'daily' | 'sanitary' | 'actions'>('overview')
  const [recordType, setRecordType] = useState<'mortality' | 'feed' | 'water' | 'weight' | null>(null)
  const [isEditModalOpen, setIsEditModalOpen] = useState(false)
  const [isTransferModalOpen, setIsTransferModalOpen] = useState(false)
  const [isManualModalOpen, setIsManualModalOpen] = useState(false)
  const [isCerrarModalOpen, setIsCerrarModalOpen] = useState(false)
  const [newGalponId, setNewGalponId] = useState('')
  
  // Estados para historial y filtros
  const [selectedDate, setSelectedDate] = useState('')
  const [selectedMonth, setSelectedMonth] = useState(new Date().getMonth() + 1)
  const [selectedYear, setSelectedYear] = useState(new Date().getFullYear())
  const [historyTab, setHistoryTab] = useState<'mortality' | 'feed' | 'water' | 'weight'>('mortality')
  const [editingItem, setEditingItem] = useState<any>(null)

  const { 
    lote, 
    isLoading: isLoadingLote,
    cerrarLote,
    reabrirLote,
    cancelarLote,
    eliminarLote,
    trasladarLote,
    rendimientoVivo: rendimiento,
    descargarReportePdf
  } = useLote(id as string)

  const { galpones } = useGalpones()

  const { pesajes, isLoading: isLoadingPesajes, eliminarPesaje } = usePesajes(id as string)
  const { mortalidad, isLoading: isLoadingMortalidad, eliminarMortalidad } = useMortalidad(id as string)
  const { historialBienestar, isLoadingHistorial: isLoadingSanidad, eliminarBienestar } = useSanidad(id as string)
  const { data: movimientos = [], isLoading: isLoadingMovimientosLote } = useMovimientosLote(id as string)
  const { eliminarMovimiento } = useInventario()

  // Tendencias (podríamos moverlo a useMortalidad más adelante)
  const { data: tendenciasResponse } = useQuery({
    queryKey: ['lote-tendencias', id],
    queryFn: () => api.get<any>(`/api/Mortalidad/lote/${id}/tendencias`),
    enabled: !!id
  })

  const tendencias = Array.isArray(tendenciasResponse) 
    ? tendenciasResponse 
    : (tendenciasResponse?.tendencias || [])

  const { data: proyeccion } = useProyeccionSacrificio(id as string)

  const { 
    calendario, 
    aplicarActividad, 
  } = useCalendarioSanitario(id as string)

  const descargarReporte = () => {
    const baseUrl = process.env.NEXT_PUBLIC_API_URL || ''
    window.open(`${baseUrl}/api/Lotes/${id}/reporte-cierre-pdf`, '_blank')
  }

  // Función para filtrar datos del historial
  const getFilteredData = () => {
    let baseData: any[] = []
    if (historyTab === 'mortality') baseData = mortalidad || []
    else if (historyTab === 'feed') baseData = movimientos || []
    else if (historyTab === 'water') baseData = historialBienestar || []
    else if (historyTab === 'weight') baseData = pesajes || []

    return baseData.filter((item: any) => {
      const fechaStr = item.fecha || item.fechaRegistro || item.fechaMovimiento
      if (!fechaStr) return true
      
      const itemDate = new Date(fechaStr)
      
      // Filtro por fecha exacta
      if (selectedDate) {
        return itemDate.toISOString().split('T')[0] === selectedDate
      }

      // Filtro por mes y año
      const matchMonth = selectedMonth ? (itemDate.getMonth() + 1) === selectedMonth : true
      const matchYear = selectedYear ? itemDate.getFullYear() === selectedYear : true
      
      return matchMonth && matchYear
    })
  }

  if (isLoadingLote) return (
    <div className="h-96 flex items-center justify-center">
      <div className="w-12 h-12 border-4 border-primary/20 border-t-primary rounded-full animate-spin" />
    </div>
  )

  const kpis = [
    { label: 'Pollos Vivos', value: lote?.avesVivas?.toLocaleString() || '0', sub: `${lote?.mortalidadPorcentaje?.toFixed(1)}% mortalidad`, icon: Bird, color: 'text-emerald-500', bg: 'bg-emerald-500/10' },
    { label: 'Conversión (FCR)', value: lote?.fcrActual?.toFixed(2) || '0.00', sub: 'Eficiencia alimenticia', icon: TrendingUp, color: 'text-blue-500', bg: 'bg-blue-500/10' },
    { label: 'Costo Acumulado', value: `Bs. ${lote?.costoTotalAcumulado?.toLocaleString() || '0'}`, sub: 'Inversión total', icon: DollarSign, color: 'text-primary', bg: 'bg-primary/10' },
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
            <h1 className="text-2xl font-black text-foreground">{lote?.nombre || lote?.nombreLote}</h1>
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
                  <p className="text-xl font-black text-primary">Bs. {rendimiento.costoPorKiloVivo?.toFixed(2)}</p>
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
              onClick={() => { setEditingItem(null); setRecordType('mortality'); }}
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
              onClick={() => { setEditingItem(null); setRecordType('feed'); }}
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
              onClick={() => { setEditingItem(null); setRecordType('water'); }}
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
              onClick={() => { setEditingItem(null); setRecordType('weight'); }}
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

          {/* Sección de Historial con Filtros */}
          <div className="mt-12 space-y-6">
            <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
              <h3 className="text-sm font-black text-foreground uppercase tracking-widest flex items-center gap-2">
                <History size={18} className="text-primary" /> Historial de Operaciones
              </h3>
              <OperationFilters 
                selectedDate={selectedDate}
                onDateChange={setSelectedDate}
                selectedMonth={selectedMonth}
                onMonthChange={setSelectedMonth}
                selectedYear={selectedYear}
                onYearChange={setSelectedYear}
                onClear={() => {
                  setSelectedDate('');
                  setSelectedMonth(new Date().getMonth() + 1);
                  setSelectedYear(new Date().getFullYear());
                }}
              />
            </div>

            {/* Pestañas de Historial */}
            <div className="flex p-1 bg-muted/30 rounded-xl border border-border/50 w-fit">
              {[
                { id: 'mortality', label: 'Bajas', icon: AlertCircle },
                { id: 'feed', label: 'Alimento', icon: Scale },
                { id: 'water', label: 'Agua', icon: Droplets },
                { id: 'weight', label: 'Pesajes', icon: Ruler },
              ].map((tab) => (
                <button
                  key={tab.id}
                  onClick={() => setHistoryTab(tab.id as any)}
                  className={`flex items-center gap-2 py-2 px-4 rounded-lg text-[10px] font-black uppercase tracking-widest transition-all ${
                    historyTab === tab.id ? 'bg-background text-foreground shadow-sm' : 'text-muted-foreground hover:text-foreground'
                  }`}
                >
                  <tab.icon size={14} />
                  {tab.label}
                </button>
              ))}
            </div>

            {/* Lista de Historial Filtrada */}
            <div className="min-h-[300px]">
              <OperationHistoryList 
                type={historyTab}
                isLoading={
                  historyTab === 'mortality' ? isLoadingMortalidad :
                  historyTab === 'feed' ? isLoadingMovimientosLote :
                  historyTab === 'water' ? isLoadingSanidad :
                  isLoadingPesajes
                }
                data={getFilteredData()}
                onEdit={(item) => {
                  setEditingItem(item);
                  setRecordType(historyTab);
                }}
                onDelete={(id) => {
                  if (historyTab === 'mortality') eliminarMortalidad.mutate(id);
                  else if (historyTab === 'weight') eliminarPesaje.mutate(id);
                  else if (historyTab === 'feed') eliminarMovimiento.mutate(id);
                  else if (historyTab === 'water') eliminarBienestar.mutate(id);
                }}
              />
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
                onClick={() => setIsCerrarModalOpen(true)}
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
                    reabrirLote.mutate(undefined, {
                      onSuccess: () => toast.success('Lote reabierto correctamente'),
                      onError: (err: any) => toast.error(err.message || 'Error al reabrir lote')
                    })
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
                  const result = await promptAction(
                    'Cancelar Lote', 
                    'Ingrese la justificación para cancelar este lote:'
                  )
                  
                  // Agregamos una confirmación extra con la explicación detallada
                  if (result.isConfirmed && result.value) {
                    const confirmCancel = await confirmAction(
                      '¿Confirmar Cancelación?',
                      'Al cancelar el lote, este pasará a un estado inactivo permanente. Se conservarán los registros para auditoría, pero no se podrán registrar más actividades ni ventas. Esta acción no se puede deshacer.',
                      'warning'
                    )
                    
                    if (confirmCancel.isConfirmed) {
                      cancelarLote.mutate(result.value, {
                        onSuccess: () => toast.success('Lote cancelado correctamente'),
                        onError: (err: any) => toast.error(err.message || 'Error al cancelar lote')
                      })
                    }
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
              onClick={() => descargarReportePdf()}
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
                  eliminarLote.mutate(undefined, {
                    onSuccess: () => {
                      toast.success('Lote eliminado correctamente')
                      router.push('/lotes')
                    },
                    onError: (err: any) => toast.error(err.message || 'Error al eliminar lote')
                  })
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
          onClose={() => {
            setRecordType(null);
            setEditingItem(null);
          }} 
          loteId={id as string} 
          type={recordType}
          lote={lote}
          initialData={editingItem}
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
                  onClick={() => trasladarLote.mutate(newGalponId, {
                    onSuccess: () => {
                      toast.success('Lote trasladado correctamente')
                      setIsTransferModalOpen(false)
                    },
                    onError: (err: any) => toast.error(err.message || 'Error al trasladar lote')
                  })}
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

      <CerrarLoteModal
        isOpen={isCerrarModalOpen}
        onClose={() => setIsCerrarModalOpen(false)}
        loteId={id as string}
        loteNombre={lote?.nombre || lote?.nombreLote || ''}
      />
    </div>
  )
}



