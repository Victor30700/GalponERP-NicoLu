'use client'

import { Calendar, X, ChevronDown } from 'lucide-react'
import { motion } from 'framer-motion'

interface OperationFiltersProps {
  selectedDate: string
  onDateChange: (date: string) => void
  selectedMonth: number
  onMonthChange: (month: number) => void
  selectedYear: number
  onYearChange: (year: number) => void
  onClear: () => void
}

const MONTHS = [
  'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
  'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
]

const YEARS = Array.from({ length: 5 }, (_, i) => new Date().getFullYear() - i)

export function OperationFilters({ 
  selectedDate, 
  onDateChange, 
  selectedMonth, 
  onMonthChange, 
  selectedYear, 
  onYearChange, 
  onClear 
}: OperationFiltersProps) {
  return (
    <div className="flex flex-wrap items-center gap-3">
      {/* Selector de Día (Opcional) */}
      <div className="relative group">
        <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none text-muted-foreground group-focus-within:text-primary transition-colors">
          <Calendar size={14} />
        </div>
        <input
          type="date"
          value={selectedDate}
          onChange={(e) => onDateChange(e.target.value)}
          className="pl-9 pr-8 py-2 bg-muted/50 border border-border rounded-xl text-[10px] font-black uppercase tracking-widest text-foreground focus:outline-none focus:ring-1 focus:ring-primary/50 transition-all"
        />
        {selectedDate && (
          <button
            onClick={() => onDateChange('')}
            className="absolute inset-y-0 right-0 pr-2 flex items-center text-muted-foreground hover:text-foreground transition-colors"
          >
            <X size={12} />
          </button>
        )}
      </div>

      {/* Selector de Mes */}
      <div className="relative">
        <select
          value={selectedMonth}
          onChange={(e) => onMonthChange(Number(e.target.value))}
          className="pl-4 pr-8 py-2 bg-muted/50 border border-border rounded-xl text-[10px] font-black uppercase tracking-widest text-foreground focus:outline-none focus:ring-1 focus:ring-primary/50 transition-all appearance-none cursor-pointer"
        >
          {MONTHS.map((month, i) => (
            <option key={i} value={i + 1} className="bg-background">{month}</option>
          ))}
        </select>
        <ChevronDown size={12} className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none" />
      </div>

      {/* Selector de Año */}
      <div className="relative">
        <select
          value={selectedYear}
          onChange={(e) => onYearChange(Number(e.target.value))}
          className="pl-4 pr-8 py-2 bg-muted/50 border border-border rounded-xl text-[10px] font-black uppercase tracking-widest text-foreground focus:outline-none focus:ring-1 focus:ring-primary/50 transition-all appearance-none cursor-pointer"
        >
          {YEARS.map((year) => (
            <option key={year} value={year} className="bg-background">{year}</option>
          ))}
        </select>
        <ChevronDown size={12} className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none" />
      </div>

      {(selectedDate || selectedMonth !== new Date().getMonth() + 1 || selectedYear !== new Date().getFullYear()) && (
        <button
          onClick={onClear}
          className="px-3 py-2 text-[10px] font-black text-red-400 uppercase tracking-tighter hover:bg-red-500/10 rounded-xl transition-all"
        >
          Limpiar Filtros
        </button>
      )}
    </div>
  )
}
