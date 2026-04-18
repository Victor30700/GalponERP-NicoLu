'use client'

import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { X, Save, User, Mail, Shield, Lock, Phone, MapPin, Briefcase, Calendar, CheckCircle2, XCircle } from 'lucide-react'
import { useUsuarios, UserRole, CreateUsuarioRequest } from '@/hooks/useUsuarios'
import { useSwal } from '@/hooks/useSwal'

interface UsuarioFormModalProps {
  isOpen: boolean
  onClose: () => void
  usuario?: any // If provided, we are editing
}

export function UsuarioFormModal({ isOpen, onClose, usuario }: UsuarioFormModalProps) {
  const { crearUsuario, actualizarUsuario } = useUsuarios()
  const swal = useSwal()
  const isEditing = !!usuario

  const [formData, setFormData] = useState<CreateUsuarioRequest>({
    nombre: '',
    apellidos: '',
    email: '',
    password: '',
    rol: UserRole.Empleado,
    telefono: '',
    direccion: '',
    profesion: '',
    fechaNacimiento: '',
    active: 1
  })

  useEffect(() => {
    if (usuario) {
      setFormData({
        nombre: usuario.nombre || '',
        apellidos: usuario.apellidos || '',
        email: usuario.email || '',
        password: '', // Password is not returned from API
        rol: usuario.rol ?? UserRole.Empleado,
        telefono: usuario.telefono || '',
        direccion: usuario.direccion || '',
        profesion: usuario.profesion || '',
        fechaNacimiento: usuario.fechaNacimiento ? usuario.fechaNacimiento.split('T')[0] : '',
        active: usuario.active ?? 1
      })
    } else {
      setFormData({
        nombre: '',
        apellidos: '',
        email: '',
        password: '',
        rol: UserRole.Empleado,
        telefono: '',
        direccion: '',
        profesion: '',
        fechaNacimiento: '',
        active: 1
      })
    }
  }, [usuario, isOpen])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    
    if (!formData.nombre || !formData.email || (!isEditing && !formData.password)) {
      swal.toast('Por favor complete los campos obligatorios', 'error')
      return
    }

    try {
      const payload = {
        ...formData,
        apellidos: formData.apellidos || "",
        direccion: formData.direccion || "",
        profesion: formData.profesion || "",
        telefono: formData.telefono || "",
        active: formData.active,
        // Aseguramos formato ISO para la fecha
        fechaNacimiento: formData.fechaNacimiento ? new Date(formData.fechaNacimiento).toISOString() : new Date().toISOString()
      }

      if (isEditing) {
        await actualizarUsuario.mutateAsync({
          id: usuario.id,
          ...payload,
          email: formData.email, // Por si acaso el backend no permite cambiar email o lo requiere
        })
        swal.toast('Usuario actualizado correctamente', 'success')
      } else {
        await crearUsuario.mutateAsync(payload)
        swal.toast('Usuario creado correctamente', 'success')
      }
      onClose()
    } catch (error: any) {
      swal.toast(error.message || 'Error al procesar la solicitud', 'error')
    }
  }

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          <motion.div 
            initial={{ opacity: 0 }} 
            animate={{ opacity: 1 }} 
            exit={{ opacity: 0 }} 
            onClick={onClose} 
            className="fixed inset-0 bg-black/80 backdrop-blur-md z-[100]" 
          />
          <motion.div 
            initial={{ opacity: 0, scale: 0.95, y: 20 }} 
            animate={{ opacity: 1, scale: 1, y: 0 }} 
            exit={{ opacity: 0, scale: 0.95, y: 20 }} 
            className="fixed inset-0 m-auto w-full max-w-2xl h-fit glass z-[110] p-8 rounded-[2.5rem] border border-border shadow-2xl max-h-[90vh] overflow-y-auto"
          >
            <div className="flex items-center justify-between mb-8">
              <div className="flex items-center gap-4">
                <div className="p-3 rounded-2xl bg-primary/10 text-primary">
                  <User size={24} />
                </div>
                <h2 className="text-2xl font-black text-foreground uppercase">
                  {isEditing ? 'Editar Usuario' : 'Nuevo Usuario'}
                </h2>
              </div>
              <button onClick={onClose} className="p-2 bg-muted/50 rounded-full text-muted-foreground hover:bg-muted/80 transition-colors">
                <X size={24} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Nombre *</label>
                  <div className="relative">
                    <User size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" />
                    <input
                      type="text"
                      required
                      value={formData.nombre}
                      onChange={(e) => setFormData({ ...formData, nombre: e.target.value })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Apellidos</label>
                  <div className="relative">
                    <User size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" />
                    <input
                      type="text"
                      value={formData.apellidos}
                      onChange={(e) => setFormData({ ...formData, apellidos: e.target.value })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Email *</label>
                  <div className="relative">
                    <Mail size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" />
                    <input
                      type="email"
                      required
                      value={formData.email}
                      onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Password {!isEditing && '*'}</label>
                  <div className="relative">
                    <Lock size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" />
                    <input
                      type="password"
                      required={!isEditing}
                      placeholder={isEditing ? "Dejar en blanco para mantener" : ""}
                      value={formData.password}
                      onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Rol *</label>
                  <div className="relative">
                    <Shield size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground z-10" />
                    <select
                      value={formData.rol}
                      onChange={(e) => setFormData({ ...formData, rol: Number(e.target.value) })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all appearance-none"
                    >
                      <option value={UserRole.Empleado} className="bg-background">Empleado</option>
                      <option value={UserRole.SubAdmin} className="bg-background">Sub-Admin</option>
                      <option value={UserRole.Admin} className="bg-background">Administrador</option>
                    </select>
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Teléfono</label>
                  <div className="relative">
                    <Phone size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" />
                    <input
                      type="text"
                      value={formData.telefono}
                      onChange={(e) => setFormData({ ...formData, telefono: e.target.value })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Dirección</label>
                  <div className="relative">
                    <MapPin size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" />
                    <input
                      type="text"
                      value={formData.direccion}
                      onChange={(e) => setFormData({ ...formData, direccion: e.target.value })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Profesión</label>
                  <div className="relative">
                    <Briefcase size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" />
                    <input
                      type="text"
                      value={formData.profesion}
                      onChange={(e) => setFormData({ ...formData, profesion: e.target.value })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-2 md:col-span-2">
                  <label className="text-xs font-black text-muted-foreground uppercase tracking-widest ml-1">Fecha de Nacimiento</label>
                  <div className="relative">
                    <Calendar size={18} className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" />
                    <input
                      type="date"
                      value={formData.fechaNacimiento}
                      onChange={(e) => setFormData({ ...formData, fechaNacimiento: e.target.value })}
                      className="w-full pl-12 pr-4 py-4 bg-muted/50 border border-border rounded-2xl text-foreground font-medium focus:outline-none focus:ring-2 focus:ring-primary/50 transition-all"
                    />
                  </div>
                </div>

                <div className="space-y-4 md:col-span-2 p-6 bg-muted/30 rounded-[2rem] border border-border/50">
                  <div className="flex items-center justify-between">
                    <div className="space-y-1">
                      <label className="text-xs font-black text-muted-foreground uppercase tracking-widest">Estado del Usuario</label>
                      <p className="text-sm text-muted-foreground font-medium">
                        {formData.active === 1 
                          ? 'El usuario puede acceder al sistema' 
                          : 'El acceso al sistema está revocado'}
                      </p>
                    </div>
                    <button
                      type="button"
                      onClick={() => setFormData({ ...formData, active: formData.active === 1 ? 0 : 1 })}
                      className={`flex items-center gap-3 px-6 py-3 rounded-2xl font-bold transition-all ${
                        formData.active === 1 
                          ? 'bg-emerald-500/10 text-emerald-500 border border-emerald-500/20' 
                          : 'bg-rose-500/10 text-rose-500 border border-rose-500/20'
                      }`}
                    >
                      {formData.active === 1 ? (
                        <>
                          <CheckCircle2 size={20} />
                          <span>ACTIVO</span>
                        </>
                      ) : (
                        <>
                          <XCircle size={20} />
                          <span>INACTIVO</span>
                        </>
                      )}
                    </button>
                  </div>
                </div>
              </div>


              <div className="pt-4">
                <button
                  type="submit"
                  disabled={crearUsuario.isPending || actualizarUsuario.isPending}
                  className="w-full py-5 bg-primary hover:bg-primary/90 text-black font-black rounded-3xl transition-all flex items-center justify-center gap-3 disabled:opacity-50 shadow-xl shadow-primary/20 active:scale-95"
                >
                  <Save size={24} />
                  {isEditing ? 'ACTUALIZAR USUARIO' : 'CREAR USUARIO'}
                </button>
              </div>
            </form>
          </motion.div>
        </>
      )}
    </AnimatePresence>
  )
}

