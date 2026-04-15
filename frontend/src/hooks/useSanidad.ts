import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface BienestarRequest {
  loteId: string;
  fecha: string;
  temperatura: number;
  humedad: number;
  consumoAgua: number;
  observaciones: string;
}

export interface BienestarItem {
  id: string;
  loteId: string;
  fecha: string;
  temperatura: number | null;
  humedad: number | null;
  consumoAgua: number | null;
  observaciones: string | null;
}

export function useSanidad(loteId?: string) {
  const queryClient = useQueryClient();

  const historialBienestar = useQuery({
    queryKey: ['sanidad', 'bienestar', 'lote', loteId],
    queryFn: () => api.get<BienestarItem[]>(`/api/Sanidad/lote/${loteId}/bienestar`),
    enabled: !!loteId,
  });

  const registrarBienestar = useMutation({
    mutationFn: (data: BienestarRequest) => api.post<{ registroId: string }>('/api/Sanidad/bienestar', data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['sanidad'] });
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
      if (loteId) {
        queryClient.invalidateQueries({ queryKey: ['sanidad', 'bienestar', 'lote', loteId] });
      }
    },
  });

  return {
    historialBienestar: historialBienestar.data || [],
    isLoadingHistorial: historialBienestar.isLoading,
    registrarBienestar,
  };
}
