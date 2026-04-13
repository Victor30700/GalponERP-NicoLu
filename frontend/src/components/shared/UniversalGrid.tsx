'use client'

import { ReactNode } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { Search, Plus, Filter, MoreVertical, Edit2, Trash2, ChevronRight } from 'lucide-react'
import { cn } from '@/lib/utils'

interface Column<T> {
  header: string
  accessor: keyof T | ((item: T) => ReactNode)
  className?: string
}

interface UniversalGridProps<T> {
  items: T[]
  columns: Column<T>[]
  title: string
  onAdd?: () => void
  onEdit?: (item: T) => void
  onDelete?: (item: T) => void
  isLoading?: boolean
  searchPlaceholder?: string
  renderMobileCard: (item: T) => ReactNode
}

export function UniversalGrid<T extends { id: string | number }>({
  items,
  columns,
  title,
  onAdd,
  onEdit,
  onDelete,
  isLoading,
  searchPlaceholder = "Buscar...",
  renderMobileCard,
}: UniversalGridProps<T>) {
  return (
    <div className="space-y-6">
      {/* Header Actions */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-white">{title}</h1>
          <p className="text-slate-400 text-sm">Gestiona tus {title.toLowerCase()} de forma eficiente.</p>
        </div>
        
        <div className="flex items-center gap-2">
          <div className="relative flex-1 md:w-64">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
            <input
              type="text"
              placeholder={searchPlaceholder}
              className="w-full pl-10 pr-4 py-2 bg-slate-900/50 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
            />
          </div>
          
          <button className="p-2 bg-slate-900/50 border border-white/10 rounded-xl text-slate-400 hover:text-white transition-all">
            <Filter size={20} />
          </button>

          {onAdd && (
            <button
              onClick={onAdd}
              className="flex items-center gap-2 px-4 py-2 bg-primary hover:bg-primary/90 text-primary-foreground font-semibold rounded-xl transition-all"
            >
              <Plus size={20} />
              <span className="hidden sm:inline">Nuevo</span>
            </button>
          )}
        </div>
      </div>

      {/* Content */}
      <div className="relative min-h-[400px]">
        {isLoading ? (
          <div className="absolute inset-0 flex items-center justify-center">
            <div className="w-10 h-10 border-4 border-primary/20 border-t-primary rounded-full animate-spin" />
          </div>
        ) : (
          <>
            {/* Desktop Table */}
            <div className="hidden md:block overflow-hidden glass-dark rounded-2xl border border-white/5">
              <table className="w-full text-left border-collapse">
                <thead>
                  <tr className="border-b border-white/5 bg-white/5">
                    {columns.map((col, idx) => (
                      <th key={idx} className={cn("px-6 py-4 text-xs font-semibold text-slate-400 uppercase tracking-wider", col.className)}>
                        {col.header}
                      </th>
                    ))}
                    {(onEdit || onDelete) && <th className="px-6 py-4 text-right">Acciones</th>}
                  </tr>
                </thead>
                <tbody className="divide-y divide-white/5">
                  <AnimatePresence mode="popLayout">
                    {items.map((item, idx) => (
                      <motion.tr
                        key={item.id}
                        initial={{ opacity: 0, y: 10 }}
                        animate={{ opacity: 1, y: 0 }}
                        exit={{ opacity: 0, scale: 0.95 }}
                        transition={{ delay: idx * 0.03 }}
                        className="hover:bg-white/[0.02] transition-colors group"
                      >
                        {columns.map((col, cIdx) => (
                          <td key={cIdx} className={cn("px-6 py-4 text-sm text-slate-300", col.className)}>
                            {typeof col.accessor === 'function' 
                              ? col.accessor(item) 
                              : (item[col.accessor] as ReactNode)}
                          </td>
                        ))}
                        {(onEdit || onDelete) && (
                          <td className="px-6 py-4 text-right">
                            <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                              {onEdit && (
                                <button 
                                  onClick={() => onEdit(item)}
                                  className="p-2 text-slate-400 hover:text-primary hover:bg-primary/10 rounded-lg transition-all"
                                >
                                  <Edit2 size={16} />
                                </button>
                              )}
                              {onDelete && (
                                <button 
                                  onClick={() => onDelete(item)}
                                  className="p-2 text-slate-400 hover:text-red-400 hover:bg-red-400/10 rounded-lg transition-all"
                                >
                                  <Trash2 size={16} />
                                </button>
                              )}
                            </div>
                          </td>
                        )}
                      </motion.tr>
                    ))}
                  </AnimatePresence>
                </tbody>
              </table>
              {items.length === 0 && (
                <div className="py-20 text-center text-slate-500">
                  No se encontraron resultados.
                </div>
              )}
            </div>

            {/* Mobile Cards */}
            <div className="md:hidden space-y-4">
              <AnimatePresence mode="popLayout">
                {items.map((item, idx) => (
                  <motion.div
                    key={item.id}
                    initial={{ opacity: 0, scale: 0.95 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.9 }}
                    transition={{ delay: idx * 0.05 }}
                    className="glass-dark rounded-2xl border border-white/5 p-4 relative overflow-hidden"
                  >
                    <div className="flex justify-between items-start mb-4">
                      <div className="flex-1">
                        {renderMobileCard(item)}
                      </div>
                      <div className="flex flex-col gap-2">
                        {onEdit && (
                          <button 
                            onClick={() => onEdit(item)}
                            className="p-3 bg-white/5 rounded-xl text-slate-400 active:text-primary active:bg-primary/10"
                          >
                            <Edit2 size={18} />
                          </button>
                        )}
                        {onDelete && (
                          <button 
                            onClick={() => onDelete(item)}
                            className="p-3 bg-white/5 rounded-xl text-slate-400 active:text-red-400 active:bg-red-400/10"
                          >
                            <Trash2 size={18} />
                          </button>
                        )}
                      </div>
                    </div>
                    <div className="flex items-center justify-between pt-4 border-t border-white/5 text-xs font-medium text-slate-500 uppercase tracking-widest">
                      <span>ID: {String(item.id).slice(0, 8)}</span>
                      <ChevronRight size={14} className="text-slate-600" />
                    </div>
                  </motion.div>
                ))}
              </AnimatePresence>
              {items.length === 0 && (
                <div className="py-20 text-center text-slate-500">
                  No se encontraron resultados.
                </div>
              )}
            </div>
          </>
        )}
      </div>
    </div>
  )
}
