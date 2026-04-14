"use client";

import { useAuth } from "@/context/AuthContext";
import { motion } from "framer-motion";
import { 
  Bird, TrendingUp, AlertTriangle, DollarSign, 
  Droplets, ClipboardCheck, Package, LayoutDashboard,
  ArrowUpRight, ArrowDownRight, Warehouse
} from "lucide-react";
import { useDashboard } from "@/hooks/useDashboard";
import { UniversalGrid } from "@/components/shared/UniversalGrid";

export default function DashboardPage() {
  const { profile, loading: authLoading } = useAuth();
  const { 
    resumen, 
    isLoadingResumen, 
    comparativaLotes, 
    isLoadingLotes,
    comparativaGalpones,
    isLoadingGalpones 
  } = useDashboard();

  if (authLoading) return null;

  const stats = [
    { 
      name: 'Aves en Producción', 
      value: resumen?.totalPollosVivos.toLocaleString() || '0', 
      icon: Bird, 
      color: 'text-emerald-500', 
      bg: 'bg-emerald-500/10',
      description: 'Población total activa'
    },
    { 
      name: 'Tareas Hoy', 
      value: resumen?.tareasSanitariasHoy || '0', 
      icon: ClipboardCheck, 
      color: 'text-blue-500', 
      bg: 'bg-blue-500/10',
      description: 'Actividades programadas'
    },
    { 
      name: 'Mortalidad Mes', 
      value: resumen?.mortalidadMesActual || '0', 
      icon: AlertTriangle, 
      color: 'text-red-500', 
      bg: 'bg-red-500/10',
      description: 'Bajas registradas'
    },
    { 
      name: 'Inversión Activa', 
      value: `$${resumen?.inversionTotalEnCurso.toLocaleString()}` || '$0', 
      icon: DollarSign, 
      color: 'text-amber-500', 
      bg: 'bg-amber-500/10',
      description: 'Capital en galpones'
    },
  ];

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <motion.h1 
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            className="text-3xl font-black text-foreground uppercase tracking-tight flex items-center gap-3"
          >
            <LayoutDashboard className="text-primary" size={32} />
            Panel de Control
          </motion.h1>
          <p className="text-muted-foreground mt-1 font-medium">Bienvenido, {profile?.nombre}. Resumen operativo de hoy.</p>
        </div>
        
        <div className="hidden md:flex items-center gap-4 px-6 py-3 bg-muted/50 border border-border rounded-2xl">
           <div className="flex flex-col items-end border-r border-border pr-4">
              <span className="text-[10px] font-black text-muted-foreground uppercase">Stock Alimento</span>
              <span className={`text-sm font-bold ${resumen?.requiereAlertaAlimento ? 'text-red-400' : 'text-emerald-400'}`}>
                {resumen?.stockAlimentoActual.toLocaleString()} kg
              </span>
           </div>
           <div className="flex flex-col items-end">
              <span className="text-[10px] font-black text-muted-foreground uppercase">Autonomía</span>
              <span className="text-sm font-bold text-foreground">{resumen?.diasAlimentoRestantes} días</span>
           </div>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {stats.map((stat, index) => (
          <motion.div
            key={stat.name}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: index * 0.1 }}
            className="p-6 glass rounded-3xl border border-border relative overflow-hidden group"
          >
            <div className="flex items-center justify-between relative z-10">
              <div className={`p-4 rounded-2xl ${stat.bg} ${stat.color} group-hover:scale-110 transition-transform`}>
                <stat.icon size={28} />
              </div>
              <div className="text-right">
                <span className="text-3xl font-black text-foreground block">{stat.value}</span>
                <span className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">{stat.name}</span>
              </div>
            </div>
            <p className="text-xs text-muted-foreground mt-4 font-bold border-t border-border pt-4 opacity-60 italic">{stat.description}</p>
          </motion.div>
        ))}
      </div>

      {/* Alertas de Stock */}
      {resumen?.alertasStockMinimo && resumen.alertasStockMinimo.length > 0 && (
        <motion.div
          initial={{ opacity: 0, scale: 0.98 }}
          animate={{ opacity: 1, scale: 1 }}
          className="p-6 bg-red-500/10 border border-red-500/20 rounded-3xl flex flex-wrap items-center gap-6"
        >
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 rounded-2xl bg-red-500/20 text-red-500 flex items-center justify-center">
              <Package size={24} />
            </div>
            <div>
              <h4 className="text-sm font-black text-foreground uppercase tracking-widest">Alertas de Inventario</h4>
              <p className="text-xs text-red-400/70 font-bold uppercase tracking-tighter">Productos por debajo del stock mínimo</p>
            </div>
          </div>
          <div className="flex flex-wrap gap-2 flex-1">
            {resumen.alertasStockMinimo.map((alerta, i) => (
              <div key={i} className="px-4 py-2 bg-red-500/20 border border-red-500/20 rounded-xl flex items-center gap-3">
                <span className="text-xs font-black text-foreground uppercase">{alerta.productoNombre}</span>
                <span className="text-xs font-black text-red-400">{alerta.stockActual} / {alerta.umbralMinimo}</span>
              </div>
            ))}
          </div>
        </motion.div>
      )}

      {/* Main Content Area */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Comparativa de Galpones */}
        <div className="space-y-4">
          <div className="flex items-center justify-between px-2">
            <h3 className="text-lg font-black text-foreground uppercase tracking-widest flex items-center gap-2">
              <Warehouse className="text-primary" size={20} />
              Galpones
            </h3>
          </div>
          <div className="glass rounded-[2.5rem] border border-border p-2">
            <UniversalGrid
              title="Galpones"
              items={comparativaGalpones.map(g => ({ ...g, id: g.galponId }))}
              isLoading={isLoadingGalpones}
              columns={[
                { header: 'Galpón', accessor: (item) => <span className="font-black text-foreground uppercase">{item.nombre}</span> },
                { header: 'Lotes', accessor: 'totalLotes' },
                { 
                  header: 'Utilidad', 
                  accessor: (item) => (
                    <span className="font-black text-emerald-400">
                      ${item.utilidadTotalAcumulada.toLocaleString()}
                    </span>
                  ) 
                },
                { 
                  header: 'Mortalidad', 
                  accessor: (item) => (
                    <span className="text-xs font-bold text-muted-foreground">
                      {item.promedioMortalidad.toFixed(1)}%
                    </span>
                  ) 
                }
              ]}
              renderMobileCard={(item) => (
                <div className="flex justify-between items-center">
                  <div>
                    <p className="font-black text-foreground uppercase">{item.nombre}</p>
                    <p className="text-[10px] text-muted-foreground font-bold uppercase tracking-widest">{item.totalLotes} lotes activos</p>
                  </div>
                  <div className="text-right">
                    <p className="font-black text-emerald-400">${item.utilidadTotalAcumulada.toLocaleString()}</p>
                    <p className="text-[10px] text-red-400 font-bold uppercase">{item.promedioMortalidad.toFixed(1)}% mort.</p>
                  </div>
                </div>
              )}
            />
          </div>
        </div>

        {/* Comparativa de Lotes */}
        <div className="space-y-4">
          <div className="flex items-center justify-between px-2">
            <h3 className="text-lg font-black text-foreground uppercase tracking-widest flex items-center gap-2">
              <TrendingUp className="text-primary" size={20} />
              Ranking de Lotes
            </h3>
          </div>
          <div className="glass rounded-[2.5rem] border border-border p-2">
            <UniversalGrid
              title="Lotes"
              items={comparativaLotes.map(l => ({ ...l, id: l.loteId }))}
              isLoading={isLoadingLotes}
              columns={[
                { header: 'Lote ID', accessor: (item) => <span className="font-mono text-[10px] font-black uppercase text-muted-foreground">{item.loteId.split('-')[0]}</span> },
                { 
                  header: 'Utilidad Neta', 
                  accessor: (item) => (
                    <span className={`font-black ${item.utilidadNeta >= 0 ? 'text-emerald-400' : 'text-red-400'}`}>
                      ${item.utilidadNeta.toLocaleString()}
                    </span>
                  ) 
                },
                { 
                  header: 'Mortalidad', 
                  accessor: (item) => (
                    <span className="text-xs font-bold text-muted-foreground">
                      {item.mortalidadTotal} aves
                    </span>
                  ) 
                }
              ]}
              renderMobileCard={(item) => (
                <div className="flex justify-between items-center">
                  <div>
                    <p className="font-black text-foreground uppercase">Lote {item.loteId.split('-')[0]}</p>
                    <p className="text-[10px] text-muted-foreground font-bold uppercase tracking-widest">Ingreso: {new Date(item.fechaIngreso).toLocaleDateString()}</p>
                  </div>
                  <div className="text-right">
                    <p className={`font-black ${item.utilidadNeta >= 0 ? 'text-emerald-400' : 'text-red-400'}`}>
                      ${item.utilidadNeta.toLocaleString()}
                    </p>
                  </div>
                </div>
              )}
            />
          </div>
        </div>
      </div>
    </div>
  );
}


