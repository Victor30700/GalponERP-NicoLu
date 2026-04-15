import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface MortalidadItem {
  id: string;
  loteId: string;
  fecha: string;
  cantidadBajas: number;
  causa: string;
}

export interface RegistrarMortalidadForm {
  loteId: string;
  cantidad: number;
  causa: string;
  fecha: string;
}

export interface ActualizarMortalidadForm {
  id: string;
  cantidad: number;
  causa: string;
  fecha: string;
}

export function useMortalidad(loteId?: string) {
  const queryClient = useQueryClient();

  const mortalidad = useQuery({
    queryKey: ['mortalidad', 'lote', loteId],
    queryFn: () => api.get<MortalidadItem[]>(`/api/Mortalidad/lote/${loteId}`),
    enabled: !!loteId,
  });

  const registrarMortalidad = useMutation({
    mutationFn: (data: RegistrarMortalidadForm) => api.post('/api/Mortalidad', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad', 'lote', loteId] });
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] });
    },
  });

  const actualizarMortalidad = useMutation({
    mutationFn: (data: ActualizarMortalidadForm) => api.put(`/api/Mortalidad/${data.id}`, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad', 'lote', loteId] });
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] });
    },
  });

  const eliminarMortalidad = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Mortalidad/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['mortalidad', 'lote', loteId] });
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] });
    },
  });

  return {
    mortalidad: mortalidad.data || [],
    isLoading: mortalidad.isLoading,
    registrarMortalidad,
    actualizarMortalidad,
    eliminarMortalidad,
  };
}
