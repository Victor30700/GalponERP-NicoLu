'use client'

import { useState, useEffect, useMemo } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, AlertCircle, Scale, Droplets, Ruler, Calendar, ChevronDown } from 'lucide-react'
import { toast } from 'sonner'
import { format } from 'date-fns'
import { useCatalogos, ProductoCatalogo } from '@/hooks/useCatalogos'

interface QuickRecordProps {
  isOpen: boolean
  onClose: () => void
  loteId: string
  type: 'mortality' | 'feed' | 'water' | 'weight'
  lote?: any
  initialData?: any // Datos para edición
}

export function QuickRecordModal({ isOpen, onClose, loteId, type, lote, initialData }: QuickRecordProps) {
  const [value, setValue] = useState('')
  const [secondaryValue, setSecondaryValue] = useState('') 
  const [tertiaryValue, setTertiaryValue] = useState('') 
  const [nota, setNota] = useState('')
  const [fecha, setFecha] = useState(format(new Date(), 'yyyy-MM-dd'))
  const [selectedProductId, setSelectedProductId] = useState('')
  
  const queryClient = useQueryClient()
  const { productos } = useCatalogos()
  const isEditing = !!initialData

  // Filtrar productos de alimento
  const alimentos = useMemo(() => 
    productos.filter(p => p.categoriaNombre === 'Alimento' && p.isActive),
    [productos]
  )

  // Producto seleccionado actualmente
  const selectedProduct = useMemo(() => 
    alimentos.find(p => p.id === selectedProductId),
    [alimentos, selectedProductId]
  )

  // Cargar datos si es edición
  useEffect(() => {
    if (isEditing && isOpen) {
      const config = {
        mortality: { main: 'cantidadBajas', note: 'causa' },
        feed: { main: 'cantidad', note: 'justificacion', prod: 'productoId' },
        water: { main: 'consumoAgua', note: 'observaciones', sec: 'temperatura', ter: 'humedad' },
        weight: { main: 'pesoPromedioGramos', sec: 'cantidadMuestreada' }
      }[type]

      // Para alimento, si estamos editando, el valor que viene es Kg. 
      // Pero el usuario ahora ingresa Unidades.
      if (type === 'feed' && initialData.productoId) {
        setSelectedProductId(initialData.productoId)
        const prod = productos.find(p => p.id === initialData.productoId)
        if (prod && prod.equivalenciaEnKg > 0) {
           setValue((initialData[config.main] / prod.equivalenciaEnKg).toString())
        } else {
           setValue(initialData[config.main]?.toString() || '')
        }
      } else {
        setValue(initialData[config.main]?.toString() || '')
      }

      setNota(initialData[config.note] || '')
      setFecha(format(new Date(initialData.fecha), 'yyyy-MM-dd'))
      
      if (config.sec) setSecondaryValue(initialData[config.sec]?.toString() || '')
      if (config.ter) setTertiaryValue(initialData[config.ter]?.toString() || '')
      if (config.prod && !selectedProductId) setSelectedProductId(initialData[config.prod] || '')
    } else if (!isOpen) {
      // Limpiar al cerrar
      setValue('')
      setSecondaryValue('')
      setTertiaryValue('')
      setNota('')
      setFecha(format(new Date(), 'yyyy-MM-dd'))
      setSelectedProductId('')
    }
  }, [isEditing, initialData, isOpen, type, productos])

  const mutation = useMutation({
    mutationFn: (data: any) => {
      let endpoint = '/api/Mortalidad'
      if (type === 'feed') endpoint = '/api/Inventario/consumo-diario'
      if (type === 'water') endpoint = '/api/Sanidad/bienestar'
      if (type === 'weight') endpoint = '/api/Pesajes'
      
      if (isEditing) {
          if (type === 'mortality' || type === 'weight') {
              return api.put(`${endpoint}/${initialData.id}`, data)
          }
          // Para alimento y agua, si no hay PUT explícito, el POST puede actuar como upsert
          // o necesitaremos implementar el PUT en el backend.
          return api.post(endpoint, data)
      }
      
      return api.post(endpoint, data)
    },
    onSuccess: () => {
      const keysToInvalidate = [
        ['lote', loteId],
        ['lote-tendencias', loteId],
        ['pesajes', 'lote', loteId],
        ['mortalidad', 'lote', loteId],
        ['sanidad', 'bienestar', 'lote', loteId],
        ['inventario', 'lote', loteId, 'movimientos']
      ]
      
      keysToInvalidate.forEach(key => queryClient.invalidateQueries({ queryKey: key }))
      
      toast.success(isEditing ? 'Registro actualizado' : 'Registro guardado')
      onClose()
    },
    onError: (err: any) => toast.error(err.message),
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (!value) return
    
    let data: any = { loteId, fecha: new Date(fecha).toISOString() }

    if (isEditing) data.id = initialData.id

    if (type === 'mortality') {
      data = { ...data, cantidad: Number(value), causa: nota || 'Registro rutinario' }
    } else if (type === 'feed') {
      if (!selectedProductId) {
          toast.error('Debe seleccionar un producto de alimento.')
          return
      }
      
      // Calcular cantidad total en Kg
      const equivalencia = selectedProduct?.equivalenciaEnKg || 1
      const cantidadCalculada = Number(value) * equivalencia

      data = { 
          ...data,
          productoId: selectedProductId,
          cantidad: cantidadCalculada, 
          justificacion: nota || 'Consumo diario' 
      }
    } else if (type === 'water') {
      data = { 
          ...data,
          consumoAgua: Number(value),
          temperatura: Number(secondaryValue) || 0,
          humedad: Number(tertiaryValue) || 0,
          observaciones: nota 
      }
    } else if (type === 'weight') {
      data = {
        ...data,
        pesoPromedioGramos: Number(value),
        cantidadMuestreada: Number(secondaryValue) || 10
      }
    }

    mutation.mutate(data)
  }

  const config = {
    mortality: { title: 'Bajas', icon: AlertCircle, color: 'text-red-500', bg: 'bg-red-500/10', label: 'Cantidad de aves', unit: 'und' },
    feed: { title: 'Alimento', icon: Scale, color: 'text-blue-500', bg: 'bg-blue-500/10', label: 'Unidades servidas', unit: 'unidades' },
    water: { title: 'Agua', icon: Droplets, color: 'text-amber-500', bg: 'bg-amber-500/10', label: 'Litros consumidos', unit: 'L' },
    weight: { title: 'Pesaje', icon: Ruler, color: 'text-indigo-500', bg: 'bg-indigo-500/10', label: 'Peso Promedio', unit: 'g' },
  }[type]

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[100]" />
          <motion.div initial={{ y: '100%' }} animate={{ y: 0 }} exit={{ y: '100%' }} transition={{ type: 'spring', damping: 25, stiffness: 200 }} className="fixed bottom-0 left-0 right-0 glass z-[110] p-8 rounded-t-[3rem] border-t border-border max-w-2xl mx-auto shadow-2xl" >
            <div className="flex items-center justify-between mb-8">
              <div className="flex items-center gap-4">
                <div className={`p-3 rounded-2xl ${config.bg} ${config.color}`}>
                  <config.icon size={24} />
                </div>
                <div>
                  <h2 className="text-2xl font-black text-foreground">{isEditing ? 'Editar' : 'Registrar'} {config.title}</h2>
                  <p className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Lote: {lote?.nombre || lote?.nombreLote || 'Cargando...'}</p>
                </div>
              </div>
              <button onClick={onClose} className="p-2 bg-muted/50 rounded-full text-muted-foreground hover:bg-muted transition-colors"><X size={24} /></button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {/* Selector de Fecha */}
                <div className="space-y-2">
                  <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1 flex items-center gap-2">
                    <Calendar size={12} /> Fecha del Registro
                  </label>
                  <input
                    type="date"
                    value={fecha}
                    onChange={(e) => setFecha(e.target.value)}
                    className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-lg font-bold text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                  />
                </div>

                {/* Selector de Producto para Alimento */}
                {type === 'feed' && (
                  <div className="space-y-2">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1 flex items-center gap-2">
                      Producto de Alimento
                    </label>
                    <div className="relative">
                      <select
                        value={selectedProductId}
                        onChange={(e) => setSelectedProductId(e.target.value)}
                        className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-lg font-bold text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all appearance-none"
                      >
                        <option value="">Seleccionar...</option>
                        {alimentos.map(p => (
                          <option key={p.id} value={p.id}>{p.nombre}</option>
                        ))}
                      </select>
                      <ChevronDown size={20} className="absolute right-4 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none" />
                    </div>
                  </div>
                )}
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">{config.label}</label>
                <div className="relative">
                  <input
                    autoFocus
                    type="number"
                    step="any"
                    value={value}
                    onChange={(e) => setValue(e.target.value)}
                    className="w-full px-6 py-6 bg-muted/50 border border-border rounded-3xl text-4xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                    placeholder="0"
                  />
                  <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-xl uppercase">{config.unit}</span>
                </div>
                {type === 'feed' && selectedProduct && (
                  <p className="text-[10px] text-primary font-black uppercase tracking-widest text-center mt-2">
                    Equivalencia: {selectedProduct.equivalenciaEnKg} kg por unidad
                    {value && ` • Total: ${Number(value) * selectedProduct.equivalenciaEnKg} kg`}
                  </p>
                )}
              </div>

              {type === 'weight' && (
                <div className="space-y-2">
                  <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Cantidad Muestreada</label>
                  <div className="relative">
                    <input
                      type="number"
                      value={secondaryValue}
                      onChange={(e) => setSecondaryValue(e.target.value)}
                      className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-2xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                      placeholder="10"
                    />
                    <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-sm uppercase">aves</span>
                  </div>
                </div>
              )}

              {type === 'water' && (
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Temperatura</label>
                    <div className="relative">
                      <input
                        type="number"
                        step="0.1"
                        value={secondaryValue}
                        onChange={(e) => setSecondaryValue(e.target.value)}
                        className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-2xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                        placeholder="0.0"
                      />
                      <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-sm uppercase">°C</span>
                    </div>
                  </div>
                  <div className="space-y-2">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Humedad</label>
                    <div className="relative">
                      <input
                        type="number"
                        step="0.1"
                        value={tertiaryValue}
                        onChange={(e) => setTertiaryValue(e.target.value)}
                        className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-2xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all"
                        placeholder="0.0"
                      />
                      <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-sm uppercase">%</span>
                    </div>
                  </div>
                </div>
              )}

              <div className="space-y-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Notas / Observaciones</label>
                <textarea
                  value={nota}
                  onChange={(e) => setNota(e.target.value)}
                  rows={2}
                  className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all resize-none"
                  placeholder="Opcional..."
                />
              </div>


              <button
                type="submit"
                disabled={mutation.isPending || !value}
                className="w-full py-5 bg-primary hover:bg-primary/90 text-black font-black rounded-3xl transition-all flex items-center justify-center gap-3 disabled:opacity-50 shadow-xl shadow-primary/20 active:scale-95"
              >
                <Save size={24} />
                {isEditing ? 'ACTUALIZAR REGISTRO' : 'GUARDAR REGISTRO'}
              </button>
            </form>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  )
}
