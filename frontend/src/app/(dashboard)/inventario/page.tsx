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
  DollarSign,
  BarChart3,
  Scale,
  Clock,
  Trash2,
  CheckCircle2
} from 'lucide-react'
import { toast } from 'sonner'
import { cn } from '@/lib/utils'
import { useForm, useFieldArray } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { 
  useInventario, 
  useProductoDetalle, 
  useCompraPagos,
  type StockProducto,
  type Movimiento,
  type Compra,
  type PagoCompra,
  type KardexItem
} from '@/hooks/useInventario'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/lib/rbac'

// --- Interfaces (Existing ones from hook are imported) ---

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

const pagoSchema = z.object({
  monto: z.number().positive('El monto debe ser mayor a 0'),
  metodoPago: z.number(),
  fechaPago: z.string(),
})

type PagoFormValues = z.infer<typeof pagoSchema>

const conciliacionSchema = z.object({
  items: z.array(z.object({
    productoId: z.string().uuid('Producto inválido'),
    cantidadFisica: z.number().min(0, 'La cantidad no puede ser negativa'),
    nota: z.string().optional(),
  }))
})

type ConciliacionFormValues = z.infer<typeof conciliacionSchema>

// --- Main Component ---

export default function InventarioPage() {
  const { profile } = useAuth()
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null
  const isEmpleado = userRole === UserRole.Empleado

  const [activeTab, setActiveTab] = useState<'stock' | 'movimientos' | 'compras' | 'reportes'>('stock')
  const [isCompraModalOpen, setIsCompraModalOpen] = useState(false)
  const [isAjusteModalOpen, setIsAjusteModalOpen] = useState(false)
  const [isConciliacionModalOpen, setIsConciliacionModalOpen] = useState(false)
  
  const [selectedProductoId, setSelectedProductoId] = useState<string | null>(null)
  const [selectedCompraId, setSelectedCompraId] = useState<string | null>(null)

  const { 
    stock, isLoadingStock, 
    movimientos, isLoadingMovimientos, 
    compras, isLoadingCompras,
    valoracion, isLoadingValoracion,
    proyecciones, isLoadingProyecciones,
    nivelesAlimento, isLoadingNiveles,
    registrarCompra, registrarAjuste, realizarConciliacion, ajustarStock
  } = useInventario()

  const { data: productos = [] } = useQuery({
    queryKey: ['productos'],
    queryFn: () => api.get<Producto[]>('/api/Productos'),
  })

  const { data: proveedores = [] } = useQuery({
    queryKey: ['proveedores'],
    queryFn: () => api.get<Proveedor[]>('/api/Proveedores'),
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

  const conciliacionForm = useForm<ConciliacionFormValues>({
    resolver: zodResolver(conciliacionSchema),
    defaultValues: {
      items: [{ productoId: '', cantidadFisica: 0, nota: '' }]
    }
  })

  const { fields, append, remove } = useFieldArray({
    control: conciliacionForm.control,
    name: "items"
  })

  const onCompraSubmit = (data: CompraFormValues) => {
    registrarCompra.mutate(data, {
      onSuccess: () => {
        toast.success('Compra registrada con éxito')
        setIsCompraModalOpen(false)
        compraForm.reset()
      },
      onError: (err: any) => toast.error(err.message)
    })
  }

  const onAjusteSubmit = (data: AjusteFormValues) => {
    registrarAjuste.mutate({
      ...data,
      tipo: data.tipo === 'Entrada' ? 0 : 1, // Mapeo segun API
      fecha: new Date().toISOString()
    }, {
      onSuccess: () => {
        toast.success('Ajuste de inventario registrado')
        setIsAjusteModalOpen(false)
        ajusteForm.reset()
      },
      onError: (err: any) => toast.error(err.message)
    })
  }

  const onConciliacionSubmit = (data: ConciliacionFormValues) => {
    realizarConciliacion.mutate(data, {
      onSuccess: () => {
        toast.success('Conciliación realizada con éxito')
        setIsConciliacionModalOpen(false)
        conciliacionForm.reset()
      },
      onError: (err: any) => toast.error(err.message)
    })
  }

  return (
    <div className="space-y-6">
      {/* Header with Stats Summary */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="p-4 glass rounded-2xl border border-border flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-blue-500/10 flex items-center justify-center text-blue-400">
            <DollarSign size={24} />
          </div>
          <div>
            <p className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Valorización Total</p>
            <p className="text-xl font-black text-foreground">
              {isLoadingValoracion ? '...' : `Bs. ${(valoracion?.valorTotalEmpresa || 0).toLocaleString()}`}
            </p>
          </div>
        </div>
        <div className="p-4 glass rounded-2xl border border-border flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-amber-500/10 flex items-center justify-center text-amber-400">
            <Scale size={24} />
          </div>
          <div>
            <p className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Stock Alimento</p>
            <p className="text-xl font-black text-foreground">
              {isLoadingNiveles ? '...' : `${(nivelesAlimento?.stockActualAlimento || 0).toLocaleString()} kg`}
            </p>
          </div>
        </div>
        <div className="p-4 glass rounded-2xl border border-border flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-emerald-500/10 flex items-center justify-center text-emerald-400">
            <Clock size={24} />
          </div>
          <div>
            <p className="text-[10px] font-black text-muted-foreground uppercase tracking-widest">Autonomía estimada</p>
            <p className="text-xl font-black text-foreground">
              {isLoadingNiveles ? '...' : `${(nivelesAlimento?.diasRestantes || 0).toFixed(1)} días`}
            </p>
          </div>
        </div>
      </div>

      {/* Tab Switcher */}
      <div className="flex flex-wrap gap-1 p-1 bg-muted/50 border border-border rounded-2xl w-full md:w-fit">
        <button
          onClick={() => setActiveTab('stock')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'stock' ? "bg-blue-500 text-white shadow-lg" : "text-muted-foreground hover:text-slate-200"
          )}
        >
          <Package size={18} />
          Stock Actual
        </button>
        <button
          onClick={() => setActiveTab('movimientos')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'movimientos' ? "bg-blue-500 text-white shadow-lg" : "text-muted-foreground hover:text-slate-200"
          )}
        >
          <ArrowLeftRight size={18} />
          Movimientos
        </button>
        <button
          onClick={() => setActiveTab('compras')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'compras' ? "bg-blue-500 text-white shadow-lg" : "text-muted-foreground hover:text-slate-200"
          )}
        >
          <ShoppingCart size={18} />
          Compras
        </button>
        <button
          onClick={() => setActiveTab('reportes')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'reportes' ? "bg-blue-500 text-white shadow-lg" : "text-muted-foreground hover:text-slate-200"
          )}
        >
          <BarChart3 size={18} />
          Reportes & Valoración
        </button>
      </div>

      <AnimatePresence mode="wait">
        {activeTab === 'stock' && (
          <motion.div key="stock" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }}>
            {!isEmpleado && (
              <div className="flex justify-end mb-4 gap-2">
                  <button 
                      onClick={() => setIsConciliacionModalOpen(true)}
                      className="flex items-center gap-2 px-4 py-2 bg-slate-800 hover:bg-slate-700 text-white rounded-xl text-xs font-bold transition-all border border-border"
                  >
                      <CheckCircle2 size={16} /> Conciliación
                  </button>
                  <button 
                      onClick={() => setIsAjusteModalOpen(true)}
                      className="flex items-center gap-2 px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-xl text-xs font-bold transition-all shadow-lg shadow-blue-500/20"
                  >
                      <Plus size={16} /> Nuevo Ajuste
                  </button>
              </div>
            )}
            <UniversalGrid
              title="Stock de Inventario"
              items={stock}
              isLoading={isLoadingStock}
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
                        <p className="font-bold text-foreground">{item.nombreProducto}</p>
                        <p className="text-xs text-muted-foreground uppercase tracking-tighter font-semibold">{item.tipoProducto}</p>
                      </div>
                    </div>
                  )
                },
                { 
                  header: 'Stock Actual', 
                  accessor: (item) => (
                    <div className="flex flex-col">
                      <span className={cn("text-lg font-bold", item.stockActual > 0 ? "text-emerald-400" : "text-red-400")}>
                        {(item.stockActual ?? 0).toLocaleString()} {item.unidadMedida}
                      </span>
                      {item.stockActualKg > 0 && (
                        <span className="text-xs text-muted-foreground">{(item.stockActualKg ?? 0).toLocaleString()} Kg equiv.</span>
                      )}
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
                },
                {
                    header: 'Acciones',
                    accessor: (item) => (
                      <div className="flex items-center gap-1">
                        {!isEmpleado && (
                          <button 
                              onClick={() => {
                                import('sweetalert2').then((Swal) => {
                                  Swal.default.fire({
                                    title: `Conciliar: ${item.nombreProducto}`,
                                    text: `Stock en sistema: ${item.stockActual} ${item.unidadMedida}. Ingrese el conteo físico real:`,
                                    input: 'number',
                                    inputAttributes: {
                                      step: '0.01',
                                      min: '0'
                                    },
                                    showCancelButton: true,
                                    confirmButtonText: 'Ajustar Stock',
                                    cancelButtonText: 'Cancelar',
                                    confirmButtonColor: '#3b82f6',
                                    background: '#1e293b',
                                    color: '#f8fafc',
                                    inputValidator: (value) => {
                                      if (!value) return 'Debe ingresar una cantidad'
                                    }
                                  }).then((result) => {
                                    if (result.isConfirmed) {
                                      ajustarStock.mutate({
                                        productoId: item.productoId,
                                        cantidadFisica: Number(result.value),
                                        nota: 'Conciliación rápida desde tabla de stock'
                                      }, {
                                        onSuccess: () => toast.success('Inventario ajustado correctamente'),
                                        onError: (err: any) => toast.error(err.message)
                                      })
                                    }
                                  })
                                })
                              }}
                              className="p-2 bg-emerald-500/10 hover:bg-emerald-500/20 rounded-lg text-emerald-500 transition-colors"
                              title="Conciliar Stock Físico"
                          >
                              <CheckCircle2 size={16} />
                          </button>
                        )}
                        <button 
                            onClick={() => setSelectedProductoId(item.productoId)}
                            className="p-2 bg-muted/50 hover:bg-muted/50 rounded-lg text-muted-foreground transition-colors"
                        >
                            <Info size={16} />
                        </button>
                      </div>
                    )
                }
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-4">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <div className="w-12 h-12 rounded-2xl bg-blue-500/10 flex items-center justify-center text-blue-400">
                        <Package size={24} />
                      </div>
                      <div>
                        <h3 className="text-lg font-bold text-foreground">{item.nombreProducto}</h3>
                        <p className="text-xs text-muted-foreground uppercase font-bold tracking-widest">{item.tipoProducto}</p>
                      </div>
                    </div>
                    <button 
                        onClick={() => setSelectedProductoId(item.productoId)}
                        className="p-3 bg-muted/50 rounded-xl text-blue-400"
                    >
                        <Info size={20} />
                    </button>
                  </div>
                  <div className="grid grid-cols-2 gap-4 pt-2">
                    <div className="p-3 rounded-xl bg-muted/50 border border-border">
                      <p className="text-[10px] text-muted-foreground uppercase font-bold mb-1">Unidades</p>
                      <p className={cn("text-lg font-black", item.stockActual > 0 ? "text-emerald-400" : "text-red-400")}>
                        {item.stockActual} <span className="text-xs font-normal opacity-60">{item.unidadMedida}</span>
                      </p>
                    </div>
                    {item.stockActualKg > 0 && (
                      <div className="p-3 rounded-xl bg-muted/50 border border-border">
                        <p className="text-[10px] text-muted-foreground uppercase font-bold mb-1">Equivalente</p>
                        <p className="text-lg font-black text-slate-300">
                          {item.stockActualKg} <span className="text-xs font-normal opacity-60">Kg</span>
                        </p>
                      </div>
                    )}
                  </div>
                </div>
              )}
            />
          </motion.div>
        )}

        {activeTab === 'movimientos' && (
          <motion.div key="movimientos" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }}>
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
                      <span className="text-foreground font-medium">{new Date(item.fecha).toLocaleDateString()}</span>
                      <span className="text-[10px] text-muted-foreground">{new Date(item.fecha).toLocaleTimeString()}</span>
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
                    <span className="font-mono font-bold text-foreground">
                      {(item.cantidad ?? 0).toLocaleString()}
                    </span>
                  ) 
                },
                { header: 'Justificación', accessor: 'justificacion', className: 'max-w-[200px] truncate' }
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-3">
                  <div className="flex justify-between items-start">
                    <div className="flex items-center gap-2">
                      <Calendar size={14} className="text-muted-foreground" />
                      <span className="text-xs font-bold text-muted-foreground">{new Date(item.fecha).toLocaleString()}</span>
                    </div>
                    <span className={cn(
                        "inline-flex items-center gap-1 px-2 py-0.5 rounded text-[10px] font-black uppercase tracking-widest",
                        item.tipo.toLowerCase().includes('entrada') || item.tipo.toLowerCase().includes('compra') ? "bg-emerald-500/10 text-emerald-400" : "bg-red-500/10 text-red-400"
                      )}>
                        {item.tipo}
                    </span>
                  </div>
                  <h3 className="font-bold text-foreground text-lg">{item.nombreProducto}</h3>
                  <div className="flex items-center justify-between p-3 bg-muted/50 rounded-xl border border-border">
                    <div className="text-xs text-muted-foreground uppercase font-bold">Cantidad</div>
                    <div className="text-xl font-black text-foreground">{(item.cantidad ?? 0).toLocaleString()}</div>
                  </div>
                  {item.justificacion && (
                    <p className="text-xs text-muted-foreground italic">"{item.justificacion}"</p>
                  )}
                </div>
              )}
            />
          </motion.div>
        )}

        {activeTab === 'compras' && (
          <motion.div key="compras" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }}>
            <UniversalGrid
              title="Ordenes de Compra"
              items={compras}
              isLoading={isLoadingCompras}
              onAdd={isEmpleado ? undefined : () => setIsCompraModalOpen(true)}
              columns={[
                { 
                  header: 'Proveedor', 
                  accessor: (item) => (
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 rounded-full bg-amber-500/10 flex items-center justify-center text-amber-500">
                        <User size={18} />
                      </div>
                      <span className="font-bold text-foreground">{item.proveedorNombre}</span>
                    </div>
                  ) 
                },
                { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
                { 
                  header: 'Total', 
                  accessor: (item) => (
                    <span className="font-mono font-bold text-foreground">
                      Bs. {(item.total ?? 0).toLocaleString()}
                    </span>
                  )
                },
                {
                  header: 'Saldo',
                  accessor: (item) => (
                    <span className={cn("font-mono font-bold", item.saldoPendiente > 0 ? "text-red-400" : "text-emerald-400")}>
                      Bs. {(item.saldoPendiente ?? 0).toLocaleString()}
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
                },
                ...(!isEmpleado ? [{
                    header: 'Acciones',
                    accessor: (item: any) => (
                        <button 
                            onClick={() => setSelectedCompraId(item.id)}
                            className="p-2 bg-emerald-500/10 hover:bg-emerald-500/20 rounded-lg text-emerald-500 transition-colors"
                        >
                            <DollarSign size={16} />
                        </button>
                    )
                }] : [])
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-4">
                  <div className="flex justify-between items-center">
                    <span className="text-xs font-bold text-muted-foreground uppercase">{new Date(item.fecha).toLocaleDateString()}</span>
                    <span className={cn(
                      "px-2 py-0.5 rounded text-[10px] font-black uppercase tracking-widest",
                      item.estadoPago === 'Pagado' ? "bg-emerald-500/10 text-emerald-400" : "bg-amber-500/10 text-amber-400"
                    )}>
                      {item.estadoPago}
                    </span>
                  </div>
                  <div className="flex justify-between items-start">
                    <h3 className="text-xl font-black text-foreground">{item.proveedorNombre}</h3>
                    {!isEmpleado && (
                      <button 
                          onClick={() => setSelectedCompraId(item.id)}
                          className="p-3 bg-emerald-500/10 rounded-xl text-emerald-500"
                      >
                          <DollarSign size={20} />
                      </button>
                    )}
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div className="p-3 bg-muted/50 rounded-xl border border-border">
                      <p className="text-[10px] text-muted-foreground font-bold uppercase mb-1">Total Compra</p>
                      <p className="text-lg font-black text-foreground">Bs. {(item.total ?? 0).toLocaleString()}</p>
                    </div>
                    <div className="p-3 bg-muted/50 rounded-xl border border-border">
                      <p className="text-[10px] text-muted-foreground font-bold uppercase mb-1">Saldo Pendiente</p>
                      <p className={cn("text-lg font-black", item.saldoPendiente > 0 ? "text-red-400" : "text-emerald-400")}>
                        Bs. {(item.saldoPendiente ?? 0).toLocaleString()}
                      </p>
                    </div>
                  </div>
                </div>
              )}
            />
          </motion.div>
        )}

        {activeTab === 'reportes' && (
          <motion.div key="reportes" initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }} className="space-y-6">
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Valoración por Categoría */}
                <div className="glass rounded-[2.5rem] border border-border p-8">
                    <h3 className="text-lg font-black text-foreground uppercase tracking-widest mb-6 flex items-center gap-2">
                        <BarChart3 className="text-blue-400" /> Valoración por Categoría
                    </h3>
                    <div className="space-y-4">
                        {valoracion?.detalles.map((det, i) => (
                            <div key={i} className="space-y-2">
                                <div className="flex justify-between text-sm">
                                    <span className="text-muted-foreground font-bold uppercase">{det.categoria}</span>
                                    <span className="text-foreground font-black">Bs. {(det.valor || 0).toLocaleString()}</span>
                                </div>
                                <div className="h-2 bg-muted/50 rounded-full overflow-hidden">
                                    <motion.div 
                                        initial={{ width: 0 }}
                                        animate={{ width: `${det.porcentaje || 0}%` }}
                                        className="h-full bg-blue-500"
                                    />
                                </div>
                                <p className="text-[10px] text-muted-foreground text-right font-bold">{(det.porcentaje || 0).toFixed(1)}% del total</p>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Proyecciones de Agotamiento */}
                <div className="glass rounded-[2.5rem] border border-border p-8">
                    <h3 className="text-lg font-black text-foreground uppercase tracking-widest mb-6 flex items-center gap-2">
                        <Clock className="text-amber-400" /> Proyecciones de Agotamiento
                    </h3>
                    <div className="space-y-4">
                        {proyecciones.map((proy, i) => (
                            <div key={i} className="p-4 bg-muted/50 rounded-2xl border border-border flex items-center justify-between">
                                <div>
                                    <p className="font-bold text-foreground">{proy.nombreProducto}</p>
                                    <p className="text-xs text-muted-foreground uppercase font-bold tracking-tighter">Consumo: {(proy.consumoDiarioEstimado || 0).toLocaleString()} / día</p>
                                </div>
                                <div className="text-right">
                                    <p className={cn(
                                        "text-lg font-black",
                                        proy.diasRestantes < 5 ? "text-red-400" : proy.diasRestantes < 15 ? "text-amber-400" : "text-emerald-400"
                                    )}>
                                        {(proy.diasRestantes || 0).toFixed(1)} días
                                    </p>
                                    <p className="text-[10px] text-muted-foreground uppercase font-bold">Agotamiento: {new Date(proy.fechaAgotamientoEstimada).toLocaleDateString()}</p>
                                </div>
                            </div>
                        ))}
                        {proyecciones.length === 0 && <p className="text-muted-foreground text-center py-8 italic uppercase text-xs font-bold tracking-widest">No hay proyecciones disponibles</p>}
                    </div>
                </div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>

      {/* --- Modales --- */}

      {/* Modal Detalle Producto */}
      <ProductoDetalleModal 
        productoId={selectedProductoId} 
        onClose={() => setSelectedProductoId(null)} 
      />

      {/* Modal Pagos Compra */}
      <CompraPagosModal
        compraId={selectedCompraId}
        onClose={() => setSelectedCompraId(null)}
      />

      {/* Modal Compra */}
      <AnimatePresence>
        {isCompraModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsCompraModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-foreground">Registrar Compra</h2>
                <button onClick={() => setIsCompraModalOpen(false)} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              <form onSubmit={compraForm.handleSubmit(onCompraSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Producto</label>
                  <select {...compraForm.register('productoId')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                    <option value="">Seleccionar producto</option>
                    {productos.map(p => <option key={p.id} value={p.id}>{p.nombre}</option>)}
                  </select>
                  {compraForm.formState.errors.productoId && <p className="text-xs text-red-400">{compraForm.formState.errors.productoId.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Proveedor</label>
                  <select {...compraForm.register('proveedorId')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                    <option value="">Seleccionar proveedor</option>
                    {proveedores.map(p => <option key={p.id} value={p.id}>{p.razonSocial}</option>)}
                  </select>
                  {compraForm.formState.errors.proveedorId && <p className="text-xs text-red-400">{compraForm.formState.errors.proveedorId.message}</p>}
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Cantidad</label>
                    <input type="number" step="0.01" {...compraForm.register('cantidad', { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                    {compraForm.formState.errors.cantidad && <p className="text-xs text-red-400">{compraForm.formState.errors.cantidad.message}</p>}
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Costo Total (Bs.)</label>

                    <input type="number" step="0.01" {...compraForm.register('costoTotalCompra', { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                    {compraForm.formState.errors.costoTotalCompra && <p className="text-xs text-red-400">{compraForm.formState.errors.costoTotalCompra.message}</p>}
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Monto Pagado Hoy (Bs.)</label>
                  <div className="relative">
                    <DollarSign className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input type="number" step="0.01" {...compraForm.register('montoPagado', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                  </div>
                  {compraForm.formState.errors.montoPagado && <p className="text-xs text-red-400">{compraForm.formState.errors.montoPagado.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Nota (Opcional)</label>
                  <textarea {...compraForm.register('nota')} rows={2} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" placeholder="Ej. Factura #1234..." />
                </div>

                <button type="submit" disabled={registrarCompra.isPending} className="w-full py-4 bg-amber-500 hover:bg-amber-600 text-black font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4">
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
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-foreground">Ajuste de Stock</h2>
                <button onClick={() => setIsAjusteModalOpen(false)} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              <form onSubmit={ajusteForm.handleSubmit(onAjusteSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Producto</label>
                  <select {...ajusteForm.register('productoId')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                    <option value="">Seleccionar producto</option>
                    {productos.map(p => <option key={p.id} value={p.id}>{p.nombre}</option>)}
                  </select>
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Tipo Ajuste</label>
                    <select {...ajusteForm.register('tipo')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                      <option value="Entrada">Entrada (+)</option>
                      <option value="Salida">Salida (-)</option>
                    </select>
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Cantidad</label>
                    <input type="number" step="0.01" {...ajusteForm.register('cantidad', { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Justificación</label>
                  <textarea {...ajusteForm.register('justificacion')} rows={3} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" placeholder="¿Por qué se realiza este ajuste?" />
                  {ajusteForm.formState.errors.justificacion && <p className="text-xs text-red-400">{ajusteForm.formState.errors.justificacion.message}</p>}
                </div>

                <button type="submit" disabled={registrarAjuste.isPending} className="w-full py-4 bg-blue-500 hover:bg-blue-600 text-white font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4">
                  <Save size={20} /> Guardar Ajuste
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>

      {/* Modal Conciliación */}
      <AnimatePresence>
        {isConciliacionModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsConciliacionModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ y: '100%' }} animate={{ y: 0 }} exit={{ y: '100%' }} className="fixed bottom-0 left-0 right-0 glass z-[110] shadow-2xl p-8 rounded-t-[3rem] border-t border-border max-h-[90vh] overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <div>
                    <h2 className="text-2xl font-bold text-foreground">Conciliación de Inventario</h2>
                    <p className="text-xs text-muted-foreground uppercase font-black tracking-widest mt-1">Sincroniza el stock físico con el sistema</p>
                </div>
                <button onClick={() => setIsConciliacionModalOpen(false)} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              <form onSubmit={conciliacionForm.handleSubmit(onConciliacionSubmit)} className="space-y-6">
                <div className="space-y-4">
                    {fields.map((field, index) => (
                        <div key={field.id} className="grid grid-cols-1 md:grid-cols-4 gap-4 p-4 bg-muted/50 rounded-2xl border border-border relative group">
                            <div className="md:col-span-2 space-y-2">
                                <label className="text-[10px] font-black text-muted-foreground uppercase ml-1">Producto</label>
                                <select {...conciliacionForm.register(`items.${index}.productoId`)} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                                    <option value="">Seleccionar</option>
                                    {productos.map(p => <option key={p.id} value={p.id}>{p.nombre}</option>)}
                                </select>
                            </div>
                            <div className="space-y-2">
                                <label className="text-[10px] font-black text-muted-foreground uppercase ml-1">Cantidad Física</label>
                                <input type="number" step="0.01" {...conciliacionForm.register(`items.${index}.cantidadFisica`, { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                            </div>
                            <div className="space-y-2 flex items-end">
                                <button type="button" onClick={() => remove(index)} className="w-full py-3 bg-red-500/10 hover:bg-red-500/20 text-red-500 rounded-xl transition-all flex items-center justify-center">
                                    <Trash2 size={18} />
                                </button>
                            </div>
                        </div>
                    ))}
                </div>

                <div className="flex flex-col md:flex-row gap-4">
                    <button type="button" onClick={() => append({ productoId: '', cantidadFisica: 0, nota: '' })} className="flex-1 py-4 bg-muted/50 hover:bg-muted/50 text-foreground font-bold rounded-2xl transition-all border border-border flex items-center justify-center gap-2">
                        <Plus size={20} /> Agregar Producto
                    </button>
                    <button type="submit" disabled={realizarConciliacion.isPending} className="flex-1 py-4 bg-emerald-500 hover:bg-emerald-600 text-black font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50">
                        <Save size={20} /> Finalizar Conciliación
                    </button>
                </div>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>

    </div>
  )
}

// --- Sub-Components (Modals) ---

function ProductoDetalleModal({ productoId, onClose }: { productoId: string | null, onClose: () => void }) {
    const { stock, kardex, movimientos, isLoadingKardex } = useProductoDetalle(productoId || '')

    return (
        <AnimatePresence>
            {productoId && (
                <>
                    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[150]" />
                    <motion.div initial={{ y: '100%' }} animate={{ y: 0 }} exit={{ y: '100%' }} className="fixed inset-x-0 bottom-0 top-10 md:inset-y-0 md:right-0 md:left-auto md:w-full md:max-w-2xl glass z-[160] shadow-2xl p-8 rounded-t-[3rem] md:rounded-none overflow-y-auto">
                        <div className="flex items-center justify-between mb-8">
                            <div>
                                <h2 className="text-2xl font-bold text-foreground">{stock?.nombreProducto || 'Cargando...'}</h2>
                                <p className="text-xs text-muted-foreground uppercase font-black tracking-widest mt-1">Kardex y Movimientos</p>
                            </div>
                            <button onClick={onClose} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
                        </div>

                        {isLoadingKardex ? (
                            <div className="flex items-center justify-center h-64">
                                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500" />
                            </div>
                        ) : (
                            <div className="space-y-8">
                                {/* Stock Summary */}
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div className="p-4 bg-muted/50 rounded-2xl border border-border">
                                        <p className="text-[10px] text-muted-foreground font-black uppercase mb-1">Stock Actual</p>
                                        <p className="text-2xl font-black text-emerald-400">{(stock?.stockActual ?? 0).toLocaleString()} <span className="text-xs font-normal opacity-60">{stock?.unidadMedida}</span></p>
                                    </div>
                                    {(stock?.stockActualKg ?? 0) > 0 && (
                                        <div className="p-4 bg-muted/50 rounded-2xl border border-border">
                                            <p className="text-[10px] text-muted-foreground font-black uppercase mb-1">Equivalente</p>
                                            <p className="text-2xl font-black text-foreground">{(stock?.stockActualKg ?? 0).toLocaleString()} <span className="text-xs font-normal opacity-60">kg</span></p>
                                        </div>
                                    )}
                                </div>

                                {/* Kardex Table */}
                                <div>
                                    <h3 className="text-xs font-black text-muted-foreground uppercase tracking-[0.2em] mb-4 border-b border-border pb-2">Kardex de Inventario</h3>
                                    <div className="overflow-x-auto">
                                        <table className="w-full text-left text-sm">
                                            <thead>
                                                <tr className="text-[10px] text-muted-foreground uppercase font-black">
                                                    <th className="pb-4 pr-4">Fecha</th>
                                                    <th className="pb-4 pr-4">Tipo</th>
                                                    <th className="pb-4 pr-4">Entrada</th>
                                                    <th className="pb-4 pr-4">Salida</th>
                                                    <th className="pb-4">Saldo</th>
                                                </tr>
                                            </thead>
                                            <tbody className="divide-y divide-white/5">
                                                {kardex.map((item, i) => (
                                                    <tr key={i} className="group">
                                                        <td className="py-3 pr-4 text-xs font-medium text-slate-300">
                                                            {new Date(item.fecha).toLocaleDateString()}
                                                        </td>
                                                        <td className="py-3 pr-4">
                                                            <span className="text-[10px] font-bold text-foreground uppercase">{item.tipo}</span>
                                                        </td>
                                                        <td className="py-3 pr-4 text-emerald-400 font-mono font-bold">
                                                            {item.entrada > 0 ? `+${item.entrada}` : '-'}
                                                        </td>
                                                        <td className="py-3 pr-4 text-red-400 font-mono font-bold">
                                                            {item.salida > 0 ? `-${item.salida}` : '-'}
                                                        </td>
                                                        <td className="py-3 font-mono font-black text-foreground">
                                                            {(item.saldo ?? 0).toLocaleString()}
                                                        </td>
                                                    </tr>
                                                ))}
                                                {kardex.length === 0 && (
                                                    <tr>
                                                        <td colSpan={5} className="py-8 text-center text-muted-foreground italic text-xs uppercase font-bold tracking-widest">Sin registros en el kardex</td>
                                                    </tr>
                                                )}
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        )}
                    </motion.div>
                </>
            )}
        </AnimatePresence>
    )
}

function CompraPagosModal({ compraId, onClose }: { compraId: string | null, onClose: () => void }) {
    const { pagos, isLoadingPagos, registrarPago, eliminarPago } = useCompraPagos(compraId || '')
    const [isAddPagoOpen, setIsAddPagoOpen] = useState(false)
    
    const form = useForm<PagoFormValues>({
        resolver: zodResolver(pagoSchema),
        defaultValues: {
            monto: 0,
            metodoPago: 1,
            fechaPago: new Date().toISOString().split('T')[0]
        }
    })

    const onSubmit = (data: PagoFormValues) => {
        registrarPago.mutate(data, {
            onSuccess: () => {
                toast.success('Pago registrado')
                setIsAddPagoOpen(false)
                form.reset()
            },
            onError: (err: any) => toast.error(err.message)
        })
    }

    return (
        <AnimatePresence>
            {compraId && (
                <>
                    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={onClose} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[150]" />
                    <motion.div initial={{ y: '100%' }} animate={{ y: 0 }} exit={{ y: '100%' }} className="fixed inset-x-0 bottom-0 glass z-[160] shadow-2xl p-8 rounded-t-[3rem] border-t border-border max-h-[80vh] overflow-y-auto">
                        <div className="flex items-center justify-between mb-8">
                            <div>
                                <h2 className="text-2xl font-bold text-foreground">Gestión de Pagos</h2>
                                <p className="text-xs text-muted-foreground uppercase font-black tracking-widest mt-1">Detalle de abonos a la compra</p>
                            </div>
                            <button onClick={onClose} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
                        </div>

                        <div className="space-y-6">
                            {!isAddPagoOpen ? (
                                <button 
                                    onClick={() => setIsAddPagoOpen(true)}
                                    className="w-full py-4 bg-emerald-500 hover:bg-emerald-600 text-black font-bold rounded-2xl transition-all flex items-center justify-center gap-2"
                                >
                                    <Plus size={20} /> Registrar Nuevo Abono
                                </button>
                            ) : (
                                <form onSubmit={form.handleSubmit(onSubmit)} className="p-6 bg-muted/50 rounded-2xl border border-border space-y-4">
                                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                                        <div className="space-y-2">
                                            <label className="text-[10px] font-black text-muted-foreground uppercase">Monto</label>
                                            <input type="number" step="0.01" {...form.register('monto', { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                                        </div>
                                        <div className="space-y-2">
                                            <label className="text-[10px] font-black text-muted-foreground uppercase">Método</label>
                                            <select {...form.register('metodoPago', { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                                                <option value={1}>Efectivo</option>
                                                <option value={2}>Transferencia</option>
                                                <option value={3}>Cheque</option>
                                            </select>
                                        </div>
                                        <div className="space-y-2">
                                            <label className="text-[10px] font-black text-muted-foreground uppercase">Fecha</label>
                                            <input type="date" {...form.register('fechaPago')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                                        </div>
                                    </div>
                                    <div className="flex gap-3">
                                        <button type="submit" className="flex-1 py-3 bg-emerald-500 text-black font-bold rounded-xl hover:bg-emerald-600 transition-all">Guardar Pago</button>
                                        <button type="button" onClick={() => setIsAddPagoOpen(false)} className="px-6 py-3 bg-muted/50 text-foreground font-bold rounded-xl hover:bg-muted/50 transition-all border border-border">Cancelar</button>
                                    </div>
                                </form>
                            )}

                            <div className="space-y-3">
                                {pagos.map((pago) => (
                                    <div key={pago.id} className="p-4 bg-muted/50 rounded-2xl border border-border flex items-center justify-between group">
                                        <div className="flex items-center gap-4">
                                            <div className="w-10 h-10 rounded-xl bg-emerald-500/10 flex items-center justify-center text-emerald-500">
                                                <DollarSign size={20} />
                                            </div>
                                            <div>
                                            <p className="text-lg font-black text-foreground">Bs. {(pago.monto ?? 0).toLocaleString()}</p>
                                                <p className="text-[10px] text-muted-foreground font-bold uppercase">{new Date(pago.fechaPago).toLocaleDateString()} • {pago.metodoPago}</p>
                                            </div>
                                        </div>
                                        <button 
                                            onClick={() => eliminarPago.mutate(pago.id)}
                                            className="p-3 text-muted-foreground hover:text-red-400 transition-colors opacity-0 group-hover:opacity-100"
                                        >
                                            <Trash2 size={18} />
                                        </button>
                                    </div>
                                ))}
                                {pagos.length === 0 && <p className="text-center py-8 text-muted-foreground italic uppercase text-[10px] font-black tracking-widest">No se han registrado abonos</p>}
                            </div>
                        </div>
                    </motion.div>
                </>
            )}
        </AnimatePresence>
    )
}



