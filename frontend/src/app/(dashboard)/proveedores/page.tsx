'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid, Column } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, Building2, Phone, Mail, MapPin, Truck } from 'lucide-react'

const proveedorSchema = z.object({
  nombre: z.string().min(3, 'El nombre es muy corto'),
  contacto: z.string().min(3, 'El contacto es muy corto'),
  email: z.string().email('Email inválido'),
  telefono: z.string().min(7, 'Teléfono inválido'),
  direccion: z.string().optional(),
})

type ProveedorFormValues = z.infer<typeof proveedorSchema>

interface Proveedor extends ProveedorFormValues {
  id: string
}

export default function ProveedoresPage() {
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingProveedor, setEditingProveedor] = useState<Proveedor | null>(null)
  const queryClient = useQueryClient()

  const { data: proveedores = [], isLoading } = useQuery({
    queryKey: ['proveedores'],
    queryFn: () => api.get<Proveedor[]>('/api/Proveedores'),
  })

  const createMutation = useMutation({
    mutationFn: (newProv: ProveedorFormValues) => api.post('/api/Proveedores', newProv),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['proveedores'] })
      closeForm()
    },
  })

  const updateMutation = useMutation({
    mutationFn: (data: { id: string; values: ProveedorFormValues }) => 
      api.patch(`/api/Proveedores/${data.id}`, data.values),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['proveedores'] })
      closeForm()
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Proveedores/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['proveedores'] }),
  })

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ProveedorFormValues>({
    resolver: zodResolver(proveedorSchema),
  })

  const onSubmit = (data: ProveedorFormValues) => {
    if (editingProveedor) {
      updateMutation.mutate({ id: editingProveedor.id, values: data })
    } else {
      createMutation.mutate(data)
    }
  }

  const openForm = (prov?: Proveedor) => {
    if (prov) {
      setEditingProveedor(prov)
      reset(prov)
    } else {
      setEditingProveedor(null)
      reset({ nombre: '', contacto: '', email: '', telefono: '', direccion: '' })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingProveedor(null)
    reset()
  }

  const columns: Column<Proveedor>[] = [
    { 
      header: 'Proveedor', 
      accessor: (item: Proveedor) => (
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-full bg-amber-500/10 flex items-center justify-center text-amber-500 font-bold text-xs">
            <Truck size={14} />
          </div>
          <div>
            <p className="font-medium text-white">{item.nombre}</p>
            <p className="text-xs text-slate-500">{item.contacto}</p>
          </div>
        </div>
      )
    },
    { header: 'Teléfono', accessor: 'telefono' },
    { header: 'Email', accessor: 'email' },
  ]

  return (
    <div className="relative">
      <UniversalGrid
        title="Proveedores"
        items={proveedores}
        columns={columns}
        isLoading={isLoading}
        onAdd={() => openForm()}
        onEdit={(item) => openForm(item)}
        onDelete={(item) => {
          if (confirm('¿Estás seguro de eliminar este proveedor?')) {
            deleteMutation.mutate(item.id)
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-1">
            <h3 className="text-lg font-bold text-white">{item.nombre}</h3>
            <p className="text-sm text-amber-500 font-medium">{item.contacto}</p>
            <div className="flex items-center gap-2 text-slate-400 text-sm mt-2">
              <Phone size={14} /> {item.telefono}
            </div>
            <div className="flex items-center gap-2 text-slate-400 text-sm">
              <Mail size={14} /> {item.email}
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
                  {editingProveedor ? 'Editar Proveedor' : 'Nuevo Proveedor'}
                </h2>
                <button onClick={closeForm} className="p-2 bg-white/5 rounded-full text-slate-400">
                  <X size={20} />
                </button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Razón Social / Nombre</label>
                  <div className="relative">
                    <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input
                      {...register('nombre')}
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                      placeholder="Ej. Distribuidora Avícola"
                    />
                  </div>
                  {errors.nombre && <p className="text-xs text-red-400 ml-1">{errors.nombre.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Persona de Contacto</label>
                  <input
                    {...register('contacto')}
                    className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                    placeholder="Ej. Carlos Ruiz"
                  />
                  {errors.contacto && <p className="text-xs text-red-400 ml-1">{errors.contacto.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Correo Electrónico</label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input
                      {...register('email')}
                      type="email"
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                      placeholder="prov@ejemplo.com"
                    />
                  </div>
                  {errors.email && <p className="text-xs text-red-400 ml-1">{errors.email.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Teléfono</label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input
                      {...register('telefono')}
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                      placeholder="+591 ..."
                    />
                  </div>
                  {errors.telefono && <p className="text-xs text-red-400 ml-1">{errors.telefono.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Dirección (Opcional)</label>
                  <div className="relative">
                    <MapPin className="absolute left-3 top-3 text-slate-500" size={18} />
                    <textarea
                      {...register('direccion')}
                      rows={3}
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                      placeholder="Dirección fiscal o de entrega..."
                    />
                  </div>
                </div>

                <button
                  type="submit"
                  disabled={createMutation.isPending || updateMutation.isPending}
                  className="w-full py-4 bg-amber-500 hover:bg-amber-600 text-black font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-8 shadow-lg shadow-amber-500/20"
                >
                  <Save size={20} />
                  {editingProveedor ? 'Guardar Cambios' : 'Crear Proveedor'}
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
