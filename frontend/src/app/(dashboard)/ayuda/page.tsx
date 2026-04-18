"use client";

import { motion } from "framer-motion";
import { 
  HelpCircle, 
  DollarSign, 
  Package, 
  Bird, 
  TrendingUp, 
  AlertTriangle,
  Info,
  Scale,
  Clock,
  ChevronRight
} from "lucide-react";

const sections = [
  {
    title: "Métricas Financieras",
    icon: DollarSign,
    color: "text-emerald-400",
    bg: "bg-emerald-500/10",
    items: [
      {
        label: "Utilidad Neta",
        description: "Es la ganancia real de un lote. Se calcula restando de las ventas totales: el costo de los pollitos, los gastos operativos (luz, agua, sueldos) y el costo del alimento/medicinas consumidos."
      },
      {
        label: "Inversión Activa",
        description: "Representa cuánto capital tienes 'en el campo' actualmente. Incluye el valor de los pollos vivos y todos los insumos que ya han consumido hasta el día de hoy."
      }
    ]
  },
  {
    title: "Inventario y Alimento",
    icon: Package,
    color: "text-amber-400",
    bg: "bg-amber-500/10",
    items: [
      {
        label: "Autonomía de Alimento",
        description: "Días estimados que durará el alimento en bodega. El sistema calcula cuánto comen tus pollos al día (basado en su edad y consumo histórico) y lo divide por el stock actual."
      },
      {
        label: "Stock Mínimo / Umbral",
        description: "Es el nivel crítico de inventario. Si un producto baja de este número, el sistema mostrará alertas rojas para que realices una compra a tiempo."
      }
    ]
  },
  {
    title: "Desempeño del Lote (Producción)",
    icon: Bird,
    color: "text-blue-400",
    bg: "bg-blue-500/10",
    items: [
      {
        label: "FCR (Conversión Alimenticia)",
        description: "Índice de eficiencia. Indica cuántos kilos de alimento necesita el pollo para ganar 1 kilo de carne. Entre más bajo sea este número, más eficiente es tu lote."
      },
      {
        label: "Mortalidad Acumulada",
        description: "Porcentaje de aves perdidas desde el inicio del lote. Es clave para medir la sanidad y el manejo del galpón."
      }
    ]
  }
];

export default function AyudaPage() {
  return (
    <div className="max-w-4xl mx-auto space-y-8 pb-12">
      {/* Header */}
      <div className="text-center space-y-4">
        <motion.div 
          initial={{ scale: 0 }}
          animate={{ scale: 1 }}
          className="w-20 h-20 bg-primary/10 text-primary rounded-3xl flex items-center justify-center mx-auto"
        >
          <HelpCircle size={40} />
        </motion.div>
        <motion.h1 
          initial={{ opacity: 0, y: 10 }}
          animate={{ opacity: 1, y: 0 }}
          className="text-4xl font-black text-foreground uppercase tracking-tight"
        >
          Centro de Ayuda
        </motion.h1>
        <p className="text-muted-foreground font-medium max-w-lg mx-auto">
          Guía rápida para entender los indicadores y el funcionamiento de GalponERP.
        </p>
      </div>

      {/* Grid de Secciones */}
      <div className="grid grid-cols-1 gap-6">
        {sections.map((section, idx) => (
          <motion.div
            key={section.title}
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: idx * 0.1 }}
            className="glass rounded-[2.5rem] border border-border overflow-hidden"
          >
            <div className="p-8 space-y-6">
              <div className="flex items-center gap-4">
                <div className={`p-3 rounded-2xl ${section.bg} ${section.color}`}>
                  <section.icon size={24} />
                </div>
                <h2 className="text-xl font-black text-foreground uppercase tracking-widest">
                  {section.title}
                </h2>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {section.items.map((item) => (
                  <div 
                    key={item.label}
                    className="p-6 bg-muted/30 rounded-3xl border border-border/50 hover:border-primary/30 transition-colors group"
                  >
                    <div className="flex items-center gap-2 mb-3">
                      <div className="w-1.5 h-6 bg-primary rounded-full" />
                      <h3 className="font-black text-foreground uppercase text-sm tracking-wider">
                        {item.label}
                      </h3>
                    </div>
                    <p className="text-sm text-muted-foreground leading-relaxed font-medium">
                      {item.description}
                    </p>
                  </div>
                ))}
              </div>
            </div>
          </motion.div>
        ))}
      </div>

      {/* Footer Info */}
      <motion.div 
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ delay: 0.5 }}
        className="p-8 bg-blue-500/5 border border-blue-500/10 rounded-[2.5rem] flex items-center gap-6"
      >
        <div className="w-12 h-12 rounded-2xl bg-blue-500/20 text-blue-400 flex items-center justify-center shrink-0">
          <Info size={24} />
        </div>
        <div>
          <h4 className="font-black text-foreground uppercase text-sm tracking-widest">¿Necesitas más ayuda?</h4>
          <p className="text-xs text-muted-foreground font-medium mt-1">
            Si tienes dudas sobre un cálculo específico o encuentras un comportamiento inesperado, contacta al administrador del sistema.
          </p>
        </div>
      </motion.div>
    </div>
  );
}
