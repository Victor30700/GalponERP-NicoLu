import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface Pesaje {
  id: string;
  loteId: string;
  fecha: string;
  pesoPromedioGramos: number;
  cantidadMuestreada: number;
  usuarioId: string;
}

export interface RegistrarPesajeForm {
  loteId: string;
  fecha: string;
  pesoPromedioGramos: number;
  cantidadMuestreada: number;
}

export function usePesajes(loteId?: string) {
  const queryClient = useQueryClient();

  const pesajes = useQuery({
    queryKey: ['pesajes', 'lote', loteId],
    queryFn: () => api.get<Pesaje[]>(`/api/Pesajes/lote/${loteId}`),
    enabled: !!loteId,
  });

  const registrarPesaje = useMutation({
    mutationFn: (data: RegistrarPesajeForm) => api.post('/api/Pesajes', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pesajes', 'lote', loteId] });
      queryClient.invalidateQueries({ queryKey: ['lote', loteId] }); // Para actualizar KPIs si los hay
    },
  });

  const eliminarPesaje = useMutation({
    mutationFn: (id: string) => api.delete(`/api/Pesajes/${id}`),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['pesajes', 'lote', loteId] });
    },
  });

  return {
    pesajes: pesajes.data || [],
    isLoading: pesajes.isLoading,
    registrarPesaje,
    eliminarPesaje,
  };
}
