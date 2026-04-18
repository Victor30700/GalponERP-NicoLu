"use client";

import { useAuth } from "@/context/AuthContext";
import { useUsuarios, UserRole, Usuario } from "@/hooks/useUsuarios";
import { useState } from "react";
import { motion } from "framer-motion";
import { 
  Users, UserPlus, Shield, Mail, CheckCircle, XCircle 
} from "lucide-react";
import { UniversalGrid } from "@/components/shared/UniversalGrid";
import { UsuarioFormModal } from "@/components/auth/UsuarioFormModal";
import { useSwal } from "@/hooks/useSwal";

export default function UsuariosPage() {
  const { profile } = useAuth();
  const { usuarios, isLoadingUsuarios, eliminarUsuario } = useUsuarios();
  
  // Filtro de seguridad adicional en el cliente
  const usuariosActivos = usuarios.filter(u => u.isActive !== false);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedUsuario, setSelectedUsuario] = useState<Usuario | undefined>(undefined);
  const swal = useSwal();

  // Role check: Only Admin (2) or SubAdmin (1)
  const canManage = profile?.rol !== undefined && (Number(profile.rol) === UserRole.Admin || Number(profile.rol) === UserRole.SubAdmin);

  if (!canManage) {
    return (
      <div className="flex flex-col items-center justify-center h-[60vh] text-center space-y-4">
        <div className="p-6 rounded-full bg-red-500/10 text-red-500">
          <Shield size={64} />
        </div>
        <h2 className="text-2xl font-black text-foreground uppercase">Acceso Denegado</h2>
        <p className="text-muted-foreground max-w-md">No tienes permisos suficientes para gestionar usuarios. Contacta con un administrador.</p>
      </div>
    );
  }

  const handleAdd = () => {
    setSelectedUsuario(undefined);
    setIsModalOpen(true);
  };

  const handleEdit = (usuario: Usuario) => {
    setSelectedUsuario(usuario);
    setIsModalOpen(true);
  };

  const handleDelete = async (usuario: Usuario) => {
    const confirm = await swal.confirm(
      "¿Eliminar usuario?",
      `Estás a punto de eliminar a ${usuario.nombre}. Esta acción no se puede deshacer.`
    );
    if (confirm) {
      try {
        await eliminarUsuario.mutateAsync(usuario.id);
        swal.toast("Usuario eliminado", "success");
      } catch (error: any) {
        swal.toast(error.message || "Error al eliminar usuario", "error");
      }
    }
  };

  const getRoleBadge = (rol: number) => {
    switch (rol) {
      case UserRole.Admin:
        return <span className="px-3 py-1 bg-red-500/20 text-red-400 rounded-full text-[10px] font-black uppercase tracking-widest border border-red-500/20">Admin</span>;
      case UserRole.SubAdmin:
        return <span className="px-3 py-1 bg-amber-500/20 text-amber-400 rounded-full text-[10px] font-black uppercase tracking-widest border border-amber-500/20">Sub-Admin</span>;
      default:
        return <span className="px-3 py-1 bg-blue-500/20 text-blue-400 rounded-full text-[10px] font-black uppercase tracking-widest border border-blue-500/20">Empleado</span>;
    }
  };

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <motion.h1 
            initial={{ opacity: 0, x: -20 }}
            animate={{ opacity: 1, x: 0 }}
            className="text-3xl font-black text-foreground uppercase tracking-tight flex items-center gap-3"
          >
            <Users className="text-primary" size={32} />
            Gestión de Usuarios
          </motion.h1>
          <p className="text-muted-foreground mt-1 font-medium">Administra los accesos y roles del personal.</p>
        </div>
      </div>

      <UniversalGrid
        title="Usuarios"
        items={usuariosActivos}
        isLoading={isLoadingUsuarios}
        onAdd={handleAdd}
        onEdit={handleEdit}
        onDelete={handleDelete}
        columns={[
          { 
            header: 'Usuario', 
            accessor: (item) => (
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-xl bg-primary/10 flex items-center justify-center text-primary font-black">
                  {item.nombre[0].toUpperCase()}
                </div>
                <div>
                  <p className="font-black text-foreground uppercase">{item.nombre} {item.apellidos}</p>
                  <p className="text-[10px] text-muted-foreground font-bold flex items-center gap-1">
                    <Mail size={10} /> {item.email}
                  </p>
                </div>
              </div>
            )
          },
          { 
            header: 'Rol', 
            accessor: (item) => getRoleBadge(item.rol)
          },
          { 
            header: 'Estado', 
            accessor: (item) => (
              <div className="flex items-center gap-2">
                {item.active === 1 ? (
                  <div className="flex items-center gap-1.5 px-3 py-1 bg-emerald-500/10 text-emerald-500 rounded-full border border-emerald-500/20">
                    <CheckCircle size={14} />
                    <span className="text-[10px] font-black uppercase">Habilitado</span>
                  </div>
                ) : (
                  <div className="flex items-center gap-1.5 px-3 py-1 bg-rose-500/10 text-rose-500 rounded-full border border-rose-500/20">
                    <XCircle size={14} />
                    <span className="text-[10px] font-black uppercase">Deshabilitado</span>
                  </div>
                )}
              </div>
            )
          }
        ]}
        renderMobileCard={(item) => (
          <div className="flex items-center gap-4">
             <div className="w-12 h-12 rounded-2xl bg-primary/10 flex items-center justify-center text-primary text-xl font-black">
                {item.nombre[0].toUpperCase()}
             </div>
             <div>
                <p className="font-black text-foreground uppercase">{item.nombre}</p>
                <div className="flex gap-2 mt-1">
                   {getRoleBadge(item.rol)}
                </div>
             </div>
          </div>
        )}
      />


      <UsuarioFormModal 
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        usuario={selectedUsuario}
      />
    </div>
  );
}

