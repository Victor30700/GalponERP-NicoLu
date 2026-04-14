'use client'

import { useState } from 'react'
import { useUnidadesMedida, useUnidadMedida, type UnidadMedida } from '@/hooks/useUnidadesMedida'
import { UniversalGrid, Column } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, Ruler, Type } from 'lucide-react'
import { toast } from 'sonner'
import { confirmDestructiveAction } from '@/lib/swal'

const unidadSchema = z.object({
  nombre: z.string().min(2, 'El nombre es muy corto'),
  abreviatura: z.string().min(1, 'La abreviatura es requerida'),
})

type UnidadFormValues = z.infer<typeof unidadSchema>

export default function UnidadesMedidaPage() {
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingUnidad, setEditingUnidad] = useState<UnidadMedida | null>(null)
  
  const { unidades, isLoading, createUnidad, refresh } = useUnidadesMedida()
  const { updateUnidad, deleteUnidad } = useUnidadMedida(editingUnidad?.id || '')

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<UnidadFormValues>({
    resolver: zodResolver(unidadSchema),
  })

  const onSubmit = (data: UnidadFormValues) => {
    if (editingUnidad) {
      updateUnidad.mutate({ id: editingUnidad.id, ...data }, {
        onSuccess: () => {
          toast.success('Unidad actualizada correctamente')
          closeForm()
        },
        onError: (err: any) => toast.error(err.message)
      })
    } else {
      createUnidad.mutate(data, {
        onSuccess: () => {
          toast.success('Unidad creada correctamente')
          closeForm()
        },
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const openForm = (unidad?: UnidadMedida) => {
    if (unidad) {
      setEditingUnidad(unidad)
      reset({
        nombre: unidad.nombre,
        abreviatura: unidad.abreviatura
      })
    } else {
      setEditingUnidad(null)
      reset({ nombre: '', abreviatura: '' })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingUnidad(null)
    reset()
  }

  const columns: Column<UnidadMedida>[] = [
    { 
      header: 'Nombre', 
      accessor: (item: UnidadMedida) => (
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-lg bg-blue-500/10 flex items-center justify-center text-blue-500 font-bold">
            <Ruler size={14} />
          </div>
          <span className="font-medium text-white">{item.nombre}</span>
        </div>
      )
    },
    { 
        header: 'Abreviatura', 
        accessor: (item: UnidadMedida) => (
            <span className="px-2 py-1 bg-white/5 rounded text-xs font-mono text-slate-400 border border-white/5">
                {item.abreviatura}
            </span>
        ) 
    },
  ]

  return (
    <div className="relative">
      <UniversalGrid
        title="Unidades de Medida"
        items={unidades}
        columns={columns}
        isLoading={isLoading}
        onAdd={() => openForm()}
        onEdit={(item) => openForm(item)}
        onDelete={async (item) => {
          const result = await confirmDestructiveAction(
            '¿Eliminar unidad de medida?', 
            'Esto podría afectar a los productos que usan esta unidad.'
          )
          if (result.isConfirmed) {
            deleteUnidad.mutate(undefined, {
                onSuccess: () => {
                    toast.success('Unidad eliminada')
                    refresh()
                },
                onError: (err: any) => toast.error(err.message)
            })
          }
        }}
        renderMobileCard={(item) => (
          <div className="flex justify-between items-center">
            <div>
              <h3 className="text-lg font-bold text-white">{item.nombre}</h3>
              <p className="text-sm text-blue-400 font-mono">{item.abreviatura}</p>
            </div>
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
              className="fixed top-0 right-0 bottom-0 w-full max-w-md glass-dark z-[70] shadow-2xl p-6 overflow-y-auto"
            >
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-white">
                  {editingUnidad ? 'Editar Unidad' : 'Nueva Unidad'}
                </h2>
                <button onClick={closeForm} className="p-2 bg-white/5 rounded-full text-slate-400">
                  <X size={20} />
                </button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Nombre Completo</label>
                  <div className="relative">
                    <Type className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input
                      {...register('nombre')}
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500/50 transition-all"
                      placeholder="Ej. Kilogramos"
                    />
                  </div>
                  {errors.nombre && <p className="text-xs text-red-400 ml-1">{errors.nombre.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Abreviatura</label>
                  <div className="relative">
                    <Ruler className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input
                      {...register('abreviatura')}
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-blue-500/50 transition-all font-mono"
                      placeholder="Ej. kg"
                    />
                  </div>
                  {errors.abreviatura && <p className="text-xs text-red-400 ml-1">{errors.abreviatura.message}</p>}
                </div>

                <button
                  type="submit"
                  disabled={createUnidad.isPending || updateUnidad.isPending}
                  className="w-full py-4 bg-blue-500 hover:bg-blue-600 text-white font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-8 shadow-lg shadow-blue-500/20"
                >
                  <Save size={20} />
                  {editingUnidad ? 'Guardar Cambios' : 'Crear Unidad'}
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
