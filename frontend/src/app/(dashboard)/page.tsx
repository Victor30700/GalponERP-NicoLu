"use client";

import { useAuth } from "@/context/AuthContext";
import { motion } from "framer-motion";
import { Bird, TrendingUp, AlertTriangle, DollarSign } from "lucide-react";

export default function Home() {
  const { profile, loading } = useAuth();

  if (loading) return null;

  const stats = [
    { name: 'Aves Vivas', value: '12,450', icon: Bird, color: 'text-emerald-500', bg: 'bg-emerald-500/10' },
    { name: 'Rendimiento (FCR)', value: '1.45', icon: TrendingUp, color: 'text-blue-500', bg: 'bg-blue-500/10' },
    { name: 'Alertas Sanitarias', value: '2', icon: AlertTriangle, color: 'text-amber-500', bg: 'bg-amber-500/10' },
    { name: 'Balance Mensual', value: '$45,200', icon: DollarSign, color: 'text-primary', bg: 'bg-primary/10' },
  ];

  return (
    <div className="space-y-8">
      <div>
        <motion.h1 
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          className="text-3xl font-bold text-white"
        >
          ¡Hola, {profile?.nombre || 'Usuario'}!
        </motion.h1>
        <p className="text-slate-400 mt-1">Aquí tienes un resumen de la producción de hoy.</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        {stats.map((stat, index) => (
          <motion.div
            key={stat.name}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: index * 0.1 }}
            className="p-6 glass-dark rounded-2xl border border-white/5"
          >
            <div className="flex items-center justify-between">
              <div className={`p-3 rounded-xl ${stat.bg} ${stat.color}`}>
                <stat.icon size={24} />
              </div>
              <span className="text-2xl font-bold text-white">{stat.value}</span>
            </div>
            <p className="text-slate-400 mt-4 font-medium">{stat.name}</p>
          </motion.div>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <motion.div
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ delay: 0.4 }}
          className="p-8 glass-dark rounded-3xl border border-white/5 h-80 flex items-center justify-center"
        >
          <div className="text-center">
            <TrendingUp size={48} className="text-slate-700 mx-auto mb-4" />
            <p className="text-slate-500 font-medium">Gráfico de Crecimiento (Próximamente)</p>
          </div>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, scale: 0.95 }}
          animate={{ opacity: 1, scale: 1 }}
          transition={{ delay: 0.5 }}
          className="p-8 glass-dark rounded-3xl border border-white/5 h-80 flex items-center justify-center"
        >
          <div className="text-center">
            <Bird size={48} className="text-slate-700 mx-auto mb-4" />
            <p className="text-slate-500 font-medium">Distribución de Lotes (Próximamente)</p>
          </div>
        </motion.div>
      </div>
    </div>
  );
}
