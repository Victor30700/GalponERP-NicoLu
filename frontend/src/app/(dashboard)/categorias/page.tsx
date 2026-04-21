'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid, Column } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, Tag, Info } from 'lucide-react'
import { toast } from 'sonner'
import { confirmDestructiveAction, showSuccessAlert } from '@/lib/swal'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/lib/rbac'

import { TipoCategoria } from '@/hooks/useCategorias'

const categoriaSchema = z.object({
  nombre: z.string().min(3, 'El nombre debe tener al menos 3 caracteres'),
  descripcion: z.string().optional().nullable(),
  tipo: z.number().int().min(0).max(4).default(4),
})

type CategoriaFormValues = z.infer<typeof categoriaSchema>

interface Categoria {
  id: string
  nombre: string
  descripcion?: string | null
  tipo: TipoCategoria
}

export default function CategoriasPage() {
  const { profile } = useAuth()
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null
  const isEmpleado = userRole === UserRole.Empleado

  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingCategoria, setEditingCategoria] = useState<Categoria | null>(null)
  const queryClient = useQueryClient()

  // Queries
  const { data: categorias = [], isLoading } = useQuery({
    queryKey: ['categorias'],
    queryFn: () => api.get<Categoria[]>('/api/Categorias'),
  })

  // Mutations
  const createMutation = useMutation({
    mutationFn: (newCat: CategoriaFormValues) => api.post('/api/Categorias', newCat),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] })
      showSuccessAlert('¡Éxito!', 'Categoría creada correctamente')
      closeForm()
    },
    onError: (err: any) => toast.error(err.message),
  })

  const updateMutation = useMutation({
    mutationFn: (data: { id: string; values: CategoriaFormValues }) => 
      api.put(`/api/Categorias/${data.id}`, { id: data.id, ...data.values }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] })
      showSuccessAlert('¡Actualizado!', 'La categoría se ha actualizado correctamente')
      closeForm()
    },
    onError: (err: any) => toast.error(err.message),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Categorias/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categorias'] })
      showSuccessAlert('¡Eliminado!', 'La categoría ha sido eliminada')
    },
    onError: (err: any) => toast.error(err.message),
  })

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CategoriaFormValues>({
    resolver: zodResolver(categoriaSchema),
  })

  const onSubmit = (data: CategoriaFormValues) => {
    if (editingCategoria) {
      updateMutation.mutate({ id: editingCategoria.id, values: data })
    } else {
      createMutation.mutate(data)
    }
  }

  const openForm = (cat?: Categoria) => {
    if (cat) {
      setEditingCategoria(cat)
      reset({
        nombre: cat.nombre,
        descripcion: cat.descripcion || '',
        tipo: cat.tipo
      })
    } else {
      setEditingCategoria(null)
      reset({ nombre: '', descripcion: '', tipo: 4 })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingCategoria(null)
    reset()
  }

  const getTipoBadge = (tipo: number) => {
    const tipos = ['Otros', 'Alimento', 'Medicamento', 'Vacuna', 'Insumo', 'Activo Fijo']
    const colors = [
      'bg-slate-500/10 text-slate-400',
      'bg-blue-500/10 text-blue-400',
      'bg-red-500/10 text-red-400',
      'bg-purple-500/10 text-purple-400',
      'bg-amber-500/10 text-amber-400',
      'bg-emerald-500/10 text-emerald-400'
    ]
    return <span className={`px-2 py-0.5 rounded text-[10px] font-black uppercase tracking-widest ${colors[tipo] || colors[0]}`}>{tipos[tipo] || 'Otros'}</span>
  }

  const columns: Column<Categoria>[] = [
    { header: 'Nombre', accessor: 'nombre' },
    { header: 'Tipo', accessor: (item) => getTipoBadge(item.tipo) },
    { header: 'Descripción', accessor: 'descripcion' },
  ]

  return (
    <div className="relative">
      <UniversalGrid
        title="Gestión de Categorías"
        items={categorias}
        columns={columns}
        isLoading={isLoading}
        onAdd={isEmpleado ? undefined : () => openForm()}
        onEdit={isEmpleado ? undefined : (item) => openForm(item as Categoria)}
        onDelete={isEmpleado ? undefined : async (item) => {
          const result = await confirmDestructiveAction(
            '¿Eliminar categoría?',
            `¿Estás seguro de que deseas eliminar la categoría "${(item as Categoria).nombre}"? Esta acción no se puede deshacer.`
          )
          if (result.isConfirmed) {
            deleteMutation.mutate((item as Categoria).id)
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-2">
            <div className="flex items-center justify-between">
              <h3 className="text-lg font-bold text-foreground">{(item as Categoria).nombre}</h3>
              {getTipoBadge((item as Categoria).tipo)}
            </div>
            <p className="text-sm text-muted-foreground">{(item as Categoria).descripcion}</p>
          </div>
        )}
      />

      <AnimatePresence>
        {isFormOpen && (
          <>
            <motion.div 
              initial={{ opacity: 0 }} 
              animate={{ opacity: 1 }} 
              exit={{ opacity: 0 }} 
              onClick={closeForm} 
              className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[60]" 
            />
            <motion.div 
              initial={{ x: '100%' }} 
              animate={{ x: 0 }} 
              exit={{ x: '100%' }} 
              transition={{ type: 'spring', damping: 25, stiffness: 200 }} 
              className="fixed top-0 right-0 bottom-0 w-full max-w-md glass z-[70] shadow-2xl p-6 overflow-y-auto"
            >
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-foreground">{editingCategoria ? 'Editar Categoría' : 'Nueva Categoría'}</h2>
                <button onClick={closeForm} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-300 flex items-center gap-2">
                    <Tag size={16} className="text-primary" /> Nombre
                  </label>
                  <input
                    {...register('nombre')}
                    className="w-full bg-muted/50 border border-border rounded-xl px-4 py-3 text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    placeholder="Ej. Alimento, Medicamento..."
                  />
                  {errors.nombre && <p className="text-xs text-red-400 mt-1">{errors.nombre.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-300 flex items-center gap-2">
                    <Info size={16} className="text-primary" /> Tipo de Categoría
                  </label>
                  <select
                    {...register('tipo', { valueAsNumber: true })}
                    className="w-full bg-muted/50 border border-border rounded-xl px-4 py-3 text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all appearance-none"
                  >
                    <option value={0}>Otros</option>
                    <option value={1}>Alimento (Calcula FCR)</option>
                    <option value={2}>Medicamento (Activa Retiro)</option>
                    <option value={3}>Vacuna (Activa Retiro)</option>
                    <option value={4}>Insumo (General)</option>
                    <option value={5}>Activo Fijo</option>
                  </select>
                  <p className="text-[10px] text-muted-foreground italic ml-1">El tipo define comportamientos automáticos del sistema (ej. bloqueo de ventas).</p>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-300 flex items-center gap-2">
                    <Info size={16} className="text-primary" /> Descripción
                  </label>
                  <textarea
                    {...register('descripcion')}
                    rows={4}
                    className="w-full bg-muted/50 border border-border rounded-xl px-4 py-3 text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all resize-none"
                    placeholder="Opcional: Breve descripción de la categoría"
                  />
                </div>

                <div className="pt-4 flex gap-3">
                  <button
                    type="button"
                    onClick={closeForm}
                    className="flex-1 px-4 py-3 rounded-xl border border-border text-foreground font-medium hover:bg-muted/50 transition-all"
                  >
                    Cancelar
                  </button>
                  <button
                    type="submit"
                    disabled={createMutation.isPending || updateMutation.isPending}
                    className="flex-[2] bg-primary text-primary-foreground px-4 py-3 rounded-xl font-bold flex items-center justify-center gap-2 hover:opacity-90 transition-all disabled:opacity-50"
                  >
                    <Save size={20} />
                    {editingCategoria ? 'Actualizar' : 'Guardar Categoría'}
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


