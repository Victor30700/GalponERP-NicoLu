"use client";

import { useAuth } from "@/context/AuthContext";
import { useUsuarios, UserRole } from "@/hooks/useUsuarios";
import { useState, useEffect } from "react";
import { motion } from "framer-motion";
import { 
  Settings, User, Mail, Phone, MapPin, 
  Briefcase, Calendar, Save, MessageCircle, Trash2
} from "lucide-react";
import { useSwal } from "@/hooks/useSwal";

export default function ConfiguracionPage() {
  const { profile } = useAuth();
  const { me, isLoadingMe, actualizarUsuario, generarCodigoWhatsapp, eliminarWhatsapp } = useUsuarios();
  const swal = useSwal();
  
  const [formData, setFormData] = useState({
    nombre: "",
    apellidos: "",
    email: "",
    telefono: "",
    direccion: "",
    profesion: "",
    fechaNacimiento: "",
  });

  useEffect(() => {
    if (me) {
      setFormData({
        nombre: me.nombre || "",
        apellidos: me.apellidos || "",
        email: me.email || "",
        telefono: me.telefono || "",
        direccion: me.direccion || "",
        profesion: me.profesion || "",
        fechaNacimiento: me.fechaNacimiento ? new Date(me.fechaNacimiento).toISOString().split('T')[0] : "",
      });
    }
  }, [me]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!me) return;

    try {
      await actualizarUsuario.mutateAsync({
        id: me.id,
        email: me.email,
        rol: me.rol,
        nombre: formData.nombre,
        apellidos: formData.apellidos || "",
        fechaNacimiento: formData.fechaNacimiento ? new Date(formData.fechaNacimiento).toISOString() : new Date().toISOString(),
        direccion: formData.direccion || "",
        profesion: formData.profesion || "",
        telefono: formData.telefono || "",
        active: me.active
      });
      swal.toast("Perfil actualizado con éxito", "success");
    } catch (error: any) {
      swal.toast(error.message || "Error al actualizar perfil", "error");
    }
  };

  const handleWhatsappCode = async () => {
    try {
      const result = await generarCodigoWhatsapp.mutateAsync();
      swal.alert({
        title: "Código de Verificación",
        text: `Tu código para WhatsApp es: ${result.codigo}. Úsalo en el bot de la empresa.`,
        icon: "info"
      });
    } catch (error: any) {
      swal.toast(error.message || "Error al generar código", "error");
    }
  };

  const handleRemoveWhatsapp = async () => {
    const confirm = await swal.confirm("¿Eliminar vinculación?", "Se eliminará la conexión con WhatsApp.");
    if (confirm) {
      try {
        await eliminarWhatsapp.mutateAsync();
        swal.toast("Vinculación eliminada", "success");
      } catch (error: any) {
        swal.toast(error.message || "Error al eliminar vinculación", "error");
      }
    }
  };

  if (isLoadingMe) {
    return <div className="flex items-center justify-center h-full">Cargando perfil...</div>;
  }

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <motion.h1 
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            className="text-3xl font-black text-foreground uppercase tracking-tight flex items-center gap-3"
          >
            <Settings className="text-primary" size={32} />
            Configuración de Perfil
          </motion.h1>
          <p className="text-muted-foreground mt-1 font-medium">Gestiona tu información personal y preferencias.</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Profile Card */}
        <motion.div 
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="lg:col-span-1 space-y-6"
        >
          <div className="glass p-8 rounded-[2.5rem] border border-border text-center">
            <div className="w-32 h-32 rounded-full bg-primary/20 flex items-center justify-center mx-auto mb-6 border-4 border-primary/10">
              <User size={64} className="text-primary" />
            </div>
            <h2 className="text-2xl font-black text-foreground uppercase">{me?.nombre} {me?.apellidos}</h2>
            <p className="text-primary font-bold uppercase tracking-widest text-xs mt-1">
              {me?.rol === UserRole.Admin ? "Administrador" : me?.rol === UserRole.SubAdmin ? "Sub-Administrador" : "Empleado"}
            </p>
            
            <div className="mt-8 space-y-4">
              <button 
                onClick={handleWhatsappCode}
                className="w-full flex items-center justify-center gap-3 px-6 py-4 bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 rounded-2xl font-black uppercase tracking-widest text-xs hover:bg-emerald-500/20 transition-all"
              >
                <MessageCircle size={18} />
                Vincular WhatsApp
              </button>
              
              <button 
                onClick={handleRemoveWhatsapp}
                className="w-full flex items-center justify-center gap-3 px-6 py-4 bg-red-500/10 text-red-400 border border-red-500/20 rounded-2xl font-black uppercase tracking-widest text-xs hover:bg-red-500/20 transition-all"
              >
                <Trash2 size={18} />
                Desvincular WhatsApp
              </button>
            </div>
          </div>
        </motion.div>

        {/* Edit Form */}
        <motion.div 
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
          className="lg:col-span-2"
        >
          <form onSubmit={handleSubmit} className="glass p-8 rounded-[2.5rem] border border-border space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div className="space-y-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Nombre</label>
                <div className="relative">
                  <User className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                  <input 
                    type="text" 
                    value={formData.nombre}
                    onChange={(e) => setFormData({...formData, nombre: e.target.value})}
                    className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Apellidos</label>
                <div className="relative">
                  <User className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                  <input 
                    type="text" 
                    value={formData.apellidos}
                    onChange={(e) => setFormData({...formData, apellidos: e.target.value})}
                    className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Correo Electrónico</label>
                <div className="relative">
                  <Mail className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                  <input 
                    type="email" 
                    disabled
                    value={formData.email}
                    className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold opacity-50 cursor-not-allowed"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Teléfono</label>
                <div className="relative">
                  <Phone className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                  <input 
                    type="text" 
                    value={formData.telefono}
                    onChange={(e) => setFormData({...formData, telefono: e.target.value})}
                    className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Profesión</label>
                <div className="relative">
                  <Briefcase className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                  <input 
                    type="text" 
                    value={formData.profesion}
                    onChange={(e) => setFormData({...formData, profesion: e.target.value})}
                    className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Fecha de Nacimiento</label>
                <div className="relative">
                  <Calendar className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                  <input 
                    type="date" 
                    value={formData.fechaNacimiento}
                    onChange={(e) => setFormData({...formData, fechaNacimiento: e.target.value})}
                    className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                  />
                </div>
              </div>

              <div className="space-y-2 md:col-span-2">
                <label className="text-[10px] font-black text-muted-foreground uppercase tracking-widest ml-1">Dirección</label>
                <div className="relative">
                  <MapPin className="absolute left-4 top-1/2 -translate-y-1/2 text-muted-foreground" size={18} />
                  <input 
                    type="text" 
                    value={formData.direccion}
                    onChange={(e) => setFormData({...formData, direccion: e.target.value})}
                    className="w-full bg-muted/50 border border-border rounded-2xl py-3 pl-12 pr-4 text-foreground font-bold focus:outline-none focus:border-primary/50 transition-colors"
                  />
                </div>
              </div>
            </div>

            <div className="pt-4 border-t border-border flex justify-end">
              <button 
                type="submit"
                disabled={actualizarUsuario.isPending}
                className="bg-primary text-primary-foreground font-black px-8 py-4 rounded-2xl uppercase tracking-widest flex items-center gap-2 hover:opacity-90 transition-opacity disabled:opacity-50"
              >
                <Save size={20} />
                {actualizarUsuario.isPending ? "Guardando..." : "Guardar Cambios"}
              </button>
            </div>
          </form>
        </motion.div>
      </div>
    </div>
  );
}


