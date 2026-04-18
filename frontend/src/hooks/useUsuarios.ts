import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export enum UserRole {
  Empleado = 0,
  SubAdmin = 1,
  Admin = 2
}

export interface Usuario {
  id: string;
  firebaseUid: string;
  email: string;
  nombre: string;
  apellidos: string;
  fechaNacimiento?: string;
  direccion?: string;
  profesion?: string;
  telefono?: string;
  rol: UserRole;
  isActive: boolean;
  active: number;
}

export interface CreateUsuarioRequest {
  email: string;
  password?: string;
  nombre: string;
  apellidos: string;
  fechaNacimiento: string;
  direccion: string;
  profesion: string;
  telefono: string;
  rol: UserRole;
  active: number;
}

export interface UpdateUsuarioRequest {
  id: string;
  email: string;
  nombre: string;
  apellidos: string;
  fechaNacimiento: string;
  direccion: string;
  profesion: string;
  telefono: string;
  rol: UserRole;
  active: number;
}

export function useUsuarios() {
  const queryClient = useQueryClient();

  const usuarios = useQuery({
    queryKey: ['usuarios'],
    queryFn: () => api.get<Usuario[]>('/api/Usuarios'),
  });

  const me = useQuery({
    queryKey: ['usuarios', 'me'],
    queryFn: () => api.get<Usuario>('/api/Usuarios/me'),
  });

  const crearUsuario = useMutation({
    mutationFn: (data: CreateUsuarioRequest) => api.post<Usuario>('/api/Usuarios', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['usuarios'] }),
  });

  const actualizarUsuario = useMutation({
    mutationFn: (data: UpdateUsuarioRequest) => api.put(`/api/Usuarios/${data.id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['usuarios'] });
      queryClient.invalidateQueries({ queryKey: ['usuarios', 'me'] });
    },
  });

  const eliminarUsuario = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Usuarios/${id}`),
    onSuccess: () => {
      // Invalidamos todas las consultas relacionadas con usuarios
      queryClient.invalidateQueries({ queryKey: ['usuarios'] });
      // Forzamos un refetch inmediato para asegurar que la lista esté limpia
      queryClient.refetchQueries({ queryKey: ['usuarios'] });
    },
  });

  const generarCodigoWhatsapp = useMutation({
    mutationFn: () => api.post<{ codigo: string }>('/api/Usuarios/me/whatsapp/code', {}),
  });

  const eliminarWhatsapp = useMutation({
    mutationFn: () => api.delete('/api/Usuarios/me/whatsapp'),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['usuarios', 'me'] }),
  });

  return {
    usuarios: usuarios.data || [],
    isLoadingUsuarios: usuarios.isLoading,
    me: me.data,
    isLoadingMe: me.isLoading,
    crearUsuario,
    actualizarUsuario,
    eliminarUsuario,
    generarCodigoWhatsapp,
    eliminarWhatsapp,
  };
}
