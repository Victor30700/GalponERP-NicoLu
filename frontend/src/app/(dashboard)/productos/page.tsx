'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, Package, Tag, Scale, Info } from 'lucide-react'
import { toast } from 'sonner'
import { confirmDestructiveAction } from '@/lib/swal'

const productoSchema = z.object({
  nombre: z.string().min(3, 'El nombre es muy corto'),
  categoriaProductoId: z.string().uuid('CategorÃ­a invÃ¡lida'),
  unidadMedidaId: z.string().uuid('Unidad de medida invÃ¡lida'),
  pesoUnitarioKg: z.number().positive('La equivalencia debe ser mayor a cero'),
  umbralMinimo: z.number().min(0, 'No puede ser negativo'),
  stockInicial: z.number().min(0, 'No puede ser negativo').optional().default(0),
})

type ProductoFormValues = z.infer<typeof productoSchema>

interface Producto {
  id: string
  nombre: string
  categoriaId: string
  categoriaNombre: string
  unidadMedidaId: string
  unidadMedidaNombre: string
  pesoUnitarioKg: number
  umbralMinimo: number
  stockActual: number
  stockActualKg: number
  isActive: boolean
}

interface Categoria {
  id: string
  nombre: string
}

interface UnidadMedida {
  id: string
  nombre: string
}

export default function ProductosPage() {
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingProducto, setEditingProducto] = useState<Producto | null>(null)
  const queryClient = useQueryClient()

  // Queries
  const { data: productos = [], isLoading } = useQuery({
    queryKey: ['productos'],
    queryFn: () => api.get<Producto[]>('/api/Productos'),
  })

  const { data: categorias = [] } = useQuery({
    queryKey: ['categorias'],
    queryFn: () => api.get<Categoria[]>('/api/Categorias'),
  })

  const { data: unidades = [] } = useQuery({
    queryKey: ['unidadesMedida'],
    queryFn: () => api.get<UnidadMedida[]>('/api/UnidadesMedida'),
  })

  // Mutations
  const createMutation = useMutation({
    mutationFn: (newProd: ProductoFormValues) => api.post('/api/Productos', newProd),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] })
      toast.success('Producto creado correctamente')
      closeForm()
    },
    onError: (err: any) => toast.error(err.message),
  })

  const updateMutation = useMutation({
    mutationFn: (data: { id: string; values: ProductoFormValues }) => 
      api.put(`/api/Productos/${data.id}`, { id: data.id, ...data.values }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] })
      toast.success('Producto actualizado')
      closeForm()
    },
    onError: (err: any) => toast.error(err.message),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Productos/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['productos'] })
      toast.success('Producto eliminado')
    },
    onError: (err: any) => toast.error(err.message),
  })

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ProductoFormValues>({
    resolver: zodResolver(productoSchema),
    defaultValues: {
      nombre: '',
      categoriaProductoId: '',
      unidadMedidaId: '',
      pesoUnitarioKg: 1,
      umbralMinimo: 0
    }
  })

  const onSubmit = (data: ProductoFormValues) => {
    const values = { 
      ...data, 
      pesoUnitarioKg: Number(data.pesoUnitarioKg),
      umbralMinimo: Number(data.umbralMinimo) 
    }
    if (editingProducto) {
      updateMutation.mutate({ id: editingProducto.id, values })
    } else {
      createMutation.mutate(values)
    }
  }

  const openForm = (prod?: Producto) => {
    if (prod) {
      setEditingProducto(prod)
      reset({
        nombre: prod.nombre,
        categoriaProductoId: prod.categoriaId,
        unidadMedidaId: prod.unidadMedidaId,
        pesoUnitarioKg: prod.pesoUnitarioKg,
        umbralMinimo: prod.umbralMinimo
      })
    } else {
      setEditingProducto(null)
      reset({ nombre: '', categoriaProductoId: '', unidadMedidaId: '', pesoUnitarioKg: 1, umbralMinimo: 0 })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingProducto(null)
    reset()
  }

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
      header: 'Inventario', 
      accessor: (item: Producto) => (
        <div className="flex flex-col">
          <span className="text-xs font-bold text-emerald-500">
            Total: {Number(item.stockActualKg || 0).toFixed(2)} Kg
          </span>
          <span className="text-[10px] text-muted-foreground">
            {item.stockActual || 0} {item.unidadMedidaNombre}(s) de {item.pesoUnitarioKg}kg
          </span>
        </div>
      )
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
        title="CatÃ¡logo de Productos"
        items={productos}
        columns={columns}
        isLoading={isLoading}
        onAdd={() => openForm()}
        onEdit={(item) => openForm(item)}
        onDelete={async (item) => {
          const result = await confirmDestructiveAction('Â¿Eliminar producto?', 'Esta acciÃ³n no se puede deshacer y podrÃ­a afectar el inventario.')
          if (result.isConfirmed) {
            deleteMutation.mutate(item.id)
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-1">
            <div className="flex items-center gap-2">
              <h3 className="text-lg font-bold text-foreground">{item.nombre}</h3>
              <span className="text-[10px] px-1.5 py-0.5 bg-blue-500/10 text-blue-400 rounded border border-blue-500/10 uppercase">
                {item.categoriaNombre}
              </span>
            </div>
            <div className="flex items-center gap-4 mt-3">
              <div className="flex items-center gap-1.5 text-xs text-muted-foreground font-medium uppercase tracking-tighter">
                <Scale size={12} /> {item.unidadMedidaNombre}
              </div>
              <div className="flex items-center gap-1.5 text-xs text-muted-foreground font-medium uppercase tracking-tighter">
                <Tag size={12} /> MÃ­n: {item.umbralMinimo}
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
                <h2 className="text-2xl font-bold text-foreground">{editingProducto ? 'Editar Producto' : 'Nuevo Producto'}</h2>
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
                  <label className="text-sm font-medium text-muted-foreground ml-1">CategorÃ­a</label>
                  <select {...register('categoriaProductoId')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500/50 appearance-none">
                    <option value="">Selecciona una categorÃ­a</option>
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
                    <label className="text-sm font-medium text-muted-foreground ml-1">Stock MÃ­nimo</label>
                    <input type="number" step="0.01" {...register('umbralMinimo', { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500/50 transition-all" />
                    {errors.umbralMinimo && <p className="text-xs text-red-400 ml-1">{errors.umbralMinimo.message}</p>}
                  </div>
                  {!editingProducto && (
                    <div className="space-y-2">
                      <label className="text-sm font-medium text-muted-foreground ml-1 font-bold text-emerald-500">Stock Inicial (Unidades)</label>
                      <input type="number" step="0.01" {...register('stockInicial', { valueAsNumber: true })} className="w-full px-4 py-3 bg-emerald-500/5 border border-emerald-500/20 rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-emerald-500/50 transition-all font-bold" />
                      {errors.stockInicial && <p className="text-xs text-red-400 ml-1">{errors.stockInicial.message}</p>}
                    </div>
                  )}
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

                <button type="submit" disabled={createMutation.isPending || updateMutation.isPending} className="w-full py-4 bg-blue-500 hover:bg-blue-600 text-white font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-8 shadow-lg shadow-blue-500/20" >
                  <Save size={20} />
                  {editingProducto ? 'Actualizar Producto' : 'Crear Producto'}
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}



