'use client'

import { format } from 'date-fns'
import { es } from 'date-fns/locale'
import { Edit2, Trash2, User } from 'lucide-react'
import { motion, AnimatePresence } from 'framer-motion'
import { useSwal } from '@/hooks/useSwal'

interface OperationHistoryListProps {
  data: any[]
  type: 'mortality' | 'feed' | 'water' | 'weight'
  onEdit: (item: any) => void
  onDelete: (id: string) => void
  isLoading?: boolean
}

export function OperationHistoryList({ data, type, onEdit, onDelete, isLoading }: OperationHistoryListProps) {
  const { confirm } = useSwal()

  const config = {
    mortality: {
      label: 'Bajas',
      valueKey: 'cantidadBajas',
      unit: 'aves',
      color: 'text-red-500',
      bg: 'bg-red-500/10'
    },
    feed: {
      label: 'Alimento',
      valueKey: 'cantidad',
      unit: 'kg',
      color: 'text-blue-500',
      bg: 'bg-blue-500/10'
    },
    water: {
      label: 'Agua',
      valueKey: 'consumoAgua',
      unit: 'L',
      color: 'text-amber-500',
      bg: 'bg-amber-500/10'
    },
    weight: {
      label: 'Pesaje',
      valueKey: 'pesoPromedioGramos',
      unit: 'g',
      color: 'text-indigo-500',
      bg: 'bg-indigo-500/10'
    }
  }[type]

  const handleDelete = async (id: string) => {
    const isConfirmed = await confirm(
      '¿Eliminar registro?',
      'Esta acción no se puede deshacer y afectará los KPIs del lote.',
      'warning'
    )
    if (isConfirmed) {
      onDelete(id)
    }
  }

  if (isLoading) {
    return (
      <div className="space-y-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-20 bg-muted/50 animate-pulse rounded-2xl border border-border" />
        ))}
      </div>
    )
  }

  if (data.length === 0) {
    return (
      <div className="text-center py-12 glass rounded-[2rem] border border-dashed border-border">
        <p className="text-muted-foreground font-medium uppercase tracking-widest text-xs">No hay registros para mostrar</p>
      </div>
    )
  }

  return (
    <div className="space-y-3">
      <AnimatePresence mode="popLayout">
        {data.map((item) => (
          <motion.div
            key={item.id}
            layout
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.95 }}
            className="group p-4 glass rounded-2xl border border-border hover:border-primary/30 transition-all flex items-center justify-between"
          >
            <div className="flex items-center gap-4">
              <div className={`w-12 h-12 rounded-xl flex flex-col items-center justify-center font-black ${config.bg} ${config.color}`}>
                <span className="text-sm leading-none">{format(new Date(item.fecha), 'dd')}</span>
                <span className="text-[10px] uppercase leading-none mt-0.5">{format(new Date(item.fecha), 'MMM', { locale: es })}</span>
              </div>
              <div>
                <div className="flex items-center gap-2">
                  <span className="text-lg font-black text-foreground">
                    {item[config.valueKey]} {config.unit}
                  </span>
                  {type === 'weight' && item.cantidadMuestreada && (
                    <span className="text-[10px] font-black text-muted-foreground uppercase bg-muted px-2 py-0.5 rounded-full">
                      n={item.cantidadMuestreada}
                    </span>
                  )}
                  {type === 'feed' && item.nombreProducto && (
                    <span className="text-[10px] font-black text-blue-400 uppercase bg-blue-500/10 px-2 py-0.5 rounded-full border border-blue-500/20">
                      {item.nombreProducto}
                    </span>
                  )}
                </div>
                <div className="flex items-center gap-3 text-muted-foreground">
                  <span className="text-[10px] font-bold uppercase tracking-widest truncate max-w-[150px]">
                    {item.causa || item.justificacion || item.observaciones || 'Sin notas'}
                  </span>
                  <div className="flex items-center gap-1 text-[10px] font-black text-primary/40">
                     <User size={10} />
                     <span className="uppercase">Sistema</span>
                  </div>
                </div>
              </div>
            </div>

            <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
              <button
                onClick={() => onEdit(item)}
                className="p-2.5 hover:bg-primary/10 text-muted-foreground hover:text-primary rounded-xl transition-colors"
                title="Editar"
              >
                <Edit2 size={18} />
              </button>
              <button
                onClick={() => handleDelete(item.id)}
                className="p-2.5 hover:bg-red-500/10 text-muted-foreground hover:text-red-500 rounded-xl transition-colors"
                title="Eliminar"
              >
                <Trash2 size={18} />
              </button>
            </div>
          </motion.div>
        ))}
      </AnimatePresence>
    </div>
  )
}
