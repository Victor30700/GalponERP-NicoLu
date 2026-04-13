'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  Package, 
  ArrowLeftRight, 
  ShoppingCart, 
  Plus, 
  Search, 
  Filter, 
  AlertTriangle,
  History,
  TrendingUp,
  TrendingDown,
  Calendar,
  User,
  Info,
  X,
  Save,
  DollarSign
} from 'lucide-react'
import { toast } from 'sonner'
import { cn } from '@/lib/utils'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'

// --- Interfaces ---

interface StockProducto {
  productoId: string
  nombreProducto: string
  tipoProducto: string
  stockActual: number
  stockActualKg: number
  unidadMedida: string
}

interface Movimiento {
  id: string
  productoId: string
  nombreProducto: string
  loteId: string | null
  cantidad: number
  tipo: string
  fecha: string
  justificacion: string | null
}

interface Compra {
  id: string
  proveedorId: string
  proveedorNombre: string
  fecha: string
  total: number
  totalPagado: number
  saldoPendiente: number
  estadoPago: string
  nota: string | null
}

interface Producto {
  id: string
  nombre: string
}

interface Proveedor {
  id: string
  razonSocial: string
}

// --- Schemas ---

const compraSchema = z.object({
  productoId: z.string().uuid('Producto inválido'),
  proveedorId: z.string().uuid('Proveedor inválido'),
  cantidad: z.number().positive('La cantidad debe ser mayor a 0'),
  costoTotalCompra: z.number().min(0, 'El costo no puede ser negativo'),
  montoPagado: z.number().min(0, 'El monto pagado no puede ser negativo'),
  nota: z.string().optional(),
})

type CompraFormValues = z.infer<typeof compraSchema>

const ajusteSchema = z.object({
  productoId: z.string().uuid('Producto inválido'),
  cantidad: z.number().positive('La cantidad debe ser mayor a 0'),
  tipo: z.enum(['Entrada', 'Salida']),
  justificacion: z.string().min(5, 'La justificación es muy corta'),
})

type AjusteFormValues = z.infer<typeof ajusteSchema>

// --- Main Component ---

export default function InventarioPage() {
  const [activeTab, setActiveTab] = useState<'stock' | 'movimientos' | 'compras'>('stock')
  const [isCompraModalOpen, setIsCompraModalOpen] = useState(false)
  const [isAjusteModalOpen, setIsAjusteModalOpen] = useState(false)
  const queryClient = useQueryClient()

  // Queries
  const { data: stock = [], isLoading: isLoadingStock } = useQuery({
    queryKey: ['inventario', 'stock'],
    queryFn: () => api.get<StockProducto[]>('/api/inventario/stock'),
  })

  const { data: movimientos = [], isLoading: isLoadingMovimientos } = useQuery({
    queryKey: ['inventario', 'movimientos'],
    queryFn: () => api.get<Movimiento[]>('/api/inventario/movimientos'),
  })

  const { data: compras = [], isLoading: isLoadingCompras } = useQuery({
    queryKey: ['inventario', 'compras'],
    queryFn: () => api.get<Compra[]>('/api/inventario/compras'),
  })

  const { data: productos = [] } = useQuery({
    queryKey: ['productos'],
    queryFn: () => api.get<Producto[]>('/api/Productos'),
  })

  const { data: proveedores = [] } = useQuery({
    queryKey: ['proveedores'],
    queryFn: () => api.get<Proveedor[]>('/api/Proveedores'),
  })

  // Mutations
  const registrarCompraMutation = useMutation({
    mutationFn: (data: CompraFormValues) => api.post('/api/inventario/compras', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inventario'] })
      toast.success('Compra registrada con éxito')
      setIsCompraModalOpen(false)
    },
    onError: (err: any) => toast.error(err.message),
  })

  const registrarAjusteMutation = useMutation({
    mutationFn: (data: AjusteFormValues) => api.put('/api/inventario/ajuste', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['inventario'] })
      toast.success('Ajuste de inventario registrado')
      setIsAjusteModalOpen(false)
    },
    onError: (err: any) => toast.error(err.message),
  })

  // Forms
  const compraForm = useForm<CompraFormValues>({
    resolver: zodResolver(compraSchema),
    defaultValues: {
      cantidad: 0,
      costoTotalCompra: 0,
      montoPagado: 0
    }
  })

  const ajusteForm = useForm<AjusteFormValues>({
    resolver: zodResolver(ajusteSchema),
    defaultValues: {
      tipo: 'Entrada',
      cantidad: 0
    }
  })

  const onCompraSubmit = (data: CompraFormValues) => {
    registrarCompraMutation.mutate(data)
  }

  const onAjusteSubmit = (data: AjusteFormValues) => {
    registrarAjusteMutation.mutate(data)
  }

  return (
    <div className="space-y-6">
      {/* Tab Switcher */}
      <div className="flex p-1 bg-slate-900/50 border border-white/5 rounded-2xl w-full md:w-fit">
        <button
          onClick={() => setActiveTab('stock')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'stock' ? "bg-blue-500 text-white shadow-lg" : "text-slate-400 hover:text-slate-200"
          )}
        >
          <Package size={18} />
          Stock Actual
        </button>
        <button
          onClick={() => setActiveTab('movimientos')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'movimientos' ? "bg-blue-500 text-white shadow-lg" : "text-slate-400 hover:text-slate-200"
          )}
        >
          <ArrowLeftRight size={18} />
          Movimientos
        </button>
        <button
          onClick={() => setActiveTab('compras')}
          className={cn( activeTab === 'compras' ? "bg-blue-500 text-white shadow-lg" : "text-slate-400 hover:text-slate-200",
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
          )}
        >
          <ShoppingCart size={18} />
          Compras
        </button>
      </div>

      <AnimatePresence mode="wait">
        {activeTab === 'stock' && (
          <motion.div
            key="stock"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
          >
            <UniversalGrid
              title="Stock de Inventario"
              items={stock}
              isLoading={isLoadingStock}
              onAdd={() => setIsAjusteModalOpen(true)}
              searchPlaceholder="Buscar producto..."
              columns={[
                { 
                  header: 'Producto', 
                  accessor: (item) => (
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 rounded-xl bg-blue-500/10 flex items-center justify-center text-blue-400">
                        <Package size={20} />
                      </div>
                      <div>
                        <p className="font-bold text-white">{item.nombreProducto}</p>
                        <p className="text-xs text-slate-500 uppercase tracking-tighter font-semibold">{item.tipoProducto}</p>
                      </div>
                    </div>
                  )
                },
                { 
                  header: 'Stock Actual', 
                  accessor: (item) => (
                    <div className="flex flex-col">
                      <span className={cn("text-lg font-bold", item.stockActual > 0 ? "text-emerald-400" : "text-red-400")}>
                        {item.stockActual.toLocaleString()} {item.unidadMedida}
                      </span>
                      <span className="text-xs text-slate-500">{item.stockActualKg.toLocaleString()} Kg equiv.</span>
                    </div>
                  )
                },
                {
                  header: 'Estado',
                  accessor: (item) => (
                    item.stockActual <= 0 ? (
                      <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-red-500/10 text-red-400 text-[10px] font-bold uppercase border border-red-500/20">
                        <AlertTriangle size={12} /> Agotado
                      </span>
                    ) : item.stockActual < 100 ? (
                      <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-amber-500/10 text-amber-400 text-[10px] font-bold uppercase border border-amber-500/20">
                        <AlertTriangle size={12} /> Stock Bajo
                      </span>
                    ) : (
                      <span className="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-emerald-500/10 text-emerald-400 text-[10px] font-bold uppercase border border-emerald-500/20">
                        Óptimo
                      </span>
                    )
                  )
                }
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-4">
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 rounded-2xl bg-blue-500/10 flex items-center justify-center text-blue-400">
                      <Package size={24} />
                    </div>
                    <div>
                      <h3 className="text-lg font-bold text-white">{item.nombreProducto}</h3>
                      <p className="text-xs text-slate-500 uppercase font-bold tracking-widest">{item.tipoProducto}</p>
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-4 pt-2">
                    <div className="p-3 rounded-xl bg-white/5 border border-white/5">
                      <p className="text-[10px] text-slate-500 uppercase font-bold mb-1">Unidades</p>
                      <p className={cn("text-lg font-black", item.stockActual > 0 ? "text-emerald-400" : "text-red-400")}>
                        {item.stockActual} <span className="text-xs font-normal opacity-60">{item.unidadMedida}</span>
                      </p>
                    </div>
                    <div className="p-3 rounded-xl bg-white/5 border border-white/5">
                      <p className="text-[10px] text-slate-500 uppercase font-bold mb-1">Equivalente</p>
                      <p className="text-lg font-black text-slate-300">
                        {item.stockActualKg} <span className="text-xs font-normal opacity-60">Kg</span>
                      </p>
                    </div>
                  </div>
                </div>
              )}
            />
          </motion.div>
        )}

        {activeTab === 'movimientos' && (
          <motion.div
            key="movimientos"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
          >
            <UniversalGrid
              title="Historial de Movimientos"
              items={movimientos}
              isLoading={isLoadingMovimientos}
              searchPlaceholder="Filtrar movimientos..."
              columns={[
                {
                  header: 'Fecha',
                  accessor: (item) => (
                    <div className="flex flex-col">
                      <span className="text-white font-medium">{new Date(item.fecha).toLocaleDateString()}</span>
                      <span className="text-[10px] text-slate-500">{new Date(item.fecha).toLocaleTimeString()}</span>
                    </div>
                  )
                },
                { header: 'Producto', accessor: 'nombreProducto' },
                {
                  header: 'Tipo',
                  accessor: (item) => {
                    const isEntry = item.tipo.toLowerCase().includes('entrada') || item.tipo.toLowerCase().includes('compra');
                    return (
                      <span className={cn(
                        "inline-flex items-center gap-1 px-2.5 py-1 rounded-lg text-[10px] font-bold uppercase",
                        isEntry ? "bg-emerald-500/10 text-emerald-400 border border-emerald-500/20" : "bg-red-500/10 text-red-400 border border-red-500/20"
                      )}>
                        {isEntry ? <TrendingUp size={12} /> : <TrendingDown size={12} />}
                        {item.tipo}
                      </span>
                    )
                  }
                },
                { 
                  header: 'Cantidad', 
                  accessor: (item) => (
                    <span className="font-mono font-bold text-white">
                      {item.cantidad.toLocaleString()}
                    </span>
                  ) 
                },
                { header: 'Justificación', accessor: 'justificacion', className: 'max-w-[200px] truncate' }
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-3">
                  <div className="flex justify-between items-start">
                    <div className="flex items-center gap-2">
                      <Calendar size={14} className="text-slate-500" />
                      <span className="text-xs font-bold text-slate-400">{new Date(item.fecha).toLocaleString()}</span>
                    </div>
                    <span className={cn(
                        "inline-flex items-center gap-1 px-2 py-0.5 rounded text-[10px] font-black uppercase tracking-widest",
                        item.tipo.toLowerCase().includes('entrada') || item.tipo.toLowerCase().includes('compra') ? "bg-emerald-500/10 text-emerald-400" : "bg-red-500/10 text-red-400"
                      )}>
                        {item.tipo}
                    </span>
                  </div>
                  <h3 className="font-bold text-white text-lg">{item.nombreProducto}</h3>
                  <div className="flex items-center justify-between p-3 bg-white/5 rounded-xl border border-white/5">
                    <div className="text-xs text-slate-500 uppercase font-bold">Cantidad</div>
                    <div className="text-xl font-black text-white">{item.cantidad.toLocaleString()}</div>
                  </div>
                  {item.justificacion && (
                    <p className="text-xs text-slate-400 italic">"{item.justificacion}"</p>
                  )}
                </div>
              )}
            />
          </motion.div>
        )}

        {activeTab === 'compras' && (
          <motion.div
            key="compras"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
          >
            <UniversalGrid
              title="Ordenes de Compra"
              items={compras}
              isLoading={isLoadingCompras}
              onAdd={() => setIsCompraModalOpen(true)}
              columns={[
                { 
                  header: 'Proveedor', 
                  accessor: (item) => (
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 rounded-full bg-amber-500/10 flex items-center justify-center text-amber-500">
                        <User size={18} />
                      </div>
                      <span className="font-bold text-white">{item.proveedorNombre}</span>
                    </div>
                  ) 
                },
                { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
                { 
                  header: 'Total', 
                  accessor: (item) => (
                    <span className="font-mono font-bold text-white">
                      ${item.total.toLocaleString()}
                    </span>
                  )
                },
                {
                  header: 'Saldo',
                  accessor: (item) => (
                    <span className={cn("font-mono font-bold", item.saldoPendiente > 0 ? "text-red-400" : "text-emerald-400")}>
                      ${item.saldoPendiente.toLocaleString()}
                    </span>
                  )
                },
                {
                  header: 'Estado',
                  accessor: (item) => (
                    <span className={cn(
                      "px-2 py-1 rounded text-[10px] font-bold uppercase",
                      item.estadoPago === 'Pagado' ? "bg-emerald-500/10 text-emerald-400" : "bg-amber-500/10 text-amber-400"
                    )}>
                      {item.estadoPago}
                    </span>
                  )
                }
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-4">
                  <div className="flex justify-between items-center">
                    <span className="text-xs font-bold text-slate-500 uppercase">{new Date(item.fecha).toLocaleDateString()}</span>
                    <span className={cn(
                      "px-2 py-0.5 rounded text-[10px] font-black uppercase tracking-widest",
                      item.estadoPago === 'Pagado' ? "bg-emerald-500/10 text-emerald-400" : "bg-amber-500/10 text-amber-400"
                    )}>
                      {item.estadoPago}
                    </span>
                  </div>
                  <h3 className="text-xl font-black text-white">{item.proveedorNombre}</h3>
                  <div className="grid grid-cols-2 gap-3">
                    <div className="p-3 bg-white/5 rounded-xl border border-white/5">
                      <p className="text-[10px] text-slate-500 font-bold uppercase mb-1">Total Compra</p>
                      <p className="text-lg font-black text-white">${item.total.toLocaleString()}</p>
                    </div>
                    <div className="p-3 bg-white/5 rounded-xl border border-white/5">
                      <p className="text-[10px] text-slate-500 font-bold uppercase mb-1">Saldo Pendiente</p>
                      <p className={cn("text-lg font-black", item.saldoPendiente > 0 ? "text-red-400" : "text-emerald-400")}>
                        ${item.saldoPendiente.toLocaleString()}
                      </p>
                    </div>
                  </div>
                </div>
              )}
            />
          </motion.div>
        )}
      </AnimatePresence>

      {/* --- Modales --- */}

      {/* Modal Compra */}
      <AnimatePresence>
        {isCompraModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsCompraModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass-dark z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-white">Registrar Compra</h2>
                <button onClick={() => setIsCompraModalOpen(false)} className="p-2 bg-white/5 rounded-full text-slate-400"><X size={20} /></button>
              </div>

              <form onSubmit={compraForm.handleSubmit(onCompraSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Producto</label>
                  <select {...compraForm.register('productoId')} className="w-full px-4 py-3 bg-slate-900 border border-white/10 rounded-xl text-white appearance-none">
                    <option value="">Seleccionar producto</option>
                    {productos.map(p => <option key={p.id} value={p.id}>{p.nombre}</option>)}
                  </select>
                  {compraForm.formState.errors.productoId && <p className="text-xs text-red-400">{compraForm.formState.errors.productoId.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Proveedor</label>
                  <select {...compraForm.register('proveedorId')} className="w-full px-4 py-3 bg-slate-900 border border-white/10 rounded-xl text-white appearance-none">
                    <option value="">Seleccionar proveedor</option>
                    {proveedores.map(p => <option key={p.id} value={p.id}>{p.razonSocial}</option>)}
                  </select>
                  {compraForm.formState.errors.proveedorId && <p className="text-xs text-red-400">{compraForm.formState.errors.proveedorId.message}</p>}
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-400 ml-1">Cantidad</label>
                    <input type="number" step="0.01" {...compraForm.register('cantidad', { valueAsNumber: true })} className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" />
                    {compraForm.formState.errors.cantidad && <p className="text-xs text-red-400">{compraForm.formState.errors.cantidad.message}</p>}
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-400 ml-1">Costo Total ($)</label>
                    <input type="number" step="0.01" {...compraForm.register('costoTotalCompra', { valueAsNumber: true })} className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" />
                    {compraForm.formState.errors.costoTotalCompra && <p className="text-xs text-red-400">{compraForm.formState.errors.costoTotalCompra.message}</p>}
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Monto Pagado Hoy ($)</label>
                  <div className="relative">
                    <DollarSign className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input type="number" step="0.01" {...compraForm.register('montoPagado', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" />
                  </div>
                  {compraForm.formState.errors.montoPagado && <p className="text-xs text-red-400">{compraForm.formState.errors.montoPagado.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Nota (Opcional)</label>
                  <textarea {...compraForm.register('nota')} rows={2} className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" placeholder="Ej. Factura #1234..." />
                </div>

                <button type="submit" disabled={registrarCompraMutation.isPending} className="w-full py-4 bg-amber-500 hover:bg-amber-600 text-black font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4">
                  <Save size={20} /> Registrar Compra
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>

      {/* Modal Ajuste */}
      <AnimatePresence>
        {isAjusteModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsAjusteModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass-dark z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-white">Ajuste de Stock</h2>
                <button onClick={() => setIsAjusteModalOpen(false)} className="p-2 bg-white/5 rounded-full text-slate-400"><X size={20} /></button>
              </div>

              <form onSubmit={ajusteForm.handleSubmit(onAjusteSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Producto</label>
                  <select {...ajusteForm.register('productoId')} className="w-full px-4 py-3 bg-slate-900 border border-white/10 rounded-xl text-white appearance-none">
                    <option value="">Seleccionar producto</option>
                    {productos.map(p => <option key={p.id} value={p.id}>{p.nombre}</option>)}
                  </select>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-400 ml-1">Tipo Ajuste</label>
                    <select {...ajusteForm.register('tipo')} className="w-full px-4 py-3 bg-slate-900 border border-white/10 rounded-xl text-white appearance-none">
                      <option value="Entrada">Entrada (+)</option>
                      <option value="Salida">Salida (-)</option>
                    </select>
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-400 ml-1">Cantidad</label>
                    <input type="number" step="0.01" {...ajusteForm.register('cantidad', { valueAsNumber: true })} className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Justificación</label>
                  <textarea {...ajusteForm.register('justificacion')} rows={3} className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" placeholder="¿Por qué se realiza este ajuste?" />
                  {ajusteForm.formState.errors.justificacion && <p className="text-xs text-red-400">{ajusteForm.formState.errors.justificacion.message}</p>}
                </div>

                <button type="submit" disabled={registrarAjusteMutation.isPending} className="w-full py-4 bg-blue-500 hover:bg-blue-600 text-white font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4">
                  <Save size={20} /> Guardar Ajuste
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>

    </div>
  )
}
