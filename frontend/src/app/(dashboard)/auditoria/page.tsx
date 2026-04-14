'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  ShieldCheck, 
  History, 
  Search, 
  Filter, 
  User, 
  Clock, 
  Database, 
  Eye, 
  RotateCcw,
  X,
  FileJson,
  Calendar,
  AlertTriangle
} from 'lucide-react'
import { toast } from 'sonner'
import { cn } from '@/lib/utils'

// --- Interfaces ---

interface AuditoriaLog {
  id: string
  usuarioId: string
  usuarioNombre: string
  accion: string
  entidad: string
  entidadNombre: string
  entidadId: string
  fecha: string
  detalles: string
  detallesJSON: string
}

// --- Main Component ---

export default function AuditoriaPage() {
  const [searchTerm, setSearchTerm] = useState('')
  const [selectedEntidad, setSelectedEntidad] = useState('')
  const [selectedLog, setSelectedLog] = useState<AuditoriaLog | null>(null)
  const queryClient = useQueryClient()

  // Queries
  const { data: logs = [], isLoading } = useQuery({
    queryKey: ['auditoria', 'logs', selectedEntidad],
    queryFn: () => {
      const params = new URLSearchParams()
      if (selectedEntidad) params.append('entidad', selectedEntidad)
      return api.get<AuditoriaLog[]>(`/api/Auditoria/logs?${params.toString()}`)
    },
  })

  // Mutations
  const restaurarMutation = useMutation({
    mutationFn: (log: AuditoriaLog) => api.patch(`/api/Auditoria/restaurar/${log.entidad}/${log.entidadId}`, {}),
    onSuccess: () => {
      toast.success('Entidad restaurada correctamente')
      queryClient.invalidateQueries({ queryKey: ['auditoria'] })
    },
    onError: (err: any) => toast.error(err.message),
  })

  const filteredLogs = logs.filter(log => 
    log.usuarioNombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
    log.detalles.toLowerCase().includes(searchTerm.toLowerCase()) ||
    log.entidadNombre.toLowerCase().includes(searchTerm.toLowerCase())
  )

  const getActionColor = (accion: string) => {
    switch (accion.toLowerCase()) {
      case 'crear': return 'text-emerald-400 bg-emerald-500/10 border-emerald-500/20'
      case 'actualizar': return 'text-blue-400 bg-blue-500/10 border-blue-500/20'
      case 'eliminar': return 'text-red-400 bg-red-500/10 border-red-500/20'
      default: return 'text-muted-foreground bg-muted/50 border-border'
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-foreground flex items-center gap-2">
            <ShieldCheck className="text-indigo-400" />
            Centro de Auditoría
          </h1>
          <p className="text-muted-foreground text-sm">Trazabilidad completa de acciones críticas en el sistema.</p>
        </div>

        <div className="flex items-center gap-2">
          <div className="relative flex-1 md:w-64">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
            <input
              type="text"
              placeholder="Buscar por usuario o detalle..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-indigo-500/50 transition-all"
            />
          </div>
          <select 
            value={selectedEntidad}
            onChange={(e) => setSelectedEntidad(e.target.value)}
            className="px-4 py-2 bg-muted/50 border border-border rounded-xl text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500/50 appearance-none"
          >
            <option value="">Todas las Entidades</option>
            <option value="Lote">Lotes</option>
            <option value="Mortalidad">Mortalidad</option>
            <option value="Pesaje">Pesajes</option>
            <option value="Venta">Ventas</option>
            <option value="Inventario">Inventario</option>
          </select>
        </div>
      </div>

      {/* Timeline */}
      <div className="relative min-h-[500px]">
        {isLoading ? (
          <div className="absolute inset-0 flex items-center justify-center">
            <div className="w-10 h-10 border-4 border-indigo-500/20 border-t-indigo-500 rounded-full animate-spin" />
          </div>
        ) : (
          <div className="space-y-4">
            {filteredLogs.length === 0 ? (
              <div className="py-20 text-center text-muted-foreground glass rounded-2xl border border-border">
                <History size={48} className="mx-auto mb-4 opacity-20" />
                No se encontraron registros de auditoría.
              </div>
            ) : (
              <div className="grid grid-cols-1 gap-4">
                {filteredLogs.map((log, idx) => (
                  <motion.div
                    key={log.id}
                    initial={{ opacity: 0, x: -10 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: idx * 0.02 }}
                    className="glass border border-border p-4 rounded-2xl hover:bg-white/[0.02] transition-all group"
                  >
                    <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
                      <div className="flex items-start gap-4">
                        <div className="w-10 h-10 rounded-xl bg-indigo-500/10 flex items-center justify-center text-indigo-400 flex-shrink-0 mt-1">
                          <User size={20} />
                        </div>
                        <div className="space-y-1">
                          <div className="flex items-center gap-2 flex-wrap">
                            <span className="font-bold text-foreground">{log.usuarioNombre}</span>
                            <span className={cn(
                              "px-2 py-0.5 rounded text-[10px] font-black uppercase tracking-widest border",
                              getActionColor(log.accion)
                            )}>
                              {log.accion}
                            </span>
                            <span className="text-[10px] text-muted-foreground flex items-center gap-1 font-bold uppercase">
                              <Database size={12} /> {log.entidad}
                            </span>
                          </div>
                          <p className="text-sm text-slate-300">{log.detalles}</p>
                          <div className="flex items-center gap-4 text-[10px] text-muted-foreground font-medium">
                            <span className="flex items-center gap-1">
                              <Calendar size={12} /> {new Date(log.fecha).toLocaleDateString()}
                            </span>
                            <span className="flex items-center gap-1">
                              <Clock size={12} /> {new Date(log.fecha).toLocaleTimeString()}
                            </span>
                            <span className="font-mono opacity-50">ID: {log.entidadId.substring(0, 8)}</span>
                          </div>
                        </div>
                      </div>

                      <div className="flex items-center gap-2 md:opacity-0 group-hover:opacity-100 transition-opacity self-end md:self-center">
                        <button 
                          onClick={() => setSelectedLog(log)}
                          className="flex items-center gap-2 px-3 py-1.5 bg-muted/50 hover:bg-muted/50 text-slate-300 rounded-lg text-xs font-bold transition-all"
                        >
                          <Eye size={14} /> Inspeccionar
                        </button>
                        {log.accion.toLowerCase() === 'eliminar' && (
                          <button 
                            onClick={() => {
                              if (confirm('¿Deseas restaurar este registro eliminado?')) {
                                restaurarMutation.mutate(log)
                              }
                            }}
                            className="flex items-center gap-2 px-3 py-1.5 bg-emerald-500/10 hover:bg-emerald-500/20 text-emerald-400 rounded-lg text-xs font-bold transition-all"
                          >
                            <RotateCcw size={14} /> Restaurar
                          </button>
                        )}
                      </div>
                    </div>
                  </motion.div>
                ))}
              </div>
            )}
          </div>
        )}
      </div>

      {/* Details Modal */}
      <AnimatePresence>
        {selectedLog && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setSelectedLog(null)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ scale: 0.9, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} exit={{ scale: 0.9, opacity: 0 }} className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-2xl glass z-[110] shadow-2xl rounded-3xl overflow-hidden">
              <div className="px-6 py-4 border-b border-border bg-muted/50 flex items-center justify-between">
                <h2 className="text-xl font-bold text-foreground flex items-center gap-2">
                  <FileJson className="text-indigo-400" />
                  Detalles Técnicos (JSON)
                </h2>
                <button onClick={() => setSelectedLog(null)} className="p-2 hover:bg-muted/50 rounded-full text-muted-foreground transition-all"><X size={20} /></button>
              </div>
              <div className="p-6 overflow-y-auto max-h-[70vh]">
                <div className="mb-4 flex items-center justify-between p-3 bg-amber-500/10 border border-amber-500/20 rounded-xl">
                  <div className="flex items-center gap-2 text-amber-400">
                    <AlertTriangle size={18} />
                    <span className="text-xs font-bold uppercase">Estado Previo al Cambio</span>
                  </div>
                </div>
                <pre className="p-4 bg-muted/50/50 rounded-2xl border border-border text-indigo-300 font-mono text-xs overflow-x-auto">
                  {JSON.stringify(JSON.parse(selectedLog.detallesJSON || '{}'), null, 2)}
                </pre>
              </div>
              <div className="px-6 py-4 border-t border-border bg-muted/50 flex justify-end">
                <button 
                  onClick={() => setSelectedLog(null)}
                  className="px-6 py-2 bg-indigo-600 hover:bg-indigo-700 text-white font-bold rounded-xl transition-all"
                >
                  Cerrar
                </button>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}



