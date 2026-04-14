import { useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '@/lib/api';

export interface BienestarRequest {
  loteId: string;
  fecha: string;
  temperatura: number;
  humedad: number;
  consumoAgua: number;
  observaciones: string;
}

export function useSanidad() {
  const queryClient = useQueryClient();

  const registrarBienestar = useMutation({
    mutationFn: (data: BienestarRequest) => api.post<{ registroId: string }>('/api/Sanidad/bienestar', data),
    onSuccess: () => {
      // Invalidate queries that might be affected
      queryClient.invalidateQueries({ queryKey: ['sanidad'] });
      queryClient.invalidateQueries({ queryKey: ['lotes'] });
    },
  });

  return {
    registrarBienestar,
  };
}
