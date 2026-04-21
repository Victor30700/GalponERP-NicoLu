'use client'

import { useState } from 'react'
import { useGalpones, useGalpon, Galpon } from '@/hooks/useGalpones'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { Warehouse, MapPin, Users, X, Save } from 'lucide-react'
import { toast } from 'sonner'
import { confirmDestructiveAction } from '@/lib/swal'

const galponSchema = z.object({
  nombre: z.string().min(3, 'El nombre es muy corto'),
  capacidad: z.number().min(1, 'Capacidad mínima 1'),
  ubicacion: z.string().min(3, 'Ubicación inválida'),
})

type GalponFormValues = z.infer<typeof galponSchema>

export default function GalponesPage() {
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingGalponId, setEditingGalponId] = useState<string | null>(null)
  
  const { galpones, isLoading, crearGalpon } = useGalpones()
  const { actualizarGalpon, eliminarGalpon } = useGalpon(editingGalponId || '')

  const { register, handleSubmit, reset, formState: { errors } } = useForm<GalponFormValues>({
    resolver: zodResolver(galponSchema),
  })

  const onSubmit = (data: GalponFormValues) => {
    const values = { ...data, capacidad: Number(data.capacidad) }
    if (editingGalponId) {
      const existingGalpon = galpones.find(g => g.id === editingGalponId)
      actualizarGalpon.mutate({ 
        ...existingGalpon, 
        ...values, 
        id: editingGalponId,
        isActive: existingGalpon?.isActive ?? true 
      } as Galpon, {
        onSuccess: () => {
          toast.success('Galpón actualizado')
          closeForm()
        },
        onError: (err: any) => toast.error(err.message)
      })
    } else {
      crearGalpon.mutate(values, {
        onSuccess: () => {
          toast.success('Galpón creado correctamente')
          closeForm()
        },
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const openForm = (galpon?: Galpon) => {
    if (galpon) {
      setEditingGalponId(galpon.id)
      reset(galpon)
    } else {
      setEditingGalponId(null)
      reset({ nombre: '', capacidad: 0, ubicacion: '' })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingGalponId(null)
    reset()
  }

  const columns = [
    { 
      header: 'Galpón', 
      accessor: (item: Galpon) => (
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-emerald-500/10 flex items-center justify-center text-emerald-500">
            <Warehouse size={20} />
          </div>
          <span className="font-bold text-foreground text-lg">{item.nombre}</span>
        </div>
      )
    },
    { 
      header: 'Capacidad', 
      accessor: (item: Galpon) => (
        <div className="flex items-center gap-2">
          <Users size={16} className="text-muted-foreground" />
          <span className="text-slate-300 font-medium">{item.capacidad.toLocaleString()} aves</span>
        </div>
      )
    },
    { 
      header: 'Ubicación', 
      accessor: (item: Galpon) => (
        <div className="flex items-center gap-2">
          <MapPin size={16} className="text-muted-foreground" />
          <span className="text-muted-foreground">{item.ubicacion}</span>
        </div>
      )
    },
  ]

  const isPending = crearGalpon.isPending || actualizarGalpon.isPending

  return (
    <div className="relative">
      <UniversalGrid
        title="Infraestructura: Galpones"
        items={galpones}
        columns={columns}
        isLoading={isLoading}
        onAdd={() => openForm()}
        onEdit={(item) => openForm(item)}
        onDelete={async (item) => {
          const result = await confirmDestructiveAction('¿Eliminar galpón?', 'Los lotes asociados podrían verse afectados.')
          if (result.isConfirmed) {
            eliminarGalpon.mutate(undefined, {
              onSuccess: () => toast.success('Galpón eliminado'),
              onError: (err: any) => toast.error(err.message)
            })
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-3">
            <div className="flex items-center gap-3">
              <div className="w-12 h-12 rounded-2xl bg-emerald-500/10 flex items-center justify-center text-emerald-500 border border-emerald-500/20">
                <Warehouse size={24} />
              </div>
              <h3 className="text-xl font-black text-foreground">{item.nombre}</h3>
            </div>
            <div className="grid grid-cols-2 gap-2 mt-4">
              <div className="p-3 bg-muted/50 rounded-xl border border-border">
                <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider mb-1">Capacidad</p>
                <p className="text-foreground font-bold">{(item.capacidad || 0).toLocaleString()}</p>
              </div>
              <div className="p-3 bg-muted/50 rounded-xl border border-border">
                <p className="text-[10px] text-muted-foreground uppercase font-bold tracking-wider mb-1">Ubicación</p>
                <p className="text-foreground font-bold truncate">{item.ubicacion}</p>
              </div>
            </div>
          </div>
        )}
      />

      <AnimatePresence>
        {isFormOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} onClick={closeForm} className="fixed inset-0 bg-black/80 backdrop-blur-md z-[60]" />
            <motion.div initial={{ y: '100%' }} animate={{ y: 0 }} exit={{ y: '100%' }} transition={{ type: 'spring', damping: 30, stiffness: 300 }} className="fixed bottom-0 left-0 right-0 md:top-0 md:right-0 md:left-auto md:w-full md:max-w-md glass z-[70] shadow-2xl p-8 rounded-t-[2.5rem] md:rounded-none overflow-y-auto" >
              <div className="flex items-center justify-between mb-10">
                <div>
                  <h2 className="text-3xl font-black text-foreground">{editingGalponId ? 'Editar' : 'Nuevo'}</h2>
                  <p className="text-emerald-500 font-bold uppercase tracking-widest text-xs mt-1">Infraestructura</p>
                </div>
                <button onClick={closeForm} className="p-3 bg-muted/50 rounded-2xl text-muted-foreground hover:text-foreground transition-all"><X size={24} /></button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-8">
                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Nombre del Galpón</label>
                  <input {...register('nombre')} className="w-full px-6 py-4 bg-muted/50 border border-border rounded-2xl text-foreground text-lg font-bold focus:outline-none focus:ring-2 focus:ring-emerald-500/50 transition-all placeholder:text-slate-700" placeholder="Ej. Galpón Norte 01" />
                  {errors.nombre && <p className="text-xs text-red-400 font-bold ml-1">{errors.nombre.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Capacidad Máxima (Aves)</label>
                  <div className="relative">
                    <Users className="absolute left-6 top-1/2 -translate-y-1/2 text-muted-foreground" size={20} />
                    <input type="number" {...register('capacidad', { valueAsNumber: true })} className="w-full pl-14 pr-6 py-4 bg-muted/50 border border-border rounded-2xl text-foreground text-lg font-bold focus:outline-none focus:ring-2 focus:ring-emerald-500/50 transition-all" />
                  </div>
                  {errors.capacidad && <p className="text-xs text-red-400 font-bold ml-1">{errors.capacidad.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Ubicación Geográfica / Sector</label>
                  <div className="relative">
                    <MapPin className="absolute left-6 top-1/2 -translate-y-1/2 text-muted-foreground" size={20} />
                    <input {...register('ubicacion')} className="w-full pl-14 pr-6 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-bold focus:outline-none focus:ring-2 focus:ring-emerald-500/50 transition-all" placeholder="Ej. Km 12 - Lado Este" />
                  </div>
                  {errors.ubicacion && <p className="text-xs text-red-400 font-bold ml-1">{errors.ubicacion.message}</p>}
                </div>

                <div className="pt-8">
                  <button type="submit" disabled={isPending} className="w-full py-5 bg-emerald-500 hover:bg-emerald-400 text-black font-black rounded-3xl transition-all flex items-center justify-center gap-3 disabled:opacity-50 shadow-xl shadow-emerald-500/20 active:scale-95" >
                    <Save size={24} />
                    {editingGalponId ? 'GUARDAR CAMBIOS' : 'REGISTRAR GALPÓN'}
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


