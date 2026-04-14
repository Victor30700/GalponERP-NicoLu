import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface Proveedor {
  id: string;
  razonSocial: string;
  nitRuc: string;
  telefono: string;
  email: string;
  direccion: string;
  isActive: boolean;
}

export interface CreateProveedorRequest {
  razonSocial: string;
  nitRuc: string;
  telefono: string;
  email: string;
  direccion: string;
}

export interface UpdateProveedorRequest {
  id: string;
  razonSocial: string;
  nitRuc: string;
  telefono: string;
  email: string;
  direccion: string;
}

export function useProveedores() {
  const queryClient = useQueryClient();

  const proveedores = useQuery({
    queryKey: ['proveedores'],
    queryFn: () => api.get<Proveedor[]>('/api/Proveedores'),
  });

  const createProveedor = useMutation({
    mutationFn: (data: CreateProveedorRequest) => api.post<{ proveedorId: string }>('/api/Proveedores', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['proveedores'] }),
  });

  return {
    proveedores: proveedores.data || [],
    isLoading: proveedores.isLoading,
    error: proveedores.error,
    createProveedor,
    refresh: () => proveedores.refetch(),
  };
}

export function useProveedor(id: string) {
  const queryClient = useQueryClient();

  const proveedor = useQuery({
    queryKey: ['proveedores', id],
    queryFn: () => api.get<Proveedor>(`/api/Proveedores/${id}`),
    enabled: !!id,
  });

  const updateProveedor = useMutation({
    mutationFn: (data: UpdateProveedorRequest) => api.put(`/api/Proveedores/${id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['proveedores'] });
      queryClient.invalidateQueries({ queryKey: ['proveedores', id] });
    },
  });

  const deleteProveedor = useMutation({
    mutationFn: () => api.delete(`/api/Proveedores/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['proveedores'] });
    },
  });

  const historial = useQuery({
    queryKey: ['proveedores', id, 'historial'],
    queryFn: () => api.get<any[]>(`/api/Proveedores/${id}/historial`),
    enabled: !!id,
  });

  return {
    proveedor: proveedor.data,
    isLoading: proveedor.isLoading,
    error: proveedor.error,
    updateProveedor,
    deleteProveedor,
    historial: historial.data || [],
    isLoadingHistorial: historial.isLoading,
  };
}
