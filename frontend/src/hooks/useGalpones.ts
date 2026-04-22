import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface Galpon {
  id: string;
  nombre: string;
  capacidad: number;
  ubicacion: string;
  isActive: boolean;
}

export interface GalponRequest {
  nombre: string;
  capacidad: number;
  ubicacion: string;
}

export function useGalpones() {
  const queryClient = useQueryClient();

  const galpones = useQuery({
    queryKey: ['galpones'],
    queryFn: () => api.get<Galpon[]>('/api/Galpones'),
  });

  const crearGalpon = useMutation({
    mutationFn: (data: GalponRequest) => api.post('/api/Galpones', data),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['galpones'] }),
  });

  return {
    galpones: galpones.data || [],
    isLoading: galpones.isLoading,
    crearGalpon,
    refresh: () => galpones.refetch(),
  };
}

export function useGalpon(id: string) {
  const queryClient = useQueryClient();

  const galpon = useQuery({
    queryKey: ['galpones', id],
    queryFn: () => api.get<Galpon>(`/api/Galpones/${id}`),
    enabled: !!id,
  });

  const actualizarGalpon = useMutation({
    mutationFn: (data: Galpon & { id: string }) => api.put(`/api/Galpones/${id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['galpones'] });
      queryClient.invalidateQueries({ queryKey: ['galpones', id] });
    },
  });

  const eliminarGalpon = useMutation({
    mutationFn: () => api.delete(`/api/Galpones/${id}`),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['galpones'] }),
  });

  return {
    galpon: galpon.data,
    isLoading: galpon.isLoading,
    actualizarGalpon,
    eliminarGalpon,
  };
}
