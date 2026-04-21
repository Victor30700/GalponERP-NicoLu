'use client'

import { useState } from 'react'
import { UniversalGrid, Column } from '@/components/shared/UniversalGrid'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import * as z from 'zod'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, Building2, Phone, Mail, MapPin, Truck } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'
import { UserRole } from '@/lib/rbac'
import { toast } from 'sonner'
import { confirmDestructiveAction } from '@/lib/swal'

import { useProveedores, type Proveedor } from '@/hooks/useProveedores'

const proveedorSchema = z.object({
  razonSocial: z.string().min(3, 'La razón social es muy corta'),
  nitRuc: z.string().min(3, 'El NIT/RUC es muy corto'),
  email: z.string().email('Email inválido').optional().or(z.literal('')),
  telefono: z.string().min(7, 'Teléfono inválido').optional().or(z.literal('')),
  direccion: z.string().optional(),
  version: z.string().optional(),
})

type ProveedorFormValues = z.infer<typeof proveedorSchema>

export default function ProveedoresPage() {
  const { profile } = useAuth()
  const userRole = profile?.rol !== undefined ? Number(profile.rol) : null
  const isEmpleado = userRole === UserRole.Empleado

  const [isFormOpen, setIsFormOpen] = useState(false)
  const [editingProveedorId, setEditingProveedorId] = useState<string | null>(null)

  const {
    proveedores,
    isLoading,
    createProveedor,
    updateProveedor,
    deleteProveedor
  } = useProveedores()

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ProveedorFormValues>({
    resolver: zodResolver(proveedorSchema),
  })

  const onSubmit = (data: ProveedorFormValues) => {
    if (editingProveedorId) {
      updateProveedor.mutate({ id: editingProveedorId, ...data }, {
        onSuccess: () => {
          toast.success('Proveedor actualizado')
          closeForm()
        },
        onError: (err: any) => {
          if (err.message !== 'CONCURRENCY_ERROR') {
            toast.error(err.message)
          }
        }
      })
    } else {
      createProveedor.mutate(data, {
        onSuccess: () => {
          toast.success('Proveedor creado con éxito')
          closeForm()
        },
        onError: (err: any) => toast.error(err.message)
      })
    }
  }

  const openForm = (prov?: Proveedor) => {
    if (prov) {
      setEditingProveedorId(prov.id)
      reset({
          razonSocial: prov.razonSocial,
          nitRuc: prov.nitRuc,
          email: prov.email || '',
          telefono: prov.telefono || '',
          direccion: prov.direccion || '',
          version: prov.version || ''
      })
    } else {
      setEditingProveedorId(null)
      reset({ razonSocial: '', nitRuc: '', email: '', telefono: '', direccion: '', version: '' })
    }
    setIsFormOpen(true)
  }

  const closeForm = () => {
    setIsFormOpen(false)
    setEditingProveedorId(null)
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
            <p className="font-medium text-foreground">{item.razonSocial}</p>
            <p className="text-xs text-muted-foreground uppercase tracking-widest">{item.nitRuc}</p>
          </div>
        </div>
      )
    },
    { header: 'Teléfono', accessor: (item) => item.telefono || 'N/A' },
    { header: 'Email', accessor: (item) => item.email || 'N/A' },
  ]

  return (
    <div className="relative">
      <UniversalGrid
        title="Proveedores"
        items={proveedores}
        columns={columns}
        isLoading={isLoading}
        onAdd={isEmpleado ? undefined : () => openForm()}
        onEdit={isEmpleado ? undefined : (item) => openForm(item as Proveedor)}
        onDelete={isEmpleado ? undefined : async (item) => {
          const result = await confirmDestructiveAction(
            '¿Eliminar proveedor?',
            `¿Estás seguro de que deseas eliminar a ${(item as Proveedor).razonSocial}?`
          )
          if (result.isConfirmed) {
            deleteProveedor.mutate((item as Proveedor).id, {
              onSuccess: () => toast.success('Proveedor eliminado'),
              onError: (err: any) => toast.error(err.message)
            })
          }
        }}
        renderMobileCard={(item) => (
          <div className="space-y-1">
            <h3 className="text-lg font-bold text-foreground">{item.razonSocial}</h3>
            <p className="text-xs text-amber-500 font-bold uppercase tracking-widest">{item.nitRuc}</p>
            <div className="flex items-center gap-2 text-muted-foreground text-sm mt-2">
              <Phone size={14} /> {item.telefono || 'Sin teléfono'}
            </div>
            <div className="flex items-center gap-2 text-muted-foreground text-sm">
              <Mail size={14} /> {item.email || 'Sin correo'}
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
                  {editingProveedorId ? 'Editar Proveedor' : 'Nuevo Proveedor'}
                </h2>
                <button onClick={closeForm} className="p-2 bg-muted/50 rounded-full text-muted-foreground">
                  <X size={20} />
                </button>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Razón Social</label>
                  <div className="relative">
                    <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input
                      {...register('razonSocial')}
                      className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                      placeholder="Ej. Distribuidora Avícola S.A."
                    />
                  </div>
                  {errors.razonSocial && <p className="text-xs text-red-400 ml-1">{errors.razonSocial.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">NIT / RUC</label>
                  <input
                    {...register('nitRuc')}
                    className="w-full px-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                    placeholder="Ej. 123456789-0"
                  />
                  {errors.nitRuc && <p className="text-xs text-red-400 ml-1">{errors.nitRuc.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Correo Electrónico</label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input
                      {...register('email')}
                      type="email"
                      className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                      placeholder="prov@ejemplo.com"
                    />
                  </div>
                  {errors.email && <p className="text-xs text-red-400 ml-1">{errors.email.message}</p>}
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium text-muted-foreground ml-1">Teléfono</label>
                  <div className="relative">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                    <input
                      {...register('telefono')}
                      className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
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
                      className="w-full pl-10 pr-4 py-3 bg-muted/50 border border-border rounded-xl text-foreground focus:outline-none focus:ring-2 focus:ring-amber-500/50 transition-all"
                      placeholder="Dirección fiscal o de entrega..."
                    />
                  </div>
                </div>

                <button
                  type="submit"
                  disabled={createProveedor.isPending || updateProveedor.isPending}
                  className="w-full py-4 bg-amber-500 hover:bg-amber-600 text-black font-bold rounded-2xl transition-all flex items-center justify-center gap-2 disabled:opacity-50 mt-8 shadow-lg shadow-amber-500/20"
                >
                  <Save size={20} />
                  {editingProveedorId ? 'Guardar Cambios' : 'Crear Proveedor'}
                </button>
              </form>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  )
}
