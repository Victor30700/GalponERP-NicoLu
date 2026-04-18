'use client'

import { useLotes, Lote } from '@/hooks/useLotes'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { Bird, Calendar, ArrowRight, RefreshCw, Plus, Search } from 'lucide-react'
import Link from 'next/link'
import { cn } from '@/lib/utils'
import { useState } from 'react'
import { LoteFormModal } from '@/components/production/LoteFormModal'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/lib/rbac'

export default function LotesPage() {
  const { profile } = useAuth()
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null
  const isEmpleado = userRole === UserRole.Empleado

  const [soloActivos, setSoloActivos] = useState(true)
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [busqueda, setBusqueda] = useState('')
  const [mes, setMes] = useState<number | ''>('')
  const [anio, setAnio] = useState<number | ''>('')

  const { lotes, isLoading, isFetching, refresh } = useLotes({
    soloActivos,
    busqueda,
    mes,
    anio
  })

  const handleRefresh = () => {
    refresh()
  }

  const columns = [
    { 
      header: 'Lote / Galpón', 
      accessor: (item: Lote) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-primary border border-primary/20">
            <Bird size={20} />
          </div>
          <div>
            <p className="font-bold text-foreground">{item.nombre || item.nombreLote}</p>
            <p className="text-xs text-muted-foreground uppercase tracking-wider">{item.galponNombre}</p>
          </div>
        </div>
      )
    },
    { 
      header: 'Población', 
      accessor: (item: Lote) => (
        <div>
          <p className="text-foreground font-medium">{(item.avesVivas ?? 0).toLocaleString()} vivas</p>
          <p className="text-[10px] text-muted-foreground">Inició con {item.cantidadInicial}</p>
        </div>
      )
    },
    { 
      header: 'Rendimiento', 
      accessor: (item: Lote) => (
        <div className="flex items-center gap-4">
          <div>
            <p className="text-[10px] text-muted-foreground uppercase font-bold">FCR</p>
            <p className="text-blue-400 font-bold">{(item.fcrActual ?? 0).toFixed(2)}</p>
          </div>
          <div>
            <p className="text-[10px] text-muted-foreground uppercase font-bold">Bajas</p>
            <p className="text-red-400 font-bold">{(item.mortalidadPorcentaje ?? 0).toFixed(1)}%</p>
          </div>
        </div>
      )
    },
    {
      header: 'Estado',
      accessor: (item: Lote) => (
        <span className={cn(
          "px-2 py-1 rounded-md text-[10px] font-bold uppercase tracking-widest",
          item.estado === 'Activo' ? "bg-emerald-500/10 text-emerald-500 border border-emerald-500/20" : 
          item.estado === 'Cerrado' ? "bg-slate-500/10 text-muted-foreground border border-border" :
          "bg-red-500/10 text-red-500 border border-red-500/20"
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
          className="flex items-center gap-2 text-primary hover:text-foreground transition-colors font-bold text-xs uppercase tracking-widest"
        >
          Gestionar <ArrowRight size={14} />
        </Link>
      )
    }
  ]

  return (
    <>
      <div className="mb-6 flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-foreground">Gestión de Lotes</h1>
          <p className="text-muted-foreground text-sm">Seguimiento y control de la producción avícola.</p>
        </div>
        
        <div className="flex items-center gap-2">
          <button
            onClick={handleRefresh}
            disabled={isFetching}
            className="p-2 bg-muted/50 text-muted-foreground border border-border rounded-xl hover:text-primary transition-all disabled:opacity-50"
            title="Actualizar lista"
          >
            <RefreshCw size={20} className={cn(isFetching && "animate-spin")} />
          </button>
          
          <button
            onClick={() => setIsModalOpen(true)}
            className="flex items-center gap-2 px-4 py-2 bg-primary hover:bg-primary/90 text-black font-bold rounded-xl transition-all shadow-lg shadow-primary/20"
          >
            <Plus size={20} />
            <span className="hidden sm:inline">Nuevo Lote</span>
          </button>
        </div>
      </div>

      <div className="mb-6 bg-muted/30 p-4 rounded-2xl border border-border/50">
        <div className="flex flex-wrap items-center gap-4 justify-between">
          <div className="flex flex-wrap items-center gap-3">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={16} />
              <input
                type="text"
                placeholder="Buscar lote o galpón..."
                value={busqueda}
                onChange={(e) => setBusqueda(e.target.value)}
                className="bg-background border border-border rounded-xl pl-9 pr-4 py-2 text-xs font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 w-64 transition-all"
              />
            </div>
            
            <div className="flex bg-muted/50 p-1 rounded-xl border border-border">
              <button 
                onClick={() => setSoloActivos(true)}
                className={cn(
                  "px-4 py-1.5 rounded-lg text-[10px] font-bold uppercase tracking-wider transition-all",
                  soloActivos 
                    ? "bg-background text-primary shadow-sm" 
                    : "text-muted-foreground hover:text-foreground"
                )}
              >
                Activos
              </button>
              <button 
                onClick={() => setSoloActivos(false)}
                className={cn(
                  "px-4 py-1.5 rounded-lg text-[10px] font-bold uppercase tracking-wider transition-all",
                  !soloActivos 
                    ? "bg-background text-primary shadow-sm" 
                    : "text-muted-foreground hover:text-foreground"
                )}
              >
                Todos
              </button>
            </div>

            <div className="h-8 w-[1px] bg-border mx-1 hidden md:block" />

            <select
              value={mes}
              onChange={(e) => setMes(e.target.value ? Number(e.target.value) : '')}
              className="bg-background border border-border rounded-xl px-4 py-2 text-xs font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 appearance-none cursor-pointer"
            >
              <option value="">Todos los Meses</option>
              {Array.from({ length: 12 }, (_, i) => (
                <option key={i + 1} value={i + 1}>
                  {new Date(2000, i).toLocaleString('default', { month: 'long' })}
                </option>
              ))}
            </select>

            <select
              value={anio}
              onChange={(e) => setAnio(e.target.value ? Number(e.target.value) : '')}
              className="bg-background border border-border rounded-xl px-4 py-2 text-xs font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 appearance-none cursor-pointer"
            >
              <option value="">Cualquier Año</option>
              {[2024, 2025, 2026, 2027, 2028].map(y => (
                <option key={y} value={y}>{y}</option>
              ))}
            </select>
          </div>
        </div>
      </div>

      <UniversalGrid
        title="Gestión de Lotes"
        items={lotes}
        columns={columns}
        isLoading={isLoading}
        hideHeaderSearch
        hideHeaderTitle
        onAdd={() => setIsModalOpen(true)}
        renderMobileCard={(item) => (
          <Link href={`/lotes/${item.id}`} className="block space-y-4">
            <div className="flex justify-between items-start">
              <div className="flex items-center gap-3">
                <div className="w-12 h-12 rounded-2xl bg-primary/10 flex items-center justify-center text-primary border border-primary/20">
                  <Bird size={24} />
                </div>
                <div>
                  <h3 className="text-xl font-black text-foreground">{item.nombre || item.nombreLote}</h3>
                  <p className="text-xs text-primary font-bold uppercase tracking-widest">{item.galponNombre}</p>
                </div>
              </div>
              <span className={cn(
                "text-[10px] font-black px-2 py-1 rounded border uppercase",
                item.estado === 'Activo' ? "bg-emerald-500/10 text-emerald-500 border-emerald-500/10" : 
                item.estado === 'Cerrado' ? "bg-slate-500/10 text-muted-foreground border-border" :
                "bg-red-500/10 text-red-500 border-red-500/10"
              )}>
                {item.estado}
              </span>
            </div>

            <div className="grid grid-cols-2 gap-2">
              <div className="p-3 bg-muted/50 rounded-xl border border-border">
                <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider mb-1">Aves Vivas</p>
                <p className="text-foreground font-black text-lg">{(item.avesVivas ?? 0).toLocaleString()}</p>
              </div>
              <div className="p-3 bg-muted/50 rounded-xl border border-border">
                <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider mb-1">FCR Actual</p>
                <p className="text-blue-400 font-black text-lg">{(item.fcrActual ?? 0).toFixed(2)}</p>
              </div>
            </div>

            <div className="flex items-center justify-between text-xs font-bold text-muted-foreground uppercase tracking-widest pt-2">
              <div className="flex items-center gap-2">
                <Calendar size={14} />
                <span>Inició: {new Date(item.fechaInicio).toLocaleDateString()}</span>
              </div>
              <ArrowRight size={16} className="text-primary" />
            </div>
          </Link>
        )}
      />

      <LoteFormModal 
        isOpen={isModalOpen} 
        onClose={() => setIsModalOpen(false)} 
      />
    </>
  )
}


