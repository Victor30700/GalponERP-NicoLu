'use client'

import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { motion, AnimatePresence } from 'framer-motion'
import { 
  FileText, 
  Plus, 
  X, 
  Save, 
  Trash2, 
  Beaker,
  Scale,
  ClipboardList,
  Activity
} from 'lucide-react'
import { toast } from 'sonner'
import { useForm, useFieldArray } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { useFormulas, type Formula, type FormulaFormValues } from '@/hooks/useFormulas'
import { cn } from '@/lib/utils'

const formulaSchema = z.object({
  nombre: z.string().min(3, 'El nombre debe tener al menos 3 caracteres'),
  etapa: z.string().min(1, 'La etapa es obligatoria'),
  cantidadBase: z.number().positive('La cantidad base debe ser mayor a 0'),
  detalles: z.array(z.object({
    productoId: z.string().uuid('Producto inválido'),
    cantidadPorBase: z.number().positive('La cantidad debe ser mayor a 0')
  })).min(1, 'La fórmula debe tener al menos un ingrediente')
})

export default function FormulasPage() {
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [selectedFormula, setSelectedFormula] = useState<Formula | null>(null)
  
  const { formulas, isLoading, crearFormula, actualizarFormula, eliminarFormula } = useFormulas()
  
  const { data: productos = [] } = useQuery({
    queryKey: ['productos'],
    queryFn: () => api.get<{ id: string, nombre: string }[]>('/api/Productos')
  })

  const form = useForm<FormulaFormValues>({
    resolver: zodResolver(formulaSchema),
    defaultValues: {
      nombre: '',
      etapa: '',
      cantidadBase: 100,
      detalles: [{ productoId: '', cantidadPorBase: 0 }]
    }
  })

  const { fields, append, remove } = useFieldArray({
    control: form.control,
    name: "detalles"
  })

  const handleEdit = (formula: Formula) => {
    setSelectedFormula(formula)
    form.reset({
      nombre: formula.nombre,
      etapa: formula.etapa,
      cantidadBase: formula.cantidadBase,
      detalles: formula.detalles.map(d => ({
        productoId: d.productoId,
        cantidadPorBase: d.cantidadPorBase
      }))
    })
    setIsModalOpen(true)
  }

  const handleAdd = () => {
    setSelectedFormula(null)
    form.reset({
      nombre: '',
      etapa: '',
      cantidadBase: 100,
      detalles: [{ productoId: '', cantidadPorBase: 0 }]
    })
    setIsModalOpen(true)
  }

  const onSubmit = (data: FormulaFormValues) => {
    if (selectedFormula) {
      actualizarFormula.mutate({ id: selectedFormula.id, ...data }, {
        onSuccess: () => {
          toast.success('Fórmula actualizada con éxito')
          setIsModalOpen(false)
        },
        onError: (err: any) => toast.error(err.message)
      })
    } else {
      crearFormula.mutate(data, {
        onSuccess: () => {
          toast.success('Fórmula creada con éxito')
          setIsModalOpen(false)
        },
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const handleDelete = (formula: Formula) => {
    import('sweetalert2').then((Swal) => {
      Swal.default.fire({
        title: '¿Eliminar fórmula?',
        text: `Esta acción desactivará la receta "${formula.nombre}".`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ef4444',
        cancelButtonColor: '#64748b',
        confirmButtonText: 'Sí, eliminar',
        background: '#1e293b',
        color: '#f8fafc'
      }).then((result) => {
        if (result.isConfirmed) {
          eliminarFormula.mutate(formula.id, {
            onSuccess: () => toast.success('Fórmula eliminada'),
            onError: (err: any) => toast.error(err.message)
          })
        }
      })
    })
  }

  return (
    <div className="space-y-6">
      <UniversalGrid
        title="Fórmulas de Nutrición"
        items={formulas}
        isLoading={isLoading}
        onAdd={handleAdd}
        onEdit={handleEdit}
        onDelete={handleDelete}
        searchPlaceholder="Buscar receta..."
        columns={[
          {
            header: 'Fórmula',
            accessor: (item) => (
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-xl bg-purple-500/10 flex items-center justify-center text-purple-400">
                  <Beaker size={20} />
                </div>
                <div>
                  <p className="font-bold text-foreground">{item.nombre}</p>
                  <p className="text-[10px] text-muted-foreground uppercase font-black tracking-widest">{item.etapa}</p>
                </div>
              </div>
            )
          },
          {
            header: 'Base',
            accessor: (item) => (
              <div className="flex flex-col">
                <span className="font-bold text-foreground">{item.cantidadBase} Kg</span>
                <span className="text-[10px] text-muted-foreground uppercase font-bold">Cantidad de mezcla</span>
              </div>
            )
          },
          {
            header: 'Ingredientes',
            accessor: (item) => (
              <div className="flex -space-x-2">
                {item.detalles.slice(0, 3).map((d, i) => (
                  <div key={i} title={d.productoNombre} className="w-8 h-8 rounded-full bg-slate-800 border-2 border-slate-900 flex items-center justify-center text-[10px] font-bold text-slate-400">
                    {d.productoNombre.substring(0, 2).toUpperCase()}
                  </div>
                ))}
                {item.detalles.length > 3 && (
                  <div className="w-8 h-8 rounded-full bg-slate-700 border-2 border-slate-900 flex items-center justify-center text-[10px] font-bold text-white">
                    +{item.detalles.length - 3}
                  </div>
                )}
              </div>
            )
          },
          {
            header: 'Estado',
            accessor: (item) => (
              <span className={cn(
                "px-2 py-1 rounded text-[10px] font-bold uppercase",
                item.isActive ? "bg-emerald-500/10 text-emerald-400" : "bg-red-500/10 text-red-400"
              )}>
                {item.isActive ? 'Activa' : 'Inactiva'}
              </span>
            )
          }
        ]}
        renderMobileCard={(item) => (
          <div className="space-y-4">
            <div className="flex items-center gap-3">
              <div className="w-12 h-12 rounded-2xl bg-purple-500/10 flex items-center justify-center text-purple-400">
                <Beaker size={24} />
              </div>
              <div>
                <h3 className="text-lg font-bold text-foreground">{item.nombre}</h3>
                <p className="text-xs text-muted-foreground uppercase font-black tracking-widest">{item.etapa}</p>
              </div>
            </div>
            <div className="grid grid-cols-2 gap-3">
              <div className="p-3 bg-muted/50 rounded-xl border border-border">
                <p className="text-[10px] text-muted-foreground font-bold uppercase mb-1">Base de Mezcla</p>
                <p className="text-lg font-black text-foreground">{item.cantidadBase} Kg</p>
              </div>
              <div className="p-3 bg-muted/50 rounded-xl border border-border">
                <p className="text-[10px] text-muted-foreground font-bold uppercase mb-1">Ingredientes</p>
                <p className="text-lg font-black text-foreground">{item.detalles.length}</p>
              </div>
            </div>
          </div>
        )}
      />

      <AnimatePresence>
        {isModalOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={() => setIsModalOpen(false)} className="fixed inset-0 bg-black/60 backdrop-blur-sm z-[100]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} className="fixed top-0 right-0 bottom-0 w-full max-w-2xl glass z-[110] shadow-2xl p-8 overflow-y-auto">
              <div className="flex items-center justify-between mb-8">
                <div>
                  <h2 className="text-2xl font-bold text-foreground">{selectedFormula ? 'Editar Fórmula' : 'Nueva Fórmula'}</h2>
                  <p className="text-xs text-muted-foreground uppercase font-black tracking-widest mt-1">Define las proporciones de los insumos</p>
                </div>
                <button onClick={() => setIsModalOpen(false)} className="p-2 bg-muted/50 rounded-full text-muted-foreground"><X size={20} /></button>
              </div>

              <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Nombre de la Receta</label>
                    <input {...form.register('nombre')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground" placeholder="Ej. Mezcla Inicio con Vitaminas" />
                    {form.formState.errors.nombre && <p className="text-xs text-red-400">{form.formState.errors.nombre.message}</p>}
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-muted-foreground ml-1">Etapa Productiva</label>
                    <select {...form.register('etapa')} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground appearance-none">
                      <option value="">Seleccionar etapa</option>
                      <option value="Iniciación">Iniciación</option>
                      <option value="Crecimiento">Crecimiento</option>
                      <option value="Engorde">Engorde</option>
                      <option value="Finalización">Finalización</option>
                    </select>
                  </div>
                </div>

                <div className="p-4 bg-purple-500/5 rounded-2xl border border-purple-500/20 flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-xl bg-purple-500/10 flex items-center justify-center text-purple-400">
                      <Scale size={20} />
                    </div>
                    <div>
                      <p className="text-sm font-bold text-foreground">Cantidad Base de la Mezcla</p>
                      <p className="text-[10px] text-muted-foreground uppercase font-bold">Proporción para cálculos</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-2">
                    <input type="number" step="0.01" {...form.register('cantidadBase', { valueAsNumber: true })} className="w-24 px-3 py-2 bg-muted/50 border border-border rounded-lg text-right font-bold text-foreground" />
                    <span className="text-sm font-bold text-muted-foreground">Kg</span>
                  </div>
                </div>

                <div className="space-y-4">
                  <div className="flex items-center justify-between">
                    <h3 className="text-sm font-black text-muted-foreground uppercase tracking-widest flex items-center gap-2">
                      <ClipboardList size={16} className="text-purple-400" /> Ingredientes y Proporciones
                    </h3>
                    <button type="button" onClick={() => append({ productoId: '', cantidadPorBase: 0 })} className="text-xs font-bold text-purple-400 hover:text-purple-300 transition-colors flex items-center gap-1">
                      <Plus size={14} /> Agregar
                    </button>
                  </div>

                  <div className="space-y-3">
                    {fields.map((field, index) => (
                      <div key={field.id} className="grid grid-cols-1 md:grid-cols-12 gap-3 p-3 bg-muted/30 rounded-xl border border-border relative group">
                        <div className="md:col-span-7">
                          <select {...form.register(`detalles.${index}.productoId`)} className="w-full px-3 py-2 bg-muted/50 border border-border rounded-lg text-sm text-foreground appearance-none">
                            <option value="">Seleccionar Producto</option>
                            {productos.map(p => <option key={p.id} value={p.id}>{p.nombre}</option>)}
                          </select>
                        </div>
                        <div className="md:col-span-3 flex items-center gap-2">
                          <input type="number" step="0.0001" {...form.register(`detalles.${index}.cantidadPorBase`, { valueAsNumber: true })} className="w-full px-3 py-2 bg-muted/50 border border-border rounded-lg text-sm text-right font-bold text-foreground" placeholder="Cant." />
                        </div>
                        <div className="md:col-span-2 flex justify-end">
                          <button type="button" onClick={() => remove(index)} className="p-2 text-muted-foreground hover:text-red-400 transition-colors">
                            <Trash2 size={16} />
                          </button>
                        </div>
                      </div>
                    ))}
                  </div>
                  {form.formState.errors.detalles && <p className="text-xs text-red-400">{form.formState.errors.detalles.message}</p>}
                </div>

                <div className="pt-4 border-t border-border">
                  <button type="submit" disabled={crearFormula.isPending || actualizarFormula.isPending} className="w-full py-4 bg-purple-500 hover:bg-purple-600 text-white font-bold rounded-2xl transition-all shadow-lg shadow-purple-500/20 flex items-center justify-center gap-2">
                    <Save size={20} /> {selectedFormula ? 'Actualizar Receta' : 'Guardar Receta'}
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
