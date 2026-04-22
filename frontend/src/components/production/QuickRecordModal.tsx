'use client'

import { useState, useEffect, useMemo } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, AlertCircle, Scale, Droplets, Ruler, Calendar, ChevronDown, Beaker } from 'lucide-react'
import { toast } from 'sonner'
import { format } from 'date-fns'
import { useCatalogos, ProductoCatalogo } from '@/hooks/useCatalogos'
import { useFormulas } from '@/hooks/useFormulas'
import { cn } from '@/lib/utils'

interface QuickRecordProps {
  isOpen: boolean
  onClose: () => void
  loteId: string
  type: 'mortality' | 'feed' | 'water' | 'weight'
  lote?: any
  initialData?: any // Datos para edición
}

export function QuickRecordModal({ isOpen, onClose, loteId, type, lote, initialData }: QuickRecordProps) {
  const [recordMode, setRecordMode] = useState<'individual' | 'formula'>('individual')
  const [value, setValue] = useState('')
  const [secondaryValue, setSecondaryValue] = useState('') 
  const [tertiaryValue, setTertiaryValue] = useState('') 
  const [quaternaryValue, setQuaternaryValue] = useState('') 
  const [nota, setNota] = useState('')
  const [fecha, setFecha] = useState(format(new Date(), 'yyyy-MM-dd'))
  const [selectedProductId, setSelectedProductId] = useState('')
  const [selectedFormulaId, setSelectedFormulaId] = useState('')
  const [stockActual, setStockActual] = useState<number | null>(null)
  
  const queryClient = useQueryClient()
  const { productos } = useCatalogos()
  const { formulas, registrarConsumo } = useFormulas()
  const isEditing = !!initialData

  // Obtener stock al seleccionar producto
  useEffect(() => {
    if (selectedProductId && type === 'feed' && recordMode === 'individual') {
      api.get<any>(`/api/Inventario/productos/${selectedProductId}/stock`)
        .then(res => setStockActual(res.cantidad))
        .catch(() => setStockActual(null))
    } else {
      setStockActual(null)
    }
  }, [selectedProductId, type, recordMode])

  // Filtrar productos de alimento
  const alimentos = useMemo(() => 
    productos.filter(p => p.categoriaNombre === 'Alimento' && p.isActive),
    [productos]
  )

  // Filtrar fórmulas activas
  const formulasActivas = useMemo(() => 
    formulas.filter(f => f.isActive),
    [formulas]
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

      // Para alimento, el valor 'cantidad' viene en unidades (sacos/bolsas)
      // y 'cantidadKg' viene en kilogramos totales.
      if (type === 'feed' && initialData.productoId) {
        setSelectedProductId(initialData.productoId)
        const units = initialData.cantidad;
        const totalKg = initialData.cantidadKg;
        
        setValue(units?.toString() || '')
        setSecondaryValue(totalKg?.toString() || '')
        setRecordMode('individual')
      } else {
        setValue(initialData[config.main]?.toString() || '')
      }

      if (config.note) setNota(initialData[config.note] || '')
      setFecha(format(new Date(initialData.fecha), 'yyyy-MM-dd'))
      
      if (config.sec) setSecondaryValue(initialData[config.sec]?.toString() || '')
      if (config.ter) setTertiaryValue(initialData[config.ter]?.toString() || '')
      if (type === 'water') setQuaternaryValue(initialData.lecturaMedidor?.toString() || '')
      if (config.prod && !selectedProductId) setSelectedProductId(initialData[config.prod] || '')
    } else if (!isOpen) {
      // Limpiar al cerrar
      setValue('')
      setSecondaryValue('')
      setTertiaryValue('')
      setNota('')
      setFecha(format(new Date(), 'yyyy-MM-dd'))
      setSelectedProductId('')
      setSelectedFormulaId('')
      setStockActual(null)
      setRecordMode('individual')
    }
  }, [isEditing, initialData, isOpen, type, productos])

  const mutation = useMutation({
    mutationFn: ({ data, idempotencyKey }: { data: any, idempotencyKey: string }) => {
      if (type === 'feed' && recordMode === 'formula') {
        return registrarConsumo.mutateAsync({ data, idempotencyKey })
      }

      let endpoint = '/api/Mortalidad'
      if (type === 'feed') endpoint = '/api/Inventario/consumo-diario'
      if (type === 'water') endpoint = '/api/Sanidad/bienestar'
      if (type === 'weight') endpoint = '/api/Pesajes'
      
      if (isEditing) {
          if (type === 'mortality' || type === 'weight') {
              return api.put(`${endpoint}/${initialData.id}`, data, idempotencyKey)
          }
          // Para alimento y agua, si no hay PUT explícito, el POST puede actuar como upsert
          // o necesitaremos implementar el PUT en el backend.
          return api.post(endpoint, data, idempotencyKey)
      }
      
      return api.post(endpoint, data, idempotencyKey)
    },
    onSuccess: () => {
      const keysToInvalidate = [
        ['lote', loteId],
        ['lote-tendencias', loteId],
        ['pesajes', 'lote', loteId],
        ['mortalidad', 'lote', loteId],
        ['sanidad', 'bienestar', 'lote', loteId],
        ['inventario', 'lote', loteId, 'movimientos'],
        ['catalogos', 'productos'],
        ['formulas']
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
    
    // Validar stock antes de enviar (Se valida en unidades)
    if (type === 'feed' && recordMode === 'individual' && stockActual !== null && Number(value) > stockActual) {
        toast.error(`Stock insuficiente. Solo quedan ${stockActual} unidades.`)
        return
    }

    const idempotencyKey = crypto.randomUUID()

    if (type === 'feed' && recordMode === 'formula') {
      if (!selectedFormulaId) {
        toast.error('Debe seleccionar una fórmula.')
        return
      }
      mutation.mutate({
        data: {
          loteId,
          formulaId: selectedFormulaId,
          cantidadTotalPreparada: Number(value),
          fecha: new Date(fecha).toISOString(),
          justificacion: nota
        },
        idempotencyKey
      })
      return
    }

    let data: any = { loteId, fecha: new Date(fecha).toISOString() }

    if (isEditing) {
      data.id = initialData.id
      if (type === 'mortality') {
        data.version = initialData.version
      }
    }

    if (type === 'mortality') {
      data = { ...data, cantidad: Number(value), causa: nota || 'Registro rutinario' }
    } else if (type === 'feed') {
      if (!selectedProductId) {
          toast.error('Debe seleccionar un producto de alimento.')
          return
      }
      
      // Enviar el valor en Kilogramos (secundaryValue) si existe, de lo contrario calcularlo
      const pesoUnit = selectedProduct ? (Number(selectedProduct.pesoUnitarioKg) || 0) : 0
      const cantidadKg = Number(secondaryValue) || (Number(value) * pesoUnit)

      if (cantidadKg <= 0) {
        toast.error('Debe ingresar una cantidad válida.')
        return
      }

      data = { 
          ...data,
          productoId: selectedProductId,
          cantidad: cantidadKg, // Enviamos Kg exactos
          justificacion: nota || 'Consumo diario' 
      }
    } else if (type === 'water') {
      data = { 
          ...data,
          consumoAgua: Number(value),
          temperatura: Number(secondaryValue) || 0,
          humedad: Number(tertiaryValue) || 0,
          lecturaMedidor: Number(quaternaryValue) || 0,
          observaciones: nota 
      }
    } else if (type === 'weight') {
      data = {
        ...data,
        pesoPromedioGramos: Number(value),
        cantidadMuestreada: Number(secondaryValue) || 10
      }
    }

    mutation.mutate({ data, idempotencyKey })
  }

  const config = {
    mortality: { title: 'Bajas', icon: AlertCircle, color: 'text-red-500', bg: 'bg-red-500/10', label: 'Cantidad de aves', unit: 'und' },
    feed: { title: 'Alimento', icon: Scale, color: 'text-blue-500', bg: 'bg-blue-500/10', label: 'Unidades Abiertas', unit: 'unidades' },
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
              {type === 'feed' && !isEditing && (
                <div className="flex p-1 bg-muted/50 rounded-2xl border border-border">
                  <button
                    type="button"
                    onClick={() => setRecordMode('individual')}
                    className={cn(
                      "flex-1 flex items-center justify-center gap-2 py-2 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all",
                      recordMode === 'individual' ? "bg-blue-500 text-white shadow-lg" : "text-muted-foreground"
                    )}
                  >
                    <Scale size={14} /> Insumo Único
                  </button>
                  <button
                    type="button"
                    onClick={() => setRecordMode('formula')}
                    className={cn(
                      "flex-1 flex items-center justify-center gap-2 py-2 rounded-xl text-[10px] font-black uppercase tracking-widest transition-all",
                      recordMode === 'formula' ? "bg-purple-500 text-white shadow-lg" : "text-muted-foreground"
                    )}
                  >
                    <Beaker size={14} /> Por Fórmula
                  </button>
                </div>
              )}

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

                {/* Selector de Producto o Fórmula */}
                {type === 'feed' && (
                  <div className="space-y-2">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1 flex items-center gap-2">
                      {recordMode === 'individual' ? 'Producto de Alimento' : 'Seleccionar Fórmula'}
                    </label>
                    <div className="relative">
                      {recordMode === 'individual' ? (
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
                      ) : (
                        <select
                          value={selectedFormulaId}
                          onChange={(e) => setSelectedFormulaId(e.target.value)}
                          className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-lg font-bold text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all appearance-none"
                        >
                          <option value="">Seleccionar Receta...</option>
                          {formulasActivas.map(f => (
                            <option key={f.id} value={f.id}>{f.nombre} ({f.etapa})</option>
                          ))}
                        </select>
                      )}
                      <ChevronDown size={20} className="absolute right-4 top-1/2 -translate-y-1/2 text-muted-foreground pointer-events-none" />
                    </div>
                    {recordMode === 'individual' && selectedProduct && stockActual !== null && (
                        <p className={`text-[10px] font-black uppercase tracking-widest ml-1 mt-1 ${stockActual <= 0 ? 'text-red-500' : 'text-emerald-500'}`}>
                           Stock Disponible: {stockActual} {selectedProduct.unidadMedidaNombre}(s) ≈ {(Number(stockActual) * Number(selectedProduct.pesoUnitarioKg)).toFixed(1)} Kg
                        </p>
                    )}
                  </div>
                )}
              </div>

              <div className={cn(
                "grid grid-cols-1 gap-4",
                type === 'feed' && recordMode === 'individual' ? "md:grid-cols-2" : "md:grid-cols-1"
              )}>
                <div className="space-y-2">
                  <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1 text-center block w-full">
                    {type === 'feed' 
                      ? (recordMode === 'individual' ? 'Cantidad de Unidades (Sacos/Bolsas)' : 'Cantidad Total de Mezcla Preparada') 
                      : config.label}
                  </label>
                  <div className="relative">
                    <input
                      autoFocus={type !== 'feed'}
                      type="number"
                      step="any"
                      value={value}
                      onChange={(e) => {
                        const val = e.target.value;
                        setValue(val);
                        // Calcular Kg automáticamente si hay producto seleccionado y modo individual
                        if (type === 'feed' && recordMode === 'individual' && selectedProduct) {
                          const pesoUnit = Number(selectedProduct.pesoUnitarioKg) || 0;
                          const calculatedKg = val ? (Number(val) * pesoUnit).toFixed(3) : '';
                          setSecondaryValue(calculatedKg);
                        }
                      }}
                      className={cn(
                        "w-full px-6 py-6 bg-muted/50 border border-border rounded-3xl text-4xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all",
                        recordMode === 'formula' ? "border-purple-500/30" : ""
                      )}
                      placeholder="0"
                    />
                    <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-xl uppercase">
                        {type === 'feed' ? (recordMode === 'individual' ? 'UNID' : 'Kg') : config.unit}
                    </span>
                  </div>
                </div>

                {type === 'feed' && recordMode === 'individual' && (
                  <div className="space-y-2 transition-all">
                    <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Equivalencia en Kilogramos (Kg)</label>
                    <div className="relative">
                      <input
                        type="number"
                        step="any"
                        value={secondaryValue}
                        onChange={(e) => {
                          const valKg = e.target.value;
                          setSecondaryValue(valKg);
                          // Recalcular unidades si hay producto
                          if (selectedProduct) {
                             const pesoUnit = Number(selectedProduct.pesoUnitarioKg) || 1;
                             const calculatedUnits = valKg ? (Number(valKg) / pesoUnit).toFixed(3) : '';
                             setValue(calculatedUnits);
                          }
                        }}
                        className="w-full px-6 py-6 bg-muted/50 border border-border rounded-3xl text-4xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all border-blue-500/30"
                        placeholder="0.00"
                      />
                      <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-xl uppercase">Kg</span>
                    </div>
                  </div>
                )}
              </div>

              {type === 'feed' && recordMode === 'individual' && selectedProduct && (
                  <div className="bg-primary/5 p-4 rounded-2xl border border-primary/20 space-y-1">
                    <p className="text-[10px] text-primary font-black uppercase tracking-widest text-center">
                      Configuración: 1 {selectedProduct.unidadMedidaNombre} = {selectedProduct.pesoUnitarioKg} kg
                    </p>
                    <p className="text-sm text-muted-foreground font-bold text-center">
                      CONSUMO ESTIMADO: <span className="text-foreground">{value || 0} Unidades</span> × <span className="text-foreground">{selectedProduct.pesoUnitarioKg} Kg</span> = <span className="text-primary font-black text-lg">{(Number(value || 0) * Number(selectedProduct.pesoUnitarioKg)).toFixed(2)} Kg totales</span>
                    </p>
                  </div>
              )}

              {type === 'feed' && recordMode === 'formula' && selectedFormulaId && (
                  <div className="bg-purple-500/5 p-4 rounded-2xl border border-purple-500/20 space-y-2">
                    <p className="text-[10px] text-purple-400 font-black uppercase tracking-widest text-center">
                      Se descontarán proporcionalmente todos los ingredientes de la receta
                    </p>
                    {formulas.find(f => f.id === selectedFormulaId)?.detalles.map((d, i) => (
                      <div key={i} className="flex justify-between text-[10px] font-bold uppercase">
                        <span className="text-muted-foreground">{d.productoNombre}</span>
                        <span className="text-purple-300">
                          {((d.cantidadPorBase * Number(value)) / (formulas.find(f => f.id === selectedFormulaId)?.cantidadBase || 1)).toFixed(3)}
                        </span>
                      </div>
                    ))}
                  </div>
              )}

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

              {type === 'water' && (
                <div className="space-y-2">
                  <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Lectura del Medidor (m³ o L)</label>
                  <div className="relative">
                    <input
                      type="number"
                      step="0.001"
                      value={quaternaryValue}
                      onChange={(e) => setQuaternaryValue(e.target.value)}
                      className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-2xl font-black text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 text-center transition-all border-amber-500/30"
                      placeholder="0.000"
                    />
                    <span className="absolute right-6 top-1/2 -translate-y-1/2 text-muted-foreground font-black text-sm uppercase font-mono">Lectura</span>
                  </div>
                  <p className="text-[10px] text-muted-foreground italic ml-1">Se recomienda registrar la lectura diaria para auditar el consumo real.</p>
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
