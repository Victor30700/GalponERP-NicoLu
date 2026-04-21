'use client'

import { useState } from 'react'
import { useClientes, useCliente, Cliente } from '@/hooks/useClientes'
import { UniversalGrid } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, User, Phone, Mail, MapPin } from 'lucide-react'
import { toast } from 'sonner'
import { confirmDestructiveAction } from '@/lib/swal'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/lib/rbac'

// Schema de validación
const clienteSchema = z.object({
  nombre: z.string().min(3, 'El nombre es muy corto'),
  ruc: z.string().min(3, 'RUC/NIT inválido'),
  email: z.string().email('Email inválido').optional().or(z.literal('')),
  telefono: z.string().min(7, 'Teléfono inválido').optional().or(z.literal('')),
  direccion: z.string().optional(),
  version: z.string().optional(),
})

type ClienteFormValues = z.infer<typeof clienteSchema>

export default function ClientesPage() {
  const { profile } = useAuth()
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null
  const isEmpleado = userRole === UserRole.Empleado

  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingClienteId, setEditingClienteId] = useState<string | null>(null)
  
  const { clientes, isLoading, crearCliente } = useClientes()
  const { actualizarCliente, eliminarCliente } = useCliente(editingClienteId || '')

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
    if (editingClienteId) {
      actualizarCliente.mutate({ ...data, id: editingClienteId } as any, {
        onSuccess: () => {
          toast.success('Cliente actualizado con éxito')
          closeForm()
        },
        onError: (err: any) => {
          if (err.message !== 'CONCURRENCY_ERROR') {
            toast.error(err.message)
          }
        }
      })
    } else {
      const { version, ...createData } = data
      crearCliente.mutate(createData as any, {
        onSuccess: () => {
          toast.success('Cliente creado con éxito')
          closeForm()
        },
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const openForm = (cliente?: Cliente) => {
    if (cliente) {
      setEditingClienteId(cliente.id)
      reset({
        nombre: cliente.nombre,
        ruc: cliente.ruc || '',
        email: cliente.email || '',
        telefono: cliente.telefono || '',
        direccion: cliente.direccion || '',
        version: cliente.version || '',
      })
    } else {
      setEditingClienteId(null)
      reset({ nombre: '', ruc: '', email: '', telefono: '', direccion: '', version: '' })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingClienteId(null)
    reset()
  }

  const isPending = crearCliente.isPending || actualizarCliente.isPending

  const columns = [
    { 
      header: 'Cliente', 
      accessor: (item: Cliente) => (
        <div className="flex items-center gap-3">
          <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center text-primary font-bold text-xs">
            {item.nombre?.charAt(0).toUpperCase() || 'C'}
          </div>
          <div>
            <p className="font-medium text-foreground">{item.nombre}</p>
            <p className="text-[10px] text-muted-foreground uppercase tracking-widest">{item.ruc}</p>
          </div>
        </div>
      )
    },
    { header: 'Teléfono', accessor: (item: Cliente) => item.telefono || 'N/A' },
    { header: 'Dirección', accessor: (item: Cliente) => item.direccion || 'No especificada' },
  ]

  return (
    <div className="relative">
      <UniversalGrid
        title="Clientes"
        items={clientes}
        columns={columns}
        isLoading={isLoading}
        onAdd={isEmpleado ? undefined : () => openForm()}
        onEdit={isEmpleado ? undefined : (item) => openForm(item as Cliente)}
        onDelete={isEmpleado ? undefined : async (item) => {
          const result = await confirmDestructiveAction(
            '¿Eliminar cliente?',
            `¿Estás seguro de que deseas eliminar a ${(item as Cliente).nombre}? Esta acción no se puede deshacer.`
          );
          
          if (result.isConfirmed) {
            setEditingClienteId((item as Cliente).id);
            eliminarCliente.mutate(undefined, {
              onSuccess: () => {
                  toast.success('Cliente eliminado');
                  setEditingClienteId(null);
              },
              onError: (err: any) => toast.error(err.message)
            })
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-1">
            <h3 className="text-lg font-bold text-foreground">{item.nombre}</h3>
            <p className="text-xs text-primary font-bold uppercase tracking-widest mb-2">{item.ruc}</p>
            <div className="flex items-center gap-2 text-muted-foreground text-sm">
              <Phone size={14} /> {item.telefono || 'Sin teléfono'}
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
              className="fixed top-0 right-0 bottom-0 w-full max-w-md glass z-[70] shadow-2xl p-6 overflow-y-auto"
            >
              <div className="flex items-center justify-between mb-8">
                <h2 className="text-2xl font-bold text-foreground">
                  {editingClienteId ? 'Editar Cliente' : 'Nuevo Cliente'}
                </h2>
                <button onClick={closeForm} className="p-2 bg-muted/50 rounded-full text-muted-foreground">
                  <X size={20} />
                </button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Nombre / Razón Social</label>
                  <div className="relative">
                    <User className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input
                      {...register('nombre')}
                      className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                      placeholder="Ej. Inversiones Pollito"
                    />
                  </div>
                  {errors.nombre && <p className="text-xs text-red-400 ml-1">{errors.nombre.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">RUC / NIT</label>
                  <input
                    {...register('ruc')}
                    className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    placeholder="Ej. 12345678-9"
                  />
                  {errors.ruc && <p className="text-xs text-red-400 ml-1">{errors.ruc.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Teléfono</label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input
                      {...register('telefono')}
                      className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                      placeholder="+591 ..."
                    />
                  </div>
                  {errors.telefono && <p className="text-xs text-red-400 ml-1">{errors.telefono.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Dirección (Opcional)</label>
                  <div className="relative">
                    <MapPin className="absolute left-3 top-3 text-muted-foreground" size={18} />
                    <textarea
                      {...register('direccion')}
                      rows={3}
                      className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                      placeholder="Ubicación del cliente..."
                    />
                  </div>
                </div>

                <button
                  type="submit"
                  disabled={isPending}
                  className="w-full py-4 bg-primary hover:bg-primary/90 text-primary-foreground font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-8 shadow-lg shadow-primary/20"
                >
                  <Save size={20} />
                  {editingClienteId ? 'Guardar Cambios' : 'Crear Cliente'}
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
