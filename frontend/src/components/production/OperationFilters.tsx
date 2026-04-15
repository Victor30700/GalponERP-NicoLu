'use client'

import { Calendar, X } from 'lucide-react'
import { motion } from 'framer-motion'

interface OperationFiltersProps {
  selectedDate: string
  onDateChange: (date: string) => void
  onClear: () => void
}

export function OperationFilters({ selectedDate, onDateChange, onClear }: OperationFiltersProps) {
  return (
    <div className="flex flex-wrap items-center gap-4 mb-6">
      <div className="relative group">
        <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none text-muted-foreground group-focus-within:text-primary transition-colors">
          <Calendar size={18} />
        </div>
        <input
          type="date"
          value={selectedDate}
          onChange={(e) => onDateChange(e.target.value)}
          className="pl-11 pr-4 py-2.5 bg-muted/50 border border-border rounded-xl text-sm font-bold text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
        />
        {selectedDate && (
          <button
            onClick={onClear}
            className="absolute inset-y-0 right-0 pr-3 flex items-center text-muted-foreground hover:text-foreground transition-colors"
          >
            <X size={16} />
          </button>
        )}
      </div>

      <div className="flex items-center gap-2">
         {/* Aquí se podrían añadir filtros por mes/año si fuera necesario, 
             pero el selector de fecha nativo ya es bastante potente. */}
         {selectedDate && (
           <motion.span 
             initial={{ opacity: 0, scale: 0.8 }}
             animate={{ opacity: 1, scale: 1 }}
             className="px-3 py-1 bg-primary/10 text-primary text-[10px] font-black uppercase tracking-widest rounded-full border border-primary/20"
           >
             Filtrando por día
           </motion.span>
         )}
      </div>
    </div>
  )
}
