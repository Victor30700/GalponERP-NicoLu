'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid, Column } from '@/components/shared/UniversalGrid'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  BadgeDollarSign, 
  HandCoins, 
  Receipt, 
  Plus, 
  Calendar, 
  User, 
  ChevronRight,
  X,
  Save,
  Scale,
  Hash,
  CreditCard,
  Banknote,
  QrCode,
  ArrowRightLeft,
  AlertCircle,
  History,
  Trash2,
  Eye
} from 'lucide-react'
import { toast } from 'sonner'
import { cn } from '@/lib/utils'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { useCatalogos } from '@/hooks/useCatalogos'
import { useVentas, Pago, Venta } from '@/hooks/useVentas'
import { useSwal } from '@/hooks/useSwal'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/lib/rbac'

// --- Interfaces ---

interface CuentaPorCobrar {
  id: string // Map from ventaId
  ventaId: string
  fecha: string
  clienteNombre: string
  loteCodigo: string
  totalVenta: number
  totalPagado: number
  saldoPendiente: number
  estadoPago: string
}

interface Lote {
  id: string
  codigo: string
  nombre?: string
  nombreLote?: string
  avesVivas: number
}

// --- Schemas ---

const ventaSchema = z.object({
  loteId: z.string().uuid('Lote inválido'),
  clienteId: z.string().uuid('Cliente inválido'),
  fecha: z.string(),
  cantidadPollos: z.number().int().positive('La cantidad debe ser mayor a 0'),
  pesoTotalVendido: z.number().positive('El peso debe ser mayor a 0'),
  precioPorKilo: z.number().positive('El precio debe ser mayor a 0'),
})

type VentaFormValues = z.infer<typeof ventaSchema>

const pagoSchema = z.object({
  monto: z.number().positive('El monto debe ser mayor a 0'),
  fechaPago: z.string(),
  metodoPago: z.number().int().min(1).max(4),
})

type PagoFormValues = z.infer<typeof pagoSchema>

// --- Main Component ---

export default function VentasPage() {
  const { profile } = useAuth()
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null
  const isEmpleado = userRole === UserRole.Empleado

  const [activeTab, setActiveTab] = useState<'pendientes' | 'todas'>('pendientes')
  const [isVentaModalOpen, setIsVentaModalOpen] = useState(false)
  const [isPagoModalOpen, setIsPagoModalOpen] = useState(false)
  const [isDetalleModalOpen, setIsDetalleModalOpen] = useState(false)
  const [selectedVenta, setSelectedVenta] = useState<string | null>(null)
  const { confirm } = useSwal()

  // Custom Hook
  const { 
    todasLasVentas, 
    isLoadingVentas, 
    registrarVenta, 
    registrarPago, 
    anularVenta, 
    eliminarPago,
    usePagosDeVenta,
    useVenta
  } = useVentas()

  // Queries
  const { data: cuentasPorCobrar = [], isLoading: isLoadingCuentas } = useQuery({
    queryKey: ['finanzas', 'cuentas-por-cobrar'],
    queryFn: async () => {
      const data = await api.get<CuentaPorCobrar[]>('/api/Finanzas/cuentas-por-cobrar');
      return data.map(item => ({ ...item, id: item.id || item.ventaId }));
    },
  })

  const { clientes } = useCatalogos()

  const { data: lotes = [] } = useQuery({
    queryKey: ['lotes', 'activos'],
    queryFn: () => api.get<Lote[]>('/api/Lotes?soloActivos=true'),
  })

  // Selected Sale Details
  const { data: ventaDetalle } = useVenta(selectedVenta || '')
  const { data: pagosDetalle = [], isLoading: isLoadingPagos } = usePagosDeVenta(selectedVenta || '')

  // Forms
  const ventaForm = useForm<VentaFormValues>({
    resolver: zodResolver(ventaSchema),
    defaultValues: {
      fecha: new Date().toISOString().split('T')[0],
      cantidadPollos: 0,
      pesoTotalVendido: 0,
      precioPorKilo: 0
    }
  })

  const pagoForm = useForm<PagoFormValues>({
    resolver: zodResolver(pagoSchema),
    defaultValues: {
      fechaPago: new Date().toISOString().split('T')[0],
      monto: 0,
      metodoPago: 1
    }
  })

  const onVentaSubmit = (data: VentaFormValues) => {
    registrarVenta.mutate(data, {
      onSuccess: () => {
        toast.success('Venta registrada con éxito')
        setIsVentaModalOpen(false)
        ventaForm.reset()
      },
      onError: (err: any) => toast.error(err.message)
    })
  }

  const onPagoSubmit = (data: PagoFormValues) => {
    if (selectedVenta) {
      registrarPago.mutate({ id: selectedVenta, data }, {
        onSuccess: () => {
          toast.success('Pago registrado correctamente')
          setIsPagoModalOpen(false)
          pagoForm.reset()
        },
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const handleAnularVenta = async (id: string) => {
    const confirmed = await confirm(
      '¿Anular Venta?',
      'Esta acción no se puede deshacer y devolverá los pollos al inventario.',
      'warning'
    )

    if (confirmed) {
      anularVenta.mutate(id, {
        onSuccess: () => toast.success('Venta anulada correctamente'),
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const handleEliminarPago = async (ventaId: string, pagoId: string) => {
    const confirmed = await confirm(
      '¿Eliminar Pago?',
      'El saldo pendiente de la venta aumentará.',
      'warning'
    )

    if (confirmed) {
      eliminarPago.mutate({ ventaId, pagoId }, {
        onSuccess: () => toast.success('Pago eliminado correctamente'),
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const openPagoModal = (ventaId: string, saldoPendiente: number) => {
    setSelectedVenta(ventaId)
    pagoForm.setValue('monto', saldoPendiente)
    setIsPagoModalOpen(true)
  }

  const openDetalleModal = (ventaId: string) => {
    setSelectedVenta(ventaId)
    setIsDetalleModalOpen(true)
  }

  const totalPorCobrar = cuentasPorCobrar.reduce((acc, c) => acc + c.saldoPendiente, 0)

  return (
    <div className="space-y-6">
      {/* Header with Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="glass border border-border p-4 rounded-2xl flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-amber-500/10 flex items-center justify-center text-amber-500">
            <BadgeDollarSign size={24} />
          </div>
          <div>
            <p className="text-xs text-muted-foreground font-bold uppercase tracking-wider">Por Cobrar</p>
            <p className="text-2xl font-black text-foreground">Bs. {totalPorCobrar.toLocaleString()}</p>
          </div>
        </div>
        <div className="glass border border-border p-4 rounded-2xl flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-blue-500/10 flex items-center justify-center text-blue-500">
            <HandCoins size={24} />
          </div>
          <div>
            <p className="text-xs text-muted-foreground font-bold uppercase tracking-wider">Pendientes</p>
            <p className="text-2xl font-black text-foreground">{cuentasPorCobrar.length} Ventas</p>
          </div>
        </div>
        <button 
          onClick={() => setIsVentaModalOpen(true)}
          className="bg-primary hover:bg-primary/90 text-primary-foreground p-4 rounded-2xl flex items-center justify-center gap-2 font-bold transition-all shadow-lg shadow-primary/20"
        >
          <Plus size={24} />
          Nueva Venta de Pollos
        </button>
      </div>

      {/* Tab Switcher */}
      <div className="flex p-1 bg-muted/50 border border-border rounded-2xl w-full md:w-fit">
        <button
          onClick={() => setActiveTab('pendientes')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'pendientes' ? "bg-blue-500 text-white shadow-lg" : "text-muted-foreground hover:text-slate-200"
          )}
        >
          <AlertCircle size={18} />
          Cuentas por Cobrar
        </button>
        <button
          onClick={() => setActiveTab('todas')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'todas' ? "bg-blue-500 text-white shadow-lg" : "text-muted-foreground hover:text-slate-200"
          )}
        >
          <History size={18} />
          Historial de Ventas
        </button>
      </div>

      <AnimatePresence mode="wait">
        {activeTab === 'pendientes' && (
          <motion.div key="pendientes" initial={{ opacity: 0, x: -10 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: 10 }}>
            <UniversalGrid
              title="Cartera de Clientes"
              items={cuentasPorCobrar}
              isLoading={isLoadingCuentas}
              searchPlaceholder="Buscar por cliente o lote..."
              columns={[
                { 
                  header: 'Cliente', 
                  accessor: (item: CuentaPorCobrar) => (
                    <div className="flex items-center gap-3">
                      <div className="w-9 h-9 rounded-full bg-slate-800 flex items-center justify-center text-muted-foreground">
                        <User size={16} />
                      </div>
                      <span className="font-bold text-foreground">{item.clienteNombre}</span>
                    </div>
                  ) 
                },
                { header: 'Lote', accessor: 'loteCodigo' },
                { header: 'Fecha', accessor: (item: CuentaPorCobrar) => new Date(item.fecha).toLocaleDateString() },
                { 
                  header: 'Total', 
                  accessor: (item: CuentaPorCobrar) => (
                    <span className="font-mono font-bold text-foreground">
                      Bs. {item.totalVenta.toLocaleString()}
                    </span>
                  ) 
                },
                { 
                  header: 'Saldo', 
                  accessor: (item: CuentaPorCobrar) => (
                    <span className="font-mono font-bold text-red-400">
                      Bs. {item.saldoPendiente.toLocaleString()}
                    </span>
                  ) 
                },
                {
                  header: 'Acción',
                  accessor: (item: CuentaPorCobrar) => (
                    <div className="flex gap-2">
                      <button 
                        onClick={() => openPagoModal(item.ventaId, item.saldoPendiente)}
                        className="px-3 py-1 bg-emerald-500 hover:bg-emerald-600 text-white text-[10px] font-bold uppercase rounded-lg transition-all"
                      >
                        Abonar Pago
                      </button>
                      <button 
                        onClick={() => openDetalleModal(item.ventaId)}
                        className="p-1.5 bg-muted/50 hover:bg-muted/50 text-muted-foreground rounded-lg transition-all"
                        title="Ver detalle"
                      >
                        <Eye size={14} />
                      </button>
                    </div>
                  )
                }
              ] as Column<CuentaPorCobrar>[]}
              renderMobileCard={(item) => (
                <div className="space-y-4">
                  <div className="flex justify-between items-start">
                    <div className="flex items-center gap-2">
                      <div className="w-8 h-8 rounded-full bg-slate-800 flex items-center justify-center text-muted-foreground">
                        <User size={14} />
                      </div>
                      <h3 className="font-bold text-foreground">{item.clienteNombre}</h3>
                    </div>
                    <span className="px-2 py-0.5 bg-red-500/10 text-red-400 text-[10px] font-black rounded uppercase tracking-widest border border-red-500/10">
                      Pendiente
                    </span>
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div className="p-3 bg-muted/50 rounded-xl border border-border">
                      <p className="text-[10px] text-muted-foreground font-bold uppercase mb-1">Total</p>
                      <p className="text-lg font-black text-foreground">Bs. {item.totalVenta.toLocaleString()}</p>
                    </div>
                    <div className="p-3 bg-muted/50 rounded-xl border border-border">
                      <p className="text-[10px] text-muted-foreground font-bold uppercase mb-1">Saldo</p>
                      <p className="text-lg font-black text-red-400">Bs. {item.saldoPendiente.toLocaleString()}</p>
                    </div>
                  </div>
                  <div className="flex gap-2">
                    <button 
                      onClick={() => openPagoModal(item.ventaId, item.saldoPendiente)}
                      className="flex-1 py-3 bg-emerald-500 hover:bg-emerald-600 text-white font-bold rounded-xl flex items-center justify-center gap-2 transition-all"
                    >
                      <HandCoins size={18} /> Registrar Pago
                    </button>
                    <button 
                      onClick={() => openDetalleModal(item.ventaId)}
                      className="px-4 bg-muted/50 hover:bg-muted/50 text-muted-foreground rounded-xl transition-all"
                    >
                      <Eye size={18} />
                    </button>
                  </div>
                </div>
              )}
            />
          </motion.div>
        )}

        {activeTab === 'todas' && (
          <motion.div key="todas" initial={{ opacity: 0, x: 10 }} animate={{ opacity: 1, x: 0 }} exit={{ opacity: 0, x: -10 }}>
            <UniversalGrid
              title="Historial Completo"
              items={todasLasVentas}
              isLoading={isLoadingVentas}
              columns={[
                { 
                  header: 'Cliente', 
                  accessor: (item) => (
                    <div className="flex items-center gap-3">
                      <span className="font-bold text-foreground">{item.clienteNombre}</span>
                    </div>
                  ) 
                },
                { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
                { 
                  header: 'Detalle', 
                  accessor: (item) => (
                    <span className="text-xs text-muted-foreground">
                      {item.cantidadPollos} pollos | {item.pesoTotalKg} Kg
                    </span>
                  ) 
                },
                { 
                  header: 'Total', 
                  accessor: (item) => (
                    <span className="font-mono font-bold text-foreground">
                      Bs. {item.total.toLocaleString()}
                    </span>
                  ) 
                },
                {
                  header: 'Estado',
                  accessor: (item) => (
                    <span className={cn(
                      "px-2 py-1 rounded text-[10px] font-bold uppercase",
                      item.estadoPago === 'Pagado' ? "bg-emerald-500/10 text-emerald-400" : "bg-red-500/10 text-red-400"
                    )}>
                      {item.estadoPago}
                    </span>
                  )
                },
                {
                  header: 'Acciones',
                  accessor: (item) => (
                    <div className="flex gap-2">
                      <button 
                        onClick={() => openDetalleModal(item.id)}
                        className="p-1.5 bg-muted/50 hover:bg-muted/50 text-muted-foreground rounded-lg transition-all"
                        title="Ver Pagos"
                      >
                        <Eye size={14} />
                      </button>
                      {!isEmpleado && (
                        <button 
                          onClick={() => handleAnularVenta(item.id)}
                          className="p-1.5 bg-red-500/10 hover:bg-red-500/20 text-red-400 rounded-lg transition-all"
                          title="Anular Venta"
                        >
                          <Trash2 size={14} />
                        </button>
                      )}
                    </div>
                  )
                }
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-3">
                  <div className="flex justify-between">
                    <span className="text-xs font-bold text-muted-foreground uppercase">{new Date(item.fecha).toLocaleDateString()}</span>
                    <span className={cn(
                      "px-2 py-0.5 rounded text-[10px] font-black tracking-tighter uppercase",
                      item.estadoPago === 'Pagado' ? "bg-emerald-500/10 text-emerald-400" : "bg-red-500/10 text-red-400"
                    )}>
                      {item.estadoPago}
                    </span>
                  </div>
                  <h3 className="font-bold text-foreground">{item.clienteNombre}</h3>
                  <div className="flex items-center justify-between text-xs text-muted-foreground pb-2 border-b border-border">
                    <span>{item.cantidadPollos} Pollos</span>
                    <span>{item.pesoTotalKg} Kg</span>
                  </div>
                  <div className="flex justify-between items-center pt-1">
                    <span className="text-xs text-muted-foreground uppercase font-bold">Total Venta</span>
                    <span className="text-lg font-black text-foreground">Bs. {item.total.toLocaleString()}</span>
                  </div>
                  <div className="flex gap-2 mt-2">
                    <button 
                      onClick={() => openDetalleModal(item.id)}
                      className="flex-1 py-2 bg-muted/50 text-muted-foreground text-xs font-bold rounded-lg flex items-center justify-center gap-2"
                    >
                      <Eye size={14} /> Ver Pagos
                    </button>
                    {!isEmpleado && (
                      <button 
                        onClick={() => handleAnularVenta(item.id)}
                        className="flex-1 py-2 bg-red-500/10 text-red-400 text-xs font-bold rounded-lg flex items-center justify-center gap-2"
                      >
                        <Trash2 size={14} /> Anular
                      </button>
                    )}
                  </div>
                </div>
              )}
            />
          </motion.div>
        )}
      </AnimatePresence>

      {/* --- Modales --- */}

      {/* Modal Nueva Venta */}
      <AnimatePresence>
        {isVentaModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsVentaModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-foreground">Nueva Venta de Pollos</h2>
                <button onClick={() => setIsVentaModalOpen(false)} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              <form onSubmit={ventaForm.handleSubmit(onVentaSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Lote de Origen</label>
                  <select {...ventaForm.register('loteId')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                    <option value="">Seleccionar lote activo</option>
                    {lotes.map(l => (
                      <option key={l.id} value={l.id}>{l.nombre || l.nombreLote || l.codigo} ({l.avesVivas} aves)</option>
                    ))}
                  </select>
                  {ventaForm.formState.errors.loteId && <p className="text-xs text-red-400">{ventaForm.formState.errors.loteId.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Cliente</label>
                  <select {...ventaForm.register('clienteId')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                    <option value="">Seleccionar cliente</option>
                    {clientes.map(c => <option key={c.id} value={c.id}>{c.nombre}</option>)}
                  </select>
                  {ventaForm.formState.errors.clienteId && <p className="text-xs text-red-400">{ventaForm.formState.errors.clienteId.message}</p>}
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Cant. Pollos</label>
                    <div className="relative">
                      <Hash className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={16} />
                      <input type="number" {...ventaForm.register('cantidadPollos', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                    </div>
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Peso Total (Kg)</label>
                    <div className="relative">
                      <Scale className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={16} />
                      <input type="number" step="0.01" {...ventaForm.register('pesoTotalVendido', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                    </div>
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Precio por Kilo (Bs.)</label>
                  <div className="relative">
                    <BadgeDollarSign className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input type="number" step="0.01" {...ventaForm.register('precioPorKilo', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                  </div>
                </div>

                <div className="p-4 bg-blue-500/10 border border-blue-500/20 rounded-2xl flex justify-between items-center">
                  <span className="text-blue-400 font-bold">Total Estimado:</span>
                  <span className="text-2xl font-black text-foreground">
                    Bs. ${(ventaForm.watch('pesoTotalVendido') * ventaForm.watch('precioPorKilo') || 0).toLocaleString()}
                  </span>
                </div>

                <button type="submit" disabled={registrarVenta.isPending} className="w-full py-4 bg-primary hover:bg-primary/90 text-primary-foreground font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4 shadow-lg shadow-primary/20">
                  <Save size={20} /> Registrar Venta
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>

      {/* Modal Detalle / Pagos */}
      <AnimatePresence>
        {isDetalleModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsDetalleModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <div>
                  <h2 className="text-2xl font-bold text-foreground">Detalle de Venta</h2>
                  <p className="text-sm text-muted-foreground">{ventaDetalle?.clienteNombre}</p>
                </div>
                <button onClick={() => setIsDetalleModalOpen(false)} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              {ventaDetalle && (
                <div className="space-y-6">
                  <div className="grid grid-cols-2 gap-4">
                    <div className="p-4 bg-muted/50 rounded-2xl border border-border">
                      <p className="text-[10px] text-muted-foreground font-bold uppercase mb-1">Total Venta</p>
                      <p className="text-xl font-black text-foreground">Bs. {ventaDetalle.total.toLocaleString()}</p>
                    </div>
                    <div className="p-4 bg-muted/50 rounded-2xl border border-border">
                      <p className="text-[10px] text-muted-foreground font-bold uppercase mb-1">Saldo Pendiente</p>
                      <p className="text-xl font-black text-red-400">Bs. {ventaDetalle.saldoPendiente.toLocaleString()}</p>
                    </div>
                  </div>

                  <div className="space-y-4">
                    <h3 className="text-sm font-bold text-muted-foreground uppercase tracking-widest flex items-center gap-2">
                      <History size={16} /> Historial de Pagos
                    </h3>
                    
                    {isLoadingPagos ? (
                      <div className="py-8 flex justify-center"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div></div>
                    ) : pagosDetalle.length === 0 ? (
                      <div className="py-12 text-center bg-muted/50 rounded-2xl border border-dashed border-border">
                        <p className="text-muted-foreground text-sm italic">No se han registrado pagos aún</p>
                      </div>
                    ) : (
                      <div className="space-y-3">
                        {pagosDetalle.map((pago) => (
                          <div key={pago.id} className="p-4 bg-muted/50 rounded-xl border border-border flex justify-between items-center group">
                            <div>
                              <p className="font-bold text-foreground">Bs. {pago.monto.toLocaleString()}</p>
                              <div className="flex items-center gap-2 mt-1">
                                <span className="text-[10px] text-muted-foreground font-medium">{new Date(pago.fechaPago).toLocaleDateString()}</span>
                                <span className="w-1 h-1 rounded-full bg-slate-700" />
                                <span className="text-[10px] text-blue-400 font-bold uppercase">
                                  {pago.metodoPago === 1 ? 'Efectivo' : pago.metodoPago === 2 ? 'Transferencia' : pago.metodoPago === 3 ? 'Depósito' : 'QR'}
                                </span>
                              </div>
                            </div>
                            {!isEmpleado && (
                              <button 
                                onClick={() => handleEliminarPago(ventaDetalle.id, pago.id)}
                                className="p-2 text-slate-600 hover:text-red-400 transition-colors opacity-0 group-hover:opacity-100"
                              >
                                <Trash2 size={16} />
                              </button>
                            )}
                          </div>
                        ))}
                      </div>
                    )}
                  </div>

                  {ventaDetalle.saldoPendiente > 0 && (
                    <button 
                      onClick={() => {
                        setIsDetalleModalOpen(false);
                        openPagoModal(ventaDetalle.id, ventaDetalle.saldoPendiente);
                      }}
                      className="w-full py-4 bg-emerald-500 hover:bg-emerald-600 text-white font-bold rounded-2xl transition-all flex items-center justify-center gap-2"
                    >
                      <Plus size={20} /> Registrar Nuevo Abono
                    </button>
                  )}
                </div>
              )}
            </motion.div>
          </>
        )}
      </AnimatePresence>

      {/* Modal Registrar Pago */}
      <AnimatePresence>
        {isPagoModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsPagoModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-foreground">Registrar Pago / Abono</h2>
                <button onClick={() => setIsPagoModalOpen(false)} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              <form onSubmit={pagoForm.handleSubmit(onPagoSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Monto del Pago (Bs.)</label>
                  <div className="relative">
                    <Banknote className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={20} />
                    <input type="number" step="0.01" {...pagoForm.register('monto', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground text-xl font-bold" />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Método de Pago</label>
                  <div className="grid grid-cols-2 gap-3">
                    <button 
                      type="button"
                      onClick={() => pagoForm.setValue('metodoPago', 1)}
                      className={cn(
                        "p-4 rounded-xl border flex flex-col items-center gap-2 transition-all",
                        pagoForm.watch('metodoPago') === 1 ? "bg-emerald-500/20 border-emerald-500 text-emerald-400" : "bg-muted/50 border-border text-muted-foreground"
                      )}
                    >
                      <Banknote size={24} />
                      <span className="text-xs font-bold uppercase">Efectivo</span>
                    </button>
                    <button 
                      type="button"
                      onClick={() => pagoForm.setValue('metodoPago', 2)}
                      className={cn(
                        "p-4 rounded-xl border flex flex-col items-center gap-2 transition-all",
                        pagoForm.watch('metodoPago') === 2 ? "bg-blue-500/20 border-blue-500 text-blue-400" : "bg-muted/50 border-border text-muted-foreground"
                      )}
                    >
                      <ArrowRightLeft size={24} />
                      <span className="text-xs font-bold uppercase">Transf.</span>
                    </button>
                    <button 
                      type="button"
                      onClick={() => pagoForm.setValue('metodoPago', 3)}
                      className={cn(
                        "p-4 rounded-xl border flex flex-col items-center gap-2 transition-all",
                        pagoForm.watch('metodoPago') === 3 ? "bg-amber-500/20 border-amber-500 text-amber-400" : "bg-muted/50 border-border text-muted-foreground"
                      )}
                    >
                      <CreditCard size={24} />
                      <span className="text-xs font-bold uppercase">Depósito</span>
                    </button>
                    <button 
                      type="button"
                      onClick={() => pagoForm.setValue('metodoPago', 4)}
                      className={cn(
                        "p-4 rounded-xl border flex flex-col items-center gap-2 transition-all",
                        pagoForm.watch('metodoPago') === 4 ? "bg-purple-500/20 border-purple-500 text-purple-400" : "bg-muted/50 border-border text-muted-foreground"
                      )}
                    >
                      <QrCode size={24} />
                      <span className="text-xs font-bold uppercase">QR</span>
                    </button>
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Fecha de Pago</label>
                  <input type="date" {...pagoForm.register('fechaPago')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" />
                </div>

                <button type="submit" disabled={registrarPago.isPending} className="w-full py-4 bg-emerald-500 hover:bg-emerald-600 text-white font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4 shadow-lg shadow-emerald-500/20">
                  <Save size={20} /> Confirmar Pago
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
