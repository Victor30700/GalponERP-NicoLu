"use client";

import { useState } from "react";
import { motion } from "framer-motion";
import { 
  ClipboardList, TrendingUp, DollarSign, Bird, 
  Scale, Calculator, ArrowRight, PieChart, Info
} from "lucide-react";
import { useSimulacion, SimulacionParams } from "@/hooks/usePlanificacion";

export default function PlanificacionPage() {
  const [params, setParams] = useState<SimulacionParams>({
    cantidadPollos: 1000,
    pesoEsperadoPorPolloKg: 2.5,
    precioAlimentoPorKg: 0.85,
    precioVentaPorKg: 1.80,
    fcrPersonalizado: 1.6
  });

  const [enabled, setEnabled] = useState(false);

  const { data: resultado, isLoading } = useSimulacion(params, enabled);

  const handleSimulate = (e: React.FormEvent) => {
    e.preventDefault();
    setEnabled(true);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setParams(prev => ({
      ...prev,
      [name]: parseFloat(value) || 0
    }));
    setEnabled(false);
  };

  return (
    <div className="space-y-8">
       {/* Header */}
       <div className="flex items-center justify-between">
        <div>
          <motion.h1 
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            className="text-3xl font-black text-foreground uppercase tracking-tight flex items-center gap-3"
          >
            <ClipboardList className="text-primary" size={32} />
            Planificación y Simulación
          </motion.h1>
          <p className="text-muted-foreground mt-1 font-medium">Proyecta la rentabilidad de tu próximo lote.</p>
        </div>
      </div>

      {/* Guía de Uso */}
      <motion.div 
        initial={{ opacity: 0, y: 10 }}
        animate={{ opacity: 1, y: 0 }}
        className="p-6 glass rounded-[2rem] border border-primary/20 bg-primary/5 flex flex-col md:flex-row gap-6 items-start md:items-center"
      >
        <div className="p-4 rounded-2xl bg-primary/20 text-primary">
          <Info size={32} />
        </div>
        <div className="space-y-2 flex-1">
          <h3 className="text-lg font-black text-foreground uppercase tracking-widest">¿Cómo usar el simulador?</h3>
          <p className="text-sm text-muted-foreground font-medium leading-relaxed">
            Esta herramienta te permite <b>estimar tus ganancias y costos</b> antes de iniciar un lote. Ingresa la cantidad de aves y el peso que esperas que alcancen. El sistema usará el <b>FCR (Conversión Alimenticia)</b> para calcular cuánto alimento necesitarás comprar y en qué etapas del crecimiento se consumirá más.
          </p>
          <div className="flex flex-wrap gap-4 pt-2">
            <div className="flex items-center gap-2 text-[10px] font-black uppercase text-primary/70">
              <span className="w-4 h-4 rounded-full bg-primary/20 flex items-center justify-center text-[8px]">1</span>
              Define la población
            </div>
            <div className="flex items-center gap-2 text-[10px] font-black uppercase text-primary/70">
              <span className="w-4 h-4 rounded-full bg-primary/20 flex items-center justify-center text-[8px]">2</span>
              Ajusta precios de mercado
            </div>
            <div className="flex items-center gap-2 text-[10px] font-black uppercase text-primary/70">
              <span className="w-4 h-4 rounded-full bg-primary/20 flex items-center justify-center text-[8px]">3</span>
              Obtén tu proyección financiera
            </div>
          </div>
        </div>
      </motion.div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Formulario de Entrada */}
        <motion.div 
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="lg:col-span-1 glass p-8 rounded-[2.5rem] border border-border space-y-6"
        >
          <div className="flex items-center gap-3 mb-2">
             <div className="p-3 rounded-2xl bg-primary/10 text-primary">
                <Calculator size={24} />
             </div>
             <h2 className="text-xl font-black text-foreground uppercase">Parámetros</h2>
          </div>

          <form onSubmit={handleSimulate} className="space-y-4">
            <div className="space-y-2">
              <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Cantidad de Pollos</label>
              <div className="relative">
                <Bird className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <input 
                  type="number" 
                  name="cantidadPollos"
                  value={params.cantidadPollos}
                  onChange={handleChange}
                  className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                  placeholder="Ej: 1000"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Peso Esperado (kg)</label>
              <div className="relative">
                <Scale className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <input 
                  type="number" 
                  step="0.01"
                  name="pesoEsperadoPorPolloKg"
                  value={params.pesoEsperadoPorPolloKg}
                  onChange={handleChange}
                  className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Precio Alimento / kg</label>
              <div className="relative">
                <DollarSign className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <input 
                  type="number" 
                  step="0.01"
                  name="precioAlimentoPorKg"
                  value={params.precioAlimentoPorKg}
                  onChange={handleChange}
                  className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Precio Venta / kg</label>
              <div className="relative">
                <TrendingUp className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <input 
                  type="number" 
                  step="0.01"
                  name="precioVentaPorKg"
                  value={params.precioVentaPorKg}
                  onChange={handleChange}
                  className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                />
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">FCR (Conversión Alimenticia)</label>
              <div className="relative">
                <PieChart className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                <input 
                  type="number" 
                  step="0.01"
                  name="fcrPersonalizado"
                  value={params.fcrPersonalizado}
                  onChange={handleChange}
                  className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                />
              </div>
            </div>

            <button 
              type="submit"
              disabled={isLoading}
              className="w-full bg-primary text-primary-foreground font-black py-4 rounded-2xl uppercase tracking-widest flex items-center justify-center gap-2 hover:opacity-90 transition-opacity disabled:opacity-50"
            >
              {isLoading ? "Simulando..." : "Calcular Proyección"}
              <ArrowRight size={20} />
            </button>
          </form>
        </motion.div>

        {/* Resultados */}
        <div className="lg:col-span-2 space-y-8">
          {resultado ? (
            <motion.div 
              initial={{ opacity: 0, x: 20 }}
              animate={{ opacity: 1, x: 0 }}
              className="space-y-8"
            >
              {/* Resumen Cards */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                 <div className="p-6 glass rounded-3xl border border-border">
                    <span className="text-[10px] font-black text-muted-foreground uppercase tracking-widest block mb-1">Ingresos Proyectados</span>
                    <span className="text-3xl font-black text-foreground">Bs. {resultado.ingresosProyectados.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</span>
                 </div>
                 <div className="p-6 glass rounded-3xl border border-border">
                    <span className="text-[10px] font-black text-muted-foreground uppercase tracking-widest block mb-1">Utilidad Bruta</span>
                    <span className="text-3xl font-black text-emerald-400">Bs. {resultado.utilidadBrutaProyectada.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</span>
                 </div>
                 <div className="p-6 glass rounded-3xl border border-border">
                    <span className="text-[10px] font-black text-muted-foreground uppercase tracking-widest block mb-1">Costo Alimento Total</span>
                    <span className="text-3xl font-black text-red-400">Bs. {resultado.costoAlimentoTotal.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</span>
                 </div>
                 <div className="p-6 glass rounded-3xl border border-border">
                    <span className="text-[10px] font-black text-muted-foreground uppercase tracking-widest block mb-1">Alimento Necesario</span>
                    <span className="text-3xl font-black text-blue-400">{resultado.alimentoTotalKg.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })} kg</span>
                 </div>
              </div>

              {/* Detalles por Etapa */}
              <div className="glass rounded-[2.5rem] border border-border overflow-hidden">
                <div className="p-8 border-b border-border">
                  <h3 className="text-xl font-black text-foreground uppercase">Desglose por Etapas</h3>
                </div>
                <div className="p-2 overflow-x-auto">
                  <table className="w-full text-left">
                    <thead>
                      <tr className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">
                        <th className="px-6 py-4">Etapa</th>
                        <th className="px-6 py-4">Días</th>
                        <th className="px-6 py-4">Consumo (kg)</th>
                        <th className="px-6 py-4">Costo Estimado</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-white/5">
                      {resultado.detallesEtapas.map((etapa, i) => (
                        <tr key={i} className="group hover:bg-white/[0.02] transition-colors">
                          <td className="px-6 py-4">
                            <span className="font-black text-foreground uppercase">{etapa.etapa}</span>
                          </td>
                          <td className="px-6 py-4 text-muted-foreground font-bold">
                            {etapa.diasInicio} - {etapa.diasFin}
                          </td>
                          <td className="px-6 py-4 text-foreground font-bold">
                            {etapa.consumoKg.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })} kg
                          </td>
                          <td className="px-6 py-4 text-emerald-400 font-black">
                            Bs. {etapa.costoEstimado.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>

              {/* Informacion adicional */}
              <div className="p-6 bg-blue-500/10 border border-blue-500/20 rounded-3xl flex items-start gap-4">
                <div className="p-3 rounded-2xl bg-blue-500/20 text-blue-500">
                  <Info size={20} />
                </div>
                <div>
                  <h4 className="text-sm font-black text-foreground uppercase tracking-widest mb-1">Nota sobre la simulación</h4>
                  <p className="text-xs text-blue-400/70 font-medium leading-relaxed">
                    Estos cálculos son proyecciones basadas en modelos teóricos de crecimiento y conversión alimenticia. 
                    Factores externos como clima, sanidad y manejo pueden afectar los resultados finales reales.
                  </p>
                </div>
              </div>
            </motion.div>
          ) : (
            <div className="h-full min-h-[400px] flex flex-col items-center justify-center text-center space-y-4 glass rounded-[2.5rem] border border-border border-dashed">
              <div className="p-6 rounded-full bg-muted/50 text-muted-foreground">
                <TrendingUp size={48} />
              </div>
              <div>
                <h3 className="text-xl font-black text-foreground uppercase">Esperando Parámetros</h3>
                <p className="text-muted-foreground font-medium max-w-xs mx-auto">Ingresa los datos de tu próximo lote a la izquierda para ver la proyección de rentabilidad.</p>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}


