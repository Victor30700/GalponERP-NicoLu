'use client'

import { useQuery } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { Bird, Calendar, Hash, ArrowRight } from 'lucide-react'
import Link from 'next/link'
import { cn } from '@/lib/utils'

interface Lote {
  id: string
  nombreLote: string
  galponNombre: string
  fechaInicio: string
  cantidadInicial: number
  avesVivas: number
  estado: string
  fcrActual: number
  mortalidadPorcentaje: number
}

export default function LotesPage() {
  const { data: lotes = [], isLoading } = useQuery({
    queryKey: ['lotes'],
    queryFn: () => api.get<Lote[]>('/api/Lotes?soloActivos=true'),
  })

  const columns = [
    { 
      header: 'Lote / Galpón', 
      accessor: (item: Lote) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-primary border border-primary/20">
            <Bird size={20} />
          </div>
          <div>
            <p className="font-bold text-white">{item.nombreLote}</p>
            <p className="text-xs text-slate-500 uppercase tracking-wider">{item.galponNombre}</p>
          </div>
        </div>
      )
    },
    { 
      header: 'Población', 
      accessor: (item: Lote) => (
        <div>
          <p className="text-white font-medium">{item.avesVivas.toLocaleString()} vivas</p>
          <p className="text-[10px] text-slate-500">Inició con {item.cantidadInicial}</p>
        </div>
      )
    },
    { 
      header: 'Rendimiento', 
      accessor: (item: Lote) => (
        <div className="flex items-center gap-4">
          <div>
            <p className="text-[10px] text-slate-500 uppercase font-bold">FCR</p>
            <p className="text-blue-400 font-bold">{item.fcrActual.toFixed(2)}</p>
          </div>
          <div>
            <p className="text-[10px] text-slate-500 uppercase font-bold">Bajas</p>
            <p className="text-red-400 font-bold">{item.mortalidadPorcentaje.toFixed(1)}%</p>
          </div>
        </div>
      )
    },
    {
      header: 'Estado',
      accessor: (item: Lote) => (
        <span className={cn(
          "px-2 py-1 rounded-md text-[10px] font-bold uppercase tracking-widest",
          item.estado === 'Activo' ? "bg-emerald-500/10 text-emerald-500 border border-emerald-500/20" : "bg-slate-500/10 text-slate-500 border border-white/5"
        )}>
          {item.estado}
        </span>
      )
    },
    {
      header: '',
      accessor: (item: Lote) => (
        <Link 
          href={`/lotes/${item.id}`}
          className="flex items-center gap-2 text-primary hover:text-white transition-colors font-bold text-xs uppercase tracking-widest"
        >
          Gestionar <ArrowRight size={14} />
        </Link>
      )
    }
  ]

  return (
    <UniversalGrid
      title="Gestión de Lotes"
      items={lotes}
      columns={columns}
      isLoading={isLoading}
      onAdd={() => {}} // TODO: Implementar creación de lote en Fase 3
      renderMobileCard={(item) => (
        <Link href={`/lotes/${item.id}`} className="block space-y-4">
          <div className="flex justify-between items-start">
            <div className="flex items-center gap-3">
              <div className="w-12 h-12 rounded-2xl bg-primary/10 flex items-center justify-center text-primary border border-primary/20">
                <Bird size={24} />
              </div>
              <div>
                <h3 className="text-xl font-black text-white">{item.nombreLote}</h3>
                <p className="text-xs text-primary font-bold uppercase tracking-widest">{item.galponNombre}</p>
              </div>
            </div>
            <span className="bg-emerald-500/10 text-emerald-500 text-[10px] font-black px-2 py-1 rounded border border-emerald-500/10 uppercase">
              {item.estado}
            </span>
          </div>

          <div className="grid grid-cols-2 gap-2">
            <div className="p-3 bg-white/5 rounded-xl border border-white/5">
              <p className="text-[10px] text-slate-500 uppercase font-bold tracking-wider mb-1">Aves Vivas</p>
              <p className="text-white font-black text-lg">{item.avesVivas.toLocaleString()}</p>
            </div>
            <div className="p-3 bg-white/5 rounded-xl border border-white/5">
              <p className="text-[10px] text-slate-500 uppercase font-bold tracking-wider mb-1">FCR Actual</p>
              <p className="text-blue-400 font-black text-lg">{item.fcrActual.toFixed(2)}</p>
            </div>
          </div>

          <div className="flex items-center justify-between text-xs font-bold text-slate-500 uppercase tracking-widest pt-2">
            <div className="flex items-center gap-2">
              <Calendar size={14} />
              <span>Inició: {new Date(item.fechaInicio).toLocaleDateString()}</span>
            </div>
            <ArrowRight size={16} className="text-primary" />
          </div>
        </Link>
      )}
    />
  )
}
