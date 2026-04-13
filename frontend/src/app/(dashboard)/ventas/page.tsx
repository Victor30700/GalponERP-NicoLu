'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
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
  History
} from 'lucide-react'
import { toast } from 'sonner'
import { cn } from '@/lib/utils'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'

// --- Interfaces ---

interface Venta {
  id: string
  loteId: string
  clienteId: string
  clienteNombre: string
  fecha: string
  cantidadPollos: number
  pesoTotalKg: number
  precioPorKilo: number
  total: number
  saldoPendiente: number
  estadoPago: string
}

interface CuentaPorCobrar {
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
  nombreLote?: string
  avesVivas: number
}

interface Cliente {
  id: string
  nombre: string
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
  metodoPago: z.coerce.number().int().min(1).max(4),
})

type PagoFormValues = z.infer<typeof pagoSchema>

// --- Main Component ---

export default function VentasPage() {
  const [activeTab, setActiveTab] = useState<'pendientes' | 'todas'>('pendientes')
  const [isVentaModalOpen, setIsVentaModalOpen] = useState(false)
  const [isPagoModalOpen, setIsPagoModalOpen] = useState(false)
  const [selectedVenta, setSelectedVenta] = useState<string | null>(null)
  const queryClient = useQueryClient()

  // Queries
  const { data: cuentasPorCobrar = [], isLoading: isLoadingCuentas } = useQuery({
    queryKey: ['finanzas', 'cuentas-por-cobrar'],
    queryFn: () => api.get<CuentaPorCobrar[]>('/api/Finanzas/cuentas-por-cobrar'),
  })

  const { data: todasLasVentas = [], isLoading: isLoadingVentas } = useQuery({
    queryKey: ['ventas'],
    queryFn: () => api.get<Venta[]>('/api/Ventas'),
  })

  const { data: lotes = [] } = useQuery({
    queryKey: ['lotes', 'activos'],
    queryFn: () => api.get<Lote[]>('/api/Lotes?soloActivos=true'),
  })

  const { data: clientes = [] } = useQuery({
    queryKey: ['clientes'],
    queryFn: () => api.get<Cliente[]>('/api/Clientes'),
  })

  // Mutations
  const registrarVentaMutation = useMutation({
    mutationFn: (data: VentaFormValues) => api.post('/api/Ventas/parcial', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ventas'] })
      queryClient.invalidateQueries({ queryKey: ['finanzas', 'cuentas-por-cobrar'] })
      toast.success('Venta registrada con éxito')
      setIsVentaModalOpen(false)
      ventaForm.reset()
    },
    onError: (err: any) => toast.error(err.message),
  })

  const registrarPagoMutation = useMutation({
    mutationFn: (data: { ventaId: string; values: PagoFormValues }) => 
      api.post(`/api/Ventas/${data.ventaId}/pagos`, data.values),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['ventas'] })
      queryClient.invalidateQueries({ queryKey: ['finanzas', 'cuentas-por-cobrar'] })
      toast.success('Pago registrado correctamente')
      setIsPagoModalOpen(false)
      pagoForm.reset()
    },
    onError: (err: any) => toast.error(err.message),
  })

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
    registrarVentaMutation.mutate(data)
  }

  const onPagoSubmit = (data: PagoFormValues) => {
    if (selectedVenta) {
      registrarPagoMutation.mutate({ ventaId: selectedVenta, values: data })
    }
  }

  const openPagoModal = (ventaId: string, saldoPendiente: number) => {
    setSelectedVenta(ventaId)
    pagoForm.setValue('monto', saldoPendiente)
    setIsPagoModalOpen(true)
  }

  const totalPorCobrar = cuentasPorCobrar.reduce((acc, c) => acc + c.saldoPendiente, 0)

  return (
    <div className="space-y-6">
      {/* Header with Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div className="glass-dark border border-white/5 p-4 rounded-2xl flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-amber-500/10 flex items-center justify-center text-amber-500">
            <BadgeDollarSign size={24} />
          </div>
          <div>
            <p className="text-xs text-slate-500 font-bold uppercase tracking-wider">Por Cobrar</p>
            <p className="text-2xl font-black text-white">${totalPorCobrar.toLocaleString()}</p>
          </div>
        </div>
        <div className="glass-dark border border-white/5 p-4 rounded-2xl flex items-center gap-4">
          <div className="w-12 h-12 rounded-xl bg-blue-500/10 flex items-center justify-center text-blue-500">
            <HandCoins size={24} />
          </div>
          <div>
            <p className="text-xs text-slate-500 font-bold uppercase tracking-wider">Pendientes</p>
            <p className="text-2xl font-black text-white">{cuentasPorCobrar.length} Ventas</p>
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
      <div className="flex p-1 bg-slate-900/50 border border-white/5 rounded-2xl w-full md:w-fit">
        <button
          onClick={() => setActiveTab('pendientes')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'pendientes' ? "bg-blue-500 text-white shadow-lg" : "text-slate-400 hover:text-slate-200"
          )}
        >
          <AlertCircle size={18} />
          Cuentas por Cobrar
        </button>
        <button
          onClick={() => setActiveTab('todas')}
          className={cn(
            "flex items-center gap-2 px-6 py-2.5 rounded-xl text-sm font-medium transition-all",
            activeTab === 'todas' ? "bg-blue-500 text-white shadow-lg" : "text-slate-400 hover:text-slate-200"
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
                  accessor: (item) => (
                    <div className="flex items-center gap-3">
                      <div className="w-9 h-9 rounded-full bg-slate-800 flex items-center justify-center text-slate-400">
                        <User size={16} />
                      </div>
                      <span className="font-bold text-white">{item.clienteNombre}</span>
                    </div>
                  ) 
                },
                { header: 'Lote', accessor: 'loteCodigo' },
                { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
                { 
                  header: 'Total', 
                  accessor: (item) => (
                    <span className="font-mono font-bold text-white">
                      ${item.totalVenta.toLocaleString()}
                    </span>
                  ) 
                },
                { 
                  header: 'Saldo', 
                  accessor: (item) => (
                    <span className="font-mono font-bold text-red-400">
                      ${item.saldoPendiente.toLocaleString()}
                    </span>
                  ) 
                },
                {
                  header: 'Acción',
                  accessor: (item) => (
                    <button 
                      onClick={() => openPagoModal(item.ventaId, item.saldoPendiente)}
                      className="px-3 py-1 bg-emerald-500 hover:bg-emerald-600 text-white text-[10px] font-bold uppercase rounded-lg transition-all"
                    >
                      Abonar Pago
                    </button>
                  )
                }
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-4">
                  <div className="flex justify-between items-start">
                    <div className="flex items-center gap-2">
                      <div className="w-8 h-8 rounded-full bg-slate-800 flex items-center justify-center text-slate-400">
                        <User size={14} />
                      </div>
                      <h3 className="font-bold text-white">{item.clienteNombre}</h3>
                    </div>
                    <span className="px-2 py-0.5 bg-red-500/10 text-red-400 text-[10px] font-black rounded uppercase tracking-widest border border-red-500/10">
                      Pendiente
                    </span>
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div className="p-3 bg-white/5 rounded-xl border border-white/5">
                      <p className="text-[10px] text-slate-500 font-bold uppercase mb-1">Total</p>
                      <p className="text-lg font-black text-white">${item.totalVenta.toLocaleString()}</p>
                    </div>
                    <div className="p-3 bg-white/5 rounded-xl border border-white/5">
                      <p className="text-[10px] text-slate-500 font-bold uppercase mb-1">Saldo</p>
                      <p className="text-lg font-black text-red-400">${item.saldoPendiente.toLocaleString()}</p>
                    </div>
                  </div>
                  <button 
                    onClick={() => openPagoModal(item.ventaId, item.saldoPendiente)}
                    className="w-full py-3 bg-emerald-500 hover:bg-emerald-600 text-white font-bold rounded-xl flex items-center justify-center gap-2 transition-all"
                  >
                    <HandCoins size={18} /> Registrar Pago
                  </button>
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
                      <span className="font-bold text-white">{item.clienteNombre}</span>
                    </div>
                  ) 
                },
                { header: 'Fecha', accessor: (item) => new Date(item.fecha).toLocaleDateString() },
                { 
                  header: 'Detalle', 
                  accessor: (item) => (
                    <span className="text-xs text-slate-400">
                      {item.cantidadPollos} pollos | {item.pesoTotalKg} Kg
                    </span>
                  ) 
                },
                { 
                  header: 'Total', 
                  accessor: (item) => (
                    <span className="font-mono font-bold text-white">
                      ${item.total.toLocaleString()}
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
                }
              ]}
              renderMobileCard={(item) => (
                <div className="space-y-3">
                  <div className="flex justify-between">
                    <span className="text-xs font-bold text-slate-500 uppercase">{new Date(item.fecha).toLocaleDateString()}</span>
                    <span className={cn(
                      "px-2 py-0.5 rounded text-[10px] font-black tracking-tighter uppercase",
                      item.estadoPago === 'Pagado' ? "bg-emerald-500/10 text-emerald-400" : "bg-red-500/10 text-red-400"
                    )}>
                      {item.estadoPago}
                    </span>
                  </div>
                  <h3 className="font-bold text-white">{item.clienteNombre}</h3>
                  <div className="flex items-center justify-between text-xs text-slate-400 pb-2 border-b border-white/5">
                    <span>{item.cantidadPollos} Pollos</span>
                    <span>{item.pesoTotalKg} Kg</span>
                  </div>
                  <div className="flex justify-between items-center pt-1">
                    <span className="text-xs text-slate-500 uppercase font-bold">Total Venta</span>
                    <span className="text-lg font-black text-white">${item.total.toLocaleString()}</span>
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
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass-dark z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-white">Nueva Venta de Pollos</h2>
                <button onClick={() => setIsVentaModalOpen(false)} className="p-2 bg-white/5 rounded-full text-slate-400"><X size={20} /></button>
              </div>

              <form onSubmit={ventaForm.handleSubmit(onVentaSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Lote de Origen</label>
                  <select {...ventaForm.register('loteId')} className="w-full px-4 py-3 bg-slate-900 border border-white/10 rounded-xl text-white appearance-none">
                    <option value="">Seleccionar lote activo</option>
                    {lotes.map(l => (
                      <option key={l.id} value={l.id}>{l.codigo} ({l.avesVivas} aves)</option>
                    ))}
                  </select>
                  {ventaForm.formState.errors.loteId && <p className="text-xs text-red-400">{ventaForm.formState.errors.loteId.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Cliente</label>
                  <select {...ventaForm.register('clienteId')} className="w-full px-4 py-3 bg-slate-900 border border-white/10 rounded-xl text-white appearance-none">
                    <option value="">Seleccionar cliente</option>
                    {clientes.map(c => <option key={c.id} value={c.id}>{c.nombre}</option>)}
                  </select>
                  {ventaForm.formState.errors.clienteId && <p className="text-xs text-red-400">{ventaForm.formState.errors.clienteId.message}</p>}
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-400 ml-1">Cant. Pollos</label>
                    <div className="relative">
                      <Hash className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={16} />
                      <input type="number" {...ventaForm.register('cantidadPollos', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" />
                    </div>
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-400 ml-1">Peso Total (Kg)</label>
                    <div className="relative">
                      <Scale className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={16} />
                      <input type="number" step="0.01" {...ventaForm.register('pesoTotalVendido', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" />
                    </div>
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Precio por Kilo ($)</label>
                  <div className="relative">
                    <BadgeDollarSign className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input type="number" step="0.01" {...ventaForm.register('precioPorKilo', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" />
                  </div>
                </div>

                <div className="p-4 bg-blue-500/10 border border-blue-500/20 rounded-2xl flex justify-between items-center">
                  <span className="text-blue-400 font-bold">Total Estimado:</span>
                  <span className="text-2xl font-black text-white">
                    ${(ventaForm.watch('pesoTotalVendido') * ventaForm.watch('precioPorKilo') || 0).toLocaleString()}
                  </span>
                </div>

                <button type="submit" disabled={registrarVentaMutation.isPending} className="w-full py-4 bg-primary hover:bg-primary/90 text-primary-foreground font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4 shadow-lg shadow-primary/20">
                  <Save size={20} /> Registrar Venta
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>

      {/* Modal Registrar Pago */}
      <AnimatePresence>
        {isPagoModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsPagoModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass-dark z-[110] shadow-2xl p-6 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-white">Registrar Pago / Abono</h2>
                <button onClick={() => setIsPagoModalOpen(false)} className="p-2 bg-white/5 rounded-full text-slate-400"><X size={20} /></button>
              </div>

              <form onSubmit={pagoForm.handleSubmit(onPagoSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Monto del Pago ($)</label>
                  <div className="relative">
                    <Banknote className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={20} />
                    <input type="number" step="0.01" {...pagoForm.register('monto', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white text-xl font-bold" />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Método de Pago</label>
                  <div className="grid grid-cols-2 gap-3">
                    <button 
                      type="button"
                      onClick={() => pagoForm.setValue('metodoPago', 1)}
                      className={cn(
                        "p-4 rounded-xl border flex flex-col items-center gap-2 transition-all",
                        pagoForm.watch('metodoPago') === 1 ? "bg-emerald-500/20 border-emerald-500 text-emerald-400" : "bg-white/5 border-white/10 text-slate-400"
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
                        pagoForm.watch('metodoPago') === 2 ? "bg-blue-500/20 border-blue-500 text-blue-400" : "bg-white/5 border-white/10 text-slate-400"
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
                        pagoForm.watch('metodoPago') === 3 ? "bg-amber-500/20 border-amber-500 text-amber-400" : "bg-white/5 border-white/10 text-slate-400"
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
                        pagoForm.watch('metodoPago') === 4 ? "bg-purple-500/20 border-purple-500 text-purple-400" : "bg-white/5 border-white/10 text-slate-400"
                      )}
                    >
                      <QrCode size={24} />
                      <span className="text-xs font-bold uppercase">QR</span>
                    </button>
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Fecha de Pago</label>
                  <input type="date" {...pagoForm.register('fechaPago')} className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white" />
                </div>

                <button type="submit" disabled={registrarPagoMutation.isPending} className="w-full py-4 bg-emerald-500 hover:bg-emerald-600 text-white font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-4 shadow-lg shadow-emerald-500/20">
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
