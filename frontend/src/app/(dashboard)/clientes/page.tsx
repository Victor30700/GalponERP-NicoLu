'use client'

import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from '@/lib/api'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, User, Phone, Mail, MapPin } from 'lucide-react'

// Schema de validación
const clienteSchema = z.object({
  nombre: z.string().min(3, 'El nombre es muy corto'),
  apellidos: z.string().min(3, 'Los apellidos son muy cortos'),
  email: z.string().email('Email inválido'),
  telefono: z.string().min(7, 'Teléfono inválido'),
  direccion: z.string().optional(),
})

type ClienteFormValues = z.infer<typeof clienteSchema>

interface Cliente extends ClienteFormValues {
  id: string
}

export default function ClientesPage() {
  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingCliente, setEditingCliente] = useState<Cliente | null>(null)
  const queryClient = useQueryClient()

  // Fetch Clientes
  const { data: clientes = [], isLoading } = useQuery({
    queryKey: ['clientes'],
    queryFn: () => api.get<Cliente[]>('/api/Clientes'),
  })

  // Mutations
  const createMutation = useMutation({
    mutationFn: (newCliente: ClienteFormValues) => api.post('/api/Clientes', newCliente),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clientes'] })
      closeForm()
    },
  })

  const updateMutation = useMutation({
    mutationFn: (data: { id: string; values: ClienteFormValues }) => 
      api.patch(`/api/Clientes/${data.id}`, data.values),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['clientes'] })
      closeForm()
    },
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Clientes/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['clientes'] }),
  })

  // Form handling
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ClienteFormValues>({
    resolver: zodResolver(clienteSchema),
  })

  const onSubmit = (data: ClienteFormValues) => {
    if (editingCliente) {
      updateMutation.mutate({ id: editingCliente.id, values: data })
    } else {
      createMutation.mutate(data)
    }
  }

  const openForm = (cliente?: Cliente) => {
    if (cliente) {
      setEditingCliente(cliente)
      reset(cliente)
    } else {
      setEditingCliente(null)
      reset({ nombre: '', apellidos: '', email: '', telefono: '', direccion: '' })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingCliente(null)
    reset()
  }

  const columns = [
    { 
      header: 'Cliente', 
      accessor: (item: Cliente) => (
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-primary font-bold text-xs">
            {item.nombre.charAt(0)}{item.apellidos.charAt(0)}
          </div>
          <div>
            <p className="font-medium text-white">{item.nombre} {item.apellidos}</p>
            <p className="text-xs text-slate-500">{item.email}</p>
          </div>
        </div>
      )
    },
    { header: 'Teléfono', accessor: 'telefono' },
    { header: 'Dirección', accessor: (item: Cliente) => item.direccion || 'No especificada' },
  ]

  return (
    <div className="relative">
      <UniversalGrid
        title="Clientes"
        items={clientes}
        columns={columns}
        isLoading={isLoading}
        onAdd={() => openForm()}
        onEdit={(item) => openForm(item)}
        onDelete={(item) => {
          if (confirm('¿Estás seguro de eliminar este cliente?')) {
            deleteMutation.mutate(item.id)
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-1">
            <h3 className="text-lg font-bold text-white">{item.nombre} {item.apellidos}</h3>
            <div className="flex items-center gap-2 text-slate-400 text-sm">
              <Phone size={14} /> {item.telefono}
            </div>
            <div className="flex items-center gap-2 text-slate-400 text-sm">
              <Mail size={14} /> {item.email}
            </div>
          </div>
        )}
      />

      {/* Slide-over Form (Mobile-friendly) */}
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
                  {editingCliente ? 'Editar Cliente' : 'Nuevo Cliente'}
                </h2>
                <button onClick={closeForm} className="p-2 bg-white/5 rounded-full text-slate-400">
                  <X size={20} />
                </button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="grid grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-400 ml-1">Nombre</label>
                    <div className="relative">
                      <User className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                      <input
                        {...register('nombre')}
                        className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                        placeholder="Ej. Juan"
                      />
                    </div>
                    {errors.nombre && <p className="text-xs text-red-400 ml-1">{errors.nombre.message}</p>}
                  </div>
                  <div className="space-y-2">
                    <label className="text-sm font-medium text-slate-400 ml-1">Apellidos</label>
                    <input
                      {...register('apellidos')}
                      className="w-full px-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                      placeholder="Ej. Pérez"
                    />
                    {errors.apellidos && <p className="text-xs text-red-400 ml-1">{errors.apellidos.message}</p>}
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-slate-400 ml-1">Correo Electrónico</label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
                    <input
                      {...register('email')}
                      type="email"
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                      placeholder="juan@ejemplo.com"
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
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
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
                      className="w-full pl-10 pr-4 py-3 bg-white/5 border border-white/10 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                      placeholder="Ubicación del cliente..."
                    />
                  </div>
                </div>

                <button
                  type="submit"
                  disabled={createMutation.isPending || updateMutation.isPending}
                  className="w-full py-4 bg-primary hover:bg-primary/90 text-primary-foreground font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-8 shadow-lg shadow-primary/20"
                >
                  <Save size={20} />
                  {editingCliente ? 'Guardar Cambios' : 'Crear Cliente'}
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
