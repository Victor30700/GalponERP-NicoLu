'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid, Column } from '@/components/shared/UniversalGrid'
import { useForm, useFieldArray } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, ClipboardList, Plus, Trash2, Calendar, Activity, Package } from 'lucide-react'
import { toast } from 'sonner'
import { confirmDestructiveAction } from '@/lib/swal'

// Tipos de actividad mapeados según la API (1=Vacuna, etc.)
const TIPO_ACTIVIDAD = [
  { id: 1, nombre: 'Vacuna' },
  { id: 2, nombre: 'Medicación' },
  { id: 3, nombre: 'Suplemento' },
  { id: 4, nombre: 'Limpieza/Desinfección' },
  { id: 5, nombre: 'Otro' },
]

const actividadSchema = z.object({
  tipo: z.number().min(1, 'Selecciona un tipo'),
  diaDeAplicacion: z.number().min(0, 'Día inválido'),
  descripcion: z.string().min(3, 'Descripción muy corta'),
  productoIdRecomendado: z.string().uuid('Producto inválido').nullable().optional(),
  cantidadRecomendada: z.number().min(0, 'No puede ser negativa').default(0),
})

const plantillaSchema = z.object({
  nombre: z.string().min(3, 'El nombre es muy corto'),
  descripcion: z.string().min(3, 'La descripción es muy corta'),
  actividades: z.array(actividadSchema).min(1, 'Agrega al menos una actividad'),
})

type PlantillaFormValues = z.infer<typeof plantillaSchema>

interface ActividadRead {
  id: string
  tipoActividad: string
  diaDeAplicacion: number
  descripcion: string
  productoIdRecomendado: string | null
  cantidadRecomendada: number
}

interface Plantilla {
  id: string
  nombre: string
  descripcion: string
  actividades: ActividadRead[]
}

interface Producto {
  id: string
  nombre: string
}

export default function PlantillasPage() {
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingPlantilla, setEditingPlantilla] = useState<Plantilla | null>(null)
  const queryClient = useQueryClient()

  const { data: plantillas = [], isLoading } = useQuery({
    queryKey: ['plantillas'],
    queryFn: () => api.get<Plantilla[]>('/api/Plantillas'),
  })

  const { data: productos = [] } = useQuery({
    queryKey: ['productos-simple'],
    queryFn: () => api.get<Producto[]>('/api/Productos'),
  })

  const { register, control, handleSubmit, reset, formState: { errors } } = useForm<PlantillaFormValues>({
    resolver: zodResolver(plantillaSchema),
    defaultValues: {
      actividades: [{ tipo: 1, diaDeAplicacion: 1, descripcion: '', productoIdRecomendado: null }]
    }
  })

  const { fields, append, remove } = useFieldArray({
    control,
    name: "actividades"
  })

  const createMutation = useMutation({
    mutationFn: (newPlantilla: PlantillaFormValues) => api.post('/api/Plantillas', newPlantilla),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plantillas'] })
      toast.success('Plantilla creada correctamente')
      closeForm()
    },
    onError: (err: any) => toast.error(err.message),
  })

  const updateMutation = useMutation({
    mutationFn: (data: { id: string; values: PlantillaFormValues }) => 
      api.put(`/api/Plantillas/${data.id}`, { id: data.id, ...data.values }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plantillas'] })
      toast.success('Plantilla actualizada')
      closeForm()
    },
    onError: (err: any) => toast.error(err.message),
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Plantillas/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['plantillas'] })
      toast.success('Plantilla eliminada')
    },
    onError: (err: any) => toast.error(err.message),
  })

  const onSubmit = (values: PlantillaFormValues) => {
    const formattedValues = {
      ...values,
      actividades: values.actividades.map(a => ({
        ...a,
        productoIdRecomendado: a.productoIdRecomendado === "" ? null : a.productoIdRecomendado
      }))
    }
    if (editingPlantilla) {
      updateMutation.mutate({ id: editingPlantilla.id, values: formattedValues as any })
    } else {
      createMutation.mutate(formattedValues as any)
    }
  }

  const openForm = (plantilla?: Plantilla) => {
    if (plantilla) {
      setEditingPlantilla(plantilla)
      reset({
        nombre: plantilla.nombre,
        descripcion: plantilla.descripcion,
        actividades: plantilla.actividades.map(a => ({
          tipo: TIPO_ACTIVIDAD.find(t => t.nombre === a.tipoActividad)?.id || 1,
          diaDeAplicacion: a.diaDeAplicacion,
          descripcion: a.descripcion,
          productoIdRecomendado: a.productoIdRecomendado,
          cantidadRecomendada: a.cantidadRecomendada
        }))
      })
    } else {
      setEditingPlantilla(null)
      reset({ nombre: '', descripcion: '', actividades: [{ tipo: 1, diaDeAplicacion: 1, descripcion: '', productoIdRecomendado: null, cantidadRecomendada: 0 }] })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingPlantilla(null)
    reset()
  }

  const columns: Column<Plantilla>[] = [
    { 
      header: 'Plantilla', 
      accessor: (item: Plantilla) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-purple-500/10 flex items-center justify-center text-purple-500">
            <ClipboardList size={20} />
          </div>
          <div>
            <span className="font-bold text-foreground block">{item.nombre}</span>
            <span className="text-xs text-muted-foreground">{item.actividades.length} actividades programadas</span>
          </div>
        </div>
      )
    },
    { header: 'Descripción', accessor: 'descripcion' },
  ]

  return (
    <div className="relative">
      <UniversalGrid
        title="Protocolos y Plantillas"
        items={plantillas}
        columns={columns}
        isLoading={isLoading}
        onAdd={() => openForm()}
        onEdit={(item) => openForm(item)}
        onDelete={async (item) => {
          const result = await confirmDestructiveAction('¿Eliminar plantilla?', 'Esta plantilla ya no podrá asignarse a nuevos lotes.')
          if (result.isConfirmed) {
            deleteMutation.mutate(item.id)
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-2">
            <h3 className="text-lg font-bold text-foreground">{item.nombre}</h3>
            <p className="text-sm text-muted-foreground">{item.descripcion}</p>
            <div className="flex items-center gap-2 mt-2">
              <span className="px-2 py-1 bg-purple-500/10 text-purple-400 rounded-md text-[10px] font-bold uppercase tracking-wider">
                {item.actividades.length} Actividades
              </span>
            </div>
          </div>
        )}
      />

      <AnimatePresence>
        {isFormOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={closeForm} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[60]" />
            <motion.div initial={{ x: '100%' }} animate={{ x: 0 }} exit={{ x: '100%' }} transition={{ type: 'spring', damping: 30, stiffness: 300 }} className="fixed top-0 right-0 bottom-0 w-full max-w-2xl glass z-[70] shadow-2xl p-8 overflow-y-auto" >
              <div className="flex items-center justify-between mb-10">
                <div>
                  <h2 className="text-3xl font-black text-foreground">{editingPlantilla ? 'Editar Protocolo' : 'Nuevo Protocolo'}</h2>
                  <p className="text-purple-500 font-bold uppercase tracking-widest text-xs mt-1">Estandarización de Manejo</p>
                </div>
                <button onClick={closeForm} className="p-3 bg-muted/50 rounded-2xl text-muted-foreground hover:text-foreground transition-all"><X size={24} /></button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-8">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div className="space-y-2">
                    <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Nombre de la Plantilla</label>
                    <input {...register('nombre')} className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-bold focus:outline-none focus:ring-2 focus:ring-purple-500/50 transition-all placeholder:text-slate-700" placeholder="Ej. Lote Cobb 500 - Invierno" />
                    {errors.nombre && <p className="text-xs text-red-400 font-bold ml-1">{errors.nombre.message}</p>}
                  </div>
                  <div className="space-y-2">
                    <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Descripción Breve</label>
                    <input {...register('descripcion')} className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-bold focus:outline-none focus:ring-2 focus:ring-purple-500/50 transition-all placeholder:text-slate-700" placeholder="Ej. Protocolo estándar de vacunación" />
                    {errors.descripcion && <p className="text-xs text-red-400 font-bold ml-1">{errors.descripcion.message}</p>}
                  </div>
                </div>

                <div className="space-y-4">
                  <div className="flex items-center justify-between">
                    <h3 className="text-lg font-bold text-foreground flex items-center gap-2">
                      <Activity size={20} className="text-purple-500" />
                      Cronograma de Actividades
                    </h3>
                    <button type="button" onClick={() => append({ tipo: 1, diaDeAplicacion: 1, descripcion: '', productoIdRecomendado: null, cantidadRecomendada: 0 })} className="flex items-center gap-2 px-4 py-2 bg-purple-500/10 text-purple-400 hover:bg-purple-500/20 rounded-xl text-xs font-bold transition-all" >
                      <Plus size={16} /> AGREGAR DÍA
                    </button>
                  </div>

                  <div className="space-y-4">
                    {fields.map((field, index) => (
                      <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} key={field.id} className="p-6 bg-muted/50 rounded-[2rem] border border-border relative group" >
                        <button type="button" onClick={() => remove(index)} className="absolute -top-2 -right-2 p-2 bg-red-500 text-white rounded-full opacity-0 group-hover:opacity-100 transition-all shadow-lg active:scale-90" >
                          <Trash2 size={14} />
                        </button>

                        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                          <div className="space-y-2">
                            <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Tipo</label>
                            <select {...register(`actividades.${index}.tipo`, { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground text-sm font-bold focus:ring-2 focus:ring-purple-500/50 appearance-none">
                              {TIPO_ACTIVIDAD.map(t => <option key={t.id} value={t.id}>{t.nombre}</option>)}
                            </select>
                          </div>
                          <div className="space-y-2">
                            <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Día</label>
                            <div className="relative">
                              <Calendar className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={16} />
                              <input type="number" {...register(`actividades.${index}.diaDeAplicacion`, { valueAsNumber: true })} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground text-sm font-bold focus:ring-2 focus:ring-purple-500/50" placeholder="Día" />
                            </div>
                          </div>
                          <div className="space-y-2">
                            <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Producto</label>
                            <div className="relative">
                              <Package className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={16} />
                              <select {...register(`actividades.${index}.productoIdRecomendado`)} className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground text-sm font-bold focus:ring-2 focus:ring-purple-500/50 appearance-none">
                                <option value="">Ninguno</option>
                                {productos.map(p => <option key={p.id} value={p.id}>{p.nombre}</option>)}
                              </select>
                            </div>
                          </div>
                          <div className="space-y-2">
                            <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Cant. Sugerida</label>
                            <input type="number" step="0.01" {...register(`actividades.${index}.cantidadRecomendada`, { valueAsNumber: true })} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground text-sm font-bold focus:ring-2 focus:ring-purple-500/50" placeholder="Cant." />
                          </div>
                        </div>

                        <div className="mt-4 space-y-2">
                          <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Instrucciones de la Actividad</label>
                          <textarea {...register(`actividades.${index}.descripcion`)} rows={2} className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground text-sm focus:ring-2 focus:ring-purple-500/50 transition-all placeholder:text-slate-700" placeholder="Detalla los pasos a seguir..." />
                        </div>
                      </motion.div>
                    ))}
                  </div>
                  {errors.actividades && <p className="text-xs text-red-400 font-bold text-center mt-2">{errors.actividades.message}</p>}
                </div>

                <div className="pt-8">
                  <button type="submit" disabled={createMutation.isPending || updateMutation.isPending} className="w-full py-5 bg-purple-600 hover:bg-purple-500 text-white font-black rounded-3xl transition-all flex items-center justify-center gap-3 disabled:opacity-50 shadow-xl shadow-purple-600/20 active:scale-95" >
                    <Save size={24} />
                    {editingPlantilla ? 'GUARDAR CAMBIOS' : 'REGISTRAR PLANTILLA'}
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



