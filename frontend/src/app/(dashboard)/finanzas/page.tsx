"use client";

import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { 
  DollarSign, TrendingUp, TrendingDown, ArrowUpRight, 
  ArrowDownRight, Calendar, Filter, PieChart, 
  Wallet, Receipt, Landmark, Clock, BarChart3
} from 'lucide-react';
import { useFinanzas } from '@/hooks/useFinanzas';
import { cn } from '@/lib/utils';
import { UniversalGrid } from '@/components/shared/UniversalGrid';

export default function FinanzasPage() {
  const [activeTab, setActiveTab] = useState<'flujo' | 'gastos' | 'proyeccion'>('flujo');
  const [dateRange, setDateRange] = useState({
    inicio: new Date(new Date().setDate(new Date().getDate() - 30)).toISOString().split('T')[0],
    fin: new Date().toISOString().split('T')[0]
  });

  const { 
    flujoCaja, isLoadingFlujo, 
    gastos, isLoadingGastos,
    gastosPorCategoria,
    flujoProyectado, isLoadingProyectado,
    cuentasPorCobrar
  } = useFinanzas({
    inicio: new Date(dateRange.inicio).toISOString(),
    fin: new Date(dateRange.fin).toISOString()
  });

  const kpis = [
    { 
      label: 'Ingresos (30d)', 
      value: `Bs. ${flujoCaja?.totalIngresos.toLocaleString() || '0'}`,

      icon: TrendingUp, 
      color: 'text-emerald-400', 
      bg: 'bg-emerald-400/10' 
    },
    { 
      label: 'Egresos (30d)', 
      value: `Bs. ${flujoCaja?.totalEgresos.toLocaleString() || '0'}`,

      icon: TrendingDown, 
      color: 'text-red-400', 
      bg: 'bg-red-400/10' 
    },
    { 
      label: 'Utilidad Neta', 
      value: `Bs. ${flujoCaja?.utilidadNeta.toLocaleString() || '0'}`, 
      icon: DollarSign, 
      color: 'text-primary', 
      bg: 'bg-primary/10' 
    },
    { 
      label: 'Por Cobrar', 
      value: `Bs. ${flujoProyectado?.totalCuentasPorCobrar.toLocaleString() || '0'}`, 
      icon: Clock, 
      color: 'text-amber-400', 
      bg: 'bg-amber-400/10' 
    },
  ];

  return (
    <div className="space-y-8 pb-20">
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-black text-foreground uppercase tracking-tight flex items-center gap-3">
            <Landmark className="text-primary" size={32} />
            Gestión Financiera
          </h1>
          <p className="text-muted-foreground mt-1 font-medium">Control de flujo de caja, gastos y proyecciones.</p>
        </div>

        <div className="flex items-center gap-3">
          <div className="flex items-center gap-2 bg-muted/50 p-1.5 rounded-2xl border border-border">
            <div className="flex items-center gap-2 px-3 py-2">
              <Calendar size={16} className="text-muted-foreground" />
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
        </div>
      </div>

      {/* KPI Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {kpis.map((kpi, idx) => (
          <motion.div
            key={kpi.label}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: idx * 0.1 }}
            className="p-6 glass rounded-3xl border border-border"
          >
            <div className="flex items-center justify-between mb-4">
              <div className={`p-3 rounded-2xl ${kpi.bg} ${kpi.color}`}>
                <kpi.icon size={24} />
              </div>
              <span className="text-2xl font-black text-foreground">{kpi.value}</span>
            </div>
            <p className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">{kpi.label}</p>
          </motion.div>
        ))}
      </div>

      {/* Tabs */}
      <div className="flex p-1 bg-muted/50 rounded-2xl border border-border w-fit">
        {[
          { id: 'flujo', label: 'Flujo de Caja', icon: BarChart3 },
          { id: 'gastos', label: 'Análisis de Gastos', icon: PieChart },
          { id: 'proyeccion', label: 'Proyecciones', icon: Clock },
        ].map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id as any)}
            className={cn(
              "flex items-center gap-2 px-6 py-3 rounded-xl text-xs font-black uppercase tracking-widest transition-all",
              activeTab === tab.id ? "bg-primary text-black shadow-lg" : "text-muted-foreground hover:text-foreground"
            )}
          >
            <tab.icon size={16} />
            {tab.label}
          </button>
        ))}
      </div>

      <AnimatePresence mode="wait">
        {activeTab === 'flujo' && (
          <motion.div key="flujo" initial={{ opacity: 0, x: -20 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: 20 }} className="space-y-6">
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
              <div className="space-y-4">
                <h3 className="text-sm font-black text-foreground uppercase tracking-widest ml-2 flex items-center gap-2">
                  <ArrowUpRight className="text-emerald-500" size={18} />
                  Ingresos por Ventas
                </h3>
                <div className="glass rounded-[2rem] border border-border p-2">
                   <UniversalGrid
                    title="Ingresos"
                    items={flujoCaja?.ventas || []}
                    isLoading={isLoadingFlujo}
                    columns={[
                      { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
                      { header: 'Lote', accessor: 'lote' },
                      { header: 'Monto', accessor: (item) => <span className="font-black text-emerald-400">Bs. {item.monto.toLocaleString()}</span> }
                    ]}
                    renderMobileCard={(item) => (
                      <div className="flex justify-between items-center">
                        <span className="font-bold text-foreground">Lote {item.lote}</span>
                        <span className="font-black text-emerald-400">Bs. {item.monto.toLocaleString()}</span>
                      </div>
                    )}
                   />
                </div>
              </div>

              <div className="space-y-4">
                <h3 className="text-sm font-black text-foreground uppercase tracking-widest ml-2 flex items-center gap-2">
                  <ArrowDownRight className="text-red-500" size={18} />
                  Egresos Operativos
                </h3>
                <div className="glass rounded-[2rem] border border-border p-2">
                   <UniversalGrid
                    title="Egresos"
                    items={flujoCaja?.gastos || []}
                    isLoading={isLoadingFlujo}
                    columns={[
                      { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
                      { header: 'Descripción', accessor: 'descripcion' },
                      { header: 'Monto', accessor: (item) => <span className="font-black text-red-400">Bs. {item.monto.toLocaleString()}</span> }
                    ]}
                    renderMobileCard={(item) => (
                      <div className="flex justify-between items-center">
                        <span className="font-bold text-foreground truncate max-w-[150px]">{item.descripcion}</span>
                        <span className="font-black text-red-400">Bs. {item.monto.toLocaleString()}</span>
                      </div>
                    )}
                   />
                </div>
              </div>
            </div>
          </motion.div>
        )}

        {activeTab === 'gastos' && (
          <motion.div key="gastos" initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.95 }} className="space-y-6">
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
              <div className="lg:col-span-1 space-y-4">
                <h3 className="text-sm font-black text-foreground uppercase tracking-widest ml-2">Gastos por Categoría</h3>
                <div className="space-y-3">
                  {gastosPorCategoria.map((cat, i) => (
                    <div key={i} className="p-4 glass rounded-2xl border border-border flex items-center justify-between">
                      <span className="text-xs font-bold text-slate-300 uppercase">{cat.categoria}</span>
                      <span className="text-sm font-black text-foreground">Bs. {cat.total.toLocaleString()}</span>
                    </div>
                  ))}
                </div>
              </div>

              <div className="lg:col-span-2 space-y-4">
                <h3 className="text-sm font-black text-foreground uppercase tracking-widest ml-2">Detalle de Egresos</h3>
                <div className="glass rounded-[2.5rem] border border-border p-2">
                  <UniversalGrid
                    title="Gastos Detallados"
                    items={gastos}
                    isLoading={isLoadingGastos}
                    columns={[
                      { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
                      { header: 'Categoría', accessor: 'tipoGasto' },
                      { header: 'Descripción', accessor: 'descripcion' },
                      { header: 'Monto', accessor: (item) => <span className="font-black text-foreground">Bs. {item.monto.toLocaleString()}</span> }
                    ]}
                    renderMobileCard={(item) => (
                      <div className="space-y-2">
                        <div className="flex justify-between items-center">
                          <span className="text-[10px] font-black text-muted-foreground uppercase">{item.tipoGasto}</span>
                          <span className="font-black text-foreground">Bs. {item.monto.toLocaleString()}</span>
                        </div>
                        <p className="text-xs text-muted-foreground">{item.descripcion}</p>
                      </div>
                    )}
                  />
                </div>
              </div>
            </div>
          </motion.div>
        )}

        {activeTab === 'proyeccion' && (
          <motion.div key="proyeccion" initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -20 }} className="space-y-8">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
               <div className="p-8 bg-gradient-to-br from-primary/20 to-transparent rounded-[2.5rem] border border-primary/10">
                  <p className="text-[10px] font-black text-primary uppercase tracking-[0.2em] mb-2">Flujo Neto 30 Días</p>
                  <h2 className="text-4xl font-black text-foreground mb-4">Bs. {flujoProyectado?.flujoNetoProyectado30Dias.toLocaleString()}</h2>
                  <div className="flex items-center gap-2 text-xs font-bold text-muted-foreground uppercase">
                    <Clock size={16} />
                    <span>Proyección basada en CxC y Alimento</span>
                  </div>
               </div>

               <div className="md:col-span-2 grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <div className="p-6 glass rounded-3xl border border-border flex flex-col justify-between">
                    <p className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Costo Alimento (Proyectado)</p>
                    <p className="text-2xl font-black text-red-400">-Bs. {flujoProyectado?.costoProyectadoAlimento30Dias.toLocaleString()}</p>
                  </div>
                  <div className="p-6 glass rounded-3xl border border-border flex flex-col justify-between">
                    <p className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Cuentas por Cobrar</p>
                    <p className="text-2xl font-black text-emerald-400">+Bs. {flujoProyectado?.totalCuentasPorCobrar.toLocaleString()}</p>
                  </div>
               </div>
            </div>

            <div className="space-y-4">
              <h3 className="text-sm font-black text-foreground uppercase tracking-widest ml-2">Línea de Tiempo de Proyecciones</h3>
              <div className="glass rounded-[2.5rem] border border-border p-2 overflow-hidden">
                <table className="w-full text-left">
                  <thead className="border-b border-border">
                    <tr>
                      <th className="px-6 py-4 text-[10px] font-black text-muted-foreground uppercase">Concepto</th>
                      <th className="px-6 py-4 text-[10px] font-black text-muted-foreground uppercase text-center">Tipo</th>
                      <th className="px-6 py-4 text-[10px] font-black text-muted-foreground uppercase">Fecha Estimada</th>
                      <th className="px-6 py-4 text-[10px] font-black text-muted-foreground uppercase text-right">Monto</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-white/5">
                    {flujoProyectado?.detalle.map((d, i) => (
                      <tr key={i} className="hover:bg-white/[0.02] transition-colors">
                        <td className="px-6 py-4 text-xs font-bold text-foreground uppercase">{d.concepto}</td>
                        <td className="px-6 py-4 text-center">
                          <span className={cn(
                            "px-2 py-0.5 rounded text-[8px] font-black uppercase",
                            d.tipo === 'Ingreso' ? "bg-emerald-500/10 text-emerald-400" : 
                            d.tipo === 'Proyeccion' ? "bg-blue-500/10 text-blue-400" : "bg-red-500/10 text-red-400"
                          )}>
                            {d.tipo}
                          </span>
                        </td>
                        <td className="px-6 py-4 text-xs text-muted-foreground font-medium">{new Date(d.fechaEstimada).toLocaleDateString()}</td>
                        <td className="px-6 py-4 text-right font-black text-foreground tracking-tighter">Bs. {d.monto.toLocaleString()}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
}


