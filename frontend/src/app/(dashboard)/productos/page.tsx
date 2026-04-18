'use client'

import { useState, useMemo } from 'react'
import { useProductos, useProducto, Producto } from '@/hooks/useProductos'
import { useCategorias, Categoria } from '@/hooks/useCategorias'
import { useUnidadesMedida, TipoUnidad } from '@/hooks/useUnidadesMedida'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, Package, Tag, Scale, Info, AlertCircle, Droplets, Hash } from 'lucide-react'
import { toast } from 'sonner'
import { confirmDestructiveAction } from '@/lib/swal'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/lib/rbac'

const getProductoSchema = (categorias: Categoria[]) => z.object({
  nombre: z.string().min(3, 'El nombre es muy corto'),
  categoriaProductoId: z.string().uuid('Categoría inválida'),
  unidadMedidaId: z.string().uuid('Unidad de medida inválida'),
  pesoUnitarioKg: z.number().min(0, 'El peso unitario no puede ser negativo'),
  equivalenciaEnKg: z.number().min(0, 'No puede ser negativo').optional(),
  umbralMinimo: z.number().min(0, 'No puede ser negativo'),
  stockInicial: z.number().min(0, 'No puede ser negativo').optional().default(0),
}).refine(data => {
    const categoria = categorias.find(c => c.id === data.categoriaProductoId)
    if (categoria?.nombre.toLowerCase().includes('alimento')) {
        return data.pesoUnitarioKg > 0
    }
    return true
}, {
    message: "Para productos de tipo 'Alimento', el peso por unidad (Kg) debe ser mayor a 0 para el correcto cálculo del FCR.",
    path: ["pesoUnitarioKg"]
})

type ProductoFormValues = z.infer<ReturnType<typeof getProductoSchema>>

export default function ProductosPage() {
  const { profile } = useAuth()
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null
  const isEmpleado = userRole === UserRole.Empleado

  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingProductoId, setEditingProductoId] = useState<string | null>(null)
  
  const { productos, isLoading, crearProducto } = useProductos()
  const { actualizarProducto, eliminarProducto } = useProducto(editingProductoId || '')
  const { categorias } = useCategorias()
  const { unidades } = useUnidadesMedida()

  const schema = useMemo(() => getProductoSchema(categorias), [categorias])

  const {
    register,
    handleSubmit,
    reset,
    watch,
    formState: { errors },
  } = useForm<ProductoFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      nombre: '',
      categoriaProductoId: '',
      unidadMedidaId: '',
      pesoUnitarioKg: 1,
      umbralMinimo: 0,
      stockInicial: 0,
      equivalenciaEnKg: 0
    }
  })

  // Cálculo reactivo de equivalencia en Kg
  const watchStockInicial = watch('stockInicial')
  const watchPesoUnitario = watch('pesoUnitarioKg')
  const watchCategoriaId = watch('categoriaProductoId')

  const categoriaSeleccionada = categorias.find(c => c.id === watchCategoriaId)
  const esAlimento = categoriaSeleccionada?.nombre.toLowerCase().includes('alimento')

  const equivalenciaCalculada = (watchStockInicial || 0) * (watchPesoUnitario || 0)

  const onSubmit = (data: ProductoFormValues) => {
    const values = { 
      ...data, 
      pesoUnitarioKg: esAlimento ? Number(data.pesoUnitarioKg) : 0,
      umbralMinimo: Number(data.umbralMinimo),
      stockInicial: Number(data.stockInicial),
      equivalenciaEnKg: (!editingProductoId && esAlimento) ? equivalenciaCalculada : 0
    }
    if (editingProductoId) {
      actualizarProducto.mutate({ ...values, id: editingProductoId } as any, {
        onSuccess: () => {
          toast.success('Producto actualizado')
          closeForm()
        },
        onError: (err: any) => toast.error(err.message)
      })
    } else {
      crearProducto.mutate(values as any, {
        onSuccess: () => {
          toast.success('Producto creado correctamente')
          closeForm()
        },
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const openForm = (prod?: Producto) => {
    if (prod) {
      setEditingProductoId(prod.id)
      reset({
        nombre: prod.nombre,
        categoriaProductoId: prod.categoriaId,
        unidadMedidaId: prod.unidadMedidaId,
        pesoUnitarioKg: prod.pesoUnitarioKg,
        umbralMinimo: prod.umbralMinimo,
        stockInicial: prod.stockActual
      })
    } else {
      setEditingProductoId(null)
      reset({ nombre: '', categoriaProductoId: '', unidadMedidaId: '', pesoUnitarioKg: 0, umbralMinimo: 0, stockInicial: 0 })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingProductoId(null)
    reset()
  }

  const isPending = crearProducto.isPending || actualizarProducto.isPending

  const columns = [
    { 
      header: 'Producto', 
      accessor: (item: Producto) => (
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-full bg-blue-500/10 flex items-center justify-center text-blue-500 font-bold text-xs">
            <Package size={14} />
          </div>
          <div>
            <p className="font-medium text-foreground">{item.nombre}</p>
            <p className="text-[10px] text-muted-foreground uppercase font-bold">{item.categoriaNombre}</p>
          </div>
        </div>
      )
    },
    { 
      header: 'Inventario Disponible', 
      accessor: (item: Producto) => {
        const esBajoStock = (item.stockActual || 0) <= (item.umbralMinimo || 0);
        
        // Determinar visualización principal según TipoUnidad
        const esMasa = item.tipoUnidad === TipoUnidad.Masa;
        const esVolumen = item.tipoUnidad === TipoUnidad.Volumen;

        const mainColor = esBajoStock ? 'text-red-500 animate-pulse' : 
                         esMasa ? 'text-orange-500' : 
                         esVolumen ? 'text-blue-500' : 
                         'text-emerald-500';

        return (
          <div className="flex flex-col gap-1">
            <div className="flex items-center gap-2">
              <span className={`text-sm font-black ${mainColor}`}>
                {Number(item.stockActual || 0).toLocaleString(undefined, { 
                  minimumFractionDigits: 0, 
                  maximumFractionDigits: 2 
                })} {item.unidadMedidaNombre}
              </span>
              
              {esBajoStock && (
                <div className="p-1 bg-red-500/10 text-red-500 rounded-full" title="Stock por debajo del mínimo">
                  <AlertCircle size={12} />
                </div>
              )}
            </div>

            {/* Sub-metrica informativa: Mostrar KG si la unidad no es KG pero tiene peso */}
            <div className="flex items-center gap-1.5 text-[9px] text-muted-foreground font-bold bg-muted/30 w-fit px-2 py-0.5 rounded-lg border border-border/50">
              {esMasa && item.unidadMedidaNombre.toLowerCase() !== 'kg' && item.unidadMedidaNombre.toLowerCase() !== 'kilogramo' ? (
                 <><Scale size={10} /> {Number(item.stockActualKg || 0).toFixed(2)} Kg Totales</>
              ) : esVolumen ? (
                 <><Droplets size={10} /> {Number(item.stockActual || 0).toFixed(1)} Lts</>
              ) : (
                 <><Package size={10} /> Stock Físico</>
              )}
            </div>
          </div>
        );
      }
    },
    { 
      header: 'Estado', 
      accessor: (item: Producto) => (
        <span className={`px-2 py-1 rounded-full text-[10px] font-bold ${item.isActive ? 'bg-emerald-500/10 text-emerald-500' : 'bg-red-500/10 text-red-500'}`}>
          {item.isActive ? 'ACTIVO' : 'INACTIVO'}
        </span>
      )
    },
  ]

  return (
    <div className="relative">
      <UniversalGrid
        title="Catálogo de Productos"
        items={productos}
        columns={columns}
        isLoading={isLoading}
        onAdd={isEmpleado ? undefined : () => openForm()}
        onEdit={isEmpleado ? undefined : (item) => openForm(item)}
        onDelete={isEmpleado ? undefined : async (item) => {
          const result = await confirmDestructiveAction('¿Eliminar producto?', 'Esta acción no se puede deshacer y podría afectar el inventario.')
          if (result.isConfirmed) {
            eliminarProducto.mutate(undefined, {
              onSuccess: () => toast.success('Producto eliminado'),
              onError: (err: any) => toast.error(err.message)
            })
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-3">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-2">
                <h3 className="text-lg font-bold text-foreground">{item.nombre}</h3>
                <span className="text-[10px] px-1.5 py-0.5 bg-blue-500/10 text-blue-400 rounded border border-blue-500/10 uppercase">
                  {item.categoriaNombre}
                </span>
              </div>
              <div className={`text-sm font-black ${
                (item.stockActual || 0) <= (item.umbralMinimo || 0) ? 'text-red-500' : 'text-primary'
              }`}>
                {Number(item.stockActual || 0).toFixed(1)} {item.unidadMedidaNombre}
              </div>
            </div>
            
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-1.5 text-[10px] text-muted-foreground font-bold uppercase tracking-tighter">
                <Scale size={12} /> {item.unidadMedidaNombre}
              </div>
              <div className="flex items-center gap-1.5 text-[10px] text-muted-foreground font-bold uppercase tracking-tighter">
                <Tag size={12} /> Mín: {item.umbralMinimo}
              </div>
            </div>
          </div>
        )}
      />

      <AnimatePresence>
        {isFormOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={closeForm} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[60]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} transition={{ type: 'spring', damping: 25, stiffness: 200 }} className="fixed top-0 right-0 bottom-0 w-full max-w-md glass z-[70] shadow-2xl p-6 overflow-y-auto" >
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-foreground">{editingProductoId ? 'Editar Producto' : 'Nuevo Producto'}</h2>
                <button onClick={closeForm} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Nombre del Producto</label>
                  <div className="relative">
                    <Package className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input {...register('nombre')} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500/50 transition-all" placeholder="Ej. Alimento Iniciador" />
                  </div>
                  {errors.nombre && <p className="text-xs text-red-400 ml-1">{errors.nombre.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Categoría</label>
                  <select {...register('categoriaProductoId')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500/50 appearance-none">
                    <option value="">Selecciona una categoría</option>
                    {categorias.map(c => <option key={c.id} value={c.id}>{c.nombre}</option>)}
                  </select>
                  {errors.categoriaProductoId && <p className="text-xs text-red-400 ml-1">{errors.categoriaProductoId.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Unidad de Medida</label>
                  <select {...register('unidadMedidaId')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500/50 appearance-none">
                    <option value="">Selecciona unidad</option>
                    {unidades.map(u => <option key={u.id} value={u.id}>{u.nombre}</option>)}
                  </select>
                  {errors.unidadMedidaId && <p className="text-xs text-red-400 ml-1">{errors.unidadMedidaId.message}</p>}
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Stock Mínimo</label>
                    <input type="number" step="0.01" {...register('umbralMinimo', { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500/50 transition-all" />
                    {errors.umbralMinimo && <p className="text-xs text-red-400 ml-1">{errors.umbralMinimo.message}</p>}
                  </div>
                  <div className="space-y-2">
                    <label className={`text-sm font-medium ml-1 font-bold ${editingProductoId ? 'text-amber-500' : 'text-emerald-500'}`}>
                      {editingProductoId ? 'Stock Actual (Corrección)' : 'Stock Inicial (Unidades)'}
                    </label>
                    <input type="number" step="0.01" {...register('stockInicial', { valueAsNumber: true })} className={`w-full px-4 py-3 border rounded-xl text-foreground focus:outline-none focus:ring-2 transition-all font-bold ${editingProductoId ? 'bg-amber-500/5 border-amber-500/20 focus:ring-amber-500/50' : 'bg-emerald-500/5 border-emerald-500/20 focus:ring-emerald-500/50'}`} />
                    {errors.stockInicial && <p className="text-xs text-red-400 ml-1">{errors.stockInicial.message}</p>}
                  </div>
                </div>

                <AnimatePresence>
                  {esAlimento && (
                    <motion.div initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} exit={{ opacity: 0, height: 0 }} className="space-y-6 overflow-hidden">
                      <div className="space-y-2">
                        <label className={`text-sm font-medium ml-1 font-bold ${editingProductoId ? 'text-amber-600' : 'text-blue-500'}`}>
                          {editingProductoId ? 'Peso Total Actual (Kg)' : 'Peso Total Inicial (Kg)'}
                        </label>
                        <div className="relative">
                          <Info className={`absolute left-3 top-1/2 -translate-y-1/2 ${editingProductoId ? 'text-amber-600/50' : 'text-blue-500/50'}`} size={18} />
                          <input type="number" value={equivalenciaCalculada.toFixed(2)} disabled className={`w-full pl-10 pr-4 py-3 border rounded-xl font-bold focus:outline-none cursor-not-allowed ${editingProductoId ? 'bg-amber-600/5 border-amber-600/20 text-amber-600' : 'bg-blue-500/5 border-blue-500/20 text-blue-500'}`} />
                        </div>
                        <p className="text-[10px] text-muted-foreground ml-1">Cálculo: Unidades × Peso Unitario</p>
                      </div>

                      <div className="space-y-2">
                        <label className="text-sm font-medium text-muted-foreground ml-1">Peso por Unidad (Kg)</label>
                        <div className="relative">
                          <Scale className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                          <input type="number" step="0.001" {...register('pesoUnitarioKg', { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500/50 transition-all" placeholder="Ej: 50.00" />
                        </div>
                        <p className="text-[10px] text-muted-foreground ml-1">Ingrese el peso en kilogramos de una sola unidad (ej: 1 saco = 50kg). Crucial para FCR.</p>
                        {errors.pesoUnitarioKg && <p className="text-xs text-red-400 ml-1">{errors.pesoUnitarioKg.message}</p>}
                      </div>
                    </motion.div>
                  )}
                </AnimatePresence>

                <button type="submit" disabled={isPending} className="w-full py-4 bg-blue-500 hover:bg-blue-600 text-white font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-8 shadow-lg shadow-blue-500/20" >
                  <Save size={20} />
                  {editingProductoId ? 'Actualizar Producto' : 'Crear Producto'}
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
